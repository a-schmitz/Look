namespace look.sender.wpf.helper
{

    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    ///     The boolean converter.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanToVisibilityConverter"/> class. 
        ///     Initializes a new instance of the <see cref="BoolToVisibleOrHidden{T}"/> class.
        ///     The default constructor
        /// </summary>
        public BooleanToVisibilityConverter() { }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var bValue = parameter != null && parameter.ToString().ToLower() == "inverse" ? !(bool)value : (bool)value;

            return bValue ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// The convert back.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var visibility = (Visibility)value;
            var reverse = parameter != null && parameter.ToString().ToLower() == "inverse";

            if (visibility == Visibility.Visible)
                return !reverse;
            else
                return reverse;
        }

        #endregion
    }

}