using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using PolyRushAPI.Helper;
using PolyRushAPI.Models;
using PolyRushAPI.TokenValidators;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

SecretSettings secretSettings = new();
builder.Configuration.Bind("SecretSettings", secretSettings);
builder.Services.AddSingleton(secretSettings);
builder.Services.AddSingleton<RefreshTokenValidator>();
builder.Services.AddScoped<IAuthenticationHelper, AuthenticationHelper>();

// Add services to the container.
//name casing to Pascal Case for the unity deserializer.
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretSettings.TokenSecret)),
            ValidateIssuerSigningKey = true,
            //don't validate client
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.FromMinutes(69420)
        }
    );


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "ApiControllers",
    pattern: "api/{controller}/{action}/{id}");


app.Run();