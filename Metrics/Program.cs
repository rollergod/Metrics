using System.Reflection;
using Metrics;
using Metrics.Abstractions.Commands;
using Metrics.Abstractions.Query;
using Metrics.ConnectionFactory;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IConnectionFactory>(_ =>
    new ConnectionFactory(builder.Configuration["DockerConnectionString"]));

builder.Services.AddDbContext<AppDbContext>(opt => 
    opt.UseSqlServer(builder.Configuration["DockerConnectionString"]));

builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<IQueryDispatcher, QueryDispatcher>();

builder.Services.AddCors();

#region query
var queryHandlers = Assembly.GetAssembly(typeof(IQueryDispatcher)).GetTypes()
    .Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
    .ToList();

queryHandlers.ForEach(handler =>
    builder.Services.AddScoped(handler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)), handler)
);
#endregion

#region commands
var commandHandlres = Assembly.GetAssembly(typeof(ICommandDispatcher)).GetTypes()
    .Where(x => x.GetInterfaces()
        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
    .ToList();

commandHandlres.ForEach(handler => 
    builder.Services.AddScoped(handler.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)), handler)
);
#endregion

builder.Services.AddOpenTelemetry()
    .WithMetrics(x =>
    {
        x.AddPrometheusExporter() //экспортирует метрики 
            .AddRuntimeInstrumentation() //  использование процессора, работа сборщика мусора и память
            .AddAspNetCoreInstrumentation(); // собираем метрики связанные с .net core , время отклика, кол-во запросов и статусы кодов
    });

var app = builder.Build();

//app.UseMetricServer();  // создат эндпоинт /metrics для экспорта метрик Prometheus
//app.UseHttpMetrics(); // добавит метрики по http запросам

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); 
app.MapPrometheusScrapingEndpoint();

app.UseAuthorization();
//app.MapMetrics();
app.MapControllers();


app.Run();
