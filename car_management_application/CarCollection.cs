using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CarManagementApp.Models
{
    public class CarCollection
    {
        private List<Car> cars;

        public CarCollection()
        {
            cars = new List<Car>();
        }

        public void AddCar(Car car)
        {
            cars.Add(car);
        }

        public bool RemoveCar(Car car)
        {
            return cars.Remove(car);
        }

        public Car GetCar(int index)
        {
            if (index < 0 || index >= cars.Count)
                throw new IndexOutOfRangeException("Индекс находится за пределами коллекции автомобилей");
            return cars[index];
        }

        public List<Car> GetAllCars()
        {
            return cars;
        }

        public int Count => cars.Count;

        public void SaveToFile(string filePath)
        {
            // Создание опций для сериализации с обработкой циклических ссылок
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(cars, options);
            File.WriteAllText(filePath, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        PropertyNameCaseInsensitive = true
                    };

                    cars = JsonSerializer.Deserialize<List<Car>>(json, options) ?? new List<Car>();
                }
                catch (JsonException ex)
                {
                    Logger.LogException(ex);
                    // Если десериализация не удается => создаем новый пустой список
                    cars = new List<Car>();
                }
            }
        }

        //TODO: Поиск << не хочу пока что делать его
        // А смысл?????
        public List<Car> SearchCars(string query)
        {
            return cars.FindAll(car =>
                car.Brand.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                car.Model.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                car.CarType.ToString().Contains(query, StringComparison.OrdinalIgnoreCase));
        }
    }
}