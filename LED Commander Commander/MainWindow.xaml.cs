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
        var dat = DeviceSettings.LoadFromFile("FILE10.PRO");

        dat.TwoWeirdBytes[0] = 255;
        dat.TwoWeirdBytes[1] = 255;
        dat.PatchMap[8] = 255;
        dat.DimmerSet[0] = false;
        dat.DimmerSet[1] = false;
        dat.ChannelNames[0] = "Pensi";

        var sb = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            sb.Append("Scene " + (i + 1).ToString().PadLeft(2) + ": ");
            var sc = dat.Scenes[i];
            for (int j = 0; j < sc.FieldArea.Length; j++)
            {
                sb.Append(" " + Convert.ToString(sc.FieldArea[j], 2).PadLeft(8, '0'));
            }
            sb.AppendLine();
        }
        sb.AppendLine();


        for (int i = 0; i < 5; i++)
        {
            sb.Append("Chaser Scene " + (i + 1).ToString().PadLeft(2) + ": ");
            var sc = dat.ChaserSteps[i];
            for (int j = 0; j < sc.FieldArea.Length; j++)
            {
                sb.Append(" " + Convert.ToString(sc.FieldArea[j], 2).PadLeft(8, '0'));
            }
            sb.AppendLine();
        }
        File.WriteAllText("data.txt", sb.ToString());
        dat.SaveToFile("FILE1 nu.PRO");
    }
}
