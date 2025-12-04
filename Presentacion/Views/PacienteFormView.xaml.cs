using System;
using System.Windows;
using System.Windows.Controls;
using ApplicationLogic.DTOs;

namespace Presentacion.Views
{
    public partial class PacienteFormView : UserControl
    {
        public event Action<PacienteDTO> OnSave;
        public event Action OnCancel;

        private PacienteDTO _currentPaciente;

        public PacienteFormView(PacienteDTO paciente = null)
        {
            InitializeComponent();
            _currentPaciente = paciente;

            if (_currentPaciente != null)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            TxtNombre.Text = _currentPaciente.Nombre;
            TxtTelefono.Text = _currentPaciente.Telefono;
            TxtContactoEmergencia.Text = _currentPaciente.ContactoEmergencia;
            TxtNumeroEmergencia.Text = _currentPaciente.NumeroEmergencia;
            TxtEmail.Text = _currentPaciente.Email;
            DpFechaNacimiento.SelectedDate = _currentPaciente.FechaNacimiento;
            CmbFacturada.Text = _currentPaciente.Facturada;
            TxtTasaIVA.Text = _currentPaciente.TasaIVA.ToString();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_currentPaciente == null)
            {
                _currentPaciente = new PacienteDTO();
            }

            _currentPaciente.Nombre = TxtNombre.Text;
            _currentPaciente.Telefono = TxtTelefono.Text;
            _currentPaciente.ContactoEmergencia = TxtContactoEmergencia.Text;
            _currentPaciente.NumeroEmergencia = TxtNumeroEmergencia.Text;
            _currentPaciente.Email = TxtEmail.Text;
            _currentPaciente.FechaNacimiento = DpFechaNacimiento.SelectedDate ?? DateTime.Now;
            _currentPaciente.Facturada = CmbFacturada.Text;

            if (decimal.TryParse(TxtTasaIVA.Text, out decimal tasaIVA))
            {
                _currentPaciente.TasaIVA = tasaIVA;
            }
            else
            {
                _currentPaciente.TasaIVA = 0;
            }

            OnSave?.Invoke(_currentPaciente);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }
    }
}
