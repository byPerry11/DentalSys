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

namespace Presentacion.Views
{
    public partial class AdminView : UserControl
    {
        private readonly UserService _userService;
        private readonly TratamientoService _tratamientoService;
        private readonly PacienteService _pacienteService;
        private readonly CitaService _citaService;
        private readonly ConsultaService _consultaService;
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

            LoadUsuarios();
            LoadTratamientos();
            LoadPacientes();
            // LoadCitas and LoadConsultas will be called when navigating to the view
        }

        private void BtnGestionCitasConsultasClick(object sender, RoutedEventArgs e)
        {
            HeaderTitle.Text = "Gestión de Citas y Consultas";
            DashboardGrid.Visibility = Visibility.Collapsed;
            UsuariosContainer.Visibility = Visibility.Collapsed;
            PacientesContainer.Visibility = Visibility.Collapsed;
            TratamientosContainer.Visibility = Visibility.Collapsed;
            CitasConsultasContainer.Visibility = Visibility.Visible;

            // Default to Citas
            BtnToggleCitas_Click(null, null);
        }

        private void BtnToggleCitas_Click(object sender, RoutedEventArgs e)
        {
            // Highlight Citas button
            BtnToggleCitas.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980B9"));
            BtnToggleCitas.BorderThickness = new Thickness(0, 0, 0, 2);

            BtnToggleConsultas.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));
            BtnToggleConsultas.BorderThickness = new Thickness(0);

            BorderCitas.Visibility = Visibility.Visible;
            BorderConsultas.Visibility = Visibility.Collapsed;

            LoadCitas();
        }

        private void BtnToggleConsultas_Click(object sender, RoutedEventArgs e)
        {
            // Highlight Consultas button
            BtnToggleConsultas.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2980B9"));
            BtnToggleConsultas.BorderThickness = new Thickness(0, 0, 0, 2);

            BtnToggleCitas.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7F8C8D"));
            BtnToggleCitas.BorderThickness = new Thickness(0);

            BorderCitas.Visibility = Visibility.Collapsed;
            BorderConsultas.Visibility = Visibility.Visible;

            LoadConsultas();
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

        private void TxtBuscarCitaConsulta_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BorderCitas.Visibility == Visibility.Visible)
            {
                FilterCitas();
            }
            else
            {
                FilterConsultas();
            }
        }

        private void FilterCitas()
        {
            if (_allCitas == null) return;

            var filter = TxtBuscarCitaConsulta.Text.ToLower();
            var filtered = _allCitas.Where(c =>
                (c.PacienteNombre != null && c.PacienteNombre.ToLower().Contains(filter)) ||
                (c.DentistaNombre != null && c.DentistaNombre.ToLower().Contains(filter))
            ).ToList();

            CitasDataGrid.ItemsSource = filtered;
        }

        private void FilterConsultas()
        {
            if (_allConsultas == null) return;

            var filter = TxtBuscarCitaConsulta.Text.ToLower();
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
                CitasConsultasContainer.Visibility = Visibility.Collapsed;
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
                CitasConsultasContainer.Visibility = Visibility.Collapsed;
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
            CitasConsultasContainer.Visibility = Visibility.Collapsed;
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

