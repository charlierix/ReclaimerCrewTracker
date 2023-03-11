using ReclaimerCrewTracker.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReclaimerCrewTracker
{
    public static class Utility
    {
        public static TimeFromTo[] GetStartStopTimes(IEnumerable<DateTime> times, DateTime now)
        {
            var retVal = new List<TimeFromTo>();

            DateTime? prev = null;

            foreach (DateTime time in times)
            {
                if (prev != null)
                {
                    retVal.Add(new TimeFromTo()
                    {
                        From = prev.Value,
                        To = time,
                    });

                    prev = null;
                }
                else
                {
                    prev = time;
                }
            }

            if (prev != null)
                retVal.Add(new TimeFromTo()
                {
                    From = prev.Value,
                    To = now,
                });

            return retVal.ToArray();
        }

        public static string TimeSpanToString(TimeSpan span, bool report_milliseconds = false)
        {
            if (span == TimeSpan.Zero)
                return "0";

            string retVal = "";

            if (span.TotalHours >= 1)
            {
                retVal = $"{span.Hours}:{span.Minutes:00}:{span.Seconds:00}";
            }
            else if (span.TotalMinutes >= 1)
            {
                retVal = span.Seconds == 60 ?
                    "1:00" :
                    $"{span.Minutes}:{span.Seconds:00}";

            }
            else if (span.TotalSeconds >= 1)
            {
                retVal = Convert.ToInt32(span.TotalSeconds).ToString();        // don't bother with milliseconds
            }
            else
            {
                retVal = report_milliseconds ?
                    Convert.ToInt32(span.TotalMilliseconds).ToString("000") :
                    "0";
            }

            return retVal;
        }

        public static TimeFromTo[] Intersect(TimeFromTo[] times_parent, TimeFromTo[] times_member)
        {
            //NOTE: This could be optimized by keeping track of which parents are already examined, but there shouldn't be enough entries to bother
            return times_member.
                SelectMany(o => Intersect(times_parent, o)).
                ToArray();
        }
        private static TimeFromTo[] Intersect(TimeFromTo[] times_parent, TimeFromTo span_member)
        {
            //NOTE: this expects times_parent to be sorted

            var retVal = new List<TimeFromTo>();

            TimeFromTo span = span_member;

            foreach (TimeFromTo parent in times_parent)
            {
                if (parent.To <= span.From)
                    continue;       // parent before

                if (parent.From >= span.To)
                    break;      // parent after

                if (parent.From > span.From)
                    span = span with { From = parent.From };        // parent clips start of span

                if(parent.To >= span.To)
                {
                    retVal.Add(span);
                    break;      // entire span can go
                }

                if (parent.To < span.To)
                {
                    retVal.Add(span with { To = parent.To });
                    span = span with { From = parent.To };
                }
            }

            return retVal.ToArray();
        }
    }
}
