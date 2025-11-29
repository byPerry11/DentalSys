using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using System.ComponentModel; //INotifyPropertyChanged
using System.Configuration; // App.config
using System.Windows;
using System.Windows.Input;

namespace Presentacion.ViewModels
{
    
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;

       
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        // Comando para ejecutar el inicio de sesión
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Evento que se dispara cuando una propiedad cambia
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

       
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    
        public LoginViewModel()
        {
            // AuthService usa ConnectionFactory 
            _authService = new AuthService();
            LoginCommand = new RelayCommand<object>(ExecuteLogin);
        }

        
        private void ExecuteLogin(object parameter)
        {
            // Validar que el username no esté vacío
            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show("Por favor ingrese un nombre de usuario.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Obtener la contraseña del PasswordBox
            var passwordBox = parameter as System.Windows.Controls.PasswordBox;
            var password = passwordBox?.Password;

            // Validar que la contraseña no esté vacía
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor ingrese una contraseña.", "Validación",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Intentar autenticar al usuario
            var user = _authService.Login(Username, password);

            if (user != null && !string.IsNullOrEmpty(user.Role))
            {
                AbrirDashboard(user.Role);
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error de autenticación",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void AbrirDashboard(string role)
        {
            // Instanciar MainWindow
            var main = new Views.MainWindow();

            // Navegar a la vista correspondiente según el rol
            switch (role?.ToLower())
            {
                case "administrador":
                    main.NavigateTo("AdminView");
                    break;
                case "dentista":
                    main.NavigateTo("DentistView");
                    break;
                case "recepcionista":
                    main.NavigateTo("ReceptionistView");
                    break;
                default:
                    MessageBox.Show($"Rol no reconocido: {role}\n\nRoles válidos: administrador, dentista, recepcionista",
                        "Error de Rol", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            main.Show();

            // Cerrar ventana de Login actual
            Application.Current.Windows[0]?.Close();
        }
    }
}
