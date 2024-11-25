using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WidgetStore.API.Middleware;
using WidgetStore.Core.Configuration;
using WidgetStore.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WidgetStore.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Configure services with Options pattern
builder.Services.Configure<AzureStorageConfig>(
    builder.Configuration.GetSection("AzureStorageConfig"));

builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("JwtConfig"));

builder.Services.Configure<AdminConfig>(
    builder.Configuration.GetSection("AdminConfig"));

builder.Services.Configure<CosmosDbConfig>(
    builder.Configuration.GetSection("CosmosDbConfig"));

builder.Services.Configure<QueueStorageConfig>(
    builder.Configuration.GetSection("QueueStorageConfig"));

// Get configurations for use in program.cs
var jwtConfig = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>();
var cosmosDbConfig = builder.Configuration.GetSection("CosmosDbConfig").Get<CosmosDbConfig>();
var storageConfig = builder.Configuration.GetSection("AzureStorageConfig").Get<AzureStorageConfig>();

if (jwtConfig == null || cosmosDbConfig == null || storageConfig == null)
{
    throw new InvalidOperationException("Missing configuration sections");
}

// Configure file upload limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = BlobStorageConfig.MaxFileSizeInMB * 1024 * 1024;
});

// Add Azure services
builder.Services.AddAzureStorage(storageConfig);
builder.Services.AddCosmosDb(cosmosDbConfig);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // No expiration for POC
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Widget Store API",
        Version = "v1",
        Description = "API for Widget Store proof of concept"
    });

    c.SchemaFilter<SwaggerFileOperationFilter>();

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });

    // XML Comments configuration
    try
    {
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
        c.IncludeXmlComments(xmlPath);
    }
    catch (Exception)
    {
        // Log the error or handle it appropriately
    }
});

// Configure IIS
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = BlobStorageConfig.MaxFileSizeInMB * 1024 * 1024; // MB in bytes
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Widget Store API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Error handling middleware should be one of the first middleware in the pipeline
app.UseErrorHandling();

app.UseHttpsRedirection();

// Important: UseAuthentication must come before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure Azure resources exist
await StorageExtensions.EnsureStorageResourcesExistAsync(storageConfig);
await CosmosDbExtensions.EnsureCosmosDbResourcesExistAsync(cosmosDbConfig);

app.Run();