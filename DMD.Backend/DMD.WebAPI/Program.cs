using DMD.Application.Interfaces;
using DMD.Application.Mappings;
using DMD.Persistence.Extensions;
using DMD.Persistence.Services;
using System.Text.Json.Serialization;


namespace DMD.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddAppDbContext(builder.Configuration);
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddAutoMapper(typeof(TaskMappingProfile));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Убираем циклические ссылки
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Убираем null значения
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins"); //Временное решение

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

