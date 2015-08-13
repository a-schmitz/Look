// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverlayAdorner.cs" company="">
//   
// </copyright>
// <summary>
//   Overlays a control with the specified content
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.controls.Helper
{
    #region

    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    #endregion

    /// <summary>
    /// Overlays a control with the specified content
    /// </summary>
    /// <typeparam name="TOverlay">
    /// The type of content to create the overlay from
    /// </typeparam>
    public class OverlayAdorner<TOverlay> : Adorner, IDisposable
        where TOverlay : UIElement, new()
    {
        #region Fields

        /// <summary>
        /// The _adorning element.
        /// </summary>
        private readonly UIElement _adorningElement;

        /// <summary>
        /// The _layer.
        /// </summary>
        private AdornerLayer _layer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayAdorner{TOverlay}"/> class.
        /// </summary>
        /// <param name="elementToAdorn">
        /// The element to adorn.
        /// </param>
        /// <param name="adorningElement">
        /// The adorning element.
        /// </param>
        private OverlayAdorner(UIElement elementToAdorn, UIElement adorningElement)
            : base(elementToAdorn) {
            this._adorningElement = adorningElement;
            if (adorningElement != null)
                this.AddVisualChild(adorningElement);
            this.Focusable = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the visual children count.
        /// </summary>
        protected override int VisualChildrenCount { get { return this._adorningElement == null ? 0 : 1; } }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// /// Overlay the specified element /// 
        /// </summary>
        /// ///
        /// <param name="elementToAdorn">
        /// The element to overlay
        /// </param>
        /// <param name="adorningElement">
        /// The content of the overlay
        /// </param>
        /// ///
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        /// public static IDisposable Overlay(UIElement elementToAdorn) { return Overlay(elementToAdorn, new TOverlay()); }
        /// <summary>
        /// Overlays the element with the specified instance of TOverlay
        /// </summary>
        /// <param name="elementToAdorn">
        /// Element to overlay
        /// </param>
        /// <returns>
        /// </returns>
        public static IDisposable Overlay(UIElement elementToAdorn, TOverlay adorningElement) {
            var adorner = new OverlayAdorner<TOverlay>(elementToAdorn, adorningElement);
            adorner._layer = AdornerLayer.GetAdornerLayer(elementToAdorn);
            adorner._layer.Add(adorner);
            return adorner as IDisposable;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose() { this._layer.Remove(this); }

        #endregion

        #region Methods

        /// <summary>
        /// The arrange override.
        /// </summary>
        /// <param name="finalSize">
        /// The final size.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize) {
            if (this._adorningElement != null) {
                var adorningPoint = new Point(0, 0);
                this._adorningElement.Arrange(new Rect(adorningPoint, this.AdornedElement.DesiredSize));
            }
            return finalSize;
        }

        /// <summary>
        /// The get visual child.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Visual"/>.
        /// </returns>
        protected override Visual GetVisualChild(int index) {
            if (index == 0 && this._adorningElement != null)
                return this._adorningElement;
            return base.GetVisualChild(index);
        }

        #endregion
    }

}