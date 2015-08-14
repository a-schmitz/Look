namespace look.communication.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    using look.common.Events;
    using look.communication.Contracts;
    using look.communication.Helper.Command;
    using look.communication.Model;
    using look.utils;

    public class ViewService : IViewService
    {

        private string GetHost() {
            try {
                return Dns.GetHostEntry((OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty).Address).HostName;
            } catch {
                return null;
            }
        }

        public bool Connect() {
            var host = this.GetHost();
            if (string.IsNullOrEmpty(host))
                return false;

            var e = new HostConnectedEventArgs { Host = host };
            this.RaiseOnHostConnected(e);

            return e.Accepted;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void PushAvailableWindows(List<Window> windows)
        {
            throw new NotImplementedException();
        }

        public void PushScreenUpdate(byte[] data)
        {
            if (data == null)
                return;

            Image partial;
            Rectangle bounds;
            Guid id;
            Utils.UnpackScreenCaptureData(data, out partial, out bounds, out id);

            ViewSession viewSession;
            if (!_sessions.ContainsKey(id))
            {
                viewSession = new ViewSession { Id = id };
                _sessions[id] = viewSession;
            }
            else
            {
                viewSession = _sessions[id];
            }

            Utils.UpdateScreen(ref viewSession.Screen, partial, bounds);

            UpdateScreenImage(id);
        }

        public string PushCursorUpdate(byte[] data)
        {
            if (data != null)
            {
                Image cursor;
                int cursorX, cursorY;
                Guid id;
                Utils.UnpackCursorCaptureData(data, out cursor, out cursorX, out cursorY, out id);

                ViewSession viewSession;
                if (!_sessions.ContainsKey(id))
                {
                    viewSession = new ViewSession { Id = id };
                    _sessions[id] = viewSession;
                }
                else
                {
                    viewSession = _sessions[id];
                }

                viewSession.Cursor = cursor;
                viewSession.CursorX = cursorX;
                viewSession.CursorY = cursorY;
                UpdateScreenImage(id);
            }

            return Commands.SerializeCommandStack();
        }

        public void RequestWindowTransfer(List<Window> windows)
        {
            throw new NotImplementedException();
        }

        #region Events

        public delegate void HostConnectedHandler(object sender, HostConnectedEventArgs e);
        public static event HostConnectedHandler OnHostConnected;

        public delegate void ImageChangeHandler(Image display, string id, string host);
        public static event ImageChangeHandler OnImageChange;

        #endregion

        private static readonly Dictionary<Guid, ViewSession> _sessions = new Dictionary<Guid, ViewSession>();        
        public static CommandInfoCollection Commands = new CommandInfoCollection();

        private static void UpdateScreenImage(Guid id)
        {
            var viewSession = _sessions[id];
            if (viewSession == null)
            {
                return;
            }
            if (viewSession.Screen == null)
            {
                return;
            }

            if (viewSession.Cursor != null)
            {
                viewSession.Display = Utils.MergeScreenAndCursor(viewSession.Screen, viewSession.Cursor, viewSession.CursorX, viewSession.CursorY);
            }
            else
            {
                viewSession.Display = viewSession.Screen;
            }

            if (OnImageChange != null)
            {
                OnImageChange(viewSession.Display, viewSession.Id.ToString(), viewSession.Host);
            }
        }


        private void RaiseOnHostConnected(HostConnectedEventArgs e)
        {
            if (OnHostConnected != null)
            {
                OnHostConnected(this, e);
            }
        }
    }
}
