// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The app bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.ViewModels
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Windows;

    using look.common.Model;
    using look.communication;
    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Views;

    using ReactiveUI;

    using Splat;

    #endregion

    /// <summary>
    ///     The app bootstrapper.
    /// </summary>
    public class MainWindowViewModel : ReactiveObject, IMainViewModel
    {
        #region Fields

        /// <summary>
        ///     The discovered host.
        /// </summary>
        private RemoteHost discoveredHost;

        /// <summary>
        ///     The discovered hosts.
        /// </summary>
        private IReactiveList<RemoteHost> discoveredHosts = new ReactiveList<RemoteHost>();

        /// <summary>
        ///     The is discovery list empty.
        /// </summary>
        private ObservableAsPropertyHelper<bool> isDiscoveryListEmpty;

        /// <summary>
        ///     The is loading.
        /// </summary>
        private bool isDiscoveryLoading;

        /// <summary>
        ///     The is discovery visible.
        /// </summary>
        private bool isDiscoveryVisible;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
        /// <param name="testRouter">
        /// The test router.
        /// </param>
        public MainWindowViewModel(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null) {
            this.Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            RxApp.MainThreadScheduler = new DispatcherScheduler(Application.Current.Dispatcher);

            this.RegisterParts(dependencyResolver);

            LogHost.Default.Level = LogLevel.Debug;

            // start local instance so that we can be discovered by other clients
            RemoteContext.Instance.StartAcceptingConnections("MyHost");

            Application.Current.Exit += this.Current_Exit;
            
            // Init ViewModel and navigate to the opening page of the application
            this.InitViewModel();
            this.Router.Navigate.Execute(new HomeViewModel(this));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the add host command.
        /// </summary>
        public ReactiveCommand<object> AddHostCommand { get; protected set; }

        /// <summary>
        ///     Gets or sets the discovered host.
        /// </summary>
        public RemoteHost DiscoveredHost { get { return this.discoveredHost; } set { this.RaiseAndSetIfChanged(ref this.discoveredHost, value); } }

        /// <summary>
        ///     Gets or sets the discovered hosts.
        /// </summary>
        public IReactiveList<RemoteHost> DiscoveredHosts {
            get { return this.discoveredHosts; }
            set { this.RaiseAndSetIfChanged(ref this.discoveredHosts, value); }
        }

        /// <summary>
        ///     Gets or sets the discovery command.
        /// </summary>
        public ReactiveCommand<IEnumerable<RemoteHost>> DiscoveryCommand { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether is discovery list empty.
        /// </summary>
        public bool IsDiscoveryListEmpty { get { return this.isDiscoveryListEmpty.Value; } }

        /// <summary>
        ///     Gets or sets a value indicating whether is loading.
        /// </summary>
        public bool IsDiscoveryLoading {
            get { return this.isDiscoveryLoading; }
            set { this.RaiseAndSetIfChanged(ref this.isDiscoveryLoading, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is discovery visible.
        /// </summary>
        public bool IsDiscoveryVisible {
            get { return this.isDiscoveryVisible; }
            set { this.RaiseAndSetIfChanged(ref this.isDiscoveryVisible, value); }
        }

        /// <summary>
        ///     Gets the router.
        /// </summary>
        public RoutingState Router { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// When application is shutdown, shutdown server instance
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Current_Exit(object sender, ExitEventArgs e) {
            // stop local instance
            RemoteContext.Instance.StopAcceptingConnections();
        }

        /// <summary>
        ///     The init view model.
        /// </summary>
        private void InitViewModel() {
            // show flyout, whenever loading is set to true
            this.WhenAnyValue(x => x.IsDiscoveryLoading).Where(x => x).Subscribe(b => { this.IsDiscoveryVisible = true; });

            // when flyout is visible and loading is done but no hosts are found, show message
            this.WhenAnyValue(
                x => x.IsDiscoveryVisible, x => x.IsDiscoveryLoading, x => x.DiscoveredHosts, 
                (visible, loading, discovered) => visible && !loading && (discovered.Count == 0)).ToProperty(
                    this, model => model.IsDiscoveryListEmpty, out this.isDiscoveryListEmpty);
            
            this.AddHostCommand = ReactiveCommand.Create();
            this.AddHostCommand.Subscribe(x => { this.IsDiscoveryVisible = false; });

            // init command for starting client discovery
            this.DiscoveryCommand = ReactiveCommand.CreateAsyncTask(
                x => {
                    this.DiscoveredHosts.Clear();
                    this.IsDiscoveryLoading = true;
                    return RemoteContext.Instance.FindClientsAsync();
                });
            this.DiscoveryCommand.Subscribe(
                x => {
                    var remoteHosts = x as IList<RemoteHost> ?? x.ToList();
                    if (x != null && remoteHosts.Any())
                        this.DiscoveredHosts.AddRange(remoteHosts);
                    this.DiscoveredHosts.Add(new RemoteHost() { Ip = "192.168.150.1", Name = "TestMeClient" });
                    this.IsDiscoveryLoading = false;
                }, exception => MessageBox.Show(exception.Message));
        }

        /// <summary>
        /// The register parts.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
        private void RegisterParts(IMutableDependencyResolver dependencyResolver) {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));
            dependencyResolver.Register(() => new HomeView(), typeof(IViewFor<HomeViewModel>));
        }

        #endregion
    }
}