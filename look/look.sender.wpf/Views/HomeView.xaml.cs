// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for MainView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using look.sender.wpf.Interfaces;

    using ReactiveUI;

    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class HomeView : UserControl, IViewFor<IHomeViewModel>
    {
        #region Static Fields

        /// <summary>
        ///     The view model property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", 
            typeof(IHomeViewModel), 
            typeof(HomeView), 
            new PropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class. 
        ///     Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public HomeView()
        {
            this.InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the view model.
        /// </summary>
        public IHomeViewModel ViewModel
        {
            get
            {
                return (IHomeViewModel)this.GetValue(ViewModelProperty);
            }

            set
            {
                this.SetValue(ViewModelProperty, value);
            }
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        ///     Gets or sets the view model.
        /// </summary>
        object IViewFor.ViewModel
        {
            get
            {
                return this.ViewModel;
            }

            set
            {
                this.ViewModel = (IHomeViewModel)value;
            }
        }

        #endregion
    }
}