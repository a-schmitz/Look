// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FavoritesService.cs" company="">
//   
// </copyright>
// <summary>
//   The favorites service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace look.sender.wpf.Services
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using look.sender.wpf.Models;

    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// The favorites service.
    /// </summary>
    public class FavoritesService
    {
        #region Fields

        /// <summary>
        /// The file name.
        /// </summary>
        private readonly string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.look.favorites";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get favorites.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Favorite> GetFavorites() { return this.loadFavoritesFromFile(fileName); }

        /// <summary>
        /// The update favorites.
        /// </summary>
        /// <param name="hosts">
        /// The hosts.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public void UpdateFavorites(IEnumerable<Favorite> hosts) {
            this.saveFavoritesToFile(this.fileName, hosts);
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// The load favorites from file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        private IEnumerable<Favorite> loadFavoritesFromFile(string fileName) {
            if (!File.Exists(fileName))
                return new List<Favorite>();

            using (var stream = new StreamReader(fileName)) {
                var hosts = JsonConvert.DeserializeObject<IEnumerable<Favorite>>(stream.ReadToEnd());
                return hosts;
            }
        }

        /// <summary>
        /// The save favorites to file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="hosts">
        /// The hosts.
        /// </param>
        private void saveFavoritesToFile(string fileName, IEnumerable<Favorite> hosts) {
            using (var stream = new StreamWriter(fileName, false)) {
                
                 stream.WriteLine(JsonConvert.SerializeObject(hosts));
                
            }
        }

        #endregion
    }

}