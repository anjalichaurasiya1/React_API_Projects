using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Controllers
// =========================
builder.Services.AddControllers();

// =========================
// Register UnitMasterService
// =========================
builder.Services.AddScoped(sp =>
    new product_api.Service.UnitMasterService(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured")));

    builder.Services.AddScoped(sp =>
    new product_api.Service.AttendenceService(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured")));
    
builder.Services.AddScoped(sp =>
    new product_api.Service.BillService(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection is not configured")));


// =========================
// CORS
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:3001",
                "http://localhost:3002"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// =========================
// JWT Authentication
// =========================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

// =========================
// Swagger (✅ FIXED)
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product API",
        Version = "v1",
        Description = "API documentation for Product API"
    });

    // 🔐 Optional: JWT support in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT like: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();


// =========================
// Middleware pipeline
// =========================
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API v1");
    c.RoutePrefix = "swagger"; // optional
});

app.UseHttpsRedirection();

// ✅ CORS BEFORE auth
app.UseCors("AllowReact");

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();