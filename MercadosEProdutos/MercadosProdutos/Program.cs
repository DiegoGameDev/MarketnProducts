using DBContext;
using DBModel;
using Repository;
using Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// definindo a rota junto ao banco de dados no serviço
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

builder.Services.AddDbContext<MarketDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<MarketDBContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
   opt.LoginPath = "/Login"; 
});

builder.Services.AddTransient<IEmailSender, EmailSender>(); // adicionando o enviador de email na service

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(12);
});

builder.Services.AddScoped<IMarketRepository, MarketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMarketSession, MarketSession>();
builder.Services.AddScoped<IMarketAssociatedRepository, MarketAssociatedRepository>();


builder.Services.AddSession(x =>
{
    x.Cookie.HttpOnly = true;
    x.Cookie.IsEssential = true;
}
);

var app = builder.Build();

#region  CreateFirstAdminUser
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string email = "admin@market.com";
    string password = "Admin123!";

    // Criar role se não existir
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Verificar se usuário já existe
    var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        var newUser = new User
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newUser, password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, "Admin");
        }
    }
}
#endregion

#region Scope de roles
using (var scope = app.Services.CreateScope()) 
{
    var rolemanager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = {"Default", "Associated", "Admin"};

    foreach (var role in roles)
    {
        if (!await rolemanager.RoleExistsAsync(role))  // adiciona as roles caso não exista
        {
            await rolemanager.CreateAsync(new IdentityRole(role));
        }

    }
}
#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
