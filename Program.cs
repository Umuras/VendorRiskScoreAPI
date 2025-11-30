using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Network;
using Serilog.Sinks.Network.Sinks.TCP;
using VendorRiskScoreAPI.Data;
using VendorRiskScoreAPI.Middlewares;
using VendorRiskScoreAPI.Repositories;
using VendorRiskScoreAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<VendorRiskScoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VendorRiskScoreDb")));

//Binding Operations
builder.Services.AddScoped<IVendorProfileRepository, VendorProfileRepository>();
builder.Services.AddScoped<IVendorProfileService, VendorProfileService>();
builder.Services.AddScoped<IVendorSecurityCertRepository, VendorSecurityCertRepository>();
builder.Services.AddScoped<IVendorSecurityCertService, VendorSecurityCertService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IRiskAssessmentRepository, RiskAssessmentRepository>();
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();
builder.Services.AddScoped<IRuleEngineService, RuleEngineService>();
builder.Services.AddScoped<IVendorProfileRiskScoreRepository, VendorProfileRiskScoreRepository>();
builder.Services.AddScoped<IVendorProfileRiskScoreService, VendorProfileRiskScoreService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "logs/log.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

Log.Information("Application started");

app.MapControllers();

app.Run();
