
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;

using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool drawing = false;
        Point? lastPoint = null;
        UdpClient client;
        IPEndPoint ep;
        byte id;
        bool connected = false;
        public MainWindow()
        {
            InitializeComponent();


            ColorDialog colorDialog = new ColorDialog();
            
            /*if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Brush barush = new SolidColorBrush(Color.FromRgb(
                colorDialog.Color.R,
                colorDialog.Color.G,
                colorDialog.Color.B));
            }
            */
        }


        public void sendPoint(Point point)
        {
            byte[] X= BitConverter.GetBytes(point.X);            
            byte[] Y= BitConverter.GetBytes(point.Y);
            var pointByte = X.Concat(Y).ToArray();
            client.Send(pointByte, 16);
        }

        public void canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (drawing && connected)
            {
                Point point = e.GetPosition(this);
                if (lastPoint != null)
                {
                    Brush brush = Brushes.Red;
                    Line line = new Line()
                    {
                        X1 = ((Point)(lastPoint)).X,
                        Y1 = ((Point)(lastPoint)).Y,
                        X2 = point.X,
                        Y2 = point.Y,
                        Stroke = brush,
                        StrokeThickness = 10
                    };

                    canvas.Children.Add(line);
                }
                lastPoint = point;
                sendPoint(point);
            }
            
        }

        public void StartStopDrawing(object sender, RoutedEventArgs e)
        {
            drawing = !drawing;
            lastPoint = null;
        }

        public void Connect(object sender, RoutedEventArgs e)
        {
            connected = true;
            client = new UdpClient();
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000); // endpoint where server is listening
            client.Connect(ep);
            client.Send(new byte[] {1},1);
            var newPortByte= client.Receive(ref ep); //get new port
            int port = BitConverter.ToInt32(newPortByte, 0);
            Port.Text = port.ToString();
            id = client.Receive(ref ep)[0]; //get id
            ID.Text = id.ToString();

            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            
            client.Close();
            client = new UdpClient();
            client.Connect(ep);
        }

        public void Disconnect(object sender, RoutedEventArgs e)
        {
            connected = false;
            client.Close();           
        }
    }
}
