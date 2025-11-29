using Presentacion.ViewModels;
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

namespace Presentacion.Views
{
    /// <summary>
    /// Vista de inicio de sesión
    /// </summary>
    public partial class LoginView : Window
    {
        /// <summary>
        /// Constructor de LoginView
        /// </summary>
        public LoginView()
        {
            InitializeComponent();
            // Establecer el DataContext al LoginViewModel para habilitar los bindings
            DataContext = new LoginViewModel();
        }

        /// <summary>
        /// Maneja el evento de clic del botón Salir
        /// </summary>
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
