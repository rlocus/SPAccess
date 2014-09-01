using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

namespace SP2013Access.Controls.Utilities
{
    internal class LocalizationHelper
    {
        private struct ResourcesGetters
        {
            public Func<ResourceManager> ResourceManager;
            public Func<CultureInfo> Culture;
        }
        private readonly Type _resourceClassType;
        private static Dictionary<Type, LocalizationHelper.ResourcesGetters> _getters;
        static LocalizationHelper()
        {
            LocalizationHelper._getters = new Dictionary<Type, LocalizationHelper.ResourcesGetters>();
        }
        public LocalizationHelper(Type resourceClassType)
        {
            if (resourceClassType == null)
            {
                throw new ArgumentNullException("resourceClassType");
            }
            this._resourceClassType = resourceClassType;
            if (!LocalizationHelper._getters.ContainsKey(this._resourceClassType))
            {
                Func<ResourceManager> resourceManager = this.CreateGetter<ResourceManager>("ResourceManager");
                Func<CultureInfo> culture = this.CreateGetter<CultureInfo>("Culture");
                LocalizationHelper.ResourcesGetters value = new LocalizationHelper.ResourcesGetters
                {
                    ResourceManager = resourceManager,
                    Culture = culture
                };
                LocalizationHelper._getters.Add(this._resourceClassType, value);
            }
        }
        public string GetString(string resourceKey)
        {
            LocalizationHelper.ResourcesGetters resourcesGetters = LocalizationHelper._getters[this._resourceClassType];
            return resourcesGetters.ResourceManager().GetString(resourceKey, resourcesGetters.Culture());
        }
        private Func<T> CreateGetter<T>(string propertyName)
        {
            Func<T> result;
            try
            {
                PropertyInfo property = this._resourceClassType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                MemberExpression body = Expression.Property(null, property);
                Func<T> func = (Func<T>)Expression.Lambda(body, new ParameterExpression[0]).Compile();
                result = func;
            }
            catch (Exception innerException)
            {
                string message = string.Format("Type must implement a static property named '{0}' that return a {1} instance.", propertyName, typeof(T));
                throw new ArgumentException(message, "resourceClassType", innerException);
            }
            return result;
        }
    }
}
