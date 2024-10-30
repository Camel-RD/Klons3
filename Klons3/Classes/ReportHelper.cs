using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using KlonsF.Classes;
using KlonsLIB.Misc;
using KlonsLIB.Forms;
using Microsoft.Reporting.WinForms;
using Klons3.ModelsFRep;
using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp;
using KlonsLIB;
using Klons3.ModelsF;

namespace KlonsF.Classes
{

    public class ReportHelperF: KlonsLIB.Forms.ReportHelper
    {
        public KlonsData MyData => KlonsData.St;
        public DbContextFRep MyDataSet => KlonsData.St.DbContextFRep;

        public override bool CheckForErrors(Action act)
        {
            var rt = KlonsData.St.DbContextF.Save();
            bool haschanges = KlonsData.St.DbContextF.HasChanges();

            if (haschanges)
            {
                KlonsData.St.MyMainForm.ShowWarning("Iespējams, ka datu tabulās ir nesaglabātas izmaiņas.");
            }
            
            try
            {
                act();
                return true;
            }
            catch (Exception e)
            {
                MyException e1 = new MyException(
                    "Neizdevās sagatavot atskaiti!\n" +
                    "(iespējams, ka kļūda programmā).", e);
                Form_Error.ShowException(e1);
            }
            return false;
        }

        public void PrepareRops1a(IList<F_ROPS1A> reprows)
        {
            foreach (var rv in reprows)
            {
                rv.RAC2 = rv.RAC2.Nz();
                rv.RAC3 = rv.RAC3.Nz();
                rv.RAC4 = rv.RAC4.Nz();
                rv.RAC5 = rv.RAC5.Nz();
                rv.SAC1 = $"{rv.RAC},{rv.RAC2},{rv.RAC3},{rv.RAC4},{rv.RAC5}";
            }
        }
        public void PrepareRops1aForKO(IList<F_ROPS1A> reprows)
        {
            foreach (var rv in reprows)
            {
                string doctyp = rv.DOCTYP1.Nz();
                string docst = rv.DOCST.Nz();
                string docnr = rv.DOCNR.Nz();
                string docstr = doctyp;
                
                if (docstr != "" && docst != "") 
                    docstr += " " + docst;
                else 
                    docstr += docst;
                
                if (docstr != "" && docnr != "") 
                    docstr += " " + docnr;
                else 
                    docstr += docnr;

                string clstr = "";
                if (!string.IsNullOrEmpty(rv.CLID2))
                {
                    clstr = rv.NAME.Nz();
                    clstr = rv.NAME.Nz();
                    string clregnr = rv.REGNR.Nz();
                    if (clstr != "" && clregnr != "")
                        clstr += " " + clregnr;
                    else
                        clstr += clregnr;
                }
                string descr = rv.DESCR.Nz();
                string descr2 = docstr;
                if (descr2 != "" && clstr != "")
                    descr2 += "\n" + clstr;
                else
                    descr2 += clstr;

                if (descr2 != "" && descr != "")
                    descr2 += "\n" + descr;
                else
                    descr2 += descr;
                rv.DESCR2 = descr2;

                if (rv.CLID2 == null)
                {
                    rv.CLID2 = rv.CLID;
                    rv.NAME2 = rv.NAME;
                    rv.REGNR2 = rv.REGNR;
                }
            }
        }

        public void PrepareRops2a(IList<F_ROPS2A> reprows)
        {
            decimal d1;
            foreach (var rv in reprows)
            {
                d1 = rv.SDB.Value - rv.SCR.Value;
                rv.ADB = 0.0M;
                rv.ACR = 0.0M;
                rv.BDB = 0.0M;
                rv.BCR = 0.0M;
                rv.B0 = d1;
                if (d1 > 0)
                    rv.ADB = d1;
                else
                    rv.ACR = -d1;
                d1 += rv.TDB.Value - rv.TCR.Value;
                if (d1 > 0)
                    rv.BDB = d1;
                else
                    rv.BCR = -d1;

            }
        }

        public void PrepareRops2a1(IList<F_ROPS2A1> reprows)
        {
            decimal d1;
            foreach (var rv in reprows)
            {
                d1 = rv.SDB.Value - rv.SCR.Value;
                rv.ADB = 0.0M;
                rv.ACR = 0.0M;
                rv.BDB = 0.0M;
                rv.BCR = 0.0M;
                rv.B0 = d1;
                if (d1 > 0)
                    rv.ADB = d1;
                else
                    rv.ACR = -d1;
                d1 += rv.TDB.Value - rv.TCR.Value;
                if (d1 > 0)
                    rv.BDB = d1;
                else
                    rv.BCR = -d1;

            }
        }

        public void PrepareRops2aRAC(IList<F_ROPS2A> reprows)
        {
            string s;
            foreach (var rv in reprows)
            {
                s = $"Konts: [{rv.AC}]";
                if (!string.IsNullOrEmpty(rv.NAME))
                    s = $"{s} {rv.NAME}";
                if (!string.IsNullOrEmpty(rv.CLID))
                    s = $"{s}\nPersona: [{rv.CLID}]";
                if (!string.IsNullOrEmpty(rv.NAME1))
                    s = $"{s} {rv.NAME1}";
                rv.RNAME = s;
            }
        }
        public void PrepareRops2aRAC(IList<F_ROPS2A> reprows, 
            string ac, string acname, string clid, string cname)
        {
            if (reprows.Count == 0)
            {
                var dr = new F_ROPS2A();
                dr.AC = ac;
                dr.NAME = acname;
                dr.CLID = clid;
                dr.NAME1 = cname;
                reprows.Add(dr);
            }
            string s;
            foreach (var rv in reprows)
            {
                s = $"Konts: [{rv.AC}]";
                if (!string.IsNullOrEmpty(rv.NAME))
                    s = $"{s} {rv.NAME}";
                if (!string.IsNullOrEmpty(rv.CLID))
                    s = $"{s}\nPersona: [{rv.CLID}]";
                if (!string.IsNullOrEmpty(rv.NAME1))
                    s = $"{s} {rv.NAME1}";
                rv.RNAME = s;
            }
        }

        public void CheckRops2aHasRow(IList<F_ROPS2A> reprows)
        {
            if (reprows.Count == 0)
            {
                reprows.Add(new F_ROPS2A());
            }
        }

        void CheckTREPMTRow(F_TREPMT row)
        {
            for (int i = 1; i <= 12; i++)
            {
                if (TREPMTRow_GetM(row, i) == null) TREPMTRow_SetM(row, i, 0M);
                if (TREPMTRow_GetN(row, i) == null) TREPMTRow_SetN(row, i, 0M);
            }
            if (row.MT == null) row.MT = 0M;
            if (row.NT == null) row.NT = 0M;
        }

        decimal? TREPMTRow_GetM(F_TREPMT row, int k)
        {
            return k switch {
                1 => row.M1,
                2 => row.M2,
                3 => row.M3,
                4 => row.M4,
                5 => row.M5,
                6 => row.M6,
                7 => row.M7,
                8 => row.M8,
                9 => row.M9,
                10 => row.M10,
                11 => row.M11,
                12 => row.M12,
                _ => throw new ArgumentException("bad index")
            };
        }

        void TREPMTRow_SetM(F_TREPMT row, int k, decimal? value)
        {
            switch(k)
            {
                case 1: row.M1 = value; break;
                case 2: row.M2 = value; break;
                case 3: row.M3 = value; break;
                case 4: row.M4 = value; break;
                case 5: row.M5 = value; break;
                case 6: row.M6 = value; break;
                case 7: row.M7 = value; break;
                case 8: row.M8 = value; break;
                case 9: row.M9 = value; break;
                case 10: row.M10 = value; break;
                case 11: row.M11 = value; break;
                case 12: row.M12 = value; break;
                default: throw new ArgumentException("bad index");
            };
        }

        decimal? TREPMTRow_GetN(F_TREPMT row, int k)
        {
            return k switch
            {
                1 => row.N1,
                2 => row.N2,
                3 => row.N3,
                4 => row.N4,
                5 => row.N5,
                6 => row.N6,
                7 => row.N7,
                8 => row.N8,
                9 => row.N9,
                10 => row.N10,
                11 => row.N11,
                12 => row.N12,
                _ => throw new ArgumentException("bad index")
            };
        }

        void TREPMTRow_SetN(F_TREPMT row, int k, decimal? value)
        {
            switch (k)
            {
                case 1: row.N1 = value; break;
                case 2: row.N2 = value; break;
                case 3: row.N3 = value; break;
                case 4: row.N4 = value; break;
                case 5: row.N5 = value; break;
                case 6: row.N6 = value; break;
                case 7: row.N7 = value; break;
                case 8: row.N8 = value; break;
                case 9: row.N9 = value; break;
                case 10: row.N10 = value; break;
                case 11: row.N11 = value; break;
                case 12: row.N12 = value; break;
                default:
                    throw new ArgumentException("bad index");
            };
        }

        public void PrepareTRepMT(IList<F_TREPMT> reprows)
        {
            foreach (var rv in reprows)
            {
                CheckTREPMTRow(rv);

                rv.MT = rv.M1 + rv.M2 + rv.M3 + rv.M4 + rv.M5 + rv.M6 + rv.M7 +
                        rv.M8 + rv.M9 + rv.M10 + rv.M11 + rv.M12;
                rv.NT = rv.N1 + rv.N2 + rv.N3 + rv.N4 + rv.N5 + rv.N6 + rv.N7 +
                        rv.N8 + rv.N9 + rv.N10 + rv.N11 + rv.N12;
                /*
                for (i = k1; i < k1 + 25; i++)
                {
                    rv[i] = (decimal) Math.Round((decimal) rv[i], 2);
                }*/
            }
        }
        public void PrepareTRepMTForNPMT1(IList<F_TREPMT> reprows, int month)
        {
            foreach (var rv in reprows)
            {
                CheckTREPMTRow(rv);

                if (rv.AD == "B" || rv.AD == "C")
                {
                    rv.MT = rv.M1 + rv.M2 + rv.M3 + rv.M4 + rv.M5 + rv.M6 + rv.M7 +
                            rv.M8 + rv.M9 + rv.M10 + rv.M11 + rv.M12;
                }
                if (rv.AD == "A" || rv.AD == "D")
                {
                    rv.M1 = rv.MT - rv.NT + rv.M1 - rv.N1;
                    rv.M2 = rv.M1 + rv.M2 - rv.N2;
                    rv.M3 = rv.M2 + rv.M3 - rv.N3;
                    rv.M4 = rv.M3 + rv.M4 - rv.N4;
                    rv.M5 = rv.M4 + rv.M5 - rv.N5;
                    rv.M6 = rv.M5 + rv.M6 - rv.N6;
                    rv.M7 = rv.M6 + rv.M7 - rv.N7;
                    rv.M8 = rv.M7 + rv.M8 - rv.N8;
                    rv.M9 = rv.M8 + rv.M9 - rv.N9;
                    rv.M10 = rv.M9 + rv.M10 - rv.N10;
                    rv.M11 = rv.M10 + rv.M11 - rv.N11;
                    rv.M12 = rv.M11 + rv.M12 - rv.N12;
                }
                if (rv.AD == "A")
                {
                    rv.M12 = rv.M11;
                    rv.M11 = rv.M10;
                    rv.M10 = rv.M9;
                    rv.M9 = rv.M8;
                    rv.M8 = rv.M7;
                    rv.M7 = rv.M6;
                    rv.M6 = rv.M5;
                    rv.M5 = rv.M4;
                    rv.M4 = rv.M3;
                    rv.M3 = rv.M2;
                    rv.M4 = rv.M3;
                    rv.M1 = rv.MT - rv.NT;
                }
                if (rv.AD == "A" || rv.AD == "D")
                {
                    rv.MT = 0.00M;
                    rv.NT = 0.00M;
                }
                if (rv.AD == "A" || rv.AD == "D")
                {
                    for (int i = 1 + month; i < 13; i++)
                    {
                        TREPMTRow_SetM(rv, i, 0M);
                    }
                }
            }
        }

        public void PrepareTRepMTForNPMT3(IList<F_TREPMT> reprows)
        {
            foreach (var rv in reprows)
            {
                CheckTREPMTRow(rv);
                rv.MT = 0.00M;
                for (int i = 1; i <= 13; i++)
                {
                    decimal d = TREPMTRow_GetN(rv, i).Value - TREPMTRow_GetM(rv, i).Value;
                    TREPMTRow_SetM(rv, i, d);
                    rv.MT += d;
                }
            }
        }

        public void PrepareTRepDarz1(List<F_TREPDARZ1> reprows)
        {
            for (int i = 0; i < reprows.Count; i++)
            {
                var rv = reprows[i];
                rv.RID = i;
                rv.DOCTYP = rv.DOCTYP.Nz();
                rv.DOCST = rv.DOCST.Nz();
                rv.DOCNR = rv.DOCNR.Nz();
                rv.DOCSTR = $"{rv.DOCTYP} {rv.DOCST} {rv.DOCNR}";
            }
        }

        public void PrepareTRepPVNZ1(List<F_TREPPVNZ1> reprows)
        {
            for (int i = 0; i < reprows.Count; i++)
            {
                var rv = reprows[i];
                rv.RID = i;
                rv.DOCTYP = rv.DOCTYP.Nz();
                rv.DOCST = rv.DOCST.Nz();
                rv.DOCNR = rv.DOCNR.Nz();
                rv.DOCSTR = $"{rv.DOCTYP} {rv.DOCST} {rv.DOCNR}";
            }
        }

        public decimal GetBal0ForDate(string pac, DateTime pdt)
        {
            var list = DataLoaderF.ROPS2A_GetBy_apgr_01(pdt, pdt, pac);
            PrepareRops2a(list);
            var rv = list[0];
            return rv.SDB.Value - rv.SCR.Value;
        }

        public void FillBalA2FromBalA21(string balid, 
            List<F_BALA2> rows_bala22,
            List<F_BALA21> rows_bala21)
        {
            var list = MyData.DbContextF.F_BALA2.Local.ToList();
            foreach (var dr in rows_bala22)
            {
                if (dr.BALID != balid) continue;
                var dr2 = rows_bala21.Find(x => x.ID ==  dr.ID);
                if (dr2 == null)
                {
                    dr.S1 = 0.00M;
                    dr.S2 = 0.00M;
                    continue;
                }
                dr.S1 = dr2.S1 ?? 0.0M;
                dr.S2 = dr2.S2 ?? 0.0M;
                /*
                if (dr.dc == "PA")
                {
                    dr.S1 = -dr.S1;
                    dr.S2 = -dr.S2;
                }*/
            }
        }

    }
}
