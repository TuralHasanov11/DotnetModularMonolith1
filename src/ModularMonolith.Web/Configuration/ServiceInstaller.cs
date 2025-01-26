using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using ModularMonolith.Users.Core.RoleAggregate;
using ModularMonolith.Users.Core.UserAggregate;
using ModularMonolith.Users.Infrastructure.Data;
using ModularMonolith.Users.Web;
using Serilog;
using SharedKernel;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace ModularMonolith.Web.Configuration;

internal static class ServiceInstaller
{
    internal static void Install(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.InstallUsersModuleServices(configuration, environment.IsDevelopment());

        ConfigureCompression(services);
        ConfigureEndpoints(services);
        ConfigureOpenApi(services, configuration);
        ConfigureHsts(services);
        ConfigureAntiforgeryProtection(services);
        ConfigureCookie(services, configuration);
        ConfigureCors(services, configuration);
        ConfigureDiagnostics(services, configuration);
        ConfigureResiliency(services);
        ConfigureInternationalization(services);
        ConfigureErrorHandling(services);
        ConfigureHost(services, configuration);
        ConfigureIdentity(services, configuration);
        ConfigureJson(services);
        ConfigureCache(services);
    }

    private static void ConfigureCache(IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy(Policies.OpenApiCachePolicy, policy => policy.Expire(TimeSpan.FromMinutes(1)));
        });
    }

    private static void ConfigureJson(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.WriteIndented = false;
            options.SerializerOptions.Encoder = JavaScriptEncoder.Default;
            options.SerializerOptions.AllowTrailingCommas = true;
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        });
    }

    private static void ConfigureIdentity(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
            options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Name;
            options.ClaimsIdentity.RoleClaimType = IdentityClaimNames.Role;
            options.ClaimsIdentity.EmailClaimType = JwtRegisteredClaimNames.Email;
        })
           .AddEntityFrameworkStores<UsersDbContext>()
           .AddDefaultTokenProviders();

        services.AddAuthentication();

        services.AddAuthorization(policyBuilder =>
        {
            policyBuilder.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            policyBuilder.FallbackPolicy = policyBuilder.DefaultPolicy;
        });
    }

    private static void ConfigureHost(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HostOptions>(configuration.GetSection("Host"));
    }
    private static void ConfigureErrorHandling(IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        services.AddExceptionHandler<ProblemExceptionHandler>();
    }

    private static void ConfigureCompression(IServiceCollection services)
    {
        services.AddRequestDecompression();

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.SmallestSize);
    }

    private static void ConfigureInternationalization(IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        string[] supportedCultures = ["en-US", "de-DE"];

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            options.ApplyCurrentCultureToResponseHeaders = true;
        });
    }

    private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var clientOrigins = configuration.GetSection("ClientOrigins").Get<string>();

        ArgumentNullException.ThrowIfNull(clientOrigins);

        services.AddCors(options =>
        {
            options.AddPolicy(name: Policies.DefaultCorsPolicy, policy =>
            {
                policy.WithOrigins(clientOrigins.Split(','))
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    private static void ConfigureCookie(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSession();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Cookie:ExpirationTime"));
            options.SlidingExpiration = true;
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.LogoutPath = "/Identity/Account/Logout";
        });
    }

    private static void ConfigureAntiforgeryProtection(IServiceCollection services)
    {
        services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
    }

    private static void ConfigureEndpoints(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddControllersWithViews();
        services.AddEndpointsApiExplorer();
    }

    private static void ConfigureHsts(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
        });
    }

    private static void ConfigureOpenApi(IServiceCollection services, IConfiguration configuration)
    {
        var openApiInfoOptions = configuration.GetSection("OpenApiInfo:v1").Get<OpenApiInfo>()!;

        services.AddOpenApi(options =>
        {
            options.UseDocumentDetails(openApiInfoOptions);
            options.UseBearerSecurityScheme();
            options.UseApiKeySecurityScheme();
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(openApiInfoOptions.Version, openApiInfoOptions);
        });
    }

    private static void ConfigureDiagnostics(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((_, config) =>
        {
            config.ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext();
        });

        services.AddServiceLogEnricher(options =>
        {
            options.ApplicationName = true;
            options.BuildVersion = true;
            options.DeploymentRing = true;
            options.EnvironmentName = true;
        });

        services.AddStaticLogEnricher<MachineNameEnricher>();

        services.AddRedaction(options =>
        {
            options.SetRedactor<ErasingRedactor>(new DataClassificationSet(ApplicationLoggingTaxonomy.SensitiveData));

            options.SetRedactor<SecretRedactor>(new DataClassificationSet(ApplicationLoggingTaxonomy.PersonalData));
        });

        services.AddScoped<RequestContextLoggingMiddleware>();
        services.AddScoped<RequestTimeLoggingMiddleware>();
    }

    private static void ConfigureResiliency(IServiceCollection services)
    {
        services.AddRequestTimeouts(options =>
        {
            options.DefaultPolicy = new RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromMilliseconds(2000),
                TimeoutStatusCode = 503,
            };
        });

        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.AddFixedWindowLimiter(
                policyName: Policies.FixedRateLimitingPolicy,
                options =>
                {
                    options.PermitLimit = 4;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 2;
                });
        });
    }
}
