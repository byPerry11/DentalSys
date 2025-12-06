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

        public AdminView()
        {
            InitializeComponent();
            _userService = new UserService();
            _tratamientoService = new TratamientoService();
            _pacienteService = new PacienteService();
            LoadUsuarios();
            LoadTratamientos();
            LoadPacientes();
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
    }
}

