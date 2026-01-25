using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MortgageComparer.Data;
using MortgageComparer.BankLogic;
using MortgageComparer.BankLogic.Banks;
using MortgageComparer.BankProviders;
using MortgageComparer.BankProviders.Banks;
using MortgageComparer.Services;
using MortgageComparer.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;


namespace MortgageComparer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
        builder.Services.AddScoped<IOfferService, OfferService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IJobTypeService, JobTypeService>();
      
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnectionString")));

        builder.Services.AddScoped<IOfferService, BankEmployeeOfferService>();
        builder.Services.AddScoped<IBankService, OurBank>();
        builder.Services.AddScoped<IBankService, LecturerBank>();
        builder.Services.AddScoped<IQuoteService, QuoteService>();
        builder.Services.AddScoped<BankAggregator>();
        builder.Services.AddHttpClient("LecturerBankApi", client =>
        {
            client.BaseAddress = new Uri("https://mini.loanbank.api.snet.com.pl/api/");
        });
        builder.Services.AddHttpClient("OurBankApi", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5046/api/");
        });



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