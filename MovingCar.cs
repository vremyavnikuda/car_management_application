using CarManagementApp.Models;

namespace CarManagementApp
{
    public class MovingCar : Car
    {
        // Положение объекта в области отображения
        public PointF CurrentPosition { get; set; }

        // Конечная точка движения
        public PointF Destination { get; set; }

        // Скорость перемещения (в пикселях за итерацию)
        public float Speed { get; set; }

        // Изображение для отображения объекта (можно загрузить из ресурсов)
        public Image CarImage { get; set; }

        // Флаг, контролирующий работу потока
        public bool IsRunning { get; private set; }

        // Объект синхронизации для паузы/возобновления
        public object PauseLock { get; } = new object();

        //TODO:Многопоточность
        private volatile bool isPaused;

        public MovingCar(string brand, int power, decimal cost, CarType carType, string owner = "Не указан")
            : base(brand, power, cost, carType, owner)
        {
            Speed = 2f;
            isPaused = false;
        }

        public void StartMoving()
        {
            IsRunning = true;
            Thread t = new Thread(Move)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            t.Start();
        }

        private void Move()
        {
            try
            {
                while (IsRunning)
                {
                    // Реализация паузы через Monitor
                    lock (PauseLock)
                    {
                        while (isPaused)
                        {
                            // Если поток находится в состоянии паузы, Monitor.Wait заставит его ждать.
                            Monitor.Wait(PauseLock);
                        }
                    }

                    // Вычисляем вектор движения
                    float dx = Destination.X - CurrentPosition.X;
                    float dy = Destination.Y - CurrentPosition.Y;
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    if (distance < Speed)
                    {
                        // Если достигнута конечная точка, генерируем новую случайную конечную точку
                        Destination = GenerateRandomDestination();
                    }
                    else
                    {
                        // Обновляем положение по направлению к Destination
                        float stepX = Speed * dx / distance;
                        float stepY = Speed * dy / distance;
                        CurrentPosition = new PointF(CurrentPosition.X + stepX, CurrentPosition.Y + stepY);
                    }
                    // Пауза между итерациями для плавности движения
                    Thread.Sleep(20);
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                IsRunning = false;
            }
        }

        // Генерация случайной конечной точки в пределах заданной области
        private PointF GenerateRandomDestination()
        {
            Random rnd = new Random();
            // Если объект первого типа, область – верхняя левая четверть (0,0; w/2, h/2)
            // Если второго типа, область – нижняя правая (w/2, h/2; w, h)
            // Здесь предполагается, что логика выбора области реализуется извне.
            // Для примера возвращаем случайную точку в пределах 0..300
            return new PointF(rnd.Next(0, 300), rnd.Next(0, 300));
        }

        public void PauseMoving()
        {
            lock (PauseLock)
            {
                isPaused = true;
            }
        }

        public void ResumeMoving()
        {
            lock (PauseLock)
            {
                isPaused = false;
                Monitor.PulseAll(PauseLock);
            }
        }

        public void StopMoving()
        {
            IsRunning = false;
            lock (PauseLock)
            {
                isPaused = false;
                Monitor.PulseAll(PauseLock);
            }
        }
    }
}