using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ETS2_ARGB_Settings_Aplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Структура данных
        //struct telemetry_state_t
        //{
        //    internal bool running;        // Состояние
        //    internal float speed;         // Скорость грузовика
        //    internal float speed_limit;   // Ограничение скорости
        //    internal float throttle;      // Газ
        //    internal int gear;            // передача
        //    internal bool parking_brake;  // Ручник
        //    internal bool parking_lights; // Габариты
        //    internal bool low_air;        // Нехватка воздуха
        //    internal bool low_fuel;       // Нехватка топлива
        //    internal bool l_blinker;      // Левый поворотник
        //    internal bool r_blinker;      // Правый поворотник
        //    internal bool hazard;         // Аварийка
        //};
        //telemetry_state_t telemetry = new telemetry_state_t();

        SerialPort port = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();
            COM_Writer();
        }
        bool stop_COM_Writer = true;
        private async void COM_Writer()
        {
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if (!stop_COM_Writer)
                        {
                            port.Write("000100000");
                            Thread.Sleep(100);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Enable_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = true;
            Effects_ComboBox.IsEnabled = true;
            stop_COM_Writer = true;
            if (COM_ComboBox.SelectedItem != null && !port.IsOpen && COM_ComboBox.SelectedItem.ToString() != "COM3")
            {
                try
                {
                    port = new SerialPort(COM_ComboBox.SelectedItem.ToString()!, 9600);
                    port.Open();
                    stop_COM_Writer = false;
                }
                catch (Exception exception)
                {
                    if (exception is IOException)
                    {
                        MessageBox.Show("Не удается найти или открыть указанный порт.");
                    }
                    else if (exception is UnauthorizedAccessException || exception is InvalidOperationException)
                    {
                        MessageBox.Show("Указанный порт уже открыт.");
                    }
                }
            }
        }

        private void Enable_CheckBox_Unhecked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = false;
            Effects_ComboBox.IsEnabled = false;
            stop_COM_Writer = true;
            port.Close();
        }


        private void COM_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stop_COM_Writer = true;
            if (COM_ComboBox.SelectedItem != null && Enable_CheckBox.IsChecked == true && COM_ComboBox.SelectedItem.ToString() != "COM3")
            {
                try
                {
                    port.Close();
                    port = new SerialPort(COM_ComboBox.SelectedItem.ToString()!, 9600);
                    port.Open();
                    stop_COM_Writer = false;
                }
                catch (Exception exception)
                {
                    if (exception is IOException)
                    {
                        MessageBox.Show("Не удается найти или открыть указанный порт.");
                    }
                    else if (exception is UnauthorizedAccessException || exception is InvalidOperationException)
                    {
                        MessageBox.Show("Указанный порт уже открыт.");
                    }
                }
            }
        }
        private void COM_ComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
        }
        private void COM_ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
        }
        private void COM_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
        }


        private void Effects_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
