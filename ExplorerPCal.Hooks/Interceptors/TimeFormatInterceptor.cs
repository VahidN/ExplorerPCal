using System;
using System.Text;
using ExplorerPCal.Hooks.Utils;

namespace ExplorerPCal.Hooks.Interceptors
{
    public static class TimeFormatInterceptor
    {
        public static int ModifyTimeFormat(
                                        uint locale,
                                        uint dwFlags,
                                        SystemTime lpTime,
                                        string lpFormat,
                                        StringBuilder lpTimeStr,
                                        int sbSize)
        {
            var date = DateTime.Now;
            if (lpTime != null)
            {
                // سه پارامتر اول زمان ارسالی از طرف ویندوز می‌تواند کاملا بی‌ربط باشد؛ هدف فقط زمان است
                date = new DateTime(date.Year, date.Month, date.Day, lpTime.Hour, lpTime.Minute, lpTime.Second);
            }

            var amPm = "ق.ظ";
            if (date.Hour >= 12)
            {
                amPm = "ب.ظ";
            }

            var time = date.ToString("hh:mm");

            if (dwFlags == 0)
            {
                time = date.ToString("hh:mm:ss");
            }

            time = string.Format("{0} {1}{2}", time, PersianDateHelper.RightToLeftEmbedding, amPm);
            lpTimeStr.SetOutputBuffer(time.ToPersianNumbers());
            return time.Length + NativeMethods.NullCharLength;
        }
    }
}