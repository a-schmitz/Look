// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FavoriteItem.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for FavoriteItem.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.controls
{
    #region

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    #endregion

    /// <summary>
    ///     Interaction logic for FavoriteItem.xaml
    /// </summary>
    public partial class FavoriteItem : UserControl
    {
        #region Static Fields

        /// <summary>
        ///     The connected property.
        /// </summary>
        public static readonly DependencyProperty ConnectedProperty = DependencyProperty.Register(
            "Connected", typeof(bool), typeof(FavoriteItem), new PropertyMetadata(default(bool)));

        /// <summary>
        ///     The sub title property.
        /// </summary>
        public static readonly DependencyProperty SubTitleProperty = DependencyProperty.Register(
            "SubTitle", typeof(string), typeof(FavoriteItem), new PropertyMetadata(default(string)));

        /// <summary>
        ///     The title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(FavoriteItem), new PropertyMetadata(default(string)));

        #endregion

        #region Fields

        /// <summary>
        ///     The dispatcher timer.
        /// </summary>
        private readonly DispatcherTimer dispatcherTimer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FavoriteItem" /> class.
        /// </summary>
        public FavoriteItem() {
            this.InitializeComponent();

            this.LayoutRoot.DataContext = this;

            // TODO: Remove Timer, replace with real connect code
            this.dispatcherTimer = new DispatcherTimer();
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            this.dispatcherTimer.Tick += this.dispatcherTimer_Tick;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether connected.
        /// </summary>
        public bool Connected { get { return (bool)this.GetValue(ConnectedProperty); } set { this.SetValue(ConnectedProperty, value); } }

        /// <summary>
        ///     Gets or sets the sub title.
        /// </summary>
        public string SubTitle { get { return (string)this.GetValue(SubTitleProperty); } set { this.SetValue(SubTitleProperty, value); } }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        public string Title { get { return (string)this.GetValue(TitleProperty); } set { this.SetValue(TitleProperty, value); } }

        #endregion

        #region Methods

        /// <summary>
        /// The border connect_ mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void borderConnect_MouseUp(object sender, MouseButtonEventArgs e) {
            if (!this.Connected) {
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateConnecting", false);
                this.dispatcherTimer.Start();
            } else {
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateDisconnected", false);
                this.Connected = false;
            }
        }

        /// <summary>
        /// TODO: Can be removed once real Connections are implemented
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void dispatcherTimer_Tick(object sender, EventArgs e) {
            // TODO: REMOVE!
            this.dispatcherTimer.Stop();
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateConnected", false);
            this.Connected = true;
        }

        #endregion
    }

}