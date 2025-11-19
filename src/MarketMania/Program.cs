using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using FluentValidation;
using MarketMania.Authentication;
using MarketMania.Authentication.DataProtection;
using MarketMania.Authentication.DataProtection.Interfaces;
using MarketMania.Authentication.Entities;
using MarketMania.BusinessLayer.Extensions;
using MarketMania.BusinessLayer.Generators;
using MarketMania.BusinessLayer.Publishers;
using MarketMania.BusinessLayer.Services;
using MarketMania.BusinessLayer.Settings;
using MarketMania.BusinessLayer.Validations;
using MarketMania.Clients.Extensions;
using MarketMania.Contracts;
using MarketMania.DataAccessLayer;
using MarketMania.DataAccessLayer.Caching;
using MarketMania.DataAccessLayer.Caching.Interfaces;
using MarketMania.Extensions;
using MarketMania.HealthChecks;
using MarketMania.Requirements;
using MarketMania.Security;
using MarketMania.Services;
using MarketMania.Startup;
using MarketMania.StorageProviders.Extensions;
using MarketMania.Swagger;
using MarketMania.TimeZoneProvider;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MinimalHelpers.Routing;
using MinimalHelpers.Validation;
using OperationResults.AspNetCore.Http;
using Serilog;
using SimpleAuthentication;
using SimpleTransit;
using TinyHelpers.AspNetCore.Extensions;
using TinyHelpers.AspNetCore.OpenApi;
using TinyHelpers.Extensions;
using ResultErrorResponseFormat = OperationResults.AspNetCore.Http.ErrorResponseFormat;
using ValidationErrorResponseFormat = MinimalHelpers.Validation.ErrorResponseFormat;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", true, true);

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new AppSettings();
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new SwaggerSettings();

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRequestLocalization(settings.SupportedCultures);
builder.Services.AddWebOptimizer(minifyCss: true, minifyJavaScript: builder.Environment.IsProduction());

builder.Services.AddDefaultExceptionHandler();
builder.Services.AddDefaultProblemDetails();

builder.Services.AddRequestTimeouts();
builder.Services.AddTimeZoneProvider();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(settings.ApplicationName, context =>
    {
        var permitLimit = int.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.PermitLimit)?.Value, out var requestsPerWindow) ? requestsPerWindow : 3;
        var window = int.TryParse(context.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Window)?.Value, out var windowMinutes) ? TimeSpan.FromMinutes(windowMinutes) : TimeSpan.FromMinutes(1);

        return RateLimitPartition.GetFixedWindowLimiter(context.User.Identity?.Name.GetValueOrDefault("Default"), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = permitLimit,
            Window = window,
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var window))
        {
            var response = context.HttpContext.Response;
            response.Headers.RetryAfter = window.TotalSeconds.ToString();
        }

        return ValueTask.CompletedTask;
    };
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.ConfigureFormOptions(options =>
{
    options.MultipartBodyLengthLimit = settings.MaxUploadSize;
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
    builder.Services.AddOpenApiOperationParameters(options =>
    {
        options.Parameters.Add(new()
        {
            Name = TimeZoneService.HeaderKey,
            In = ParameterLocation.Header,
            Required = false,
            Schema = OpenApiSchemaHelper.CreateStringSchema()
        });
    });

    builder.Services.AddOpenApi(options =>
    {
        options.RemoveServerList();
        options.AddSimpleAuthentication(builder.Configuration);

        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();

        options.AddOperationParameters();
    });
}

builder.Services.AddSingleton<IPageService, PageService>();
builder.Services.AddEmailClient(builder.Configuration);

builder.Services.AddSentimentApi(builder.Configuration);
builder.Services.AddPdfSmith(builder.Configuration);

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(settings.MaxRetryCount, settings.MaxRetryDelay, null);
        sqlOptions.CommandTimeout(settings.CommandTimeout);
    });
});

builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("SqlConnection");
    options.SchemaName = "dbo";
    options.TableName = "CacheStore";
    options.DefaultSlidingExpiration = settings.DefaultSlidingExpiration;
});

builder.Services.AddSingleton<IDataContextCache, DataContextDistributedCache>();

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

builder.Services.AddDataProtection()
    .SetApplicationName(settings.ApplicationName)
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddSingleton(services =>
{
    var dataProtectionProvider = services.GetRequiredService<IDataProtectionProvider>();
    var dataProtector = dataProtectionProvider.CreateProtector(settings.ApplicationName);

    return dataProtector;
});

builder.Services.AddSingleton(services =>
{
    var dataProtector = services.GetRequiredService<IDataProtector>();
    return dataProtector.ToTimeLimitedDataProtector();
});

builder.Services.AddSingleton<IDataProtectionService, DataProtectionService>();
builder.Services.AddSingleton<ITimeLimitedDataProtectionService, TimeLimitedDataProtectionService>();

//builder.Services.AddScoped<AuthenticationDbContext>(services => services.GetRequiredService<ApplicationDbContext>());
//builder.Services.AddTransient<IApiKeyValidator, SubscriptionValidator>();

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
        options.ContainerName = settings.StorageFolder ?? string.Empty;
    });
}
else
{
    builder.Services.AddFileSystemStorage(options =>
    {
        options.StorageFolder = settings.StorageFolder ?? AppContext.BaseDirectory;
    });
}

builder.Services.Scan(scan => scan.FromAssemblyOf<BarcodeGenerator>()
    .AddClasses(classes => classes.InNamespaceOf<BarcodeGenerator>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.Scan(scan => scan.FromAssemblyOf<IdentityService>()
    .AddClasses(classes => classes.InNamespaceOf<IdentityService>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());

builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>("Database", tags: ["ready"]);

if (settings.ExecuteStartup)
{
    builder.Services.AddHostedService<DatabaseInitializerService>();
    builder.Services.AddHostedService<IdentityStartupService>();
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
}

builder.Services.AddEncryption(builder.Configuration);

builder.Services.AddSimpleTransit(options =>
{
    options.RegisterServicesFromAssemblyContaining<UserRegistratedPublisher>();
});

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

app.UseWhen(context => context.IsApiRequest(), builder =>
{
    builder.UseAuthentication();
    builder.UseAuthorization();

    builder.UseRateLimiter();
    builder.UseRequestTimeouts();
});

app.MapRazorPages();
app.MapEndpoints();

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthCheckHelper.HealthChecksResponseWriter()
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
    ResponseWriter = HealthCheckHelper.HealthChecksResponseWriter()
});

await app.RunAsync();