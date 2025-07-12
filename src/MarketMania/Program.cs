using AutoMapper;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection;
using MarketMania.Authentication.Entities;
using MarketMania.Authentication.Generators;
using MarketMania.BusinessLayer.Clients;
using MarketMania.BusinessLayer.Clients.Interfaces;
using MarketMania.BusinessLayer.Mapping;
using MarketMania.BusinessLayer.Services;
using MarketMania.BusinessLayer.Services.Interfaces;
using MarketMania.BusinessLayer.Settings;
using MarketMania.BusinessLayer.Startup;
using MarketMania.DataAccessLayer;
using MarketMania.Extensions;
using MarketMania.Requirements;
using MarketMania.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using MinimalHelpers.Routing;
using MinimalHelpers.Validation;
using OperationResults.AspNetCore.Http;
using SimpleAuthentication;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using ResultErrorResponseFormat = OperationResults.AspNetCore.Http.ErrorResponseFormat;
using ValidationErrorResponseFormat = MinimalHelpers.Validation.ErrorResponseFormat;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings));
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings));

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRequestLocalization(settings.SupportedCultures);
builder.Services.AddWebOptimizer(minifyCss: true, minifyJavaScript: builder.Environment.IsProduction());

builder.Services.AddDefaultExceptionHandler();
builder.Services.AddDefaultProblemDetails();

builder.Services.AddAutoMapper(options =>
{
    var profiles = new List<Profile>();
    var profileTypes = typeof(UserMapperProfile).Assembly.GetTypes().Where(t => typeof(Profile).IsAssignableFrom(t));

    foreach (var profileType in profileTypes)
    {
        profiles.Add((Profile)Activator.CreateInstance(profileType));
    }

    options.AddProfiles(profiles);
});

builder.Services.AddOperationResult(options =>
{
    options.ErrorResponseFormat = ResultErrorResponseFormat.List;
});

builder.Services.ConfigureValidation(options =>
{
    options.ErrorResponseFormat = ValidationErrorResponseFormat.List;
});

if (swagger.IsEnabled)
{
    builder.Services.AddOpenApi(options =>
    {
        options.AddDefaultProblemDetailsResponse();
        options.AddAcceptLanguageHeader();
        options.AddSimpleAuthentication(builder.Configuration);
    });
}

builder.Services.AddDataProtection()
    .SetApplicationName(settings.ApplicationName)
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddScoped(services =>
{
    var dataProtectionProvider = services.GetRequiredService<IDataProtectionProvider>();
    var dataProtector = dataProtectionProvider.CreateProtector(settings.ApplicationName);

    return dataProtector;
});

builder.Services.AddScoped(services =>
{
    var dataProtector = services.GetRequiredService<IDataProtector>();
    return dataProtector.ToTimeLimitedDataProtector();
});

builder.Services.AddScoped<IDataProtectionService, DataProtectionService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailClient, EmailClient>();

builder.Services.AddAzureSql<ApplicationDbContext>(builder.Configuration.GetConnectionString("SqlConnection"));
builder.Services.AddScoped<IApplicationDbContext>(services => services.GetRequiredService<ApplicationDbContext>());

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddSimpleAuthentication(builder.Configuration, addAuthorizationServices: false)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Accounts/Login";
    options.LogoutPath = "/Accounts/Logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddScoped<IAuthorizationHandler, UserActiveHandler>();
builder.Services.AddAuthorization(options =>
{
    var authorizationPolicyBuilder = new AuthorizationPolicyBuilder().RequireAuthenticatedUser();
    authorizationPolicyBuilder.Requirements.Add(new UserActiveRequirement());

    options.DefaultPolicy = authorizationPolicyBuilder.Build();

    options.AddPolicy("Admin", policy =>
    {
        policy.Requirements.Add(new UserActiveRequirement());
        policy.RequireRole(RoleNames.Administrator, RoleNames.PowerUser);
    });
});

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IMeService, MeService>();

if (settings.ExecuteStartup)
{
    builder.Services.AddHostedService<IdentityStartupService>();
}

var app = builder.Build();
app.Environment.ApplicationName = settings.ApplicationName;

app.UseHttpsRedirection();
app.UseRequestLocalization();

app.UseRouting();
app.UseWebOptimizer();

app.UseWhen(context => context.IsWebRequest(), builder =>
{
    if (!app.Environment.IsDevelopment())
    {
        builder.UseExceptionHandler("/Errors/500");
        builder.UseHsts();
    }

    builder.UseStatusCodePagesWithReExecute("/Errors/{0}");
});

app.UseStaticFiles();
app.UseDefaultFiles();

app.UseWhen(context => context.IsApiRequest(), builder =>
{
    builder.UseExceptionHandler();
    builder.UseStatusCodePages();
});

if (swagger.IsEnabled)
{
    app.UseMiddleware<SwaggerBasicAuthenticationMiddleware>();
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", settings.ApplicationName);
        options.InjectStylesheet("/css/swagger.css");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapEndpoints();

await app.RunAsync();