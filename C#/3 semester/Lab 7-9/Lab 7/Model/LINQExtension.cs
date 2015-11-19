using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_7.Model
{
    public static class LINQExtension
    {
        public static string TotalDuration(this IEnumerable<SongModel> source)
        {
            long sum = source.Sum(item => item.TotalDuration);
            var duration = new TimeSpan(sum);

            return "" + (duration.Minutes + duration.Hours * 60 + duration.Days * 60 * 24) +
                ":" + duration.Seconds.ToString("D2");
        }
    }
}
