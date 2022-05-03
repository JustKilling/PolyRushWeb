global using PolyRushLibrary;
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using PolyRushWeb.Helper;
using PolyRushWeb.Models;
using Microsoft.EntityFrameworkCore;
using PolyRushWeb.Data;
using PolyRushWeb.DA;
using Newtonsoft.Json;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PolyRushWebConnection");;

builder.Services.AddDbContext<PolyRushWebContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("5.7")), ServiceLifetime.Transient);
builder.Services.AddDbContext<polyrushContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.Parse("5.7"));
}, ServiceLifetime.Transient);
builder.Services.AddDbContextFactory<polyrushContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.Parse("5.7"));
}, ServiceLifetime.Transient);



//Add DA dependencies
builder.Services.AddTransient<UserDA>();
builder.Services.AddTransient<LeaderboardDA>();
builder.Services.AddTransient<ItemDA>();
builder.Services.AddTransient<SettingDA>();
builder.Services.AddTransient<GameSessionDA>();

//Add identity
builder.Services.AddIdentityCore<User>(options =>
{
   
    //Password settings
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 7;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;

})
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<PolyRushWebContext>()
    .AddSignInManager<SignInManager<User>>();

SecretSettings secretSettings = new();
builder.Configuration.Bind("SecretSettings", secretSettings);
builder.Services.AddSingleton(secretSettings);
//builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new()
        {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretSettings.TokenSecret)),
        ValidateIssuerSigningKey = true,
        //don't validate client.
        ValidateAudience = false,
        ValidateIssuer = false,
    }
);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add progressive web app
builder.Services.AddProgressiveWebApp();

//Add httpclient
builder.Services.AddHttpClient("polyrush", httpClient =>
{
    httpClient.BaseAddress = new(builder.Configuration["Api:Uri"]); 
});

WebApplication app = builder.Build();

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



app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();