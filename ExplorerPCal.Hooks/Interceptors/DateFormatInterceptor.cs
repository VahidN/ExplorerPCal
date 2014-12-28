using System;
using System.Text;
using ExplorerPCal.Hooks.Utils;

namespace ExplorerPCal.Hooks.Interceptors
{
    public static class DateFormatInterceptor
    {
        public static int ModifyDateFormat(
                                        uint locale,
                                        uint dwFlags,
                                        SystemTime lpDate,
                                        string lpFormat,
                                        StringBuilder lpDateStr,
                                        int sbSize)
        {
            var date = DateTime.Now;
            if (lpDate != null)
            {
                date = new DateTime(lpDate.Year, lpDate.Month, lpDate.Day, lpDate.Hour, lpDate.Minute, lpDate.Second);
            }

            if (lpFormat == "dddd")
            {
                var dayOfWeek = PersianDateHelper.DayWeekName[date.DayOfWeek];
                lpDateStr.SetOutputBuffer(dayOfWeek);
                return dayOfWeek.Length + NativeMethods.NullCharLength;
            }

            var pDate = date.ToShamsiDateTime(includeHourMinute: false, includeDayWeekName: false);

            var dateFlags = (NlsDateFlags)dwFlags;
            if (dateFlags.HasFlag(NlsDateFlags.DateLongDate))
            {
                pDate = date.ToShamsiDateTime(includeHourMinute: false, includeDayWeekName: true);
            }

            lpDateStr.SetOutputBuffer(pDate);
            return pDate.Length + NativeMethods.NullCharLength;
        }
    }
}