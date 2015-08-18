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
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Windows;

    using DynamicData;

    using look.communication;
    using look.communication.Model;
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

            RemoteContext.Instance.HostConnected += Instance_HostConnected;
            RemoteContext.Instance.WindowsShared += Instance_WindowsShared;
            RemoteContext.Instance.WindowsRequested += Instance_WindowsRequested;
            RemoteContext.Instance.HostDisconnected += Instance_HostDisconnected;

        }

        void Instance_HostDisconnected(object sender, common.Events.HostDisconnectedEventArgs e)
        {
            Debug.WriteLine(string.Format("Client Disconnected: {0}", e.Ip));
        }

        void Instance_WindowsRequested(object sender, common.Events.WindowsRequestedEventArgs e)
        {
            Debug.WriteLine(string.Format("Windows Requested: {0}", string.Join("\r\n", e.Windows.Select(x => x.Name).ToList())));
        }

        void Instance_WindowsShared(object sender, common.Events.WindowsSharedEventArgs e)
        {
            Debug.WriteLine(string.Format("Windows shared: {0}", string.Join("\r\n", e.Windows.Select(x => x.Name).ToList()) ));
        }

        void Instance_HostConnected(object sender, common.Events.HostConnectedEventArgs e)
        {
            Debug.WriteLine(string.Format("Client connected: {0}", e.Ip));
            e.Accepted = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Delete favorite Command object
        /// </summary>
        public ReactiveCommand<object> FavoriteDeleteCommand { get; protected set; }

        public ReactiveCommand<bool> ConnectCommand { get; protected set; }

        /// <summary>
        ///     Reference to parent screen object (MainWindow)
        /// </summary>
        public IScreen HostScreen { get; private set; }

        public IMainViewModel MainViewModel { get { return ((IMainViewModel)this.HostScreen); } }

        /// <summary>
        ///     Quick Add Command object
        /// </summary>
        public ReactiveCommand<object> QuickAddCommand { get; protected set; }

        /// <summary>
        ///     Text of QuickAdd Inputbox
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

            if (this.RemoteHosts != null)
                favService.UpdateFavorites(this.RemoteHosts.Select(x => new Favorite() { Ip = x.IpAddress, Name = x.Name }));
        }

        /// <summary>
        ///     Init command objects
        /// </summary>
        private void InitCommands() {
            // init command for adding new favorites from quickadd box
            this.QuickAddCommand = ReactiveCommand.Create(this.WhenAny(viewmodel => viewmodel.QuickAddInput, x => !string.IsNullOrEmpty(x.Value)));
            this.QuickAddCommand.Subscribe(
                _ => {
                    this.RemoteHosts.Add(this.CreateRemoteHost(this.QuickAddInput, null));
                    this.QuickAddInput = string.Empty;
                });

            // init command for deleting favorites
            this.FavoriteDeleteCommand = ReactiveCommand.Create(this.WhenAny(viewmodel => viewmodel.SelectedRemoteHost, x => x != null));
            this.FavoriteDeleteCommand.Subscribe(_ => this.RemoteHosts.Remove(this.SelectedRemoteHost));

            // subscribe to addhostcommand from mainviewmodel for adding new hosts from discovery
            this.MainViewModel.AddHostCommand.Subscribe(
                x => {
                    var rh = x as look.common.Model.RemoteHost;
                    if (rh == null)
                        return;
                    this.RemoteHosts.Add(this.CreateRemoteHost(rh.Name, rh.Ip));
                });

            // init command for triggering connect to remotehost
            this.ConnectCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAny(viewmodel => viewmodel.SelectedRemoteHost, x => x != null), param => RemoteContext.Instance.ConnectAsync(this.SelectedRemoteHost.Name));

            this.ConnectCommand.Subscribe(x => Debug.WriteLine("Conected: " + x));

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
            var timer = DispatcherScheduler.Current.SchedulePeriodic(TimeSpan.FromSeconds(1), this.RefreshShareableWindows);

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