using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CarManagementApp.Models;
using Timer = System.Windows.Forms.Timer;

namespace CarManagementApp
{
    public partial class MultiThreadForm : Form
    {
        private Panel displayPanel;
        private Panel controlPanel;
        private Button startButton, pauseButton, resumeButton, stopButton;
        private Label statusLabel;
        private List<MovingCar> movingCarsType1 = new List<MovingCar>();
        private List<MovingCar> movingCarsType2 = new List<MovingCar>();
        private Random rnd = new Random();
        private int displayWidth, displayHeight;
        private Timer refreshTimer;

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
            displayPanel = new Panel() { Location = new Point(10, 10), Size = new Size(700, 700), BackColor = Color.White };
            // Панель управления
            controlPanel = new Panel() { Location = new Point(720, 10), Size = new Size(260, 700), BackColor = Color.LightGray };

            startButton = new Button() { Text = "Старт", Location = new Point(20, 20), Size = new Size(100, 40) };
            pauseButton = new Button() { Text = "Пауза", Location = new Point(20, 80), Size = new Size(100, 40) };
            resumeButton = new Button() { Text = "Возобновить", Location = new Point(20, 140), Size = new Size(100, 40) };
            stopButton = new Button() { Text = "Стоп", Location = new Point(20, 200), Size = new Size(100, 40) };
            statusLabel = new Label() { Text = "Статус: не запущено", Location = new Point(20, 260), Size = new Size(220, 40) };

            startButton.Click += StartButton_Click;
            pauseButton.Click += PauseButton_Click;
            resumeButton.Click += ResumeButton_Click;
            stopButton.Click += StopButton_Click;

            controlPanel.Controls.AddRange(new Control[] { startButton, pauseButton, resumeButton, stopButton, statusLabel });
            this.Controls.AddRange(new Control[] { displayPanel, controlPanel });

            displayWidth = displayPanel.Width;
            displayHeight = displayPanel.Height;

            // Таймер для обновления графики
            refreshTimer = new Timer();
            refreshTimer.Interval = 50;
            refreshTimer.Tick += (s, e) => displayPanel.Invalidate();
            displayPanel.Paint += DisplayPanel_Paint;
        }

        private void InitializeMovingCars()
        {
            // Создание случайного числа объектов от 5 до 10 для каждого типа
            int countType1 = rnd.Next(5, 11);
            int countType2 = rnd.Next(5, 11);

            // Тип 1: объекты движутся в верхнюю левую четверть (0,0) до (w/2, h/2)
            for (int i = 0; i < countType1; i++)
            {
                MovingCar car = new MovingCar("Mazda", 150, 1000000, CarType.Sedan, "Type1");
                car.CurrentPosition = new PointF(rnd.Next(0, displayWidth / 2), rnd.Next(0, displayHeight / 2));
                car.Destination = new PointF(rnd.Next(0, displayWidth / 2), rnd.Next(0, displayHeight / 2));
                // Загрузка изображения (можно использовать Image.FromFile("path")) – для примера не добавляем.
                movingCarsType1.Add(car);
            }

            // Тип 2: объекты движутся в нижнюю правую четверть (w/2, h/2) до (w, h)
            for (int i = 0; i < countType2; i++)
            {
                MovingCar car = new MovingCar("Nissan", 130, 900000, CarType.Hatchback, "Type2");
                car.CurrentPosition = new PointF(rnd.Next(displayWidth / 2, displayWidth), rnd.Next(displayHeight / 2, displayHeight));
                car.Destination = new PointF(rnd.Next(displayWidth / 2, displayWidth), rnd.Next(displayHeight / 2, displayHeight));
                movingCarsType2.Add(car);
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            foreach (var car in movingCarsType1)
                car.StartMoving();
            foreach (var car in movingCarsType2)
                car.StartMoving();
            refreshTimer.Start();
            statusLabel.Text = "Статус: запущено";
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            // Здесь можно реализовать паузу, используя Monitor. Пример: вызывать Pause() для каждого объекта.
            foreach (var car in movingCarsType1)
                car.Pause();
            foreach (var car in movingCarsType2)
                car.Pause();
            statusLabel.Text = "Статус: на паузе";
        }

        private void ResumeButton_Click(object sender, EventArgs e)
        {
            foreach (var car in movingCarsType1)
                car.Resume();
            foreach (var car in movingCarsType2)
                car.Resume();
            statusLabel.Text = "Статус: запущено";
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            foreach (var car in movingCarsType1)
                car.StopMoving();
            foreach (var car in movingCarsType2)
                car.StopMoving();
            refreshTimer.Stop();
            statusLabel.Text = "Статус: остановлено";
        }

        private void DisplayPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            // Отрисовка объектов: для примера представляем их как эллипсы
            foreach (var car in movingCarsType1)
            {
                g.FillEllipse(Brushes.Blue, car.CurrentPosition.X, car.CurrentPosition.Y, 20, 10);
            }
            foreach (var car in movingCarsType2)
            {
                g.FillEllipse(Brushes.Red, car.CurrentPosition.X, car.CurrentPosition.Y, 20, 10);
            }
        }
    }
}
