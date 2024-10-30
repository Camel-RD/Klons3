using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Misc;

namespace KlonsLIB.Data
{
    public static class SomeExtensions
   {
        public static bool IsSuccess(this EBsSaveResult rt) => 
            rt == EBsSaveResult.Saved || rt == EBsSaveResult.SaveSkipped;
    }

    public class FilterBuilder
    {
        public string FilterString = "";
        public void AddFilterPart(string fpart)
        {
            if (FilterString == null) FilterString = "";
            if (fpart == null) fpart = "";
            if (fpart == "") return;
            if (FilterString == "") FilterString = $"({fpart})";
            else FilterString = $"{FilterString} AND ({fpart})";
        }
    }
}
