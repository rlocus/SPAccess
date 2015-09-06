using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using SharePoint.Remote.Access.Extensions;

namespace SharePoint.Remote.Access.Helpers
{
    public static class SPRecurrenceHelper
    {
        public static RecurrenceRule ParseRule(DateTime startDate, DateTime endDate, string recurrenceData)
        {
            RecurrenceRule recurrenceRule = null;
            var ruleParser = new RecurrenceRuleParser(recurrenceData);
            if (ruleParser.Type.HasValue)
            {
                switch (ruleParser.Type)
                {
                    case RecurrenceType.Daily:
                        var dyilyRecurrenceRule = new DyilyRecurrenceRule
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            dyilyRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        recurrenceRule = dyilyRecurrenceRule;
                        break;
                    case RecurrenceType.Weekly:
                        var weeklyRecurrenceRule = new WeeklyRecurrenceRule
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            weeklyRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        var daysOfWeek = new List<DayOfWeek>(ruleParser.DaysOfWeek.Select(GetDayOfWeek));
                        if (ruleParser.IsDay)
                        {
                            daysOfWeek.Add(DayOfWeek.Day);
                        }
                        if (ruleParser.IsWeekday)
                        {
                            daysOfWeek.Add(DayOfWeek.Weekday);
                        }
                        if (ruleParser.IsWeekendDay)
                        {
                            daysOfWeek.Add(DayOfWeek.WeekendDay);
                        }
                        weeklyRecurrenceRule.DaysOfWeek = daysOfWeek.ToArray();
                        if (ruleParser.FirstDayOfWeek.HasValue)
                        {
                            weeklyRecurrenceRule.FirstDayOfWeek = ruleParser.FirstDayOfWeek.Value;
                        }
                        recurrenceRule = weeklyRecurrenceRule;
                        break;
                    case RecurrenceType.Monthly:
                        var monthlyRecurrenceRule = new MonthlyRecurrenceRule
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            monthlyRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        if (ruleParser.Day != null)
                        {
                            monthlyRecurrenceRule.DayOfMonth = ruleParser.Day.Value;
                        }
                        recurrenceRule = monthlyRecurrenceRule;
                        break;
                    case RecurrenceType.MonthlyByDay:
                        var monthlyByDayRecurrenceRule = new MonthlyByDayRecurrenceRule
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            monthlyByDayRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        if (ruleParser.IsDay)
                        {
                            monthlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.Day;
                        }
                        else if (ruleParser.IsWeekday)
                        {
                            monthlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.Weekday;
                        }
                        else if (ruleParser.IsWeekendDay)
                        {
                            monthlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.WeekendDay;
                        }
                        else if (ruleParser.DaysOfWeek.Length > 0)
                        {
                            monthlyByDayRecurrenceRule.DayOfWeek = GetDayOfWeek(ruleParser.DaysOfWeek.First());
                        }
                        if (ruleParser.Ordinal != null)
                        {
                            monthlyByDayRecurrenceRule.DayOfWeekOrdinal = ruleParser.Ordinal.Value;
                        }
                        recurrenceRule = monthlyByDayRecurrenceRule;
                        break;
                    case RecurrenceType.Yearly:
                        var yearlyRecurrenceRule = new YearlyRecurrenceRule()
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            yearlyRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        if (ruleParser.Month != null)
                        {
                            yearlyRecurrenceRule.Month = (Month)ruleParser.Month;
                        }
                        if (ruleParser.Day != null)
                        {
                            yearlyRecurrenceRule.DayOfMonth = ruleParser.Day.Value;
                        }
                        if (ruleParser.RepeatInstances != null)
                        {
                            yearlyRecurrenceRule.RepeatInstances = ruleParser.RepeatInstances.Value;
                        }
                        recurrenceRule = yearlyRecurrenceRule;
                        break;
                    case RecurrenceType.YearlyByDay:
                        var yearlyByDayRecurrenceRule = new YearlyByDayRecurrenceRule()
                        {
                            StartDate = startDate,
                            EndDate = endDate,
                            HasEnd = ruleParser.WindowEnd.HasValue
                        };
                        if (ruleParser.Frequency != null)
                        {
                            yearlyByDayRecurrenceRule.Interval = ruleParser.Frequency.Value;
                        }
                        if (ruleParser.Month != null)
                        {
                            yearlyByDayRecurrenceRule.Month = (Month)ruleParser.Month;
                        }
                        if (ruleParser.IsDay)
                        {
                            yearlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.Day;
                        }
                        else if (ruleParser.IsWeekday)
                        {
                            yearlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.Weekday;
                        }
                        else if (ruleParser.IsWeekendDay)
                        {
                            yearlyByDayRecurrenceRule.DayOfWeek = DayOfWeek.WeekendDay;
                        }
                        else if (ruleParser.DaysOfWeek.Length > 0)
                        {
                            yearlyByDayRecurrenceRule.DayOfWeek = GetDayOfWeek(ruleParser.DaysOfWeek.First());
                        }
                        if (ruleParser.Ordinal != null)
                        {
                            yearlyByDayRecurrenceRule.DayOfWeekOrdinal = ruleParser.Ordinal.Value;
                        }
                        if (ruleParser.RepeatInstances != null)
                        {
                            yearlyByDayRecurrenceRule.RepeatInstances = ruleParser.RepeatInstances.Value;
                        }
                        recurrenceRule = yearlyByDayRecurrenceRule;
                        break;
                }

                if (recurrenceRule != null)
                {
                    recurrenceRule.StartDate = startDate;
                    if (ruleParser.WindowEnd.HasValue)
                    {
                        recurrenceRule.EndDate = endDate > ruleParser.WindowEnd.Value
                            ? ruleParser.WindowEnd.Value
                            : endDate;
                        recurrenceRule.HasEnd = true;
                    }
                    else
                    {
                        recurrenceRule.EndDate = endDate;
                        recurrenceRule.HasEnd = false;
                    }
                }
            }
            return recurrenceRule;
        }
        
        private static DayOfWeek GetDayOfWeek(System.DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    return DayOfWeek.Sunday;
                case System.DayOfWeek.Monday:
                    return DayOfWeek.Monday;
                case System.DayOfWeek.Tuesday:
                    return DayOfWeek.Tuesday;
                case System.DayOfWeek.Wednesday:
                    return DayOfWeek.Wednesday;
                case System.DayOfWeek.Thursday:
                    return DayOfWeek.Thursday;
                case System.DayOfWeek.Friday:
                    return DayOfWeek.Friday;
                case System.DayOfWeek.Saturday:
                    return DayOfWeek.Saturday;
            }
            return DayOfWeek.Sunday;
        }

        internal enum RecurrenceType
        {
            Daily,
            Weekly,
            Monthly,
            MonthlyByDay,
            Yearly,
            YearlyByDay
        }

        internal class RecurrenceRuleParser
        {
            private const string RecurrenceTag = "recurrence";
            private const string RuleTag = "rule";
            private const string RepeatTag = "repeat";
            private const string WindowEndTag = "windowEnd";
            private const string RepeatInstancesTag = "repeatInstances";
            private const string FirstDayOfWeekTag = "firstDayOfWeek";
            private const string DailyTag = "daily";
            private const string WeeklyTag = "weekly";
            private const string MonthlyTag = "monthly";
            private const string MonthlyByDayTag = "monthlyByDay";
            private const string YearlyTag = "yearly";
            private const string YearlyByDayTag = "yearlyByDay";

            public DateTime? WindowEnd { get; private set; }

            public RecurrenceType? Type { get; private set; }

            public int? Frequency { get; private set; }

            public int? RepeatInstances { get; private set; }

            public bool IsDay { get; private set; }

            public bool IsWeekday { get; private set; }

            public bool IsWeekendDay { get; private set; }

            public int? Day { get; private set; }

            public int? Month { get; private set; }

            public System.DayOfWeek? FirstDayOfWeek { get; private set; }

            public System.DayOfWeek[] DaysOfWeek { get; private set; }

            public DayOfWeekOrdinal? Ordinal { get; private set; }

            private void SetRecurrenceType(XElement repeat)
            {
                XElement daily = repeat.ElementIgnoreCase(DailyTag);
                if (daily != null)
                {
                    if (IsWeekday)
                    {
                        Frequency = 1;
                    }
                    else
                    {
                        XAttribute dayFrequency = daily.AttributeIgnoreCase("dayFrequency");
                        Frequency = (Convert.ToInt32(dayFrequency.Value, CultureInfo.InvariantCulture));
                    }
                    Type = RecurrenceType.Daily;
                    return;
                }
                XElement weekly = repeat.ElementIgnoreCase(WeeklyTag);
                if (weekly != null)
                {
                    XAttribute weekFrequency = weekly.AttributeIgnoreCase("weekFrequency");
                    Frequency = (Convert.ToInt32(weekFrequency.Value, CultureInfo.InvariantCulture));
                    Type = RecurrenceType.Weekly;
                    SetDaysOfWeek(weekly);
                    return;
                }
                XElement monthly = repeat.ElementIgnoreCase(MonthlyTag);
                if (monthly != null)
                {
                    XAttribute monthFrequency = monthly.AttributeIgnoreCase("monthFrequency");
                    Frequency = (Convert.ToInt32(monthFrequency.Value, CultureInfo.InvariantCulture));
                    Type = RecurrenceType.Monthly;
                    return;
                }
                XElement monthlyByDay = repeat.ElementIgnoreCase(MonthlyByDayTag);
                if (monthlyByDay != null)
                {
                    XAttribute monthFrequency = monthlyByDay.AttributeIgnoreCase("monthFrequency");
                    Frequency = (Convert.ToInt32(monthFrequency.Value, CultureInfo.InvariantCulture));
                    Type = RecurrenceType.MonthlyByDay;
                    SetDaysOfWeek(monthlyByDay);
                    SetOrdinal(monthlyByDay);
                    return;
                }
                XElement yearly = repeat.ElementIgnoreCase(YearlyTag);
                if (yearly != null)
                {
                    XAttribute yearFrequency = yearly.AttributeIgnoreCase("yearFrequency");
                    Frequency = (Convert.ToInt32(yearFrequency.Value, CultureInfo.InvariantCulture));
                    Type = RecurrenceType.Yearly;
                    return;
                }
                XElement yearlyByDay = repeat.ElementIgnoreCase(YearlyByDayTag);
                if (yearlyByDay != null)
                {
                    XAttribute yearFrequency = yearlyByDay.AttributeIgnoreCase("yearFrequency");
                    Frequency = (Convert.ToInt32(yearFrequency.Value, CultureInfo.InvariantCulture));
                    Type = RecurrenceType.YearlyByDay;
                    SetDaysOfWeek(yearlyByDay);
                    SetOrdinal(yearlyByDay);
                }
            }

            private void SetDaysOfWeek(XElement recurrByDay)
            {
                List<System.DayOfWeek> days = new List<System.DayOfWeek>();
                XAttribute su = recurrByDay.AttributeIgnoreCase("su");
                if (su != null && Convert.ToBoolean(su.Value))
                {
                    days.Add(System.DayOfWeek.Sunday);
                }
                XAttribute mo = recurrByDay.AttributeIgnoreCase("mo");
                if (mo != null && Convert.ToBoolean(mo.Value))
                {
                    days.Add(System.DayOfWeek.Monday);
                }
                XAttribute tu = recurrByDay.AttributeIgnoreCase("tu");
                if (tu != null && Convert.ToBoolean(tu.Value))
                {
                    days.Add(System.DayOfWeek.Tuesday);
                }
                XAttribute we = recurrByDay.AttributeIgnoreCase("we");
                if (we != null && Convert.ToBoolean(we.Value))
                {
                    days.Add(System.DayOfWeek.Wednesday);
                }
                XAttribute th = recurrByDay.AttributeIgnoreCase("th");
                if (th != null && Convert.ToBoolean(th.Value))
                {
                    days.Add(System.DayOfWeek.Thursday);
                }
                XAttribute fr = recurrByDay.AttributeIgnoreCase("fr");
                if (fr != null && Convert.ToBoolean(fr.Value))
                {
                    days.Add(System.DayOfWeek.Friday);
                }
                XAttribute sa = recurrByDay.AttributeIgnoreCase("sa");
                if (sa != null && Convert.ToBoolean(sa.Value))
                {
                    days.Add(System.DayOfWeek.Saturday);
                }
                DaysOfWeek = days.ToArray();
            }

            private void SetOrdinal(XElement recurrByDay)
            {
                var weekdayOfMonth = recurrByDay.AttributeIgnoreCase("weekdayOfMonth");
                if (weekdayOfMonth != null && (Type == RecurrenceType.MonthlyByDay || Type == RecurrenceType.YearlyByDay))
                {
                    string value = weekdayOfMonth.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (string.Equals(value, "first", StringComparison.OrdinalIgnoreCase))
                        {
                            Ordinal = DayOfWeekOrdinal.First;
                        }
                        else if (string.Equals(value, "second", StringComparison.OrdinalIgnoreCase))
                        {
                            Ordinal = DayOfWeekOrdinal.Second;
                        }
                        else if (string.Equals(value, "third", StringComparison.OrdinalIgnoreCase))
                        {
                            Ordinal = DayOfWeekOrdinal.Third;
                        }
                        else if (string.Equals(value, "fourth", StringComparison.OrdinalIgnoreCase))
                        {
                            Ordinal = DayOfWeekOrdinal.Fourth;
                        }
                        else if (string.Equals(value, "last", StringComparison.OrdinalIgnoreCase))
                        {
                            Ordinal = DayOfWeekOrdinal.Last;
                        }
                    }
                }
            }

            private void SetFirstDayOfWeek(XElement firstDayOfWeek)
            {
                string value = firstDayOfWeek.Value;
                if (string.Equals(value, "su", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Sunday;
                    return;
                }
                if (string.Equals(value, "mo", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Monday;
                    return;
                }
                if (string.Equals(value, "tu", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Tuesday;
                    return;
                }
                if (string.Equals(value, "we", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Wednesday;
                    return;
                }
                if (string.Equals(value, "th", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Thursday;
                    return;
                }
                if (string.Equals(value, "fr", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Friday;
                    return;
                }
                if (string.Equals(value, "sa", StringComparison.OrdinalIgnoreCase))
                {
                    FirstDayOfWeek = System.DayOfWeek.Saturday;
                }
            }

            public RecurrenceRuleParser(string ruleXml)
            {
                XElement recurrence = XElement.Parse(ruleXml);
                if (string.Equals(recurrence.Name.LocalName, RecurrenceTag, StringComparison.OrdinalIgnoreCase))
                {
                    XElement rule = recurrence.ElementIgnoreCase(RuleTag);
                    if (rule != null)
                    {
                        XElement repeat = rule.ElementIgnoreCase(RepeatTag);
                        if (repeat != null)
                        {
                            XAttribute weekendDay = repeat.AttributeIgnoreCase("weekend_day");
                            if (weekendDay != null)
                            {
                                IsWeekendDay = Convert.ToBoolean(weekendDay);
                            }
                            XAttribute weekday = repeat.AttributeIgnoreCase("weekday");
                            if (weekday != null)
                            {
                                IsWeekday = Convert.ToBoolean(weekday);
                            }
                            SetRecurrenceType(repeat);
                            XAttribute day = repeat.AttributeIgnoreCase("day");
                            if (day != null)
                            {
                                IsDay = Convert.ToBoolean(day);
                                if (Type == RecurrenceType.Monthly || Type == RecurrenceType.Yearly)
                                {
                                    Day = Convert.ToInt32(day.Value, CultureInfo.InvariantCulture);
                                }
                            }
                            if (Type == RecurrenceType.Yearly || Type == RecurrenceType.YearlyByDay)
                            {
                                XAttribute month = repeat.AttributeIgnoreCase("month");
                                if (month != null)
                                {
                                    Month = (Convert.ToInt32(month.Value, CultureInfo.InvariantCulture));
                                }
                            }
                        }
                        XElement windowEnd = rule.ElementIgnoreCase(WindowEndTag);
                        if (windowEnd != null)
                        {
                            WindowEnd = DateTime.Parse(windowEnd.Value);
                        }
                        XElement repeatInstances = rule.ElementIgnoreCase(RepeatInstancesTag);
                        if (repeatInstances != null)
                        {
                            RepeatInstances = Convert.ToInt32(repeatInstances.Value, CultureInfo.InvariantCulture);
                        }
                        XElement firstDayOfWeek = rule.ElementIgnoreCase(FirstDayOfWeekTag);
                        if (firstDayOfWeek != null)
                        {
                            SetFirstDayOfWeek(firstDayOfWeek);
                        }
                    }
                }
            }
        }
    }
}

