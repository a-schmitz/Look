// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHomeViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The WelcomeViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Interfaces
{
    #region

    using System;

    using DynamicData;

    using look.sender.wpf.Models;

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The WelcomeViewModel interface.
    /// </summary>
    public interface IHomeViewModel : IRoutableViewModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the favorites.
        /// </summary>
        ReactiveList<RemoteHost> RemoteHosts { get; set; }

        /// <summary>
        ///     Gets or sets the favorite.
        /// </summary>
        RemoteHost SelectedRemoteHost { get; set; }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        ISourceCache<ShareableWindow, IntPtr> ShareableWindows { get; set; }

        #endregion
    }

}