// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Views
{
    #region

    using look.sender.wpf.ViewModels;

    using MahApps.Metro.Controls;

    #endregion

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow() {
            this.InitializeComponent();

            this.AppBootstrapper = new AppBootstrapper();
            this.DataContext = this.AppBootstrapper;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the app bootstrapper.
        /// </summary>
        public AppBootstrapper AppBootstrapper { get; protected set; }

        #endregion
    }

}