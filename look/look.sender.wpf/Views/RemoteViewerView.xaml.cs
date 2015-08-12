// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteViewerView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for WindowViewerView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.ViewModels;

    using ReactiveUI;

    /// <summary>
    ///     Interaction logic for WindowViewerView.xaml
    /// </summary>
    public partial class RemoteViewerView : UserControl, IViewFor<IRemoteViewerViewModel>
    {
        #region Constructors and Destructors

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
           "ViewModel",
           typeof(RemoteViewerViewModel),
           typeof(RemoteViewerView),
           new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewerView"/> class.
        /// </summary>
        public RemoteViewerView()
        {
            this.InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public IRemoteViewerViewModel ViewModel { get; set; }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }

            set
            {
                this.ViewModel = (IRemoteViewerViewModel)value;
            }
        }

        #endregion
    }
}