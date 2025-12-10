using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Presentacion.Views
{
    public partial class RecepcionistView : UserControl
    {
        // ViewModels
        public ObservableCollection<CalendarDay> CalendarDays { get; set; } = new ObservableCollection<CalendarDay>();
        public ObservableCollection<CitaViewModel> ProximasCitas { get; set; } = new ObservableCollection<CitaViewModel>();

        private DateTime _currentMonth;

        public RecepcionistView()
        {
            InitializeComponent();

            // Set DataContext for Bindings
            this.DataContext = this;

            // Initialize Calendar
            _currentMonth = DateTime.Today;
            UpdateCalendar(_currentMonth);

            // Load Data
            LoadUpcomingAppointments();

            // Draw Chart (Wait for load to get actual sizes)
            this.Loaded += (s, e) => DrawWeeklyChart();
            this.SizeChanged += (s, e) => DrawWeeklyChart();
        }

        #region UI Events
        private void BtnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (SideMenuColumn.Width.Value > 100)
                SideMenuColumn.Width = new GridLength(80);
            else
                SideMenuColumn.Width = new GridLength(220);
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            LoginView Login = new LoginView();
            Login.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendar(_currentMonth);
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendar(_currentMonth);
        }
        #endregion

        #region Calendar Logic
        private void UpdateCalendar(DateTime monthDate)
        {
            if (TxtMesAnio != null)
                TxtMesAnio.Text = monthDate.ToString("MMMM yyyy", CultureInfo.CurrentCulture).ToUpper();

            CalendarDays.Clear();
            if (CalendarItemsControl != null)
                CalendarItemsControl.ItemsSource = CalendarDays;

            // Calculate start date
            DateTime firstDayOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

            // Determine offset (DayOfWeek) - Sunday=0, Monday=1...
            int offset = (int)firstDayOfMonth.DayOfWeek;

            // Add empty padding days
            for (int i = 0; i < offset; i++)
            {
                CalendarDays.Add(new CalendarDay { IsEmpty = true });
            }

            // Fill days
            Random rnd = new Random();
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(monthDate.Year, monthDate.Month, day);
                var calendarDay = new CalendarDay
                {
                    Date = date,
                    DayNumber = day.ToString(),
                    IsEmpty = false,
                    IsToday = date.Date == DateTime.Today
                };

                // Simulator: Add random indicators
                int numCitas = rnd.Next(0, 4);
                for (int k = 0; k < numCitas; k++)
                {
                    // Random status for demo
                    int statusType = rnd.Next(0, 3); // 0=Confirmed, 1=Pending, 2=Cancelled
                    string colorHex = statusType == 0 ? "#2ECC71" : (statusType == 1 ? "#F1C40F" : "#E74C3C");

                    calendarDay.Indicators.Add(new CitaIndicator
                    {
                        ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex))
                    });
                }

                CalendarDays.Add(calendarDay);
            }
        }
        #endregion

        #region Data Logic
        private void LoadUpcomingAppointments()
        {
            // Mock Data for Demo
            if (DgProximasCitas == null) return;

            ProximasCitas.Clear();
            ProximasCitas.Add(new CitaViewModel { HoraStr = "09:00", NombrePaciente = "Ana López", Tratamiento = "Limpieza", Estatus = "Confirmada" });
            ProximasCitas.Add(new CitaViewModel { HoraStr = "10:30", NombrePaciente = "Carlos Ruiz", Tratamiento = "Extracción", Estatus = "Pendiente" });
            ProximasCitas.Add(new CitaViewModel { HoraStr = "12:00", NombrePaciente = "Maria Garcia", Tratamiento = "Ortodoncia", Estatus = "Cancelada" });
            ProximasCitas.Add(new CitaViewModel { HoraStr = "15:00", NombrePaciente = "Luis Diaz", Tratamiento = "Consulta", Estatus = "Confirmada" });
            ProximasCitas.Add(new CitaViewModel { HoraStr = "16:45", NombrePaciente = "Pedro Silva", Tratamiento = "Revisión", Estatus = "Pendiente" });

            DgProximasCitas.ItemsSource = ProximasCitas;
        }

        private void DrawWeeklyChart()
        {
            if (WeeklyChartCanvas == null) return;

            WeeklyChartCanvas.Children.Clear();

            // Generate dummy data
            double[] values = { 5, 8, 12, 7, 10, 4, 2 };
            string[] labels = { "L", "M", "M", "J", "V", "S", "D" };

            double canvasWidth = WeeklyChartCanvas.ActualWidth;
            double canvasHeight = WeeklyChartCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0) return;

            double padding = 20;
            double chartWidth = canvasWidth - (padding * 2);
            double chartHeight = canvasHeight - (padding * 2);

            double maxVal = 15;
            double barWidth = (chartWidth / values.Length) * 0.5;
            double spacing = (chartWidth / values.Length);

            // Draw Axis lines
            var axisBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BDC3C7"));

            Line xAxis = new Line
            {
                X1 = padding,
                Y1 = canvasHeight - padding,
                X2 = canvasWidth - padding,
                Y2 = canvasHeight - padding,
                Stroke = axisBrush,
                StrokeThickness = 1
            };
            WeeklyChartCanvas.Children.Add(xAxis);

            for (int i = 0; i < values.Length; i++)
            {
                double val = values[i];
                double barHeight = (val / maxVal) * chartHeight;

                double xPos = padding + (i * spacing) + (spacing - barWidth) / 2;
                double yPos = (canvasHeight - padding) - barHeight;

                // Bar
                Rectangle bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                    RadiusX = 4,
                    RadiusY = 4,
                    ToolTip = val.ToString() + " Citas"
                };

                Canvas.SetLeft(bar, xPos);
                Canvas.SetTop(bar, yPos);
                WeeklyChartCanvas.Children.Add(bar);

                // Axis Label
                TextBlock label = new TextBlock
                {
                    Text = labels[i],
                    Foreground = Brushes.Gray,
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(label, xPos);
                Canvas.SetTop(label, canvasHeight - padding + 4);
                WeeklyChartCanvas.Children.Add(label);
            }
        }
        #endregion
    }

    #region Helper Classes
    public class CalendarDay
    {
        public string DayNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsEmpty { get; set; }
        public bool IsToday { get; set; }

        public ObservableCollection<CitaIndicator> Indicators { get; set; } = new ObservableCollection<CitaIndicator>();

        public Brush BackgroundBrush => IsToday ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5F5E3")) : Brushes.Transparent;
        public Brush ForegroundBrush => IsEmpty ? Brushes.Transparent : (IsToday ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#186A3B")) : Brushes.Black);
    }

    public class CitaIndicator
    {
        public Brush ColorBrush { get; set; }
    }

    public class CitaViewModel
    {
        public string HoraStr { get; set; }
        public string NombrePaciente { get; set; }
        public string Tratamiento { get; set; }
        public string Estatus { get; set; }
    }
    #endregion
}
