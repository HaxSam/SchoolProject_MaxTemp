using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace MaxTemp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<double> werte { get; set; } = new List<double>();
        public List<(List<double> werte, string name)> WerteList { get; set; } = new List<(List<double>, string)>();
        public double MaximalTemp { get; set; } = double.NegativeInfinity;

        public SeriesCollection series = new SeriesCollection();

        public string[] Labels { get; set; }

        public Func<double, string> YFormatter { get; set; }
        public MainWindow()
        {

            InitializeComponent();

            StreamReader reader = new StreamReader(File.OpenRead("./temps.csv"));
            
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                werte.Add(Convert.ToDouble(line[line.Length - 1].Replace('.', ',')));
                (List<double> werte, string name) save = WerteList.Find(x => x.name == line[0]);
                if (save.name == null)
                    WerteList.Add((new List<double>() { Convert.ToDouble(line[line.Length - 1].Replace('.', ',')) }, line[0]));
                save.werte?.Add(Convert.ToDouble(line[line.Length - 1].Replace('.', ',')));

            }

            reader.Close();

            YFormatter = value => Convert.ToString(value);
            Labels = new[] { "1", "2", "3", "4", "5" };

            foreach ((List<double> werte, string name) in WerteList)
            {
                series.Add(new LineSeries
                {
                    Title = name,
                    Values = new ChartValues<double>(werte),
                    PointGeometry = DefaultGeometries.Circle
                });
            }

        }

        private int GetSekunde(string time)
        {
            return DateTime.Parse(time).Second + DateTime.Parse(time).Minute * 60 + DateTime.Parse(time).Hour * 360 + DateTime.Parse(time).Day * 24 * 360;
        }

        private void BtnAuswerten_Click(object sender, RoutedEventArgs e)
        {
            foreach(double val in werte)
            {
                if (val > MaximalTemp)
                    MaximalTemp = val;
            }
            lblAusgabe.Content = "";
            foreach((List<double> werte, string name) in WerteList)
            {
                MaximalTemp = Double.NegativeInfinity;
                foreach(double val in werte)
                {
                    if (val > MaximalTemp)
                        MaximalTemp = val;
                }
                lblAusgabe.Content += $"{name} hat die maximal Temperatur von {MaximalTemp}° \n";
            }

            
        }
    }

    public class DataWindow : Window
    {
        public DataWindow()
        {
           
        }
    }
}
