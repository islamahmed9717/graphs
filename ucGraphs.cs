using Common.Classes;
using SlimDX.Direct3D9;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using CheckBox = System.Windows.Forms.CheckBox;
using GroupBox = System.Windows.Forms.GroupBox;
using Timer = System.Windows.Forms.Timer;

namespace ThrusterTest.UserControls
{
    public partial class ucGraphs : UserControl
    {

        private Random random = new Random();
        private Timer timer = new Timer();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, Series>> chartSeriesDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, Series>>();
        private double mouseX;

        public ucGraphs()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            InitializeCharts();
            //PopulateInitialData();
            //InitializeAllCharts();
            InitializeVerticalLine();
            InizializeTimer();
            CreateChartColors();
            accessDic();

        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int middleY = panel1.Height / 2;
            int lineLength = panel1.Width; // Set the length of the dashed lines to match the width of the panel
            int gap = 20; // Set the gap between the two lines

            // Calculate the Y positions for the two dashed lines
            int topLineY = middleY - gap / 2;
            int bottomLineY = middleY + gap / 2;

            // Draw dashed lines at the middle of the Panel
            using (Pen pen = new Pen(Color.Black))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Draw the top line
                e.Graphics.DrawLine(pen, new Point(0, topLineY), new Point(lineLength, topLineY));

                // Draw the bottom line
                e.Graphics.DrawLine(pen, new Point(0, bottomLineY), new Point(lineLength, bottomLineY));
            }
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            int middleY = panel2.Height / 2;
            int lineLength = panel2.Width; // Set the length of the dashed lines to match the width of the panel
            int gap = 20; // Set the gap between the two lines

            // Calculate the Y positions for the two dashed lines
            int topLineY = middleY - gap / 2;
            int bottomLineY = middleY + gap / 2;

            // Draw dashed lines at the middle of the Panel
            using (Pen pen = new Pen(Color.Black))
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                // Draw the top line
                e.Graphics.DrawLine(pen, new Point(0, topLineY), new Point(lineLength, topLineY));

                // Draw the bottom line
                e.Graphics.DrawLine(pen, new Point(0, bottomLineY), new Point(lineLength, bottomLineY));
            }
        }


        private void InitializeCharts()
        {
            chartSeriesDictionary.TryAdd("chart1", new ConcurrentDictionary<string, Series>());
            chartSeriesDictionary.TryAdd("chart2", new ConcurrentDictionary<string, Series>());
            chartSeriesDictionary.TryAdd("chart3", new ConcurrentDictionary<string, Series>());
            chartSeriesDictionary.TryAdd("chart4", new ConcurrentDictionary<string, Series>());


            AddSeries("chart1", new string[] { "T1", "T2", "T3", "T4" });
            AddSeries("chart2", new string[] { "A1", "A2", "A3", "A4" });
            AddSeries("chart3", new string[] { "Actual", "Desired" });
            AddSeries("chart4", new string[] { "Actual1", "Desired1" });


            SetAxisRanges("chart1", -100, 100); // Y1 range for chart1
            SetAxisRanges("chart2", 0, 360);    // Y2 range for chart2
            SetAutoScale("chart3");
            SetAutoScale("chart4");

        }

        private Panel Verticalline;
        private Panel Verticalline1;
        private Panel Verticalline2;
        private Panel Verticalline3;
        private void InitializeVerticalLine()
        {

            // Create a Panel for the vertical line
            Verticalline = new Panel
            {
                Width = 2, // Adjust the width of the line as needed
                Height = chart1.ClientRectangle.Height,
                BackColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(0, 0) // Initial position of the line
            };
            Verticalline1 = new Panel
            {
                Width = 2, // Adjust the width of the line as needed
                Height = chart1.ClientRectangle.Height,
                BackColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(0, 0) // Initial position of the line
            };


            Verticalline2 = new Panel
            {
                Width = 2, // Adjust the width of the line as needed
                Height = chart1.ClientRectangle.Height,
                BackColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(0, 0) // Initial position of the line
            };


            Verticalline3 = new Panel
            {
                Width = 2, // Adjust the width of the line as needed
                Height = chart1.ClientRectangle.Height,
                BackColor = System.Drawing.Color.Red,
                Location = new System.Drawing.Point(0, 0) // Initial position of the line
            };

            // Add the vertical line to the form
            chart1.Controls.Add(Verticalline);
            chart2.Controls.Add(Verticalline1);
            chart3.Controls.Add(Verticalline2);
            chart4.Controls.Add(Verticalline3);

            chart1.MouseMove += Chart_MouseMove;
            chart2.MouseMove += Chart_MouseMove;
            chart3.MouseMove += Chart_MouseMove;
            chart4.MouseMove += Chart_MouseMove;

        }

        private void Chart_MouseMove(object? sender, MouseEventArgs e)
        {

            Chart chart = (Chart)sender;

            foreach (var chartName in chartSeriesDictionary.Keys)
            {
                chart = Controls.Find(chartName, true).FirstOrDefault() as Chart;
                if (chart != null && chart.ClientRectangle.Contains(e.Location))
                {
                    // Calculate the X position relative to the chart
                    int mouseX = e.Location.X - chart.Location.X;

                    // Calculate the maximum and minimum X positions within the chart
                    int maxX = chart.ClientSize.Width - Verticalline.Width;
                    int minX = 0;

                    // Ensure the mouseX stays within the bounds of the chart
                    mouseX = Math.Max(minX, Math.Min(maxX, mouseX));

                    // Update the position of the vertical line within the chart
                    Verticalline.Location = new Point(mouseX - (Verticalline.Width / 2), 0);
                    Verticalline.Visible = true; // Show the vertical line
                    Verticalline.BringToFront();
                    Verticalline1.Location = new Point(mouseX - (Verticalline.Width / 2), 0);
                    Verticalline1.Visible = true; // Show the vertical line
                    Verticalline1.BringToFront();
                    Verticalline2.Location = new Point(mouseX - (Verticalline.Width / 2), 0);
                    Verticalline2.Visible = true; // Show the vertical line
                    Verticalline2.BringToFront();
                    Verticalline3.Location = new Point(mouseX - (Verticalline.Width / 2), 0);
                    Verticalline3.Visible = true; // Show the vertical line
                    Verticalline3.BringToFront();

                }
                else
                {
                    // Hide the vertical line if the mouse is not over any chart
                    Verticalline.Visible = false;
                    Verticalline1.Visible = false;
                    Verticalline2.Visible = false;
                    Verticalline3.Visible = false;
                }
            }
        }

        private void AddSeries(string chartName, string[] seriesNames)
        {
            var chart = Controls.Find(chartName, true).FirstOrDefault() as Chart;

            if (chart != null)
            {
                foreach (var seriesName in seriesNames)
                {
                    Series series = new Series(seriesName); // Set series name here
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = 4;
                    series.XValueType = ChartValueType.Int32;
                    series.YValueType = ChartValueType.Int32;
                    // Add circular markers at each data point
                    series.MarkerStyle = MarkerStyle.Circle;
                    series.MarkerSize = 2;
                    series.MarkerColor = Color.Black;


                    chart.Series.Add(series);
                    chartSeriesDictionary[chartName].TryAdd(seriesName, series);
                }
            }
            chart.Legends.Clear();
        }


        private void SetAxisRanges(string chartName, double minY, double maxY)
        {
            var chart = Controls.Find(chartName, true).FirstOrDefault() as Chart;

            if (chart != null)
            {
                // Check which chart is being configured and set the Y-axis limits accordingly
                if (chartName == "chart1")
                {
                    // Y1 axis
                    chart.ChartAreas[0].AxisY.Minimum = minY;
                    chart.ChartAreas[0].AxisY.Maximum = maxY;
                }
                else if (chartName == "chart2")
                {
                    // Y2 axis
                    chart.ChartAreas[0].AxisY.Minimum = minY;
                    chart.ChartAreas[0].AxisY.Maximum = maxY;
                }
            }
            else
            {
                Console.WriteLine($"Chart '{chartName}' not found.");
            }
        }


        private void SetAutoScale(string chartName)
        {
            var chart = Controls.Find(chartName, true).FirstOrDefault() as Chart;

            if (chart != null)
            {
                // Enable autoscaling for X axis
                foreach (var axis in chart.ChartAreas[0].Axes)
                {
                    axis.Minimum = double.NaN;
                    axis.Maximum = double.NaN;
                }

                // Check if the chart is either chart1 or chart2, then enable autoscaling for Y axis
                if (chartName == "chart1" || chartName == "chart2")
                {
                    chart.ChartAreas[0].AxisY.Minimum = double.NaN;
                    chart.ChartAreas[0].AxisY.Maximum = double.NaN;
                }
            }
            else
            {
                Console.WriteLine($"Chart '{chartName}' not found.");
            }
        }



        private void InizializeTimer()
        {
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }


        private void Timer_Tick(object? sender, EventArgs e)
        {
            double ranvalue = 10;

            foreach (var chartName in chartSeriesDictionary.Keys)
            {
                var seriesDictionary = chartSeriesDictionary[chartName];
                foreach (var seriesName in seriesDictionary.Keys)
                {
                    double newValue;
                    if (chartName == "chart3" || chartName == "chart4")
                    {
                        // For chart3 and chart4, generate random numbers within a reasonable range for Decimal type
                        newValue = random.NextDouble() * ranvalue;
                    }
                    else if (chartName == "chart1")
                    {
                        newValue = random.Next(-100, 100);
                    }
                    else
                    {
                        // For other charts, generate random numbers within a range
                        newValue = random.Next(0, 360);
                    }

                    double xInterval = 10;
                    UpdateChart(chartName, seriesName, newValue, xInterval);
                }
                ranvalue++;
            }

        }


        private void UpdateChart(string chartName, string seriesName, double newValue, double xInterval)
        {
            var series = chartSeriesDictionary[chartName][seriesName];

            double maxX = series.Points.Count > 0 ? series.Points.Max(p => p.XValue) : 0;

            double newX = maxX + xInterval;

            series.Points.AddXY(newX + 1, newValue);
        }

        private void CreateChartColors()
        {
            Dictionary<string, Dictionary<string, Color[]>> chartColors = new Dictionary<string, Dictionary<string, Color[]>>()
    {
        {"chart1", new Dictionary<string, Color[]>
            {
                {"T1", new Color[] { Color.Green }},
                {"T2", new Color[] { Color.Red }},
                {"T3", new Color[] { Color.Blue }},
                {"T4", new Color[] { Color.LightGreen }}
            }
        },
        {"chart2", new Dictionary<string, Color[]>
            {
                {"A1", new Color[] { Color.Blue }},
                {"A2", new Color[] { Color.Red }},
                {"A3", new Color[] { Color.Green }},
                {"A4", new Color[] { Color.LightGreen }}
            }
        },
        {"chart3", new Dictionary<string, Color[]>
            {
                {"Actual", new Color[] { Color.Blue }},
                {"Desired", new Color[] { Color.Red }}
            }
        },
        {"chart4", new Dictionary<string, Color[]>
            {
                {"Actual1", new Color[] { Color.Blue }},
                {"Desired1", new Color[] { Color.Red }}
            }
        }
    };

            foreach (var chartName in chartSeriesDictionary.Keys)
            {
                Console.WriteLine($"Processing chart: {chartName}");

                var chart = Controls.Find(chartName, true).FirstOrDefault() as Chart;
                if (chart != null && chartColors.ContainsKey(chartName))
                {
                    var seriesDictionary = chartSeriesDictionary[chartName];
                    var colors = chartColors[chartName];
                    foreach (var seriesName in seriesDictionary.Keys)
                    {
                        Console.WriteLine($"  Processing series: {seriesName}");
                        if (colors.ContainsKey(seriesName))
                        {
                            var colorArray = colors[seriesName];
                            var series = seriesDictionary[seriesName];

                            // Ensure colorArray has sufficient length
                            if (series.Points.Count <= colorArray.Length)
                            {
                                // Assign series color
                                series.Color = colorArray[0];
                                Console.WriteLine($"      Assigned color: {colorArray[0]}");
                            }
                            else
                            {
                                Console.WriteLine($"  Warning: Insufficient colors for series '{seriesName}' in chart '{chartName}'.");
                                // Assign default color to the series
                                series.Color = Color.Black;
                                Console.WriteLine($"      Assigned default color: Black");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"  Warning: No colors defined for series '{seriesName}' in chart '{chartName}'.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Chart '{chartName}' not found or no colors defined.");
                }
            }
        }



        private void chkT4_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart1.Series.FirstOrDefault(s => s.Name == "T4");
            if (series != null)
            {

                series.Enabled = chkT4.Checked;
            }
            else
            {

                Console.WriteLine("Series 'T4' not found in the SeriesCollection of chart1.");
            }

        }

        private void chkT2_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart1.Series.FirstOrDefault(s => s.Name == "T2");
            if (series != null)
            {

                series.Enabled = chkT2.Checked;
            }
            else
            {

                Console.WriteLine("Series 'T2' not found in the SeriesCollection of chart1.");
            }
        }

        private void chkT3_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart1.Series.FirstOrDefault(s => s.Name == "T3");
            if (series != null)
            {
                series.Enabled = chkT3.Checked;
            }
            else
            {

                Console.WriteLine("Series 'T3' not found in the SeriesCollection of chart1.");
            }
        }

        private void chkT1_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart1.Series.FirstOrDefault(s => s.Name == "T1");
            if (series != null)
            {

                series.Enabled = chkT1.Checked;
            }

            else
            {

                Console.WriteLine("Series 'T1' not found in the SeriesCollection of chart1.");
            }

        }

        private void chkA4_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart2.Series.FirstOrDefault(s => s.Name == "A4");
            if (series != null)
            {
                // Enable or disable the series based on the checked state of chkT4
                series.Enabled = chkA4.Checked;
            }

            else
            {
                // The series with the name "T4" does not exist in the SeriesCollection
                Console.WriteLine("Series 'A4' not found in the SeriesCollection of chart2.");
            }
        }

        private void chkA2_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart2.Series.FirstOrDefault(s => s.Name == "A2");
            if (series != null)
            {
                series.Enabled = chkA2.Checked;
            }

            else
            {
                Console.WriteLine("Series 'A2' not found in the SeriesCollection of chart2.");
            }
        }

        private void chkA3_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart2.Series.FirstOrDefault(s => s.Name == "A3");
            if (series != null)
            {
                series.Enabled = chkA3.Checked;
            }

            else
            {
                Console.WriteLine("Series 'A3' not found in the SeriesCollection of chart2.");
            }
        }

        private void chkA1_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart2.Series.FirstOrDefault(s => s.Name == "A1");
            if (series != null)
            {
                series.Enabled = chkA1.Checked;
            }

            else
            {
                Console.WriteLine("Series 'A1' not found in the SeriesCollection of chart2.");
            }
        }


        private void chkActual_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart3.Series.FirstOrDefault(s => s.Name == "Actual");
            if (series != null)
            {
                series.Enabled = chkActual.Checked;
            }

            else
            {
                Console.WriteLine("Series 'Actual' not found in the SeriesCollection of chart3.");
            }
        }

        private void chkDesired_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart3.Series.FirstOrDefault(s => s.Name == "Desired");
            if (series != null)
            {
                series.Enabled = chkDesired.Checked;
            }
            else { Console.WriteLine("Series 'Desired' not found in the SeriesCollection of chart3."); }

        }

        private void chkActual1_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart4.Series.FirstOrDefault(s => s.Name == "Actual1");
            if (series != null)
            {
                series.Enabled = chkActual1.Checked;
            }

            else
            {
                Console.WriteLine("Series 'Actual1' not found in the SeriesCollection of chart4.");
            }
        }

        private void chkDesired1_CheckedChanged(object sender, EventArgs e)
        {
            var series = chart4.Series.FirstOrDefault(s => s.Name == "Desired1");
            if (series != null)
            {
                series.Enabled = chkDesired1.Checked;
            }

            else
            {
                Console.WriteLine("Series 'Desired1' not found in the SeriesCollection of chart4.");
            }
        }


        private void accessDic()
        {
            string cal = "chart1";

            if (chartSeriesDictionary.ContainsKey(cal))
            {
                var innerDic = chartSeriesDictionary[cal];

                foreach (var seriesname in innerDic.Keys)
                {

                    Console.WriteLine($"the series name is = {seriesname}");

                    var series = innerDic[seriesname];
                }
            }

            else
            {
                Console.WriteLine($"chart '{cal}' not found in chart series dictionary");
            }
        }

    }
}


//private bool IsCheckboxChecked(string seriesName)
//{
//    System.Windows.Forms.CheckBox checkBox = FindCheckBox(seriesName, Controls);

//    if (checkBox != null && chartSeriesDictionary.ContainsKey(checkBox.Parent.Name) && chartSeriesDictionary[checkBox.Parent.Name] != null && chartSeriesDictionary[checkBox.Parent.Name].ContainsKey(seriesName))
//    {
//        var series = chartSeriesDictionary[checkBox.Parent.Name][seriesName];

//        if (checkBox.Checked)
//        {
//            series.Enabled = true;
//        }
//        else
//        {
//            series.Enabled = false;
//        }

//        return checkBox.Checked && series.Enabled;
//    }
//    else
//    {
//        // Handle the case where the object is not set
//        return false;
//    }
//}

//private System.Windows.Forms.CheckBox FindCheckBox(string seriesName, ControlCollection controls)
//{
//    foreach (Control control in controls)
//    {
//        if (control is GroupBox groupBox)
//        {
//            CheckBox checkBox = FindCheckBox(seriesName, groupBox.Controls);
//            if (checkBox != null)
//                return checkBox;
//        }
//        else if (control is CheckBox checkBox && checkBox.Name == seriesName)
//        {
//            return checkBox;
//        }
//    }
//    return null;
//}