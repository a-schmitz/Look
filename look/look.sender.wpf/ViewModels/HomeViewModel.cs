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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Windows;

    using DynamicData;

    using look.communication;
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

        private string quickAddInput;

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
        ///     Initializes a new instance of the <see cref="HomeViewModel" /> class.
        /// </summary>
        /// <param name="screen">
        ///     The screen context
        /// </param>
        public HomeViewModel(IMainViewModel screen) {
            
            Application.Current.Exit += this.Current_Exit;

            this.HostScreen = screen;

            // TODO: add DI
            this.WindowService = new WindowService();
            
            this.WhenNavigatedTo(this.InitViewModel);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// List of discovered hosts. Property is just a proxy for the real implementation in MainWindow 
        /// </summary>
        public IReactiveList<common.Model.RemoteHost> DiscoveredHosts { get { return ((IMainViewModel)this.HostScreen).DiscoveredHosts; } }

        /// <summary>
        /// Discovery Command object
        /// </summary>
        public ReactiveCommand<IEnumerable<common.Model.RemoteHost>> DiscoveryCommand { get; protected set; }

        /// <summary>
        /// Delete favorite Command object
        /// </summary>
        public ReactiveCommand<object> FavoriteDeleteCommand { get; protected set; }

        /// <summary>
        ///     Reference to parent screen object (MainWindow)
        /// </summary>
        public IScreen HostScreen { get; private set; }

        /// <summary>
        /// Indicator that discovery is running. Property is just a proxy for the real implementation in MainWindow 
        /// </summary>
        public bool IsDiscoveryLoading {
            get { return ((IMainViewModel)this.HostScreen).IsDiscoveryLoading; }
            set { ((IMainViewModel)this.HostScreen).IsDiscoveryLoading = value; }
        }

        /// <summary>
        /// Quick Add Command object
        /// </summary>
        public ReactiveCommand<object> QuickAddCommand { get; protected set; }

        /// <summary>
        /// Text of QuickAdd Inputbox
        /// </summary>
        public string QuickAddInput { get { return this.quickAddInput; } set { this.RaiseAndSetIfChanged(ref this.quickAddInput, value); } }

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
        ///     create new remotehost instance
        /// </summary>
        /// <param name="name">Hostname</param>
        /// <param name="ip">IPAddress</param>
        /// <returns></returns>
        private RemoteHost CreateRemoteHost(string name, string ip) {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(ip))
                throw new ArgumentException("Please enter either IPAddress or Hostname!");

            // create new remotehost
            var rh = new RemoteHost { IpAddress = ip, Name = name };

            // add some information if missing
            var ns = new NetworkServices();
            if (string.IsNullOrEmpty(name))
                rh.Name = ns.GetHostnameByIp(ip);
            else if (string.IsNullOrEmpty(ip)) // in case of quickadd, any input will be provided in name attribute, so we have to check for ip or name
                if (ns.IsValidIpAddress(name)) {
                    rh.IpAddress = name;
                    rh.Name = ns.GetHostnameByIp(rh.IpAddress);
                } else
                    rh.IpAddress = ns.GetIpByHostname(name);

            rh.Name = rh.Name.ToUpper();

            // add first tab for "my shares"
            rh.Tabs.Add(new SharedWindowsViewModel(this.HostScreen, rh, this.ShareableWindows.AsObservableCache(), "My Shares"));

            return rh;
        }

        /// <summary>
        ///     persist favorites when application is shutdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Current_Exit(object sender, ExitEventArgs e) {
            var favService = new FavoritesService();
            favService.UpdateFavorites(this.RemoteHosts.Select(x => new Favorite() { Ip = x.IpAddress, Name = x.Name }));
        }

        /// <summary>
        ///     Init command objects
        /// </summary>
        private void InitCommands() {
            // init command for adding new favorites from quickadd box
            this.QuickAddCommand = ReactiveCommand.Create(this.WhenAny(x => x.QuickAddInput, x => !string.IsNullOrEmpty(x.Value)));
            this.QuickAddCommand.Subscribe(
                _ => {
                    this.RemoteHosts.Add(this.CreateRemoteHost(this.QuickAddInput, null));
                    this.QuickAddInput = string.Empty;
                });

            // init command for deleting favorites
            this.FavoriteDeleteCommand = ReactiveCommand.Create(this.WhenAny(x => x.SelectedRemoteHost, x => x != null));
            this.FavoriteDeleteCommand.Subscribe(_ => this.RemoteHosts.Remove(this.SelectedRemoteHost));

            // init command for starting client discovery
            this.DiscoveryCommand = ReactiveCommand.CreateAsyncTask(
                x => {
                    this.IsDiscoveryLoading = true;
                    return RemoteContext.Instance.FindClientsAsync();
                });
            this.DiscoveryCommand.Subscribe(
                x => {
                    this.IsDiscoveryLoading = false;
                    this.DiscoveredHosts.AddRange(x);
                });
        }

        /// <summary>
        ///     Initialize our ViewModel
        /// </summary>
        /// <returns>
        ///     The <see cref="IDisposable" />.
        /// </returns>
        private IDisposable InitViewModel() {
            // init command objects
            this.InitCommands();

            // init collection that contains our own shareable windows; content is created by refresh Task afterwards
            this.ShareableWindows = new SourceCache<ShareableWindow, IntPtr>(w => w.Handle);

            // get favorites and init list of RemoteHosts
            var favService = new FavoritesService();
            var favs = favService.GetFavorites();
            this.RemoteHosts = new ReactiveList<RemoteHost>(favs.Select(x => this.CreateRemoteHost(x.Name, x.Ip)));

            // start refresh timer for collecting shareable windows
            var timer = TaskPoolScheduler.Default.SchedulePeriodic(TimeSpan.FromSeconds(1), this.RefreshShareableWindows);

            return new CompositeDisposable(timer, this.ShareableWindows);
        }

        /// <summary>
        ///     The refresh shareable windows.
        /// </summary>
        private void RefreshShareableWindows() {
            var windows = this.WindowService.GetShareableWindows();
            this.ShareableWindows.AddOrUpdate(windows.Where(ws => this.ShareableWindows.Items.All(s => s.Handle != ws.Handle)));
            this.ShareableWindows.RemoveKeys(this.ShareableWindows.Items.Where(ws => windows.All(w => w.Handle != ws.Handle)).Select(x => x.Handle));
        }

        #endregion
    }
} ;