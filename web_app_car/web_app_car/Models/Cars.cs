using System.ComponentModel.DataAnnotations;

namespace web_app_car.Models;

public class Cars
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

    [Required]
    public string OwnerName { get; set; }

    [Required]
    public string LicensePlate { get; set; }

    [Range(0, int.MaxValue)]
    public int Power { get; set; }

    public string Type { get; set; }

    public string Transmission { get; set; }

    public string RepairDates { get; set; }
}