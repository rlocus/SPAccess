using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharePoint.Remote.Access.Helpers;

namespace SharePoint.Client.Test
{
    [TestClass]
    public class TestRecurrence
    {
        [TestMethod]
        public void TestRecurrence1()
        {
            RecurrenceRule rule = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(10), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><daily dayFrequency='1'/></repeat><windowEnd>2016-01-02T23:00:00Z</windowEnd></rule></recurrence>");

            var occurences1 = rule.GetOccurrences().ToArray();
            //var occurences = rule.GetOccurrences().ToArray();

            //every 1 week on a specific day, end after 10 occurrences:
            RecurrenceRule rule1 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(2), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><weekly tu='TRUE' weekFrequency='2' /></repeat><repeatInstances>0</repeatInstances><windowEnd>2016-01-02T23:00:00Z</windowEnd></rule></recurrence>");

            var occurences2 = rule1.GetOccurrences().ToArray();

            //Recurring every 1 week on a specific day, specific end date
            RecurrenceRule rule2 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(1), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><weekly tu='TRUE' weekFrequency='1' /></repeat><windowEnd>2012-04-26T20:00:00Z</windowEnd></rule></recurrence>");
            //every 2 weeks on a specific day, no end date:
            RecurrenceRule rule3 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(1), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><weekly tu='TRUE' weekFrequency='2' /></repeat><repeatForever>FALSE</repeatForever></rule></recurrence>");

            var occurences3 = rule3.GetOccurrences().ToArray();
            //on a specific date of the month, no end date:
            RecurrenceRule rule4 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(1), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><monthly monthFrequency='4' day='1' /></repeat><repeatForever>FALSE</repeatForever><repeatInstances>10</repeatInstances></rule></recurrence>");
            var occurences4 = rule4.GetOccurrences().ToArray();

            //on a specific day every 3 months, end on a specific date:
            RecurrenceRule rule5 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(1), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><monthlyByDay tu='TRUE' weekdayOfMonth='first' monthFrequency='4' /></repeat><windowEnd>2016-04-26T20:00:00Z</windowEnd></rule></recurrence>");

            var occurences5 = rule5.GetOccurrences().ToArray();

            //annually on a specific date, no end date:
            RecurrenceRule rule6 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(3), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><yearly yearFrequency='2' month='4' day='26' /></repeat><repeatForever>FALSE</repeatForever></rule></recurrence>");

            var occurences6 = rule6.GetOccurrences().ToArray();

            //annually on a specific date, ends on a specific date:
            RecurrenceRule rule7 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddYears(1), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><yearly yearFrequency='1' month='4' day='26' /></repeat><windowEnd>2012-04-26T20:00:00Z</windowEnd></rule></recurrence>");
            //Recurring annually on a specific day, ends after 10 occurrences:
            RecurrenceRule rule8 = SPRecurrenceHelper.ParseRule(DateTime.Now, DateTime.Now.AddHours(2).AddYears(8), "<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><yearlyByDay yearFrequency='1' tu='TRUE' weekdayOfMonth='first' month='4' /></repeat><repeatInstances>10</repeatInstances></rule></recurrence>");

            var occurences8 = rule8.GetOccurrences().ToArray();
        }
    }
}
