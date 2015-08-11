﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The main view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.ViewModels
{
    #region

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows.Threading;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;
    using look.sender.wpf.Services;

    using ReactiveUI;

    #endregion

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
        /// The refresh timer.
        /// </summary>
        private DispatcherTimer refreshTimer;

        /// <summary>
        ///     The selected favorite.
        /// </summary>
        private Favorite selectedFavorite;

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
        public HomeViewModel(IScreen screen) {
            this.HostScreen = screen;

            // TODO: add DI
            this.WindowService = new WindowService();

            this.WhenNavigatedTo(this.InitViewModel);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the favorites.
        /// </summary>
        public ReactiveList<Favorite> Favorites { get { return this.favorites; } set { this.RaiseAndSetIfChanged(ref this.favorites, value); } }

        /// <summary>
        ///     Gets the host screen.
        /// </summary>
        public IScreen HostScreen { get; private set; }

        /// <summary>
        ///     Gets or sets the selected favorite.
        /// </summary>
        public Favorite SelectedFavorite {
            get { return this.selectedFavorite; }
            set { this.RaiseAndSetIfChanged(ref this.selectedFavorite, value); }
        }

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
        public string UrlPathSegment { get { return "home"; } }

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
        private IDisposable InitViewModel() {
            this.ShareableWindows = new ReactiveList<ShareableWindow>() { ChangeTrackingEnabled = true };

            // TODO: loading Favorites from file
            this.Favorites = new ReactiveList<Favorite> {
                new Favorite() { Name = "HAN07WST12345", IPAddress = "192.168.150.1" }, 
                new Favorite() { Name = "HAN07WST54321", IPAddress = "192.168.150.10" }, 
                new Favorite() { Name = "HAN07WST23321", IPAddress = "192.168.150.100" }
            };
            this.SelectedFavorite = this.Favorites[0];

            // when selected favorite changes, set respective SharedWindows flags
            this.WhenAny(x => x.SelectedFavorite, x => x.Value).Subscribe(
                x => {
                    foreach (var window in this.ShareableWindows)
                        window.IsShared = window.Favorites.Contains(x);
                });

            // when a new window is shared, add currently selected favorite reference to SharedWindow instance
            this.ShareableWindows.ItemChanged.Where(x => x.PropertyName == "IsShared" && x.Sender.IsShared).Select(x => x.Sender).Subscribe(
                x => x.Favorites.Add(this.SelectedFavorite));

            // when a window is no longer shared, remove favorite reference from SharedWindow instance
            this.ShareableWindows.ItemChanged.Where(x => x.PropertyName == "IsShared" && !x.Sender.IsShared).Select(x => x.Sender).Subscribe(
                x => x.Favorites.Remove(this.SelectedFavorite));

            this.refreshTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            this.refreshTimer.Tick += this.refreshTimer_Tick;
            this.refreshTimer.Start();

            return Disposable.Create(this.Shutdown);
        }

        /// <summary>
        /// The refresh shareable windows.
        /// </summary>
        private void RefreshShareableWindows() {
            // using (this.ShareableWindows.SuppressChangeNotifications()) {
            Debug.WriteLine("Refreshing ------------- ");
            var windows = this.WindowService.GetShareableWindows();
            foreach (var w in windows.Where(ws => this.ShareableWindows.All(s => s.Handle != ws.Handle))) {
                this.ShareableWindows.Add(w);
                Debug.WriteLine("Added " + w.Title);
            }

            var listDelete = this.ShareableWindows.Where(ws => windows.All(w => w.Handle != ws.Handle)).ToList();
            listDelete.ForEach(
                ld => {
                    this.ShareableWindows.Remove(ld);
                    Debug.WriteLine("Removed " + ld.Title);
                });
            // }
        }

        /// <summary>
        ///     The shutdown.
        /// </summary>
        private void Shutdown() {
            this.refreshTimer.Stop();
            Debug.WriteLine("Shutdown.");
        }

        /// <summary>
        /// The refresh timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void refreshTimer_Tick(object sender, EventArgs e) {
            this.refreshTimer.Stop();

            this.RefreshShareableWindows();

            this.refreshTimer.Start();
        }

        #endregion
    }

}