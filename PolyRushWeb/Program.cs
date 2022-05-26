global using PolyRushLibrary;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
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
string? connectionString = builder.Configuration.GetConnectionString("PolyRushWebConnection");;
ServerVersion? serverVersion = ServerVersion.AutoDetect(connectionString);
builder.Services.AddDbContext<PolyRushWebContext>(options =>
    {
        options.UseMySql(connectionString, serverVersion);
        options.EnableSensitiveDataLogging();
    }
   , ServiceLifetime.Scoped);

builder.Services.AddDbContextFactory<PolyRushWebContext>(options =>
{
    options.UseMySql(connectionString, serverVersion);
}, ServiceLifetime.Scoped);



//Add DA dependencies
builder.Services.AddScoped<UserDA>();
builder.Services.AddScoped<LeaderboardDA>();
builder.Services.AddScoped<ItemDA>();
builder.Services.AddScoped<SettingDA>();
builder.Services.AddScoped<GameSessionDA>();
builder.Services.AddScoped<AchievementDA>();

//configure email
//add emailhelper as a singleton
builder.Services.AddSingleton<EmailHelper>();
//configure fluentemail
SmtpClient? client = new SmtpClient();
client.UseDefaultCredentials = false;
client.Credentials = new NetworkCredential(builder.Configuration["Email:Username"], builder.Configuration["Email:Password"]);
client.Host = builder.Configuration["Email:Host"];
client.Port = Convert.ToInt32(builder.Configuration["Email:Port"]);
client.EnableSsl = Convert.ToBoolean(builder.Configuration["Email:EnableSsl"]);

builder.Services
    .AddFluentEmail(builder.Configuration["Email:Email"], "PolyRush")
    .AddSmtpSender(client);

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
    .AddSignInManager<SignInManager<User>>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider); //TODO Time limit also say in mail!

    SecretSettings secretSettings = new();
builder.Configuration.Bind("SecretSettings", secretSettings);
builder.Services.AddSingleton(secretSettings);
//builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ClientHelper>();
builder.Services.AddScoped<AuthenticationHelper>();

JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
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
builder.Services.AddHttpClient("api", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["Api:Uri"]); 
});

builder.Services.AddSession();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseDeveloperExceptionPage();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();