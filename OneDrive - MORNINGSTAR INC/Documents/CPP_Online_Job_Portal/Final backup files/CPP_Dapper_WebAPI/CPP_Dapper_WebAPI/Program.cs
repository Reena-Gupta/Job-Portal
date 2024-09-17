
using System.Text;
using CPP_Dapper_WebAPI.Admin_Repository;
using CPP_Dapper_WebAPI.JobPosting_Repository;
using CPP_Dapper_WebAPI.Recruiters_Repository;
using JobPortalWebAPI.JobApplicationRepository;
using JobPortalWebAPI.JobSeekerRepo;
using JobPortalWebAPI.LoginRequest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace CPP_Dapper_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddTransient<IJobPostings, JobPostingsOperations>();
            builder.Services.AddTransient<IRecruiters, RecruitersOperations>();
            builder.Services.AddTransient<IAdmin, AdminOperations>();
            builder.Services.AddTransient<ILoginRequest, LoginRequestOperation>();
            builder.Services.AddTransient<IJobApplication, JobApplicationOperation>();
            builder.Services.AddTransient<IJobSeeker, JobSeekerOperation>();

            //SeriLog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/CPP_JPS.txt")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddControllers().AddNewtonsoftJson(option =>
            option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson(option => option.SerializerSettings.ContractResolver = new DefaultContractResolver());


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
