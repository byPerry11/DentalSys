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
    }
}
