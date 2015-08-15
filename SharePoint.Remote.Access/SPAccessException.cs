using System;

namespace SharePoint.Remote.Access
{
    public class SPAccessException : Exception
    {
        public SPAccessException(string message)
            : base(message)
        {
        }
    }
}