// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedWindowsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The shared windows view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.ViewModels
{
    #region

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Linq;

    using DynamicData;
    using DynamicData.Binding;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The shared windows view model.
    /// </summary>
    public class SharedWindowsViewModel : ReactiveObject, ISharedWindowsViewModel, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The clean up.
        /// </summary>
        private readonly IDisposable cleanUp;

        /// <summary>
        ///     The shareable windows.
        /// </summary>
        private readonly ReadOnlyObservableCollection<RemoteHostShareableWindow> shareableWindows;

        /// <summary>
        ///     The header.
        /// </summary>
        private string header;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedWindowsViewModel"/> class.
        ///     Initializes a new instance of the <see cref="RemoteViewerViewModel"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <param name="remoteHost">
        /// The favorite.
        /// </param>
        /// <param name="shareableWindows">
        /// The shareable Windows.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        public SharedWindowsViewModel
            (IScreen screen, RemoteHost remoteHost, IObservableCache<ShareableWindow, IntPtr> shareableWindows, string header) {
            this.HostScreen = screen;
            this.RemoteHost = remoteHost;

            this.Header = header;

            // create proxy objects from source cache to have individual instances per RemoteHost
            this.cleanUp = shareableWindows.Connect().Transform(
                window => {
                    // observe IsShared property and update header if necessary
                    var w = new RemoteHostShareableWindow(window);
                    w.WhenPropertyChanged(shareableWindow => shareableWindow.IsShared).Subscribe(
                        shareableWindow => this.RaisePropertyChanged("Header"));
                    return w;
                }).ObserveOnDispatcher().Bind(out this.shareableWindows).Subscribe();

            // Update Header when Number of windows changed, maybe a shared window has been removed
            this.shareableWindows.WhenPropertyChanged(windows => windows.Count).Subscribe(windows => this.RaisePropertyChanged("Header"));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header {
            get { return string.Format("{0} ({1})", this.header, this.shareableWindows.Count(window => window.IsShared)); }
            set { this.RaiseAndSetIfChanged(ref this.header, value); }
        }

        /// <summary>
        ///     Gets or sets the host screen.
        /// </summary>
        public IScreen HostScreen { get; set; }

        /// <summary>
        ///     Gets or sets the favorite.
        /// </summary>
        public RemoteHost RemoteHost { get; set; }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        public ReadOnlyObservableCollection<RemoteHostShareableWindow> ShareableWindows { get { return this.shareableWindows; } }

        /// <summary>
        ///     Gets the url path segment.
        /// </summary>
        public string UrlPathSegment { get { return "sharedwindows"; } }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose() {
            this.cleanUp.Dispose();
        }

        #endregion
    }

}