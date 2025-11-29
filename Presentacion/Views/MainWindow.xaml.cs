using Presentacion.Views;
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
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void NavigateTo(string viewName)
        {
            if (MainContent == null)
            {
                MessageBox.Show("Error: El contenedor principal no está inicializado.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            switch (viewName)
            {
                case "AdminView":
                    MainContent.Content = new AdminView();
                    break;
                case "DentistView":
                    MainContent.Content = new DentistView();
                    break;
                case "ReceptionistView":
                    MainContent.Content = new RecepcionistView();
                    break;
                default:
                    MessageBox.Show($"Vista no encontrada: {viewName}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

            this.Title = $"DentalSys - {viewName}";
        }
    }
}