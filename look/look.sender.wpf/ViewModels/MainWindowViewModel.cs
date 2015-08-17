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

    using System.Reactive.Concurrency;
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
        ///     The discovered hosts.
        /// </summary>
        private IReactiveList<RemoteHost> discoveredHosts = new ReactiveList<RemoteHost>();

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

            // Bind 
            this.RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
            LogHost.Default.Level = LogLevel.Debug;

            // Navigate to the opening page of the application
            this.Router.Navigate.Execute(new HomeViewModel(this));

            // start local instance so that we can be discovered by other clients
            RemoteContext.Instance.StartAcceptingConnections("MyHost");
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the discovered hosts.
        /// </summary>
        public IReactiveList<RemoteHost> DiscoveredHosts {
            get { return this.discoveredHosts; }
            set { this.RaiseAndSetIfChanged(ref this.discoveredHosts, value); }
        }

        /// <summary>
        ///     Gets a value indicating whether is discovery list empty.
        /// </summary>
        public bool IsDiscoveryListEmpty {
            get {
                var result = this.IsDiscoveryVisible && !this.isDiscoveryLoading && (this.DiscoveredHosts == null || this.DiscoveredHosts.Count == 0);
                return result;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is loading.
        /// </summary>
        public bool IsDiscoveryLoading {
            get { return this.isDiscoveryLoading; }
            set {
                this.RaiseAndSetIfChanged(ref this.isDiscoveryLoading, value);
                this.IsDiscoveryVisible = true;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is discovery visible.
        /// </summary>
        public bool IsDiscoveryVisible {
            get { return this.isDiscoveryVisible; }
            set {
                this.RaiseAndSetIfChanged(ref this.isDiscoveryVisible, value);
                this.RaisePropertyChanged("IsDiscoveryListEmpty");
            }
        }

        /// <summary>
        ///     Gets the router.
        /// </summary>
        public RoutingState Router { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The register parts.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
        private void RegisterParts(IMutableDependencyResolver dependencyResolver) {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new HomeView(), typeof(IViewFor<HomeViewModel>));
            // dependencyResolver.Register(() => new RemoteViewerView(), typeof(IViewFor<IRemoteViewerViewModel>));
            // dependencyResolver.Register(() => new SharedWindowsView(), typeof(IViewFor<ISharedWindowsViewModel>));
        }

        #endregion
    }
}