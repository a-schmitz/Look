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
    #region

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Windows.Threading;

    using DynamicData;

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
        /// The cleanup.
        /// </summary>
        private IDisposable cleanup;

        /// <summary>
        ///     The refresh timer.
        /// </summary>
        private DispatcherTimer refreshTimer;

        /// <summary>
        ///     The favorites.
        /// </summary>
        private ReactiveList<RemoteHost> remoteHosts;

        /// <summary>
        ///     The selected favorite.
        /// </summary>
        private RemoteHost selectedRemoteHost;

        /// <summary>
        ///     The shareable windows.
        /// </summary>
        private ISourceCache<ShareableWindow, IntPtr> shareableWindows;

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
        ///     Gets the host screen.
        /// </summary>
        public IScreen HostScreen { get; private set; }

        /// <summary>
        ///     Gets or sets the favorites.
        /// </summary>
        public ReactiveList<RemoteHost> RemoteHosts {
            get { return this.remoteHosts; }
            set { this.RaiseAndSetIfChanged(ref this.remoteHosts, value); }
        }

        /// <summary>
        ///     Gets or sets the selected favorite.
        /// </summary>
        public RemoteHost SelectedRemoteHost {
            get { return this.selectedRemoteHost; }
            set { this.RaiseAndSetIfChanged(ref this.selectedRemoteHost, value); }
        }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        public ISourceCache<ShareableWindow, IntPtr> ShareableWindows {
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
            // this.ShareableWindows = new ReactiveList<ShareableWindow>() { ChangeTrackingEnabled = true };

            this.ShareableWindows = new SourceCache<ShareableWindow, IntPtr>(w => w.Handle);

            // TODO: loading Favorites from file
            this.RemoteHosts = new ReactiveList<RemoteHost> {
                new RemoteHost() { Name = "HAN07WST12345", IpAddress = "192.168.150.1" }, 
                new RemoteHost() { Name = "HAN07WST54321", IpAddress = "192.168.150.10" }, 
                new RemoteHost() { Name = "HAN07WST23321", IpAddress = "192.168.150.100" }
            };

            foreach (var f in this.RemoteHosts)
                f.Tabs.Add(new SharedWindowsViewModel(this.HostScreen, this.RemoteHosts[0], this.ShareableWindows.AsObservableCache(), "My Shares"));

            //var f2 = new FavoritesService();
            //f2.UpdateFavorites(this.RemoteHosts.Select(x => new Favorite() { Ip = x.IpAddress, Name = x.Name }));

            // this.RemoteHosts[0].Tabs.Add(new RemoteViewerViewModel(this.HostScreen, this.RemoteHosts[0], "Test1"));
            // this.RemoteHosts[0].Tabs.Add(new RemoteViewerViewModel(this.HostScreen, this.RemoteHosts[0], "Test2"));

            // start refresh timer for collecting shareable windows
            this.refreshTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            this.refreshTimer.Tick += this.refreshTimer_Tick;
            this.refreshTimer.Start();

            // select first host and select first tab
            this.SelectedRemoteHost = this.RemoteHosts[0];
            this.SelectedRemoteHost.SelectedViewModel = this.SelectedRemoteHost.Tabs[0];

            this.cleanup = new CompositeDisposable(this.ShareableWindows);

            return Disposable.Create(this.Shutdown);
        }

        /// <summary>
        ///     The refresh shareable windows.
        /// </summary>
        private void RefreshShareableWindows() {
            var windows = this.WindowService.GetShareableWindows();
            this.ShareableWindows.AddOrUpdate(windows.Where(ws => this.ShareableWindows.Items.All(s => s.Handle != ws.Handle)));
            this.ShareableWindows.RemoveKeys(this.ShareableWindows.Items.Where(ws => windows.All(w => w.Handle != ws.Handle)).Select(x => x.Handle));
        }

        /// <summary>
        ///     The shutdown.
        /// </summary>
        private void Shutdown() {
            this.refreshTimer.Stop();

            this.cleanup.Dispose();

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