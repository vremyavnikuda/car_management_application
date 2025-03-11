using CarManagementApp.Models;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CarManagementApp
{
    public partial class CarEditForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Car EditedCar { get; private set; }


        /// <summary>
        /// Инициализирует новый экземпляр формы <see cref="CarEditForm"/> для создания нового автомобиля.
        /// </summary>
        public CarEditForm()
        {
            InitializeComponent();
            EditedCar = new Car();
            dtpRepairStart.Value = DateTime.Now;
            dtpRepairEnd.Value = DateTime.Now.AddDays(3);
            Text = "Создание нового клиента";
        }

        /// <summary>
        /// Инициализирует новый экземпляр формы <see cref="CarEditForm"/> для редактирования существующего автомобиля.
        /// </summary>
        /// <param name="car">Объект <see cref="Car"/>, данные которого будут отображаться и редактироваться в форме.</param>
        /// <remarks>
        /// В этом конструкторе копируются значения полей из объекта <see cref="Car"/> в соответствующие поля формы.
        /// </remarks>
        public CarEditForm(Car car)
        {
            InitializeComponent();
            EditedCar = car;
            txtClientName.Text = car.ClientName;
            txtPhoneNumber.Text = car.PhoneNumber;
            txtIssue.Text = car.Issue;
            txtBrand.Text = car.Brand;
            txtModel.Text = car.Model;
            numYear.Value = car.YearOfManufacture;
            numPower.Value = car.Power;
            numCost.Value = car.Cost;
            txtResponsiblePerson.Text = car.OwnerName;
            txtRepairPerson.Text = car.ClientName;
            txtFullCarInfo.Text = $"{car.Brand} {car.Model}, {car.YearOfManufacture} год, {car.Power} л.с.";
            txtDiagnosis.Text = car.Issue;

            if (car.RepairStartDate.Year > 1900)
                dtpRepairStart.Value = car.RepairStartDate;
            else
                dtpRepairStart.Value = DateTime.Now;

            if (car.RepairEndDate.Year > 1900)
                dtpRepairEnd.Value = car.RepairEndDate;
            else
                dtpRepairEnd.Value = DateTime.Now.AddDays(3);

            Text = "Редактирование карточки клиента";
        }

        /// <summary>
        /// Обработчик события клика по кнопке "Сохранить".
        /// Обновляет свойства изменяемого автомобиля на основе введенных данных и закрывает форму.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка).</param>
        /// <param name="e">Аргументы события.</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show(
                "Вы хотите принять изменения в форме клиента?",
                "Подтверждение изменений",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                UpdateCarFromForm();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Обработчик события клика по кнопке "Отмена".
        /// Отменяет все изменения и закрывает форму.
        /// </summary>
        /// <param name="sender">Объект, вызвавший событие (кнопка).</param>
        /// <param name="e">Аргументы события.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Обновляет свойства автомобиля на основе данных из формы
        /// </summary>
        private void UpdateCarFromForm()
        {
            EditedCar.ClientName = txtClientName.Text;
            EditedCar.PhoneNumber = txtPhoneNumber.Text;
            EditedCar.Issue = txtIssue.Text;
            EditedCar.Brand = txtBrand.Text;
            EditedCar.Model = txtModel.Text;
            EditedCar.YearOfManufacture = (int)numYear.Value;
            EditedCar.Power = (int)numPower.Value;
            EditedCar.Cost = numCost.Value;
            EditedCar.OwnerName = txtResponsiblePerson.Text;
            EditedCar.RepairStartDate = dtpRepairStart.Value;
            EditedCar.RepairEndDate = dtpRepairEnd.Value;
            EditedCar.FullCarInfo = txtFullCarInfo.Text;
            EditedCar.Diagnosis = txtDiagnosis.Text;
            EditedCar.RepairPerson = txtRepairPerson.Text;

            if (EditedCar.CreationDate.Year < 1900)
            {
                EditedCar.CreationDate = DateTime.Now;
            }
        }
    }
}