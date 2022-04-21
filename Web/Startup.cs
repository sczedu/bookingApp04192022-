using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using MediatR;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.DynamoDB.Services;

namespace Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
        });

        services.AddTransient<IScheduleRepository, ScheduleRepository>();

        services.AddTransient<IMediator, Mediator>();
        services.AddMediatR(typeof(Application.Query.GetSchedule).Assembly);
        services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddTransient<IDynamoDBContext, DynamoDBContext>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "Booking API V1");
            c.RoutePrefix = "swagger";
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}