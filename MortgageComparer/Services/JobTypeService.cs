using Microsoft.EntityFrameworkCore;
using MortgageComparer.Data;
using MortgageComparer.Models;
using MortgageComparer.Services.Interfaces;

namespace MortgageComparer.Services;

public class JobTypeService : IJobTypeService
{
    private readonly IUserService _userService;
    private AppDbContext  _context;

    public JobTypeService(IUserService userService,  AppDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    public async Task<GetJobAndDocumentResponse> GetJobAndDocumentAsync()
    {
        int? userId = _userService.GetUserId();
        if (userId == null)
        {
            throw new Exception("Użytkownik nie jest zalogowany.");
        }

        var user = await _context.Users
            .Include(u => u.JobType)
            .Include(u => u.PersonalDocument) 
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            throw new Exception("Not Found");
        }

        bool dataChanged = false;

        if (dataChanged)
        {
            await _context.SaveChangesAsync();
        }
        return new GetJobAndDocumentResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            BirthDate = user.DateOfBirth,
            Job = new DocumentDto(){
                Name = user.JobType?.Name,
                Description = user.JobType?.Description
            },
            Document = new DocumentDto(){
                Name = user.PersonalDocument?.Name,
                Description = user.PersonalDocument?.Description
            }
        };
    }
}
