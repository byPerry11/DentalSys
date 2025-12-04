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
    public partial class RecepcionistView : UserControl
    {
        private readonly CitaService _citaService;
        private readonly PacienteService _pacienteService;

        public RecepcionistView()
        {
            InitializeComponent();
            _citaService = new CitaService();
            _pacienteService = new PacienteService();
            LoadCitas();
        }

        private void LoadCitas()
        {
            try
            {
                var citas = _citaService.GetUpcomingCitas();
                CitasDataGrid.ItemsSource = citas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegistrarPaciente_Click(object sender, RoutedEventArgs e)
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
                    window.Close();
                    MessageBox.Show("Paciente registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar paciente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnNuevaCita_Click(object sender, RoutedEventArgs e)
        {
            OpenCitaForm();
        }

        private void BtnEditarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CitaDTO cita)
            {
                OpenCitaForm(cita);
            }
        }

        private void OpenCitaForm(CitaDTO cita = null)
        {
            var form = new CitaFormView(cita);
            var title = cita == null ? "Nueva Cita" : "Editar Cita";
            var window = new Window
            {
                Title = title,
                Content = form,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            form.OnSave += (citaToSave) =>
            {
                try
                {
                    if (citaToSave.Id == 0)
                    {
                        _citaService.AddCita(citaToSave);
                        MessageBox.Show("Cita agendada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _citaService.UpdateCita(citaToSave);
                        MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    LoadCitas();
                    window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al guardar cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            form.OnCancel += () => window.Close();

            window.ShowDialog();
        }

        private void BtnEliminarCita_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CitaDTO cita)
            {
                var result = MessageBox.Show($"¿Está seguro de eliminar la cita del paciente {cita.PacienteNombre}?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _citaService.DeleteCita(cita.Id);
                        LoadCitas();
                        MessageBox.Show("Cita eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            LoginView Login = new LoginView();
            Login.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
