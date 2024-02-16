using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QualityStation.API;
using QualityStation.API.Services.DataParserService;
using QualityStation.API.Services.PasswordService;
using QualityStation.API.Services.StationDbService;
using QualityStation.API.Services.TokenService;
using QualityStation.API.Services.UserDbService;
using QualityStation.API.Services.UserValidationService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database context registration
builder.Services.AddDbContext<StationContext>(options =>
{
	options.UseSqlite(builder.Configuration.GetSection("ConnectionStrings:SQLite").Value!);
});

// Add services
builder.Services.AddAutoMapper(typeof(Program));

// Add project services
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IUserValidationService, UserValidationService>();
builder.Services.AddSingleton<IDataParserService, DataParserService>();
builder.Services.AddSingleton<IPasswordService, NormalPasswordService>();
builder.Services.AddScoped<IUserDbService, RealUserDbService>();
builder.Services.AddScoped<IStationDbService, RealStationDbService>();

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(
		builder =>
		{
			builder.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod();
		});
});

// Add Authentication service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			string strSecreteKey = builder.Configuration.GetSection("Authentication:Jwt:SecreteKey").Value!;

			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strSecreteKey))
		};
		});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.UseCors();

app.Run();
