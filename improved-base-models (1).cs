using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CarManagementApp.Models
{
    // Перечисления для улучшения типизации
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

    // Интерфейсы
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

    // Базовый абстрактный класс для транспортных средств
    [Serializable]
    public abstract class Vehicle : INotifyPropertyChanged
    {
        // Защищенные поля класса
        protected string brand;
        protected int power;
        protected decimal cost;
        protected List<DateTime> repairDates;
        protected string vin;
        protected int yearOfManufacture;
        protected double mileage;
        protected Color color;
        protected DateTime registrationDate;
        protected DateTime lastInspectionDate;
        protected bool isInsured;
        protected DateTime insuranceExpiryDate;

        // Конструкторы
        public Vehicle()
        {
            brand = "Неизвестно";
            power = 0;
            cost = 0;
            repairDates = new List<DateTime>();
            vin = string.Empty;
            yearOfManufacture = DateTime.Now.Year;
            mileage = 0;
            color = Color.White;
            registrationDate = DateTime.Now;
            lastInspectionDate = DateTime.Now;
            isInsured = false;
            insuranceExpiryDate = DateTime.Now;
        }

        public Vehicle(string brand, int power, decimal cost)
        {
            this.brand = brand;
            this.power = power;
            this.cost = cost;
            repairDates = new List<DateTime>();
            vin = string.Empty;
            yearOfManufacture = DateTime.Now.Year;
            mileage = 0;
            color = Color.White;
            registrationDate = DateTime.Now;
            lastInspectionDate = DateTime.Now;
            isInsured = false;
            insuranceExpiryDate = DateTime.Now;
        }

        // Свойства с поддержкой уведомлений об изменении
        public string Brand
        {
            get { return brand; }
            set { SetProperty(ref brand, value); }
        }

        public int Power
        {
            get { return power; }
            set 
            { 
                if (value < 0)
                    throw new PowerNegativeException("Мощность не может быть отрицательной");
                SetProperty(ref power, value); 
            }
        }

        public decimal Cost
        {
            get { return cost; }
            set 
            { 
                if (value < 0)
                    throw new CostNegativeException("Стоимость не может быть отрицательной");
                SetProperty(ref cost, value); 
            }
        }

        public List<DateTime> RepairDates
        {
            get { return repairDates; }
        }

        public string VIN
        {
            get { return vin; }
            set { SetProperty(ref vin, value); }
        }

        public int YearOfManufacture
        {
            get { return yearOfManufacture; }
            set 
            { 
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentOutOfRangeException("Год производства должен быть между 1900 и текущим годом");
                SetProperty(ref yearOfManufacture, value); 
            }
        }

        public double Mileage
        {
            get { return mileage; }
            set 
            { 
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Пробег не может быть отрицательным");
                SetProperty(ref mileage, value); 
            }
        }

        [JsonIgnore] // Используем JsonIgnore для свойств, которые нельзя сериализовать напрямую
        public Color Color
        {
            get { return color; }
            set { SetProperty(ref color, value); }
        }

        // Сериализуемое представление цвета
        public string ColorHex
        {
            get { return ColorTranslator.ToHtml(color); }
            set { Color = ColorTranslator.FromHtml(value); }
        }

        public DateTime RegistrationDate
        {
            get { return registrationDate; }
            set { SetProperty(ref registrationDate, value); }
        }

        public DateTime LastInspectionDate
        {
            get { return lastInspectionDate; }
            set { SetProperty(ref lastInspectionDate, value); }
        }

        public bool IsInsured
        {
            get { return isInsured; }
            set { SetProperty(ref isInsured, value); }
        }

        public DateTime InsuranceExpiryDate
        {
            get { return insuranceExpiryDate; }
            set { SetProperty(ref insuranceExpiryDate, value); }
        }

        // Вычисляемые свойства
        public int Age
        {
            get { return DateTime.Now.Year - yearOfManufacture; }
        }

        public bool NeedsInspection
        {
            get { return (DateTime.Now - lastInspectionDate).TotalDays > 365; }
        }

        public bool InsuranceExpired
        {
            get { return isInsured && DateTime.Now > insuranceExpiryDate; }
        }

        // Индексатор для доступа к датам ремонта
        public DateTime this[int index]
        {
            get
            {
                if (index < 0 || index >= repairDates.Count)
                    throw new IndexOutOfRangeException("Индекс за пределами списка дат ремонта");
                return repairDates[index];
            }
        }

        // Виртуальные методы
        public virtual void AddRepairDate(DateTime date)
        {
            repairDates.Add(date);
            OnPropertyChanged(nameof(RepairDates));
        }

        public virtual void RemoveRepairDate(DateTime date)
        {
            repairDates.Remove(date);
            OnPropertyChanged(nameof(RepairDates));
        }

        public virtual DateTime GetLastRepairDate()
        {
            if (repairDates.Count == 0)
                return DateTime.MinValue;
            
            return repairDates[repairDates.Count - 1];
        }

        public abstract string GetInfo();

        public virtual decimal CalculateDepreciation()
        {
            // Базовый расчет амортизации
            decimal ageDepreciation = Math.Min(0.5m, Age * 0.1m); // Максимум 50% от старости
            return cost * ageDepreciation;
        }

        public virtual decimal CalculateMarketValue()
        {
            return cost - CalculateDepreciation();
        }

        public virtual decimal CalculateInsurancePremium()
        {
            // Базовый расчет страховой премии
            decimal basePremium = cost * 0.05m; // 5% от стоимости
            decimal ageFactor = 1 + (Age * 0.1m); // Увеличение на 10% за каждый год
            
            return basePremium * ageFactor;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }

    // Класс-наследник для легковых автомобилей с реализацией интерфейсов
    [Serializable]
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

        // Конструкторы
        public Car() : base()
        {
            carType = CarType.Sedan;
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
            ownerName = "Не указан";
            licensePlate = "Не указан";
        }

        public Car(string brand, int power, decimal cost, CarType carType) : base(brand, power, cost)
        {
            this.carType = carType;
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
            ownerName = "Не указан";
            licensePlate = "Не указан";
        }

        // Расширенный конструктор
        public Car(string brand, string model, int power, decimal cost, CarType carType, FuelType fuelType,
                int year, double mileage, Color color) : base(brand, power, cost)
        {
            this.carType = carType;
            this.fuelType = fuelType;
            this.model = model;
            photos = new List<PhotoInfo>();
            fuelRecords = new List<FuelRecord>();
            serviceHistory = new List<ServiceRecord>();
            currentLocation = new GeoLocation();
            locationHistory = new List<GeoLocation>();
            yearOfManufacture = year;
            this.mileage = mileage;
            this.color = color;
            transmission = "Механическая";
            fuelCapacity = 50;
            doors = 4;
            seats = 5;
            nextMaintenanceDate = DateTime.Now.AddMonths(6);
            maintenanceIntervalKm = 10000;
            ownerName = "Не указан";
            licensePlate = "Не указан";
        }

        // Свойства
        public CarType CarType
        {
            get { return carType; }
            set { SetProperty(ref carType, value); }
        }

        public FuelType FuelType
        {
            get { return fuelType; }
            set { SetProperty(ref fuelType, value); }
        }

        public double FuelCapacity
        {
            get { return fuelCapacity; }
            set 
            { 
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Емкость топливного бака должна быть положительной");
                SetProperty(ref fuelCapacity, value); 
            }
        }

        public string Model
        {
            get { return model; }
            set { SetProperty(ref model, value); }
        }

        public int Doors
        {
            get { return doors; }
            set 
            { 
                if (value < 1 || value > 6)
                    throw new ArgumentOutOfRangeException("Количество дверей должно быть от 1 до 6");
                SetProperty(ref doors, value); 
            }
        }

        public int Seats
        {
            get { return seats; }
            set 
            { 
                if (value < 1 || value > 9)
                    throw new ArgumentOutOfRangeException("Количество мест должно быть от 1 до 9");
                SetProperty(ref seats, value); 
            }
        }

        public string Transmission
        {
            get { return transmission; }
            set { SetProperty(ref transmission, value); }
        }

        public List<PhotoInfo> Photos
        {
            get { return photos; }
        }

        public List<FuelRecord> FuelRecords
        {
            get { return fuelRecords; }
        }

        public List<ServiceRecord> ServiceHistory
        {
            get { return serviceHistory; }
        }

        public GeoLocation CurrentLocation
        {
            get { return currentLocation; }
            set { SetProperty(ref currentLocation, value); }
        }

        public List<GeoLocation> LocationHistory
        {
            get { return locationHistory; }
        }

        public DateTime NextMaintenanceDate
        {
            get { return nextMaintenanceDate; }
            set { SetProperty(ref nextMaintenanceDate, value); }
        }

        public double MaintenanceIntervalKm
        {
            get { return maintenanceIntervalKm; }
            set 
            { 
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("Интервал обслуживания должен быть положительным");
                SetProperty(ref maintenanceIntervalKm, value); 
            }
        }

        public string OwnerName
        {
            get { return ownerName; }
            set { SetProperty(ref ownerName, value); }
        }

        public string LicensePlate
        {
            get { return licensePlate; }
            set { SetProperty(ref licensePlate, value); }
        }

        // Вычисляемые свойства
        public string FullName
        {
            get { return $"{brand} {model} ({yearOfManufacture})"; }
        }

        public bool MaintenanceDue
        {
            get 
            { 
                return DateTime.Now > nextMaintenanceDate || 
                       GetLastServiceOdometer() + maintenanceIntervalKm < mileage;
            }
        }

        // Переопределение виртуальных методов
        public override string GetInfo()
        {
            return $"Марка: {Brand}, Модель: {Model}, Тип: {CarType}, Мощность: {Power} л.с., Стоимость: {Cost:C}";
        }

        public override decimal CalculateDepreciation()
        {
            // Специфичный расчет амортизации для автомобиля
            decimal ageDepreciation = Math.Min(0.7m, Age * 0.1m); // Максимум 70% от старости
            decimal mileageDepreciation = Math.Min(0.5m, (decimal)(Mileage / 100000) * 0.2m); // Дополнительно от пробега
            
            return Cost * Math.Max(ageDepreciation, mileageDepreciation);
        }

        public override decimal CalculateInsurancePremium()
        {
            // Специфичный расчет страховой премии для автомобиля
            decimal basePremium = Cost * 0.05m;
            decimal ageFactor = 1 + (Age * 0.1m);
            decimal powerFactor = 1 + (Power / 100.0m) * 0.2m;
            decimal typeFactor = GetTypeInsuranceFactor();
            
            return basePremium * ageFactor * powerFactor * typeFactor;
        }

        private decimal GetTypeInsuranceFactor()
        {
            switch (carType)
            {
                case CarType.SUV: return 1.2m;
                case CarType.Coupe: return 1.3m;
                case CarType.Convertible: return 1.4m;
                case CarType.Sedan: return 1.0m;
                default: return 1.1m;
            }
        }

        // Реализация методов интерфейса IServiceable
        public void PerformMaintenance()
        {
            DateTime now = DateTime.Now;
            serviceHistory.Add(new ServiceRecord 
            { 
                Date = now, 
                Description = "Плановое техническое обслуживание", 
                Cost = CalculateMaintenanceCost(),
                Odometer = mileage
            });
            
            nextMaintenanceDate = now.AddMonths(6);
            OnPropertyChanged(nameof(ServiceHistory));
            OnPropertyChanged(nameof(NextMaintenanceDate));
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
            // Расчет стоимости обслуживания в зависимости от возраста и типа
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

        // Реализация методов интерфейса IPhotoContainer
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

        // Реализация методов интерфейса IFuelTracking
        public void AddFuelRecord(DateTime date, double liters, decimal cost, double odometer)
        {
            if (liters <= 0)
                throw new ArgumentException("Количество литров должно быть положительным");
                
            if (cost <= 0)
                throw new ArgumentException("Стоимость должна быть положительной");
                
            if (odometer < mileage)
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
                
            // Расчет среднего расхода топлива на 100 км
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

        // Реализация методов интерфейса ILocationTracking
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
                totalDistance += locationHistory[i-1].CalculateDistance(locationHistory[i]);
            }
            
            return totalDistance;
        }

        // Реализация IComparable для сортировки автомобилей
        public int CompareTo(Car other)
        {
            if (other == null) return 1;
            
            // Сначала сравниваем по марке
            int brandComparison = this.Brand.CompareTo(other.Brand);
            if (brandComparison != 0) return brandComparison;
            
            // Затем по модели
            int modelComparison = this.Model.CompareTo(other.Model);
            if (modelComparison != 0) return modelComparison;
            
            // Наконец по стоимости
            return this.Cost.CompareTo(other.Cost);
        }

        // Дополнительные методы
        public void AddServiceRecord(DateTime date, string description, decimal cost)
        {
            serviceHistory.Add(new ServiceRecord 
            { 
                Date = date, 
                Description = description, 
                Cost = cost,
                Odometer = mileage
            });
            OnPropertyChanged(nameof(ServiceHistory));
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

    // Вспомогательные классы для хранения данных
    [Serializable]
    public class PhotoInfo
    {
        public string Path { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
    }

    [Serializable]
    public class FuelRecord
    {
        public DateTime Date { get; set; }
        public double Liters { get; set; }
        public decimal Cost { get; set; }
        public decimal PricePerLiter { get; set; }
        public double Odometer { get; set; }
    }

    [Serializable]
    public class ServiceRecord
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public double Odometer { get; set; }
    }

    [Serializable]
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
                
            // Расчет расстояния по формуле гаверсинуса
            const double R = 6371.0; // Радиус Земли в км
            double lat1 = Latitude * Math.PI / 180;
            double lat2 = other.Latitude * Math.PI / 180;
            double dLat = (other.Latitude - Latitude) * Math.PI / 180;
            double dLon = (other.Longitude - Longitude) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        public override string ToString()
        {
            return $"Широта: {Latitude:F6}, Долгота: {Longitude:F6}, Время: {Timestamp}";
        }
    }

    // Пользовательские исключения
    [Serializable]
    public class VehicleException : Exception
    {
        public VehicleException(string message) : base(message) { }
    }

    [Serializable]
    public class PowerNegativeException : VehicleException
    {
        public PowerNegativeException(string message) : base(message) { }
    }

    [Serializable]
    public class CostNegativeException : VehicleException
    {
        public CostNegativeException(string message) : base(message) { }
    }

    [Serializable]
    public class LicensePlateFormatException : VehicleException
    {
        public LicensePlateFormatException(string message) : base(message) { }
    }

    [Serializable]
    public class VINFormatException : VehicleException
    {
        public VINFormatException(string message) : base(message) { }
    }
}
