using Contracts;
using Repository;
using Service;
using Service.Contracts;
using AlinaverseAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureCors();
builder.Services.AddControllers();
builder.Services.ConfigureJWT();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

app.UseAuthorization();


app.Run();