using System.Net.Sockets;
using System.Text;

namespace lab_1;

public class ConnectionForm : Form
{
    private TextBox ipTextBox;
    private TextBox portTextBox;
    private Button connectButton;
    private TcpClient client;


    public ConnectionForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Connection to server";
        this.Size = new Size(300, 200);

        Label ipLabel = new Label()
        {
            Text = "Ip addres:",
            Location = new Point(20, 20),
            Size = new Size(80, 20)
        };

        ipTextBox = new TextBox()
        {
            Location = new Point(120, 20),
            Size = new Size(150, 20)
        };

        Label portLabel = new Label()
        {
            Text = "Port:",
            Location = new Point(20, 60),
            Size = new Size(80, 20)
        };

        portTextBox = new TextBox()
        {
            Location = new Point(120, 60),
            Size = new Size(150, 20)
        };

        connectButton = new Button()
        {
            Text = "Connect",
            Location = new Point(90, 100),
            Size = new Size(120, 30)
        };

        connectButton.Click += new EventHandler(ConnectButton_Click);

        this.Controls.Add(ipLabel);
        this.Controls.Add(ipTextBox);
        this.Controls.Add(portLabel);
        this.Controls.Add(portTextBox);
        this.Controls.Add(connectButton);
    }

    private void ConnectButton_Click(object serder, EventArgs e)
    {
        string ipAddress = ipTextBox.Text;
        int port;

        if (int.TryParse(portTextBox.Text, out port))
        {
            try
            {
                client = new TcpClient(ipAddress, port);
                MessageBox.Show("Connected to server done!!!", "Success", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ListenForMessage();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Fatal connection ERROR: {exception.Message}", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show("Invalid port number", "Fatal ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ListenForMessage()
    {
        Thread listenerThread = new Thread(() =>
        {
            while (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                int byteRead = stream.Read(buffer, 0, buffer.Length);

                if (byteRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, byteRead);
                    Invoke(new Action(() => MessageBox.Show(message, "Hello random User =<<<<<")));
                }
            }
        });
        listenerThread.Start();
    }
}