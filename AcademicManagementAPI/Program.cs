using AcademicManagement.BLL.Services;
using AcademicManagement.BLL.Validation;
using AcademicManagement.DAL.Data;
using AcademicManagement.DAL.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AcademicManagementAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<AcademicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<DepartmentRepo>();
            builder.Services.AddScoped<StudentRepo>();
            builder.Services.AddScoped<CourseRepo>();

            builder.Services.AddScoped<DepartmentService>();
            builder.Services.AddScoped<StudentService>();
            builder.Services.AddScoped<CourseService>();

            builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentValidator>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Academic Management API",
                    Version = "v1",
                    Description = "RESTful API for managing academic departments, students, courses and enrollments."
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                    if (exception != null)
                        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                            context.Request.Method, context.Request.Path);

                    context.Response.ContentType = "application/problem+json";

                    var statusCode = exception switch
                    {
                        KeyNotFoundException => StatusCodes.Status404NotFound,
                        InvalidOperationException => StatusCodes.Status409Conflict,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    context.Response.StatusCode = statusCode;

                    var problem = new ProblemDetails
                    {
                        Status = statusCode,
                        Title = statusCode switch
                        {
                            404 => "Resource Not Found",
                            409 => "Conflict",
                            _ => "Internal Server Error"
                        },
                        Detail = statusCode == StatusCodes.Status500InternalServerError
                            ? "An unexpected error occurred. Please try again later."
                            : exception?.Message,
                        Instance = context.Request.Path
                    };

                    await context.Response.WriteAsJsonAsync(problem);
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}
