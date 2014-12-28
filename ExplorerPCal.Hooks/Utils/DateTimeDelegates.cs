using System.Runtime.InteropServices;
using System.Text;

namespace ExplorerPCal.Hooks.Utils
{
    public class DateTimeDelegates
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int GetDateFormatWDelegate(
                                        uint locale,
                                        uint dwFlags,
                                        SystemTime lpDate,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpDateStr,
                                        int sbSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int GetTimeFormatWDelegate(
                                        uint locale,
                                        uint dwFlags,
                                        SystemTime lpTime,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpTimeStr,
                                        int sbSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int GetDateFormatExDelegate(
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpLocaleName,
                                        uint dwFlags,
                                        SystemTime lpDate,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpDateStr,
                                        int sbSize,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpCalendar);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public delegate int GetTimeFormatExDelegate(
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpLocaleName,
                                        uint dwFlags,
                                        SystemTime lpTime,
                                        [MarshalAs(UnmanagedType.LPWStr)] string lpFormat,
                                        StringBuilder lpTimeStr,
                                        int sbSize);
    }
}