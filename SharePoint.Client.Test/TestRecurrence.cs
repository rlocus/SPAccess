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
            RecurrenceRule rule = SPRecurrenceHelper.ParseRule("<recurrence><rule><firstDayOfWeek>su</firstDayOfWeek><repeat><monthlyByDay we=\"TRUE\" weekdayOfMonth=\"third\" monthFrequency=\"2\" /></repeat><repeatForever> FALSE </repeatForever></rule></recurrence>");
        }
    }
}
