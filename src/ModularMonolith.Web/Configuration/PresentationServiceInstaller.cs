using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using ModularMonolith.Core.RoleAggregate;
using ModularMonolith.Core.UserAggregate;
using ModularMonolith.Infrastructure.Data;
using Serilog;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace ModularMonolith.Web.Configuration;

public class PresentationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();
        AddCompression(services);
        AddEndpoints(services);
        AddOpenApi(services, configuration);
        AddHealthChecks(services);
        AddHsts(services);
        AddAntiforgeryProtection(services);
        ConfigureCookie(services, configuration);
        AddCors(services, configuration);
        AddLogging(services);
        AddRateLimiting(services);
        AddRequestTimeouts(services);
        AddInternationalization(services);
        AddProblemDetails(services);
        ConfigureHost(services, configuration);
        AddDbContext(services, configuration, environment);
        AddIdentity(services, configuration);
        ConfigureJson(services);
        AddCqrs(services);
    }

    private static void AddCqrs(IServiceCollection services)
    {
        services.AddMediatR(config
            => config.RegisterServicesFromAssembly(UseCases.AssemblyReference.Assembly));
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

    private static void AddIdentity(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

        services.AddAuthentication()
            .AddOpenIdConnect("GitHub", "GitHub Login", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.Authority = "https://github.com";
                options.ClientId = configuration.GetValue<string>("GitHubClientId");
                options.ClientSecret = configuration.GetValue<string>("GitHubClientSecret");
                options.ResponseType = "code";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };
                options.SaveTokens = true;
            });

        services.AddAuthorization(policyBuilder =>
        {

        });
    }

    private static void AddDbContext(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("ApplicationDbContextConnection")
            ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

        services.AddApplicationDbContext(connectionString, environment.IsDevelopment());
    }

    private static void ConfigureHost(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HostOptions>(configuration.GetSection("Host"));
    }
    private static void AddProblemDetails(IServiceCollection services)
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

    private static void AddCompression(IServiceCollection services)
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

    private static void AddInternationalization(IServiceCollection services)
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

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        var clientOrigins = configuration.GetSection("ClientOrigins").Get<string>();

        ArgumentNullException.ThrowIfNull(clientOrigins);

        services.AddCors(options =>
        {
            options.AddPolicy(name: Constants.DefaultCorsPolicy, policy =>
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
        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Cookie:ExpirationTime"));
            options.SlidingExpiration = true;
        });
    }

    private static void AddAntiforgeryProtection(IServiceCollection services)
    {
        services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
    }

    private static void AddEndpoints(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddEndpointsApiExplorer();
    }

    private static void AddHsts(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
        });
    }

    private static void AddOpenApi(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();

        var openApiInfoOptions = configuration.GetSection("OpenApiInfo:V1").Get<OpenApiInfo>()!;

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
            });
            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key access to API",
                Name = "x-api-key",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKey",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    new List<string>()
                },
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey",
                        },
                    },
                    new List<string>()
                },
            });
            options.SwaggerDoc(openApiInfoOptions.Version, openApiInfoOptions);
        });
    }

    private static void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
    }

    private static void AddLogging(IServiceCollection services)
    {
        services.AddSerilog();

        services.AddW3CLogging(logging =>
        {
            logging.LoggingFields = W3CLoggingFields.All;
            logging.AdditionalRequestHeaders.Add("x-forwarded-for");
            logging.AdditionalRequestHeaders.Add("x-client-ssl-protocol");
            logging.FileSizeLimit = 5 * 1024 * 1024;
            logging.RetainedFileCountLimit = 2;
            logging.FileName = "W3CLogs";
            logging.LogDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            logging.FlushInterval = TimeSpan.FromSeconds(2);
        });

        services.AddServiceLogEnricher(options =>
        {
            options.ApplicationName = true;
            options.BuildVersion = true;
            options.DeploymentRing = true;
            options.EnvironmentName = true;
        });

        services.AddRedaction(options =>
        {
            options.SetRedactor<ErasingRedactor>(new DataClassificationSet(ApplicationLoggingTaxonomy.SensitiveData));

            options.SetRedactor<SecretRedactor>(new DataClassificationSet(ApplicationLoggingTaxonomy.PersonalData));
        });

        services.AddScoped<RequestContextLoggingMiddleware>();
    }

    private static void AddRequestTimeouts(IServiceCollection services)
    {
        services.AddRequestTimeouts(options =>
        {
            options.DefaultPolicy = new RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromMilliseconds(2000),
                TimeoutStatusCode = 503,
            };
        });
    }

    private static void AddRateLimiting(IServiceCollection services)
    {
        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.AddFixedWindowLimiter(
                policyName: RateLimitingPolicies.Fixed,
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