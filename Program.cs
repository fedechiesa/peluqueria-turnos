using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;
using TurnosPeluqueria.Services;

namespace TurnosPeluqueria
{
    public class Program
    {
        public static void Main(string[] args)

        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<TurnosContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddSession();
            builder.Services.AddScoped<TurnoService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession(); 

            app.UseAuthorization();

            app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Para crear el horario de los peluqueros.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TurnosContext>();

                var peluqueros = db.Peluqueros.ToList();

                foreach (var peluquero in peluqueros)
                {
                    if (!db.HorariosPeluqueros.Any(h => h.PeluqueroId == peluquero.Id))
                    {
                        db.HorariosPeluqueros.AddRange(new[]
                        {
                new HorarioPeluquero
                {
                    PeluqueroId = peluquero.Id,
                    Dia = DayOfWeek.Monday,
                    Desde = new TimeSpan(9, 0, 0),
                    Hasta = new TimeSpan(13, 0, 0)
                },
                new HorarioPeluquero
                {
                    PeluqueroId = peluquero.Id,
                    Dia = DayOfWeek.Tuesday,
                    Desde = new TimeSpan(15, 0, 0),
                    Hasta = new TimeSpan(18, 0, 0)
                }
            });
                    }
                }

                db.SaveChanges();
            }


            app.Run();
        }
    }
}
