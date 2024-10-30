using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Misc;
using KlonsLIB.Data;
using KlonsF.Classes;
using Klons3.ModelsA;

namespace KlonsA.Classes
{
    public class SalarySheetRowSetInfo
    {
        public static KlonsData MyData => KlonsData.St;

        public SalarySheetInfo SalarySheet = null;

        public A_PERSONS DR_Person = null;
        public A_PERSONS_R DR_Person_r = null;

        public SalarySheetRowInfo TotalRow = null;
        public SalaryInfo TotalPersonPay = null;

        public SalarySheetRowInfo[] LinkedRows = null;

        public A_SALARY_SHEETS_R DrTotalRow = null;
        public A_SALARY_SHEETS_R[] DrLinkedRows = null;

        public EventsInfo Events = null;

        public SalarySheetRowSetInfo()
        {
        }

        public bool GetPersonRow(ErrorList err, A_PERSONS dr_person, SalarySheetInfo sh)
        {
            var table_persons = MyData.DbContextA.GetBindingListByType<A_PERSONS>();
            DR_Person = dr_person;
            var drs_persons_r = dr_person.PersonsStateRows.OrderBy(d => d.EDIT_DATE).ToArray();

            var dt2 = sh.MDT2;

            DR_Person_r = PeriodInfo.FindNearestBefore(drs_persons_r,
                dt2, d => d.EDIT_DATE);

            if (DR_Person_r == null)
            {
                var es = $"{dr_person.FNAME} {dr_person.LNAME} ({sh.CalendarMonth.Year}-{sh.CalendarMonth.Month})";
                err.AddError(es, "Norādītajam periodam nav ievadīti darbinieka dati.");
                return false;
            }
            return true;
        }

        public ErrorList SetUpFromRowA(SalarySheetRowInfo sr)
        {
            if (sr?.Row == null)
                throw new Exception("Bad init.");

            var error_list = new ErrorList();

            DR_Person = sr.Row.Person;
            DR_Person_r = sr.DR_Person_r;
            SalarySheet = sr.SalarySheet;
            error_list += SetUpFromRowZ();

            return error_list;
        }

        public ErrorList SetUpFromRowB(SalarySheetInfo sh, A_PERSONS dr_person)
        {
            var error_list = new ErrorList();
            SalarySheet = sh;

            if (!GetPersonRow(error_list, dr_person, SalarySheet))
                return error_list;

            error_list += SetUpFromRowZ();
            
            return error_list;
        }

        private ErrorList SetUpFromRowZ(SalarySheetRowInfo sr = null)
        {
            if ((sr != null && sr.Row == null) || DR_Person_r == null || SalarySheet == null)
                throw new Exception("Bad init.");

            var error_list = new ErrorList();
            ErrorList err;

            TotalRow = null;
            LinkedRows = new SalarySheetRowInfo[0];

            CheckLinkedRows(DR_Person);

            if (DrTotalRow == null || DrLinkedRows.Length == 0)
                return error_list;

            LinkedRows = new SalarySheetRowInfo[DrLinkedRows.Length];

            var error_source = string.Format("{0} {1} ({2}-{3})",
                DR_Person_r.FNAME,
                DR_Person_r.LNAME,
                SalarySheet.CalendarMonth.Year,
                SalarySheet.CalendarMonth.Month);

            GetEventList(error_list, DR_Person);

            if (error_list.Count > 0)
            {
                return error_list;
            }

            for (int i = 0; i < DrLinkedRows.Length; i++)
            {
                var dr = DrLinkedRows[i];
                if (dr == sr?.Row)
                {
                    LinkedRows[i] = sr;
                    sr.SalarySheetRowSet = this;
                    sr.Events = Events;
                    continue;
                }
                var lr = new SalarySheetRowInfo(SalarySheet, dr);
                LinkedRows[i] = lr;
                lr.SalarySheetRowSet = this;
                lr.Events = Events;
                err = lr.SetUpFromRow(dr);
                error_list += err;
            }

            if (IsSingleRow())
            {
                TotalRow = LinkedRows[0];
            }
            else
            {
                if(DrTotalRow == sr?.Row)
                {
                    TotalRow = sr;
                    TotalRow.SalarySheetRowSet = this;
                }
                else
                {
                    TotalRow = new SalarySheetRowInfo(SalarySheet, DrTotalRow);
                    TotalRow.SalarySheetRowSet = this;
                    TotalRow.Events = Events;
                    error_list += TotalRow.SetUpT(DR_Person);
                }
            }

            if (error_list.HasErrors) return error_list;

            if (TotalRow?.TotalPositionPay != null)
                TotalPersonPay = TotalRow.TotalPositionPay;

            if (error_list.HasErrors)
            {
                LinkedRows = null;
                return error_list;
            }

            return error_list;
        }


        public ErrorList SetUpFromRowX(A_SALARY_SHEETS_R dr_r)
        {
            var dr_sar1 = dr_r.SalarySheet;
            var ssh = new SalarySheetInfo(dr_sar1);
            var er = ssh.SetUpLightFromSarRow(dr_sar1);
            if (er != "OK")
                return new ErrorList("", er);
            return SetUpFromRowB(ssh, dr_r.Person);
        }

        public bool GetRow(int addmonth, out SalarySheetRowSetInfo row)
        {
            row = null;
            var table_sar = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS>();
            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            int yr = SalarySheet.YR;
            int mt = SalarySheet.MT;
            Utils.AddMonths(ref yr, ref mt, addmonth);

            var drs = table_sar_r.Where(
                d =>
                {
                    var dr_sar = d.SalarySheet;
                    return
                        d.Person == DR_Person &&
                        d.Position is null &&
                        dr_sar.YR == yr &&
                        dr_sar.MT == mt;
                }).ToArray();

            if (drs.Length == 0) return true;
            var dr = drs[0];

            var shr = new SalarySheetRowSetInfo();
            var err = shr.SetUpFromRowX(dr);
            if (err != null && err.Count > 0)
            {
                return false;
            }

            row = shr;

            return true;
        }

        public void MakeTotalsRow(A_PERSONS dr_person)
        {
            if (SalarySheet == null || SalarySheet.TotalsList == null)
                throw new Exception("Bad init.");

            var table_sar_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();
            var new_dr_sar_r = table_sar_r.NewItem();

            //new_dr_sar_r.ID = (int)MyData.KlonsAQueriesTableAdapter.SP_A_GEN_SALARY_SHEETS_R_ID();
            new_dr_sar_r.SalarySheet = SalarySheet.TotalsList;
            new_dr_sar_r.RowSetTotalRow = new_dr_sar_r;
            new_dr_sar_r.SalarySheetTotal = SalarySheet.TotalsList;
            new_dr_sar_r.SalarySheetTotal = SalarySheet.TotalsList;
            new_dr_sar_r.Person = dr_person;
            new_dr_sar_r.Position = null;
            new_dr_sar_r.XType = ESalarySheetRowType.Total;
            new_dr_sar_r.FNAME = DR_Person_r.FNAME;
            new_dr_sar_r.LNAME = DR_Person_r.LNAME;
            new_dr_sar_r.POSITION_TITLE = "KOPĀ";
            new_dr_sar_r.IS_TEMP = SalarySheet.TotalsList.IS_TEMP;

            table_sar_r.Add(new_dr_sar_r);

            DrTotalRow = new_dr_sar_r;
        }


        private void SetIntValue(ref int rec, int src)
        {
            if (rec != src) rec = src;
        }

        // seperate totalrow is used only when there are more than one linked rows
        public void CheckLinkedRows(A_PERSONS dr_person)
        {
            if (SalarySheet == null || SalarySheet.TotalsList == null)
                throw new Exception("No totals sheet.");

            var table_lapas_r = MyData.DbContextA.GetBindingListByType<A_SALARY_SHEETS_R>();

            var dt1 = SalarySheet.DT1;
            var dt2 = SalarySheet.DT2;

            var drs_lapas_r = table_lapas_r.Where(
                d =>
                d.Person == DR_Person &&
                d.SalarySheet.DT1 == dt1 &&
                d.SalarySheet.DT2 == dt2
            ).ToArray();

            var drs_total_r = drs_lapas_r.Where(
                d =>
                d.XType == ESalarySheetRowType.Total
            ).ToArray();

            var drs_linked_r = drs_lapas_r.Where(
                d =>
                d.XType != ESalarySheetRowType.Total
            ).ToArray();

            if (drs_linked_r.Length == 0)
            {
                foreach (var dr in drs_total_r)
                    table_lapas_r.Remove(dr);
                this.DrTotalRow = null;
                this.DrLinkedRows = new A_SALARY_SHEETS_R[0];
            }
            else if (drs_linked_r.Length == 1)
            {
                foreach (var dr in drs_total_r)
                    table_lapas_r.Remove(dr);

                var dr0 = drs_linked_r[0];

                this.DrTotalRow = dr0;

                 dr0.SalarySheetTotal = SalarySheet.TotalsList;
                 dr0.RowSetTotalRow = dr0;
                 dr0.XType = ESalarySheetRowType.OnlyOne;

                this.DrLinkedRows = new A_SALARY_SHEETS_R[1];
                this.DrLinkedRows[0] = this.DrTotalRow;
            }
            else
            {
                for (int i = 1; i < drs_total_r.Length; i++)
                    table_lapas_r.Remove(drs_total_r[i]);

                if (drs_total_r.Length == 0)
                {
                    MakeTotalsRow(dr_person);
                }
                else
                {
                    DrTotalRow = drs_total_r[0];
                }

                ChangeIDSXForAlgasPSRows(DrTotalRow);

                foreach (var dr in drs_linked_r)
                {
                    dr.RowSetTotalRow = DrTotalRow;
                    dr.SalarySheetTotal = null;
                    dr.XType = ESalarySheetRowType.OneOfMany;
                }

                DrLinkedRows = drs_linked_r;
            }
        }


        public A_POSITIONS_PLUSMINUS[] GetRelevantPersonPSRows()
        {
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            var drs_amps = DR_Person_r.Person.PositionsPlusMinusRows.Where(
                d =>
                d.DATE1 <= dt2 &&
                d.DATE2 >= dt1
                ).ToArray();

            return drs_amps;
        }

        public A_SALARY_PLUSMINUS[] GetAlgasAllPSRows()
        {
            if (DrTotalRow == null)
                throw new Exception("Bad init.");
            var drst = DrTotalRow.SalaryPlusMinusRows;
            var drsl = DrLinkedRows.SelectMany(d => d.SalaryPlusMinusRows);
            var drsz = drst.Concat(drsl);
            return drsz.ToArray();
        }

        public A_SALARY_PLUSMINUS[] GetAlgasPSRowsT()
        {
            if (DrTotalRow == null)
                throw new Exception("Bad init.");
            var drst = DrTotalRow.SalaryPlusMinusRows;
            return drst.ToArray();
        }

        public A_SALARY_PLUSMINUS[] GetAlgasPSRows(A_POSITIONS dr_position)
        {
            return GetAlgasAllPSRows().Where(
                d =>
                d.Position != null &&
                d.Position == dr_position
                ).ToArray();
        }

        public void CheckAlgasPS()
        {
            var table_algas_papildsummas = MyData.DbContextA.GetBindingListByType<A_SALARY_PLUSMINUS>();
            var dt1 = SalarySheet.MDT1;
            var dt2 = SalarySheet.MDT2;

            var drs_amps = GetRelevantPersonPSRows();
            var drs_algasps = GetAlgasAllPSRows();

            // delete rows without template
            foreach (var dr in drs_algasps)
            {
                if (dr.PositionsPlusMinusRow == null) continue;
                var dr2 = drs_amps
                    .Where(d => d == dr.PositionsPlusMinusRow)
                    .FirstOrDefault();
                if (dr2 == null)
                    table_algas_papildsummas.Remove(dr);
            }

            // add rows from template
            // and update rows
            foreach (var dr in drs_amps)
            {
                var dr2 = drs_algasps.Where(
                    d =>
                    d.PositionsPlusMinusRow != null &&
                    d.PositionsPlusMinusRow == dr
                    ).FirstOrDefault();
                if (dr2 == null)
                {
                    dr2 = table_algas_papildsummas.NewItem();
                    table_algas_papildsummas.Add(dr2);
                    dr2.PositionsPlusMinusRow = dr;
                    dr2.SalarySheetRow = DrTotalRow.RowSetTotalRow;
                    dr2.Person = DrTotalRow.Person;
                }

                if (dr2.Position != dr.Position) dr2.Position = dr.Position;
                if (dr2.PlusMinusType != dr.PlusMinusType) dr2.PlusMinusType = dr.PlusMinusType;
                if (dr2.PlusMinusFrom != dr.PlusMinusFromRow) dr2.PlusMinusFrom = dr.PlusMinusFromRow;
                if (dr2.DESCR != dr.DESCR) dr2.DESCR = dr.DESCR;
                if (dr2.RATE != dr.RATE) dr2.RATE = dr.RATE;
                if (dr2.RATE_TYPE != dr.RATE_TYPE) dr2.RATE_TYPE = dr.RATE_TYPE;
            }
        }

        public void ChangeIDSXForAlgasPSRows(A_SALARY_SHEETS_R dr_total)
        {
            if (DrTotalRow == null) return;
            var drs_ps = DrTotalRow.SalaryPlusMinusRows.ToList();
            foreach (var dr in drs_ps)
            {
                dr.SalarySheetRow = dr_total;
            }
        }

        public void DeleteAlgasPSRows()
        {
            if (DrTotalRow == null)
                throw new Exception("Bad init.");
            var drs_ps = DrTotalRow.SalaryPlusMinusRows.ToList();
            foreach (var dr in drs_ps)
            {
                MyData.DbContextA.A_SALARY_PLUSMINUS.Remove(dr);
            }
        }

        public bool GetEventList(ErrorList errorlist, A_PERSONS dr_person)
        {
            Events = new EventsInfo();
            string ret = Events.ProcessData(dr_person, SalarySheet.DT1, SalarySheet.DT2);
            if (ret == "OK") return true;
            errorlist.AddPersonError(dr_person, ret);
            return false;
        }

        public bool IsSingleRow()
        {
            return LinkedRows.Length == 1;
        }

        public ErrorList FillRow()
        {
            var dt1 = SalarySheet.DT1;
            var dt2 = SalarySheet.DT2;

            TotalPersonPay = new SalaryInfo();
            var sci = new SalaryCalcTInfo(this, TotalPersonPay, false);

            var err_list = sci.FillRow();
            if (err_list.HasErrors) return err_list;

            sci.WriteData();
            return err_list;
        }

        public int CountCalendarDays()
        {
            return this.Events.HireFire.CountDates(SalarySheet.DT1, SalarySheet.DT2);
        }

        public TimeSheetRowSetList GetDLRowSetList(int addmt = 0)
        {
            var ret = new TimeSheetRowSetList();
            foreach (var lr in LinkedRows)
                ret.Add(lr.GetDLRowSet(addmt, lr.Row.Position));
            return ret;
        }
    }

}