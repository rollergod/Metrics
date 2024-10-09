using Metrics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithMetrics(x =>
    {
        x.AddPrometheusExporter();

        x.AddMeter("Microsoft.AspNetCore.Hosting","Microsoft.AspNetCore.Server.Kestrel");
        x.AddView("request-duration", new ExplicitBucketHistogramConfiguration
        {
            Boundaries = new[] { 0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1 }
        });
    });

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("AppDbContext"));

var app = builder.Build();

//app.UseMetricServer(); // создат эндпоинт /metrics для экспорта метрик Prometheus
//app.UseHttpMetrics(); // добавит метрики по http запросам

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.UseAuthorization();
//app.MapMetrics();
app.MapControllers();

app.Run();
