using System.Windows;
using ApplicationLogic.Services;

namespace Presentacion
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _dbService = new DatabaseService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            // Ejecuta la prueba de conexion y muestra el resultado
            TxtResult.Text = _dbService.ProbarConexion();
        }
    }
}