using Portfolio_AppRepo_API.Models;
using Portfolio_AppRepo_API.Repository.ApplicationFileRepo;
using Portfolio_AppRepo_API.Repository.ApplicationRepo;
using Portfolio_AppRepo_API.Repository.AuditRepo;
using Portfolio_AppRepo_API.Repository.EndpointRepo;
using Portfolio_AppRepo_API.Repository.ErrorLogRepo;
using Portfolio_AppRepo_API.Repository.UserRepo;
using Portfolio_AppRepo_API.Services.AuthorizeAD;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<ApplicationRepoContext>(op => op.UseSqlServer(builder.Configuration["ConnectionStrings:DbConnection"]));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IApplicationRepo, AplicationRepo>();
builder.Services.AddScoped<IApplicationFileRepo, ApplicationFileRepo>();
builder.Services.AddScoped<IAuditRepo, AuditRepo>();
builder.Services.AddScoped<IEndpointRepo, EndpointRepo>();
builder.Services.AddScoped<IErrorLogRepo, ErrorLogRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAuthorizeADService, AuthorizeADService>();
builder.Services.Configure<IISServerOptions>(options => options.AutomaticAuthentication = false);
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddControllers();
builder.Services.AddCors(op=>op.AddPolicy("AppRepoPolicy", build => build.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();
app.MapControllers();

app.Run();
