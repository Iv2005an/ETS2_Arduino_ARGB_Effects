using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO.Ports;

namespace ETS2_ARGB_Settings_Aplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg == "start_from_ets2") { SendKeys.SendWait("%{Tab}"); break; }
            }
            InitializeComponent();
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
            COM_ComboBox.SelectedIndex = -1;
        }


        // Чекбокс вкл/выкл
        private void Enable_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = true;
            COM_ComboBox.SelectedIndex = 0;
        }
        private void Enable_CheckBox_Unhecked(object sender, RoutedEventArgs e)
        {
            COM_ComboBox.IsEnabled = false;
            COM_ComboBox.SelectedIndex = -1;
        }


        // Список с портами
        private void COM_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            COM_ComboBox.ItemsSource = SerialPort.GetPortNames();
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


        // Список с эффектами
        private void Effects_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
