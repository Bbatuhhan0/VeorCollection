using Microsoft.EntityFrameworkCore;
using VeorCollection.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=VeorCollectionDb;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. AKILLI KÝMLÝK DOÐRULAMA AYARI (Burasý Deðiþti)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Varsayýlan giriþ sayfasý (Müþteriler için)
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

        // ÖZEL KURAL: Eðer kullanýcý "/Admin" sayfasýna gitmeye çalýþýyorsa...
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                // ...onu Admin Giriþ sayfasýna yönlendir.
                context.Response.Redirect("/Account/AdminLogin");
            }
            else
            {
                // ...deðilse normal müþteri giriþine yönlendir.
                context.Response.Redirect(context.RedirectUri);
            }
            return Task.CompletedTask;
        };
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Sýralama Önemli: Önce Kimlik, Sonra Yetki
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=VeorCollection}/{action=Index}/{id?}");

app.Run();