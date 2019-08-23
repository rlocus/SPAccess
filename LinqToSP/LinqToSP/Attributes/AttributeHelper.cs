using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Attributes
{
  internal static class AttributeHelper
  {
    public static IEnumerable<KeyValuePair<string, TAttribute>> GetFieldAttributes<TEntity, TAttribute>()
      where TAttribute: Attribute
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

    public static IEnumerable<KeyValuePair<string, TAttribute>> GetPopertyAttributes<TEntity, TAttribute>()
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

  }
}
