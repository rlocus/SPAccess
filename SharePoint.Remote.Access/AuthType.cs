namespace SharePoint.Remote.Access
{
    /// <summary>
    /// Authentication type used for connecting to the site collection.
    /// </summary>
    public enum AuthType
    {
        /// <summary>
        /// Default authentication for Windows credentials (like DOMAIN\Username).
        /// </summary>
        Default = 0,

        /// <summary>
        /// Microsoft SharePoint Online (Office 365) authentication.
        /// </summary>
        SharePointOnline = 1,

        /// <summary>
        /// Anonymous authentication.
        /// </summary>
        Anonymous = 2,

        /// <summary>
        /// Forms Based authentication.
        /// </summary>
        Forms = 3
    }
}