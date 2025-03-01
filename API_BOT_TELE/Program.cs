using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Diagnostics;
using TELEBOT_CSKH;
using TELEBOT_CSKH.Constant;
using TELEBOT_CSKH.Data;
using TELEBOT_CSKH.Middleware.TicketRequestMW;
using TELEBOT_CSKH.Services;
using TELEBOT_CSKH.Services.MediaServices;
using TELEBOT_CSKH.Services.SignalR;
using TELEBOT_CSKH.Services.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IdentityConstant>(builder.Configuration.GetSection("IdentityConstant"));
var baseIdentityConstantOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<IdentityConstant>>().Value;

BuildOidc(builder);
BuildOther(builder);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CORSPolicy", builder => builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(hosts => true));
});
builder.Services.AddCors();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Logging.ClearProviders();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IBaseServices, BaseServices>();
builder.Services.AddScoped<IBOServices, BOServices>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>(); // Đăng ký IService>
var app = builder.Build();
app.UseCors("CORSPolicy");
app.MapHub<SignalRHub>("/hubs/updateState");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API with OIDC v1");

        //// Cấu hình OIDC cho Swagger UI
        //options.OAuthClientId(baseIdentityConstantOptions.ClientId); // Client ID của ứng dụng
        //options.OAuthClientSecret(baseIdentityConstantOptions.ClientSecret); // Client Secret (nếu cần)
        //options.OAuthUsePkce(); // Sử dụng Proof Key for Code Exchange (PKCE)
        //options.OAuthScopeSeparator("api1,openid,profile"); // Phân cách các scope
    });
}

if (app.Environment.IsDevelopment())
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "ngrok",
            Arguments = "http 5000",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    process.Start();
    Thread.Sleep(3000);
}
ApplyMigration();

app.UseMiddleware<TelegramCustomerMiddleware>();
//app.UseMiddleware<RateLimitMiddleware>();
//app.UseMiddleware<CheckAccountMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

static void BuildOidc(WebApplicationBuilder builder)
{
    var baseIdentityConstantOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<IdentityConstant>>().Value;

    builder.Services
        .AddAuthentication("Bearer")
        .AddJwtBearer("Bearer", options =>
        {
            options.Authority = baseIdentityConstantOptions.IdentityUrl;
            options.MetadataAddress = $"{baseIdentityConstantOptions.IdentityUrl}/.well-known/openid-configuration";

            //options.Audience = "api1,livestream"; // Cấu hình audience mong đợi

            var validAudiences = new[] { "CSKH_AUTO", "api1" };
            options.TokenValidationParameters =
                new()
                {
                    //ValidateAudience = false 
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudiences = validAudiences,
                    ValidateLifetime = true, // Kiểm tra thời gian hết hạn
                    ClockSkew = TimeSpan.Zero // Không cho phép chênh lệch thời gian
                };
        });

    builder.Services
        .AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("openid", "profile", "api1", "ofline_access");
            });

        });
}

static void BuildOther(WebApplicationBuilder builder)
{
    var baseIdentityConstantOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<IdentityConstant>>().Value;

    // Đăng ký handler

    //Mapper
    builder.Services.AddSingleton(MappingConfig.RegisterMaps().CreateMapper());
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "API with OIDC", Version = "v1" });

        // Cấu hình OAuth2
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Nhập 'Bearer <token>' vào ô bên dưới. Ví dụ: Bearer abcdef12345"
        });

        // Bảo mật cho các endpoint
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });

    });

}

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
