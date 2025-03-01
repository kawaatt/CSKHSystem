using Microsoft.AspNetCore.Authentication.Cookies;
using SHBET_CLIENT.Constant;
using SHBET_CLIENT.Middleware;
using SHBET_CLIENT.Provider;
using SHBET_CLIENT.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IdentityConstant>(builder.Configuration.GetSection("IdentityConstant"));
builder.Services.Configure<ApiEndPointConstant>(builder.Configuration.GetSection("ApiEndPointConstant"));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddDistributedMemoryCache(); // Cung c?p b? nh? cho Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(2); // Th?i gian h?t h?n c?a session
    options.Cookie.HttpOnly = true; // ??m b?o cookie không th? truy c?p t? JavaScript
    options.Cookie.IsEssential = true; // ??m b?o cookie ???c g?i ngay c? khi ng??i dùng không ??ng ý cookies
});
builder.Services.AddMemoryCache();
//builder.Services.AddScoped<ITicketStore, MemoryCacheTicketStore>();

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseServices, BaseServices>();
builder.Services.AddScoped<IHomeServices, HomeServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ITicketServices, TicketServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMiddleware<TokenProcessingMiddleware>();
//app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();
