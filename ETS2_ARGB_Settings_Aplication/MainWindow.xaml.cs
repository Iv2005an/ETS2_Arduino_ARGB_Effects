using System.IO.Ports;
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
        struct telemetry_state_t
        {
            internal bool running;        // Состояние
            internal float speed;         // Скорость грузовика
            internal float speed_limit;   // Ограничение скорости
            internal float throttle;      // Газ
            internal int gear;            // передача
            internal bool parking_brake;  // Ручник
            internal bool parking_lights; // Габариты
            internal bool low_air;        // Нехватка воздуха
            internal bool low_fuel;       // Нехватка топлива
            internal bool l_blinker;      // Левый поворотник
            internal bool r_blinker;      // Правый поворотник
            internal bool hazard;         // Аварийка
        };
        telemetry_state_t telemetry = new telemetry_state_t();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Enable_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = true;
            Update_Button.IsEnabled = true;
            TestConnection_Button.IsEnabled = true;
            Effects_ComboBox.IsEnabled = true;
        }

        private void Enable_CheckBox_Unhecked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = false;
            Update_Button.IsEnabled = false;
            TestConnection_Button.IsEnabled = false;
            Effects_ComboBox.IsEnabled = false;
        }

        private void COM_ComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
            COM_ComboBox.SelectedIndex = 1;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
            COM_ComboBox.SelectedIndex = 1;
        }
        private void TestConnection_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
