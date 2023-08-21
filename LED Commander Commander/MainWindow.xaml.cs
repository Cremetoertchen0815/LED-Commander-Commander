using LED_Commander_Commander.ManagedData;
using LED_Commander_Commander.RawData;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace LED_Commander_Commander;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void button_Click(object sender, RoutedEventArgs e)
    {
        var data = new SaveFile();
        data.LoadFromFile("FILE9.PRO");
    }
}
