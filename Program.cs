using Context;
using Question.Interface;
using Question.Repository;
using Student.Interface;
using Student.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("XPolicy", builder =>
       {
           builder
                  //    .WithOrigins("http://localhost:3000")
                  .AllowAnyOrigin()
                  .WithMethods("GET", "POST")
                  .WithHeaders("Content-Type", "Access-Control-Allow-Origin", "Auth");
       });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    },
});
app.UseCors("XPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
