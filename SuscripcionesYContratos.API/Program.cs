using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SuscripcionesYContratos.Infraestructura;
using SuscripcionesYContratos.Infraestructura.Persistencia.ModeloPersistencia;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//--------------------------------------
var keycloakSection = builder.Configuration.GetSection("Keycloak");
var authority = keycloakSection["Authority"];
var audience = keycloakSection["Audience"];
var clientId = keycloakSection["ClientId"];
var requireHttpsMetadata = bool.TryParse(keycloakSection["RequireHttpsMetadata"], out var parsedRequireHttps)
    ? parsedRequireHttps
    : false;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.Audience = audience;
        options.RequireHttpsMetadata = requireHttpsMetadata;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                {
                    return Task.CompletedTask;
                }

                AddRealmRoles(identity);
                AddClientRoles(identity, clientId);

                return Task.CompletedTask;
            }
        };
    });

static void AddRealmRoles(ClaimsIdentity identity)
{
    var realmAccessClaim = identity.FindFirst("realm_access")?.Value;
    if (string.IsNullOrWhiteSpace(realmAccessClaim))
    {
        return;
    }

    using var realmDoc = JsonDocument.Parse(realmAccessClaim);
    if (!realmDoc.RootElement.TryGetProperty("roles", out var rolesElement) || rolesElement.ValueKind != JsonValueKind.Array)
    {
        return;
    }

    foreach (var roleElement in rolesElement.EnumerateArray())
    {
        var role = roleElement.GetString();
        if (string.IsNullOrWhiteSpace(role))
        {
            continue;
        }

        if (!identity.HasClaim(ClaimTypes.Role, role))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}

static void AddClientRoles(ClaimsIdentity identity, string? clientId)
{
    if (string.IsNullOrWhiteSpace(clientId))
    {
        return;
    }

    var resourceAccessClaim = identity.FindFirst("resource_access")?.Value;
    if (string.IsNullOrWhiteSpace(resourceAccessClaim))
    {
        return;
    }

    using var resourceDoc = JsonDocument.Parse(resourceAccessClaim);
    if (!resourceDoc.RootElement.TryGetProperty(clientId, out var clientElement))
    {
        return;
    }

    if (!clientElement.TryGetProperty("roles", out var rolesElement) || rolesElement.ValueKind != JsonValueKind.Array)
    {
        return;
    }

    foreach (var roleElement in rolesElement.EnumerateArray())
    {
        var role = roleElement.GetString();
        if (string.IsNullOrWhiteSpace(role))
        {
            continue;
        }

        if (!identity.HasClaim(ClaimTypes.Role, role))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
    }
}

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOrSuscripcion", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("admin", "suscripcionycontrato");
        });
    });
//--------------------------------------


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa: Bearer {tu access_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

// Aplica migraciones al arrancar (importante en Docker)
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PersistenceDbContext>();

    const int maxRetries = 10;
    for (var i = 1; i <= maxRetries; i++)
    {
        try
        {
            await db.Database.MigrateAsync();
            break;
        }
        catch (Exception) when (i < maxRetries)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
