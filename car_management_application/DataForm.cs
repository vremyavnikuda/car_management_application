using System;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using CarManagementApp.Models;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace CarManagementApp
{
    public class DataForm : Form
    {
        private readonly CarCollection carCollection;
        private readonly TcpClient client;
        private Button sendButton;
        private Button requestButton;
        private Label statusLabel;

        public DataForm(CarCollection carCollection, TcpClient client)
        {
            this.carCollection = carCollection;
            this.client = client;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Синхронизация данных";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            sendButton = new Button
            {
                Text = "Отправить данные на сервер",
                Location = new Point(20, 20),
                Size = new Size(340, 30)
            };
            sendButton.Click += SendButton_Click;

            requestButton = new Button
            {
                Text = "Запросить данные с сервера",
                Location = new Point(20, 60),
                Size = new Size(340, 30)
            };
            requestButton.Click += RequestButton_Click;

            statusLabel = new Label
            {
                Text = "Статус: ожидание действия",
                Location = new Point(20, 100),
                Size = new Size(340, 20)
            };

            this.Controls.AddRange(new Control[] { sendButton, requestButton, statusLabel });
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                string jsonData = JsonSerializer.Serialize(carCollection.GetAllCars(), options);
                byte[] data = Encoding.UTF8.GetBytes($"SEND_DATA:{jsonData}");
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                statusLabel.Text = "Статус: данные отправлены на сервер";
                MessageBox.Show("Данные успешно отправлены на сервер!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Статус: ошибка при отправке";
                MessageBox.Show($"Ошибка при отправке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RequestButton_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] request = Encoding.UTF8.GetBytes("REQUEST_DATA");
                NetworkStream stream = client.GetStream();
                stream.Write(request, 0, request.Length);

                byte[] buffer = new byte[65536]; // Large enough to handle JSON data
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (response.StartsWith("DATA:"))
                {
                    string jsonData = response.Substring("DATA:".Length);
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        PropertyNameCaseInsensitive = true
                    };
                    List<Car> receivedCars = JsonSerializer.Deserialize<List<Car>>(jsonData, options) ?? new List<Car>();

                    carCollection.GetAllCars().Clear();
                    foreach (var car in receivedCars)
                    {
                        carCollection.AddCar(car);
                    }
                    carCollection.SaveToFile("cars.json"); // Save to local file
                    statusLabel.Text = "Статус: данные получены и обновлены";
                    MessageBox.Show("Данные успешно получены с сервера и обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    statusLabel.Text = "Статус: неверный ответ от сервера";
                    MessageBox.Show("Неверный ответ от сервера", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Статус: ошибка при запросе";
                MessageBox.Show($"Ошибка при запросе данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}