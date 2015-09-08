using System;
using System.Linq;
using System.Collections.Generic;

namespace SharePoint.Remote.Access.Helpers
{
    [Serializable]
    public abstract class RecurrenceRule
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasEnd
        {
            get { return NumberOfOccurrences > 0 || EndDate.HasValue; }
        }
        public int NumberOfOccurrences { get; set; }

        public IEnumerable<Occurrence> GetOccurrences()
        {
            TimeSpan startTime = StartDate.TimeOfDay;
            TimeSpan endTime = EndDate.HasValue
                ? (startTime >= EndDate.Value.TimeOfDay
                    ? startTime
                    : EndDate.Value.TimeOfDay)
                : new TimeSpan(1, 0, 0, 0);
            return GetOccurrences(startTime, endTime);
        }

        public IEnumerable<Occurrence> GetOccurrences(TimeSpan endTime)
        {
            TimeSpan startTime = new TimeSpan(StartDate.Hour, StartDate.Minute, StartDate.Second);
            return GetOccurrences(startTime, endTime);
        }

        public IEnumerable<Occurrence> GetOccurrences(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime > endTime)
            {
                throw new ArgumentException("endTime");
            }
            int occurrenceCounter = 0;
            if (HasEnd)
            {
                Occurrence occurrence = null;
                while ((occurrence = GetNextOccurrence(occurrence, startTime, endTime)).Events.Length > 0)
                {
                    occurrenceCounter++;
                    yield return occurrence;
                    if (NumberOfOccurrences == occurrenceCounter)
                    {
                        break;
                    }
                }
            }
        }

        protected abstract Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime);
    }

    [Serializable]
    public abstract class IntervalRecurrenceRule : RecurrenceRule
    {
        public int Interval { get; set; }
    }

    public sealed class DyilyRecurrenceRule : IntervalRecurrenceRule
    {
        internal DyilyRecurrenceRule()
        {
        }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddDays(Interval);
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
            }
            var events = new List<RecurrentEvent>();
            if (!EndDate.HasValue || startDate <= EndDate.Value)
            {
                DateTime endDate = startDate.Date.Add(endTime);
                if (startDate <= endDate)
                {
                    if (EndDate.HasValue && endDate >= EndDate.Value)
                    {
                        events.Add(new RecurrentEvent(startDate, EndDate.Value));
                    }
                    else
                    {
                        events.Add(new RecurrentEvent(startDate, endDate));
                    }
                }
            }
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class WeeklyRecurrenceRule : IntervalRecurrenceRule
    {
        public DayOfWeek[] DaysOfWeek { get; set; }
        public System.DayOfWeek FirstDayOfWeek { get; set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime);
                int frequency = Interval;
                if (frequency <= 1)
                {
                    while (startDate.DayOfWeek != FirstDayOfWeek)
                    {
                        startDate = startDate.AddDays(1);
                    }
                }
                else
                {
                    while (frequency > 1)
                    {
                        startDate = startDate.AddDays(7);
                        while (startDate.DayOfWeek != FirstDayOfWeek)
                        {
                            startDate = startDate.AddDays(1);
                        }
                        frequency--;
                    }
                }
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
            }
            var events = new List<RecurrentEvent>();
            do
            {
                if (EndDate.HasValue && startDate > EndDate.Value)
                {
                    break;
                }
                DateTime endDate = startDate.Date.Add(endTime);
                if (startDate <= endDate)
                {
                    if (SPRecurrenceHelper.IsDayOfWeekMatched(DaysOfWeek, startDate))
                    {
                        if (EndDate.HasValue && endDate >= EndDate.Value)
                        {
                            events.Add(new RecurrentEvent(startDate, EndDate.Value));
                        }
                        else
                        {
                            events.Add(new RecurrentEvent(startDate, endDate));
                        }
                    }
                }
                startDate = startDate.AddDays(1);
            } while (startDate.DayOfWeek != FirstDayOfWeek);
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class MonthlyRecurrenceRule : IntervalRecurrenceRule
    {
        public int DayOfMonth { get; set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddMonths(Interval);
                startDate = startDate.AddDays(-startDate.Day);
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
            }

            if (startDate.Day > DayOfMonth)
            {
                startDate = startDate.AddMonths(Interval);
            }
            startDate = startDate.AddDays(-startDate.Day + 1);
            var events = new List<RecurrentEvent>();
            int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            startDate = startDate.AddDays(((daysInMonth < DayOfMonth) ? daysInMonth : DayOfMonth) - 1);
            if (!EndDate.HasValue || startDate <= EndDate.Value)
            {
                DateTime endDate = startDate.Date.Add(endTime);
                if (startDate <= endDate)
                {
                    if (EndDate.HasValue && endDate >= EndDate.Value)
                    {
                        events.Add(new RecurrentEvent(startDate, EndDate.Value));
                    }
                    else
                    {
                        events.Add(new RecurrentEvent(startDate, endDate));
                    }
                }
            }
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class MonthlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            int[] days;
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddMonths(Interval);
                startDate = startDate.AddDays(1 - startDate.Day);
                days = SPRecurrenceHelper.GetMatchedDays(startDate, DayOfWeekOrdinal, DayOfWeek).ToArray();
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
                days = SPRecurrenceHelper.GetMatchedDays(startDate, DayOfWeekOrdinal, DayOfWeek).ToArray();
                if (days.Length == 0)
                {
                    startDate = startDate.AddMonths(Interval).AddDays(1 - startDate.Day);
                    days = SPRecurrenceHelper.GetMatchedDays(startDate, DayOfWeekOrdinal, DayOfWeek).ToArray();
                }
            }
            var events = new List<RecurrentEvent>();
            foreach (int day in days)
            {
                startDate = startDate.AddDays(day - startDate.Day);
                if (!EndDate.HasValue || startDate <= EndDate.Value)
                {
                    DateTime endDate = startDate.Date.Add(endTime);
                    if (startDate <= endDate)
                    {
                        if (EndDate.HasValue && endDate >= EndDate.Value)
                        {
                            events.Add(new RecurrentEvent(startDate, EndDate.Value));
                        }
                        else
                        {
                            events.Add(new RecurrentEvent(startDate, endDate));
                        }
                    }
                }
            }
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class YearlyRecurrenceRule : IntervalRecurrenceRule
    {
        public int DayOfMonth { get; set; }
        public Month Month { get; set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddYears(Interval);
                startDate = startDate.AddDays(-startDate.Day);
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
                if (startDate.Month > (int)Month || (startDate.Month == (int)Month && startDate.Day > DayOfMonth))
                {
                    startDate = startDate.AddYears(Interval).AddMonths((int)Month - startDate.Month);
                }
            }

            startDate = startDate.AddDays(1 - startDate.Day);
            var events = new List<RecurrentEvent>();
            int daysInMonth = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            startDate = startDate.AddDays(((daysInMonth < DayOfMonth) ? daysInMonth : DayOfMonth) - 1);
            if (!EndDate.HasValue || startDate <= EndDate.Value)
            {
                DateTime endDate = startDate.Date.Add(endTime);
                if (startDate <= endDate)
                {
                    if (EndDate.HasValue && endDate >= EndDate.Value)
                    {
                        events.Add(new RecurrentEvent(startDate, EndDate.Value));
                    }
                    else
                    {
                        events.Add(new RecurrentEvent(startDate, endDate));
                    }
                }
            }
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class YearlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        public DayOfWeek DayOfWeek { get; set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; set; }
        public Month Month { get; internal set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddYears(Interval);
                startDate = startDate.AddDays(1 - startDate.Day);
            }
            else
            {
                startDate = StartDate.Date.Add(startTime);
                if (StartDate > startDate)
                {
                    startDate = StartDate;
                }
                if (startDate.Month > (int)Month)
                {
                    startDate = startDate.AddYears(Interval).AddMonths((int)Month - startDate.Month).AddDays(1 - startDate.Day);
                }
            }
            var days = SPRecurrenceHelper.GetMatchedDays(startDate, DayOfWeekOrdinal, DayOfWeek).ToArray();
            var events = new List<RecurrentEvent>();
            foreach (int day in days)
            {
                startDate = startDate.AddDays(day - startDate.Day);
                if (!EndDate.HasValue || startDate <= EndDate.Value)
                {
                    DateTime endDate = startDate.Date.Add(endTime);
                    if (startDate <= endDate)
                    {
                        if (EndDate.HasValue && endDate >= EndDate.Value)
                        {
                            events.Add(new RecurrentEvent(startDate, EndDate.Value));
                        }
                        else
                        {
                            events.Add(new RecurrentEvent(startDate, endDate));
                        }
                    }
                }
            }
            return new Occurrence(events);
        }
    }

    [Serializable]
    public sealed class Occurrence
    {
        public Occurrence(IEnumerable<RecurrentEvent> events)
        {
            if (events == null) throw new ArgumentNullException("events");
            Events = events.ToArray();
        }

        public RecurrentEvent FirstEvent
        {
            get { return Events.FirstOrDefault(); }
        }
        public RecurrentEvent LastEvent
        {
            get { return Events.LastOrDefault(); }
        }
        public RecurrentEvent[] Events { get; private set; }
    }

    [Serializable]
    public sealed class RecurrentEvent
    {
        public RecurrentEvent(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        public DateTime End { get; private set; }
        public DateTime Start { get; private set; }

        public TimeSpan Duration
        {
            get { return (End - Start).Duration(); }
        }
    }

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
