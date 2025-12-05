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

    public partial class DentistView : UserControl
    {
        public DentistView()
        {
            InitializeComponent();
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnCerrarSesionClick(object sender, RoutedEventArgs e)
        {
            //Creamos una instancia de la ventana de Login para cerrar sesión y volver a ella
            LoginView Login = new LoginView();
            Login.Show();
            //Cerramos la ventana actual de Dentista
            Window.GetWindow(this)?.Close();
        }
    }
}
