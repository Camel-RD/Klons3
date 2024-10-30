using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Data;
using KlonsF.Classes;
using KlonsLIB;
using Klons3.ModelsP;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KlonsP.Classes
{
    public static class DataTasksP
    {
        public static KlonsData MyData => KlonsData.St;

        public static string GetCat1Name(int id)
        {
            var dr = MyData.DbContextP.P_CAT1.Find(id);
            if (dr == null) return null;
            return dr.DESCR;
        }

        public static string GetCatDName(int id)
        {
            var dr = MyData.DbContextP.P_CATD.Find(id);
            if (dr == null) return null;
            return dr.DESCR;
        }

        public static string GetCatTName(int id)
        {
            var dr = MyData.DbContextP.P_CATT.Find(id);
            if (dr == null) return null;
            return dr.DESCR;
        }

        public static string GetDepName(int id)
        {
            var dr = MyData.DbContextP.P_DEPARTMENTS.Find(id);
            if (dr == null) return null;
            return dr.DESCR;
        }

        public static string GetPlaceName(int id)
        {
            var dr = MyData.DbContextP.P_PLACES.Find(id);
            if (dr == null) return null;
            return dr.DESCR;
        }
        public static decimal Round(decimal d, int k)
        {
            return Math.Round(d, k, MidpointRounding.AwayFromZero);
        }

        public static int CountItemsWithErrors()
        {
            return MyData.DbContextP.P_ITEMS.Where(d => d.XState == EState.Error).Count();
        }

        public static P_CAT1 GetTCat1Row(this P_ITEMS item)
        {
            var dr = MyData.DbContextP.P_CAT1.Find(item.TCAT1);
            return dr;
        }

        public static P_CATD GetTCatDRow(this P_ITEMS item)
        {
            var dr = MyData.DbContextP.P_CATD.Find(item.TCATD);
            return dr;
        }

        public static P_CATT GetTCatTRow(this P_ITEMS item)
        {
            var dr = MyData.DbContextP.P_CATT.Find(item.TCATT);
            return dr;
        }

        public static P_DEPARTMENTS GetTDEPARTMENTSRow(this P_ITEMS item)
        {
            var dr = MyData.DbContextP.P_DEPARTMENTS.Find(item.TDEPARTMENT);
            return dr;
        }

        public static P_PLACES GetTPLACESRow(this P_ITEMS item)
        {
            var dr = MyData.DbContextP.P_PLACES.Find(item.TPLACE);
            return dr;
        }

    }

    public class ErrorInfo
    {
        public string Source { get; set; } = null;
        public string Message { get; set; } = null;
    }

    public class ErrorList : List<ErrorInfo>
    {
        public bool HasErrors { get { return Count > 0; } }

        public ErrorList()
        {
        }

        public ErrorList(string source, string msg)
        {
            AddError(source, msg);
        }

        public void AddError(string source, string msg)
        {
            var ei = new ErrorInfo()
            {
                Source = source,
                Message = msg
            };
            Add(ei);
        }

        public void AddPersonError(int idp, string msg)
        {
            var ei = new ErrorInfo();
            /*
            var table_persons = KlonsData.St.DataSetKlonsP.PERSONS;
            if (table_persons != null)
            {
                var dr = table_persons.FindByID(idp);
                if (dr != null)
                    ei.Source = string.Format("{0}", dr.ZNAME);
            }
            */
            ei.Message = msg;
            Add(ei);
        }

        public void SetErrorList(ErrorList newlist)
        {
            Clear();
            AddRange(newlist);
        }

        public static ErrorList operator +(ErrorList e1, ErrorList e2)
        {
            if (e1 == null || e2 == null) return e1;
            e1.AddRange(e2);
            return e1;
        }

    }
}
