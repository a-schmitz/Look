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
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The shared windows view model.
    /// </summary>
    public class SharedWindowsViewModel : ReactiveObject, ISharedWindowsViewModel
    {
        #region Fields

        /// <summary>
        ///     The header.
        /// </summary>
        private string header;

        /// <summary>
        ///     The shareable windows.
        /// </summary>
        private ReactiveList<ShareableWindow> shareableWindows;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedWindowsViewModel"/> class.
        ///     Initializes a new instance of the <see cref="RemoteViewerViewModel"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        /// <param name="favorite">
        /// The favorite.
        /// </param>
        /// <param name="shareableWindows">
        /// The shareable Windows.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        public SharedWindowsViewModel(IScreen screen, Favorite favorite, ReactiveList<ShareableWindow> shareableWindows, string header) {
            this.HostScreen = screen;
            this.Favorite = favorite;
            this.ShareableWindows = shareableWindows;
            this.Header = header;

            this.ShareableWindows.ItemChanged.Where(x => x.PropertyName == "IsShared").Select(x => x.Sender).Subscribe(
                x => {this.RaisePropertyChanged("Header"); });

        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the favorite.
        /// </summary>
        public Favorite Favorite { get; set; }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        public string Header {
            get { return string.Format("{0} ({1})", this.header, this.ShareableWindows.Where(s => s.IsShared).Count()); }
            set { this.RaiseAndSetIfChanged(ref this.header, value); }
        }

        /// <summary>
        ///     Gets or sets the host screen.
        /// </summary>
        public IScreen HostScreen { get; set; }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        public ReactiveList<ShareableWindow> ShareableWindows {
            get { return this.shareableWindows; }
            set { this.RaiseAndSetIfChanged(ref this.shareableWindows, value); }
        }

        /// <summary>
        ///     Gets the url path segment.
        /// </summary>
        public string UrlPathSegment { get { return "sharedwindows"; } }

        #endregion
    }

}