using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SharePoint.Remote.Access.Helpers;

namespace SP2013Access
{
    [XmlRoot("Configuration")]
    public sealed class ConfigManager
    {
        public ConfigManager()
        {
            RecentSites = new List<RecentSite>();
        }

        [XmlArray("RecentSites")]
        public List<RecentSite> RecentSites { get; private set; }

        /// <summary>
        /// </summary>
        public void Add(SPClientContext clientContext)
        {
            var recentSite = RecentSites.SingleOrDefault(site => site.Url != null && site.Url.Equals(clientContext.Url));
            if (clientContext != null)
            {
                if (recentSite != null)
                {
                    RecentSites.Remove(recentSite);
                }
                RecentSites.Add(new RecentSite
                {
                    Authentication = clientContext.Authentication,
                    Url = clientContext.Url,
                    UserName = clientContext.UserName
                });
            }
        }

        /// <summary>
        ///     Removes site from collection.
        /// </summary>
        public void Remove(SPClientContext clientContext)
        {
            if (clientContext != null)
            {
                foreach (var site in RecentSites)
                {
                    if (site.Url.Equals(clientContext.Url))
                    {
                        RecentSites.Remove(site);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Loads previous loaded site collections from configuration file.
        /// </summary>
        public void Load()
        {
            var config = OpenAndRead(Constants.CONFIG_FILENAME);

            if (config?.RecentSites != null)
            {
                RecentSites.AddRange(config.RecentSites);
            }
        }

        /// <summary>
        ///     Saves the current set of site collections to configuration file.
        /// </summary>
        public void Save()
        {
            Write(Constants.CONFIG_FILENAME, this);
        }

        private static ConfigManager OpenAndRead(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    // Create the serializer
                    var serializer = new XmlSerializer(typeof (ConfigManager));

                    // Open config file
                    using (var stream = new StreamReader(fileName))
                    {
                        // De-serialize the XML
                        return serializer.Deserialize(stream) as ConfigManager;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Can't read the configuration, 'Recent Sites' is empty.", ex);
            }

            return null;
        }

        private static void Write(string fileName, ConfigManager config)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                // Create the serializer
                var serializer = new XmlSerializer(typeof (ConfigManager));

                // Open config file
                using (var stream = new StreamWriter(fileName))
                {
                    // Serialize the XML
                    serializer.Serialize(stream, config);
                }
            }
        }
    }
}