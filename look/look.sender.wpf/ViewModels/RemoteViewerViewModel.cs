// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteViewerViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The remote viewer view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.ViewModels
{
    #region

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The remote viewer view model.
    /// </summary>
    public class RemoteViewerViewModel : ReactiveObject, IRemoteViewerViewModel
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewerViewModel"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <param name="remoteHost">
        /// The favorite.
        /// </param>
        /// <param name="tabName">
        /// The tab name.
        /// </param>
        public RemoteViewerViewModel(IScreen screen, RemoteHost remoteHost, string tabName) {
            this.HostScreen = screen;
            this.RemoteHost = remoteHost;
            this.Header = tabName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        ///     Gets or sets the host screen.
        /// </summary>
        public IScreen HostScreen { get; set; }

        /// <summary>
        ///     Gets or sets the favorite.
        /// </summary>
        public RemoteHost RemoteHost { get; set; }

        /// <summary>
        ///     Gets the url path segment.
        /// </summary>
        public string UrlPathSegment { get { return "remoteviewer"; } }

        #endregion
    }

}