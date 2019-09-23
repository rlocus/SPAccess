using Microsoft.SharePoint.Client;
using SP.Client.Linq;
using SP.Client.Linq.Attributes;
using System.Linq;

namespace LinqToSP.Test.Model
{
    [List(Title = "Employees", Url = "Lists/Employees")]
    public class Employee : ListItemEntity
    {
        public Employee()
        {
            Manager = new SpEntityLookup<Employee>();
            string query = new SpEntitySet<Employee>().Where(employee => employee.IsManager).Query();
            Managers = new SpEntitySet<Employee>(query);
        }

        [Field(Name = "FirstName", Title = "First Name", DataType = FieldType.Text)]
        public string FirstName
        {
            get;
            set;
        }

        [Field(Name = "LastName", Title = "Last Name", DataType = FieldType.Text)]
        public string LastName
        {
            get;
            set;
        }

        [Field(Name = "Phone", DataType = FieldType.Text)]
        public string Phone
        {
            get;
            set;
        }

        [Field(Name = "Email", DataType = FieldType.Text)]
        public string Email
        {
            get;
            set;
        }

        [Field(Name = "IsManager", Title = "Is Manager", DataType = FieldType.Boolean)]
        public bool IsManager
        {
            get;
            set;
        }

        public ISpEntityLookup<Employee> Manager
        {
            get;
        }

        public ISpEntitySet<Employee> Managers
        {
            get;
        }
    }
}
