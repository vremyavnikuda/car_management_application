using CarManagementApp.Models;
using CarManagementApp.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarManagementApp
{
    public partial class Form1 : Form
    {
        private ICarService _carService;
        private ILogService _logService;
        private string _dataFilePath;
        private BindingSource _bindingSource = new BindingSource();

        public Form1()
        {
            InitializeComponent();
            InitializeServices();
            InitializeSortOptions();
            HookEvents();
        }

        /// <summary>
        /// Инициализирует сервисы приложения.
        /// </summary>
        /// <remarks>
        /// Определяет путь для хранения данных (например, в AppData),
        /// создает папку для хранения данных, если она не существует,
        /// создает экземпляр <see cref="LogService"/> и <see cref="CarService"/>,
        /// а также подписывается на событие <see cref="CarService.CollectionChanged"/>
        /// для обновления таблицы при изменении данных.
        /// </remarks>
        private void InitializeServices()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "CarManagementApp");
            Directory.CreateDirectory(folder);
            _dataFilePath = Path.Combine(folder, "cars.json");
            _logService = new LogService(folder);
            _carService = new CarService(_dataFilePath, _logService);
            _carService.CollectionChanged += async (s, e) => await RefreshGridAsync();
        }

        /// <summary>
        /// Инициализирует выпадающий список для сортировки,
        /// добавляя варианты сортировки по марке, году и стоимости.
        /// </summary>
        private void InitializeSortOptions()
        {
            cmbSort.Items.Clear();
            cmbSort.Items.AddRange(new string[] {
                "ФИО",
                "Марка",
                "Год",
                "Стоимость"
            });
            cmbSort.SelectedIndex = 0;
        }

        /// <summary>
        /// Подписывается на события изменений в полях формы.
        /// </summary>
        /// <remarks>
        /// Подписывается на события измения текста в поле поиска,
        /// нажатия кнопок "Добавить", "Изменить", "Удалить", "Импорт JSON", "Экспорт JSON",
        /// "Импорт XML", "Экспорт XML", "Сгенерировать отчет" и изменения выбора в комбо-боксе
        /// для сортировки.
        private void HookEvents()
        {
            txtSearch.TextChanged += async (s, e) => await RefreshGridAsync();
            btnAdd.Click += async (s, e) => await AddCarAsync();
            btnEdit.Click += async (s, e) => await EditCarAsync();
            btnDelete.Click += async (s, e) => await DeleteCarAsync();
            btnImportJson.Click += async (s, e) => await ImportJsonAsync();
            btnExportJson.Click += async (s, e) => await ExportJsonAsync();
            btnImportXml.Click += async (s, e) => await ImportXmlAsync();
            btnExportXml.Click += async (s, e) => await ExportXmlAsync();
            btnGenerateReport.Click += async (s, e) => await GenerateReportAsync();
            cmbSort.SelectedIndexChanged += async (s, e) => await RefreshGridAsync();
        }

        /// <summary>
        /// Обновляет данные таблицы автомобилей на основе поиска и выбранного критерия сортировки.
        /// </summary>
        /// <remarks>
        /// Загружает данные автомобилей, применяет фильтр поиска по бренду и модели, 
        /// сортирует автомобили по выбранному критерию (марка, год или стоимость),
        /// и обновляет источник данных для элемента управления DataGridView.
        /// </remarks>
        private async Task RefreshGridAsync()
        {
            await _carService.LoadDataAsync();
            var cars = await _carService.GetAllCarsAsync();
            string searchText = txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                cars = cars.Where(c => c.ClientName.ToLower().Contains(searchText) || c.Brand.ToLower().Contains(searchText));
            }

            string sortOption = "ФИО"; // Значение по умолчанию
            if (cmbSort.SelectedItem != null)
            {
                sortOption = cmbSort.SelectedItem.ToString()!;
            }

            switch (sortOption)
            {
                case "ФИО":
                    cars = cars.OrderBy(c => c.ClientName);
                    break;
                case "Марка":
                    cars = cars.OrderBy(c => c.Brand);
                    break;
                case "Год":
                    cars = cars.OrderBy(c => c.YearOfManufacture);
                    break;
                case "Стоимость":
                    cars = cars.OrderBy(c => c.Cost);
                    break;
                default:
                    cars = cars.OrderBy(c => c.ClientName);
                    break;
            }

            _bindingSource.DataSource = null;
            _bindingSource.DataSource = cars.ToList();
            dgvCars.DataSource = _bindingSource;

            // Настройка отображения колонок
            dgvCars.Columns.Clear();

            DataGridViewTextBoxColumn clientNameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ClientName",
                HeaderText = "ФИО владельца",
                Width = 150
            };

            DataGridViewTextBoxColumn brandColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Brand",
                HeaderText = "Марка автомобиля",
                Width = 120
            };

            DataGridViewTextBoxColumn creationDateColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreationDate",
                HeaderText = "Дата создания",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" }
            };

            DataGridViewTextBoxColumn repairStartColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RepairStartDate",
                HeaderText = "Начало ремонта",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" }
            };

            DataGridViewTextBoxColumn repairEndColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "RepairEndDate",
                HeaderText = "Окончание ремонта",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy" }
            };

            DataGridViewTextBoxColumn costColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cost",
                HeaderText = "Стоимость работ",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C0" }
            };

            dgvCars.Columns.AddRange(new DataGridViewColumn[]
            {
                clientNameColumn,
                brandColumn,
                creationDateColumn,
                repairStartColumn,
                repairEndColumn,
                costColumn
            });

            // Настройка контекстного меню
            dgvCars.ContextMenuStrip = contextMenu;
        }

        /// <summary>
        /// Добавляет новый автомобиль.
        /// </summary>
        /// <remarks>
        /// Открывает форму редактирования, а затем добавляет автомобиль
        /// в коллекцию и обновляет таблицу.
        /// </remarks>
        private async Task AddCarAsync()
        {
            var form = new CarEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _carService.AddCarAsync(form.EditedCar);
                    await RefreshGridAsync();
                }
                catch (Exception ex)
                {
                    _logService.LogErrorAsync($"Ошибка при добавлении автомобиля: {ex.Message}");
                    MessageBox.Show($"Произошла ошибка при добавлении автомобиля: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Редактирует выбранный автомобиль.
        /// </summary>
        /// <remarks>
        /// Если автомобиль выбран в DataGridView, открывает форму редактирования.
        /// При успешном изменении обновляет данные автомобиля и таблицу.
        /// Если автомобиль не выбран, отображает сообщение с просьбой выбрать автомобиль.
        /// </remarks>
        private async Task EditCarAsync()
        {
            if (dgvCars.CurrentRow?.DataBoundItem is Car selectedCar)
            {
                CarEditForm editForm = new CarEditForm(selectedCar);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    await _carService.UpdateCarAsync(editForm.EditedCar);
                    await RefreshGridAsync();
                }
            }
            else
            {
                MessageBox.Show("Выберите автомобиль для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Удаляет выбранный автомобиль из коллекции.
        /// </summary>
        /// <remarks>
        /// Если автомобиль выбран в DataGridView, отображает диалог подтверждения удаления.
        /// При подтверждении выполняет удаление, обновляет таблицу и отображает сообщение об успешном удалении.
        /// Если автомобиль не выбран, отображает сообщение с просьбой выбрать автомобиль.
        /// </remarks>
        private async Task DeleteCarAsync()
        {
            if (dgvCars.CurrentRow == null)
            {
                MessageBox.Show("Выберите автомобиль для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Car selectedCar = dgvCars.CurrentRow.DataBoundItem as Car;
            if (selectedCar == null) return;

            DialogResult confirmResult = MessageBox.Show(
                "Вы действительно хотите удалить карту клиента?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    await _carService.DeleteCarAsync(selectedCar.Id);
                    await RefreshGridAsync();
                }
                catch (Exception ex)
                {
                    _logService.LogErrorAsync($"Ошибка при удалении автомобиля: {ex.Message}");
                    MessageBox.Show($"Произошла ошибка при удалении автомобиля: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Импортирует данные из файла JSON.
        /// </summary>
        /// <remarks>
        /// Показывает OpenFileDialog для выбора файла JSON, а затем вызывает
        /// метод <see cref="CarService.ImportFromJsonAsync"/> для импорта данных.
        /// </remarks>
        private async Task ImportJsonAsync()
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "JSON Files|*.json" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                bool success = await _carService.ImportFromJsonAsync(ofd.FileName);
                MessageBox.Show(success ? "Импорт JSON выполнен успешно." : "Ошибка импорта JSON.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshGridAsync();
            }
        }

        /// <summary>
        /// Экспортирует данные в файл JSON.
        /// </summary>
        /// <remarks>
        /// Показывает SaveFileDialog для выбора файла JSON, а затем вызывает
        /// метод <see cref="CarService.ExportToJsonAsync"/> для экспорта данных.
        /// </remarks>
        private async Task ExportJsonAsync()
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON Files|*.json" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bool success = await _carService.ExportToJsonAsync(sfd.FileName);
                MessageBox.Show(success ? "Экспорт JSON выполнен успешно." : "Ошибка экспорта JSON.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Импортирует данные из файла XML.
        /// </summary>
        /// <remarks>
        /// Показывает OpenFileDialog для выбора файла XML, а затем вызывает
        /// метод <see cref="CarService.ImportFromXmlAsync"/> для импорта данных.
        /// </remarks>
        private async Task ImportXmlAsync()
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "XML Files|*.xml" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                bool success = await _carService.ImportFromXmlAsync(ofd.FileName);
                MessageBox.Show(success ? "Импорт XML выполнен успешно." : "Ошибка импорта XML.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshGridAsync();
            }
        }

        /// <summary>
        /// Экспортирует данные в файл XML.
        /// </summary>
        /// <remarks>
        /// Вызывает метод <see cref="CarService.ExportToXmlAsync"/> для экспорта данных.
        /// </remarks>
        private async Task ExportXmlAsync()
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "XML Files|*.xml" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                bool success = await _carService.ExportToXmlAsync(sfd.FileName);
                MessageBox.Show(success ? "Экспорт XML выполнен успешно." : "Ошибка экспорта XML.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Генерирует отчет о коллекции автомобилей и отображает его в отдельной форме.
        /// </summary>
        private async Task GenerateReportAsync()
        {
            string report = await _carService.GenerateReportAsync();
            ReportForm reportForm = new ReportForm(report);
            reportForm.ShowDialog();
        }

        /// <summary>
        /// Обработчик события загрузки формы.
        /// </summary>
        /// <remarks>
        /// Вызывает <see cref="RefreshGridAsync"/> для обновления таблицы автомобилей
        /// при загрузке формы.
        /// </remarks>
        private async void Form1_Load(object sender, EventArgs e)
        {
            await RefreshGridAsync();
        }
        private async void editMenuItem_Click(object sender, EventArgs e)
        {
            await EditCarAsync();
        }

        private async void deleteMenuItem_Click(object sender, EventArgs e)
        {
            await DeleteCarAsync();
        }

        private void infoMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvCars.CurrentRow == null) return;

            Car selectedCar = dgvCars.CurrentRow.DataBoundItem as Car;
            if (selectedCar == null) return;

            // Создание формы информации и показ её в режиме только для чтения
            var form = new CarEditForm(selectedCar);
            // Делаем поля недоступными для редактирования
            foreach (Control control in form.Controls)
            {
                if (control is TextBox || control is NumericUpDown || control is DateTimePicker)
                {
                    control.Enabled = false;
                }
            }
            // Скрываем кнопки сохранения и отмены
            form.Controls["btnSave"].Visible = false;
            form.Controls["btnCancel"].Visible = false;

            form.Text = "Информация о клиенте";
            form.ShowDialog();
        }
    }
}
