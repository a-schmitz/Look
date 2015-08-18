// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteHostShareableWindow.cs" company="">
//   
// </copyright>
// <summary>
//   The shared window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Models
{
    #region

    using System;
    using System.Windows.Media;

    using DynamicData.Binding;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The shared window.
    /// </summary>
    public class RemoteHostShareableWindow : ReactiveObject
    {
        #region Fields

        /// <summary>
        ///     The is shared.
        /// </summary>
        private bool isShared;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteHostShareableWindow"/> class.
        ///     Initializes a new instance of the <see cref="SharedWindow"/> class.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        public RemoteHostShareableWindow(ShareableWindow window) { this.Window = window; }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        public string Application { get { return this.Window.Application; } }

        /// <summary>
        ///     Gets or sets the handle.
        /// </summary>
        public IntPtr Handle { get { return this.Window.Handle; } }

        /// <summary>
        ///     Gets or sets the icon.
        /// </summary>
        public ImageSource Icon { get { return this.Window.Icon; } }

        /// <summary>
        ///     Gets or sets a value indicating whether is shared.
        /// </summary>
        public bool IsShared { get { return this.isShared; } set { this.RaiseAndSetIfChanged(ref this.isShared, value); } }

        /// <summary>
        ///     Gets or sets the process name.
        /// </summary>
        public string ProcessName { get { return this.Window.ProcessName; } }

        /// <summary>
        ///     Gets or sets the process path.
        /// </summary>
        public string ProcessPath { get { return this.Window.ProcessPath; } }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        public string Title { get { return this.Window.Title; } }

        /// <summary>
        ///     Gets or sets the window.
        /// </summary>
        public ShareableWindow Window { get; set; }

        #endregion
    }

}