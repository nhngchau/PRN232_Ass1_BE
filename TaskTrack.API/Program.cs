using Microsoft.EntityFrameworkCore;
using TaskTrack.API.Configuration;
using TaskTrack.Repo;
using TaskTrack.Repo.Repositories.Implementations;
using TaskTrack.Repo.Repositories.Interfaces;
using TaskTrack.Service.Implementations;
using TaskTrack.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = DatabaseUrlParser.BuildConnectionString(builder.Configuration);
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection is missing. Configure ConnectionStrings:DefaultConnection or DATABASE_URL.");
}

builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("TaskTrackCors", policy =>
    {
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
            ?? builder.Configuration["FrontendUrl"]
            ?? "http://localhost:3000";

        policy.WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("TaskTrackCors");
app.MapControllers();

app.Run();
