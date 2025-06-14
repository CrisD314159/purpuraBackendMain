namespace purpuraMain.Services.Implementations;
using purpuraMain.Model;
using purpuraMain.DbContext;
using FluentValidation;
using purpuraMain.Validations;
using FluentValidation.Results;
using purpuraMain.Dto.InputDto;
using purpuraMain.Utils;
using Microsoft.EntityFrameworkCore;
using purpuraMain.Dto.OutputDto;
using System.Data.Common;
using purpuraMain.Exceptions;
using System.Reflection.Metadata.Ecma335;
using purpuraMain.Services.Interfaces;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Sprache;

public class UserService (PurpuraDbContext dbContext, IValidator<CreateUserDTO> createuserValidator, IValidator<UpdateUserDto> updateUserValidator,
 IMapper mapper, UserManager<User> userManager, IValidator<PasswordChangeDTO> passwordChangeValidator
) : IUserService
{

    private readonly PurpuraDbContext _dbContext = dbContext;

    private readonly IValidator<CreateUserDTO> _createuserValidator = createuserValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator = updateUserValidator;
    private readonly IValidator<PasswordChangeDTO> _passwordChangeValidator = passwordChangeValidator;
    private readonly IMapper _mapper = mapper;
    private readonly UserManager<User> _userManager = userManager;
    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public async Task<GetUserDto> GetUserById(string id)
    {


        var user = await _dbContext.Users!.Where(u => u.State != UserState.INACTIVE && u.Id == id)
        .ProjectTo<GetUserDto>(_mapper.ConfigurationProvider)
        .FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User not found");

        return user;


    }

   /// <summary>
   /// Este metodo crea un usuario, su biblioteca y la playlist de recomendaciones por defecto
   /// </summary>
   /// <param name="user"></param>
   /// <param name="_dbContext"></param>
   /// <returns></returns>
    public async Task CreateUser(CreateUserDTO userDTO)
    {
        _createuserValidator.ValidateAndThrow(userDTO);

        // Generar el código de verificación
        int code = new Random().Next(1000, 9999);

        // Crear el usuario
        User newUser = new()
        {
            UserName = userDTO.Name.Trim(),
            Email = userDTO.Email,
            State = UserState.UNVERIFIED,
            CreatedAt = DateTime.UtcNow,
            ProfilePicture = $"https://api.dicebear.com/9.x/glass/svg?seed={userDTO.Name.Trim()}",
            VerificationCode = code.ToString()
        };

        var result = await _userManager.CreateAsync(newUser, userDTO.Password);

        if (!result.Succeeded)
            throw new InternalServerException("Error creating user");

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            // Crear library y playlist
            var playlist = new Playlist
            {
                Name = "Purple Day List",
                Description = "This is your Purple Day List, here you will find daily recommendations based on the music you most like",
                UserId = newUser.Id,
                User = newUser,
                CreatedAt = DateTime.UtcNow,
                IsPublic = false,
                Editable = false,
            };

            var library = new Library
            {
                UserId = newUser.Id,
                User = newUser,
                Playlists = [playlist]
            };

            await _dbContext.Libraries.AddAsync(library);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            await MailService.SendVerificationEmail(newUser.Email, code.ToString(), newUser.UserName);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    /// <summary>
    /// Este metodo actualiza la información de un usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="user"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task UpdateUser(string userId, UpdateUserDto updateUserDto)
    {

        _updateUserValidator.ValidateAndThrow(updateUserDto);
        var userToUpdate = await VerifyAndReturnValidUser(userId);

        userToUpdate.UserName = updateUserDto.Name.Trim();

        var result = await _userManager.UpdateAsync(userToUpdate);

        if (!result.Succeeded)
        {
            throw new InternalServerException("Cannot update user");
        }

   
    }


    /// <summary>
    /// Este metodo elimina un usuario de forma lógica
    /// </summary>
    /// <param name="id"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
        ?? throw new EntityNotFoundException("User not found");

        user.State = UserState.INACTIVE;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new InternalServerException("Cannot delete user");
        }
    }


    /// <summary>
    /// Este metodo actualiza la contraseña de un usuario
    /// </summary>
    /// <param name="passwordDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task UpdateUserPassword(PasswordChangeDTO passwordDTO)
    {

        _passwordChangeValidator.ValidateAndThrow(passwordDTO);

        var user = await _userManager.FindByEmailAsync(passwordDTO.Email)
        ?? throw new EntityNotFoundException("User not found");
        if (user.State == UserState.INACTIVE) throw new EntityNotFoundException("User not found");

        var result = await _userManager.ResetPasswordAsync(user, passwordDTO.Code, passwordDTO.Password);

        if (!result.Succeeded)
        {
            throw new InternalServerException("An error occurred while updating user's password");
        }

        await MailService.SendPasswordChangeMail(passwordDTO.Email, user.UserName ?? "User");

     
    }

    /// <summary>
    /// Este metodo verifica la cuenta de un usuario
    /// </summary>
    /// <param name="verifyAccountDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task VerifyAccount(VerifyAccountDTO verifyAccountDTO)
    {

   
        var user = await _userManager.FindByEmailAsync(verifyAccountDTO.Email)
        ?? throw new EntityNotFoundException("User not found");

        if (user.State != UserState.UNVERIFIED) throw new BadRequestException("User not found or already verified");
        if (user.VerificationCode != verifyAccountDTO.Code || user.VerificationCode == "0") throw new ValidationException("Invalid code");

        user.State = UserState.ACTIVE;
        user.VerificationCode = "0";
        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new InternalServerException("An error occurred while updating user's password");
        }
        await MailService.SendVerifiedAccountMail(verifyAccountDTO.Email, user.UserName ?? "User");

        
    }


    /// <summary>
    /// Este metodo envia un email con un codigo de verificación para recuperar la contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task SendPasswordRecoveryCode(string email)
    {
   
        var user = await _userManager.FindByEmailAsync(email)
        ?? throw new EntityNotFoundException("User not found");

        if (user.State == UserState.INACTIVE) throw new EntityNotFoundException("User not found");

        var recoveryCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            
        await MailService.SendPasswordRecoveryCodeEmail(email, recoveryCode, user.UserName ?? "User");
     
    }

    /// <summary>
    /// Returns a valid user using a provided user id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    /// <exception cref="UnauthorizedException"></exception>
    public async Task<User> VerifyAndReturnValidUser(string userId)
    {
        var userToVerify = await _userManager.FindByIdAsync(userId)
        ?? throw new EntityNotFoundException("User not found");

        if (userToVerify.State == UserState.INACTIVE) throw new EntityNotFoundException("User not found");

        if (userToVerify.EmailConfirmed || userToVerify.State == UserState.UNVERIFIED)
        {
            throw new UnauthorizedException("User not verified, check your email for code verification");
        }

        return userToVerify;

    }
}