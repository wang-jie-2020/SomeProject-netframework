using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeProject.Infrastructure.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool Compare(this string orig, string target)
        {
            return string.Compare(orig, target, true) == 0;
        }

        public static bool Compare(this string orig, string target, bool ignoreCase)
        {
            return string.Compare(orig, target, ignoreCase) == 0;
        }
    }
}
