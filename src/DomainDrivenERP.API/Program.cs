using System.Text.Json;
using System.Text.Json.Serialization;
using DomainDrivenERP.Application;
using DomainDrivenERP.Identity;
using DomainDrivenERP.Infrastructure;
using DomainDrivenERP.MiddleWares;
using DomainDrivenERP.Persistence;
using DomainDrivenERP.Presentation.Configuration.Extensions.Swagger;
using Serilog;
using DomainDrivenERP.API.Extensions;
#region DI
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<GlobalExceptionHandlerMiddleWare>();
builder
    .Services
    .AddControllers()

    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.IgnoreNullValues = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddApplicationPart(DomainDrivenERP.Presentation.AssemblyReference.Assembly);

builder.Services.AddSwaggerDocumentation();

builder.Services.AddCustomLocalization(builder.Configuration);

builder.Services.AddApplicationDependencies()
                .AddInfrustructureDependencies()
                .AddPersistenceDependencies(builder.Configuration)
                .AddIdentityDependencies(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfig) =>
loggerConfig.ReadFrom.Configuration(context.Configuration));
//builder.Services.AddCors(options => 
//    options.AddPolicy("AllowAngularApp", policy => 
//        policy.WithOrigins("http://localhost:4200")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials()
//              .SetPreflightMaxAge(TimeSpan.FromMinutes(10)) // Cache preflight for 10 minutes
//    )
//);
builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins", policy => policy.SetIsOriginAllowedToAllowWildcardSubdomains()
.WithOrigins("https://localhost", "http://localhost", "https://localhost:44372", "http://localhost:44372", "http://localhost:5200", "https://localhost:5200")//localhost:44372
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

#endregion
#region MiddleWare
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerDocumentation();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseRequestLocalization();

// Add CORS middleware here - IMPORTANT: Must be before UseAuthentication
//app.UseCors("AllowAngularApp");
app.UseCors("AllowAllOrigins");
app.UseMiddleware<GlobalExceptionHandlerMiddleWare>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion
