using Context;
using Question.Interface;
using Question.Repository;
using Student.Interface;
using Student.Repository;
using TimedBackgroundTasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ErrorCatcher;
using User.Interface;
using User.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddHostedService<TimedHostedService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("XPolicy", builder =>
       {
           builder
                  .WithOrigins("http://localhost:3000")
                    // .AllowAnyOrigin()
                     .SetIsOriginAllowed((host) => true)
                  .AllowAnyMethod()
                  //   .WithMethods("GET", "POST")
                  .AllowCredentials()
                  .AllowAnyHeader();
           //   .WithHeaders("Content-Type", "Access-Control-Allow-Origin", "Auth");
       });
});
builder.Services.AddDistributedMemoryCache(); // Use in-memory cache for session (for demo purposes)
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout as needed
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "your_issuer",
                    ValidAudience = "your_audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_ludex")),
                };
            });

IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()  // Optionally, add environment variables.
            .Build();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseErrorHandlingMiddleware();
app.UseCors("XPolicy");
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        ctx.Context.Response.Headers.Append("Content-Disposition", "inline; filename=FileName.pdf");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers",
          "Origin, X-Requested-With, Content-Type, Accept, Content-Disposition");
    },
});
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();

// public class MyCustomMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly ICorsPolicyProvider _corsPolicyProvider;

//     public MyCustomMiddleware(RequestDelegate next, ICorsPolicyProvider corsPolicyProvider)
//     {
//         _next = next;
//         _corsPolicyProvider = corsPolicyProvider;
//     }

//     public async Task InvokeAsync(HttpContext context)
//     {
//         // Check if the request is for static files
//         if (context.Request.Path.StartsWithSegments("/path/to/static/files"))
//         {
//             // Retrieve CORS policy
//             var policy = await _corsPolicyProvider.GetPolicyAsync(context, "MyCorsPolicy");

//             // Evaluate CORS policy
//             var corsResult = policy.Evaluate(context);

//             // Apply CORS result to the response
//             corsResult.ApplyResult(context.Response);
//         }
//         else
//         {
//             // Call the next middleware in the pipeline
//             await _next(context);
//         }
//     }
// }