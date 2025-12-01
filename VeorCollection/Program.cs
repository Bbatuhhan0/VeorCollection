using Microsoft.EntityFrameworkCore;
using VeorCollection.Data;
using Microsoft.AspNetCore.Authentication.Cookies; // Bu gerekli

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=VeorCollectionDb;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Kimlik Doðrulama Servisini Ekliyoruz (YENÝ)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Giriþ yapmamýþ kiþi buraya atýlsýn
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // 20 dk sonra oturum düþsün
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

// 3. Sýralama ÇOK ÖNEMLÝ: Önce Kimlik Doðrulama, Sonra Yetkilendirme (YENÝ)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=VeorCollection}/{action=Index}/{id?}");

app.Run();