using System;
using System.Windows;
using ApplicationLogic.DTOs;
using ApplicationLogic.Services;

namespace Presentacion.Views
{
    public partial class UserFormView : Window
    {
        private readonly UserService _userService;
        private readonly UserDTO? _userToEdit;

        public UserFormView(UserDTO? userToEdit = null)
        {
            InitializeComponent();
            _userService = new UserService();
            _userToEdit = userToEdit;

            if (_userToEdit != null)
            {
                Title = "Editar Usuario";
                TxtUsername.Text = _userToEdit.Username;
                CmbRole.Text = _userToEdit.Role;
                // Password is left empty for security, only update if entered
            }
            else
            {
                Title = "Nuevo Usuario";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = TxtUsername.Text;
                string password = TxtPassword.Password;
                string role = CmbRole.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(role))
                {
                    MessageBox.Show("Por favor complete todos los campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_userToEdit == null) // Create
                {
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("La contraseña es obligatoria para nuevos usuarios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newUser = new UserDTO { Username = username, Role = role };
                    _userService.CreateUser(newUser, password);
                }
                else // Edit
                {
                    _userToEdit.Username = username;
                    _userToEdit.Role = role;
                    _userService.UpdateUser(_userToEdit, string.IsNullOrWhiteSpace(password) ? null : password);
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
