namespace look.capture.Helper
{

    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using look.capture.Unmanaged;

    class Capture
    {
        public struct SIZE
        {
            public int Cx;
            public int Cy;
        }

        public static Bitmap CaptureDesktop()
        {
            Bitmap bmp = null;

            var hDC = IntPtr.Zero;
            try
            {
                SIZE size;
                hDC = WIN32.GetDC(WIN32.GetDesktopWindow());
                var hMemDC = GDI32.CreateCompatibleDC(hDC);

                size.Cx = WIN32.GetSystemMetrics(WIN32.SM_CXSCREEN);
                size.Cy = WIN32.GetSystemMetrics(WIN32.SM_CYSCREEN);

                var hBitmap = GDI32.CreateCompatibleBitmap(hDC, size.Cx, size.Cy);

                if (hBitmap != IntPtr.Zero)
                {
                    var hOld = GDI32.SelectObject(hMemDC, hBitmap);

                    GDI32.BitBlt(hMemDC, 0, 0, size.Cx, size.Cy, hDC, 0, 0, GDI32.SRCCOPY);
                    GDI32.SelectObject(hMemDC, hOld);
                    GDI32.DeleteDC(hMemDC);

                    bmp = Image.FromHbitmap(hBitmap);

                    GDI32.DeleteObject(hBitmap);
                    GC.Collect();
                }
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                {
                    WIN32.ReleaseDC(WIN32.GetDesktopWindow(), hDC);
                }
            }

            return bmp;
        }

        public static Bitmap CaptureCursor(ref int x, ref int y)
        {
            Bitmap bmp = null;
            var ci = new WIN32.CURSORINFO();
            ci.cbSize = Marshal.SizeOf(ci);

            if (!WIN32.GetCursorInfo(out ci) || ci.flags != WIN32.CURSOR_SHOWING)
                return bmp;

            var hicon = WIN32.CopyIcon(ci.hCursor);
            WIN32.ICONINFO icInfo;

            if (WIN32.GetIconInfo(hicon, out icInfo))
            {
                if (icInfo.hbmMask != IntPtr.Zero)
                    GDI32.DeleteObject(icInfo.hbmMask);
                
                if (icInfo.hbmColor != IntPtr.Zero)
                    GDI32.DeleteObject(icInfo.hbmColor);

                x = ci.ptScreenPos.x - icInfo.xHotspot;
                y = ci.ptScreenPos.y - icInfo.yHotspot;

                var ic = Icon.FromHandle(hicon);
                if (ic.Width > 0 && ic.Height > 0)
                    bmp = ic.ToBitmap();
                
                WIN32.DestroyIcon(hicon);
            }

            return bmp;
        }
    }
}
