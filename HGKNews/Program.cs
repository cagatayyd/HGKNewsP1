using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using HGKNews.Context;
using HGKNews.Factories;
using HGKNews.Factories.Abstract;
using HGKNews.Services;
using HGKNews.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<NewsDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionC")));

//Service
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));
builder.Services.AddAutoMapper(typeof(NewsRepository).Assembly);

//Factory
builder.Services.AddScoped<INewsItemModelFactory, NewsItemModelFactory>();

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
