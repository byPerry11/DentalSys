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
        private readonly DentistaService _dentistaService;
        private readonly AdministradorService _administradorService;
        private readonly RecepcionistaService _recepcionistaService;
        private List<CitaDTO> _allCitas;
        private List<ConsultaDTO> _allConsultas;
        private List<PacienteDTO> _allPacientes;

        public AdminView()
        {
            InitializeComponent();
            _userService = new UserService();
            _tratamientoService = new TratamientoService();
            _pacienteService = new PacienteService();
            _citaService = new CitaService();
            _consultaService = new ConsultaService();
            _facturaService = new FacturaService();
            _dentistaService = new DentistaService();
            _administradorService = new AdministradorService();
            _recepcionistaService = new RecepcionistaService();

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
            DentistasContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Visible;
            AdministradoresContainer.Visibility = Visibility.Collapsed;
            RecepcionistasContainer.Visibility = Visibility.Collapsed;

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
                DentistasContainer.Visibility = Visibility.Collapsed;
                AdministradoresContainer.Visibility = Visibility.Collapsed;
                RecepcionistasContainer.Visibility = Visibility.Collapsed;

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

                // Ocultar textos
                ImgProfile.Visibility = Visibility.Collapsed;
                TxtAdminName.Visibility = Visibility.Collapsed;
                TxtAdminRole.Visibility = Visibility.Collapsed;
                TxtGestionUsuarios.Visibility = Visibility.Collapsed;
                TxtGestionPacientes.Visibility = Visibility.Collapsed;
                TxtGestionCitas.Visibility = Visibility.Collapsed;
                TxtGestionAdministrador.Visibility = Visibility.Collapsed;
                TxtGestionDentistas.Visibility = Visibility.Collapsed;
                TxtGestionRecepcionistas.Visibility = Visibility.Collapsed;
                TxtGestionConsultas.Visibility = Visibility.Collapsed;
                TxtTratamientos.Visibility = Visibility.Collapsed;
                TxtCerrarSesion.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Expandir menu
                SideMenuColumn.Width = new GridLength(220);

                // Mostrar textos
                ImgProfile.Visibility = Visibility.Visible;
                TxtAdminName.Visibility = Visibility.Visible;
                TxtAdminRole.Visibility = Visibility.Visible;
                TxtGestionUsuarios.Visibility = Visibility.Visible;
                TxtGestionPacientes.Visibility = Visibility.Visible;
                TxtGestionCitas.Visibility = Visibility.Visible;
                TxtGestionAdministrador.Visibility = Visibility.Visible;
                TxtGestionDentistas.Visibility = Visibility.Visible;
                TxtGestionRecepcionistas.Visibility = Visibility.Visible;
                TxtGestionConsultas.Visibility = Visibility.Visible;
                TxtTratamientos.Visibility = Visibility.Visible;
                TxtCerrarSesion.Visibility = Visibility.Visible;
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
                DentistasContainer.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Visible;
                AdministradoresContainer.Visibility = Visibility.Collapsed;
                RecepcionistasContainer.Visibility = Visibility.Collapsed;
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
                DentistasContainer.Visibility = Visibility.Collapsed;
                AdministradoresContainer.Visibility = Visibility.Collapsed;
                RecepcionistasContainer.Visibility = Visibility.Collapsed;
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
                _allPacientes = _pacienteService.GetAllPacientes();
                FilterPacientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar pacientes: {ex.Message}");
            }
        }

        private void TxtBuscarPacientes_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterPacientes();
        }

        private void FilterPacientes()
        {
            if (_allPacientes == null) return;

            var filter = TxtBuscarPacientes.Text.ToLower();
            var filtered = _allPacientes.Where(p =>
                (p.Nombre != null && p.Nombre.ToLower().Contains(filter)) ||
                (p.Email != null && p.Email.ToLower().Contains(filter)) ||
                (p.Telefono != null && p.Telefono.ToLower().Contains(filter))
            ).ToList();

            PacientesDataGrid.ItemsSource = filtered;
        }

        private void BtnGestionPacientesClick(object sender, RoutedEventArgs e)
        {
            DashboardGrid.Visibility = Visibility.Collapsed;
            TratamientosContainer.Visibility = Visibility.Collapsed;
            UsuariosContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Visible;
            DentistasContainer.Visibility = Visibility.Collapsed;
            AdministradoresContainer.Visibility = Visibility.Collapsed;
            RecepcionistasContainer.Visibility = Visibility.Collapsed;
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

        private void LoadDentistas()
        {
            try
            {
                var dentistas = _dentistaService.GetAllDentistas();
                DentistasDataGrid.ItemsSource = dentistas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar dentistas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGestionDentistasClick(object sender, RoutedEventArgs e)
        {
            HeaderTitle.Text = "Gestion de Dentistas";

            DashboardGrid.Visibility = Visibility.Collapsed;
            UsuariosContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Collapsed;
            TratamientosContainer.Visibility = Visibility.Collapsed;
            CitasContainer.Visibility = Visibility.Collapsed;
            ConsultasContainer.Visibility = Visibility.Collapsed;
            AdministradoresContainer.Visibility = Visibility.Collapsed;
            RecepcionistasContainer.Visibility = Visibility.Collapsed;

            DentistasContainer.Visibility = Visibility.Visible;

            LoadDentistas();
        }

        private void BtnNuevoDentista_Click(object sender, RoutedEventArgs e)
        {
            var form = new DentistaFormView();

            var window = new Window
            {
                Title = "Nuevo Dentista",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (dentista) =>
            {
                try
                {
                    _dentistaService.AddDentista(dentista);
                    LoadDentistas();
                    window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar dentista: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnEditarDentista_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DentistaDTO dentista)
            {
                var form = new DentistaFormView(dentista);

                var window = new Window
                {
                    Title = "Editar Dentista",
                    Content = form,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                form.OnSave += (updatedDentista) =>
                {
                    try
                    {
                        _dentistaService.UpdateDentista(updatedDentista);
                        LoadDentistas();
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar dentista: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                form.OnCancel += () => window.Close();

                window.ShowDialog();
            }
        }

        private void BtnEliminarDentista_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DentistaDTO dentista)
            {
                var result = MessageBox.Show(
                    $"¿Esta seguro de eliminar al dentista {dentista.Nombre}?",
                    "Confirmar Eliminacion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _dentistaService.DeleteDentista(dentista.Id_Dentista);
                        LoadDentistas();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar dentista: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void LoadAdministradores()
        {
            try
            {
                var admins = _administradorService.GetAllAdministradores();
                AdministradoresDataGrid.ItemsSource = admins;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar administradores: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGestionAdministradorClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Administradores";

                DashboardGrid.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Collapsed;
                PacientesContainer.Visibility = Visibility.Collapsed;
                CitasContainer.Visibility = Visibility.Collapsed;
                ConsultasContainer.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Collapsed;
                DentistasContainer.Visibility = Visibility.Collapsed;
                RecepcionistasContainer.Visibility = Visibility.Collapsed;

                AdministradoresContainer.Visibility = Visibility.Visible;

                LoadAdministradores();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar administradores: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoAdministrador_Click(object sender, RoutedEventArgs e)
        {
            var form = new AdministradorFormView();
            var window = new Window
            {
                Title = "Nuevo Administrador",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (admin) =>
            {
                try
                {
                    _administradorService.AddAdministrador(admin);
                    LoadAdministradores();
                    window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar administrador: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnEditarAdministrador_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AdministradorDTO admin)
            {
                var form = new AdministradorFormView(admin);
                var window = new Window
                {
                    Title = "Editar Administrador",
                    Content = form,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                form.OnSave += (updated) =>
                {
                    try
                    {
                        _administradorService.UpdateAdministrador(updated);
                        LoadAdministradores();
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar administrador: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                form.OnCancel += () => window.Close();

                window.ShowDialog();
            }
        }

        private void BtnEliminarAdministrador_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is AdministradorDTO admin)
            {
                var result = MessageBox.Show($"¿Estás seguro de eliminar al administrador {admin.Nombre}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _administradorService.DeleteAdministrador(admin.Id_Administrador);
                        LoadAdministradores();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar administrador: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnNuevoRecepcionista_Click(object sender, RoutedEventArgs e)
        {
            var form = new RecepcionistaFormView();
            var window = new Window
            {
                Title = "Nuevo Recepcionista",
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (recep) =>
            {
                try
                {
                    _recepcionistaService.AddRecepcionista(recep);
                    LoadRecepcionistas();
                    window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar recepcionista: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnEditarRecepcionista_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RecepcionistaDTO recep)
            {
                var form = new RecepcionistaFormView(recep);
                var window = new Window
                {
                    Title = "Editar Recepcionista",
                    Content = form,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                form.OnSave += (updated) =>
                {
                    try
                    {
                        _recepcionistaService.UpdateRecepcionista(updated);
                        LoadRecepcionistas();
                        window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar recepcionista: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };

                form.OnCancel += () => window.Close();

                window.ShowDialog();
            }
        }

        private void BtnEliminarRecepcionista_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is RecepcionistaDTO recep)
            {
                var result = MessageBox.Show($"¿Estás seguro de eliminar al recepcionista {recep.Nombre}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _recepcionistaService.DeleteRecepcionista(recep.Id_Recepcionista);
                        LoadRecepcionistas();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar recepcionista: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnGestionRecepcionistasClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Recepcionistas";

                DashboardGrid.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Collapsed;
                PacientesContainer.Visibility = Visibility.Collapsed;
                CitasContainer.Visibility = Visibility.Collapsed;
                ConsultasContainer.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Collapsed;
                DentistasContainer.Visibility = Visibility.Collapsed;
                AdministradoresContainer.Visibility = Visibility.Collapsed;
                RecepcionistasContainer.Visibility = Visibility.Visible;

                LoadRecepcionistas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar recepcionistas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRecepcionistas()
        {
            try
            {
                var receps = _recepcionistaService.GetAllRecepcionistas();
                RecepcionistasDataGrid.ItemsSource = receps;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar recepcionistas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ConsultasDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ConsultasDataGrid.SelectedItem is ConsultaDTO consulta)
            {
                var view = new ConsultaInfoView(consulta);
                var window = new Window
                {
                    Title = "Detalles de Consulta",
                    Content = view,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                window.ShowDialog();
            }
        }
    }
}

