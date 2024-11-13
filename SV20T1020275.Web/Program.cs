using Microsoft.AspNetCore.Authentication.Cookies;
using SV20T1020275.Web;

var builder = WebApplication.CreateBuilder(args);

//Add c�c Services c?n d�ng trong Application
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews().AddMvcOptions(option =>
{
    //Chuy?n t? th�ng b�o l?i m?c ??nh sang th�ng b�o ?� ???c c�i ??t
    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "AuthenticationCookie";
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/AccessDenined";
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(120);
                });

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    hostEnvironment: app.Services.GetService<IWebHostEnvironment>()
);

string connectionString = "server=DESKTOP-DABCK0S\\SQLEXPRESS; user id=sa; password=123; database=LiteCommerceDB; TrustServerCertificate=true";
SV20T1020275.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();