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

namespace Presentacion.Views
{

    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
        }
        private void BtnCerrarSesionClick(object sender, RoutedEventArgs e)
        {
            //Creamos una instancia de la ventana de Login para cerrar sesión y volver a ella
            LoginView Login = new LoginView();
            Login.Show();
            //Cerramos la ventana actual de Administrador
            Window.GetWindow(this)?.Close();
        }

        private void BtnTratamientosClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Configurar el título
                HeaderTitle.Text = "Gestión de Tratamientos";

                // Ocultar el dashboard y mostrar el DataGrid
                DashboardGrid.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Visible;

                // Cargar datos
                var service = new TratamientoService();
                var tratamientos = service.GetAllTratamientos();

                TratamientosDataGrid.ItemsSource = tratamientos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tratamientos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void BtnGestionUsuariosClick(object sender, RoutedEventArgs e)
        {
            try
            {
                HeaderTitle.Text = "Gestión de Usuarios";

                DashboardGrid.Visibility = Visibility.Collapsed;
                TratamientosContainer.Visibility = Visibility.Collapsed;
                UsuariosContainer.Visibility = Visibility.Visible;

                var service = new UserService();
                var users = service.GetAllUsers();

                UsuariosDataGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el usuario seleccionado desde el botón
                var button = sender as Button;
                var user = button?.DataContext as ApplicationLogic.DTOs.UserDTO;

                if (user != null)
                {
                    var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar al usuario '{user.Username}'?",
                                                 "Confirmar eliminación",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        var service = new UserService();
                        service.DeleteUser(user.Id);

                        // Recargar la lista
                        BtnGestionUsuariosClick(null, null);
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
                BtnGestionUsuariosClick(null, null); // Refrescar lista
            }
        }

        private void BtnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var user = button?.DataContext as ApplicationLogic.DTOs.UserDTO;

            if (user != null)
            {
                var form = new UserFormView(user);
                if (form.ShowDialog() == true)
                {
                    BtnGestionUsuariosClick(null, null); // Refrescar lista
                }
            }
        }

        private void BtnEditarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var tratamiento = button?.DataContext as ApplicationLogic.DTOs.TratamientoDTO;

            if (tratamiento != null)
            {
                var form = new TratamientoFormView(tratamiento);
                if (form.ShowDialog() == true)
                {
                    BtnTratamientosClick(null, null); // Refrescar lista
                }
            }
        }

        private void BtnEliminarTratamiento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var tratamiento = button?.DataContext as ApplicationLogic.DTOs.TratamientoDTO;

                if (tratamiento != null)
                {
                    var result = MessageBox.Show($"¿Estás seguro de que deseas eliminar el tratamiento '{tratamiento.Nombre}'?",
                                                 "Confirmar eliminación",
                                                 MessageBoxButton.YesNo,
                                                 MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        var service = new TratamientoService();
                        service.DeleteTratamiento(tratamiento.Id);

                        BtnTratamientosClick(null, null); // Refrescar lista
                        MessageBox.Show("Tratamiento eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar tratamiento: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
