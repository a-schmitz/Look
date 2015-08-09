// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowTile.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for WindowTile.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    ///     Interaction logic for WindowTile.xaml
    /// </summary>
    public partial class WindowTile : UserControl
    {
        #region Static Fields

        /// <summary>
        ///     The checked property.
        /// </summary>
        public static readonly DependencyProperty CheckedProperty = DependencyProperty.Register(
            "Checked", 
            typeof(bool), 
            typeof(WindowTile), 
            new PropertyMetadata(default(bool)));

        /// <summary>
        ///     The window title property.
        /// </summary>
        public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
            "WindowTitle", 
            typeof(string), 
            typeof(WindowTile), 
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     The disabled image property.
        /// </summary>
        public static DependencyProperty ApplicationImageProperty = DependencyProperty.Register(
            "ApplicationImage", 
            typeof(ImageSource), 
            typeof(WindowTile), 
            new UIPropertyMetadata(null));

        /// <summary>
        ///     The application name property.
        /// </summary>
        public static DependencyProperty ApplicationNameProperty = DependencyProperty.Register(
            "ApplicationName", 
            typeof(string), 
            typeof(WindowTile), 
            new UIPropertyMetadata(null));

        /// <summary>
        ///     The color counter.
        /// </summary>
        private static int colorCounter = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowTile" /> class.
        /// </summary>
        public WindowTile()
        {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;

            this.setBackgroundColor();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the image.
        /// </summary>
        public ImageSource ApplicationImage
        {
            get
            {
                return (ImageSource)this.GetValue(ApplicationImageProperty);
            }

            set
            {
                this.SetValue(ApplicationImageProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the application name.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return (string)this.GetValue(ApplicationNameProperty);
            }

            set
            {
                this.SetValue(ApplicationNameProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether checked.
        /// </summary>
        public bool Checked
        {
            get
            {
                return (bool)this.GetValue(CheckedProperty);
            }

            set
            {
                this.SetValue(CheckedProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets the window title.
        /// </summary>
        public string WindowTitle
        {
            get
            {
                return (string)this.GetValue(WindowTitleProperty);
            }

            set
            {
                this.SetValue(WindowTitleProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The layout root_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LayoutRoot_Click(object sender, RoutedEventArgs e)
        {
            this.Checked = !this.Checked;
        }

        /// <summary>
        /// The user control_ mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Checked = !this.Checked;
        }

        /// <summary>
        ///     The set background color.
        /// </summary>
        private void setBackgroundColor()
        {
            var colors = new List<Color>();
            colors.Add((Color)ColorConverter.ConvertFromString("#FF00B2F0"));
            colors.Add((Color)ColorConverter.ConvertFromString("#FF297BCB"));
            colors.Add((Color)ColorConverter.ConvertFromString("#FF10883C"));
            colors.Add((Color)ColorConverter.ConvertFromString("#FF3AB060"));

            var brush = new SolidColorBrush(colors[colorCounter]);
            this.Background = brush;

            colorCounter++;
            if (colorCounter == colors.Count)
            {
                colorCounter = 0;
            }
        }

        #endregion
    }
}