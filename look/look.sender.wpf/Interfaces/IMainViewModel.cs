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

    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        ///     The do client discovery async.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        ReactiveCommand<IEnumerable<RemoteHost>> DiscoveryCommand { get; }

        ReactiveCommand<object> AddHostCommand { get; }

        #endregion
    }
}