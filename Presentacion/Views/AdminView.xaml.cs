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
    }
}
