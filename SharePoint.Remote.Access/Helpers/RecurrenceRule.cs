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

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasEnd
        {
            get { return NumberOfOccurrences > 0 || EndDate.HasValue; }
        }
        public int NumberOfOccurrences { get; set; }

        public IEnumerable<Occurrence> GetOccurrences(TimeSpan? startTime = null, TimeSpan? endTime = null)
        {
            TimeSpan start = startTime.HasValue ? new TimeSpan(startTime.Value.Hours, startTime.Value.Minutes, startTime.Value.Seconds) : new TimeSpan(0, 0, 0);
            var end = endTime.HasValue
                ? (start > endTime.Value
                    ? new TimeSpan(start.Hours, start.Minutes, start.Seconds)
                    : new TimeSpan(endTime.Value.Hours, endTime.Value.Minutes, endTime.Value.Seconds))
                : /*new TimeSpan(start.Hours, start.Minutes, start.Seconds)*/new TimeSpan(1, 0, 0, 0);
            int occurrenceCounter = 0;

            if (HasEnd)
            {
                Occurrence occurrence = null;
                while ((occurrence = GetNextOccurrence(occurrence, start, end)).Events.Length > 0)
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
        internal IntervalRecurrenceRule()
        {
        }
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
            return new Occurrence
            {
                Events = events.ToArray()
            };
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
            return new Occurrence
            {
                Events = events.ToArray()
            };
        }
    }

    [Serializable]
    public sealed class MonthlyRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyRecurrenceRule()
        {
        }

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
                startDate = startDate.AddMonths(1);
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
            return new Occurrence
            {
                Events = events.ToArray()
            };
        }
    }

    [Serializable]
    public sealed class MonthlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        internal MonthlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
        {
            DateTime startDate;
            if (lastOccurrence != null && lastOccurrence.Events.Length > 0)
            {
                startDate = lastOccurrence.LastEvent.Start.Date.Add(startTime).AddMonths(Interval);
                startDate = startDate.AddDays(1 - startDate.Day);
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
            int[] days;
            do
            {
                days = SPRecurrenceHelper.GetMatchedDays(startDate, DayOfWeekOrdinal, DayOfWeek).ToArray();
                if (days.Length == 0)
                {
                    startDate = startDate.AddMonths(1).AddDays(1 - startDate.Day);
                }
            } while (days.Length == 0);

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
            return new Occurrence
            {
                Events = events.ToArray()
            };
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
            }
            if (startDate.Month > (int)Month || (startDate.Month == (int)Month && startDate.Day > DayOfMonth))
            {
                startDate = startDate.AddYears(1).AddMonths(-startDate.Month + (int)Month);
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
            return new Occurrence
            {
                Events = events.ToArray()
            };
        }

    }

    [Serializable]
    public sealed class YearlyByDayRecurrenceRule : IntervalRecurrenceRule
    {
        internal YearlyByDayRecurrenceRule()
        {
        }
        public DayOfWeek DayOfWeek { get; set; }
        public DayOfWeekOrdinal DayOfWeekOrdinal { get; set; }
        public Month Month { get; internal set; }

        protected override Occurrence GetNextOccurrence(Occurrence lastOccurrence, TimeSpan startTime, TimeSpan endTime)
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
        public RecurrentEvent[] Events { get; set; }
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
