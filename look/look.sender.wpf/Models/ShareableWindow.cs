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
    #region

    using System;
    using System.Windows.Media;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The shareable window.
    /// </summary>
    public class ShareableWindow : ReactiveObject
    {
        #region Fields

        /// <summary>
        ///     The is shared.
        /// </summary>
        private bool isShared;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShareableWindow" /> class.
        /// </summary>
        public ShareableWindow() { this.Favorites = new ReactiveList<Favorite>(); }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        ///     Gets or sets the favorites.
        /// </summary>
        public ReactiveList<Favorite> Favorites { get; set; }

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
        ///     Gets or sets a value indicating whether is shared.
        /// </summary>
        public bool IsShared { get { return this.isShared; } set { this.RaiseAndSetIfChanged(ref this.isShared, value); } }

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