using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
                if (arg == "start_from_ets2") SendKeys.SendWait("%{Tab}");
            }
            InitializeComponent();
        }


        // Чекбокс вкл/выкл
        private void Enable_CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void Enable_CheckBox_Unhecked(object sender, RoutedEventArgs e)
        {

        }


        // Список с портами
        private void COM_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void COM_ComboBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void COM_ComboBox_DropDownOpened(object sender, EventArgs e)
        {

        }
        private void COM_ComboBox_DropDownClosed(object sender, EventArgs e)
        {

        }


        private void Effects_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
