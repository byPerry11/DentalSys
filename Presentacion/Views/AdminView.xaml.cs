using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ApplicationLogic.Services;
using ApplicationLogic.DTOs;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Microsoft.Win32;
using System.Printing;

namespace Presentacion.Views
{
    public partial class AdminView : UserControl
    {
        private readonly UserService _userService;
        private readonly TratamientoService _tratamientoService;
        private readonly PacienteService _pacienteService;
        private readonly CitaService _citaService;
        private readonly ConsultaService _consultaService;
        private readonly FacturaService _facturaService;
        private List<CitaDTO> _allCitas;
        private List<ConsultaDTO> _allConsultas;

        public AdminView()
        {
            InitializeComponent();
            _userService = new UserService();
            _tratamientoService = new TratamientoService();
            _pacienteService = new PacienteService();
            _citaService = new CitaService();
            _consultaService = new ConsultaService();
            _facturaService = new FacturaService();

            LoadUsuarios();
            LoadTratamientos();
            LoadPacientes();
            // LoadCitas and LoadConsultas will be called when navigating to the view
        }

        private void BtnGestionCitasClick(object sender, RoutedEventArgs e)
        {
            HeaderTitle.Text = "Gestión de Citas";
            DashboardGrid.Visibility = Visibility.Collapsed;
            UsuariosContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Collapsed;
            TratamientosContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Visible;

            LoadCitas();
        }

        private void BtnGestionConsultasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Consultas";

                DashboardGrid.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Collapsed;
                PacientesContainer.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Collapsed;
                CitasContainer.Visibility = Visibility.Collapsed;
                ConsultasContainer.Visibility = Visibility.Visible;

                LoadConsultas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar consultas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerarFactura_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var consulta = button?.DataContext as ConsultaDTO;

                if (consulta == null)
                {
                    MessageBox.Show("No se pudo obtener la consulta selcciona.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var factura = _facturaService.BuildFacturaFromConsulta(consulta);

                GenerateFacturaPdf(factura);

                MessageBox.Show("Factura generada correctamente.", "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
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



        private void LoadCitas()
        {
            try
            {
                _allCitas = _citaService.GetAllCitas();
                FilterCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}");
            }
        }

        private void LoadConsultas()
        {
            try
            {
                _allConsultas = _consultaService.GetAllConsultas();
                FilterConsultas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar consultas: {ex.Message}");
            }
        }

        private void TxtBuscarCitas_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCitas();
        }

        private void TxtBuscarConsultas_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterConsultas();
        }

        private void FilterCitas()
        {
            if (_allCitas == null) return;

            var filter = TxtBuscarCitas.Text.ToLower();
            var filtered = _allCitas.Where(c =>
                (c.PacienteNombre != null && c.PacienteNombre.ToLower().Contains(filter)) ||
                (c.DentistaNombre != null && c.DentistaNombre.ToLower().Contains(filter))
            ).ToList();

            CitasDataGrid.ItemsSource = filtered;
        }

        private void FilterConsultas()
        {
            if (_allConsultas == null) return;

            var filter = TxtBuscarConsultas.Text.ToLower();
            var filtered = _allConsultas.Where(c =>
                (c.PacienteNombre != null && c.PacienteNombre.ToLower().Contains(filter)) ||
                (c.DentistaNombre != null && c.DentistaNombre.ToLower().Contains(filter))
            ).ToList();

            ConsultasDataGrid.ItemsSource = filtered;
        }


        private void BtnHamburger_Click(object sender, RoutedEventArgs e)
        {
            // Si la columna esta ancha, la colapsamos; si esta chica, la expandimos
            if (SideMenuColumn.Width.Value > 100)
            {
                // Colapsar menu
                SideMenuColumn.Width = new GridLength(80);
            }
            else
            {
                // Expandir menu
                SideMenuColumn.Width = new GridLength(220);
            }
        }


        private void BtnCerrarSesionClick(object sender, RoutedEventArgs e)
        {
            LoginView Login = new LoginView();
            Login.Show();
            Window.GetWindow(this)?.Close();
        }

        private void BtnTratamientosClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Tratamientos";
                DashboardGrid.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Collapsed;
                PacientesContainer.Visibility = Visibility.Collapsed;
                CitasContainer.Visibility = Visibility.Collapsed;
                ConsultasContainer.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Visible;
                LoadTratamientos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tratamientos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTratamientos()
        {
            var tratamientos = _tratamientoService.GetAllTratamientos();
            TratamientosDataGrid.ItemsSource = tratamientos;
        }

        private void BtnGestionUsuariosClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Usuarios";
                DashboardGrid.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Collapsed;
                PacientesContainer.Visibility = Visibility.Collapsed;
                CitasContainer.Visibility = Visibility.Collapsed;
                ConsultasContainer.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Visible;
                LoadUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsuarios()
        {
            var users = _userService.GetAllUsers();
            UsuariosDataGrid.ItemsSource = users;
        }

        private void BtnEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var user = button?.DataContext as UserDTO;

                if (user != null)
                {
                    var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al usuario '{user.Username}'?",
                                                 "Confirmar eliminación",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _userService.DeleteUser(user.Id);
                        LoadUsuarios();
                        MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            var form = new UserFormView();
            if (form.ShowDialog() == true)
            {
                LoadUsuarios();
            }
        }

        private void BtnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as UserDTO;

            if (user != null)
            {
                var form = new UserFormView(user);
                if (form.ShowDialog() == true)
                {
                    LoadUsuarios();
                }
            }
        }

        private void BtnAgregarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            var form = new TratamientoFormView();
            if (form.ShowDialog() == true)
            {
                LoadTratamientos();
            }
        }

        private void BtnEditarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tratamiento = button?.DataContext as TratamientoDTO;

            if (tratamiento != null)
            {
                var form = new TratamientoFormView(tratamiento);
                if (form.ShowDialog() == true)
                {
                    LoadTratamientos();
                }
            }
        }

        private void BtnEliminarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var tratamiento = button?.DataContext as TratamientoDTO;

                if (tratamiento != null)
                {
                    var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar el tratamiento '{tratamiento.Nombre}'?",
                                                 "Confirmar eliminación",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        _tratamientoService.DeleteTratamiento(tratamiento.Id);
                        LoadTratamientos();
                        MessageBox.Show("Tratamiento eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar tratamiento: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPacientes()
        {
            try
            {
                var pacientes = _pacienteService.GetAllPacientes();
                PacientesDataGrid.ItemsSource = pacientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}");
            }
        }

        private void BtnGestionPacientesClick(object sender, RoutedEventArgs e)
        {
            DashboardGrid.Visibility = Visibility.Collapsed;
            TratamientosContainer.Visibility = Visibility.Collapsed;
            UsuariosContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Visible;
            HeaderTitle.Text = "Gestión de Pacientes";
            LoadPacientes();
        }

        private void BtnNuevoPaciente_Click(object sender, RoutedEventArgs e)
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
                    LoadPacientes();
                    window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar paciente: {ex.Message}");
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
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
                        LoadPacientes();
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
                        LoadPacientes();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar paciente: {ex.Message}");
                    }
                }
            }
        }


        private void ConsultasDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ConsultasDataGrid.SelectedItem is ConsultaDTO consulta)
            {
                var view = new ConsultaInfoView(consulta);
                view.ShowDialog();
            }
        }
    }
}

