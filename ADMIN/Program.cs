using ADMIN.AuthHandler;
using ADMIN.Constant;
using ADMIN.Middleware;
using ADMIN.Provider;
using ADMIN.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký cấu hình
builder.Services.Configure<IdentityConstant>(builder.Configuration.GetSection("IdentityConstant"));
builder.Services.Configure<ApiEndPointConstant>(builder.Configuration.GetSection("ApiEndPointConstant"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();

ConfigJson(builder);


BuildOidc(builder);
RegisPolicyPermission(builder);
BuildOtherServive(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

useMidlleWare(app);

app.MapGet("/SignOutCallback", (HttpContext httpContext) =>
{
    // Xử lý callback sau khi logout
    httpContext.Response.Redirect("/");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


static void BuildOidc(WebApplicationBuilder builder)
{
    var baseIdentityConstantOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<IdentityConstant>>().Value;

    builder.Services.AddDistributedMemoryCache(); // Cung cấp bộ nhớ cho Session
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromDays(365); // Thời gian hết hạn của session
        options.Cookie.HttpOnly = true; // Đảm bảo cookie không thể truy cập từ JavaScript
        options.Cookie.IsEssential = true; // Đảm bảo cookie được gửi ngay cả khi người dùng không đồng ý cookies
    });
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ITicketStore, MemoryCacheTicketStore>();

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultScheme = baseIdentityConstantOptions.DefaultScheme;
            options.DefaultChallengeScheme = baseIdentityConstantOptions.OidcScheme;
        })
        .AddCookie(baseIdentityConstantOptions.DefaultScheme, options =>
        {
            // Cấu hình cookie authentication
            options.ExpireTimeSpan = TimeSpan.FromDays(365); // Cookie tồn tại lâu hơn AccessToken
            options.SlidingExpiration = true;
            options.Cookie.MaxAge = TimeSpan.FromDays(365);
            //options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            // Sử dụng TicketStore để lưu session
            options.SessionStore = builder.Services.BuildServiceProvider().GetRequiredService<ITicketStore>();
        })
        .AddOpenIdConnect(baseIdentityConstantOptions.OidcScheme, options =>
        {
            //options.Authority = "https://dev-idserver.attcloud2.win";
            options.Authority = baseIdentityConstantOptions.IdentityUrl;
            options.ClientId = baseIdentityConstantOptions.ClientId;
            options.ClientSecret = baseIdentityConstantOptions.ClientSecret;
            options.ResponseType = "code";

            options.Scope.Add("api1");
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access"); // Để yêu cầu Refresh Token

            options.SignedOutRedirectUri = baseIdentityConstantOptions.SignOutCallBack;


            options.SaveTokens = true;
            options.UsePkce = true; // Kích hoạt PKCE
            options.GetClaimsFromUserInfoEndpoint = true;

            options.ClaimActions.MapJsonKey(ClaimConstant.TenantId, ClaimConstant.TenantId); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.TenantName, ClaimConstant.TenantName); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.UserName, ClaimConstant.UserName); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.UserFullInfor, ClaimConstant.UserFullInfor); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.Permissions, ClaimConstant.Permissions); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.Role, ClaimConstant.Role); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.RoleFullInfor, ClaimConstant.RoleFullInfor); // Ánh xạ claim permission
            options.ClaimActions.MapJsonKey(ClaimConstant.UserAllocationData, ClaimConstant.UserAllocationData); // Ánh xạ claim userallow
            options.ClaimActions.MapJsonKey(ClaimConstant.UserId, ClaimConstant.UserId); // Ánh xạ claim userallow

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = context =>
                {
                    // Lưu token vào session khi xác thực thành công
                    var accessToken = context.TokenEndpointResponse.AccessToken;
                    var refreshToken = context.TokenEndpointResponse.RefreshToken;
                    var idToken = context.TokenEndpointResponse.IdToken;

                    // Lưu access token và refresh token vào Session
                    context.HttpContext.Session.SetString("access_token", accessToken);
                    context.HttpContext.Session.SetString("refresh_token", refreshToken);
                    context.HttpContext.Session.SetString("id_token", idToken);

                    return Task.CompletedTask;
                },

                OnSignedOutCallbackRedirect = context =>
                {
                    // Chuyển hướng sau khi logout
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                },
            };

            options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = async context =>
                {
                    var logoutUri = $"{options.Authority}/connect/endsession";

                    // Append id_token_hint if available
                    var idToken = await context.HttpContext.GetTokenAsync("id_token");
                    if (idToken != null)
                    {
                        logoutUri = QueryHelpers.AddQueryString(logoutUri, "id_token_hint", idToken);
                    }

                    context.Response.Redirect(logoutUri);
                    context.HandleResponse();
                }
            };
        });
}

static void RegisPolicyPermission(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(PolicyConstant.F168_TICKET_VIEW, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_VIEW)));
        options.AddPolicy(PolicyConstant.F168_TICKET_UPDATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_UPDATE)));
        options.AddPolicy(PolicyConstant.F168_TICKET_DELETE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_DELETE)));
        options.AddPolicy(PolicyConstant.F168_TICKET_CREATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_CREATE)));
        options.AddPolicy(PolicyConstant.F168_TICKET_SETTING, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_SETTING)));

        options.AddPolicy(PolicyConstant.F168_TELE_BOT_VIEW, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TELE_BOT_VIEW)));
        options.AddPolicy(PolicyConstant.F168_TELE_BOT_CREATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_UPDATE)));
        options.AddPolicy(PolicyConstant.F168_TELE_BOT_DELETE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TICKET_DELETE)));
        options.AddPolicy(PolicyConstant.F168_TELE_BOT_UPDATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TELE_BOT_UPDATE)));
        options.AddPolicy(PolicyConstant.F168_TELE_BOT_UPDATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_TELE_BOT_UPDATE)));

        options.AddPolicy(PolicyConstant.F168_DEPOSIT_VIEW, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_DEPOSIT_VIEW)));
        options.AddPolicy(PolicyConstant.F168_DEPOSIT_UPDATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_DEPOSIT_UPDATE)));
        options.AddPolicy(PolicyConstant.F168_DEPOSIT_DELETE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_DEPOSIT_DELETE)));
        options.AddPolicy(PolicyConstant.F168_DEPOSIT_CREATE, policy =>
            policy.Requirements.Add(new PermissionRequirement(PolicyConstant.F168_DEPOSIT_CREATE)));

        //options.AddPolicy(PolicyConstant.SocialLoginView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.SocialLoginView)));
        //options.AddPolicy(PolicyConstant.SocialLoginAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.SocialLoginAddAndEdit)));
        //options.AddPolicy(PolicyConstant.SocialLoginDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.SocialLoginDelete)));

        //options.AddPolicy(PolicyConstant.DashBoardView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.DashBoardView)));
        //options.AddPolicy(PolicyConstant.DashBoardAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.DashBoardAddAndEdit)));
        //options.AddPolicy(PolicyConstant.DashBoardDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.DashBoardDelete)));

        //options.AddPolicy(PolicyConstant.EmployeeView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.EmployeeView)));
        //options.AddPolicy(PolicyConstant.EmployeeAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.EmployeeAddAndEdit)));
        //options.AddPolicy(PolicyConstant.EmployeeDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.EmployeeDelete)));

        //options.AddPolicy(PolicyConstant.ConfigStreamView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.ConfigStreamView)));
        //options.AddPolicy(PolicyConstant.ConfigStreamAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.ConfigStreamAddAndEdit)));
        //options.AddPolicy(PolicyConstant.ConfigStreamDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.ConfigStreamDelete)));

        //options.AddPolicy(PolicyConstant.LiveStreamView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.LiveStreamView)));
        //options.AddPolicy(PolicyConstant.LiveStreamAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.LiveStreamAddAndEdit)));
        //options.AddPolicy(PolicyConstant.LiveStreamDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.LiveStreamDelete)));

        //options.AddPolicy(PolicyConstant.PlayerView, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.PlayerView)));
        //options.AddPolicy(PolicyConstant.PlayerAddAndEdit, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.PlayerAddAndEdit)));
        //options.AddPolicy(PolicyConstant.PlayerDelete, policy =>
        //    policy.Requirements.Add(new PermissionRequirement(PolicyConstant.PlayerDelete)));


    });
}

static void BuildOtherServive(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ITokenProvider, TokenProvider>();
    builder.Services.AddScoped<IAuthServices, AuthServices>();
    builder.Services.AddScoped<IBaseServices,BaseServices>();
    builder.Services.AddScoped<IUserContextService, UserContextService>();
    builder.Services.AddScoped<ITicketServices, TicketServices>();
    builder.Services.AddScoped<ITeleBotService, TeleBotService>();
}

static void ConfigJson(WebApplicationBuilder builder)
{

    // Hoặc cấu hình cho API Controllers nếu bạn đang sử dụng Web API
    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;  // Bỏ qua các giá trị null
    });
}

static void useMidlleWare(WebApplication app)
{
    app.UseMiddleware<TokenProcessingMiddleware>();
    app.UseMiddleware<ErrorHandlingMiddleware>(); 
}