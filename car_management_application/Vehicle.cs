using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CarManagementApp.Models
{
    public abstract class Vehicle : INotifyPropertyChanged
    {
        private string brand;
        private int power;
        private decimal cost;
        private List<DateTime> repairDates;
        private string vin;
        private int yearOfManufacture;
        private double mileage;
        private Color color;
        private DateTime registrationDate;
        private DateTime lastInspectionDate;
        private bool isInsured;
        private DateTime insuranceExpiryDate;

        protected Vehicle(string brand, int power, decimal cost)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Марка не может быть пустой.");

            if (power < 0)
                throw new ArgumentOutOfRangeException(nameof(power), "Мощность должна быть положительной.");

            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Стоимость должна быть положительной.");

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

        public string Brand
        {
            get => brand;
            set => SetProperty(ref brand, value);
        }

        public int Power
        {
            get => power;
            set
            {
                if (value < 0)
                    throw new GeoLocation.PowerNegativeException("Мощность не может быть отрицательной");
                SetProperty(ref power, value);
            }
        }

        public decimal Cost
        {
            get => cost;
            set
            {
                if (value < 0)
                    throw new GeoLocation.CostNegativeException("Стоимость не может быть отрицательной");
                SetProperty(ref cost, value);
            }
        }

        public List<DateTime> RepairDates
        {
            get => repairDates;
        }

        public string VIN
        {
            get => vin;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length != 17)
                    throw new GeoLocation.VinFormatException("VIN должен содержать 17 символов");
                SetProperty(ref vin, value);
            }
        }

        public int YearOfManufacture
        {
            get => yearOfManufacture;
            set
            {
                if (value < 1900 || value > DateTime.Now.Year)
                    throw new ArgumentOutOfRangeException("Год производства должен быть между 1900 и текущим годом");
                SetProperty(ref yearOfManufacture, value);
            }
        }

        public double Mileage
        {
            get => mileage;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Пробег не может быть отрицательным");
                SetProperty(ref mileage, value);
            }
        }

        [JsonIgnore]
        public Color Color
        {
            get => color;
            set => SetProperty(ref color, value);
        }

        public string ColorHex
        {
            get => ColorTranslator.ToHtml(color);
            set
            {
                try
                {
                    Color = ColorTranslator.FromHtml(value);
                }
                catch
                {
                    Color = Color.White;
                }
            }
        }

        public DateTime RegistrationDate
        {
            get => registrationDate;
            set => SetProperty(ref registrationDate, value);
        }

        public DateTime LastInspectionDate
        {
            get => lastInspectionDate;
            set => SetProperty(ref lastInspectionDate, value);
        }

        public bool IsInsured
        {
            get => isInsured;
            set => SetProperty(ref isInsured, value);
        }

        public DateTime InsuranceExpiryDate
        {
            get => insuranceExpiryDate;
            set => SetProperty(ref insuranceExpiryDate, value);
        }

        public int Age => DateTime.Now.Year - YearOfManufacture;

        public abstract string GetInfo();
        public abstract decimal CalculateDepreciation();
        public abstract decimal CalculateInsurancePremium();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged = null!;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }
}