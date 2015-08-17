namespace look.capture.Helper
{

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ScreenCapture
    {
        private Bitmap _prevBitmap;
        private Bitmap _newBitmap = new Bitmap(1, 1);
        private Graphics _graphics;

        public double PercentOfImage { get; set; }

        public ScreenCapture()
        {
            var junk = new Bitmap(10, 10);
            this._graphics = Graphics.FromImage(junk);
        }

        /// <summary>
        /// Capture the changes to the screen since the last
        /// capture.
        /// </summary>
        /// <param name="bounds">The bounding box that encompasses
        /// all changed pixels.</param>
        /// <returns>Full or partial bitmap, null for no changes</returns>
        public Bitmap Screen(ref Rectangle bounds, IntPtr handle = default(IntPtr))
        {
            
            Bitmap diff = null;

            // Capture a new screenshot.
            //
            lock (this._newBitmap) {
                this._newBitmap = handle.Equals(default(IntPtr)) ? Capture.CaptureDesktop() : Capture.CaptureHandle(handle);

                // If we have a previous screenshot, only send back
                //	a subset that is the minimum rectangular area
                //	that encompasses all the changed pixels.
                //
                if (this._prevBitmap != null)
                {
                    // Get the bounding box.
                    //
                    bounds = this.GetBoundingBoxForChanges();
                    if (bounds == Rectangle.Empty)
                    {
                        // Nothing has changed.
                        //
                        this.PercentOfImage = 0.0;
                    }
                    else
                    {
                        // Get the minimum rectangular area
                        //
                        diff = new Bitmap(bounds.Width, bounds.Height);
                        this._graphics = Graphics.FromImage(diff);
                        this._graphics.DrawImage(this._newBitmap, 0, 0, bounds, GraphicsUnit.Pixel);

                        // Set the current bitmap as the previous to prepare
                        //	for the next screen capture.
                        //
                        this._prevBitmap = this._newBitmap;

                        lock (this._newBitmap)
                        {
                            this.PercentOfImage = 100.0 * (diff.Height * diff.Width) / (this._newBitmap.Height * this._newBitmap.Width);
                        }
                    }
                }
                // We don't have a previous screen capture. Therefore
                //	we need to send back the whole screen this time.
                //
                else
                {
                    // Set the previous bitmap to the current to prepare
                    //	for the next screen capture.
                    //
                    this._prevBitmap = this._newBitmap;
                    diff = this._newBitmap;

                    // Create a bounding rectangle.
                    //
                    bounds = new Rectangle(0, 0, this._newBitmap.Width, this._newBitmap.Height);

                    this.PercentOfImage = 100.0;
                }
            }
            return diff;
        }

        /// <summary>
        /// Capture the cursor bitmap.
        /// </summary>
        /// <param name="cursorX">The cursor X.</param>
        /// <param name="cursorY">The cursor Y.</param>
        /// <returns>The bitmap or null.</returns>
        public Bitmap Cursor(ref int cursorX, ref int cursorY)
        {
            int screenWidth = 1;
            int screenHeight = 1;
            lock (this._newBitmap)
            {
                try
                {
                    screenWidth = this._newBitmap.Width;
                    screenHeight = this._newBitmap.Height;
                }
                catch (Exception)
                {
                    // Need to debug the exception!
                }
            }
            if (screenWidth == 1 && screenHeight == 1)
            {
                return null;
            }
            Bitmap img = Capture.CaptureCursor(ref cursorX, ref cursorY);
            if (img != null && cursorX < screenWidth && cursorY < screenHeight)
            {
                // The cursor is mostly transparent. This makes it difficult
                //	to see when the cursor is the text editing icon. Easy
                //	fix is to make the cursor slighly less transparent.
                //
                int width = img.Width;
                int height = img.Height;

                // Get the bitmap data.
                //
                BitmapData imgData = img.LockBits(
                    new Rectangle(0, 0, width, height),
                    ImageLockMode.ReadOnly, img.PixelFormat);

                // The images are ARGB (4 bytes)
                //
                const int numBytesPerPixel = 4;

                // Get the number of integers (4 bytes) in each row
                //	of the image.
                //
                int stride = imgData.Stride;
                IntPtr scan0 = imgData.Scan0;
                unsafe
                {
                    // Cast the safe pointers into unsafe pointers.
                    //
                    byte* pByte = (byte*)(void*)scan0;
                    for (int h = 0; h < height; h++)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            int offset = h * stride + w * numBytesPerPixel + 3;
                            if (*(pByte + offset) == 0)
                            {
                                *(pByte + offset) = 60;
                            }
                        }
                    }
                }
                img.UnlockBits(imgData);

                return img;
            }
            return null;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this._prevBitmap = null;
            this._newBitmap = new Bitmap(1, 1);
        }


        /// <summary>
        /// Gets the bounding box for changes.
        /// </summary>
        /// <returns></returns>
        private Rectangle GetBoundingBoxForChanges()
        {
            // The search algorithm starts by looking
            //	for the top and left bounds. The search
            //	starts in the upper-left corner and scans
            //	left to right and then top to bottom. It uses
            //	an adaptive approach on the pixels it
            //	searches. Another pass is looks for the
            //	lower and right bounds. The search starts
            //	in the lower-right corner and scans right
            //	to left and then bottom to top. Again, an
            //	adaptive approach on the search area is used.
            //

            // Notice: The GetPixel member of the Bitmap class
            //	is too slow for this purpose. This is a good
            //	case of using unsafe code to access pointers
            //	to increase the speed.
            //

            // Validate the images are the same shape and type.
            //
            if (this._prevBitmap.Width != this._newBitmap.Width ||
                this._prevBitmap.Height != this._newBitmap.Height ||
                this._prevBitmap.PixelFormat != this._newBitmap.PixelFormat)
            {
                // Not the same shape...can't do the search.
                //
                return Rectangle.Empty;
            }

            // Init the search parameters.
            //
            int width = this._newBitmap.Width;
            int height = this._newBitmap.Height;
            int left = width;
            int right = 0;
            int top = height;
            int bottom = 0;

            BitmapData bmNewData = null;
            BitmapData bmPrevData = null;
            try
            {
                // Lock the bits into memory.
                //
                bmNewData = this._newBitmap.LockBits(
                    new Rectangle(0, 0, this._newBitmap.Width, this._newBitmap.Height),
                    ImageLockMode.ReadOnly, this._newBitmap.PixelFormat);
                bmPrevData = this._prevBitmap.LockBits(
                    new Rectangle(0, 0, this._prevBitmap.Width, this._prevBitmap.Height),
                    ImageLockMode.ReadOnly, this._prevBitmap.PixelFormat);

                // The images are ARGB (4 bytes)
                //
                const int numBytesPerPixel = 4;

                // Get the number of integers (4 bytes) in each row
                //	of the image.
                //
                int strideNew = bmNewData.Stride / numBytesPerPixel;
                int stridePrev = bmPrevData.Stride / numBytesPerPixel;

                // Get a pointer to the first pixel.
                //
                // Notice: Another speed up implemented is that I don't
                //	need the ARGB elements. I am only trying to detect
                //	change. So this algorithm reads the 4 bytes as an
                //	integer and compares the two numbers.
                //
                IntPtr scanNew0 = bmNewData.Scan0;
                IntPtr scanPrev0 = bmPrevData.Scan0;

                // Enter the unsafe code.
                //
                unsafe
                {
                    // Cast the safe pointers into unsafe pointers.
                    //
                    int* pNew = (int*)(void*)scanNew0;
                    int* pPrev = (int*)(void*)scanPrev0;

                    // First Pass - Find the left and top bounds
                    //	of the minimum bounding rectangle. Adapt the
                    //	number of pixels scanned from left to right so
                    //	we only scan up to the current bound. We also
                    //	initialize the bottom & right. This helps optimize
                    //	the second pass.
                    //
                    // For all rows of pixels (top to bottom)
                    //
                    for (int y = 0; y < this._newBitmap.Height; ++y)
                    {
                        // For pixels up to the current bound (left to right)
                        //
                        for (int x = 0; x < left; ++x)
                        {
                            // Use pointer arithmetic to index the
                            //	next pixel in this row.
                            //
                            if ((pNew + x)[0] != (pPrev + x)[0])
                            {
                                // Found a change.
                                //
                                if (x < left)
                                {
                                    left = x;
                                }
                                if (x > right)
                                {
                                    right = x;
                                }
                                if (y < top)
                                {
                                    top = y;
                                }
                                if (y > bottom)
                                {
                                    bottom = y;
                                }
                            }
                        }

                        // Move the pointers to the next row.
                        //
                        pNew += strideNew;
                        pPrev += stridePrev;
                    }

                    // If we did not find any changed pixels
                    //	then no need to do a second pass.
                    //
                    if (left != width)
                    {
                        // Second Pass - The first pass found at
                        //	least one different pixel and has set
                        //	the left & top bounds. In addition, the
                        //	right & bottom bounds have been initialized.
                        //	Adapt the number of pixels scanned from right
                        //	to left so we only scan up to the current bound.
                        //	In addition, there is no need to scan past
                        //	the top bound.
                        //

                        // Set the pointers to the first element of the
                        //	bottom row.
                        //
                        pNew = (int*)(void*)scanNew0;
                        pPrev = (int*)(void*)scanPrev0;
                        pNew += (this._newBitmap.Height - 1) * strideNew;
                        pPrev += (this._prevBitmap.Height - 1) * stridePrev;

                        // For each row (bottom to top)
                        //
                        for (int y = this._newBitmap.Height - 1; y > top; y--)
                        {
                            // For each column (right to left)
                            //
                            for (int x = this._newBitmap.Width - 1; x > right; x--)
                            {
                                // Use pointer arithmetic to index the
                                //	next pixel in this row.
                                //
                                if ((pNew + x)[0] != (pPrev + x)[0])
                                {
                                    // Found a change.
                                    //
                                    if (x > right)
                                    {
                                        right = x;
                                    }
                                    if (y > bottom)
                                    {
                                        bottom = y;
                                    }
                                }
                            }

                            // Move up one row.
                            //
                            pNew -= strideNew;
                            pPrev -= stridePrev;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Do something with this info.
            }
            finally
            {
                // Unlock the bits of the image.
                //
                if (bmNewData != null)
                {
                    this._newBitmap.UnlockBits(bmNewData);
                }
                if (bmPrevData != null)
                {
                    this._prevBitmap.UnlockBits(bmPrevData);
                }
            }

            // Validate we found a bounding box. If not
            //	return an empty rectangle.
            //
            int diffImgWidth = right - left + 1;
            int diffImgHeight = bottom - top + 1;
            if (diffImgHeight < 0 || diffImgWidth < 0)
            {
                // Nothing changed
                return Rectangle.Empty;
            }

            // Return the bounding box.
            //
            return new Rectangle(left, top, diffImgWidth, diffImgHeight);
        }
    }
}
