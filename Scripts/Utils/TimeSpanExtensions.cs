using System;

namespace AtomicApps.Utils
{
    public static class TimeSpanExtension
    {
        public static string ToStringMMSS(this TimeSpan timeSpan) => 
            $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        
        public static string ToStringHHMMSS(this TimeSpan timeSpan) => 
            $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

        public static string ToStringDHHMM(this TimeSpan timeSpan) =>
            $"{timeSpan.Days}d {timeSpan.Hours}h {timeSpan.Minutes}m";
    }
}
