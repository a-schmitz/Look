namespace look.communication.Model
{
    using System;
    using System.Drawing;

    public class ViewSession
    {
        public Guid Id;
        public Image Screen;
        public Image Cursor;
        public int CursorX;
        public int CursorY;
        public Image Display;
        public string Ip;
    }
}
