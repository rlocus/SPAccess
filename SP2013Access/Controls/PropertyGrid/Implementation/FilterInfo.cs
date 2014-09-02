using System;

namespace SP2013Access.Controls.PropertyGrid
{
    internal struct FilterInfo
    {
        public string InputString;
        public Predicate<object> Predicate;
    }
}