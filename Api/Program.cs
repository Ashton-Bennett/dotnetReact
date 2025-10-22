using Api.Authorization;
using Api.Data;
using Api.Models;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TestDb"));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WeatherForecast API",
        Version = "v1",
        Description = "A simple API for weather forecasts"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddSingleton<IAuthorizationHandler, AdminOrSelfHandler>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var schemeConfig = builder.Configuration.GetSection("Authentication:Schemes:LocalAuthIssuer");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = schemeConfig["ValidIssuer"],
        ValidAudiences = schemeConfig.GetSection("ValidAudiences").Get<string[]>(),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
         policy => policy.RequireRole("Admin"));

    options.AddPolicy("AdminOrSelf", policy =>
        policy.Requirements.Add(new AdminOrSelfRequirement()));
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

    // Use this to authenticate the use of endpoint points that require Admin role. 
    //User fakeAdminUser = new User
    //{
    //    Email = "test@test.com",
    //    Username = "ashtonb",
    //    Roles = new List<string> { "Admin" },
    //    Id = 123
    //};

    //string token = tokenService.GenerateAccessToken(fakeAdminUser);
    //Console.WriteLine("Swagger Admin Token:" + token);

    // Use this to authenticate the use of endpoint points that require guest role. 
    User fakeGuestUser = new User
    {
        Email = "guest@SeedData.com",
        Username = "guest_mike",
        Roles = new List<string> { "Guest" },
        Id = 4
    };

    string token = tokenService.GenerateAccessToken(fakeGuestUser);
    Console.WriteLine("Swagger Guest Token:" + token);
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); // Generates swagger.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
        options.RoutePrefix = ""; // URL: /swagger
        options.DocumentTitle = "Test API Docs";
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // optional
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext context) =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = feature?.Error;

    var statusCode = exception switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };

    var result = new
    {
        message = exception?.Message ?? "An unexpected error occurred.",
        statusCode = statusCode,
        timestamp = DateTime.UtcNow
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    return Results.Json(result);
});


app.UseStatusCodePagesWithReExecute("/Error/{0}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        var json = File.ReadAllText("userSeedData.json"); // save your JSON in this file
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var users = JsonSerializer.Deserialize<List<User>>(json, options);

        db.Users.AddRange(users!);
        db.SaveChanges();
    }
}

app.Run();
