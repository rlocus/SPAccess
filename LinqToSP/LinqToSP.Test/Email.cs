using Microsoft.SharePoint.Client;
using SP.Client.Linq;
using SP.Client.Linq.Attributes;
using System.Collections.Generic;

namespace LinqToSP.Test
{
    [ContentType(Name = "Email")]
    [List(Url = "Lists/TestEmails", Title = "Test Emails")]
    class Email : ListItemEntity
    {
        public Email()
        {
            Accounts = new SpEntitySet<Account>("Accounts");
            Account = new SpEntityLookup<Account>("Accounts");
            Contacts = new SpEntityLookupCollection<Contact>("Contacts");
        }

        [LookupField("Account", Required = false)]
        //public FieldLookupValue Account { get; set; }

        public SpEntityLookup<Account> Account { get; set; }

        [DependentLookupField(ShowField = "WorkCity", List = "a1", LookupFieldName = "Account", Result = LookupItemResult.Value)]
        public string AccountCity { get; set; }

        //[LookupField("Contact", Required = false, IsLookupId = false, IsMultiple = true)]
        //public FieldLookupValue[] Contact { get; set; }

        [LookupField("Contact", Required = false, Result = LookupItemResult.None, IsMultiple = true)]
        public /*FieldLookupValue[]*/ string[] Contact { get; set; }

        public SpEntityLookupCollection<Contact> Contacts { get; set; }

        [LookupField("Contact", Required = false, Result = LookupItemResult.Id, IsMultiple = true)]
        public List<int> ContactId { get; set; }

        [LookupField("Contact", Required = false, Result = LookupItemResult.Value, IsMultiple = true)]
        public List<string> ContactValue { get; set; }

        [LookupField("AssignedTo", IsMultiple = true)]
        public FieldUserValue[] AssignedTo { get; set; }

        public SpEntitySet<Account> Accounts { get; }
    }

    [List(Url ="Lists/Accounts")]
    public class Account : ListItemEntity
    {
    }

    public class Contact : ListItemEntity
    {
    }
}

