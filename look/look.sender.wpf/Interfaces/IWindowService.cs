// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowService.cs" company="">
//   
// </copyright>
// <summary>
//   The WindowService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Interfaces
{
    using System.Collections.Generic;

    using look.sender.wpf.Models;

    /// <summary>
    ///     The WindowService interface.
    /// </summary>
    public interface IWindowService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get shareable windows.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" />.
        /// </returns>
        IEnumerable<ShareableWindow> GetShareableWindows();

        #endregion
    }
}