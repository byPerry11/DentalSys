using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ApplicationLogic.Services;
using ApplicationLogic.DTOs;

namespace Presentacion.Views
{
    public partial class RecepcionistView : UserControl
    {
        // ViewModels
        public ObservableCollection<CalendarDay> CalendarDays { get; set; } = new ObservableCollection<CalendarDay>();
        public ObservableCollection<CitaViewModel> ProximasCitas { get; set; } = new ObservableCollection<CitaViewModel>();

        private DateTime _currentMonth;
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;

        public RecepcionistView()
        {
            InitializeComponent();
            InitializeCommands();

            // Initialize Services
            _citaService = new CitaService();
            _pacienteService = new PacienteService();

            // Set DataContext for Bindings
            this.DataContext = this;

            // Initialize Calendar
            _currentMonth = DateTime.Today;
            // Load Data will be called in UpdateCalendar and LoadUpcomingAppointments

            LoadUpcomingAppointments();
            UpdateCalendar(_currentMonth);

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

        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            var form = new CitaFormView();
            var window = new Window
            {
                Title = "Nueva Cita",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (cita) =>
            {
                try
                {
                    _citaService.AddCita(cita);
                    LoadUpcomingAppointments();
                    UpdateCalendar(_currentMonth);
                    window.Close();
                    MessageBox.Show("Cita agendada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnRegistrarPaciente_Click(object sender, RoutedEventArgs e)
        {
            var form = new PacienteFormView();
            var window = new Window
            {
                Title = "Nuevo Paciente",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (paciente) =>
            {
                try
                {
                    _pacienteService.AddPaciente(paciente);
                    window.Close();
                    MessageBox.Show("Paciente registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
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

            // Fetch dates with appointments for this month
            var allCitas = _citaService.GetAllCitas(); // Optimized to filter in memory for now
            var citasInMonth = allCitas.Where(c => c.FechaHora.Year == monthDate.Year && c.FechaHora.Month == monthDate.Month).ToList();

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

                // Add indicators for appointments on this day
                var citasList = citasInMonth.Where(c => c.FechaHora.Date == date.Date).ToList();
                foreach (var cita in citasList)
                {
                    string colorHex;
                    switch (cita.Estatus?.ToLower())
                    {
                        case "confirmada":
                        case "completada":
                            colorHex = "#2ECC71"; // Green
                            break;
                        case "pendiente":
                            colorHex = "#F1C40F"; // Yellow
                            break;
                        case "cancelada":
                            colorHex = "#E74C3C"; // Red
                            break;
                        default:
                            colorHex = "#3498DB"; // Blue default
                            break;
                    }

                    calendarDay.Indicators.Add(new CitaIndicator
                    {
                        ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex))
                    });
                }

                CalendarDays.Add(calendarDay);
            }
        }
        #endregion

        public RelayCommand<CalendarDay> OpenDayDetailsCommand { get; private set; }

        private void InitializeCommands()
        {
            OpenDayDetailsCommand = new RelayCommand<CalendarDay>(OpenDayDetails);
        }

        private void OpenDayDetails(CalendarDay day)
        {
            if (day == null || day.IsEmpty) return;

            var allCitas = _citaService.GetAllCitas();
            var citasForDay = allCitas.Where(c => c.FechaHora.Date == day.Date.Date).ToList();

            var viewModels = new List<CitaViewModel>();
            foreach (var cita in citasForDay)
            {
                viewModels.Add(new CitaViewModel
                {
                    Id = cita.Id,
                    HoraStr = cita.FechaHora.ToString("HH:mm"),
                    NombrePaciente = cita.PacienteNombre,
                    Tratamiento = "Consulta General",
                    Estatus = cita.Estatus,
                    DentistaNombre = cita.DentistaNombre
                });
            }

            var view = new DayAppointmentsView(day.Date, viewModels);
            var window = new Window
            {
                Title = $"Citas - {day.Date:dd/MM/yyyy}",
                Content = view,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        #region Data Logic
        private void LoadUpcomingAppointments()
        {
            if (DgProximasCitas == null) return;

            try
            {
                var upcomingCitas = _citaService.GetUpcomingCitas();
                ProximasCitas.Clear();

                foreach (var cita in upcomingCitas)
                {
                    ProximasCitas.Add(new CitaViewModel
                    {
                        Id = cita.Id,
                        HoraStr = cita.FechaHora.ToString("HH:mm"),
                        NombrePaciente = cita.PacienteNombre,
                        Tratamiento = "Consulta General", // Default as per plan, since specific Treatment field is missing in CitaDTO
                        Estatus = cita.Estatus,
                        DentistaNombre = cita.DentistaNombre
                    });
                }

                DgProximasCitas.ItemsSource = ProximasCitas;
            }
            catch (Exception ex)
            {
                // Fail silently or log, but don't crash UI loop unless critical
                System.Diagnostics.Debug.WriteLine($"Error loading appointments: {ex.Message}");
            }
        }

        private void DrawWeeklyChart()
        {
            if (WeeklyChartCanvas == null) return;

            WeeklyChartCanvas.Children.Clear();

            // Calculate actual stats for the current week
            int delta = DayOfWeek.Monday - DateTime.Today.DayOfWeek;
            if (delta > 0) delta -= 7;
            var startOfWeek = DateTime.Today.AddDays(delta); // Monday
            var endOfWeek = startOfWeek.AddDays(6); // Sunday

            var allCitas = _citaService.GetAllCitas();
            double[] values = new double[7];
            string[] labels = { "L", "M", "M", "J", "V", "S", "D" };

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDay = startOfWeek.AddDays(i);
                values[i] = allCitas.Count(c => c.FechaHora.Date == currentDay.Date);
            }

            double canvasWidth = WeeklyChartCanvas.ActualWidth;
            double canvasHeight = WeeklyChartCanvas.ActualHeight;

            if (canvasWidth <= 0 || canvasHeight <= 0) return;

            double padding = 20;
            double chartWidth = canvasWidth - (padding * 2);
            double chartHeight = canvasHeight - (padding * 2);

            double maxVal = values.Max();
            if (maxVal == 0) maxVal = 10; // Default scale if no data

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
        #region Navigation Logic
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ShowDashboard();
        }

        private void BtnGestionCitas_Click(object sender, RoutedEventArgs e)
        {
            ShowGestionCitas();
        }

        private void BtnGestionPacientes_Click(object sender, RoutedEventArgs e)
        {
            ShowGestionPacientes();
        }

        private void ShowDashboard()
        {
            HeaderTitle.Text = "Dashboard Recepción";
            DashboardContainer.Visibility = Visibility.Visible;
            CitasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Collapsed;

            // Refresh Dashboard Data
            LoadUpcomingAppointments();
            UpdateCalendar(DateTime.Today);
            DrawWeeklyChart();
        }

        private void ShowGestionCitas()
        {
            HeaderTitle.Text = "Gestión de Citas";
            DashboardContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Visible;
            PacientesContainer.Visibility = Visibility.Collapsed;
            LoadAllCitasList();
        }

        private void ShowGestionPacientes()
        {
            HeaderTitle.Text = "Gestión de Pacientes";
            DashboardContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Visible;
            LoadPacientesList();
        }
        #endregion

        #region Gestion Citas Logic
        private List<CitaDTO> _allCitasList; // For Management View
        #endregion

        #region Gestion Pacientes Logic
        private List<PacienteDTO> _allPacientesList; // For Management View

        private void LoadAllCitasList()
        {
            try
            {
                _allCitasList = _citaService.GetAllCitas();
                FilterCitasList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}");
            }
        }

        private void TxtBuscarCitas_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCitasList();
        }

        private void FilterCitasList()
        {
            if (_allCitasList == null) return;
            var filter = TxtBuscarCitas.Text.ToLower();
            var filtered = _allCitasList.Where(c =>
                (c.PacienteNombre != null && c.PacienteNombre.ToLower().Contains(filter)) ||
                (c.DentistaNombre != null && c.DentistaNombre.ToLower().Contains(filter))
            ).ToList();
            CitasDataGrid.ItemsSource = filtered;
        }

        private void BtnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CitaDTO cita)
            {
                var form = new CitaFormView(cita);
                var window = new Window
                {
                    Title = "Editar Cita",
                    Content = form,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                form.OnSave += (updatedCita) =>
                {
                    try
                    {
                        _citaService.UpdateCita(updatedCita);
                        LoadAllCitasList();
                        LoadUpcomingAppointments();
                        UpdateCalendar(_currentMonth);
                        window.Close();
                        MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                form.OnCancel += () => window.Close();
                window.ShowDialog();
            }
        }

        private void BtnEliminarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CitaDTO cita)
            {
                var result = MessageBox.Show($"¿Está seguro de eliminar la cita del paciente {cita.PacienteNombre} programada para {cita.FechaHora:dd/MM/yyyy HH:mm}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _citaService.DeleteCita(cita.Id);
                        LoadAllCitasList();
                        LoadUpcomingAppointments();
                        UpdateCalendar(_currentMonth);
                        MessageBox.Show("Cita eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        #endregion

        #region Gestion Pacientes Logic
        private void LoadPacientesList()
        {
            try
            {
                _allPacientesList = _pacienteService.GetAllPacientes();
                FilterPacientesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}");
            }
        }

        private void TxtBuscarPacientes_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPacientesList();
        }

        private void FilterPacientesList()
        {
            if (_allPacientesList == null) return;
            var filter = TxtBuscarPacientes.Text.ToLower();
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
                var result = MessageBox.Show($"¿Está seguro de eliminar al paciente {paciente.Nombre}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _pacienteService.DeletePaciente(paciente.Id);
                        LoadPacientesList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar paciente: {ex.Message}");
                    }
                }
            }
        }
        #endregion
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
        public int Id { get; set; }
        public string HoraStr { get; set; }
        public string NombrePaciente { get; set; }
        public string Tratamiento { get; set; }
        public string Estatus { get; set; }
        public string? DentistaNombre { get; set; }
    }

    public class RelayCommand<T> : System.Windows.Input.ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute((T)parameter!);
        }

        public void Execute(object? parameter)
        {
            _execute((T)parameter!);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }
    }
    #endregion
}
