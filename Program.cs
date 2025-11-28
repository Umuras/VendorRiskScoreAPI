using Microsoft.EntityFrameworkCore;
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
