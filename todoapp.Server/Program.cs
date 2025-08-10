using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace todoapp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var serviceName = "dice-server";
            var serviceVersion = "1.0.0";
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<Instrumentation>();
            // Add services to the container.
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(
            serviceName: serviceName,
                    serviceVersion: serviceVersion))
                .WithTracing(tracing => tracing
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    // .AddConsoleExporter()
                    )
                .WithMetrics(metrics => metrics
                    .AddMeter(serviceName)
                    .AddAspNetCoreInstrumentation() // Built-in ASP.NET Core metrics
                    .AddHttpClientInstrumentation() // Built-in HTTP client metrics
                    .AddOtlpExporter(options =>
                    {
                        // Configure OTLP exporter to send to OTel collector
                        options.Endpoint = new Uri("http://127.0.0.1:4318/v1/metrics");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    })
                )
                .WithLogging(logging => logging
                    .AddOtlpExporter(options =>
                    {
                        // Configure OTLP exporter to send to OTel collector
                        options.Endpoint = new Uri("http://127.0.0.1:4318/v1/logs");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                    }));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
