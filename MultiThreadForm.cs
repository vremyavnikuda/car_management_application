using System.Diagnostics;
using CarManagementApp.Models;
using Exception = System.Exception;
using Timer = System.Windows.Forms.Timer;

namespace CarManagementApp
{
    public partial class MultiThreadForm : Form
    {
        private Panel? displayPanel;
        private Panel? controlPanel;
        private Button? startButton;
        private Button? pauseButton;
        private Button? resumeButton;
        private Button? stopButton;
        private Label? statusLabel;
        private List<MovingCar> movingCarsType1 = new List<MovingCar>();
        private List<MovingCar> movingCarsType2 = new List<MovingCar>();
        private Random rnd = new Random();
        private int displayWidth, displayHeight;
        private Timer? refreshTimer;

        public MultiThreadForm()
        {
            InitializeComponent();
            InitializeMovingCars();
        }

        private void InitializeComponent()
        {
            this.Text = "Многопоточное отображение";
            this.Size = new Size(1000, 800);

            // Панель отображения объектов
            displayPanel = new Panel()
                { Location = new Point(10, 10), Size = new Size(700, 700), BackColor = Color.White };
            // Панель управления
            controlPanel = new Panel()
                { Location = new Point(720, 10), Size = new Size(260, 700), BackColor = Color.LightGray };

            startButton = new Button() { Text = "Старт", Location = new Point(20, 20), Size = new Size(100, 40) };
            pauseButton = new Button() { Text = "Пауза", Location = new Point(20, 80), Size = new Size(100, 40) };
            resumeButton = new Button()
                { Text = "Возобновить", Location = new Point(20, 140), Size = new Size(100, 40) };
            stopButton = new Button() { Text = "Стоп", Location = new Point(20, 200), Size = new Size(100, 40) };
            statusLabel = new Label()
                { Text = "Статус: не запущено", Location = new Point(20, 260), Size = new Size(220, 40) };

            startButton.Click += StartButton_Click;
            pauseButton.Click += PauseButton_Click;
            resumeButton.Click += ResumeButton_Click;
            stopButton.Click += StopButton_Click;

            controlPanel.Controls.AddRange(new Control[]
                { startButton, pauseButton, resumeButton, stopButton, statusLabel });
            this.Controls.AddRange(new Control[] { displayPanel, controlPanel });

            displayWidth = displayPanel.Width;
            displayHeight = displayPanel.Height;

            // Таймер для обновления графики
            refreshTimer = new Timer();
            refreshTimer.Interval = 100;
            refreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
            displayPanel.Paint += DisplayPanel_Paint;
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                Debug.Assert(displayPanel != null, nameof(displayPanel) + " != null");
                displayPanel.Invalidate();
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
            }
        }

        private void InitializeMovingCars()
        {
            try
            {
                // Загружаем коллекцию автомобилей из файла "cars.json"
                CarCollection carCollection = new CarCollection();
                carCollection.LoadFromFile("cars.json");
                var cars = carCollection.GetAllCars();

                // Если автомобилей нет, выводим предупреждение
                if (cars.Count == 0)
                {
                    MessageBox.Show("В коллекции нет автомобилей. Добавьте автомобили в главное окно.",
                        "Предупреждение");
                    return;
                }

                // Разбиваем список на две части: первая половина – для верхней левой четверти, вторая – для нижней правой
                int halfCount = cars.Count / 2;

                for (int i = 0; i < cars.Count; i++)
                {
                    var car = cars[i];

                    // Создаём MovingCar, используя свойства базового объекта Car
                    MovingCar movingCar = new MovingCar(car.Brand, car.Power, car.Cost, car.CarType, car.OwnerName);

                    // При необходимости можно скопировать и другие свойства, например пробег, модель и т.д.
                    if (i < halfCount)
                    {
                        // Верхняя левая четверть (0,0) до (displayWidth/2, displayHeight/2)
                        movingCar.CurrentPosition =
                            new PointF(rnd.Next(0, displayWidth / 2), rnd.Next(0, displayHeight / 2));
                        movingCar.Destination =
                            new PointF(rnd.Next(0, displayWidth / 2), rnd.Next(0, displayHeight / 2));
                        movingCarsType1.Add(movingCar);
                    }
                    else
                    {
                        // Нижняя правая четверть (displayWidth/2, displayHeight/2) до (displayWidth, displayHeight)
                        movingCar.CurrentPosition = new PointF(rnd.Next(displayWidth / 2, displayWidth),
                            rnd.Next(displayHeight / 2, displayHeight));
                        movingCar.Destination = new PointF(rnd.Next(displayWidth / 2, displayWidth),
                            rnd.Next(displayHeight / 2, displayHeight));
                        movingCarsType2.Add(movingCar);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                MessageBox.Show("Ошибка при инициализации объектов движения: " + exception.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            try
            {
                foreach (var car in movingCarsType1)
                    car.StartMoving();
                foreach (var car in movingCarsType2)
                    car.StartMoving();
                Debug.Assert(refreshTimer != null, nameof(refreshTimer) + " != null");
                refreshTimer.Start();
                Debug.Assert(statusLabel != null, nameof(statusLabel) + " != null");
                statusLabel.Text = "Статус: запущено";
                Logger.Log("Запуск движения всех объектов");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                MessageBox.Show("Произошла ошибка при запуске движения", "Ошибка" + exception.Message);
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var car in movingCarsType1)
                    car.PauseMoving();
                foreach (var car in movingCarsType2)
                    car.PauseMoving();
                Debug.Assert(statusLabel != null, nameof(statusLabel) + " != null");
                statusLabel.Text = "Статус: на паузе";
                Logger.Log("Все объекты поставлены на паузу.");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                MessageBox.Show("Ошибка при постановке на паузу: " + exception.Message);
            }
        }

        private void ResumeButton_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var car in movingCarsType1)
                    car.ResumeMoving();
                foreach (var car in movingCarsType2)
                    car.ResumeMoving();
                statusLabel!.Text = "Статус: запущено";
                Logger.Log("Все объекты возобновили движение.");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                MessageBox.Show("Ошибка при возобновлении: " + exception.Message);
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            try
            {
                foreach (var car in movingCarsType1)
                    car.StopMoving();
                foreach (var car in movingCarsType2)
                    car.StopMoving();
                refreshTimer!.Stop();
                statusLabel!.Text = "Статус: остановлено";
                Logger.Log("Все объекты остановлены.");
            }
            catch (Exception exception)
            {
                Logger.LogException(exception);
                MessageBox.Show("Ошибка при остановке: " + exception.Message);
            }
        }

        private void DisplayPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Отрисовка объектов: для примера представляем их как эллипсы
            foreach (var car in movingCarsType1)
            {
                g.FillEllipse(Brushes.Blue, car.CurrentPosition.X, car.CurrentPosition.Y, 10, 10);
            }

            foreach (var car in movingCarsType2)
            {
                g.FillEllipse(Brushes.Red, car.CurrentPosition.X, car.CurrentPosition.Y, 10, 10);
            }
        }
    }
}