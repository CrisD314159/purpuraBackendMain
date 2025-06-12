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

public class UserService (PurpuraDbContext dbContext, IValidator<CreateUserDTO> createuserValidator) : IUserService
{

    private readonly PurpuraDbContext _dbContext = dbContext;

    private readonly IValidator<CreateUserDTO> _createuserValidator = createuserValidator;
    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public async Task<GetUserDto> GetUserById(string id)
    {


        var user = await _dbContext.Users!.Where(u => u.State != UserState.INACTIVE && u.Id == id)
        .Select(u => new GetUserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            SurName = u.SurName,
            Email = u.Email,
            Country = u.Country!.Name,
            ProfilePicture = u.ProfilePicture,
            IsVerified = u.State == UserState.ACTIVE,
            CountryId = u.Country.Id
        }).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User not found");

        return user;


    }

   /// <summary>
   /// Este metodo crea un usuario, su biblioteca y la playlist de recomendaciones por defecto
   /// </summary>
   /// <param name="user"></param>
   /// <param name="_dbContext"></param>
   /// <returns></returns>
    public async Task<bool> CreateUser(CreateUserDTO user)
    {
        var transaction = await _dbContext.Database.BeginTransactionAsync();
            _createuserValidator.ValidateAndThrow(user);

            if (await _dbContext.Users!.AnyAsync(u => u.Email == user.Email)) throw new ValidationException("User already exists");


            string id = Guid.NewGuid().ToString();
            PasswordManipulation passwordManipulation = new();
            string hashedPassword = passwordManipulation.HashPassword(user.Password);

            // Generate a random code for the user verification
            int code = new Random().Next(1000, 9999);

            // Create a new user
            User newUser = new()
            {
                Id = id,
                FirstName = user.FirstName,
                Email = user.Email,
                Password = hashedPassword,
                SurName = user.SurName,
                CountryId = user.Country,
                State = UserState.UNVERIFIED,
                CreatedAt = DateTime.UtcNow,
                ProfilePicture = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1735614730/sd4xg3gxzsgtdiie0aht.jpg",
                VerifyCode = code
            };

            await _dbContext.Users!.AddAsync(newUser);
            

            // Create a new library and a playlist for the user
            string libraryId = Guid.NewGuid().ToString();
            var library = new Library
            {
                Id = Guid.NewGuid().ToString(),
                UserId = id
            };

            // Create a new playlist (Purpura Daylist) for the user
            var playlist = new Playlist
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Purple Day List",
                Description= "Esta es tu Purpura Day - List, aquí encontrarás recomendaciones diarias de la música que más te gusta",
                UserId = id,
                IsPublic = false,
                Editable= false,
                ImageUrl = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1734735036/pskgqakw7ojmfkn7x076.jpg",
                CreatedAt = DateTime.UtcNow
            };

            library.Playlists = [playlist];

            await _dbContext.Libraries!.AddAsync(library);
            await _dbContext.Playlists!.AddAsync(playlist);
            await _dbContext.SaveChangesAsync();

            await MailService.SendVerificationEmail(user.Email, code.ToString(), user.FirstName);
            transaction.Commit();


            return true;
    }


    /// <summary>
    /// Este metodo actualiza la información de un usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="user"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    public async Task<bool> UpdateUser(string userId, UpdateUserDto user)
    {

   
            UserUpdateValidator validator = new();
            if(validator.Validate(user).IsValid == false) throw new ValidationException("User input is not valid");
            var userToUpdate = await _dbContext.Users!.Where(u => u.State != UserState.INACTIVE).FirstOrDefaultAsync(u => u.Id == userId) ?? throw new EntityNotFoundException("User not found");
            if(userToUpdate.State == UserState.UNVERIFIED) throw new ValidationException("User is not verified");
            var country = await _dbContext.Countries!.FindAsync(user.Country) ?? throw new EntityNotFoundException("Country not found");
      
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.SurName = user.SurName;
            userToUpdate.Country = country;
            await _dbContext.SaveChangesAsync();

            return true;
   
    }


    /// <summary>
    /// Este metodo elimina un usuario de forma lógica
    /// </summary>
    /// <param name="id"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> DeleteUser(string id)
    {
   
            if(id == null) throw new ValidationException("Id cannot be null");
            var user = await _dbContext.Users!.FindAsync(id) ?? throw new EntityNotFoundException("User not found");
            user.State = UserState.INACTIVE;
            await _dbContext.SaveChangesAsync();
            return true;
     
    }


    /// <summary>
    /// Este metodo actualiza la contraseña de un usuario
    /// </summary>
    /// <param name="passwordDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> UpdateUserPassword(PasswordChangeDTO passwordDTO)
    {

   
            var passwordValidator = new PasswordValidation();
            if(passwordValidator.Validate(passwordDTO).IsValid == false) throw new ValidationException("Password input is not valid");

            var user = await _dbContext.Users!.Where(u=> u.Email == passwordDTO.Email && u.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User not found");
            if(user.VerifyCode != passwordDTO.Code) throw new ValidationException("Invalid code");

            PasswordManipulation passwordManipulation = new();
            string hashedPassword = passwordManipulation.HashPassword(passwordDTO.Password!);
            user.Password = hashedPassword;
            user.VerifyCode = 0;
            await MailService.SendPasswordChangeMail(passwordDTO.Email!, user.FirstName!);
            await _dbContext.SaveChangesAsync();
            return true;
     
    }

    /// <summary>
    /// Este metodo verifica la cuenta de un usuario
    /// </summary>
    /// <param name="verifyAccountDTO"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> VerifyAccount(VerifyAccountDTO verifyAccountDTO)
    {

   
            var user = await _dbContext.Users!.Where(u=> u.Email == verifyAccountDTO.Email && u.State == UserState.UNVERIFIED).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User not found");
            if(user.VerifyCode != verifyAccountDTO.Code) throw new ValidationException("Invalid code");
            user.State = UserState.ACTIVE;
            user.VerifyCode = 0;
            await _dbContext.SaveChangesAsync();
            await MailService.SendVerifiedAccountMail(verifyAccountDTO.Email!, user.FirstName!);
            return true;
        
    }


    /// <summary>
    /// Este metodo envia un email con un codigo de verificación para recuperar la contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="_dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> SendPasswordRecoveryCode(string email)
    {
   
            var user = await _dbContext.Users!.Where(u=> u.Email == email && u.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User not found");
            int code = new Random().Next(1000, 9999);
            user.VerifyCode = code;
            await _dbContext.SaveChangesAsync();
            await MailService.SendPasswordRecoveryCodeEmail(email, code.ToString(), user.FirstName!);
            return true;
    }


}