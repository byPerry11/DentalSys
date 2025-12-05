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
    public partial class RecepcionistView : UserControl
    {
        // Bandera interna para saber si el menu esta colapsado
        private bool _menuColapsado = false;

        public RecepcionistView()
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


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

    private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            //Creamos una instancia de la ventana de Login para cerrar sesión y volver a ella
            LoginView Login = new LoginView();
            Login.Show();
            //Cerramos la ventana actual de Recepcionista
            Window.GetWindow(this)?.Close();
        }
    }
}
