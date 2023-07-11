using System;
using Aware.Util;
using Aware.ECommerce.Enums;

namespace Aware.Task.Model
{
    public class Trigger
    {
        public TriggerType TriggerType { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DaysOfTheWeek WeekDays { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int IntervalMinutes { get; set; }
        public string Value { get; set; }

        public string GetWeekDayName()
        {
            switch (WeekDays)
            {
                case DaysOfTheWeek.Monday:
                    return "Pazartesi";
                case DaysOfTheWeek.Tuesday:
                    return "Salı";
                case DaysOfTheWeek.Wednesday:
                    return "Çarşamba";
                case DaysOfTheWeek.Thursday:
                    return "Perşembe";
                case DaysOfTheWeek.Friday:
                    return "Cuma";
                case DaysOfTheWeek.Saturday:
                    return "Cumartesi";
                case DaysOfTheWeek.Sunday:
                    return "Pazar";
            }
            return WeekDays == 0 ? "Belirtilmemiş" : WeekDays.ToString();
        }
    }
}
