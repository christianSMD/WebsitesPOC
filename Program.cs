using Eridian_Websites.Data;
using Eridian_Websites.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register custom services
builder.Services.AddSingleton<WpDbService>();
builder.Services.AddSingleton<WooCommerceApiService>();
builder.Services.AddScoped<NucleusService>();

// Register custom controllers
builder.Services.AddControllers();

// Register EF Core DbContext
builder.Services.AddDbContext<NucleusDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NucleusDevelopment")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
