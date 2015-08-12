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
    using look.sender.wpf.Models;

    using ReactiveUI;

    /// <summary>
    ///     The SharedWindowsViewModel interface.
    /// </summary>
    public interface ISharedWindowsViewModel : ITabViewModel
    {
        ReactiveList<ShareableWindow> ShareableWindows { get; set; }
    }
}