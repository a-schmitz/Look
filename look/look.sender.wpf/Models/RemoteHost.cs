// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteHost.cs" company="">
//   
// </copyright>
// <summary>
//   The favorite.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Models
{
    #region

    using look.sender.wpf.Interfaces;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The favorite.
    /// </summary>
    public class RemoteHost
    {
        #region Fields

        /// <summary>
        ///     The selected view model.
        /// </summary>
        private ITabViewModel selectedViewModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteHost" /> class.
        /// </summary>
        public RemoteHost() {
            this.Tabs = new ReactiveList<ITabViewModel>();
            this.SharedWindows = new ReactiveList<RemoteHostShareableWindow>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether connected.
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        ///     Gets or sets the ip address.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the selected view model.
        /// </summary>
        public ITabViewModel SelectedViewModel {
            get {
                if (this.selectedViewModel == null && this.Tabs.Count > 0)
                    return this.Tabs[0];
                return this.selectedViewModel;
            }
            set { this.selectedViewModel = value; }
        }

        /// <summary>
        ///     Gets or sets the shared windows.
        /// </summary>
        public ReactiveList<RemoteHostShareableWindow> SharedWindows { get; set; }

        /// <summary>
        ///     Gets or sets the tabs.
        /// </summary>
        public ReactiveList<ITabViewModel> Tabs { get; set; }

        #endregion
    }

}