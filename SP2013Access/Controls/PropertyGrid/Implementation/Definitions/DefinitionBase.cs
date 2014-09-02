using SP2013Access.Controls.Utilities;
using System;
using System.Linq.Expressions;
using System.Windows;

namespace SP2013Access.Controls.PropertyGrid
{
    public abstract class DefinitionBase : DependencyObject
    {
        internal bool IsLocked { get; private set; }

        internal void ThrowIfLocked<TMember>(Expression<Func<TMember>> propertyExpression)
        {
            if (this.IsLocked)
            {
                string propertyName = ReflectionHelper.GetPropertyOrFieldName(propertyExpression);
                string message = string.Format(
                    @"Cannot modify {0} once the definition has beed added to a collection.",
                    propertyName);
                throw new InvalidOperationException(message);
            }
        }

        internal virtual void Lock()
        {
            if (!IsLocked)
            {
                IsLocked = true;
            }
        }
    }
}