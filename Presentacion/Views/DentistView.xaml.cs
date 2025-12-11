using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using Presentacion.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Presentacion.Views
{
    public partial class DentistView : UserControl, INotifyPropertyChanged
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;
        private int _currentDentistaId;
        private DateTime _currentMonth;

        public ICommand OpenDayDetailsCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public DentistView()
        {
            InitializeComponent();
            DataContext = this;

            _citaService = new CitaService();
            _pacienteService = new PacienteService();
            _currentMonth = DateTime.Now;

            // Get logged-in dentist ID
            if (UserSession.CurrentUser != null)
            {
                _currentDentistaId = UserSession.CurrentUser.Id;
                TxtDentistName.Text = $"Dr. {UserSession.CurrentUser.Username}";
            }

            OpenDayDetailsCommand = new RelayCommand(ExecuteOpenDayDetails);

            // Initialize dashboard
            ShowAgenda();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region UI Events
        private void BtnHamburger_Click(object sender, RoutedEventArgs e)
        {
            if (SideMenuColumn.Width.Value > 100)
            {
                SideMenuColumn.Width = new GridLength(80);
            }
            else
            {
                SideMenuColumn.Width = new GridLength(220);
            }
        }

        private void BtnCerrarSesionClick(object sender, RoutedEventArgs e)
        {
            UserSession.Clear();
            LoginView Login = new LoginView();
            Login.Show();
            Window.GetWindow(this)?.Close();
        }
        #endregion

        #region Navigation Logic
        private void BtnAgenda_Click(object sender, RoutedEventArgs e)
        {
            ShowAgenda();
        }

        private void BtnPacientes_Click(object sender, RoutedEventArgs e)
        {
            ShowPacientes();
        }

        private void ShowAgenda()
        {
            HeaderTitle.Text = "Mi Agenda Hoy";
            AgendaContainer.Visibility = Visibility.Visible;
            PacientesContainer.Visibility = Visibility.Collapsed;

            LoadDashboardData();
        }

        private void ShowPacientes()
        {
            HeaderTitle.Text = "Mis Pacientes";
            AgendaContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Visible;

            LoadPacientesList();
        }
        #endregion

        #region Dashboard Logic
        private void LoadDashboardData()
        {
            UpdateCalendar();
            LoadUpcomingAppointments();
            DrawWeeklyChart();
        }

        private void UpdateCalendar()
        {
            TxtMesAnio.Text = _currentMonth.ToString("MMMM yyyy", new CultureInfo("es-ES"));

            var firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            int startDayOfWeek = (int)firstDay.DayOfWeek;

            // Get appointments for this month filtered by dentist
            var monthAppointments = _citaService.GetAllCitas()
                .Where(c => c.DentistaId == _currentDentistaId &&
                           c.FechaHora.Year == _currentMonth.Year &&
                           c.FechaHora.Month == _currentMonth.Month)
                .ToList();

            var calendarDays = new ObservableCollection<CalendarDay>();

            // Empty cells before first day
            for (int i = 0; i < startDayOfWeek; i++)
            {
                calendarDays.Add(new CalendarDay
                {
                    DayNumber = "",
                    IsCurrentMonth = false,
                    BackgroundBrush = Brushes.Transparent,
                    ForegroundBrush = Brushes.Transparent
                });
            }

            // Days of the month
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(_currentMonth.Year, _currentMonth.Month, day);
                var dayAppointments = monthAppointments.Where(c => c.FechaHora.Date == currentDate.Date).ToList();

                var indicators = new ObservableCollection<CitaIndicator>();
                foreach (var cita in dayAppointments.Take(3))
                {
                    var color = cita.Estatus?.ToLower() switch
                    {
                        "confirmada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2ECC71")),
                        "pendiente" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F1C40F")),
                        "cancelada" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E74C3C")),
                        _ => Brushes.Gray
                    };
                    indicators.Add(new CitaIndicator { ColorBrush = color });
                }

                bool isToday = currentDate.Date == DateTime.Today;

                calendarDays.Add(new CalendarDay
                {
                    DayNumber = day.ToString(),
                    Date = currentDate,
                    IsCurrentMonth = true,
                    BackgroundBrush = isToday ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")) : Brushes.Transparent,
                    ForegroundBrush = isToday ? Brushes.White : Brushes.Black,
                    Indicators = indicators
                });
            }

            CalendarItemsControl.ItemsSource = calendarDays;
        }

        private void LoadUpcomingAppointments()
        {
            try
            {
                var upcomingCitas = _citaService.GetAllCitas()
                    .Where(c => c.DentistaId == _currentDentistaId &&
                               c.FechaHora >= DateTime.Now)
                    .OrderBy(c => c.FechaHora)
                    .Take(10)
                    .Select(c => new CitaViewModel
                    {
                        HoraStr = c.FechaHora.ToString("HH:mm"),
                        NombrePaciente = c.PacienteNombre ?? "N/A",
                        Tratamiento = "Consulta",
                        Estatus = c.Estatus ?? "Pendiente"
                    })
                    .ToList();

                DgProximasCitas.ItemsSource = upcomingCitas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar próximas citas: {ex.Message}");
            }
        }

        private void DrawWeeklyChart()
        {
            try
            {
                WeeklyChartCanvas.Children.Clear();

                var today = DateTime.Today;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                var weekDays = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };

                var weeklyCounts = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    var day = startOfWeek.AddDays(i);
                    weeklyCounts[i] = _citaService.GetAllCitas()
                        .Count(c => c.DentistaId == _currentDentistaId &&
                                   c.FechaHora.Date == day.Date);
                }

                int maxCount = weeklyCounts.Max();
                if (maxCount == 0) maxCount = 1;

                double canvasWidth = WeeklyChartCanvas.ActualWidth > 0 ? WeeklyChartCanvas.ActualWidth : 400;
                double canvasHeight = WeeklyChartCanvas.ActualHeight > 0 ? WeeklyChartCanvas.ActualHeight : 200;
                double barWidth = canvasWidth / 7 * 0.6;
                double spacing = canvasWidth / 7;

                for (int i = 0; i < 7; i++)
                {
                    double barHeight = (weeklyCounts[i] / (double)maxCount) * (canvasHeight - 40);

                    var bar = new Rectangle
                    {
                        Width = barWidth,
                        Height = barHeight,
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3498DB")),
                        RadiusX = 5,
                        RadiusY = 5
                    };

                    Canvas.SetLeft(bar, i * spacing + (spacing - barWidth) / 2);
                    Canvas.SetTop(bar, canvasHeight - barHeight - 25);
                    WeeklyChartCanvas.Children.Add(bar);

                    var countText = new TextBlock
                    {
                        Text = weeklyCounts[i].ToString(),
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Black
                    };
                    Canvas.SetLeft(countText, i * spacing + spacing / 2 - 10);
                    Canvas.SetTop(countText, canvasHeight - barHeight - 45);
                    WeeklyChartCanvas.Children.Add(countText);

                    var dayText = new TextBlock
                    {
                        Text = weekDays[i],
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"))
                    };
                    Canvas.SetLeft(dayText, i * spacing + spacing / 2 - 15);
                    Canvas.SetTop(dayText, canvasHeight - 20);
                    WeeklyChartCanvas.Children.Add(dayText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al dibujar gráfica: {ex.Message}");
            }
        }

        private void BtnPrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendar();
        }

        private void BtnNextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendar();
        }

        private void ExecuteOpenDayDetails(object parameter)
        {
            if (parameter is CalendarDay day && day.IsCurrentMonth && day.Date.HasValue)
            {
                var dayCitas = _citaService.GetAllCitas()
                    .Where(c => c.DentistaId == _currentDentistaId &&
                               c.FechaHora.Date == day.Date.Value.Date)
                    .OrderBy(c => c.FechaHora)
                    .ToList();

                var citaViewModels = dayCitas.Select(c => new Views.CitaViewModel
                {
                    HoraStr = c.FechaHora.ToString("HH:mm"),
                    NombrePaciente = c.PacienteNombre ?? "N/A",
                    Tratamiento = "Consulta",
                    Estatus = c.Estatus ?? "Pendiente",
                    DentistaNombre = c.DentistaNombre ?? "No asignado"
                }).ToList();

                var dayView = new DentistDayAppointmentsView(day.Date.Value, citaViewModels);
                var window = new Window
                {
                    Content = dayView,
                    Title = $"Mis Citas - {day.Date.Value:dd/MM/yyyy}",
                    Width = 600,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };
                window.ShowDialog();
            }
        }
        #endregion

        #region Pacientes Logic
        private List<PacienteDTO> _allPacientesList;

        private void LoadPacientesList()
        {
            try
            {
                // Get distinct patient IDs from appointments for this dentist
                var patientIds = _citaService.GetAllCitas()
                    .Where(c => c.DentistaId == _currentDentistaId)
                    .Select(c => c.PacienteId)
                    .Distinct()
                    .ToList();

                // Get patient details
                _allPacientesList = _pacienteService.GetAllPacientes()
                    .Where(p => patientIds.Contains(p.Id))
                    .ToList();

                FilterPacientesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}");
            }
        }

        private void TxtBuscarCitas_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPacientesList();
        }

        private void FilterPacientesList()
        {
            if (_allPacientesList == null) return;

            // Verificar que el TextBox existe antes de acceder a su propiedad Text
            if (TxtBuscarCitas == null)
            {
                PacientesDataGrid.ItemsSource = _allPacientesList;
                return;
            }

            var filter = TxtBuscarCitas.Text?.ToLower() ?? string.Empty;
            var filtered = _allPacientesList.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(filter)) ||
                (p.Email != null && p.Email.ToLower().Contains(filter)) ||
                (p.Telefono != null && p.Telefono.ToLower().Contains(filter))
            ).ToList();
            PacientesDataGrid.ItemsSource = filtered;
        }

        private void BtnEditarPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is PacienteDTO paciente)
            {
                var form = new PacienteFormView(paciente);
                var window = new Window
                {
                    Title = "Editar Paciente",
                    Content = form,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                form.OnSave += (updatedPaciente) =>
                {
                    try
                    {
                        _pacienteService.UpdatePaciente(updatedPaciente);
                        LoadPacientesList();
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar paciente: {ex.Message}");
                    }
                };

                form.OnCancel += () => window.Close();
                window.ShowDialog();
            }
        }

        private void BtnEliminarPaciente_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is PacienteDTO paciente)
            {
                var result = MessageBox.Show(
                    $"¿Está seguro de eliminar al paciente '{paciente.Nombre}'?",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _pacienteService.DeletePaciente(paciente.Id);
                        LoadPacientesList();
                        MessageBox.Show("Paciente eliminado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar paciente: {ex.Message}");
                    }
                }
            }
        }
        #endregion

        #region Helper Classes
        public class CalendarDay
        {
            public string DayNumber { get; set; }
            public DateTime? Date { get; set; }
            public bool IsCurrentMonth { get; set; }
            public Brush BackgroundBrush { get; set; }
            public Brush ForegroundBrush { get; set; }
            public ObservableCollection<CitaIndicator> Indicators { get; set; } = new ObservableCollection<CitaIndicator>();
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
            public string DentistaNombre { get; set; }
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Func<object, bool>? _canExecute;

            public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);
            public void Execute(object? parameter) => _execute(parameter);
        }
        #endregion
    }
}
