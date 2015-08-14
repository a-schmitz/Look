// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISharedWindowsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The SharedWindowsViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Interfaces
{
    #region

    using System.Collections.ObjectModel;

    using look.sender.wpf.Models;

    #endregion

    /// <summary>
    ///     The SharedWindowsViewModel interface.
    /// </summary>
    public interface ISharedWindowsViewModel : ITabViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets the shareable windows.
        /// </summary>
        ReadOnlyObservableCollection<RemoteHostShareableWindow> ShareableWindows { get; }

        #endregion
    }

}