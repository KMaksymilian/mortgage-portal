using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MortgageComparer.Data;
using MortgageComparer.Services;
using MortgageComparer.Services.BackgroundLogic;
using MortgageComparer.Services.Interfaces;
using MortgageComparer.Workers;
using MortgageComparer.Workers;
using SendGrid.Extensions.DependencyInjection;
using System.Text;


namespace MortgageComparer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
        builder.Services.AddScoped<IOfferService, OfferService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IJobTypeService, JobTypeService>();
        builder.Services.AddHttpClient();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"])),
                
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                
                ValidateLifetime = true
            };
        });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddScoped<ICleanupService, CleanupService>();
        builder.Services.AddHostedService<CleanupWorker>();

        builder.Services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]);
        });
        builder.Services.AddTransient<IFileStorageService, AzureBlobStorageService>();

        builder.Services.AddSendGrid(options => {
            options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
        });
        builder.Services.AddTransient<IEmailService, SendGridEmailService>();
        builder.Services.AddTransient<IEmailTemplateService, MockEmailTemplateService>();
        builder.Services.AddHostedService<ReminderWorker>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseRouting();
        
        app.UseCors("AllowReactApp");

        //app.UseHttpsRedirection();
        
        app.UseAuthorization();

        app.MapControllers();



        app.Run();
    }
}