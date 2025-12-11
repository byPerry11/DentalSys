using System;
using System.Windows;
using System.Windows.Controls;
using ApplicationLogic.DTOs;

namespace Presentacion.Views
{
    public partial class DentistaFormView : UserControl
    {
        // Eventos para notificar a la capa que usa el formulario
        public event Action<DentistaDTO> OnSave;
        public event Action OnCancel;

        private DentistaDTO _currentDentista;

        public DentistaFormView(DentistaDTO dentista = null)
        {
            InitializeComponent();
            _currentDentista = dentista;

            if (_currentDentista != null)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            // Carga valores existentes en el formulario
            TxtNombre.Text = _currentDentista.Nombre;
            TxtTelefono.Text = _currentDentista.Telefono;
            TxtEspecialidad.Text = _currentDentista.Especialidad;
            TxtEmail.Text = _currentDentista.Email;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validacion basica
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTelefono.Text))
            {
                MessageBox.Show("El telefono es obligatorio.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("El email es obligatorio.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_currentDentista == null)
            {
                _currentDentista = new DentistaDTO();
            }

            // Asignar valores desde el formulario al DTO
            _currentDentista.Nombre = TxtNombre.Text;
            _currentDentista.Telefono = TxtTelefono.Text;
            _currentDentista.Especialidad = TxtEspecialidad.Text;
            _currentDentista.Email = TxtEmail.Text;

            OnSave?.Invoke(_currentDentista);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }
    }
}
