namespace look.common.Helper
{

    using System;
    using System.Drawing;
    using System.IO;

    public static class Utils
    {
        private const int NumBytesInInt = sizeof(int);

        public static byte[] PackScreenCaptureData(Guid id, Image image, Rectangle bounds)
        {
            var idData = id.ToByteArray();

            byte[] imgData;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imgData = ms.ToArray();
            }

            var topData = BitConverter.GetBytes(bounds.Top);
            var botData = BitConverter.GetBytes(bounds.Bottom);
            var leftData = BitConverter.GetBytes(bounds.Left);
            var rightData = BitConverter.GetBytes(bounds.Right);
            
            var sizeOfInt = topData.Length;
            var result = new byte[imgData.Length + (4 * sizeOfInt) + idData.Length];
            Array.Copy(topData, 0, result, 0, topData.Length);
            Array.Copy(botData, 0, result, sizeOfInt, botData.Length);
            Array.Copy(leftData, 0, result, 2 * sizeOfInt, leftData.Length);
            Array.Copy(rightData, 0, result, 3 * sizeOfInt, rightData.Length);
            Array.Copy(imgData, 0, result, 4 * sizeOfInt, imgData.Length);
            Array.Copy(idData, 0, result, (4 * sizeOfInt) + imgData.Length, idData.Length);

            return result;
        }

        public static void UnpackScreenCaptureData(byte[] data, out Image image, out Rectangle bounds, out Guid id)
        {
            var idLength = Guid.NewGuid().ToByteArray().Length;
            var imgLength = data.Length - (4 * NumBytesInInt) - idLength;
            var topPosData = new byte[NumBytesInInt];
            var botPosData = new byte[NumBytesInInt];
            var leftPosData = new byte[NumBytesInInt];
            var rightPosData = new byte[NumBytesInInt];
            var imgData = new byte[imgLength];
            var idData = new byte[idLength];

            Array.Copy(data, 0, topPosData, 0, NumBytesInInt);
            Array.Copy(data, NumBytesInInt, botPosData, 0, NumBytesInInt);
            Array.Copy(data, 2 * NumBytesInInt, leftPosData, 0, NumBytesInInt);
            Array.Copy(data, 3 * NumBytesInInt, rightPosData, 0, NumBytesInInt);
            Array.Copy(data, 4 * NumBytesInInt, imgData, 0, imgLength);
            Array.Copy(data, (4 * NumBytesInInt) + imgLength, idData, 0, idLength);

            var ms = new MemoryStream(imgData, 0, imgData.Length);
            ms.Write(imgData, 0, imgData.Length);
            image = Image.FromStream(ms, true);

            var top = BitConverter.ToInt32(topPosData, 0);
            var bot = BitConverter.ToInt32(botPosData, 0);
            var left = BitConverter.ToInt32(leftPosData, 0);
            var right = BitConverter.ToInt32(rightPosData, 0);
            var width = right - left + 1;
            var height = bot - top + 1;
            bounds = new Rectangle(left, top, width, height);
           
            id = new Guid(idData);
        }

        public static byte[] PackCursorCaptureData(Guid id, Image image, int cursorX, int cursorY)
        {
            var idData = id.ToByteArray();
            byte[] imgData;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imgData = ms.ToArray();
            }

            var xPosData = BitConverter.GetBytes(cursorX);
            var yPosData = BitConverter.GetBytes(cursorY);

            var result = new byte[imgData.Length + xPosData.Length + yPosData.Length + idData.Length];
            Array.Copy(xPosData, 0, result, 0, xPosData.Length);
            Array.Copy(yPosData, 0, result, xPosData.Length, yPosData.Length);
            Array.Copy(imgData, 0, result, xPosData.Length + yPosData.Length, imgData.Length);
            Array.Copy(idData, 0, result, xPosData.Length + yPosData.Length + imgData.Length, idData.Length);

            return result;
        }

        public static void UnpackCursorCaptureData(byte[] data, out Image image, out int cursorX, out int cursorY, out Guid id)
        {
            var idLength = Guid.NewGuid().ToByteArray().Length;
            var imgLength = data.Length - 2 * NumBytesInInt - idLength;
            var xPosData = new byte[NumBytesInInt];
            var yPosData = new byte[NumBytesInInt];
            var imgData = new byte[imgLength];
            var idData = new byte[idLength];

            Array.Copy(data, 0, xPosData, 0, NumBytesInInt);
            Array.Copy(data, NumBytesInInt, yPosData, 0, NumBytesInInt);
            Array.Copy(data, 2 * NumBytesInInt, imgData, 0, imgLength);
            Array.Copy(data, 2 * NumBytesInInt + imgLength, idData, 0, idLength);

            cursorX = BitConverter.ToInt32(xPosData, 0);
            cursorY = BitConverter.ToInt32(yPosData, 0);

            using (var ms = new MemoryStream(imgData, 0, imgData.Length))
            {
                ms.Write(imgData, 0, imgData.Length);
                image = Image.FromStream(ms, true);
            }

            id = new Guid(idData);
        }

        public static void UpdateScreenImage(ref Image screen, Image newPartialScreen, Rectangle boundingBox)
        {
            if (screen == null)
                screen = new Bitmap(boundingBox.Width, boundingBox.Height);

            Graphics g = null;
            try
            {
                lock (screen)
                {
                    g = Graphics.FromImage(screen);
                    g.DrawImage(newPartialScreen, boundingBox);
                    g.Flush();
                }
            }
            finally
            {
                if (g != null)
                    g.Dispose();
            }
        }

        public static Image MergeScreenAndCursor(Image screen, Image cursor, int cursorX, int cursorY)
        {
            Image mergedImage;
            Graphics g = null;
            try
            {
                lock (screen)
                {
                    mergedImage = (Image)screen.Clone();
                }

                Rectangle r;
                lock (cursor)
                {
                    r = new Rectangle(cursorX, cursorY, cursor.Width, cursor.Height);
                }

                g = Graphics.FromImage(mergedImage);
                g.DrawImage(cursor, r);
                g.Flush();
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }
            }

            return mergedImage;
        }
    }
}
