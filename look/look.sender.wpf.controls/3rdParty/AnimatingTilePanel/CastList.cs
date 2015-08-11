using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace look.sender.wpf.controls._3rdParty.AnimatingTilePanel
{

    public class CastList<TFrom, TTo> : ListBase<TTo>
        where TFrom : TTo
    {
        /// <summary>
        ///     Creats a new <see cref="CastList{TFrom,TTo}"/>.
        /// </summary>
        /// <param name="from">The source collection.</param>
        public CastList(IList<TFrom> from)
        {
            m_source = from;
        }

        /// <summary>
        ///     Gets the element at the specified index.
        /// </summary>
        protected override TTo GetItem(int index)
        {
            return m_source[index];
        }

        /// <summary>
        ///     Gets the number of contained elements.
        /// </summary>
        public override int Count
        {
            get { return m_source.Count; }
        }

        #region Implementation

        private readonly IList<TFrom> m_source;

        #endregion
    }
}
