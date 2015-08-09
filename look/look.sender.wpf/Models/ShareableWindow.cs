// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShareableWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The shareable window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Models
{
    using System;
    using System.Windows.Media;

    /// <summary>
    ///     The shareable window.
    /// </summary>
    public class ShareableWindow
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        ///     Gets or sets the handle.
        /// </summary>
        public IntPtr Handle { get; set; }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public ImageSource Icon { get; set; }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the process name.
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        ///     Gets or sets the process path.
        /// </summary>
        public string ProcessPath { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}