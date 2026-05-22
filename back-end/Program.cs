using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Watchly.API.Middlewares;
using Watchly.Application.Auth;
using Watchly.Application.Titulos;
using Watchly.Application.UsuarioTitulo;
using Watchly.Infrastructure;
using Watchly.Infrastructure.ExternalApis;

namespace Watchly
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUsuarioTitulo, UsuarioTituloService>();
            builder.Services.AddScoped<ITitulosService, TituloService>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                                                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                    };
                });

            builder.Services.AddHttpClient<TmdbClient>((sp, client) =>
            {
                var key = builder.Configuration["Tmdb:ApiKey"];
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Dev", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddHttpCacheHeaders(
                expirationModelOptionsAction: options =>
                {
                    options.MaxAge = 60;
                    options.CacheLocation = CacheLocation.Public;
                },
                validationModelOptionsAction: options =>
                {
                    options.MustRevalidate = true;
                }
                );

            builder.Services.AddResponseCompression();

            // Add services to the container.

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira apenas o seu token JWT abaixo. Exemplo: eyJhbG..."
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(policyName: "Dev");

            app.UseResponseCompression();

            app.UseHttpCacheHeaders();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
