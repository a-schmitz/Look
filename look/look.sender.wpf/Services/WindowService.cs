// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowService.cs" company="">
//   
// </copyright>
// <summary>
//   The window service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Services
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;

    using look.sender.wpf.Interfaces;
    using look.sender.wpf.Models;

    /// <summary>
    ///     The window service.
    /// </summary>
    public class WindowService : IWindowService
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The get shareable windows.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<ShareableWindow> GetShareableWindows()
        {
            // foreach (var p in Process.GetProcesses().Where(pr => pr.MainWindowTitle != ""))
            // {
            // Debug.WriteLine(p.MainModule.FileVersionInfo.ProductName);
            // }
            var list =
                Process.GetProcesses()
                    .Where(process => !string.IsNullOrEmpty(process.MainWindowTitle))
                    .Select(
                        p =>
                        new ShareableWindow()
                            {
                                Handle = p.MainWindowHandle, 
                                ProcessName = p.ProcessName, 
                                Title = p.MainWindowTitle, 
                                Id = p.Id, 
                                ProcessPath = p.MainModule.FileName, 
                                Application = p.MainModule.FileVersionInfo.ProductName
                            })
                    .ToList();

            list.ForEach(
                p =>
                    {
                        using (var sysicon = Icon.ExtractAssociatedIcon(p.ProcessPath))
                        {
                            if (sysicon != null)
                            {
                                // using (var s = new FileStream(string.Format("c:\\temp\\{0}.ico", p.Application), FileMode.CreateNew))
                                // sysicon.Save(s);
                                p.Icon = Imaging.CreateBitmapSourceFromHIcon(
                                    sysicon.Handle, 
                                    Int32Rect.Empty, 
                                    BitmapSizeOptions.FromEmptyOptions());
                            }
                        }
                    });

            return list;
        }

        #endregion
    }
}