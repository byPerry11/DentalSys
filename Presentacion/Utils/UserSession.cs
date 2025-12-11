using ApplicationLogic.DTOs;

namespace Presentacion.Utils
{
    /// <summary>
    /// Static class to store the current logged-in user session
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// Gets or sets the current logged-in user
        /// </summary>
        public static UserDTO? CurrentUser { get; set; }

        /// <summary>
        /// Clears the current user session
        /// </summary>
        public static void Clear()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Checks if there is a user currently logged in
        /// </summary>
        public static bool IsLoggedIn => CurrentUser != null;
    }
}
