using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ExplorerPCal.Hooks.Utils
{
    /// <summary>
    /// باید به صورت کلاس تعریف می‌شد
    /// چون ویندوز ممکن است آن‌را به صورت نال ارسال کند؛ یعنی درخواست فرمت زمان جاری
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class SystemTime
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort Year;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Month;
        [MarshalAs(UnmanagedType.U2)]
        public ushort DayOfWeek;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Day;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Hour;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Minute;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Second;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Milliseconds;
    }

    [Flags]
    public enum NlsTimeFlags
    {
        /// <summary>
        /// 0
        /// </summary>
        TimeMinutesAndSeconds = 0,

        /// <summary>
        /// 0x00000001
        /// </summary>
        TimeNoMinutesOrSeconds = 0x00000001,

        /// <summary>
        /// 0x00000002
        /// </summary>
        TimeNoSeconds = 0x00000002,

        /// <summary>
        /// 0x00000004
        /// </summary>
        TimeNoTimeMarker = 0x00000004,

        /// <summary>
        /// 0x00000008
        /// </summary>
        TimeForce24HourFormat = 0x00000008
    }

    [Flags]
    public enum NlsDateFlags
    {
        /// <summary>
        /// 0x00000001
        /// </summary>
        DateShortDate = 0x00000001,

        /// <summary>
        /// 0x00000002
        /// </summary>
        DateLongDate = 0x00000002,

        /// <summary>
        /// 0x00000004
        /// </summary>
        DateUseAltCalendar = 0x00000004,

        /// <summary>
        /// 0x00000008
        /// </summary>
        DateYearMonth = 0x00000008,

        /// <summary>
        /// 0x00000010
        /// </summary>
        DateLtrReading = 0x00000010,

        /// <summary>
        /// 0x00000020
        /// </summary>
        DateRtlReading = 0x00000020
    }

    public static class NativeMethods
    {
        public const int HwndBroadcast = 0xffff;
        public const int WmWinIniChange = 0x001A;
        public const int NullCharLength = 1;

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetTimeFormatW(
                                        uint locale,
                                        uint dwFlags, // NLS_TIME_FLAGS
                                        SystemTime lpTime,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpTimeStr,
                                        int sbSize);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetDateFormatW(
                                        uint locale,
                                        uint dwFlags, // NLS_DATE_FLAGS
                                        SystemTime lpDate,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpDateStr,
                                        int sbSize);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);


        public static void RefreshWindowsSettings()
        {
            SendMessage(HwndBroadcast, WmWinIniChange, 0, 0);
        }

        public static void SetOutputBuffer(this StringBuilder buffer, string data)
        {
            if (buffer == null)
                buffer = new StringBuilder();

            buffer.Clear();
            buffer.Append(data);
        }
    }
}