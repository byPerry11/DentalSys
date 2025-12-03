using System;
using System.Windows;
using ApplicationLogic.DTOs;
using ApplicationLogic.Services;

namespace Presentacion.Views
{
    public partial class TratamientoFormView : Window
    {
        private TratamientoDTO _tratamiento;
        private bool _isEditMode;

        public TratamientoFormView(TratamientoDTO tratamiento = null)
        {
            InitializeComponent();
            _tratamiento = tratamiento;
            _isEditMode = tratamiento != null;

            if (_isEditMode)
            {
                TxtNombre.Text = _tratamiento.Nombre;
                TxtPrecio.Text = _tratamiento.Precio.ToString();
                TxtDescripcion.Text = _tratamiento.Descripcion;
                Title = "Editar Tratamiento";
            }
            else
            {
                Title = "Nuevo Tratamiento";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtPrecio.Text))
                {
                    MessageBox.Show("Por favor complete los campos obligatorios.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(TxtPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un número válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var service = new TratamientoService();

                if (_isEditMode)
                {
                    _tratamiento.Nombre = TxtNombre.Text;
                    _tratamiento.Precio = precio;
                    _tratamiento.Descripcion = TxtDescripcion.Text;

                    service.UpdateTratamiento(_tratamiento);
                    MessageBox.Show("Tratamiento actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var newTratamiento = new TratamientoDTO
                    {
                        Nombre = TxtNombre.Text,
                        Precio = precio,
                        Descripcion = TxtDescripcion.Text
                    };

                    service.AddTratamiento(newTratamiento);
                    MessageBox.Show("Tratamiento creado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
