using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace look.sender.wpf.controls._3rdParty.AnimatingTilePanel
{
    using System.Threading;

    public sealed class ActionOnDispose : IDisposable
    {
        /// <summary>
        ///     Creats a new <see cref="ActionOnDispose"/>
        ///     using the provided <see cref="Action"/>.
        /// </summary>
        /// <param name="unlockAction">
        ///     The <see cref="Action"/> to invoke when <see cref="Dispose"/> is called.
        /// </param>
        /// <exception cref="ArgumentNullException">if <paramref name="unlockAction"/> is null.</exception>
        public ActionOnDispose(Action unlockAction)
        {
            m_unlockDelegate = unlockAction;
        }

        /// <summary>
        ///     Calls the provided Action if it has not been called;
        ///     otherwise, throws an <see cref="Exception"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <see cref="Dispose()"/> has already been called.</exception>
        public void Dispose()
        {
            Action action = Interlocked.Exchange(ref m_unlockDelegate, null);
            Util.ThrowUnless<ObjectDisposedException>(action != null, "Dispose has already been called on this object.");
            action();
        }

        #region Implementation

        private Action m_unlockDelegate;

        #endregion
    } //*** class ActionOnDispose
}
