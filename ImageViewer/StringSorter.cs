using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ImageViewer
{
    public static class StringCompare
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);
        public static int CompareStringLogical(string a, string b)
        {
            return StrCmpLogicalW(a, b);
        }
    }
}
