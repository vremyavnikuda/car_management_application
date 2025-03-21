﻿using System;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using CarManagementApp.Models;
using lab_1;
using MenuStrip = System.Windows.Forms.MenuStrip;

namespace CarManagementApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            carCollection = new CarCollection();
            carCollection.LoadFromFile(dataFilePath);
            UpdateCarList();
            UpdateConnectionMenuItem();
        }

        private void InitializeComponent()
        {
            // Создание компонентов
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.addToolStripMenuItem = new ToolStripMenuItem();
            this.editToolStripMenuItem = new ToolStripMenuItem();
            this.deleteToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.toolStrip = new ToolStrip();
            this.addButton = new ToolStripButton();
            this.editButton = new ToolStripButton();
            this.deleteButton = new ToolStripButton();
            this.carListBox = new ListBox();
            this.detailsPanel = new Panel();
            this.brandLabel = new Label();
            this.powerLabel = new Label();
            this.costLabel = new Label();
            this.typeLabel = new Label();
            this.repairDatesLabel = new Label();
            this.repairDatesListBox = new ListBox();
            this.ownerNameLabel = new Label();
            this.ownerNameTextBox = new TextBox();


            // Настройка формы
            this.Text = "Управление автомобилями";
            this.Size = new Size(800, 600);

            // Настройка меню
            this.menuStrip.Items.AddRange(new ToolStripItem[]
            {
                fileToolStripMenuItem, helpToolStripMenuItem
            });

            this.multiThreadMenuItem = new ToolStripMenuItem();
            this.multiThreadMenuItem.Text = "Многопоточность";
            this.multiThreadMenuItem.Click += new EventHandler(OpenMultiThreadForm);
            this.menuStrip.Items.Add(this.multiThreadMenuItem);

            // Добавлено: меню всегда сверху
            this.menuStrip.Dock = DockStyle.Top;

            this.fileToolStripMenuItem.Text = "Файл";
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                addToolStripMenuItem, editToolStripMenuItem, deleteToolStripMenuItem,dataToolStripMenuItem = new
                    ToolStripMenuItem("Данные",null,OpenDataForm)
            });

            this.addToolStripMenuItem.Text = "Добавить";
            this.addToolStripMenuItem.Click += new EventHandler(AddCar);

            this.editToolStripMenuItem.Text = "Редактировать";
            this.editToolStripMenuItem.Click += new EventHandler(EditCar);

            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new EventHandler(DeleteCar);

            this.helpToolStripMenuItem.Text = "Справка";
            this.helpToolStripMenuItem.Click += new EventHandler(ShowHelp);

            // Настройка панели инструментов (скрыта)
            this.toolStrip.Visible = false;

            this.addButton = new ToolStripButton();
            this.addButton.Text = "Добавить";
            this.addButton.Click += new EventHandler(AddCar);

            this.editButton = new ToolStripButton();
            this.editButton.Text = "Редактировать";
            this.editButton.Click += new EventHandler(EditCar);

            this.deleteButton = new ToolStripButton();
            this.deleteButton.Text = "Удалить";
            this.deleteButton.Click += new EventHandler(DeleteCar);

            // Настройка списка автомобилей
            this.carListBox.Location = new Point(10, 70);
            this.carListBox.Size = new Size(300, 450);
            // Добавлено: растягивается по вертикали
            this.carListBox.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.carListBox.SelectedIndexChanged += new EventHandler(CarSelected);

            // Настройка панели деталей
            this.detailsPanel.Location = new Point(320, 70);
            this.detailsPanel.Size = new Size(450, 450);
            // Добавлено: растягивается по вертикали и горизонтали
            this.detailsPanel.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left |
                AnchorStyles.Right;
            this.detailsPanel.BorderStyle = BorderStyle.FixedSingle;

            // Добавление элементов на панель деталей с настройкой Anchor
            this.brandLabel.Location = new Point(10, 10);
            this.brandLabel.Size = new Size(430, 20);
            this.brandLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.powerLabel.Location = new Point(10, 40);
            this.powerLabel.Size = new Size(430, 20);
            this.powerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.costLabel.Location = new Point(10, 70);
            this.costLabel.Size = new Size(430, 20);
            this.costLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.typeLabel.Location = new Point(10, 100);
            this.typeLabel.Size = new Size(430, 20);
            this.typeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.repairDatesLabel.Location = new Point(10, 130);
            this.repairDatesLabel.Text = "Даты ремонта:";
            this.repairDatesLabel.Size = new Size(430, 20);
            this.repairDatesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.repairDatesListBox.Location = new Point(10, 160);
            this.repairDatesListBox.Size = new Size(430, 200);
            this.repairDatesListBox.Anchor =
                AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.ownerNameLabel.Location = new Point(10, 370);
            this.ownerNameLabel.Size = new Size(430, 20);
            this.ownerNameLabel.Text = "ФИО Владельца:";
            this.ownerNameLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.ownerNameTextBox.Location = new Point(10, 400);
            this.ownerNameTextBox.Size = new Size(430, 20);
            this.ownerNameTextBox.ReadOnly = true;
            this.ownerNameTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Добавление элементов на панель деталей
            this.detailsPanel.Controls.AddRange(new Control[]
            {
                brandLabel, powerLabel, costLabel, typeLabel,
                repairDatesLabel, repairDatesListBox,
                ownerNameLabel, ownerNameTextBox
            });

            // Добавление элементов на форму
            this.Controls.AddRange(new Control[]
            {
                menuStrip, toolStrip, carListBox, detailsPanel
            });
            this.MainMenuStrip = this.menuStrip;

            this.connectionToolStripMenuItem = new ToolStripMenuItem();
            this.connectionToolStripMenuItem.Text = "Подключиться";
            this.connectionToolStripMenuItem.Click += new EventHandler(ConnectionMenuItem_Click);
            this.fileToolStripMenuItem.DropDownItems.Add(this.connectionToolStripMenuItem);

            //TODO: connection status indicator
            _ConnectionStatusIndicator();
        }

        private void OpenDataForm(object sender,EventArgs eventArgs)
        {
            if (!isConnected || client == null || !client.Connected)
            {
                MessageBox.Show("Сначала подключитесь к серверу", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataForm dataform = new DataForm(carCollection, client);
            dataform.ShowDialog();
        }

        //TODO: task_1
        private void ConnectionMenuItem_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                OpenConnectionForm(sender, e);
            }
            else
            {
                DisconectedServer();
            }
        }

        private void DisconectedServer()
        {
            try
            {
                if (client != null && client.Connected)
                {
                    client.Close();
                    client.Dispose();
                    client = null;
                    isConnected = false;
                    UpdateConnectionStatus(false,null);
                    MessageBox.Show("Сервер отключен", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Fatal connection ERROR: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void OpenMultiThreadForm(object sender, EventArgs e)
        {
            MultiThreadForm multiThreadForm = new MultiThreadForm();
            multiThreadForm.ShowDialog();
        }

        private void OpenConnectionForm(object sender, EventArgs e)
        {
            ConnectionForm connectionForm = new ConnectionForm();
            connectionForm.OnConnectionStatusChanged += UpdateConnectionStatus;
            connectionForm.Show();
        }

        private void UpdateConnectionStatus(bool isConnected, TcpClient tcpClient)
        {
            this.isConnected = isConnected;
            if (isConnected && tcpClient != null)
            {
                this.client = tcpClient;
            }
            _connectionStatusIndicator.BackColor = isConnected ? Color.Green : Color.Red;
            UpdateConnectionMenuItem();
        }
        
        private void UpdateConnectionMenuItem()
        {
            connectionToolStripMenuItem.Text = isConnected ? "Отключиться" : "Подключиться";
        }

        private void UpdateCarList()
        {
            carListBox.Items.Clear();
            foreach (Car car in carCollection.GetAllCars())
            {
                carListBox.Items.Add(car.GetInfo());
            }
        }

        private void _ConnectionStatusIndicator()
        {
            _connectionStatusIndicator = new PictureBox();
            _connectionStatusIndicator.Location = new Point(20, 550);
            _connectionStatusIndicator.Size = new Size(10, 10);
            _connectionStatusIndicator.BackColor = isConnected? Color.Green : Color.Red;
            this.Controls.Add(_connectionStatusIndicator);
        }

        private void ShowCarDetails(Car car)
        {
            if (car == null)
            {
                ClearDetails();
                return;
            }

            brandLabel.Text = $"Марка: {car.Brand} {car.Model}";
            powerLabel.Text = $"Мощность: {car.Power} л.с.";
            costLabel.Text = $"Стоимость: {car.Cost:C}";
            typeLabel.Text = $"Тип: {car.CarType}";
            ownerNameTextBox.Text = car.OwnerName;
            repairDatesListBox.Items.Clear();
            foreach (DateTime date in car.RepairDates)
            {
                repairDatesListBox.Items.Add(date.ToShortDateString());
            }
        }

        private void ClearDetails()
        {
            brandLabel.Text = "";
            powerLabel.Text = "";
            costLabel.Text = "";
            typeLabel.Text = "";
            ownerNameTextBox.Text = "";
            repairDatesListBox.Items.Clear();
        }

        private void CarSelected(object? sender, EventArgs e)
        {
            if (carListBox.SelectedIndex >= 0)
            {
                Car selectedCar = carCollection.GetCar(carListBox.SelectedIndex);
                ShowCarDetails(selectedCar);
            }
        }

        private void AddCar(object? sender, EventArgs e)
        {
            CarDialog dialog = new CarDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                carCollection.AddCar(dialog.Car);
                carCollection.SaveToFile(dataFilePath);
                UpdateCarList();
            }
        }

        private void EditCar(object? sender, EventArgs e)
        {
            if (carListBox.SelectedIndex >= 0)
            {
                Car selectedCar = carCollection.GetCar(carListBox.SelectedIndex);
                CarDialog dialog = new CarDialog(selectedCar);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    carCollection.RemoveCar(selectedCar);
                    carCollection.AddCar(dialog.Car);
                    carCollection.SaveToFile(dataFilePath);
                    UpdateCarList();
                    ShowCarDetails(dialog.Car);
                }
            }
            else
            {
                MessageBox.Show("Выберите автомобиль для редактирования", "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void DeleteCar(object? sender, EventArgs e)
        {
            if (carListBox.SelectedIndex >= 0)
            {
                Car selectedCar = carCollection.GetCar(carListBox.SelectedIndex);

                if (MessageBox.Show($"Вы уверены, что хотите удалить {selectedCar.OwnerName} {selectedCar.Brand}?",
                        "Подтверждение удаления", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    carCollection.RemoveCar(selectedCar);
                    carCollection.SaveToFile(dataFilePath);
                    UpdateCarList();
                    ClearDetails();
                }
            }
            else
            {
                MessageBox.Show("Выберите автомобиль для удаления", "Предупреждение");
            }
        }

        private void ShowHelp(object? sender, EventArgs e)
        {
            MessageBox.Show(
                "Andrew Nevsky\n\n" +
                "github.com/vremyavnikuda\n"
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                carCollection.SaveToFile(dataFilePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Ошибка сохранения данных: {exception.Message}", "Ошибка");
                throw;
            }

            base.OnFormClosing(e);
        }

        // Поля для компонентов
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStrip toolStrip;
        private ToolStripButton addButton;
        private ToolStripButton editButton;
        private ToolStripButton deleteButton;
        private ListBox carListBox;
        private Panel detailsPanel;
        private Label brandLabel;
        private Label powerLabel;
        private Label costLabel;
        private Label typeLabel;
        private Label repairDatesLabel;
        private ListBox repairDatesListBox;
        private Label ownerNameLabel;
        private TextBox ownerNameTextBox;
        private ToolStripMenuItem multiThreadMenuItem;
        private readonly CarCollection carCollection;
        private readonly string dataFilePath = "cars.json";
        private TcpClient client;
        private NetworkStream stream;
        private List<TcpClient> connectedClients = new List<TcpClient>();
        private Thread listenerThread;
        private ToolStripMenuItem connectionToolStripMenuItem;
        //TODO: add indicator connection server
        private PictureBox _connectionStatusIndicator;
        //TODO: task_1
        private bool isConnected = false;
        private ToolStripMenuItem dataToolStripMenuItem;
    }
}