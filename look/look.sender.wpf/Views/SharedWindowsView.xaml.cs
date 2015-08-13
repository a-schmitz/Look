// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedWindowsView.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for SharedWindowsView.xaml
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
    ///     Interaction logic for SharedWindowsView.xaml
    /// </summary>
    public partial class SharedWindowsView : UserControl, IViewFor<ISharedWindowsViewModel>
    {
        #region Static Fields

        /// <summary>
        ///     The view model property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", 
            typeof(SharedWindowsViewModel), 
            typeof(SharedWindowsView), 
            new PropertyMetadata(null));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SharedWindowsView" /> class.
        /// </summary>
        public SharedWindowsView()
        {
            this.InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the view model.
        /// </summary>
        public ISharedWindowsViewModel ViewModel
        {
            get
            {
                return (ISharedWindowsViewModel)this.GetValue(ViewModelProperty);
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
                this.ViewModel = (ISharedWindowsViewModel)value;
            }
        }

        #endregion
    }
}