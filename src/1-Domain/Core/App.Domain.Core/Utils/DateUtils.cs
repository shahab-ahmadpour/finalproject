using System;
using System.Globalization;

namespace App.Domain.Core.Utils
{
    public static class DateUtils
    {
        public static string ToPersianDate(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
        }

        public static string ToPersianDateWithTime(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00} {date.Hour:00}:{date.Minute:00}";
        }

        public static string GetTimeOnly(DateTime date)
        {
            return date.ToString("HH:mm");
        }
    }
}