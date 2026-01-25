using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using MortgageComparer.Data;
using MortgageComparerAPI.Services;
using MortgageComparerAPI.Services.BackgroundLogic;
using MortgageComparerAPI.Services.Interfaces;
using MortgageComparerAPI.Workers;
using SendGrid.Extensions.DependencyInjection;
using System.Text;

namespace MortgageComparerAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        var settings = builder.Configuration.GetSection("JwtToken");
        
        // To do: Zmienić ValidateIssuer i ValidateAudience na true, dodać secrety zamiast wartości
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        (settings["SecretKey"])))
                };
            });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        builder.Services.AddScoped<IApiAuthService, ApiAuthService>();
        builder.Services.AddScoped<IApiQuoteService, ApiQuoteService>();
        builder.Services.AddScoped<IApiOfferService, ApiOfferService>();


        builder.Services.Configure<AzureStorageSettings>(builder.Configuration.GetSection("AzureStorage"));
        builder.Services.AddAzureClients(clientBuilder => {
            clientBuilder.AddBlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]);
        });
        builder.Services.AddTransient<IFileStorageService, AzureBlobStorageService>();

        builder.Services.AddScoped<ICleanupService, CleanupService>();
        builder.Services.AddHostedService<CleanupWorker>();



        builder.Services.AddSendGrid(options => {
            options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
        });
        builder.Services.AddTransient<IEmailService, SendGridEmailService>();
        builder.Services.AddTransient<IEmailTemplateService, MockEmailTemplateService>();
        builder.Services.AddHostedService<ReminderWorker>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        
        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.Run();
    }
}