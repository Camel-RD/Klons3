﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsF;
using KlonsF.Classes;
using KlonsF.FormsReportParams;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsF.Forms
{
    public partial class Form_LogDoc : MyFormBaseF
    {
        public Form_LogDoc()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvDocs.AutoGenerateColumns = false;
            dgvRows.AutoGenerateColumns = false;
            dgvDocs.DisableAllColumnSorting();
            dgvRows.DisableAllColumnSorting();
        }

        private void Form_LogDoc_Load(object sender, EventArgs e)
        {
            ActiveControl = dgvDocs;
        }

        private DbContextF MyDataSet => MyData.DbContextF;
        public List<LDoc> DocList = new List<LDoc>();
        bool UsingSimpleView = false;
        int? CurrentDocId = null;

        public void GetData(int docid)
        {
            var table_docs = DataLoaderF.F_LOPSD_GetBy_SP_F_DIFF_01(docid);
            var table_rows = DataLoaderF.F_LOPS_GetBy_SP_F_DIFF_02(docid);
            bsRows.DataSource = null;
            bsDocs.DataSource = null;
            UsingSimpleView = true;
            bsDocs.DataSource = table_docs;
            bsRows.DataSource = table_rows;
            CurrentDocId = docid;
        }

        public void GetData2(int docid)
        {
            var table_docs = DataLoaderF.F_LOPSD_GetBy_SP_F_DIFF_01(docid);
            var table_rows = DataLoaderF.F_LOPS_GetBy_SP_F_DIFF_02(docid);
            table_rows.Sort((x, y) => Comparer<int>.Default.Compare(x.IDL, y.IDL));
            table_rows.Sort((x, y) => Comparer<int>.Default.Compare(x.IDL, y.IDL));

            var docs1 = GetDistinctLDocs(table_docs.ToList());
            docs1 = CheckIntervals(docs1, new TimeSpan(0, 1, 0, 0));
            docs1 = GetDistinctLDocs(docs1);

            var rows1 = GetDistinctLRows(table_rows.ToList());
            var rows2 = CheckIntervalsX(rows1, new TimeSpan(0, 1, 0, 0));
            rows2 = GetDistinctLRows(rows2);

            var docs2 = MergeA(docs1, rows2);
            var docs3 = CheckIntervals(docs2, new TimeSpan(0, 1, 0, 0));
            DocList = MergeB(docs3, rows2);

            bsDocs.DataSource = null;
            bsRows.DataSource = null;
            UsingSimpleView = false;

            bsDocs.DataSource = DocList;
            CurrentDocId = docid;
        }

        public void UpdateRowList()
        {
            if (UsingSimpleView) return;
            if (bsDocs.DataSource == null || 
                bsDocs.Position == -1 ||
                bsDocs.Current == null)
            {
                bsRows.DataSource = null;
                return;
            }
            var ldoc = bsDocs.Current as LDoc;
            bsRows.DataSource = ldoc.Rows;
        }

        public DateTime GetDt2(F_LOPSD row)
        {
            if(row.TPL == 1)
            {
                if (row.DTLD is null) return row.ZDT.Value;
                return row.DTLD.Value;
            }
            return DateTime.MaxValue;
        }

        public DateTime GetDt2(F_LOPS row)
        {
            if (row.TPL == 1)
            {
                if (row.DTLD is null) return row.DTL.Value;
                return row.DTLD.Value;
            }
            return DateTime.MaxValue;
        }

        public List<LDoc> GetDistinctLDocs(List<F_LOPSD> docs)
        {
            var rows2 = docs
                .Select(x => new LDoc()
                {
                    Dt1 = x.ODT.Value,
                    Dt2 = GetDt2(x),
                    Doc = x
                })
                .ToList();
            return GetDistinctLDocs(rows2);
        }

        public List<LDoc> GetDistinctLDocs(List<LDoc> docs)
        {
            var ret = new List<LDoc>();
            docs = docs
                .OrderBy(x => x.Dt1)
                .ThenBy(x => x.Dt2)
                .ToList();
            var prow = docs[0];
            var ldoc = new LDoc()
            {
                Dt1 = prow.Dt1,
                Dt2 = prow.Dt2,
                Doc = prow.Doc
            };
            for (int i = 1; i < docs.Count; i++)
            {
                var crow = docs[i];
                if (crow.ODT == prow.ODT)
                {
                    ldoc.Dt2 = crow.Dt2;
                    ldoc.Doc = crow.Doc;
                    continue;
                }
                if (HasDiff(crow.Doc, prow.Doc))
                {
                    ret.Add(ldoc);
                    prow = crow;
                    ldoc = new LDoc()
                    {
                        Dt1 = prow.Dt1,
                        Dt2 = prow.Dt2,
                        Doc = prow.Doc
                    };
                    continue;
                }
                ldoc.Dt2 = crow.Dt2;
            }
            ret.Add(ldoc);
            return ret;
        }

        public List<LRow> GetDistinctLRows(List<F_LOPS> rows)
        {
            var rows2 = rows
                .Select(x => new LRow()
                {
                    Dt1 = x.ODT.Value,
                    Dt2 = GetDt2(x),
                    Row = x
                })
                .ToList();
            return GetDistinctLRows(rows2);
        }

        public List<LRow> GetDistinctLRows(List<LRow> rows)
        {
            var ret = new List<LRow>();
            rows = rows
                .OrderBy(x => x.id)
                .ThenBy(x => x.Dt1)
                .ThenBy(x => x.Dt2)
                .ThenBy(x => x.idl)
                .ToList();
            var prow = rows[0];
            var lrow = new LRow()
            {
                Dt1 = prow.Dt1,
                Dt2 = prow.Dt2,
                Row = prow.Row
            };
            for (int i = 1; i < rows.Count; i++)
            {
                var crow = rows[i];
                if (crow.id == prow.id && crow.ODT == prow.ODT)
                {
                    lrow.Dt2 = crow.Dt2;
                    lrow.Row = crow.Row;
                    continue;
                }
                if (crow.id != prow.id || HasDiff(crow.Row, prow.Row))
                {
                    ret.Add(lrow);
                    prow = crow;
                    lrow = new LRow()
                    {
                        Dt1 = prow.Dt1,
                        Dt2 = prow.Dt2,
                        Row = prow.Row
                    };
                    continue;
                }
                lrow.Dt2 = crow.Dt2;
            }
            ret.Add(lrow);
            return ret;
        }

        public List<LDoc> MergeA(List<LDoc> docs, List<LRow> rows)
        {
            var ret = new List<LDoc>();
            var dts1 = rows.Select(x => x.Dt1);
            var dts2 = rows.Select(x => x.Dt2);
            var rowdts = dts1.Union(dts2).Distinct().OrderBy(x => x).ToList();
            LDoc ldoc = null;
            foreach (var doc in docs)
            {
                var rdts = rowdts
                    .Where(x => x > doc.Dt1 && x < doc.Dt2)
                    .OrderBy(x => x)
                    .ToList();
                if(rdts.Count == 0)
                {
                    ldoc = new LDoc()
                    {
                        Dt1 = doc.Dt1,
                        Dt2 = doc.Dt2,
                        Doc = doc.Doc
                    };
                    ret.Add(ldoc);
                    continue;
                }
                ldoc = new LDoc()
                {
                    Dt1 = doc.Dt1,
                    Dt2 = doc.Dt2,
                    Doc = doc.Doc
                };
                foreach(var rdt in rdts)
                {
                    ldoc.Dt2 = rdt;
                    ret.Add(ldoc);
                    ldoc = new LDoc()
                    {
                        Dt1 = rdt,
                        Dt2 = doc.Dt2,
                        Doc = doc.Doc
                    };
                }
                ldoc.Dt2 = doc.Dt2;
                ret.Add(ldoc);
            }
            return ret;
        }

        public List<LDoc> CheckIntervals(List<LDoc> docs, TimeSpan mindelata)
        {
            if (docs.Count < 2) return docs;
            var ret = new List<LDoc>();
            ret.Add(docs[docs.Count - 1]);
            for (int i = docs.Count - 2; i >= 0; i--)
            {
                var cdoc = docs[i];
                var pdoc = ret[ret.Count - 1];
                if (cdoc.Dt2 - cdoc.Dt1 < mindelata)
                {
                    pdoc.Dt1 = cdoc.Dt1;
                    continue;
                }
                ret.Add(cdoc);
            }
            ret.Reverse();
            return ret;
        }
        
        public List<LRow> CheckIntervalsX(List<LRow> rows, TimeSpan mindelata)
        {
            var rows2 = rows
                .GroupBy(x => x.id)
                .SelectMany(x => CheckIntervals(x.OrderBy(y => y.Dt1).ToList(), mindelata))
                .ToList();
            return rows2;
        }

        public List<LRow> CheckIntervals(List<LRow> rows, TimeSpan mindelata)
        {
            if (rows.Count < 2) return rows;
            var ret = new List<LRow>();
            ret.Add(rows[rows.Count - 1]);
            for (int i = rows.Count - 2; i >= 0; i--)
            {
                var cdoc = rows[i];
                var pdoc = ret[ret.Count - 1];
                if (cdoc.Dt2 - cdoc.Dt1 < mindelata)
                {
                    pdoc.Dt1 = cdoc.Dt1;
                    continue;
                }
                ret.Add(cdoc);
            }
            ret.Reverse();
            return ret;
        }


        public List<LDoc> MergeB(List<LDoc> docs, List<LRow> rows)
        {
            var ret = new List<LDoc>();

            var grrows = rows
                .GroupBy(x => x.Row.ID)
                .OrderBy(x => x.Key)
                .ToList();

            foreach(var doc in docs)
            {
                ret.Add(doc);
                doc.Rows = new List<LRow>();
                foreach (var grrow in grrows)
                {
                    var grow = grrow
                        .Where(x => x.Dt1 < doc.Dt2)
                        .LastOrDefault();
                    if (grow == null) continue;
                    if (grow.IsDeleted && grow.dtld < doc.Dt1) continue;
                    doc.Rows.Add(grow);
                }
            }

            return ret;
        }

        public bool HasDiff(F_LOPSD row1, F_LOPSD row2)
        {
            var columnnames = new[] { "DETE", "CLID", "DESCR", "SUMM", "PVN" };
            var prop_descr = TypeDescriptor.GetProperties(typeof(F_LOPSD));
            foreach(var name in columnnames)
            {
                var prop = prop_descr[name];
                object val1 = prop.GetValue(row1);
                object val2 = prop.GetValue(row2);
                if (!object.Equals(val1, val2)) 
                    return true;
            }
            return false;
        }

        public bool HasDiff(F_LOPS row1, F_LOPS row2)
        {
            var columnnames = new[] 
            { 
                "AC11", "AC12", "AC13", "AC14", "AC15",
                "AC21", "AC22", "AC23", "AC24", "AC25",
                "SUMM"
            };
            var prop_descr = TypeDescriptor.GetProperties(typeof(F_LOPS));
            foreach (var name in columnnames)
            {
                var prop = prop_descr[name];
                object val1 = prop.GetValue(row1);
                object val2 = prop.GetValue(row2);
                if (!object.Equals(val1, val2)) 
                    return true;
            }
            return false;
        }

        public class LDoc
        {
            public F_LOPSD Doc;
            public List<LRow> Rows;

            public DateTime Dt1 { get; set; }
            public DateTime Dt2 { get; set; }

            public int idl => Doc.IDL;
            public int id => Doc.ID.Value;
            public DateTime? ZDt => Doc.ZDT;
            public DateTime? dtld => Doc.DTLD;
            public DateTime? ODT => Doc.ODT;
            public DateTime Dete => Doc.DETE.Value;
            public string usl => Doc.USL;
            public int tpl => Doc.TPL.Value;
            public string NrX => Doc.NRX;
            public string DocTyp => Doc.DOCTYP;
            public string DocSt => Doc.DOCST;
            public string DocNr => Doc.DOCNR;
            public string ClId => Doc.CLID;
            public string ClId2 => Doc.CLID2;
            public string Descr => Doc.DESCR;
            public decimal Summ => Doc.SUMM;
            public decimal PVN => Doc.PVN;
            public int ZNR => Doc.ZNR;
            public string ZU => Doc.ZU;
        }

        public class LRow
        {
            public DateTime Dt1 { get; set; }
            public DateTime Dt2 { get; set; }
            public F_LOPS Row;
            public bool IsDeleted => Row.DTLD is not null;

            public int idl => Row.IDL;
            public int id => Row.ID.Value;
            public int DocId => Row.DOCID.Value;
            public DateTime? dtl => Row.DTL;
            public DateTime? dtld => Row.DTLD;
            public DateTime? ODT => Row.ODT;
            public string usl => Row.USL;
            public int tpl => Row.TPL.Value;
            public string AC11 => Row.AC11;
            public string AC12 => Row.AC12;
            public string AC13 => Row.AC13;
            public string AC14 => Row.AC14;
            public string AC15 => Row.AC15;
            public string AC21 => Row.AC21;
            public string AC22 => Row.AC22;
            public string AC23 => Row.AC23;
            public string AC24 => Row.AC24;
            public string AC25 => Row.AC25;
            public decimal SummC => Row.SUMMC;
            public float QV => Row.QV.Value;
            public decimal Summ => Row.SUMM;
        }

        private void bsDocs_CurrentChanged(object sender, EventArgs e)
        {
            UpdateRowList();
        }

        private void dgvDocs_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsDocs;
            tslActiveGrid.Text = "Dokumenti";
        }

        private void dgvRows_Enter(object sender, EventArgs e)
        {
            bNav.BindingSource = bsRows;
            tslActiveGrid.Text = "Rindas";
        }

        private void tsbSimpleView_Click(object sender, EventArgs e)
        {
            if (CurrentDocId == null) return;
            if (UsingSimpleView) return;
            GetData(CurrentDocId.Value);
        }

        private void tsbSplitView_Click(object sender, EventArgs e)
        {
            if (CurrentDocId == null) return;
            if (!UsingSimpleView) return;
            GetData2(CurrentDocId.Value);
        }
    }

}
