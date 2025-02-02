﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Klons3.ModelsA;
using KlonsA.Classes;
using KlonsF.Classes;
using KlonsLIB.Data;
using KlonsLIB.Forms;
using KlonsLIB.Misc;

namespace KlonsA.Forms
{
    public partial class FormA_PaymentsByPerson : MyFormBaseF
    {
        public FormA_PaymentsByPerson()
        {
            InitializeComponent();
            CheckMyFontAndColors();
            bsPersons.SetFilter<A_PERSONS>(x => x.USED == 1);
        }

        public List<PersonPayRow> PersonPayRows = new List<PersonPayRow>();
        public int AddPercent = 0;

        private void Form_PaymentsByPerson_Load(object sender, EventArgs e)
        {
            cbPerson.Text = null;
            if (!MyData.Settings.SpecialFeatures)
            {
                miMaksājumuPārskats.Visible = false;
                dgcCalcVal.Visible = false;
            }
        }

        public void DoFilter()
        {
            PersonPayRows = new List<PersonPayRow>();
            DateTime dt1 = DataLoaderA.LoadedDT1;
            DateTime dt2 = DataLoaderA.LoadedDT2;
            if (!string.IsNullOrEmpty(tbDate1.Text))
                dt1 = Utils.StringToDate(tbDate1.Text).Value;
            if (!string.IsNullOrEmpty(tbDate2.Text))
                dt2 = Utils.StringToDate(tbDate2.Text).Value;
            A_PERSONS dr_person = null;
            if (cbPerson.SelectedIndex > -1 && cbPerson.SelectedValue != null)
                dr_person = cbPerson.SelectedValue as A_PERSONS;
            if (dt1 < DataLoaderA.LoadedDT1) dt1 = DataLoaderA.LoadedDT1;
            if (dt2 > DataLoaderA.LoadedDT2) dt2 = DataLoaderA.LoadedDT2;

            string sdt1 = Utils.DateToString(dt1);
            string sdt2 = Utils.DateToString(dt2);

            if (sdt1 != tbDate1.Text) tbDate1.Text = sdt1;
            if (sdt2 != tbDate2.Text) tbDate2.Text = sdt2;

            /*
            if (idp == -1)
            {
                bsRows.DataSource = PersonPayRows;
                return;
            }
            */

            var table_payrows = MyData.DbContextA.BL_A_PAYLISTS_R;
            var drs_pay = table_payrows
                .Where(d =>
                    (dr_person == null || d.Person == dr_person) &&
                    d.PayList.DT >= dt1 &&
                    d.PayList.DT <= dt2)
                .OrderBy(d => d.PayList.DT)
                .ThenBy(d => d.PayList.SNR)
                .ThenBy(d => d.PayList.ID)
                .ToList();

            foreach(var dr_pay in drs_pay)
            {
                var pprow = new PersonPayRow()
                {
                    PaylistRRow = dr_pay,
                    PersonPayRowType = EPersonPayRowType.Pay,
                    Date = dr_pay.PayList.DT,
                    ListDescription = dr_pay.PayList.DESCR,
                    PositionTitle = dr_pay.Position.TITLE,
                    NotPaid = dr_pay.PAY0,
                    NotPaidAdvance = dr_pay.ADVANCE0,
                    NotPaidWithhold = dr_pay.WITHHOLDINGS0,
                    NotPaidT = dr_pay.TPAY0,
                    PayT = dr_pay.TPAY,
                    Diff = dr_pay.TPAY0 - dr_pay.TPAY,
                    CalcIIN = dr_pay.IIN0,
                    TakeIIN = dr_pay.IIN,
                };
                PersonPayRows.Add(pprow);
            }
            if (dr_person == null)
            {
                bsRows.DataSource = PersonPayRows;
                return;
            }

            var table_salary_r = MyData.DbContextA.BL_A_SALARY_SHEETS_R;
            var drs_salary_r = table_salary_r
                .Where(d =>
                    d.Person == dr_person &&
                    d.XType != ESalarySheetRowType.Total &&
                    d.SalarySheet.DT2.Value.IsBetween(dt1, dt2))
                .OrderBy(d => d.SalarySheet.DT2.Value)
                .ToList();

            foreach (var dr_salary_r in drs_salary_r)
            {
                var pprow = new PersonPayRow()
                {
                    SalarySheetRRow = dr_salary_r,
                    PersonPayRowType = EPersonPayRowType.Calc,
                    Date = dr_salary_r.SalarySheet.DT2.Value,
                    ListDescription = dr_salary_r.SalarySheet.DESCR,
                    PositionTitle = dr_salary_r.Position.TITLE,
                    Calc = dr_salary_r.PAY,
                    CalcAdvance = dr_salary_r.ADVANCE,
                    CalcWithhold = dr_salary_r.MINUS_AFTER_IIN,
                    CalcT = dr_salary_r.PAYT,
                    CalcIIN = dr_salary_r.IIN_AMOUNT
                };
                PersonPayRows.Add(pprow);
            }

            PersonPayRows = PersonPayRows
                .OrderBy(d => d.Date)
                .ThenBy(d => d.PersonPayRowType)
                .ToList();

            bsRows.DataSource = PersonPayRows;
        }

        private bool IsKaseRow(PersonPayRow row)
        {
            string descr = row.ListDescription;
            if (descr.IsNOE()) return false;
            string s = descr.ToLower();
            return s.Contains("kase") || s.Contains("kasē");
        }

        public void CheckKase(List<PersonPayRow> rows, int last_pay_percent = 0)
        {
            var rows_pay = rows
                .Where(x =>
                    x.PersonPayRowType == EPersonPayRowType.Pay)
                .ToList();
            var rows_kase = rows
                .Where(x => 
                    x.PersonPayRowType == EPersonPayRowType.Pay &&
                    IsKaseRow(x))
                .ToList();
            if (rows_kase.Count == 0) return;

            decimal[] xdiff = new decimal[rows_pay.Count];
            decimal v1 = 0.0M;
            for (int i = 0; i < rows_pay.Count; i++)
            {
                var row = rows_pay[i];
                if (IsKaseRow(row))
                    v1 += row.PayT;
                xdiff[i] = row.Diff + v1;
            }

            var last_calc_row = rows
                .Where(x => x.PersonPayRowType == EPersonPayRowType.Calc)
                .LastOrDefault();

            var last_pay_row = rows_pay.Last();
            decimal add_to_last = 0.0M;
            if (last_calc_row != null && last_pay_row.Date <= last_calc_row.Date)
            {
                add_to_last = Math.Round(last_calc_row.CalcT * (decimal)last_pay_percent / 100.0M, 0);
                xdiff[xdiff.Length - 1] += add_to_last;
            }

            decimal[] calc_min = new decimal[xdiff.Length];
            decimal min1 = xdiff[xdiff.Length - 1];
            calc_min[calc_min.Length - 1] = min1;
            for (int i = xdiff.Length - 2; i >= 0; i--)
            {
                decimal v = Math.Max(xdiff[i], 0.0M);
                if(v < min1) min1 = v;
                calc_min[i] = min1;
            }

            decimal to_pay_left = xdiff[xdiff.Length - 1];
            to_pay_left = Math.Max(to_pay_left, 0.0M);
            decimal paid = 0.0M;
            for (int i = 0; i < xdiff.Length; i++)
            {
                var pay_row = rows_pay[i];
                if (IsKaseRow(pay_row))
                {
                    decimal calcv = xdiff[i] - paid;
                    calcv = Math.Min(calcv, calc_min[i] - paid);
                    calcv = Math.Max(calcv, 0.0M);
                    calcv = Math.Min(calcv, to_pay_left);
                    to_pay_left -= calcv;
                    paid += calcv;
                    pay_row.CalcVal = calcv;
                }
                pay_row.Diff = xdiff[i] - paid;
                if (i == xdiff.Length - 1)
                    pay_row.Diff -= add_to_last;
            }
        }

        public string SaveCalcVal(List<PersonPayRow> rows)
        {
            var rows_kase = rows
                .Where(x =>
                    x.PersonPayRowType == EPersonPayRowType.Pay &&
                    IsKaseRow(x))
                .ToList();
            if (rows_kase.Count == 0) return "Nav datu";

            var table_payrows = MyData.DbContextA.BL_A_PAYLISTS_R;

            var dbrows = rows_kase
                .Select(x => x.PaylistRRow)
                .ToList();
            if (dbrows.Contains(null))
                return "Neizdevās saglabāt datus";
            
            for (int i = 0; i < rows_kase.Count; i++)
            {
                var row = rows_kase[i];
                var dbrow = dbrows[i];
                dbrow.TPAY = row.CalcVal;
            }

            return "OK";
        }

        private void CmFilter_Click(object sender, EventArgs e)
        {
            DoFilter();
        }

        private void miKasesMaksājumuSarēķins_Click(object sender, EventArgs e)
        {
            if (PersonPayRows == null || PersonPayRows.Count == 0) return;
            DoFilter();
            if (PersonPayRows == null || PersonPayRows.Count == 0) return;
            CheckKase(PersonPayRows, AddPercent);
            sgvRows.Refresh();
        }

        private void miAvansaMaksājumaProcentsNoPēdējāsAlgas_Click(object sender, EventArgs e)
        {
            var f = new Form_InputBox("", "Norādi procentus avansa maksājumam no pēdējās algas", AddPercent.ToString());
            var rt = f.ShowDialog();
            if (rt != DialogResult.OK) return;
            string s = f.SelectedValueStr;
            if (!int.TryParse(s, out int p)) return;
            if (p < 0 || p > 100) return;
            AddPercent = p;
        }

        private void miSaglabātSarēķinātosKasesMaksājumus_Click(object sender, EventArgs e)
        {
            if (PersonPayRows == null || PersonPayRows.Count == 0) return;
            var rt = SaveCalcVal(PersonPayRows);
            if(rt != "OK")
            {
                MyMainForm.ShowWarning(rt);
                return;
            }
            MyMainForm.ShowInfo("Dati saglabāti.\nNeaizmirsti veikt maksājumu sarakstu pārrēķinu");
        }

        private void miMaksājumuPārskats_DropDownOpening(object sender, EventArgs e)
        {
            miAvansaMaksājumaProcentsNoPēdējāsAlgas.Text = $"Avansa maksājuma procents no pēdējās algas: {AddPercent}%";
        }
    }

    public enum EPersonPayRowType
    {
        Calc = 0,
        Pay = 1
    }

    public enum EPersonPayRowType2
    {
        Kase = 0,
        Banka = 1
    }

    public class PersonPayRow
    {
        public A_PAYLISTS_R PaylistRRow { get; set; }
        public A_SALARY_SHEETS_R SalarySheetRRow { get; set; }
        public EPersonPayRowType PersonPayRowType { get; set; }
        public DateTime Date { get; set; }
        public string ListDescription { get; set; } = null;
        public string PositionTitle { get; set; } = null;
        public decimal Calc { get; set; } = 0.0M;
        public decimal CalcAdvance { get; set; } = 0.0M;
        public decimal CalcWithhold { get; set; } = 0.0M;
        public decimal CalcT { get; set; } = 0.0M;
        public decimal NotPaid { get; set; } = 0.0M;
        public decimal NotPaidAdvance { get; set; } = 0.0M;
        public decimal NotPaidWithhold { get; set; } = 0.0M;
        public decimal NotPaidT { get; set; } = 0.0M;
        public decimal PayT { get; set; } = 0.0M;
        public decimal Diff { get; set; } = 0.0M;
        public decimal CalcIIN { get; set; } = 0.0M;
        public decimal TakeIIN { get; set; } = 0.0M;
        public decimal CalcVal { get; set; } = 0.0M;
    }
}
