using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using FluentValidation;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection;
using MarketMania.Authentication.Entities;
using MarketMania.Authentication.Generators;
using MarketMania.Authentication.Generators.Interfaces;
using MarketMania.BusinessLayer.Clients;
using MarketMania.BusinessLayer.Clients.Interfaces;
using MarketMania.BusinessLayer.Extensions;
using MarketMania.BusinessLayer.Generators;
using MarketMania.BusinessLayer.Generators.Interfaces;
using MarketMania.BusinessLayer.Providers;
using MarketMania.BusinessLayer.Services;
using MarketMania.BusinessLayer.Settings;
using MarketMania.BusinessLayer.Templating;
using MarketMania.BusinessLayer.Templating.Interfaces;
using MarketMania.BusinessLayer.Validations;
using MarketMania.Contracts;
using MarketMania.DataAccessLayer;
using MarketMania.Extensions;
using MarketMania.HealthChecks;
using MarketMania.Requirements;
using MarketMania.Services;
using MarketMania.Startup;
using MarketMania.StorageProviders.Extensions;
using MarketMania.Swagger;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MinimalHelpers.Routing;
using MinimalHelpers.Validation;
using OperationResults.AspNetCore.Http;
using Serilog;
using SimpleAuthentication;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using TinyHelpers.Extensions;
using TinyHelpers.Json.Serialization;
using ResultErrorResponseFormat = OperationResults.AspNetCore.Http.ErrorResponseFormat;
using ValidationErrorResponseFormat = MinimalHelpers.Validation.ErrorResponseFormat;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings));
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings));

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRequestLocalization(settings.SupportedCultures);
builder.Services.AddWebOptimizer(minifyCss: true, minifyJavaScript: builder.Environment.IsProduction());

builder.Services.AddDefaultExceptionHandler();
builder.Services.AddDefaultProblemDetails();
builder.Services.AddRequestTimeouts();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<TimeZoneTimeProvider>();
builder.Services.AddSingleton<ITimeZoneService, TimeZoneService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.SerializerOptions.Converters.Add(new UtcDateTimeConverter());
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddAutoMapper();
builder.Services.AddValidatorsFromAssemblyContaining<SaveCategoryRequestValidator>();

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
        options.RemoveServerList();
        options.AddSimpleAuthentication(builder.Configuration);

        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();

        options.AddOperationParameters();
    });
}

builder.Services.AddDataProtection()
    .SetApplicationName(settings.ApplicationName)
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddSingleton(services =>
{
    var dataProtectionProvider = services.GetRequiredService<IDataProtectionProvider>();
    var dataProtector = dataProtectionProvider.CreateProtector(settings.ApplicationName);

    return dataProtector.ToTimeLimitedDataProtector();
});

builder.Services.AddSingleton<IDataProtectionService, DataProtectionService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

builder.Services.AddSingleton<IPageService, PageService>();
builder.Services.AddSingleton<IQRCodeGenerator, QRCodeHandlerGenerator>();

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

    options.AddPolicy("UserActive", policy =>
    {
        policy.Requirements.Add(new UserActiveRequirement());
        policy.RequireRole(RoleNames.User);
    });
});

var azureStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorageConnection");
if (azureStorageConnectionString.HasValue())
{
    builder.Services.AddAzureStorage(options =>
    {
        options.ConnectionString = azureStorageConnectionString;
        options.ContainerName = settings.StorageFolder;
    });
}
else
{
    builder.Services.AddFileSystemStorage(options =>
    {
        options.StorageFolder = settings.StorageFolder;
    });
}

builder.Services.Scan(scan => scan.FromAssemblyOf<IdentityService>()
    .AddClasses(classes => classes.InNamespaceOf<IdentityService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddSingleton<PlaywrightHealthCheck>();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("Database", tags: ["ready"])
    .AddCheck<PlaywrightHealthCheck>("Playwright", tags: ["ready"]);

if (settings.ExecuteStartup)
{
    builder.Services.AddHostedService<IdentityStartupService>();
    builder.Services.AddHostedService<InstallPlaywrightService>();
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
}

builder.Services.AddSingleton<IProductCodeGenerator, ProductCodeGenerator>();
builder.Services.AddSingleton<IPdfGenerator, ChromiumPdfGenerator>();

builder.Services.AddKeyedSingleton<ITemplateEngine, ScribanTemplateEngine>("scriban");

var app = builder.Build();
app.Environment.ApplicationName = settings.ApplicationName;

app.UseHttpsRedirection();

app.UseWhen(context => context.IsWebRequest(), builder =>
{
    if (!app.Environment.IsDevelopment())
    {
        builder.UseExceptionHandler("/Errors/500");
        builder.UseHsts();
    }

    builder.UseStatusCodePagesWithReExecute("/Errors/{0}");
});

app.UseWhen(context => context.IsApiRequest(), builder =>
{
    builder.UseExceptionHandler();
    builder.UseStatusCodePages();
});

app.UseSerilogRequestLogging(options =>
{
    options.IncludeQueryInRequestPath = true;
});

app.UseWebOptimizer();
app.UseStaticFiles();

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

app.UseRouting();
app.UseRequestLocalization();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapEndpoints();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthChecksResponseWriter()
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
    ResponseWriter = HealthChecksResponseWriter()
});

await app.RunAsync();

static Func<HttpContext, HealthReport, Task> HealthChecksResponseWriter()
    => async (context, report) =>
    {
        var result = JsonSerializer.Serialize(
            new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                details = report.Entries.Select(entry => new
                {
                    service = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    exception = entry.Value.Exception?.Message,
                })
            });

        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    };