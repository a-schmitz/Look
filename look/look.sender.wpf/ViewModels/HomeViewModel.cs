// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The main view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Disposables;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;
    using look.sender.wpf.Services;

    using ReactiveUI;

    /// <summary>
    ///     The main view model.
    /// </summary>
    public class HomeViewModel : ReactiveObject, IHomeViewModel
    {
        #region Fields

        /// <summary>
        ///     The favorites.
        /// </summary>
        private ReactiveList<Favorite> favorites;

        /// <summary>
        ///     The shareable windows.
        /// </summary>
        private ReactiveList<ShareableWindow> shareableWindows;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="screen">
        /// The screen.
        /// </param>
        public HomeViewModel(IScreen screen)
        {
            this.HostScreen = screen;

            // TODO: add DI
            this.WindowService = new WindowService();

            this.ShareableWindows = new ReactiveList<ShareableWindow>(this.WindowService.GetShareableWindows());

            this.Favorites = new ReactiveList<Favorite>
                                 {
                                     new Favorite() { Name = "HAN07WST12345" }, 
                                     new Favorite() { Name = "HAN07WST54321" }, 
                                     new Favorite() { Name = "HAN07WST23321" }
                                 };

            this.WhenNavigatedTo(this.InitViewModel);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the favorites.
        /// </summary>
        public ReactiveList<Favorite> Favorites
        {
            get
            {
                return this.favorites;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.favorites, value);
            }
        }

        /// <summary>
        ///     Gets the host screen.
        /// </summary>
        public IScreen HostScreen { get; private set; }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        public ReactiveList<ShareableWindow> ShareableWindows
        {
            get
            {
                return this.shareableWindows;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref this.shareableWindows, value);
            }
        }

        /// <summary>
        ///     Gets the url path segment.
        /// </summary>
        public string UrlPathSegment
        {
            get
            {
                return "home";
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the window service.
        /// </summary>
        private IWindowService WindowService { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The init view model.
        /// </summary>
        /// <returns>
        ///     The <see cref="IDisposable" />.
        /// </returns>
        private IDisposable InitViewModel()
        {
            return Disposable.Create(this.Shutdown);
        }

        /// <summary>
        ///     The shutdown.
        /// </summary>
        private void Shutdown()
        {
            Debug.WriteLine("Shutdown.");
        }

        #endregion
    }
}