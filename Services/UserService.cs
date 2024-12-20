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
    /*
    This method is used to get a user by id
    */
    public static async Task<GetUserDto> GetUserById(string id, PurpuraDbContext dbContext)
    {
       try
       {
        
        var user = await dbContext.Users!.Join(dbContext.Countries!, user => user.CountryId, country => country.Id, (user, country) => new GetUserDto{ Email = user.Email, Name = user.Name, Country= country.Name, Id= user.Id, Phone = user.Phone, ProfilePicture= user.ProfilePicture }).FirstOrDefaultAsync(user => user.Id == id) ?? throw new EntityNotFoundException("User does not exist");
        
            return user;
       }
       catch(ValidationException val)
       {
           throw new ValidationException(val.Message);
       }
       catch(EntityNotFoundException)
       {
           throw new EntityNotFoundException("User does not exist");
       }
       catch (System.Exception)
       {
        
        throw new Exception("An error occured while fetching the user");
       }
     
    }

    /*
    This method is used to get a user by id in a private way
    */
    private static async Task<User> GetUser(string id, PurpuraDbContext dbContext)
    {
       try
       {
        
         var user = await dbContext.Users!.FindAsync(id) ?? throw new EntityNotFoundException("User does not exist");
        
        return user;
       }
        catch(ValidationException val)
       {
           throw new ValidationException(val.Message);
       }
       catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
       catch (System.Exception)
       {
        
        throw new Exception("An error occured while fetching the user");
       }
     
    }

    /*

    This method is used to get a user by email
    */
    public static async Task<GetUserDto> GetUserByEmail(string email, PurpuraDbContext dbContext)
    {

        try
        {
            
            var user = await dbContext.Users!.Join(dbContext.Countries!, user => user.CountryId, country => country.Id, (user, country) => new GetUserDto{ Email = user.Email, Name = user.Name, Country= country.Name, Id= user.Id, Phone = user.Phone, ProfilePicture= user.ProfilePicture }).FirstOrDefaultAsync(user => user.Email == email)?? throw new EntityNotFoundException("User does not exist");
            
            return user;
        }
         catch(ValidationException val)
       {
           throw new ValidationException(val.Message);
       }
       catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception)
        {
            throw new Exception("An error occured while fetching the user");
        }
       
    }


    /*
    This method is used to get a user by email in a private way
    */
     private static async Task<User> GetUserByEmailPrivate(string email, PurpuraDbContext dbContext)
    {

        try
        {
            var user = await dbContext.Users!.FirstOrDefaultAsync(user => user.Email == email);
            if(user == null) return null!;
            return user;
        }
         catch(ValidationException val)
       {
           throw new ValidationException(val.Message);
       }
        catch (System.Exception)
        {
            throw new Exception("An error occured while fetching the user");
        }
       
    }

    /*
    This method is used to verify if a user exists
    */
    private static async Task<bool> VerifyUserExists(string email, PurpuraDbContext dbContext)
    {

        try
        {
            var user = await dbContext.Users!.FirstOrDefaultAsync(user => user.Email == email);
            if(user == null) return false;
            return true;
        }
        catch(ValidationException val)
       {
           throw new ValidationException(val.Message);
       }
        catch (System.Exception)
        {
            throw new Exception("An error occured while fetching the user");
        }
       
    }

    /*
    This method creates a new user, asigns a library and a playlist (purple day list) to the user
    */
    public static async Task<bool> CreateUser(CreateUserDTO user, PurpuraDbContext dbContext)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            if(user == null) throw new ValidationException("User cannot be null");
            UserValidator validator = new ();
            ValidationResult result = validator.Validate(user);
            if (!result.IsValid) throw new ValidationException(result.Errors);

            if (await VerifyUserExists(user.Email, dbContext)) throw new ValidationException("User already exists");


            string id = Guid.NewGuid().ToString();
            PasswordManipulation passwordManipulation = new();
            string hashedPassword = passwordManipulation.HashPassword(user.Password);
            // Crear codigo de verificación
            int code = new Random().Next(1000, 9999);
            User newUser = new()
            {
                Id = id,
                Name = user.Name,
                Email = user.Email,
                Password = hashedPassword,
                Phone = user.Phone,
                CountryId = user.Country,
                State = UserState.UNVERIFIED,
                CreatedAt = DateTime.UtcNow,
                ProfilePicture = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1734452153/omhdxtxwbsza78sz4nkr.png",
                VerifyCode = code
            };

            await dbContext.Users!.AddAsync(newUser);
            


            string libraryId = Guid.NewGuid().ToString();
            var library = new Library
            {
                Id = Guid.NewGuid().ToString(),
                UserId = id
            };

            var playlist = new Playlist
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Purple Day List",
                Description= "Esta es tu Purple Day List, aquí encontrarás recomendaciones diarias de la música que más te gusta",
                UserId = id,
                IsPublic = false,
                ImageUrl = "https://res.cloudinary.com/dw43hgf5p/image/upload/v1734735036/pskgqakw7ojmfkn7x076.jpg",
                CreatedAt = DateTime.UtcNow
            };

            library.Playlists = [playlist];

            await dbContext.Libraries!.AddAsync(library);
            await dbContext.Playlists!.AddAsync(playlist);
            await dbContext.SaveChangesAsync();

            await MailService.SendVerificationEmail(user.Email, code.ToString(), user.Name);
            transaction.Commit();


            return true;
        }
        catch (ValidationException val)
        {   transaction.Rollback();
            throw new ValidationException(val.Message);
        }
        catch(DbException)
        {
            transaction.Rollback();
            throw new ValidationException("An error occured with the server while creating the user");
        }
        catch (System.Exception e)
        {
            transaction.Rollback();
            throw new Exception(e.Message);
        }
    }


    /*
    This method is used to update user's information (Only name and phone)
    */
    public static async Task<bool> UpdateUser(UpdateUserDto user, PurpuraDbContext dbContext)
    {

        try
        {
            if(user == null) throw new ValidationException("User cannot be null");
            UserUpdateValidator validator = new();
            if(validator.Validate(user).IsValid == false) throw new ValidationException("User input is not valid");
            var userToUpdate = await dbContext.Users!.FindAsync(user.Id) ?? throw new EntityNotFoundException("User does not exist");
            userToUpdate.Name = user.Name;
            userToUpdate.Phone = user.Phone;

            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (ValidationException val)
        {
            throw new ValidationException(val.Message);
        }
        catch(DbException)
        {
            
            throw new ValidationException("An error occured with the server while updating the user");
        }
         catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception e)
        {
            throw new Exception(e.Message);
        }
   
    }


    /*
    This method is used to delete Logically a user (Change state to inactive)
    */
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
        catch (ValidationException val)
        {
            throw new ValidationException(val.Message);
        }
        catch(DbException)
        {
            throw new ValidationException("An error occured with the server while deleting the user");
        }
         catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception e)
        {
            throw new Exception(e.Message);
        }
     
    }


    /*
    This method is used to change the password of a user after receiving a code in the email
    */
    public static async Task<bool> UpdateUserPassword(PasswordChangeDTO passwordDTO, PurpuraDbContext dbContext)
    {

        try
        {
            if(passwordDTO == null) throw new ValidationException("Password cannot be null");
            var passwordValidator = new PasswordValidation();
            if(passwordValidator.Validate(passwordDTO).IsValid == false) throw new ValidationException("Password input is not valid");

            var user = await GetUserByEmailPrivate(passwordDTO.Email!, dbContext) ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.UNVERIFIED) throw new ValidationException("User is not verified");
            if(user.VerifyCode != passwordDTO.Code) throw new ValidationException("Invalid code");

            PasswordManipulation passwordManipulation = new();
            string hashedPassword = passwordManipulation.HashPassword(passwordDTO.Password!);
            user.Password = hashedPassword;
            user.VerifyCode = 0;
            await MailService.SendPasswordChangeMail(passwordDTO.Email!, user.Name!);
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (ValidationException val)
        {
            throw new ValidationException(val.Message);
            
        }
         catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception)
        {
            
            throw new Exception("An error occured while updating the password");
        }
     
    }

    /*
    This method is used to verify the user account after de sign up and code sending via email process
    */
    public static async Task<bool> VerifyAccount(VerifyAccountDTO verifyAccountDTO, PurpuraDbContext dbContext)
    {

        try
        {
            var user = await GetUserByEmailPrivate(verifyAccountDTO.Email!, dbContext) ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.ACTIVE) throw new ValidationException("User is already verified");
            if(user.VerifyCode != verifyAccountDTO.Code) throw new ValidationException("Invalid code");
            user.State = UserState.ACTIVE;
            user.VerifyCode = 0;
            await dbContext.SaveChangesAsync();
            await MailService.SendVerifiedAccountMail(verifyAccountDTO.Email!, user.Name!);
            return true;
        }
        catch (ValidationException val)
        {
            throw new ValidationException(val.Message);
        }
         catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception)
        {
            throw new Exception("An error occured while verifying the account");
        }
        
    }


    public static async Task<bool> SendPasswordRecoveryCode(string email, PurpuraDbContext dbContext)
    {
        try
        {
            var user = await GetUserByEmailPrivate(email, dbContext) ?? throw new EntityNotFoundException("User does not exist");
            if(user.State == UserState.UNVERIFIED) throw new ValidationException("User is not verified");
            int code = new Random().Next(1000, 9999);
            user.VerifyCode = code;
            await dbContext.SaveChangesAsync();
            await MailService.SendPasswordRecoveryCodeEmail(email, code.ToString(), user.Name!);
            return true;
        }
        catch (ValidationException val)
        {
            throw new ValidationException(val.Message);
        }
         catch(EntityNotFoundException ex)
       {
           throw new EntityNotFoundException(ex.Message);
       }
        catch (System.Exception)
        {
            throw new Exception("An error occured while sending the verification code");
        }
    }



    // Send verification code and the rest methods realated to smtp services will be implemented in a nodejs microservice
}