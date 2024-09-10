using DMD.Application.Interfaces;
using DMD.Persistence.Extensions;
using DMD.Persistence.Services;


namespace DMD.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddAppDbContext(builder.Configuration);
            builder.Services.AddScoped<ITaskService, TaskService>();

            builder.Services.AddControllers();
            
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

