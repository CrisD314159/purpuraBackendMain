namespace purpuraMain.Services;
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

public static class UserService
{
    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static async Task<GetUserDto> GetUserById(string id, PurpuraDbContext dbContext)
    {
       try
       {
        
        var user = await dbContext.Users!.Where(u => u.State != UserState.INACTIVE).Join(dbContext.Countries!, user => user.CountryId, country => country.Id, (user, country) => new GetUserDto{ Email = user.Email, FirstName = user.FirstName!, Country= country.Name, Id= user.Id, SurName = user.SurName!, ProfilePicture= user.ProfilePicture, IsVerified= user.State == UserState.ACTIVE, CountryId = country.Id }).FirstOrDefaultAsync(user => user.Id == id) ?? throw new EntityNotFoundException("User does not exist");
        
        return user;
       }
       catch (System.Exception)
       {
        
        throw ;
       }
     
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    private static async Task<User> GetUser(string id, PurpuraDbContext dbContext)
    {
       try
       {
        
        var user = await dbContext.Users!.Where(u => u.State != UserState.INACTIVE).FirstOrDefaultAsync(u => u.Id == id) ?? throw new EntityNotFoundException("User does not exist");
        
        return user;
       }
       catch (System.Exception)
       {
        
        throw;
       }
     
    }
   
   /// <summary>
   /// Este metodo crea un usuario, su biblioteca y la playlist de recomendaciones por defecto
   /// </summary>
   /// <param name="user"></param>
   /// <param name="dbContext"></param>
   /// <returns></returns>
    public static async Task<bool> CreateUser(CreateUserDTO user, PurpuraDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            UserValidator validator = new ();
            ValidationResult result = validator.Validate(user);
            if (!result.IsValid) throw new ValidationException("User input is not valid");

            if (await dbContext.Users!.AnyAsync(u => u.Email == user.Email)) throw new ValidationException("User already exists");


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

            await dbContext.Users!.AddAsync(newUser);
            

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

            await dbContext.Libraries!.AddAsync(library);
            await dbContext.Playlists!.AddAsync(playlist);
            await dbContext.SaveChangesAsync();

            await MailService.SendVerificationEmail(user.Email, code.ToString(), user.FirstName);
            transaction.Commit();


            return true;
        }
        catch (System.Exception)
        {
            transaction.Rollback();
            throw;
        }
    }


    /// <summary>
    /// Este metodo actualiza la información de un usuario
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="user"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static async Task<bool> UpdateUser(string userId, UpdateUserDto user, PurpuraDbContext dbContext)
    {

        try
        {
            UserUpdateValidator validator = new();
            if(validator.Validate(user).IsValid == false) throw new ValidationException("User input is not valid");
            var userToUpdate = await dbContext.Users!.Where(u => u.State != UserState.INACTIVE).FirstOrDefaultAsync(u => u.Id == userId) ?? throw new EntityNotFoundException("User does not exist");
            if(userToUpdate.State == UserState.UNVERIFIED) throw new ValidationException("User is not verified");
            var country = await dbContext.Countries!.FindAsync(user.Country) ?? throw new EntityNotFoundException("Country  not found");
      
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.SurName = user.SurName;
            userToUpdate.Country = country;
            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (System.Exception)
        {
            throw ;
        }
   
    }


    /// <summary>
    /// Este metodo elimina un usuario de forma lógica
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<bool> DeleteUser(string id, PurpuraDbContext dbContext)
    {
        try
        {
            if(id == null) throw new ValidationException("Id cannot be null");
            var user = await GetUser(id, dbContext) ?? throw new EntityNotFoundException("User does not exist");
            user.State = UserState.INACTIVE;
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (System.Exception e)
        {
            throw new Exception(e.Message);
        }
     
    }


    /// <summary>
    /// Este metodo actualiza la contraseña de un usuario
    /// </summary>
    /// <param name="passwordDTO"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<bool> UpdateUserPassword(PasswordChangeDTO passwordDTO, PurpuraDbContext dbContext)
    {

        try
        {
            var passwordValidator = new PasswordValidation();
            if(passwordValidator.Validate(passwordDTO).IsValid == false) throw new ValidationException("Password input is not valid");

            var user = await dbContext.Users!.Where(u=> u.Email == passwordDTO.Email && u.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.UNVERIFIED) throw new NotVerifiedException("User is not verified");
            if(user.VerifyCode != passwordDTO.Code) throw new ValidationException("Invalid code");

            PasswordManipulation passwordManipulation = new();
            string hashedPassword = passwordManipulation.HashPassword(passwordDTO.Password!);
            user.Password = hashedPassword;
            user.VerifyCode = 0;
            await MailService.SendPasswordChangeMail(passwordDTO.Email!, user.FirstName!);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (System.Exception)
        {
            
            throw new Exception("An error occured while updating the password");
        }
     
    }

    /// <summary>
    /// Este metodo verifica la cuenta de un usuario
    /// </summary>
    /// <param name="verifyAccountDTO"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<bool> VerifyAccount(VerifyAccountDTO verifyAccountDTO, PurpuraDbContext dbContext)
    {

        try
        {
            var user = await dbContext.Users!.Where(u=> u.Email == verifyAccountDTO.Email && u.State == UserState.UNVERIFIED).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.ACTIVE) throw new ValidationException("User is already verified");
            if(user.VerifyCode != verifyAccountDTO.Code) throw new ValidationException("Invalid code");
            user.State = UserState.ACTIVE;
            user.VerifyCode = 0;
            await dbContext.SaveChangesAsync();
            await MailService.SendVerifiedAccountMail(verifyAccountDTO.Email!, user.FirstName!);
            return true;
        }
        catch (System.Exception)
        {
            throw new Exception("An error occured while trying to verify your account");
        }
        
    }


    /// <summary>
    /// Este metodo envia un email con un codigo de verificación para recuperar la contraseña
    /// </summary>
    /// <param name="email"></param>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<bool> SendPasswordRecoveryCode(string email, PurpuraDbContext dbContext)
    {
        try
        {
           var user = await dbContext.Users!.Where(u=> u.Email == email && u.State == UserState.ACTIVE).FirstOrDefaultAsync() ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.UNVERIFIED) throw new NotVerifiedException("User is not verified");
            int code = new Random().Next(1000, 9999);
            user.VerifyCode = code;
            await dbContext.SaveChangesAsync();
            await MailService.SendPasswordRecoveryCodeEmail(email, code.ToString(), user.FirstName!);
            return true;
        }
        catch (System.Exception)
        {
            throw new Exception("An error occured while sending the verification code");
        }
    }


}