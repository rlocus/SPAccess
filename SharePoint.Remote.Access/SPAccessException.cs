using System;

namespace SharePoint.Remote.Access
{
    [Serializable]
    public class SPAccessException : Exception
    {
        public SPAccessException(string message)
            : base(message)
        {
        }
    }
}