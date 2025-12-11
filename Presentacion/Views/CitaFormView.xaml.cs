using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ApplicationLogic.DTOs;
using ApplicationLogic.Services;

namespace Presentacion.Views
{
    public partial class CitaFormView : UserControl
    {
        private readonly PacienteService _pacienteService;
        private readonly UserService _userService;
        private CitaDTO _cita;

        public event Action<CitaDTO> OnSave;
        public event Action OnCancel;

        public CitaFormView(CitaDTO cita = null)
        {
            InitializeComponent();
            _pacienteService = new PacienteService();
            _userService = new UserService();
            _cita = cita;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var pacientes = _pacienteService.GetAllPacientes();
                CmbPaciente.ItemsSource = pacientes;

                // Only load users with 'dentista' role
                var users = _userService.GetAllUsers();
                var dentistas = users.Where(u => u.Role != null && u.Role.ToLower() == "dentista").ToList();
                CmbDentista.ItemsSource = dentistas;

                // Populate Hours 09:00 to 18:00
                var hours = new List<string>();
                for (int i = 9; i <= 18; i++)
                {
                    hours.Add($"{i:00}:00");
                }
                CmbHora.ItemsSource = hours;

                if (_cita != null)
                {
                    CmbPaciente.SelectedValue = _cita.PacienteId;
                    CmbDentista.SelectedValue = _cita.DentistaId;
                    DpFecha.SelectedDate = _cita.FechaHora.Date;
                    CmbHora.SelectedItem = _cita.FechaHora.ToString("HH:mm");

                    foreach (ComboBoxItem item in CmbEstatus.Items)
                    {
                        if (item.Content.ToString() == _cita.Estatus)
                        {
                            CmbEstatus.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbPaciente.SelectedValue == null || CmbDentista.SelectedValue == null || !DpFecha.SelectedDate.HasValue || CmbHora.SelectedItem == null)
                {
                    MessageBox.Show("Por favor complete todos los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!TimeSpan.TryParse(CmbHora.SelectedItem.ToString(), out TimeSpan hora))
                {
                    MessageBox.Show("Hora inválida.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var fecha = DpFecha.SelectedDate.Value.Date + hora;

                if (_cita == null)
                {
                    _cita = new CitaDTO();
                }

                _cita.PacienteId = (int)CmbPaciente.SelectedValue;
                _cita.DentistaId = (int)CmbDentista.SelectedValue;
                _cita.FechaHora = fecha;
                _cita.Estatus = (CmbEstatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Pendiente";

                OnSave?.Invoke(_cita);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }
    }
}
