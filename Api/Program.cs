using Api.Authorization;
using Api.Data;
using Api.Middleware;
using Api.Models.Data;
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        Title = "Base Application API",
        Version = "v1",
        Description = "Provides a modern starter API."
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
builder.Services.AddScoped<IAuthService, AuthService>();

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
app.MapOpenApi();

// Exception handling should be first
app.UseExceptionHandler("/error");

// Logging middleware early so it captures all requests
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("/index.html");

// Swagger (only in dev)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test API");
        options.RoutePrefix = "";
        options.DocumentTitle = "Test API Docs";
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
    });

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

}
else
{
    app.UseHsts();
}

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// DB seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Users.Any())
    {
        var json = File.ReadAllText("userSeedData.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var users = JsonSerializer.Deserialize<List<User>>(json, options);
        db.Users.AddRange(users!);
        db.SaveChanges();
    }
}

app.Run();