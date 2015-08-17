// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMainViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The MainViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Interfaces
{
    #region

    using look.common.Model;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The MainViewModel interface.
    /// </summary>
    public interface IMainViewModel : IScreen
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the discovered hosts.
        /// </summary>
        IReactiveList<RemoteHost> DiscoveredHosts { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is loading.
        /// </summary>
        bool IsDiscoveryLoading { get; set; }

        #endregion
    }

}