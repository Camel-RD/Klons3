using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klons3.ModelsA;
using Klons3.ModelsM;
using KlonsLIB.Misc;

namespace KlonsF.Classes
{
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

        public void AddPersonError(A_PERSONS dr_person, string msg)
        {
            var ei = new ErrorInfo();
            if (dr_person != null)
                ei.Source = dr_person.ZNAME;
            ei.Message = msg;
            Add(ei);
        }

        public void AddItemError(M_ITEMS dr_item, string msg)
        {
            var ei = new ErrorInfo();
            if (dr_item != null)
                ei.Source = $"artikuls: {dr_item.BARCODE} {dr_item.NAME}";
            ei.Message = msg;
            Add(ei);
        }

        public void AddItemError(int iditem, string msg)
        {
            var ei = new ErrorInfo();

            var table_items = KlonsData.St.DbContextM.M_ITEMS;
            if (table_items != null)
            {
                var dr = table_items.Find(iditem);
                if (dr != null)
                    ei.Source = $"artikuls: {dr.BARCODE} {dr.NAME}";
            }
            ei.Message = msg;

            Add(ei);
        }

        public void AddDocError(int iddoc, string msg)
        {
            var ei = new ErrorInfo();
            var table_docs = KlonsData.St.DbContextM.M_DOCS;
            if (table_docs != null)
            {
                var dr = table_docs.Find(iddoc);
                if (dr != null)
                    ei.Source = $"dokuments: {Utils.DateToString(dr.DT)} {dr.DocSrNr}";
            }
            ei.Message = msg;
            Add(ei);
        }

        public void SetErrorList(ErrorList newlist)
        {
            Clear();
            AddRange(newlist);
        }

        public bool HasSourceData => this.Where(x => !x.Source.IsNOE()).Any();

        public static ErrorList operator +(ErrorList e1, ErrorList e2)
        {
            if (e1 == null || e2 == null) return e1;
            e1.AddRange(e2);
            return e1;
        }

    }

}
