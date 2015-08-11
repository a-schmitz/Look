// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Favorite.cs" company="">
//   
// </copyright>
// <summary>
//   The favorite.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Models {

    /// <summary>
    ///     The favorite.
    /// </summary>
    public class Favorite {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether connected.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        ///     Gets or sets the ip address.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }

}