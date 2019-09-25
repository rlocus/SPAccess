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
            string query = new SpEntitySet<Employee>().Where(employee => employee.Position == EmployeePosition.Manager).Caml(true, true);

            //string query = new SpEntitySet<Employee>().Where(employee => Equals(employee.Position, EmployeePosition.Manager)).Caml(true, true);
            Managers = new SpEntitySet<Employee>(query);
        }

        [CalculatedField(Name = "FullName", Title = "Full Name", Order = 0, Formula = "=CONCATENATE([FirstName], \" \", [LastName])", FieldRefs = new[] { "FirstName", "LastName" })]
        public string FullName
        {
            get;
            set;
        }

        [Field(Name = "FirstName", Title = "First Name", Order = 1, DataType = FieldType.Text)]
        public string FirstName
        {
            get;
            set;
        }

        [Field(Name = "LastName", Title = "Last Name", Order = 2, DataType = FieldType.Text)]
        public string LastName
        {
            get;
            set;
        }     

        [Field(Name = "Position", Title = "Position", Order = 3, DataType = FieldType.Choice)]
        public EmployeePosition Position
        {
            get;
            set;
        }

        [Field(Name = "Phone", DataType = FieldType.Text, Order = 4)]
        public string Phone
        {
            get;
            set;
        }

        [Field(Name = "Email", DataType = FieldType.Text, Order = 5)]
        public string Email
        {
            get;
            set;
        }

        [Field(Name = "Manager", Title = "Manager", DataType = FieldType.Lookup, Order = 6)]
        public ISpEntityLookup<Employee> Manager
        {
            get;
            //set;
        }

        public ISpEntitySet<Employee> Managers
        {
            get;
        }
    }

    public enum EmployeePosition
    {
        [Choice(Value = "Specialist", Index = 0)]
        Specialist = 0,
        [Choice(Value = "Manager", Index = 1)]
        Manager = 1
    }
}
