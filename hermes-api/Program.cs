using hermes_api.DAL;
using idunno.Authentication.Basic;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HermesDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("Entities")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hermes Project",
        Version = "v1"
    });
});

builder.Services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
    .AddBasic(options =>
    {
        options.Realm = "Basic Authentication";
        options.Events = new BasicAuthenticationEvents
        {
            OnValidateCredentials = context =>
            {
                if (context.Username == configuration.GetSection("Authentication:User").Value && context.Password == configuration.GetSection("Authentication:Password").Value)
                {
                    var claims = new[]
                    {
                                    new Claim(ClaimTypes.NameIdentifier, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                                };

                    context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                    context.Success();
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
