using MortgageComparer.Controllers;
using MortgageComparer.Entities;

namespace MortgageComparer.Services.Interfaces;

public interface IUserService
{
    public int? GetUserId();
    public Task<UserEntity> AddUserAsync(UserEntity? user);
    public Task UpdateBirthdayAsync(UserBirthDateDto user);
    public Task<UserProfileDto> GetProfileAsync();
    public Task UpdateProfileAsync(UserToUpdateData userId);
}