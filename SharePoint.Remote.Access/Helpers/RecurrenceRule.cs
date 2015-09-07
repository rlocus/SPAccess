using System;
using System.Linq;
using System.Collections.Generic;

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
        public bool HasEnd
        {
            get { return NumberOfOccurrences > 0 || EndDate.HasValue; }
        }
        public int NumberOfOccurrences { get; internal set; }

        public IEnumerable<Occurrence> GetOccurrences(TimeSpan? startTime = null, TimeSpan? endTime = null)
        {
            TimeSpan start;
            TimeSpan end;
            if (startTime.HasValue)
            {
                start = new TimeSpan(startTime.Value.Hours, startTime.Value.Minutes, startTime.Value.Seconds);
            }
            else
            {
                start = new TimeSpan(0, 0, 0);
            }
            if (endTime.HasValue)
            {
                end = new TimeSpan(endTime.Value.Hours, endTime.Value.Minutes, endTime.Value.Seconds);
            }
            else
            {
                end = new TimeSpan(start.Hours, start.Minutes, start.Seconds);
            }
            int occurrenceCounter = 0;
            if (end < start)
            {
                end = start;
            }
            DateTime startDate = StartDate.Date.Add(start);
            if (StartDate > startDate)
            {
                startDate = StartDate;
            }
            if (start > end)
            {
                end = start;
            }
            if (EndDate.HasValue && startDate < EndDate.Value)
            {
                Occurrence occurrence;
                while ((occurrence = GetNextOccurrence(startDate, end)).Events.Length > 0)
                {
                    occurrenceCounter++;
                    yield return occurrence;
                    if (NumberOfOccurrences <= occurrenceCounter)
                    {
                        break;
                    }                   
                }
            }
        }

        protected abstract Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime);
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

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {           
            var events = new List<RecurrentEvent>();
            bool loop = true;
            while (loop)
            {
                DateTime endDate = startDate.Date.Add(endTime);
                if (EndDate.HasValue && endDate > EndDate.Value)
                {
                    endDate = EndDate.Value;
                }
                var currentEvent = new RecurrentEvent
                {
                    Start = startDate,
                    End = endDate
                };
                events.Add(currentEvent);

                if (!HasEnd || (EndDate.HasValue && currentEvent.End >= EndDate.Value))
                {
                    events.Add(currentEvent);
                    loop = false;
                }
                else
                {
                    events.Add(currentEvent);
                    startDate = startDate.AddDays(Interval);
                    if(EndDate.HasValue && startDate > EndDate.Value)
                    {
                        loop = false;
                    }
                }
            }
            return new Occurrence
            {
                Events = events.ToArray()
            };         
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

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class MonthlyRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyRecurrenceRule()
        {
        }
        public int DayOfMonth { get; internal set; }

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class MonthlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; internal set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; internal set; }

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class YearlyRecurrenceRule : IntervalRecurrenceRule
    {
        internal YearlyRecurrenceRule()
        {
        }
        public int DayOfMonth { get; set; }
        public Month Month { get; set; }

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class YearlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        internal YearlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; internal set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; internal set; }
        public Month Month { get; internal set; }

        protected override Occurrence GetNextOccurrence(DateTime startDate, TimeSpan endTime)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public sealed class Occurrence
    {
        public RecurrentEvent FirstEvent
        {
            get { return Events.FirstOrDefault(); }
        }
        public RecurrentEvent LastEvent
        {
            get { return Events.LastOrDefault(); }
        }
        public RecurrentEvent[] Events { get; internal set; }
    }

    [Serializable]
    public sealed class RecurrentEvent
    {
        public DateTime End { get; internal set; }
        public DateTime Start { get; internal set; }
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
