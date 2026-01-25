using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MortgageComparer.Controllers;
using MortgageComparer.Data;
using MortgageComparer.Entities;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private AppDbContext _context;

    public UserService(IHttpContextAccessor httpContextAccessor,  AppDbContext context)
    {
        _context =  context;
        _httpContextAccessor = httpContextAccessor;
    }
    public int? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return null;
        }
        return userId;
    }
    
    public async Task<UserEntity> AddUserAsync(UserEntity? user)
    {
        if (user == null)
        {
            throw new Exception("Brak danych użytkownika");
        }
        bool exists = await _context.Users.AnyAsync((u) => u.Email == user.Email);
        if (exists)
        {
            throw new Exception("Użytkownik o tych samych danych istnieje");
        }
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateBirthdayAsync(UserBirthDateDto user)
    {
        int? userId = GetUserId();
        if (!userId.HasValue)
        {
            throw new Exception("Brak użytkownika");
        }
        
        UserEntity foundUser = await _context.Users.FindAsync(userId);
        if (foundUser == null)
        {
            throw new Exception("Nie znaleziono.");
        }
        // PostgreSQL przyjmuje tylko UTC time
        foundUser.DateOfBirth = user.BirthDate.ToUniversalTime();
        await _context.SaveChangesAsync();
    }

    public async Task<UserProfileDto> GetProfileAsync()
    {
        int? userId = GetUserId();

        if (!userId.HasValue)
        {
            throw new Exception("Brak użytkownika w bazie");
        }
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            throw new Exception("Nie znaleziono użytkownika.");
        }
        return new UserProfileDto()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            HasBirthDate = user.DateOfBirth != null,
        };   
    }

    public async Task UpdateProfileAsync(UserToUpdateData user)
    {
        int? userId = GetUserId();
        var userToUpdate = await _context.Users.FindAsync(userId);
        if (userToUpdate == null)
        {
            throw new Exception("Problemik");
        }
        userToUpdate.DateOfBirth = DateTime.Parse(user.BirthDate).ToUniversalTime();
        if (user.JobEndDate != null)
        {
            userToUpdate.JobEndDate = DateTime.Parse(user.JobEndDate).ToUniversalTime();
        }
        else
        {
            userToUpdate.JobEndDate = DateTime.UtcNow;
        }
        userToUpdate.JobStartDate = DateTime.Parse(user.JobStartDate).ToUniversalTime();
        userToUpdate.Income = user.Earnings;
        await _context.SaveChangesAsync();
        
    }
}