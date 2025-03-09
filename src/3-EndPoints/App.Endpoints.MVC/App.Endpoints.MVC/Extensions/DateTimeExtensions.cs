using App.Domain.Core.Utils;
using System;

namespace App.Endpoints.MVC.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPersianDate(this DateTime date)
        {
            return DateUtils.ToPersianDate(date);
        }

        public static string ToPersianDateWithTime(this DateTime date)
        {
            return DateUtils.ToPersianDateWithTime(date);
        }

        public static string ToTimeString(this DateTime date)
        {
            return DateUtils.GetTimeOnly(date);
        }
    }
}