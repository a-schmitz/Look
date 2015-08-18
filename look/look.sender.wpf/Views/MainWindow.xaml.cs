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

    using System.Reactive.Linq;
    using System.Windows.Input;

    using look.sender.wpf.ViewModels;

    using MahApps.Metro.Controls;

    using ReactiveUI;

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

            this.MainWindowViewModel = new MainWindowViewModel();
            this.DataContext = this.MainWindowViewModel;


        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the app bootstrapper.
        /// </summary>
        public MainWindowViewModel MainWindowViewModel { get; protected set; }

        #endregion

        private void Lst_OnMouseDoubleClick(object sender, MouseButtonEventArgs e) {

            if (lst.SelectedItem == null)
                return;

            this.MainWindowViewModel.AddHostCommand.Execute(lst.SelectedItem);
            
        }
    }

}