using Api.SalesOrder.API.Middleware;
using Api.SalesOrder.API.Services;
using Api.SalesOrder.Application.Mappings;
using Api.SalesOrder.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // Configure Swagger with JWT
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sales Order Management API",
                Version = "v1",
                Description = "API for managing sales orders with Clean Architecture and CQRS pattern",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "dev@salesorder.com"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        // Add Infrastructure
        builder.Services.AddInfrastructure(builder.Configuration);

        // Add MediatR
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.Load("Api.SalesOrder.Application")));

        // Add AutoMapper
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        // Add FluentValidation
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssembly(Assembly.Load("Api.SalesOrder.Application"));

        // Add JWT Authentication
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "SuperSecretKeyForDevelopment123!");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("Vendedor", policy => policy.RequireRole("Vendedor", "Admin"));
        });

        // Add JWT Service
        builder.Services.AddScoped<IJwtService, JwtService>();

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure pipeline
        //if (app.Environment.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales Order API v1");
                c.RoutePrefix = "swagger";
            });
        //}

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.MapControllers();

        app.Run();
    }
}