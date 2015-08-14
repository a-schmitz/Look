// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITabViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The TabViewModel interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Interfaces
{
    #region

    using ReactiveUI;

    #endregion

    /// <summary>
    ///     The TabViewModel interface.
    /// </summary>
    public interface ITabViewModel : IRoutableViewModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        string Header { get; set; }

        #endregion
    }

}