using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExplorerPCal.Hooks.Utils
{
    public static class PersianDateHelper
    {
        public const char RightToLeftEmbedding = (char)0x202B;

        public static string ToShamsiDateTime(this DateTime info, bool includeHourMinute, bool includeDayWeekName)
        {
            var persianCalendar = new PersianCalendar();
            var year = persianCalendar.GetYear(info);
            var month = persianCalendar.GetMonth(info);
            var day = persianCalendar.GetDayOfMonth(info);

            const string separator = " ";
            var result = string.Format("{0}{1}{2}{1}{3}", day, separator, ShamsiMonthName[month], year);

            if (includeHourMinute)
                result += string.Format("{0}{1}:{2}", separator, info.Hour.ToString("00"), info.Minute.ToString("00"));

            if (includeDayWeekName)
                result = string.Format("{0}{1}{2}", DayWeekName[info.DayOfWeek], separator, result);

            return string.Format("{0}{1}", RightToLeftEmbedding, result.ToPersianNumbers());
        }

        public static string ToPersianNumbers(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            return
               data.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
        }

        public static readonly IDictionary<int, string> ShamsiMonthName = new Dictionary<int, string>
        {
            {1, "فروردین"},
            {2, "اردیبهشت"},
            {3, "خرداد"},
            {4, "تیر"},
            {5, "مرداد"},
            {6, "شهریور"},
            {7, "مهر"},
            {8, "آبان"},
            {9, "آذر"},
            {10, "دی"},
            {11, "بهمن"},
            {12, "اسفند"}
        };

        public static readonly IDictionary<DayOfWeek, string> DayWeekName = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Saturday, "شنبه"},
            {DayOfWeek.Sunday,  "یک شنبه"},
            {DayOfWeek.Monday,  "دو شنبه"},
            {DayOfWeek.Tuesday, "سه شنبه"},
            {DayOfWeek.Wednesday, "چهار شنبه"},
            {DayOfWeek.Thursday, "پنج شنبه"},
            {DayOfWeek.Friday, "جمعه"}
        };
    }
}