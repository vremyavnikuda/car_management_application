using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace CarManagementApp.Models
{
    public enum CarType
    {
        Sedan,
        Hatchback,
        Universal,
        SUV,
        Coupe,
        Minivan,
        Pickup,
        Convertible
    }

    public enum FuelType
    {
        Gasoline,
        Diesel,
        Electric,
        Hybrid,
        LPG,
        CNG
    }

    public interface IServiceable
    {
        void PerformMaintenance();
        bool NeedsService();
        DateTime GetNextServiceDate();
    }

    public interface IPhotoContainer
    {
        List<PhotoInfo> Photos { get; }
        void AddPhoto(string path, string description);
        void RemovePhoto(int index);
    }

    public interface IFuelTracking
    {
        List<FuelRecord> FuelRecords { get; }
        void AddFuelRecord(DateTime date, double liters, decimal cost, double odometer);
        double CalculateAverageFuelConsumption();
    }

    public interface ILocationTracking
    {
        GeoLocation CurrentLocation { get; set; }
        List<GeoLocation> LocationHistory { get; }
        void UpdateLocation(GeoLocation location);
        double CalculateTotalDistance();
    }

    public class Car : Vehicle, IServiceable, IPhotoContainer, IFuelTracking, ILocationTracking, IComparable<Car>
    {
        private CarType carType;
        private FuelType fuelType;
        private double fuelCapacity;
        private string model;
        private int doors;
        private int seats;
        private string transmission;
        private List<PhotoInfo> photos;
        private List<FuelRecord> fuelRecords;
        private List<ServiceRecord> serviceHistory;
        private GeoLocation currentLocation;
        private List<GeoLocation> locationHistory;
        private DateTime nextMaintenanceDate;
        private double maintenanceIntervalKm;
        private string ownerName;
        private string licensePlate;

        // Конструктор без параметров для десириализации JSON
        [JsonConstructor]
        public Car() : base("Неизвестно", 0, 0)
        {
            InitializeDefaults();
        }

        public Car(string brand, int power, decimal cost, CarType carType, string owner = "Не указан")
            : base(brand, power, cost)
        {
            this.carType = carType;
            ownerName = owner;
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            fuelType = FuelType.Gasoline;
            fuelCapacity = 50;
            model = "Неизвестно";
            doors = 4;
            seats = 5;
            transmission = "Механическая";
            photos = new List<PhotoInfo>();
            fuelRecords = new List<FuelRecord>();
            serviceHistory = new List<ServiceRecord>();
            currentLocation = new GeoLocation();
            locationHistory = new List<GeoLocation>();
            nextMaintenanceDate = DateTime.Now.AddMonths(6);
            maintenanceIntervalKm = 10000;
            licensePlate = "Не указан";
        }

        public CarType CarType
        {
            get => carType;
            set => SetProperty(ref carType, value);
        }

        public FuelType FuelType
        {
            get => fuelType;
            set => SetProperty(ref fuelType, value);
        }

        public double FuelCapacity
        {
            get => fuelCapacity;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Емкость топливного бака должна быть положительной");
                SetProperty(ref fuelCapacity, value);
            }
        }

        public string Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        public int Doors
        {
            get => doors;
            set
            {
                if (value < 1 || value > 6)
                    throw new ArgumentOutOfRangeException("Количество дверей должно быть от 1 до 6");
                SetProperty(ref doors, value);
            }
        }

        public int Seats
        {
            get => seats;
            set
            {
                if (value < 1 || value > 9)
                    throw new ArgumentOutOfRangeException("Количество мест должно быть от 1 до 9");
                SetProperty(ref seats, value);
            }
        }

        public string Transmission
        {
            get => transmission;
            set => SetProperty(ref transmission, value);
        }

        public List<PhotoInfo> Photos => photos;

        public List<FuelRecord> FuelRecords => fuelRecords;

        public List<ServiceRecord> ServiceHistory => serviceHistory;

        public GeoLocation CurrentLocation
        {
            get => currentLocation;
            set => SetProperty(ref currentLocation, value);
        }

        public List<GeoLocation> LocationHistory => locationHistory;

        public DateTime NextMaintenanceDate
        {
            get => nextMaintenanceDate;
            set => SetProperty(ref nextMaintenanceDate, value);
        }

        public double MaintenanceIntervalKm
        {
            get => maintenanceIntervalKm;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Интервал обслуживания должен быть положительным");
                SetProperty(ref maintenanceIntervalKm, value);
            }
        }

        public string OwnerName
        {
            get => ownerName;
            set => SetProperty(ref ownerName, value);
        }

        public string LicensePlate
        {
            get => licensePlate;
            set => SetProperty(ref licensePlate, value);
        }

        public string FullName => $"{Brand} {Model} ({YearOfManufacture})";

        public bool MaintenanceDue => DateTime.Now > nextMaintenanceDate ||
                                      GetLastServiceOdometer() + maintenanceIntervalKm < Mileage;

        // Добавляем для совместимости со старым кодом
        public List<DateTime> RepairDates
        {
            get
            {
                List<DateTime> dates = new List<DateTime>();
            foreach (var service in serviceHistory)
                {
                    dates.Add(service.Date);
                }

                return dates;
            }
        }

        public void AddRepairDate(DateTime date)
        {
            AddServiceRecord(date, "Ремонт", 0);
        }

        public void RemoveRepairDate(DateTime date)
        {
            // Находим и удаляем запись о ремонте с этой датой
            for (int i = serviceHistory.Count - 1; i >= 0; i--)
            {
                if (serviceHistory[i].Date.Date == date.Date)
                {
                    serviceHistory.RemoveAt(i);
                    OnPropertyChanged(nameof(ServiceHistory));
                    OnPropertyChanged(nameof(RepairDates));
                    break;
                }
            }
        }


        // TODO: мб когда нибудь пригодится
        // public override string GetInfo()
        // {
        //     return
        //         $"Марка: {Brand}, Модель: {Model}, Тип: {CarType}, Мощность: {Power} л.с., Стоимость: {Cost:C}, Владелец: {OwnerName}";
        // }

        public override string GetInfo()
        {
            return
                $"{OwnerName}: {Brand} {Model}";
        }

        public override decimal CalculateDepreciation()
        {
            decimal ageDepreciation = Math.Min(0.7m, Age * 0.1m);
            decimal mileageDepreciation = Math.Min(0.5m, (decimal)(Mileage / 100000) * 0.2m);
            return Cost * Math.Max(ageDepreciation, mileageDepreciation);
        }

        public override decimal CalculateInsurancePremium()
        {
            decimal basePremium = Cost * 0.05m;
            decimal ageFactor = 1 + (Age * 0.1m);
            decimal powerFactor = 1 + (Power / 100.0m) * 0.2m;
            decimal typeFactor = GetTypeInsuranceFactor();

            return basePremium * ageFactor * powerFactor * typeFactor;
        }

        private decimal GetTypeInsuranceFactor()
        {
            return carType switch
            {
                CarType.SUV => 1.2m,
                CarType.Coupe => 1.3m,
                CarType.Convertible => 1.4m,
                CarType.Sedan => 1.0m,
                _ => 1.1m,
            };
        }

        public void PerformMaintenance()
        {
            DateTime now = DateTime.Now;
            serviceHistory.Add(new ServiceRecord
            {
                Date = now,
                Description = "Плановое техническое обслуживание",
                Cost = CalculateMaintenanceCost(),
                Odometer = Mileage
            });

            nextMaintenanceDate = now.AddMonths(6);
            OnPropertyChanged(nameof(ServiceHistory));
            OnPropertyChanged(nameof(NextMaintenanceDate));
            OnPropertyChanged(nameof(RepairDates));
        }

        public bool NeedsService()
        {
            return MaintenanceDue;
        }

        public DateTime GetNextServiceDate()
        {
            return nextMaintenanceDate;
        }

        private decimal CalculateMaintenanceCost()
        {
            decimal baseCost = 5000;
            decimal ageFactor = 1 + (Age * 0.1m);
            decimal typeFactor = carType == CarType.SUV || carType == CarType.Pickup ? 1.3m : 1.0m;

            return baseCost * ageFactor * typeFactor;
        }

        private double GetLastServiceOdometer()
        {
            if (serviceHistory.Count == 0)
                return 0;

            return serviceHistory[serviceHistory.Count - 1].Odometer;
        }

        public void AddPhoto(string path, string description)
        {
            photos.Add(new PhotoInfo { Path = path, Description = description, DateAdded = DateTime.Now });
            OnPropertyChanged(nameof(Photos));
        }

        public void RemovePhoto(int index)
        {
            if (index >= 0 && index < photos.Count)
            {
                photos.RemoveAt(index);
                OnPropertyChanged(nameof(Photos));
            }
        }

        public void AddFuelRecord(DateTime date, double liters, decimal cost, double odometer)
        {
            if (liters <= 0)
                throw new ArgumentException("Количество литров должно быть положительным");

            if (cost <= 0)
                throw new ArgumentException("Стоимость должна быть положительной");

            if (odometer < Mileage)
                throw new ArgumentException("Пробег не может быть меньше текущего");

            fuelRecords.Add(new FuelRecord
            {
                Date = date,
                Liters = liters,
                Cost = cost,
                PricePerLiter = cost / (decimal)liters,
                Odometer = odometer
            });

            Mileage = odometer;
            OnPropertyChanged(nameof(FuelRecords));
        }

        public double CalculateAverageFuelConsumption()
        {
            if (fuelRecords.Count < 2)
                return 0;

            double totalFuel = 0;
            double totalDistance = 0;

            for (int i = 1; i < fuelRecords.Count; i++)
            {
                totalFuel += fuelRecords[i].Liters;
                totalDistance += fuelRecords[i].Odometer - fuelRecords[i - 1].Odometer;
            }

            if (totalDistance == 0)
                return 0;

            return (totalFuel / totalDistance) * 100;
        }

        public void UpdateLocation(GeoLocation location)
        {
            locationHistory.Add(currentLocation);
            CurrentLocation = location;
            OnPropertyChanged(nameof(LocationHistory));
        }

        public double CalculateTotalDistance()
        {
            double totalDistance = 0;

            for (int i = 1; i < locationHistory.Count; i++)
            {
                totalDistance += locationHistory[i - 1].CalculateDistance(locationHistory[i]);
            }

            return totalDistance;
        }

        public int CompareTo(Car? other)
        {
            if (other == null) return 1;

            int brandComparison = String.Compare(Brand, other.Brand, StringComparison.Ordinal);
            if (brandComparison != 0) return brandComparison;

            int modelComparison = String.Compare(Model, other.Model, StringComparison.Ordinal);
            if (modelComparison != 0) return modelComparison;

            return Cost.CompareTo(other.Cost);
        }

        public void AddServiceRecord(DateTime date, string description, decimal cost)
        {
            serviceHistory.Add(new ServiceRecord
            {
                Date = date,
                Description = description,
                Cost = cost,
                Odometer = Mileage
            });
            OnPropertyChanged(nameof(ServiceHistory));
            OnPropertyChanged(nameof(RepairDates));
        }

        public decimal GetTotalServiceCost()
        {
            decimal total = 0;

            foreach (var record in serviceHistory)
            {
                total += record.Cost;
            }

            return total;
        }

        public decimal GetTotalFuelCost()
        {
            decimal total = 0;

            foreach (var record in fuelRecords)
            {
                total += record.Cost;
            }

            return total;
        }

        public decimal CalculateTotalCostOfOwnership()
        {
            return GetTotalServiceCost() + GetTotalFuelCost() + CalculateDepreciation();
        }
    }

    public class PhotoInfo
    {
        public string? Path { get; set; }
        public string? Description { get; set; }
        public DateTime DateAdded { get; set; }
    }

    public class FuelRecord
    {
        public DateTime Date { get; set; }
        public double Liters { get; set; }
        public decimal Cost { get; set; }
        public decimal PricePerLiter { get; set; }
        public double Odometer { get; set; }
    }

    public class ServiceRecord
    {
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public double Odometer { get; set; }
    }

    public class GeoLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        public GeoLocation()
        {
            Latitude = 0;
            Longitude = 0;
            Timestamp = DateTime.Now;
        }

        public GeoLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = DateTime.Now;
        }

        public double CalculateDistance(GeoLocation other)
        {
            if (other == null)
                return 0;
            // Earth's radius in kilometers
            const double R = 6371.0;

            double lat1 = Latitude * Math.PI / 180.0;
            double lon1 = Longitude * Math.PI / 180.0;
            double lat2 = other.Latitude * Math.PI / 180.0;
            double lon2 = other.Longitude * Math.PI / 180.0;

            // Haversine formula
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }

        public class PowerNegativeException : Exception
        {
            public PowerNegativeException(string message) : base(message)
            {
                // KRUTO: тут должна была быть какая то крутая реализация кода
            }
        }

        public class CostNegativeException : Exception
        {
            public CostNegativeException(string message) : base(message)
            {
                // KRUTO: тут должна была быть какая то крутая реализация кода
            }
        }

        public class VinFormatException : Exception
        {
            public VinFormatException(string message) : base(message)
            {
                // KRUTO: тут должна была быть какая то крутая реализация кода
            }
        }
    }
}