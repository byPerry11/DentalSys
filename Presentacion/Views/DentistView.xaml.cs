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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.Win32;

namespace Presentacion.Views
{
    public partial class DentistView : UserControl, INotifyPropertyChanged
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;
        private readonly FacturaService _facturaService;
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
            _facturaService = new FacturaService();
            _currentMonth = DateTime.Now;

            // Get logged-in dentist ID
            if (UserSession.CurrentUser != null)
            {
                if (UserSession.CurrentUser.Id_Dentista.HasValue)
                {
                    _currentDentistaId = UserSession.CurrentUser.Id_Dentista.Value;
                    TxtDentistName.Text = $"Dr. {UserSession.CurrentUser.Username}";
                }
                else
                {
                    MessageBox.Show("No se encontró el registro de dentista asociado a este usuario.",
                                  "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    // User will see empty dashboard, but won't crash
                    _currentDentistaId = 0;
                }
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

        private void BtnConsultas_Click(object sender, RoutedEventArgs e)
        {
            ShowConsultas();
        }

        private void ShowAgenda()
        {
            HeaderTitle.Text = "Mi Agenda Hoy";
            AgendaContainer.Visibility = Visibility.Visible;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Collapsed;

            LoadDashboardData();
        }

        private void ShowPacientes()
        {
            HeaderTitle.Text = "Mis Pacientes";
            AgendaContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Visible;

            LoadPacientesList();
        }

        private void ShowConsultas()
        {
            HeaderTitle.Text = "Consultas";
            AgendaContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Visible;
            PacientesContainer.Visibility = Visibility.Collapsed;

            LoadCitasConfirmadas();
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

        #region Consultas Logic
        private CitaDTO? _selectedCita;

        private void LoadCitasConfirmadas()
        {
            try
            {
                var citasConfirmadas = _citaService.GetAllCitas()
                    .Where(c => c.DentistaId == _currentDentistaId &&
                               c.Estatus?.ToLower() == "confirmada" &&
                               c.FechaHora >= DateTime.Now)
                    .OrderBy(c => c.FechaHora)
                    .ToList();

                CitasConfirmadasDataGrid.ItemsSource = citasConfirmadas;

                // Clear selection
                _selectedCita = null;
                CitaDetallesPanel.Visibility = Visibility.Collapsed;
                TxtNoSeleccion.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas confirmadas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CitasConfirmadasDataGrid_Click(object sender, MouseButtonEventArgs e)
        {
            if (CitasConfirmadasDataGrid.SelectedItem is CitaDTO cita)
            {
                _selectedCita = cita;
                MostrarDetallesCita(cita);
            }
        }

        private void MostrarDetallesCita(CitaDTO cita)
        {
            TxtDetallePaciente.Text = cita.PacienteNombre;
            TxtDetalleFecha.Text = cita.FechaHora.ToString("dd/MM/yyyy HH:mm");
            TxtDetalleEstado.Text = cita.Estatus;
            TxtDetalleDentista.Text = cita.DentistaNombre;

            CitaDetallesPanel.Visibility = Visibility.Visible;
            TxtNoSeleccion.Visibility = Visibility.Collapsed;

            // Load consultations for this appointment
            LoadConsultas(cita.Id);
        }

        private void LoadConsultas(int citaId)
        {
            try
            {
                var consultaService = new ConsultaService();
                var todasConsultas = consultaService.GetAllConsultas();

                // Filter consultations for this specific appointment
                var consultasCita = todasConsultas.Where(c => c.CitaId == citaId).ToList();

                ConsultasDataGrid.ItemsSource = consultasCita;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar consultas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConsultasDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ConsultasDataGrid.SelectedItem is ConsultaDTO consulta)
            {
                MostrarDetallesConsulta(consulta);
            }
        }

        private void MostrarDetallesConsulta(ConsultaDTO consulta)
        {
            var form = new ConsultaInfoView(consulta);
            var window = new Window
            {
                Title = "Detalles de Consulta",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            window.ShowDialog();
        }

        private void CitasConfirmadasDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selectedCita != null)
            {
                MostrarFormularioConvertirConsulta(_selectedCita);
            }
        }

        private void MostrarFormularioConvertirConsulta(CitaDTO cita)
        {
            // Check if consultation already exists for this appointment
            var consultaService = new ConsultaService();
            if (consultaService.ExistsConsultaByCitaId(cita.Id))
            {
                MessageBox.Show("Ya existe una consulta generada para esta cita.",
                              "Consulta Existente",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
                return;
            }

            var form = new GenerarConsultaView(cita);
            var window = new Window
            {
                Title = "Generar Consulta",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnGenerar += (consulta) =>
            {
                try
                {
                    consultaService.AddConsulta(consulta);
                    window.Close();
                    MessageBox.Show("Consulta generada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCitasConfirmadas(); // Refresh list
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al generar consulta: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancelar += () => window.Close();
            window.ShowDialog();
        }

        private void BtnGenerarFactura_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var consulta = button?.DataContext as ConsultaDTO;

                if (consulta == null)
                {
                    MessageBox.Show("No se pudo obtener la consulta seleccionada.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var factura = _facturaService.BuildFacturaFromConsulta(consulta);

                GenerateFacturaPdf(factura);

                MessageBox.Show("Factura generada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateFacturaPdf(FacturaDTO factura)
        {
            if (factura == null)
                throw new ArgumentNullException(nameof(factura));

            // Crear documento PDF
            var document = new PdfDocument();
            document.Info.Title = $"Factura {factura.Folio}";

            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
            var fontLabel = new XFont("Arial", 10, XFontStyle.Regular);
            var fontValue = new XFont("Arial", 10, XFontStyle.Bold);

            double marginLeft = 40;
            double y = 40;

            // Titulo
            gfx.DrawString("Consultorio Dental - Factura",
                fontTitle, XBrushes.Black, new XRect(marginLeft, y, page.Width - 2 * marginLeft, 30), XStringFormats.TopLeft);
            y += 40;

            // Datos generales
            gfx.DrawString("Folio:", fontLabel, XBrushes.Black, marginLeft, y);
            gfx.DrawString(factura.Folio, fontValue, XBrushes.Black, marginLeft + 80, y);
            y += 20;

            gfx.DrawString("Fecha:", fontLabel, XBrushes.Black, marginLeft, y);
            gfx.DrawString(factura.FechaConsulta.ToString("dd/MM/yyyy HH:mm"),
                fontValue, XBrushes.Black, marginLeft + 80, y);
            y += 30;

            // Paciente
            gfx.DrawString("Paciente:", fontLabel, XBrushes.Black, marginLeft, y);
            gfx.DrawString(factura.PacienteNombre, fontValue, XBrushes.Black, marginLeft + 80, y);
            y += 20;

            if (!string.IsNullOrWhiteSpace(factura.PacienteTelefono))
            {
                gfx.DrawString("Telefono:", fontLabel, XBrushes.Black, marginLeft, y);
                gfx.DrawString(factura.PacienteTelefono, fontValue, XBrushes.Black, marginLeft + 80, y);
                y += 20;
            }

            if (!string.IsNullOrWhiteSpace(factura.PacienteEmail))
            {
                gfx.DrawString("Email:", fontLabel, XBrushes.Black, marginLeft, y);
                gfx.DrawString(factura.PacienteEmail, fontValue, XBrushes.Black, marginLeft + 80, y);
                y += 30;
            }

            // Tratamiento
            gfx.DrawString("Tratamiento:", fontLabel, XBrushes.Black, marginLeft, y);
            gfx.DrawString(factura.TratamientoNombre, fontValue, XBrushes.Black, marginLeft + 90, y);
            y += 30;

            // Totales
            double xTotales = page.Width - 220;

            gfx.DrawString("Subtotal:", fontLabel, XBrushes.Black, xTotales, y);
            gfx.DrawString(factura.Subtotal.ToString("C"), fontValue, XBrushes.Black, xTotales + 80, y);
            y += 20;

            gfx.DrawString($"IVA ({factura.TasaIVA:P0}):", fontLabel, XBrushes.Black, xTotales, y);
            gfx.DrawString(factura.IVA.ToString("C"), fontValue, XBrushes.Black, xTotales + 80, y);
            y += 20;

            gfx.DrawString("Total:", fontTitle, XBrushes.Black, xTotales, y);
            gfx.DrawString(factura.Total.ToString("C"), fontTitle, XBrushes.Black, xTotales + 80, y);

            // Guardar archivo
            var dialog = new SaveFileDialog
            {
                FileName = $"Factura_{factura.Folio}.pdf",
                Filter = "Archivo PDF (.pdf)|.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                document.Save(dialog.FileName);
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
