using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ApplicationLogic.DTOs;
using ApplicationLogic.Services;

namespace Presentacion.Views
{
    public partial class GenerarConsultaView : UserControl
    {
        private readonly CitaDTO _cita;
        private readonly TratamientoService _tratamientoService;

        public event Action<ConsultaDTO>? OnGenerar;
        public event Action? OnCancelar;

        public GenerarConsultaView(CitaDTO cita)
        {
            InitializeComponent();
            _cita = cita;
            _tratamientoService = new TratamientoService();
            LoadData();
        }

        private void LoadData()
        {
            // Display appointment info
            TxtPaciente.Text = _cita.PacienteNombre;
            TxtFechaHora.Text = _cita.FechaHora.ToString("dd/MM/yyyy HH:mm");

            // Load treatments
            try
            {
                var tratamientos = _tratamientoService.GetAllTratamientos();
                CmbTratamiento.ItemsSource = tratamientos;

                if (tratamientos.Any())
                {
                    CmbTratamiento.SelectedIndex = 0;
                    // Set default price from selected treatment
                    if (CmbTratamiento.SelectedItem is TratamientoDTO tratamiento)
                    {
                        TxtPrecio.Text = tratamiento.Precio.ToString("F2");
                    }
                }

                // Update price when treatment changes
                CmbTratamiento.SelectionChanged += (s, e) =>
                {
                    if (CmbTratamiento.SelectedItem is TratamientoDTO t)
                    {
                        TxtPrecio.Text = t.Precio.ToString("F2");
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tratamientos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (CmbTratamiento.SelectedItem == null)
            {
                MessageBox.Show("Por favor seleccione un tratamiento.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtPrecio.Text) || !decimal.TryParse(TxtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Por favor ingrese un precio válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected treatment
            var tratamientoSeleccionado = CmbTratamiento.SelectedItem as TratamientoDTO;
            if (tratamientoSeleccionado == null)
            {
                MessageBox.Show("Error al obtener el tratamiento seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create ConsultaDTO
            var consulta = new ConsultaDTO
            {
                CitaId = _cita.Id,
                TratamientoId = tratamientoSeleccionado.Id,
                Precio = precio,
                Notas = TxtNotas.Text
            };

            OnGenerar?.Invoke(consulta);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OnCancelar?.Invoke();
        }
    }
}
