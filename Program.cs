using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Data;
using ApiEcommerce.Mapping;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSQL");
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(dbConnectionString));

//Codigo para seed de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      // Seeding de Roles
      DataSeeder.SeedData(appContext);
  })
);

//codigo de cache
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configurar Mapster con los perfiles de mapeo
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(CategoryProfile).Assembly);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

//codigo para .Net Indentity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


//codigo para usar JWT en el API
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("Secret key no configurada");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; //desactiva el pedir https
    options.SaveToken = true; //guarda en token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, //el token debe estar firmado con clave valida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? "")), //con UTF8
        ValidateIssuer = false, //no se valide el emisor del token
        ValidateAudience = false //no se valida el publico si no se restringe a ciertos clientes
    };
});

builder.Services.AddControllers(

//profiles de cache que se pueden usar en los controladores
option =>
{
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);

    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
}

);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


builder.Services.AddEndpointsApiExplorer();

//Authorization code
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. \n\r\n\r" +
                      "Ejemplo: \"12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc),
            new List<string>()
        }
    });

    //documentacion de encabezado de SWAGGER 
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Ecommerce",
        Description = "API para gestionar productos",
        TermsOfService = new Uri("https://example.com/termns"),
        Contact = new OpenApiContact
        {
            Name = "AFALLAS",
            Url = new Uri("https://linkedin.com/alfabe")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("https://example.com/license"),
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "API Ecommerce V2",
        Description = "API para gestionar productos V2",
        TermsOfService = new Uri("https://example.com/termns"),
        Contact = new OpenApiContact
        {
            Name = "AFALLAS",
            Url = new Uri("https://linkedin.com/alfabe")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("https://example.com/license"),
        }
    });
});

//codigo de versionamiento (antes de CORS)
var apiVersioningBuilder = builder.Services.AddApiVersioning(option =>
{
    option.AssumeDefaultVersionWhenUnspecified = true;
    option.DefaultApiVersion = new ApiVersion(1, 0);
    option.ReportApiVersions = true;
    //option.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-version
});
apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; //v1, v2, v3, ...
    options.SubstituteApiVersionInUrl = true; // api/v{version}/products
});

//codigo para CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder =>
    {
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }
    );

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        //permite escoger la documentacion de la version
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}

app.Use(async (context, next) =>
{
    try
    {
        await next(); // Si algo falla en cualquier controlador, cae aquí
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { Error = "Algo salió mal, intenta luego.", Details = ex.Message });
        // Aquí también podrías loguear el error real en un archivo
    }
});

//codigo para recibir archivos estaticos (imgs, pdfs,...)
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors(PolicyNames.AllowSpecificOrigin);

//activacion de cache (despues de CORS)
app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
