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
SmtpClient? client = new();
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
    //Set the role interface
    .AddRoles<IdentityRole<int>>()
    //set the dbcontext
    .AddEntityFrameworkStores<PolyRushWebContext>()
    //add the signinmanager
    .AddSignInManager<SignInManager<User>>()
    //Add tokenprovider so tokens can be generated
    .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider); 

//Class to get secret settings
SecretSettings secretSettings = new();
builder.Configuration.Bind("SecretSettings", secretSettings);
builder.Services.AddSingleton(secretSettings);

//add Ihttpcontextaccessor & ClientHelper as singleton
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ClientHelper>();
//add authenticationhelper as scoped
builder.Services.AddScoped<AuthenticationHelper>();

//ignore self referencing loops
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
};
//add newtonsoftjson as json serializer
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
});


//add jwt authentication for server side validation
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

//add session so session variables can be used
builder.Services.AddSession();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //set error page
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//enable session
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//make sure error page shows up when there is an error
app.UseDeveloperExceptionPage();

//use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

//map the default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

//Start the app
app.Run();