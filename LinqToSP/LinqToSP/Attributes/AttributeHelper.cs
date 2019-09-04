using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Attributes
{
    internal static class AttributeHelper
    {
        public static IEnumerable<KeyValuePair<string, TAttribute>> GetFieldAttributes<TEntity, TAttribute>()
          where TAttribute : Attribute
        {
            foreach (var field in typeof(TEntity).GetFields())
            {
                var att = (TAttribute)Attribute.GetCustomAttribute(field, typeof(TAttribute), true);
                if (att != null)
                {
                    yield return new KeyValuePair<string, TAttribute>(field.Name, att);
                }
                else
                {
                    foreach (var f in typeof(TEntity).GetInterfaces().SelectMany(i => i.GetFields()))
                    {
                        if (f.Name == field.Name)
                        {
                            att = (TAttribute)Attribute.GetCustomAttribute(f, typeof(TAttribute), true);
                            if (att != null)
                            {
                                yield return new KeyValuePair<string, TAttribute>(f.Name, att);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, TAttribute>> GetPropertyAttributes<TEntity, TAttribute>()
          where TAttribute : Attribute
        {
            foreach (var property in typeof(TEntity).GetProperties())
            {
                var att = (TAttribute)Attribute.GetCustomAttribute(property, typeof(TAttribute), true);
                if (att != null)
                {
                    yield return new KeyValuePair<string, TAttribute>(property.Name, att);
                }
                else
                {
                    foreach (var p in typeof(TEntity).GetInterfaces().SelectMany(i => i.GetProperties()))
                    {
                        if (p.Name == property.Name)
                        {
                            att = (TAttribute)Attribute.GetCustomAttribute(p, typeof(TAttribute), true);
                            if (att != null)
                            {
                                yield return new KeyValuePair<string, TAttribute>(p.Name, att);
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, object>> GetPropertyValues<TEntity, TAttribute>(TEntity entity)
            where TAttribute : Attribute
        {
            if (entity != null)
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    var att = (TAttribute)Attribute.GetCustomAttribute(property, typeof(TAttribute), true);
                    if (att != null)
                    {
                        yield return new KeyValuePair<string, object>(property.Name, property.GetValue(entity));
                    }
                    else
                    {
                        foreach (var p in typeof(TEntity).GetInterfaces().SelectMany(i => i.GetProperties()))
                        {
                            if (p.Name == property.Name)
                            {
                                att = (TAttribute)Attribute.GetCustomAttribute(p, typeof(TAttribute), true);
                                if (att != null)
                                {
                                    yield return new KeyValuePair<string, object>(p.Name, property.GetValue(entity));
                                }
                            }
                        }
                    }
                }
        }

        public static IEnumerable<KeyValuePair<string, object>> GetFieldValues<TEntity, TAttribute>(TEntity entity)
            where TAttribute : Attribute
        {
            if (entity != null)
                foreach (var field in typeof(TEntity).GetFields())
                {
                    var att = (TAttribute)Attribute.GetCustomAttribute(field, typeof(TAttribute), true);
                    if (att != null)
                    {
                        yield return new KeyValuePair<string, object>(field.Name, field.GetValue(entity));
                    }
                    else
                    {
                        foreach (var f in typeof(TEntity).GetInterfaces().SelectMany(i => i.GetFields()))
                        {
                            if (f.Name == field.Name)
                            {
                                att = (TAttribute)Attribute.GetCustomAttribute(f, typeof(TAttribute), true);
                                if (att != null)
                                {
                                    yield return new KeyValuePair<string, object>(f.Name, field.GetValue(entity));
                                }
                            }
                        }
                    }
                }
        }

    }
}
