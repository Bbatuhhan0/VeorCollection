using Microsoft.EntityFrameworkCore;
using VeorCollection.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý (SQL Server)
// Not: Baðlantý adresini kendi bilgisayarýna göre ayarlayacaðýz.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=VeorCollectionDb;Trusted_Connection=True;MultipleActiveResultSets=true"; builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Statik dosyalarý (css, js, img) wwwroot'tan sunmak için:
app.UseStaticFiles();
// .NET 9 kullanýyorsan MapStaticAssets kalabilir ama UseStaticFiles garantidir.
// app.MapStaticAssets(); 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=VeorCollection}/{action=Index}/{id?}");
// .WithStaticAssets(); // Eðer .NET 9 deðilse bu satýrý kaldýrabilirsin.

app.Run();