using Microsoft.EntityFrameworkCore;
using web_app_car.Models;

namespace web_app_car.Controllers;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
}