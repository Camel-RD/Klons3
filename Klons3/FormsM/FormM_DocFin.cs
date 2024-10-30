using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsM.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using KlonsLIB.Components;
using Klons3.ModelsM;
using Klons3.ModelsF;

namespace KlonsM.FormsM
{
    public partial class FormM_DocFin : MyFormBaseF
    {
        public FormM_DocFin()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            dgvRepByPVNRate.AutoGenerateColumns = false;
            dgvAcc.AutoGenerateColumns = false;
            dgvRepByPVNRate.DisableAllColumnSorting();
            dgvAcc.DisableAllColumnSorting();
        }

        private void FormM_DocFin_Load(object sender, EventArgs e)
        {
            GetData();
            bsRepByPVNRate.DataSource = DocPVNRateRepRows;
            bsAccRows.DataSource = DocAccRepRows;
        }

        public M_DOCS Doc;
        public List<DocPVNRateRepRow> DocPVNRateRepRows = new List<DocPVNRateRepRow>();
        public List<DocAccRepRow> DocAccRepRows = new List<DocAccRepRow>();
        public DocAccData DocFinData = null;

        public static void ShowDialog(M_DOCS dr_doc)
        {
            if (dr_doc.ID == 0)
            {
                MyMainFormBase.MyInstance.ShowWarning("Dokuments nav saglabāts");
                return;
            }
            var fm = new FormM_DocFin();
            fm.Doc = dr_doc;
            fm.ShowDialog();
        }

        public void GetData()
        {
            var dr_doc = Doc;
            if(dr_doc == null)
            {
                Close();
                return;
            }
            var pvndata = PVNCalc.GetPVNData(dr_doc);
            if (pvndata.Err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, pvndata.Err);
                return;
            }
            DocFinData = pvndata;
            DocPVNRateRepRows = new List<DocPVNRateRepRow>();
            DocAccRepRows = new List<DocAccRepRow>();
            var totalreprow = new DocPVNRateRepRow()
            {
                PVNRateId = "Kopā"
            };
            foreach (var x in pvndata.DocPVNTypeData)
            {
                var reprow = new DocPVNRateRepRow();
                reprow.SetFrom(x);
                DocPVNRateRepRows.Add(reprow);
                totalreprow.PVNBaze += reprow.PVNBaze;
                totalreprow.PVN += reprow.PVN;
                totalreprow.Total += reprow.Total;
                totalreprow.ReversePVN += reprow.ReversePVN;
            }
            DocPVNRateRepRows.Add(totalreprow);
            foreach(var x in pvndata.RowAccDataTotalB)
            {
                var reprow = new DocAccRepRow();
                reprow.SetFrom(x);
                DocAccRepRows.Add(reprow);
            }
        }

        public bool SaveDataF()
        {
            //if (!dgvAcc.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = MyData.DbContextF.Save();
            return rt.IsSuccess();
        }

        public bool SaveDataM()
        {
            //if (!dgvAcc.EndEditX()) return false;
            if (!this.Validate()) return false;
            var rt = MyData.DbContextM.Save();
            return rt.IsSuccess();
        }


        public M_STORES GetPartnerId(M_DOCS dr_doc)
        {
            if (SomeDataDefsM.IsStorePartner(dr_doc.XStoreOutType))
                return dr_doc.StoreOut;
            else
                return dr_doc.StoreIn;
        }

        public string[] GetAccFinList()
        {
            var ret = DocAccRepRows.Select(x => x.DebFin)
                .Union(DocAccRepRows.Select(x => x.CredFin))
                .ToArray();
            return ret;
        }

        public string[] GetAccPVNList()
        {
            var ret = DocAccRepRows.Select(x => x.DebPVN)
                .Union(DocAccRepRows.Select(x => x.CredPVN))
                .Where(x => !x.IsNOE())
                .ToArray();
            return ret;
        }

        public string TestAccFin(string[] accs)
        {
            var missingacc = new List<string>();
            foreach(var acc in accs)
            {
                var table_accplan = MyData.DbContextF.F_ACP21;
                if (table_accplan.Find(acc) != null) continue;
                missingacc.Add(acc);
            }
            if (missingacc.Count == 0) return "OK";
            var ret = string.Join(", ", missingacc);
            ret = "Šie konti nav kontu plānā:\n" + ret;
            return ret;
        }

        public string TestAccPVN(string[] accs)
        {
            var missingacc = new List<string>();
            foreach (var acc in accs)
            {
                var table_accplan = MyData.DbContextF.F_ACP25;
                if (table_accplan.Find(acc) != null) continue;
                missingacc.Add(acc);
            }
            if (missingacc.Count == 0) return "OK";
            var ret = string.Join(", ", missingacc);
            ret = "Šie PVN kods nav kodu sarakstā:\n" + ret;
            return ret;
        }
        
        public string GetPartnerCode(M_DOCS dr_doc)
        {
            var dr_partner = GetPartnerId(dr_doc);
            string clid = dr_partner.CODE;
            return clid;
        }

        public string TestPartnerCode(M_DOCS dr_doc)
        {
            string clid = GetPartnerCode(dr_doc);
            if (MyData.DbContextF.F_PERSONS.Find(clid) == null)
                return $"Personu sarakstā nav personas ar kodu {clid}";
            return "OK";
        }

        public void DoDeleteFinDocA(M_DOCS dr_doc)
        {
            DataTasksM.DetachFinDocByIdDocM(dr_doc.ID);
            DataLoaderM.Call_SP_M_DEL_FINDOC(dr_doc.ID);
        }

        public string GetDocTypeCode(M_DOCS dr_doc)
        {
            switch (dr_doc.XDocType)
            {
                case EDocType.Nenoteikts: return null;
                case EDocType.Iepirkums:
                    return "PPRks";
                case EDocType.Realizācija:
                    return "PPRdi";
                case EDocType.Atgriezts_piegādātājam:
                case EDocType.Atgriezts_no_pircēja:
                case EDocType.No_noliktavas:
                case EDocType.Uz_noliktavu:
                    return "PPR";
                case EDocType.Kredītrēķins_no_piegādātāja:
                case EDocType.Kredītrēķins_pircējam:
                    return "Kr.rēķ.";
                case EDocType.Pārvietots:
                    return "IPP";
                case EDocType.Sākuma_atlikums:
                case EDocType.Norakstīts:
                case EDocType.Pierakstīts:
                case EDocType.Izlietots:
                case EDocType.Saražots:
                    return "Akts";
                case EDocType.Saņemti_pakalpojumi:
                case EDocType.Sniegti_pakalpojumi:
                    return "Rēķ";
            }
            return null;
        }

        public string TestDocTypeCode(M_DOCS dr_doc)
        {
            var code = GetDocTypeCode(dr_doc);
            var table = MyData.DbContextF.F_DOCTYP;
            var dr = table.Find(code);
            if (dr != null) return "OK";
            var ret = $"Dokumentu veidu saraksts nesatur ierakstu ar kodu {code}";
            return ret;
        }

        public string GetDocDescription(M_DOCS dr_doc)
        {
            var descr = dr_doc.TransactionType.NAME;
            if (!descr.IsNOE() && descr != "?" && descr != ".?") return descr.ToLower();
            var dr_doctype = dr_doc.DocType;
            descr = dr_doctype.NAME?.ToLower();
            return descr;
        }

        public ErrorList CanDoAccounting(M_DOCS dr_doc)
        {
            var ret = new ErrorList();

            if (dr_doc.XState == EDocState.Iekontēts)
            {
                ret.AddError("", "Dokuments jau ir iekontēts.");
                return ret;
            }

            if (dr_doc.XState != EDocState.Iegrāmatots)
            {
                ret.AddError("", "Dokuments nav iegrāmatots.");
                return ret;
            }

            if (!SomeDataDefsM.AutoMakeFinOps(dr_doc.XDocType))
            {
                ret.AddError("", "Šim dokumenta veidam netiek veidots automātisks kontējums.\n"+
                    "Kontēšana jāveic par mēneša kopsavilkumam.");
                return ret;
            }

            var table_fdocs = MyData.DbContextF.F_OPSD;
            var table_frows = MyData.DbContextF.F_OPS;
            if (MyData.DbContextF.HasChanges<F_OPSD>() || MyData.DbContextF.HasChanges<F_OPS>())
                ret.AddError("", "Ir nesaglabātas izmaiņas finanšu grāmatvedības dokumentos.");

            var err = TestPartnerCode(dr_doc);
            if (err != "OK") ret.AddError("", err);

            var accfin = GetAccFinList();
            err = TestAccFin(accfin);
            if (err != "OK") ret.AddError("", err);

            var accpvn = GetAccPVNList();
            err = TestAccPVN(accpvn);
            if (err != "OK") ret.AddError("", err);

            err = TestDocTypeCode(dr_doc);
            if (err != "OK") ret.AddError("", err);

            return ret;
        }

        public void SetDocIekontēts(M_DOCS dr_doc, bool hasautofinops)
        {
            dr_doc.XState = EDocState.Iekontēts;
            dr_doc.XDoAutoFinOps = hasautofinops;
        }

        public void ClearDocIekontēts(M_DOCS dr_doc)
        {
            dr_doc.XState = EDocState.Iegrāmatots;
            dr_doc.XDoAutoFinOps = false;
        }

        public void DoAccountingA(M_DOCS dr_doc)
        {
            if (DocAccRepRows.Count == 0)
            {
                MyMainForm.ShowWarning("Nav kontējuma ierakstu.");
                return;
            }

            var table_fdocs = MyData.DbContextF.F_OPSD;
            var table_frows = MyData.DbContextF.F_OPS;

            var dr_fdoc = new F_OPSD();
            dr_fdoc.DETE = dr_doc.DT;
            dr_fdoc.ZU = MyData.CurrentUserName;
            dr_fdoc.ZDT = DateTime.Now;
            dr_fdoc.DOCTYP = GetDocTypeCode(dr_doc);
            dr_fdoc.DOCST = dr_doc.SR;
            dr_fdoc.DOCNR = dr_doc.NR;
            dr_fdoc.CLID = GetPartnerCode(dr_doc);
            dr_fdoc.DESCR = GetDocDescription(dr_doc);
            dr_fdoc.SUMM = DocFinData.PVNBase + DocFinData.PVN;
            dr_fdoc.PVN = DocFinData.SumPVNRows();
            table_fdocs.Add(dr_fdoc);
            foreach(var accdata in DocAccRepRows)
            {
                var dr_frow = new F_OPS();
                dr_frow.DOCID = dr_fdoc.ID;
                dr_frow.DOCRow = dr_fdoc;
                dr_frow.ZDT = DateTime.Now;
                dr_frow.AC11 = accdata.DebFin;
                dr_frow.AC15 = accdata.DebPVN;
                dr_frow.AC21 = accdata.CredFin;
                dr_frow.AC25 = accdata.CredPVN;
                dr_frow.SUMM = accdata.Amount;
                dr_frow.SUMMC = accdata.Amount;
                dr_frow.CUR = "EUR";
                table_frows.Add(dr_frow);
            }

        }

        public bool DoAccounting()
        {
            var err = CanDoAccounting(Doc);
            if (err.HasErrors)
            {
                FormM_ErrorList.ShowErrorList(this, err);
                return false;
            }
            DoDeleteFinDocA(Doc);
            DoAccountingA(Doc);
            bool rt = SaveDataF();
            if (!rt)
            {
                DoDeleteFinDocA(Doc);
                return false;
            }
            SetDocIekontēts(Doc, true);
            SaveDataM();
            return true;
        }

        public void DoUseManualAccounting()
        {
            var dr_doc = Doc;

            if (dr_doc.XState == EDocState.Iekontēts)
            {
                MyMainForm.ShowWarning("Dokuments jau ir iekontēts.");
                return;
            }

            if (dr_doc.XState != EDocState.Iegrāmatots)
            {
                MyMainForm.ShowWarning("Dokuments nav iegrāmatots.");
                return;
            }

            if (DocAccRepRows.Count == 0)
            {
                MyMainForm.ShowWarning("Nav kontējuma ierakstu.");
                return;
            }

            if (!SomeDataDefsM.AutoMakeFinOps(dr_doc.XDocType))
            {
                MyMainForm.ShowWarning("Šim dokumenta veidam netiek veidots automātisks kontējums.\n" +
                    "Kontēšana jāveic par mēneša kopsavilkumam.");
                return;
            }

            SetDocIekontēts(Doc, false);
            SaveDataM();
        }

        public void DoClearAccounting()
        {
            var dr_doc = Doc;

            if (dr_doc.XState != EDocState.Iekontēts)
            {
                MyMainForm.ShowWarning("Dokuments nav kontēts.");
                return;
            }

            if (!SomeDataDefsM.AutoMakeFinOps(dr_doc.XDocType))
            {
                MyMainForm.ShowWarning("Šim dokumenta veidam netiek veidots kontējums.\n" +
                    "Kontēšana jāveic par mēneša kopsavilkumam.");
                return;
            }

            if (!dr_doc.XDoAutoFinOps)
            {
                MyMainForm.ShowWarning("Dokumentā ir norādīts, ka tas izmanto manuālu kontējumu.\n"+
                    "Dokuments tiks atzīmēts kā neiekontēts\n"+
                    "bet grāmatvedim pašam ir jādzēs veiktie kontējumi.");
            }
            
            DoDeleteFinDocA(Doc);
            var rt = SaveDataF();
            if (!rt) return;
            ClearDocIekontēts(Doc);
            SaveDataM();
        }

        private void btMakeFinDoc_Click(object sender, EventArgs e)
        {
            DoAccounting();
            DialogResult = DialogResult.OK;
        }

        private void btDeleteFinDoc_Click(object sender, EventArgs e)
        {
            DoClearAccounting();
            DialogResult = DialogResult.OK;
        }

        private void btCustomFinDoc_Click(object sender, EventArgs e)
        {
            DoUseManualAccounting();
            DialogResult = DialogResult.OK;
        }
    }

    public class DocPVNRateRepRow
    {
        public string PVNRateId { get; set; } = null;
        public string PVNRateName { get; set; } = null;
        public bool IsReversePVN { get; set; } = false;
        public string SIsReversePVN  => IsReversePVN ? "✕" : "";
        public decimal PVNBaze { get; set; } = 0M;
        public decimal PVNRate { get; set; } = 0M;
        public decimal PVN { get; set; } = 0M;
        public decimal ReversePVN { get; set; } = 0M;
        public decimal Total { get; set; } = 0M;

        public void SetFrom(DocPVNRateData data)
        {
            PVNRateId = data.PVNRateCode;
            PVNRateName = data.PVNRateName;
            IsReversePVN = data.IsReversePVN;
            PVNBaze = data.PVNBase;
            PVNRate = data.PVNRate;
            PVN = data.PVN;
            ReversePVN = data.ReversePVN;
            Total = data.PVNBase + data.PVN;
        }
    }

    public class DocAccRepRow
    {
        public string DebFin { get; set; } = null;
        public string DebPVN { get; set; } = null;
        public string CredFin { get; set; } = null;
        public string CredPVN { get; set; } = null;
        public decimal Amount { get; set; } = 0M;

        public void SetFrom(RowAccDataTotalB data)
        {
            DebFin = data.Acc.DebFin;
            DebPVN = data.Acc.DebPVN;
            CredFin = data.Acc.CredFin;
            CredPVN = data.Acc.CredPVN;
            Amount = data.Amount;
        }
    }
}
