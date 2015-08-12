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
        ReactiveList<Favorite> Favorites { get; set; }

        /// <summary>
        ///     Gets or sets the favorite.
        /// </summary>
        Favorite SelectedFavorite { get; set; }

        /// <summary>
        ///     Gets or sets the shareable windows.
        /// </summary>
        ReactiveList<ShareableWindow> ShareableWindows { get; set; }

        #endregion
    }
}