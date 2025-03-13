using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using ModularMonolith.Users.Core.Outbox;
using ModularMonolith.Users.Core.RoleAggregate;
using ModularMonolith.Users.Core.UserAggregate;
using ModularMonolith.Users.Infrastructure;
using ModularMonolith.Users.Infrastructure.Data;
using ModularMonolith.Users.Web;
using ModularMonolith.Web.Metrics;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using SharedKernel;

namespace ModularMonolith.Web.Configuration;

internal static class ServiceInstaller
{
    internal static void Install(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        ConfigureCompression(services);
        ConfigureEndpoints(services);
        ConfigureOpenApi(services, configuration);
        ConfigureHsts(services);
        ConfigureAntiforgeryProtection(services);
        ConfigureCors(services, configuration);
        ConfigureDiagnostics(services, configuration, environment);
        ConfigureResiliency(services);
        ConfigureInternationalization(services);
        ConfigureErrorHandling(services);
        ConfigureDbContext(services, configuration, environment);
        ConfigureIdentity(services);
        ConfigureCookie(services);
        ConfigureJson(services);
        ConfigureCache(services);
        ConfigureMessaging(services, configuration);
        ConfigureEmail(services, configuration);
        ConfigureCqrs(services);
        ConfigureHealthChecks(services);
        ConfigureHost(services, configuration);

        services.AddFeatureManagement(configuration.GetSection("FeatureFlags"));
    }

    private static void ConfigureDbContext(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' not found.");

        services.ConfigureUsersDbContext(connectionString, environment.IsDevelopment());

        services.AddScoped<IOutboxProcessor, OutboxProcessor>();

        services.AddHangfire(options =>
        {
            options.UseInMemoryStorage();
        });

        services.AddHangfireServer(options =>
        {
            options.SchedulePollingInterval = TimeSpan.FromSeconds(10);
        });
    }

    private static void ConfigureCqrs(IServiceCollection services)
    {
        services.AddMediatR(config
            => config.RegisterServicesFromAssemblies(Users.UseCases.AssemblyReference.Assembly));
    }

    private static void ConfigureHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<UsersDbContext>();
    }

    private static void ConfigureEmail(IServiceCollection services, IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("Email").Get<EmailSettings>();
        ArgumentNullException.ThrowIfNull(emailSettings);
        services.AddFluentEmail(emailSettings.SenderEmail, emailSettings.Sender)
                .AddSmtpSender(emailSettings.Host, emailSettings.Port);

        services.AddScoped<IEmailSender, EmailSender>();
    }

    private static void ConfigureMessaging(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MessageBrokerSettings>()
            .Bind(configuration.GetSection(MessageBrokerSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<MessageBrokerSettings>, ValidateMessageBrokerSettings>();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MessageBrokerSettings>>().Value);

        services.AddMassTransit(options =>
        {
            options.SetKebabCaseEndpointNameFormatter();

            options.AddConsumers(AssemblyReference.Assembly);

            options.UsingRabbitMq((context, config) =>
            {
                var settings = context.GetRequiredService<MessageBrokerSettings>();

                config.Host(new Uri(settings.Host), h =>
                {
                    h.Username(settings.Username);
                    h.Password(settings.Password);
                });

                config.ConfigureEndpoints(context);
            });
        });
    }

    private static void ConfigureCache(IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            options.AddPolicy(Policies.OpenApiCachePolicy, policy => policy.Expire(TimeSpan.FromMinutes(1)));
        });

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
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
        IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = true;
            options.ClaimsIdentity.UserIdClaimType = JwtRegisteredClaimNames.Sub;
            options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Name;
            options.ClaimsIdentity.RoleClaimType = IdentityClaimNames.Role;
            options.ClaimsIdentity.EmailClaimType = JwtRegisteredClaimNames.Email;
        })
           .AddEntityFrameworkStores<UsersDbContext>()
           .AddDefaultTokenProviders();

        services.AddAuthorization(policyBuilder =>
        {
            //policyBuilder.DefaultPolicy = new AuthorizationPolicyBuilder()
            //    .RequireAuthenticatedUser()
            //    .Build();

            //policyBuilder.FallbackPolicy = policyBuilder.DefaultPolicy;

            policyBuilder.AddPolicy(
                Policies.ApiTesterPolicy,
                policy => policy.RequireRole(ApplicationRoles.Administrator));
        });

        services.AddSingleton(
            _ => Channel.CreateUnbounded<EmailRequest>(new UnboundedChannelOptions
            {
                SingleReader = true,
                AllowSynchronousContinuations = false,
            }));

        services.AddHostedService<EmailBackgroundProcessor>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
            options.LogoutPath = "/Identity/Account/Logout";
            options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        });
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

    private static void ConfigureCookie(IServiceCollection services)
    {
        services.AddSession();


    }

    private static void ConfigureAntiforgeryProtection(IServiceCollection services)
    {
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-XSRF-TOKEN";
            options.SuppressXFrameOptionsHeader = true;
        });
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
            options.UseDocumentDetails(openApiInfoOptions)
                .UseBearerSecurityScheme()
                .UseApiKeySecurityScheme();
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(openApiInfoOptions.Version, openApiInfoOptions);
        });
    }

    private static void ConfigureDiagnostics(
        IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddSerilog((services, options) =>
        {
            options.ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services)
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

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                        DiagnosticsConfiguration.ServiceName,
                        serviceInstanceId: Environment.MachineName)
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["service.name"] = "ModularMonolith",
                        ["machine.name"] = Environment.MachineName,
                        // endpoint and protocol are optional
                    });

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (environment is not null)
                {
                    resource.AddAttributes(new Dictionary<string, object>
                    {
                        ["environment.name"] = environment,
                    });
                }
            })
            .WithMetrics(b =>
            {
                b.AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(
                        DiagnosticsConfiguration.Meter.Name,
                        MassTransit.Monitoring.InstrumentationOptions.MeterName, // MassTransit Meter
                        "Microsoft.AspNetCore.Hosting",
                        "System.Net.Http",
                        "Microsoft.AspNetCore.Server.Kestrel",
                        "ModularMonolith.Web")
                    .AddOtlpExporter();
            })
            .WithTracing(b =>
            {
                b.AddSource(DiagnosticsConfiguration.Source.Name)
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
                    .AddNpgsql()
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();

                if (environment.IsDevelopment())
                {
                    b.SetSampler<AlwaysOnSampler>();
                }
            });
    }

    private static void ConfigureResiliency(IServiceCollection services)
    {
        services.AddLoadShedding((_, options) =>
        {
            options.SubscribeEvents(events =>
            {
                events.ItemEnqueued.Subscribe(LoadShedding.SubscribeToItemEnqueued);
                events.ItemDequeued.Subscribe(LoadShedding.SubscribeToItemDequeued);
                events.ItemProcessing.Subscribe(LoadShedding.SubscribeToItemProcessing);
                events.ItemProcessed.Subscribe(LoadShedding.SubscribeToItemProcessed);
                events.Rejected.Subscribe(LoadShedding.SubscribeToRejected);
            });
        });

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

    private static void ConfigureHost(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HostOptions>(configuration.GetSection("Host"));

        services.AddScoped<ContentTypeOptionsMiddleware>();
    }
}
