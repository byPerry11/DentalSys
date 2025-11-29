using System.Configuration;
using System.Data;
using System.Windows;
using Presentacion.Views;

namespace Presentacion
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoginView loginView = new LoginView();
            loginView.Show();
        }
    }

}
