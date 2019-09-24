using Microsoft.SharePoint.Client;
using SP.Client.Linq.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Client.Linq.Provisioning
{
  public sealed class FieldProvisionHandler<TContext, TEntity> : SpProvisionHandler<TContext, TEntity>
      where TContext : class, ISpEntryDataContext
      where TEntity : class, IListItemEntity
  {
    private readonly FieldAttribute _field;
    private readonly ContentTypeAttribute _contentType;
    private readonly ListAttribute _list;
    private readonly Type _valueType;

    public event Action<FieldProvisionHandler<TContext, TEntity>, Field> OnProvisioning;

    public event Action<FieldProvisionHandler<TContext, TEntity>, Field> OnProvisioned;

    public FieldProvisionHandler(FieldAttribute field, ListAttribute list, SpProvisionModel<TContext, TEntity> model, Type valueType) : this(field, null, list, model, valueType)
    {
    }

    public FieldProvisionHandler(FieldAttribute field, ContentTypeAttribute contentType, SpProvisionModel<TContext, TEntity> model, Type valueType) : this(field, contentType, null, model, valueType)
    {
    }

    public FieldProvisionHandler(FieldAttribute field, ContentTypeAttribute contentType, ListAttribute list, SpProvisionModel<TContext, TEntity> model, Type valueType) : base(model)
    {
      _field = field;
      _contentType = contentType;
      _list = list;
      _valueType = valueType;
    }

    private List GetLookupList(Type lookupEntityType)
    {
      var listAtt = AttributeHelper.GetCustomAttributes<ListAttribute>(lookupEntityType, false).FirstOrDefault();
      if (listAtt != null)
      {
        var context = Model.Context.Context;
        var list = listAtt.Title != null
                   ? context.Web.Lists.GetByTitle(listAtt.Title)
                   : (listAtt.Url != null ? context.Web.GetList($"{ Model.Context.SiteUrl.TrimEnd('/')}/{listAtt.Url.TrimStart('/')}") : null);
        if (list != null)
        {
          context.Load(list);
          context.ExecuteQuery();
        }
        return list;
      }
      return null;
    }

    public override void Provision()
    {
      if (_field != null && Model != null && Model.Context != null && Model.Context.Context != null)
      {
        var context = Model.Context.Context;
        Web web = context.Web;
        List list = null;
        ContentType contentType = null;
        Field field = null;

        if (_list != null)
        {
          list = _list.Title != null
              ? context.Web.Lists.GetByTitle(_list.Title)
              : (_list.Url != null ? context.Web.GetList($"{ Model.Context.SiteUrl.TrimEnd('/')}/{_list.Url.TrimStart('/')}") : null);
        }

        if (_contentType != null)
        {
          string ctName = _contentType.Name;
          if (string.IsNullOrEmpty(ctName))
          {
            string ctId = _contentType.Id;
            if (!string.IsNullOrEmpty(ctId))
            {
              IEnumerable<ContentType> webContentTypes = context.LoadQuery(web.AvailableContentTypes.Where(ct => ct.Id.StringValue == ctId));
              IEnumerable<ContentType> listContentTypes = null;
              if (list != null)
              {
                listContentTypes = context.LoadQuery(list.ContentTypes.Where(ct => ct.Id.StringValue == ctId));
              }

              context.ExecuteQuery();

              contentType = webContentTypes.FirstOrDefault();
              if (listContentTypes != null)
              {
                contentType = listContentTypes.FirstOrDefault();
              }
            }
          }
          else
          {
            IEnumerable<ContentType> webContentTypes = context.LoadQuery(web.AvailableContentTypes.Where(ct => ct.Name == ctName));
            IEnumerable<ContentType> listContentTypes = null;
            if (list != null)
            {
              listContentTypes = context.LoadQuery(list.ContentTypes.Where(ct => ct.Name == ctName));
            }

            context.ExecuteQuery();

            contentType = webContentTypes.FirstOrDefault();
            if (listContentTypes != null)
            {
              contentType = listContentTypes.FirstOrDefault();
            }
          }
        }

        string fieldXml = $"<Field Type='{_field.DataType}' Name='{_field.Name}' StaticName='{_field.Name}' DisplayName='{_field.Title ?? _field.Name}' />";

        var fields = list != null ? list.Fields : web.Fields;
        field = fields.GetByInternalNameOrTitle(_field.Name);
        try
        {
          context.Load(field);
          context.ExecuteQuery();
        }
        catch
        {
          field = null;
        }

        if (field == null)
        {
          field = fields.AddFieldAsXml(fieldXml, true, AddFieldOptions.AddFieldInternalNameHint);
          field.FieldTypeKind = _field.DataType;
          field.Required = _field.Required;
          field.ReadOnlyField = _field.IsReadOnly;

          if (_field.DataType == FieldType.Lookup)
          {
            var lookupField = context.CastTo<FieldLookup>(field);
            if (typeof(LookupFieldAttribute).IsAssignableFrom(_field.GetType()))
            {
              lookupField.AllowMultipleValues = (_field as LookupFieldAttribute).IsMultiple;
            }

            if (_valueType != null && typeof(ISpEntityLookup).IsAssignableFrom(_valueType) || typeof(ISpEntityLookupCollection).IsAssignableFrom(_valueType))
            {
              Type lookupEntityType = _valueType.GenericTypeArguments.FirstOrDefault();
              var lookupList = GetLookupList(lookupEntityType);
              if (lookupList != null)
              {
                lookupField.LookupList = lookupList.Id.ToString();
                lookupField.LookupField = "Title";
              }
            }
            OnProvisioning?.Invoke(this, lookupField);
          }
          else if ((_field.DataType == FieldType.Choice || _field.DataType == FieldType.MultiChoice) && _valueType.IsEnum)
          {
            var choiceField = context.CastTo<FieldChoice>(field);
            var choices = AttributeHelper.GetFieldAttributes<ChoiceAttribute>(_valueType).Select(choice => choice.Value);
            choiceField.Choices = choices.OrderBy(choice => choice.Index).Select(choice => choice.Value).ToArray();
            OnProvisioning?.Invoke(this, choiceField);
          }
          else
          {
            OnProvisioning?.Invoke(this, field);
          }
          field.Update();
          context.Load(field);
          context.ExecuteQuery();
        }

        if (contentType != null)
        {
          Guid fieldId = field.Id;
          var fieldLink = contentType.FieldLinks.GetById(fieldId);

          try
          {
            context.Load(contentType.FieldLinks);
            context.Load(fieldLink);
            context.ExecuteQuery();
            fieldLink = contentType.FieldLinks.FirstOrDefault(f => f.Id == fieldId);
          }
          catch
          {
            fieldLink = null;
          }

          if (fieldLink == null)
          {
            fieldLink = contentType.FieldLinks.Add(new FieldLinkCreationInformation() { Field = field });
            contentType.Update(false);
            context.ExecuteQuery();
          }
        }

        if (field != null)
        {
          OnProvisioned?.Invoke(this, field);
        }
      }
    }
  }
}

