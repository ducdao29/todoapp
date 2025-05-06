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

            // Ideally, you will want this name to come from a config file, constants file, etc.
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
                    .AddConsoleExporter()
                    .AddOtlpExporter(options => {
                        options.Endpoint = new Uri("http://localhost:4317");
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    })
                    )
                .WithMetrics(metrics => metrics
                    .AddMeter(serviceName)
                    .AddConsoleExporter());

            builder.Logging.AddOpenTelemetry(options => options
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion))
                .AddConsoleExporter());

            builder.Services.AddControllers();

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
