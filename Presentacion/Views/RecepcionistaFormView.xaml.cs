using System;
using System.Windows;
using System.Windows.Controls;
using ApplicationLogic.DTOs;

namespace Presentacion.Views
{
    public partial class RecepcionistaFormView : UserControl
    {
        public event Action<RecepcionistaDTO> OnSave;
        public event Action OnCancel;

        private RecepcionistaDTO _current;

        public RecepcionistaFormView(RecepcionistaDTO recep = null)
        {
            InitializeComponent();
            _current = recep;
            if (_current != null) LoadData();
        }

        private void LoadData()
        {
            TxtNombre.Text = _current.Nombre;
            TxtTelefono.Text = _current.Telefono;
            TxtEmail.Text = _current.Email;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTelefono.Text))
            {
                MessageBox.Show("El teléfono es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("El email es obligatorio.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_current == null) _current = new RecepcionistaDTO();

            _current.Nombre = TxtNombre.Text;
            _current.Telefono = TxtTelefono.Text;
            _current.Email = TxtEmail.Text;

            OnSave?.Invoke(_current);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }
    }
}