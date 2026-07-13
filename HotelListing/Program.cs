using System.Text.Json.Serialization;
using HotelListing.Constants;
using HotelListing.Data;
using HotelListing.Handlers;
using HotelListing.Interfaces;
using HotelListing.MappingProfiles;
using HotelListing.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<HotelListingDbContext>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = AuthenticationDefaults.BasicScheme;
        options.DefaultChallengeScheme = AuthenticationDefaults.BasicScheme;
    })
    .AddScheme<AuthenticationSchemeOptions,BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => {});
builder.Services.AddAuthorization();

// builder.Services.AddIdentityCore<ApplicationUser>()
//     .AddRoles<IdentityRole>()
//     .AddDefaultTokenProviders();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IHotelsService, HotelsService>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<HotelMappingProfile>();
    config.AddProfile<CountryMappingProfile>();
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();