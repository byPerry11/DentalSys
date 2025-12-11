using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Presentacion.Views
{
    public partial class DentistDayAppointmentsView : UserControl
    {
        private List<CitaViewModel> _appointments;

        public DentistDayAppointmentsView(DateTime date, List<CitaViewModel> appointments)
        {
            InitializeComponent();
            _appointments = appointments ?? new List<CitaViewModel>();

            TxtHeaderDate.Text = date.ToString("dddd, dd MMMM", CultureInfo.CurrentCulture).ToTitleCase();

            LoadList();

            LstCitas.SelectionChanged += LstCitas_SelectionChanged;
        }

        private void LoadList()
        {
            if (_appointments.Count == 0)
            {
                TxtNoCitas.Visibility = Visibility.Visible;
                LstCitas.Visibility = Visibility.Collapsed;
            }
            else
            {
                TxtNoCitas.Visibility = Visibility.Collapsed;
                LstCitas.Visibility = Visibility.Visible;
                LstCitas.ItemsSource = _appointments;
            }
        }

        private void LstCitas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCitas.SelectedItem is CitaViewModel selectedCita)
            {
                ShowDetail(selectedCita);
                LstCitas.SelectedItem = null; // Reset selection
            }
        }

        private void ShowDetail(CitaViewModel cita)
        {
            DetailHora.Text = cita.HoraStr;
            DetailPaciente.Text = cita.NombrePaciente;
            DetailTratamiento.Text = cita.Tratamiento;
            DetailEstatus.Text = cita.Estatus; // We might want to color code this in code-behind too
            DetailDentista.Text = cita.DentistaNombre ?? "No asignado"; // Added DentistaNombre to ViewModel in RecepcionistView if needed, else ignore

            ListViewGrid.Visibility = Visibility.Collapsed;
            DetailViewGrid.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            DetailViewGrid.Visibility = Visibility.Collapsed;
            ListViewGrid.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Collapsed;
        }
    }
}
