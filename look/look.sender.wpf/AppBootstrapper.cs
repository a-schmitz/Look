// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBootstrapper.cs" company="">
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

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Views;

    using ReactiveUI;

    using Splat;

    #endregion

    /// <summary>
    ///     The app bootstrapper.
    /// </summary>
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
        /// <param name="testRouter">
        /// The test router.
        /// </param>
        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null) {
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
        }

        #endregion

        #region Public Properties

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
            //dependencyResolver.Register(() => new RemoteViewerView(), typeof(IViewFor<IRemoteViewerViewModel>));
           // dependencyResolver.Register(() => new SharedWindowsView(), typeof(IViewFor<ISharedWindowsViewModel>));
        }

        #endregion
    }

}