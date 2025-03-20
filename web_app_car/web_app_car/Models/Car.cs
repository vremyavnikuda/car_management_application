using System.ComponentModel.DataAnnotations;

namespace web_app_car.Models;

public class Car
{
    public int Id { get; set; }

    [Required]
    public string Brand { get; set; }

    [Required]
    public string Model { get; set; }

    [Range(1900, 9999)]
    public int YearOfManufacture { get; set; }

    public string FuelType { get; set; }

    [Range(0, double.MaxValue)]
    public double Price { get; set; }
}
