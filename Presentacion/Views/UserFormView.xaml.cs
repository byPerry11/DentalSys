using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using System;
using System.Windows;
using System.Windows.Controls;

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

            CmbRole.Items.Clear();
            CmbRole.Items.Add(new ComboBoxItem { Content = "Dentista" });
            CmbRole.Items.Add(new ComboBoxItem { Content = "Recepcionista" });
            CmbRole.Items.Add(new ComboBoxItem { Content = "Administrador" });

            if (_userToEdit != null)
            {
                Title = "Editar Usuario";
                TxtUsername.Text = _userToEdit.Username;

                // 🔹 Preseleccionar correctamente el rol actual
                var item = CmbRole.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content.ToString()
                    .Equals(_userToEdit.Role, StringComparison.OrdinalIgnoreCase));

                CmbRole.SelectedItem = item;

                // 🔹 Ocultar perfiles porque en edición NO se asignan nuevos
                CmbProfiles.Visibility = Visibility.Collapsed;
                LblProfile.Visibility = Visibility.Collapsed;
            }
            else
            {
                Title = "Nuevo Usuario";

                // 🔹 También ocultar perfiles hasta que el usuario elija un rol
                CmbProfiles.Visibility = Visibility.Collapsed;
                LblProfile.Visibility = Visibility.Collapsed;
            }
        }

        private void CmbRole_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var role = (CmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString()?.ToLower().Trim();
            if (string.IsNullOrEmpty(role))
            {
                CmbProfiles.Visibility = Visibility.Collapsed;
                LblProfile.Visibility = Visibility.Collapsed;
                return;
            }

            // Mostrar y cargar perfiles disponibles según rol
            switch (role)
            {
                case "dentista":
                    var dentistas = _userService.GetDentistasDisponibles();
                    if (dentistas == null || dentistas.Count == 0)
                    {
                        MessageBox.Show("No hay dentistas disponibles para asignar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        CmbProfiles.ItemsSource = null;
                        CmbProfiles.Visibility = Visibility.Collapsed;
                        LblProfile.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        // Proyectamos a objetos simples para binding
                        var items = dentistas.Select(d => new { Id = d.Id_Dentista, Nombre = $"{d.Nombre} ({d.Email ?? d.Telefono})" }).ToList();
                        CmbProfiles.ItemsSource = items;
                        CmbProfiles.DisplayMemberPath = "Nombre";
                        CmbProfiles.SelectedValuePath = "Id";
                        CmbProfiles.Visibility = Visibility.Visible;
                        LblProfile.Visibility = Visibility.Visible;
                    }
                    break;

                case "recepcionista":
                    var receps = _userService.GetRecepcionistasDisponibles();
                    if (receps == null || receps.Count == 0)
                    {
                        MessageBox.Show("No hay recepcionistas disponibles para asignar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        CmbProfiles.ItemsSource = null;
                        CmbProfiles.Visibility = Visibility.Collapsed;
                        LblProfile.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        var items = receps.Select(r => new { Id = r.Id_Recepcionista, Nombre = $"{r.Nombre} ({r.Email ?? r.Telefono})" }).ToList();
                        CmbProfiles.ItemsSource = items;
                        CmbProfiles.DisplayMemberPath = "Nombre";
                        CmbProfiles.SelectedValuePath = "Id";
                        CmbProfiles.Visibility = Visibility.Visible;
                        LblProfile.Visibility = Visibility.Visible;
                    }
                    break;

                case "administrador":
                    var admins = _userService.GetAdministradoresDisponibles();
                    if (admins == null || admins.Count == 0)
                    {
                        MessageBox.Show("No hay administradores disponibles para asignar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        CmbProfiles.ItemsSource = null;
                        CmbProfiles.Visibility = Visibility.Collapsed;
                        LblProfile.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        var items = admins.Select(a => new { Id = a.Id_Administrador, Nombre = $"{a.Nombre} ({a.Email ?? a.Telefono})" }).ToList();
                        CmbProfiles.ItemsSource = items;
                        CmbProfiles.DisplayMemberPath = "Nombre";
                        CmbProfiles.SelectedValuePath = "Id";
                        CmbProfiles.Visibility = Visibility.Visible;
                        LblProfile.Visibility = Visibility.Visible;
                    }
                    break;

                default:
                    CmbProfiles.ItemsSource = null;
                    CmbProfiles.Visibility = Visibility.Collapsed;
                    LblProfile.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = TxtUsername.Text?.Trim();
                string password = TxtPassword.Password;
                string role = (CmbRole.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString();

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

                    var normalizedRole = role.ToLower().Trim();
                    // Si el rol requiere seleccionar perfil, validar que esté seleccionado
                    if (normalizedRole == "dentista")
                    {
                        if (CmbProfiles.SelectedValue == null)
                        {
                            MessageBox.Show("Debes seleccionar un dentista disponible para asignarle el usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        newUser.Id_Dentista = Convert.ToInt32(CmbProfiles.SelectedValue);
                    }
                    else if (normalizedRole == "recepcionista")
                    {
                        if (CmbProfiles.SelectedValue == null)
                        {
                            MessageBox.Show("Debes seleccionar una recepcionista disponible para asignarle el usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        newUser.Id_Recepcionista = Convert.ToInt32(CmbProfiles.SelectedValue);
                    }
                    else if (normalizedRole == "administrador")
                    {
                        if (CmbProfiles.SelectedValue == null)
                        {
                            MessageBox.Show("Debes seleccionar un administrador disponible para asignarle el usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        newUser.Id_Administrador = Convert.ToInt32(CmbProfiles.SelectedValue);
                    }

                    // Crear y asignar (CreateUser lanza excepción si algo falla)
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
