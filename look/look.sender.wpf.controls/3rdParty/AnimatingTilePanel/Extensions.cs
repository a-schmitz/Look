using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace look.sender.wpf.controls._3rdParty.AnimatingTilePanel
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    public static class Extensions
    {
        public static IEqualityComparer<T> ToEqualityComparer<T>(this Func<T, T, bool> func)
        {
            return new FuncEqualityComparer<T>(func);
        }

        public static IComparer<T> ToComparer<T>(this Func<T, T, int> compareFunction)
        {
            return new FuncComparer<T>(compareFunction);
        }

        public static IComparer<T> ToComparer<T>(this Comparison<T> compareFunction)
        {
            return new ComparisonComparer<T>(compareFunction);
        }

        public static IComparer<string> ToComparer<T>(this CompareInfo compareInfo)
        {
            return new FuncComparer<string>(compareInfo.Compare);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return str == null || str.Trim().Length == 0;
        }

        /// <summary>
        /// Verifies that a property name exists in this ViewModel. This method
        /// can be called before the property is used, for instance before
        /// calling RaisePropertyChanged. It avoids errors when a property name
        /// is changed but some places are missed.
        /// <para>This method is only active in DEBUG mode.</para>
        /// </summary>
        /// <param name="element">The object to watch.</param>
        /// <remarks>Thanks to Laurent Bugnion for the idea.</remarks>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void VerifyPropertyNamesOnChange(this INotifyPropertyChanged element)
        {
            var myType = element.GetType();
            element.PropertyChanged += (sender, args) =>
            {
                Util.ThrowUnless<InvalidOperationException>(myType.HasPublicInstanceProperty(args.PropertyName), "The object '{0}' of type '{1}' raised a property change for '{2}' which isn't a public property on the type.".DoFormat(element, myType, args.PropertyName));
            };
        }

        public static TResponse UseAndDispose<T, TResponse>(this T source, Func<T, TResponse> func) where T : IDisposable
        {
            using (source)
            {
                return func(source);
            }
        }

        public static string DoFormat(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

#if SILVERLIGHT
        public static void VerifyAccess(this System.Windows.DependencyObject dependencyObj)
        {
            Contract.Requires(dependencyObj != null);
            if (!dependencyObj.CheckAccess())
            {
                throw new InvalidOperationException("A call was made off the Dispatcher thread.");
            }
        }
#endif

        public static bool NextBool(this Random rnd)
        {
            return rnd.Next() % 2 == 0;
        }

        public static float NextFloat(this Random rnd, float min = 0, float max = 1)
        {
            var delta = max - min;
            return (float)rnd.NextDouble() * delta + min;
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : Attribute
        {
            return attributeProvider.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        public static bool HasPublicInstanceProperty(this IReflect type, string name)
        {
            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance) != null;
        }

     

        public static IComparer<string> GetStringComparer(this CultureInfo cultureInfo, CompareOptions options = CompareOptions.None)
        {
            var func = new Func<string, string, int>((a, b) => cultureInfo.CompareInfo.Compare(a, b, options));
            return func.ToComparer();
        }

        #region impl
        private class FuncComparer<T> : IComparer<T>
        {
            public FuncComparer(Func<T, T, int> func)
            {
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Func<T, T, int> m_func;
        }

        private class ComparisonComparer<T> : IComparer<T>
        {
            public ComparisonComparer(Comparison<T> func)
            {
                m_func = func;
            }

            public int Compare(T x, T y)
            {
                return m_func(x, y);
            }

            private readonly Comparison<T> m_func;
        }

        private class FuncEqualityComparer<T> : IEqualityComparer<T>
        {
            public FuncEqualityComparer(Func<T, T, bool> func)
            {
                m_func = func;
            }
            public bool Equals(T x, T y)
            {
                return m_func(x, y);
            }

            public int GetHashCode(T obj)
            {
                return 0; // this is on purpose. Should only use function...not short-cut by hashcode compare
            }

            void ObjectInvariant()
            {
            }

            private readonly Func<T, T, bool> m_func;
        }
        #endregion
    }
}
