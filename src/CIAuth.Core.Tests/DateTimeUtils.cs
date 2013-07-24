﻿using System;

namespace CIAuth.Core.Tests
{
    /// <summary>
    /// http://skysanders.net/subtext/archive/2010/04/20/c-truncate-datetime.aspx
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// <para>Truncates a DateTime to a specified resolution.</para>
        /// <para>A convenient source for resolution is TimeSpan.TicksPerXXXX constants.</para>
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(this DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        public static DateTimeOffset Truncate(this DateTimeOffset date, long resolution)
        {
            return new DateTimeOffset(date.Ticks - (date.Ticks % resolution),TimeSpan.Zero);
        }
    }
}