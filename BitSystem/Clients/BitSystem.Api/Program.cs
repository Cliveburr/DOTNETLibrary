
using BitSystem.Api.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddSingleton<TokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        var key = Encoding.UTF8.GetBytes("TODOMOVEPRECISASERUMTEXTOUMPOUCOMAIOR");

        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true,
            ValidIssuer = "API",
            ValidateIssuer = true,
            ValidAudience = "API",
            ValidateAudience = true,
            ValidateLifetime = false
        };
    });











var app = builder.Build();

//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
