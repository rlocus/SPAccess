using LinqToSP.Test.Model;
using Microsoft.SharePoint.Client;
using SP.Client.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToSP.Test
{
  class Program
  {
    static void Main(string[] args)
    {
      string siteUrl = ConfigurationManager.AppSettings["siteUrl"];
      while (string.IsNullOrWhiteSpace(siteUrl))
      {
        Console.WriteLine("Enter Site Url: ");
        siteUrl = Console.ReadLine();
      }

      string userLogin = ConfigurationManager.AppSettings["userLogin"];
      while (string.IsNullOrWhiteSpace(userLogin))
      {
        Console.WriteLine("Enter User Login: ");
        userLogin = Console.ReadLine();
      }

      string userPassword = ConfigurationManager.AppSettings["userPassword"];

      using (var ctx = new SpDataContext(siteUrl))
      {
        var clientContext = ctx.Context;
        clientContext.Credentials = new SharePointOnlineCredentials(userLogin, string.IsNullOrWhiteSpace(userPassword) ? GetPassword() : ConvertToSecureString(userPassword));
        //Deploy(ctx);

        //while (ctx.List<Employee>().Take(100).DeleteAll())
        //{
        //  ctx.SaveChanges();
        //}

        //ctx.List<Employee>().AddOrUpdate(new Employee()
        //{
        //  FirstName = "FirstName 1",
        //  LastName = "LastName 1",
        //  Position = EmployeePosition.Specialist
        //}, 1);

        //ctx.SaveChanges();

        var employee = ctx.List<Employee>().FirstOrDefault();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Done!");
        Console.ResetColor();
      }

      Console.ReadKey();
    }

    private static void Deploy(SpDataContext spContext)
    {
      Console.WriteLine("Deploying...");
      var model = spContext.CreateModel<EmployeeProvisionModel<SpDataContext>, SpDataContext, Employee>();
      model.Provision();
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Done!");
      Console.ResetColor();
    }

    private static SecureString GetPassword()
    {
      Console.WriteLine("Enter Password: ");

      ConsoleKeyInfo info;
      //Get the user's password as a SecureString  
      SecureString securePassword = new SecureString();
      do
      {
        info = Console.ReadKey(true);
        if (info.Key != ConsoleKey.Enter && info.Key != ConsoleKey.LeftArrow && info.Key != ConsoleKey.RightArrow)
        {
          if (info.Key == ConsoleKey.Backspace || info.Key == ConsoleKey.Delete)
          {
            if (securePassword.Length > 0)
            {
              securePassword.RemoveAt(securePassword.Length - 1);
            }
            Console.Write("\b \b");
          }
          else
          {
            securePassword.AppendChar(info.KeyChar);
            Console.Write("*");
          }
        }
      }
      while (info.Key != ConsoleKey.Enter);
      return securePassword;
    }

    private static SecureString ConvertToSecureString(string password)
    {
      if (password == null)
        throw new ArgumentNullException("password");

      var securePassword = new SecureString();

      foreach (char c in password)
        securePassword.AppendChar(c);

      securePassword.MakeReadOnly();
      return securePassword;
    }

  }
}
