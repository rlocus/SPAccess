using System;

namespace SharePoint.Remote.Access.Helpers
{
    [Serializable]
    public abstract class RecurrenceRule
    {
        internal RecurrenceRule()
        {
        }

        public DateTime StartDate { get; internal set; }
        public DateTime? EndDate { get; internal set; }
        public bool HasEnd { get; internal set; }
        public int? NumberOfOccurrences { get; internal set; }
    }

    [Serializable]
    public abstract class IntervalRecurrenceRule : RecurrenceRule
    {
        internal IntervalRecurrenceRule()
        {
        }
        public int Interval { get; internal set; }
    }

    public sealed class DyilyRecurrenceRule : IntervalRecurrenceRule
    {
        internal DyilyRecurrenceRule()
        {
        }
    }

    [Serializable]
    public sealed class WeeklyRecurrenceRule : IntervalRecurrenceRule
    {
        internal WeeklyRecurrenceRule()
        {
        }
        public DayOfWeek[] DaysOfWeek { get; internal set; }
        public System.DayOfWeek FirstDayOfWeek { get; internal set; }
    }

    [Serializable]
    public sealed class MonthlyRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyRecurrenceRule()
        {
        }
        public int DayOfMonth { get; internal set; }
    }

    [Serializable]
    public sealed class MonthlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; internal set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; internal set; }
    }

    [Serializable]
    public sealed class YearlyRecurrenceRule : RecurrenceRule
    {
        internal YearlyRecurrenceRule()
        {
        }
        public int DayOfMonth { get; set; }
        public Month Month { get; set; }
    }

    [Serializable]
    public sealed class YearlyByDayRecurrenceRule : RecurrenceRule
    {
        internal YearlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; internal set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; internal set; }
        public Month Month { get; internal set; }
    }

    //[Serializable]
    //public sealed class Occurrence
    //{
    //    public DateTime End { get; internal set; }
    //    public DateTime Start { get; internal set; }
    //}

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }

    public enum DayOfWeekOrdinal
    {
        None = 0,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Last = 5
    }

    public enum DayOfWeek
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Day = 7,
        Weekday = 8,
        WeekendDay = 9
    }
}
