using System;
using System.Drawing;
using System.Windows.Forms;
using CarManagementApp.Models;

namespace CarManagementApp
{
    public class CarDialog : Form
    {
        private Car car;
        private TextBox brandTextBox;
        private TextBox modelTextBox;
        private TextBox powerTextBox;
        private TextBox costTextBox;
        private ComboBox typeComboBox;
        private ComboBox fuelTypeComboBox;
        private TextBox doorsTextBox;
        private TextBox seatsTextBox;
        private ComboBox transmissionComboBox;
        private ListBox repairDatesListBox;
        private Button addDateButton;
        private Button removeDateButton;
        private Button okButton;
        private Button cancelButton;
        private ErrorProvider errorProvider;
        private Label ownerNameLabel;
        public TextBox ownerNameTextBox;
        private Label licensePlateLabel;
        public TextBox licensePlateTextBox;

        public Car Car => car;
        public string OwnerName => ownerNameTextBox.Text;

        public CarDialog()
        {
            InitializeComponent();
            car = new Car("Неизвестно", 0, 0, CarType.Sedan);
        }

        public CarDialog(Car existingCar)
        {
            InitializeComponent();
            car = new Car(existingCar.Brand, existingCar.Power, existingCar.Cost, existingCar.CarType, existingCar.OwnerName);

            // Копируем свойства из существующего автомобиля
            car.Model = existingCar.Model;
            car.FuelType = existingCar.FuelType;
            car.Doors = existingCar.Doors;
            car.Seats = existingCar.Seats;
            car.Transmission = existingCar.Transmission;
            car.LicensePlate = existingCar.LicensePlate;
            car.Mileage = existingCar.Mileage;
            car.YearOfManufacture = existingCar.YearOfManufacture;
            car.VIN = existingCar.VIN;
            car.ColorHex = existingCar.ColorHex;

            // Копируем даты ремонта
            foreach (var date in existingCar.RepairDates)
            {
                car.AddRepairDate(date);
            }

            // Заполняем элементы формы данными
            brandTextBox.Text = car.Brand;
            modelTextBox.Text = car.Model;
            powerTextBox.Text = car.Power.ToString();
            costTextBox.Text = car.Cost.ToString();
            typeComboBox.SelectedItem = car.CarType.ToString();
            fuelTypeComboBox.SelectedItem = car.FuelType.ToString();
            doorsTextBox.Text = car.Doors.ToString();
            seatsTextBox.Text = car.Seats.ToString();
            transmissionComboBox.SelectedItem = car.Transmission;
            ownerNameTextBox.Text = car.OwnerName;
            licensePlateTextBox.Text = car.LicensePlate;

            UpdateRepairDatesList();
        }

        private void InitializeComponent()
        {
            // Настройка формы
            this.Text = "Данные автомобиля";
            this.Size = new Size(400, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Добавление поля для ФИО владельца
            ownerNameLabel = new Label();
            ownerNameLabel.Text = "ФИО Владельца:";
            ownerNameLabel.Location = new Point(20, 20);
            ownerNameLabel.Size = new Size(100, 20);

            ownerNameTextBox = new TextBox();
            ownerNameTextBox.Location = new Point(130, 20);
            ownerNameTextBox.Size = new Size(230, 20);

            // Добавление поля для номерного знака
            licensePlateLabel = new Label();
            licensePlateLabel.Text = "Номерной знак:";
            licensePlateLabel.Location = new Point(20, 50);
            licensePlateLabel.Size = new Size(100, 20);

            licensePlateTextBox = new TextBox();
            licensePlateTextBox.Location = new Point(130, 50);
            licensePlateTextBox.Size = new Size(230, 20);

            // Создание компонентов
            Label brandLabel = new Label();
            brandLabel.Text = "Марка:";
            brandLabel.Location = new Point(20, 80);
            brandLabel.Size = new Size(100, 20);

            brandTextBox = new TextBox();
            brandTextBox.Location = new Point(130, 80);
            brandTextBox.Size = new Size(230, 20);

            Label modelLabel = new Label();
            modelLabel.Text = "Модель:";
            modelLabel.Location = new Point(20, 110);
            modelLabel.Size = new Size(100, 20);

            modelTextBox = new TextBox();
            modelTextBox.Location = new Point(130, 110);
            modelTextBox.Size = new Size(230, 20);

            Label powerLabel = new Label();
            powerLabel.Text = "Мощность (л.с.):";
            powerLabel.Location = new Point(20, 140);
            powerLabel.Size = new Size(100, 20);

            powerTextBox = new TextBox();
            powerTextBox.Location = new Point(130, 140);
            powerTextBox.Size = new Size(230, 20);

            Label costLabel = new Label();
            costLabel.Text = "Стоимость:";
            costLabel.Location = new Point(20, 170);
            costLabel.Size = new Size(100, 20);

            costTextBox = new TextBox();
            costTextBox.Location = new Point(130, 170);
            costTextBox.Size = new Size(230, 20);

            Label typeLabel = new Label();
            typeLabel.Text = "Тип:";
            typeLabel.Location = new Point(20, 200);
            typeLabel.Size = new Size(100, 20);

            typeComboBox = new ComboBox();
            typeComboBox.Location = new Point(130, 200);
            typeComboBox.Size = new Size(230, 20);
            typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            typeComboBox.Items.AddRange(Enum.GetNames(typeof(CarType)));
            typeComboBox.SelectedIndex = 0;

            Label fuelTypeLabel = new Label();
            fuelTypeLabel.Text = "Тип топлива:";
            fuelTypeLabel.Location = new Point(20, 230);
            fuelTypeLabel.Size = new Size(100, 20);

            fuelTypeComboBox = new ComboBox();
            fuelTypeComboBox.Location = new Point(130, 230);
            fuelTypeComboBox.Size = new Size(230, 20);
            fuelTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fuelTypeComboBox.Items.AddRange(Enum.GetNames(typeof(FuelType)));
            fuelTypeComboBox.SelectedIndex = 0;

            Label doorsLabel = new Label();
            doorsLabel.Text = "Количество дверей:";
            doorsLabel.Location = new Point(20, 260);
            doorsLabel.Size = new Size(100, 20);

            doorsTextBox = new TextBox();
            doorsTextBox.Location = new Point(130, 260);
            doorsTextBox.Size = new Size(230, 20);
            doorsTextBox.Text = "4";

            Label seatsLabel = new Label();
            seatsLabel.Text = "Количество мест:";
            seatsLabel.Location = new Point(20, 290);
            seatsLabel.Size = new Size(100, 20);

            seatsTextBox = new TextBox();
            seatsTextBox.Location = new Point(130, 290);
            seatsTextBox.Size = new Size(230, 20);
            seatsTextBox.Text = "5";

            Label transmissionLabel = new Label();
            transmissionLabel.Text = "Коробка передач:";
            transmissionLabel.Location = new Point(20, 320);
            transmissionLabel.Size = new Size(100, 20);

            transmissionComboBox = new ComboBox();
            transmissionComboBox.Location = new Point(130, 320);
            transmissionComboBox.Size = new Size(230, 20);
            transmissionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            transmissionComboBox.Items.AddRange(new string[] { "Механическая", "Автоматическая", "Роботизированная", "Вариатор" });
            transmissionComboBox.SelectedIndex = 0;

            Label repairDatesLabel = new Label();
            repairDatesLabel.Text = "Даты ремонта:";
            repairDatesLabel.Location = new Point(20, 350);
            repairDatesLabel.Size = new Size(100, 20);

            repairDatesListBox = new ListBox();
            repairDatesListBox.Location = new Point(20, 380);
            repairDatesListBox.Size = new Size(340, 120);

            addDateButton = new Button();
            addDateButton.Text = "Добавить дату";
            addDateButton.Location = new Point(20, 510);
            addDateButton.Size = new Size(160, 30);
            addDateButton.Click += new EventHandler(AddRepairDate);

            removeDateButton = new Button();
            removeDateButton.Text = "Удалить дату";
            removeDateButton.Location = new Point(200, 510);
            removeDateButton.Size = new Size(160, 30);
            removeDateButton.Click += new EventHandler(RemoveRepairDate);

            okButton = new Button();
            okButton.Text = "OK";
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(20, 550);
            okButton.Size = new Size(160, 30);
            okButton.Click += new EventHandler(SaveCar);

            cancelButton = new Button();
            cancelButton.Text = "Отмена";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(200, 550);
            cancelButton.Size = new Size(160, 30);

            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Добавление компонентов на форму
            this.Controls.AddRange(new Control[] {
                ownerNameLabel, ownerNameTextBox,
                licensePlateLabel, licensePlateTextBox,
                brandLabel, brandTextBox,
                modelLabel, modelTextBox,
                powerLabel, powerTextBox,
                costLabel, costTextBox,
                typeLabel, typeComboBox,
                fuelTypeLabel, fuelTypeComboBox,
                doorsLabel, doorsTextBox,
                seatsLabel, seatsTextBox,
                transmissionLabel, transmissionComboBox,
                repairDatesLabel, repairDatesListBox,
                addDateButton, removeDateButton,
                okButton, cancelButton
            });

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void AddRepairDate(object sender, EventArgs e)
        {
            using (DateTimePicker dateTimePicker = new DateTimePicker())
            {
                Form dateForm = new Form();
                dateForm.Text = "Выберите дату ремонта";
                dateForm.Size = new Size(300, 200);
                dateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                dateForm.StartPosition = FormStartPosition.CenterParent;
                dateForm.MinimizeBox = false;
                dateForm.MaximizeBox = false;

                dateTimePicker.Format = DateTimePickerFormat.Short;
                dateTimePicker.Location = new Point(50, 50);
                dateTimePicker.Size = new Size(200, 20);

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;
                okButton.Location = new Point(50, 100);
                okButton.Size = new Size(75, 30);

                Button cancelButton = new Button();
                cancelButton.Text = "Отмена";
                cancelButton.DialogResult = DialogResult.Cancel;
                cancelButton.Location = new Point(175, 100);
                cancelButton.Size = new Size(75, 30);

                dateForm.Controls.AddRange(new Control[] { dateTimePicker, okButton, cancelButton });
                dateForm.AcceptButton = okButton;
                dateForm.CancelButton = cancelButton;

                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    car.AddRepairDate(dateTimePicker.Value);
                    UpdateRepairDatesList();
                }
            }
        }

        private void RemoveRepairDate(object sender, EventArgs e)
        {
            if (repairDatesListBox.SelectedIndex >= 0)
            {
                DateTime selectedDate = car.RepairDates[repairDatesListBox.SelectedIndex];
                car.RemoveRepairDate(selectedDate);
                UpdateRepairDatesList();
            }
            else
            {
                MessageBox.Show("Выберите дату для удаления", "Предупреждение");
            }
        }

        private void UpdateRepairDatesList()
        {
            repairDatesListBox.Items.Clear();
            foreach (DateTime date in car.RepairDates)
            {
                repairDatesListBox.Items.Add(date.ToShortDateString());
            }
        }

        private void SaveCar(object sender, EventArgs e)
        {
            try
            {
                car.Brand = brandTextBox.Text;
                car.Model = modelTextBox.Text;
                car.Power = int.Parse(powerTextBox.Text);
                car.Cost = decimal.Parse(costTextBox.Text);
                car.CarType = (CarType)Enum.Parse(typeof(CarType), typeComboBox.SelectedItem.ToString());
                car.FuelType = (FuelType)Enum.Parse(typeof(FuelType), fuelTypeComboBox.SelectedItem.ToString());
                car.Doors = int.Parse(doorsTextBox.Text);
                car.Seats = int.Parse(seatsTextBox.Text);
                car.Transmission = transmissionComboBox.SelectedItem.ToString();
                car.OwnerName = ownerNameTextBox.Text;
                car.LicensePlate = licensePlateTextBox.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}