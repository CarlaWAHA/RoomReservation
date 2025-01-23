using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using Serilog;

namespace RoomBookingApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
          
            try
            {

                Log.Information("Strating app");
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(open =>
                {
                    open.SwaggerDoc("V1", new OpenApiInfo
                    {
                        Version="V1",
                        Title="Room Api"
                    });
                });
                builder.Services.AddControllers();
                builder.Services.AddDbContext<RoomApiContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
                );
                builder.Services.Configure<ApplicationSettings>(
                    builder.Configuration.GetSection("ApplicationSettings"));

                builder.Services.AddApiVersioning(options=>{
                    options.DefaultApiVersion=new ApiVersion(1,0);
                    options.AssumeDefaultVersionWhenUnspecified=true;
                    options.ReportApiVersions=true;
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(setup =>
                    {
                        setup.SwaggerEndpoint("/swagger/v1/swagger.json","Room API V1");
                        setup.SwaggerEndpoint("/swagger/v2/swagger.json","Room API V2");
                    });
                }

                app.UseHttpsRedirection();
                app.MapControllers();
                app.MapGet("/", () => "Hello World!");

                app.Run();                 
            }
            catch (Exception ex)
            {
                Log.Fatal(ex,"Application Failed unexpectedly");
            }
            finally
            {
                
            }
        }
    }
}