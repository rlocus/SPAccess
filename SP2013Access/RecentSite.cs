using SharePoint.Remote.Access;
using System.Xml.Serialization;

namespace SP2013Access
{
    [XmlRoot("RecentSite")]
    public class RecentSite
    {
        /// <summary>
        /// Gets and sets the site collection URL as string.
        /// </summary>
        [XmlAttribute()]
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the site collection URL as string.
        /// </summary>
        [XmlAttribute()]
        public bool UseCurrentUserCredentials
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the username for authentication with the site collection.
        /// </summary>
        [XmlAttribute()]
        public string UserName
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets and sets the password.
        ///// </summary>
        //[XmlIgnore()]
        //public string Password
        //{
        //    get; set;
        //}

        /// <summary>
        /// Authentication mode used for authentication with the SharePoint.
        /// </summary>
        [XmlAttribute()]
        public AuthType Authentication
        {
            get;
            set;
        }
    }
}