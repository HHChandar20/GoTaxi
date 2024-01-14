using GoTaxi.BLL.Interfaces;
using GoTaxi.BLL.Services;
using GoTaxi.DAL.Data;
using GoTaxi.DAL.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IClientService, ClientService>();

builder.Services.AddScoped<DriverRepository>();
builder.Services.AddScoped<ClientRepository>();

builder.Services.AddDbContext<GoTaxiDbContext>(options =>
{
    options.UseSqlServer(@"
                Server = .\SQLEXPRESS;
                Database = GoTaxiDB;
                Trusted_Connection=true;
                Integrated Security=true;
                TrustServerCertificate=true");
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();