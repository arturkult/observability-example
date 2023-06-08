using System.Reflection;
using DotnetObservabilityExample;
using Elastic.Apm.NetCoreAll;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://elastic:changeme@localhost:9200"))
{
    IndexFormat = $"َapp-logs-docker-{DateTimeOffset.Now.LocalDateTime:yyyy-MM}",
    AutoRegisterTemplate = true,
    OverwriteTemplate = true,
    TemplateName = "yourTemplateName",
    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
    TypeName = null,
    BatchAction = ElasticOpType.Create,
    ModifyConnectionSettings = configuration => configuration.ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true)
})
    .WriteTo.Console()
);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<ApplicationDbContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(""));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
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

app.UseSerilogRequestLogging();

app.UseAllElasticApm(builder.Configuration);

app.Run();
