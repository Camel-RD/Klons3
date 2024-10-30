using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using KlonsF.Classes;
using Klons3.ModelsA;
using NPOI.SS.Formula.Functions;
using System.Collections;
using System.DirectoryServices;
using System.Security.Policy;

namespace KlonsA.Classes;

[ToolboxItem(false)]
public class DLJoinView : BindingList<TimesheetJoinedRow>, IComparer<TimesheetJoinedRow>, IDisposable
{
    public KlonsData MyData { get { return KlonsData.St; } }

    public static DLJoinView Empty = [];

    public DLJoinView()
    {
        HookLists();
    }

    public event MyItemPropertyChangedventHandler ListItemPropertyChanged;

    public void HookLists()
    {
        MyData.DbContextA.A_TIMESHEET.Local.CollectionChanged += Local_CollectionChanged;
        MyData.DbContextA.BL_A_TIMESHEET.ListChanged += BL_A_TIMESHEET_ListChanged;
        MyData.DbContextA.BL_A_TIMESHEET_LISTS_R.ListChanged += BL_A_TIMESHEET_LISTS_R_ListChanged; ;
    }

    public void UnHookLists()
    {
        MyData.DbContextA.A_TIMESHEET.Local.CollectionChanged += Local_CollectionChanged;
        MyData.DbContextA.BL_A_TIMESHEET.ListChanged += BL_A_TIMESHEET_ListChanged;
        MyData.DbContextA.BL_A_TIMESHEET_LISTS_R.ListChanged += BL_A_TIMESHEET_LISTS_R_ListChanged; ;
    }

    private void BL_A_TIMESHEET_ListChanged(object sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType == ListChangedType.ItemChanged)
        {
            var drt = MyData.DbContextA.BL_A_TIMESHEET[e.NewIndex];
            var ks = FindByDRT(drt);
            foreach (var k in ks)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, k));
        }
    }

    private void BL_A_TIMESHEET_LISTS_R_ListChanged(object sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType == ListChangedType.ItemChanged)
        {
            var drl = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R[e.NewIndex];
            var ks = FindByDRL(drl);
            foreach (var k in ks)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, k));
        }
    }

    private void Local_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        {
            if (e.NewItems == null) return;
            foreach (var item in e.OldItems)
            {
                if (item is A_TIMESHEET drt)
                    RemoveByDRT(drt);
                else if (item is A_TIMESHEET_LISTS_R drl)
                    RemoveByDRL(drl);
            }
        }
    }

    private void Row_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var args = new MyItemPropertyChangedEventArgs(sender, e.PropertyName);
        ListItemPropertyChanged?.Invoke(this, args);
    }

    public void RemoveByDRT(A_TIMESHEET drt)
    {
        var drs = this.Where(x => x.DRT == drt).ToList();
        foreach (var dr in drs)
            RemoveFromView(dr);
    }

    public void RemoveByDRL(A_TIMESHEET_LISTS_R drl)
    {
        var drs = this.Where(x => x.DRL == drl).ToList();
        if (drs.Count == 0) return;
        var dr1 = drs[0];
        dr1.RowSet.Clear();
        foreach (var dr in drs)
            RemoveFromView(dr);
    }

    public List<int> FindByDRT(A_TIMESHEET drt)
    {
        var ret = new List<int>();
        for (int i = 0; i < Count; i++)
            if (this[i].DRT == drt) ret.Add(i);
        return ret;
    }

    public List<int> FindByDRL(A_TIMESHEET_LISTS_R drl)
    {
        var ret = new List<int>();
        for (int i = 0; i < Count; i++)
            if (this[i].DRL == drl) ret.Add(i);
        return ret;
    }

    public void ClearList()
    {
        var jrs = this.ToList();
        Clear();
        for (int i = 0; i < jrs.Count; i++)
        {
            var jr = jrs[i];
            jr.PropertyChanged -= Row_PropertyChanged;
            jr.RowSet.Clear();
            jr.Clear();
        }
    }

    public void AddFrom(List<TimesheetJoinedRow> rows)
    {
        foreach (var row in rows)
        {
            row.PropertyChanged += Row_PropertyChanged;
            Add(row);
        }
    }

    public void RemoveFromView(TimesheetJoinedRow row)
    {
        row.PropertyChanged -= Row_PropertyChanged;
        row.Clear();
        this.Remove(row);
    }

    public void MakeList(A_TIMESHEET_LISTS dr_sar)
    {
        ClearList();

        if (dr_sar == null) return;

        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;

        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;

        var table_persons = MyData.DbContextA.BL_A_PERSONS;
        var table_amati = MyData.DbContextA.BL_A_POSITIONS;

        var dv_darba_laiks = table_darba_laiks.ToDictionary(x => x.ID);

        var drs_sar_r = dr_sar.TimesheetListRows
            .OrderBy(x => x.SNR)
            .ThenBy(x => x.ID);

        var jr_list = new List<TimesheetJoinedRow>();

        foreach (var dr_sar_r in drs_sar_r)
        {
            var drs_darba_laiks = dr_sar_r.TimeSheetRows
                .OrderBy(x => x.KIND1)
                .ToArray();

            var dr_pers = dr_sar_r.Person;
            var dr_amats = dr_sar_r.Position;
            var s_name = $"{dr_pers.FNAME} {dr_pers.LNAME}";
            var s_position = dr_amats.TITLE;
            var s_title = $"{dr_pers.FNAME} {dr_pers.LNAME}, {s_position}";

            var jrset = new TimesheetJoinedRowSet();

            var act_add_row = new Action<A_TIMESHEET>(dr => {
                var drv_darba_laiks = dv_darba_laiks[dr.ID];
                var jr_darba_laiks = new TimesheetJoinedRow(dr_sar_r, drv_darba_laiks) {
                    Name = s_name,
                    Position = s_position,
                    Title = s_title
                };
                jr_darba_laiks.RowSet = jrset;
                jrset.SetRow(jr_darba_laiks);
                jr_list.Add(jr_darba_laiks);
            });


            if (dr_sar_r.XPlanType == EPlanType.Fixed)
            {
                var drs_darba_laiks_plans = table_darba_laiks.Where(dr =>
                    dr.TimePlan != null &&
                    dr.TimeSheetListRow == null &&
                    dr.TimePlan == dr_sar_r.TimePlan &&
                    dr.YR == dr_sar.YR &&
                    dr.MT == dr_sar.MT)
                    .OrderBy(x => x.KIND1)
                    .ToArray();

                foreach (var dr_darba_laiks in drs_darba_laiks_plans)
                {
                    act_add_row(dr_darba_laiks);
                }
            }

            foreach (var dr_darba_laiks in drs_darba_laiks)
            {
                act_add_row(dr_darba_laiks);
            }
        }

        AddFrom(jr_list);
    }

    public string AddNew(A_TIMESHEET_LISTS dr_sar, out A_TIMESHEET_LISTS_R dr_r_new,
        int snr, A_PERSONS dr_pers, A_POSITIONS dr_amats, A_TIMEPLAN_LIST dr_pl, 
        bool plind, bool night, bool overtime)
    {
        string er = "OK";
        dr_r_new = null;

        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;

        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;

        var table_persons = MyData.DbContextA.BL_A_PERSONS;
        var table_amati = MyData.DbContextA.BL_A_POSITIONS;

        var dv_darba_laiks = table_darba_laiks.ToDictionary(x => x.ID);

        int yr = dr_sar.YR;
        int mt = dr_sar.MT;


        var has_it = table_sar_r.Where(d =>
            d.Timesheet.YR == yr &&
            d.Timesheet.MT == mt &&
            d.Person == dr_pers &&
            d.Position == dr_amats
        ).Count() > 0;
        if (has_it)
        {
            er =
                "Nevar izveidot ierakstu darbiniekam un amatam,\n" +
                "jo tāds ieraksts šajā mēnesî jau ir izveidots.";
            return er;
        }

        var s_name = $"{dr_pers.FNAME} {dr_pers.LNAME}";
        var s_position = dr_amats.TITLE;
        var s_title = $"{dr_pers.FNAME} {dr_pers.LNAME}, {s_position}";

        var new_dr_sar_r = table_sar_r.NewItem();

        new_dr_sar_r.Timesheet = dr_sar;
        new_dr_sar_r.Person = dr_pers;
        new_dr_sar_r.Position = dr_amats;
        new_dr_sar_r.TimePlan = dr_pl;
        new_dr_sar_r.SNR = (short)snr;
        new_dr_sar_r.XPlanType = plind ? EPlanType.Individual : EPlanType.Fixed;
        new_dr_sar_r.NIGHT = dr_pl.NIGHT.Value;
        new_dr_sar_r.OVERTIME = (short)(overtime ? 1 : 0);

        table_sar_r.Add(new_dr_sar_r);

        var jrset = new TimesheetJoinedRowSet();
        var jr_add_list = new List<TimesheetJoinedRow>();

        var act_add_row = new Action<A_TIMESHEET>(dr => {
            var drv_darba_laiks = dv_darba_laiks[0];
            var jr_darba_laiks = new TimesheetJoinedRow(new_dr_sar_r, drv_darba_laiks)
            {
                Name = s_name,
                Position = s_position,
                Title = s_title
            };
            jrset.SetRow(jr_darba_laiks);
            jr_darba_laiks.RowSet = jrset;
            jr_add_list.Add(jr_darba_laiks);
        });

        var act_copy_darba_laiks_row_data = new Action<A_TIMESHEET,
            A_TIMESHEET>((dfrom, dto) =>
            {
                for (int i = 0; i < 31; i++)
                {
                    dto.Vx[i] = dfrom.Vx[i];
                    dto.Dx[i] = dfrom.Dx[i];
                }
            });


        var drs_darba_laiks_plans = table_darba_laiks.Where(dr =>
            dr.TimePlan != null &&
            dr.TimeSheetListRow == null &&
            dr.TimePlan == dr_pl &&
            dr.YR == yr &&
            dr.MT == mt).ToArray();

        //add plan
        if (plind)
        {
            foreach (var dr_darba_laiks in drs_darba_laiks_plans)
            {
                var new_dr_dl_pl = table_darba_laiks.NewItem();

                new_dr_dl_pl.TimeSheetListRow = new_dr_sar_r;
                new_dr_dl_pl.YR = yr;
                new_dr_dl_pl.MT = mt;
                new_dr_dl_pl.SNR = (short)snr;

                table_darba_laiks.Add(new_dr_dl_pl);

                switch (dr_darba_laiks.XKind1)
                {
                    case EKind1.PlanGroupDay:
                        new_dr_dl_pl.XKind1 = EKind1.PlanIndividualDay;
                        break;
                    case EKind1.PlanGroupaNight:
                        new_dr_dl_pl.XKind1 = EKind1.PlanIndividualNight;
                        break;
                }

                act_copy_darba_laiks_row_data(dr_darba_laiks, new_dr_dl_pl);
                act_add_row(new_dr_dl_pl);
            }
        }
        else
        {
            foreach (var dr_darba_laiks in drs_darba_laiks_plans)
            {
                act_add_row(dr_darba_laiks);
            }
        }

        //add day
        var new_dr_darba_laiks = table_darba_laiks.NewItem();
        new_dr_darba_laiks.TimeSheetListRow = new_dr_sar_r;
        new_dr_darba_laiks.SNR = (short)snr;
        new_dr_darba_laiks.XKind1 = EKind1.Fact;
        new_dr_darba_laiks.YR = yr;
        new_dr_darba_laiks.MT = mt;
        new_dr_darba_laiks.Person = dr_pers;
        new_dr_darba_laiks.Position = dr_amats;

        table_darba_laiks.Add(new_dr_darba_laiks);

        act_add_row(new_dr_darba_laiks);

        //add night
        if (night)
        {
            new_dr_darba_laiks = table_darba_laiks.NewItem();
            new_dr_darba_laiks.SNR = (short)snr;
            new_dr_darba_laiks.TimeSheetListRow = new_dr_sar_r;
            new_dr_darba_laiks.XKind1 = EKind1.FactNight;
            new_dr_darba_laiks.YR = yr;
            new_dr_darba_laiks.MT = mt;
            new_dr_darba_laiks.Person = dr_pers;
            new_dr_darba_laiks.Position = dr_amats;

            table_darba_laiks.Add(new_dr_darba_laiks);

            act_add_row(new_dr_darba_laiks);
        }

        //add overtime
        if (overtime)
        {
            new_dr_darba_laiks = table_darba_laiks.NewItem();
            new_dr_darba_laiks.SNR = (short)snr;
            new_dr_darba_laiks.TimeSheetListRow = new_dr_sar_r;
            new_dr_darba_laiks.XKind1 = EKind1.FactOvertime;
            new_dr_darba_laiks.YR = yr;
            new_dr_darba_laiks.MT = mt;
            new_dr_darba_laiks.Person = dr_pers;
            new_dr_darba_laiks.Position = dr_amats;

            table_darba_laiks.Add(new_dr_darba_laiks);

            act_add_row(new_dr_darba_laiks);
        }

        AddFrom(jr_add_list);
        DoSort();
        dr_r_new = new_dr_sar_r;
        return er;
    }

    public string EditCurrent(TimesheetJoinedRow drj, int snr,
        A_PERSONS dr_pers, A_POSITIONS dr_amats, A_TIMEPLAN_LIST dr_pl,
        bool plind, bool night, bool overtime)
    {
        string er = "OK";
        var _view = this;
        var RowSet = drj.RowSet;

        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;
        var dr_sar_r = drj.DRL;

        bool cur_plind = dr_sar_r.XPlanType == EPlanType.Individual;

        if (dr_pers == dr_sar_r.Person &&
            dr_amats == dr_sar_r.Position &&
            snr == dr_sar_r.SNR &&
            dr_pl == dr_sar_r.TimePlan &&
            plind == cur_plind &&
            night == (dr_sar_r.NIGHT == 1) &&
            overtime == (dr_sar_r.OVERTIME == 1))
            return er;


        var table_sar = MyData.DbContextA.BL_A_TIMESHEET_LISTS;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;
        var table_plans = MyData.DbContextA.BL_A_TIMEPLAN_LIST;
        var table_persons = MyData.DbContextA.BL_A_PERSONS;
        var table_amati = MyData.DbContextA.BL_A_POSITIONS;

        var dr_sar = dr_sar_r.Timesheet;

        var dv_darba_laiks = table_darba_laiks.ToDictionary(x => x.ID);

        int yr = dr_sar_r.Timesheet.YR;
        int mt = dr_sar_r.Timesheet.MT;


        if (dr_pers != dr_sar_r.Person ||
            dr_amats != dr_sar_r.Position)
        {
            var has_it = table_sar_r.Where(d =>
                d.Timesheet.YR == yr &&
                d.Timesheet.MT == mt &&
                d.Person == dr_pers &&
                d.Position == dr_amats
            ).Count() > 0;
            if (has_it)
            {
                er = "Nevar mainît ierakstam darbinieku un amatu,\n" +
                    "jo tāds ieraksts šajā mēnesī jau ir izveidots.";
                return er;
            }
        }

        dr_sar_r.Person = dr_pers;
        dr_sar_r.Position = dr_amats;

        if (snr != dr_sar_r.SNR)
            dr_sar_r.SNR = (short)snr;


        var s_name = $"{dr_pers.FNAME} {dr_pers.LNAME}";
        var s_position = dr_amats.TITLE;
        var s_title = $"{dr_pers.FNAME} {dr_pers.LNAME}, {s_position}";

        var jr_add_list = new List<TimesheetJoinedRow>();

        var act_add_row = new Action<A_TIMESHEET>(dr => {
            var jr_darba_laiks = new TimesheetJoinedRow(dr_sar_r, dr)
            {
                Name = s_name,
                Position = s_position,
                Title = s_title
            };
            jr_darba_laiks.RowSet = RowSet;
            RowSet.SetRow(jr_darba_laiks);
            jr_add_list.Add(jr_darba_laiks);
        });

        var act_copy_darba_laiks_row_data = new Action<A_TIMESHEET,
            A_TIMESHEET>((dfrom, dto) =>
            {
                for (int i = 0; i < 31; i++)
                {
                    dto.Vx[i] = dfrom.Vx[i];
                    dto.Dx[i] = dfrom.Dx[i];
                }
            });

        // remove rows for night
        if (!night && dr_sar_r.NIGHT == 1)
        {
            if (RowSet.PlanNight != null &&
                RowSet.PlanNight.DRT.XKind1 == EKind1.PlanIndividualNight)
            {
                RowSet.PlanNight.DRT.Delete();
                RowSet.PlanNight = null;
            }
            if (RowSet.FactNight != null)
            {
                RowSet.FactNight.DRT.Delete();
                RowSet.FactNight = null;
            }
        }

        // remove overtime
        if (!overtime && dr_sar_r.OVERTIME == 1)
        {
            if (RowSet.FactOvertime != null)
            {
                RowSet.FactOvertime.DRT.Delete();
                RowSet.FactOvertime = null;
            }
        }


        //remove individual plan
        if (cur_plind && !plind)
        {
            if (RowSet.Plan != null &&
                RowSet.Plan.DRT.XKind1 == EKind1.PlanIndividualDay)
            {
                RowSet.Plan.DRT.Delete();
                RowSet.Plan = null;
            }
            if (RowSet.PlanNight != null &&
                RowSet.PlanNight.DRT.XKind1 == EKind1.PlanIndividualNight)
            {
                RowSet.PlanNight.DRT.Delete();
                RowSet.PlanNight = null;
            }
        }

        //remove group plan
        if (!cur_plind && (plind || dr_pl != dr_sar_r.TimePlan))
        {
            if (RowSet.Plan != null &&
                RowSet.Plan.DRT.XKind1 == EKind1.PlanGroupDay)
            {
                RemoveFromView(RowSet.Plan);
                RowSet.Plan = null;
            }
            if (RowSet.PlanNight != null &&
                RowSet.PlanNight.DRT.XKind1 == EKind1.PlanGroupaNight)
            {
                RemoveFromView(RowSet.PlanNight);
                RowSet.PlanNight = null;
            }
        }

        //add group plan
        if (!plind && (cur_plind || dr_pl != dr_sar_r.TimePlan))
        {
            var drs_darba_laiks_plans = table_darba_laiks.Where(dr =>
                dr.TimePlan != null &&
                dr.TimeSheetListRow == null &&
                dr.TimePlan == dr_pl &&
                dr.YR == yr &&
                dr.MT == mt
                ).ToArray();

            foreach (var dr_darba_laiks in drs_darba_laiks_plans)
            {
                act_add_row(dr_darba_laiks);
            }
        }

        //add night
        if (night && dr_sar_r.NIGHT == 0)
        {
            var new_dr_darba_laiks = table_darba_laiks.NewItem();
            new_dr_darba_laiks.TimeSheetListRow = dr_sar_r;
            new_dr_darba_laiks.SNR = (short)snr;
            new_dr_darba_laiks.XKind1 = EKind1.FactNight;
            new_dr_darba_laiks.YR = yr;
            new_dr_darba_laiks.MT = mt;
            new_dr_darba_laiks.Person = dr_pers;
            new_dr_darba_laiks.Position = dr_amats;

            table_darba_laiks.Add(new_dr_darba_laiks);

            if (plind)
            {
                bool have_it = table_darba_laiks.Where(d =>
                    d.TimeSheetListRow != null &&
                    d.TimeSheetListRow == dr_sar_r &&
                    d.XKind1 == EKind1.PlanIndividualNight
                ).Count() > 0;

                if (!have_it)
                {
                    new_dr_darba_laiks = table_darba_laiks.NewItem();
                    new_dr_darba_laiks.TimeSheetListRow = dr_sar_r;
                    new_dr_darba_laiks.SNR = (short)snr;
                    new_dr_darba_laiks.TimePlan = dr_pl;
                    new_dr_darba_laiks.XKind1 = EKind1.PlanIndividualNight;
                    new_dr_darba_laiks.YR = yr;
                    new_dr_darba_laiks.MT = mt;
                    new_dr_darba_laiks.Person = dr_pers;
                    new_dr_darba_laiks.Position = dr_amats;

                    table_darba_laiks.Add(new_dr_darba_laiks);
                }
            }

            act_add_row(new_dr_darba_laiks);
        }

        // add overtime
        if (overtime && dr_sar_r.OVERTIME == 0)
        {
            var new_dr_darba_laiks = table_darba_laiks.NewItem();
            new_dr_darba_laiks.TimeSheetListRow = dr_sar_r;
            new_dr_darba_laiks.SNR = (short)snr;
            new_dr_darba_laiks.XKind1 = EKind1.FactOvertime;
            new_dr_darba_laiks.YR = yr;
            new_dr_darba_laiks.MT = mt;
            new_dr_darba_laiks.Person = dr_pers;
            new_dr_darba_laiks.Position = dr_amats;

            table_darba_laiks.Add(new_dr_darba_laiks);

            act_add_row(new_dr_darba_laiks);
        }

        //add individual plan
        if (plind && !cur_plind)
        {
            var drs_p = table_darba_laiks.Where(d =>
                d.TimeSheetListRow == null &&
                d.TimePlan != null &&
                d.TimePlan == dr_pl &&
                d.YR == yr &&
                d.MT == mt
            ).ToArray();

            foreach (var dr_p in drs_p)
            {
                var new_dr_darba_laiks_plans_r = table_darba_laiks.NewItem();
                new_dr_darba_laiks_plans_r.TimeSheetListRow = dr_sar_r;
                new_dr_darba_laiks_plans_r.YR = yr;
                new_dr_darba_laiks_plans_r.MT = mt;
                new_dr_darba_laiks_plans_r.TimePlan = dr_sar_r.TimePlan;
                new_dr_darba_laiks_plans_r.SNR = (short)snr;

                act_copy_darba_laiks_row_data(dr_p, new_dr_darba_laiks_plans_r);

                table_darba_laiks.Add(new_dr_darba_laiks_plans_r);

                switch (dr_p.XKind1)
                {
                    case EKind1.PlanGroupDay:
                        new_dr_darba_laiks_plans_r.XKind1 = EKind1.PlanIndividualDay;
                        break;
                    case EKind1.PlanGroupaNight:
                        new_dr_darba_laiks_plans_r.XKind1 = EKind1.PlanIndividualNight;
                        break;
                }

                act_add_row(new_dr_darba_laiks_plans_r);
            }
        }

        _view.AddFrom(jr_add_list);

        //update ind plan
        if (dr_pl != dr_sar_r.TimePlan && plind)
        {
            var drs_pl_cur = table_darba_laiks.Where(d =>
                d.TimePlan != null &&
                d.TimeSheetListRow != null &&
                d.TimeSheetListRow == dr_sar_r
            ).ToArray();

            var drs_pl_new = table_darba_laiks.Where(d =>
                d.TimePlan != null &&
                d.TimeSheetListRow == null &&
                d.TimePlan == dr_pl &&
                d.YR == yr &&
                d.MT == mt
            ).ToArray();

            foreach (var dr_pl_cur in drs_pl_cur)
            {
                var dr_pl_new = drs_pl_new.Where(d =>
                    d.KIND1 == dr_pl_cur.KIND1
                ).FirstOrDefault();

                if (dr_pl_new == null) continue;
                act_copy_darba_laiks_row_data(dr_pl_cur, dr_pl_new);
            }
        }

        if (night != (dr_sar_r.NIGHT == 1))
            dr_sar_r.NIGHT = (short)(night ? 1 : 0);
        if (overtime != (dr_sar_r.OVERTIME == 1))
            dr_sar_r.OVERTIME = (short)(overtime ? 1 : 0);

        if (plind != cur_plind)
            if (plind)
                dr_sar_r.XPlanType = EPlanType.Individual;
            else
                dr_sar_r.XPlanType = EPlanType.Fixed;

        dr_sar_r.TimePlan = dr_pl;

        _view.DoSort();
        return er;
    }

    public void RemoveJoinRows(TimesheetJoinedRow[] rows)
    {
        foreach (var row in rows)
            RemoveFromView(row);
    }

    public void DeleteRowSet(TimesheetJoinedRow drj)
    {
        var dr_sar_r = drj.DRL;
        var table_darba_laiks = MyData.DbContextA.BL_A_TIMESHEET;
        var table_sar_r = MyData.DbContextA.BL_A_TIMESHEET_LISTS_R;

        // remove rows for fact, night, overtime, individual plan
        var drs_n = dr_sar_r.TimeSheetRows;
        foreach (var dr_n in drs_n)
            dr_n.Delete();

        //remove group plan
        var jrs_remove = this.Where(d => d.DRL == dr_sar_r).ToArray(); ;

        RemoveJoinRows(jrs_remove);

        // remove sheet row
        if (dr_sar_r != null)
            dr_sar_r.Delete();

        drj.RowSet.Clear();
    }

    public int Compare(TimesheetJoinedRow x, TimesheetJoinedRow y)
    {
        if (x is null)
            return y is null ? 0 : -1;
        if (y is null)
            return 1;
        var rt = x.SNRX.Value.CompareTo(y.SNRX.Value);
        if (rt != 0) return rt;
        rt = x.KIND1.Value.CompareTo(y.KIND1.Value);
        return rt;
    }

    public void DoSort()
    {
        ((List<TimesheetJoinedRow>)Items).Sort(this);
        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnHookLists();
                ClearList();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class TimesheetJoinedRow : INotifyPropertyChanged
{
    public A_TIMESHEET_LISTS_R DRL { get; private set; }
    public A_TIMESHEET DRT { get; private set; }

    public TimesheetJoinedRowSet RowSet;

    public TimesheetJoinedRow(A_TIMESHEET_LISTS_R drl, A_TIMESHEET drt)
    {
        DRL = drl;
        DRT = drt;
        DRL.PropertyChanged += DRL_PropertyChanged;
        DRT.PropertyChanged += Drt_PropertyChanged;
    }

    public void Clear()
    {
        DRL.PropertyChanged -= DRL_PropertyChanged;
        DRT.PropertyChanged -= Drt_PropertyChanged;
        DRL = null;
        DRT = null;
        //RowSet.Clear();
        RowSet = null;
    }

    private void DRL_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        string[] pnames = e.PropertyName switch
        {
            "IDX" => ["ID"],
            "Person" => ["Name", "Position", "Title"],
            "Position" => ["Position", "Title"],
            _ => []
        };
        foreach (var pname in pnames)
            OnPropertyChanged(pname);
    }

    private void Drt_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        string pname = e.PropertyName switch
        {
            "IDX" => "ID",
            "KIND1" => "KIND1",
            "KIND2" => "KIND2",
            "XKIND1" => "XKIND1",
            "K1" => "K1",
            var x when A_TIMESHEET.IsVx(x) => x,
            _ => null
        };
        if (pname != null)
            OnPropertyChanged(pname);
    }

    void OnPropertyChanged(string propertname)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertname));
    }

    public int IDX => DRL.ID;
    public short? SNRX => DRL?.SNR;
    public string Name { get; set; } = null;
    public string Position { get; set; } = null;
    public string Title { get; set; } = null;

    public short? KIND1 { get => DRT?.KIND1; set => DRT.KIND1 = value; }
    public short? KIND2 { get => DRT?.KIND2; set => DRT.KIND2 = value; }

    public EKind1 XKind1
    {
        get { return (EKind1)KIND1; }
        set { KIND1 = (short)value; }
    }

    float GetVx(int k) => DRT?.Vx[k] ?? 0.0f;

    public float V1 { get => GetVx(0); set => DRT.Vx[0] = value; }
    public float V2 { get => GetVx(1); set => DRT.Vx[1] = value; }
    public float V3 { get => GetVx(2); set => DRT.Vx[2] = value; }
    public float V4 { get => GetVx(3); set => DRT.Vx[3] = value; }
    public float V5 { get => GetVx(4); set => DRT.Vx[4] = value; }
    public float V6 { get => GetVx(5); set => DRT.Vx[5] = value; }
    public float V7 { get => GetVx(6); set => DRT.Vx[6] = value; }
    public float V8 { get => GetVx(7); set => DRT.Vx[7] = value; }
    public float V9 { get => GetVx(8); set => DRT.Vx[8] = value; }
    public float V10 { get => GetVx(9); set => DRT.Vx[9] = value; }
    public float V11 { get => GetVx(10); set => DRT.Vx[10] = value; }
    public float V12 { get => GetVx(11); set => DRT.Vx[11] = value; }
    public float V13 { get => GetVx(12); set => DRT.Vx[12] = value; }
    public float V14 { get => GetVx(13); set => DRT.Vx[13] = value; }
    public float V15 { get => GetVx(14); set => DRT.Vx[14] = value; }
    public float V16 { get => GetVx(15); set => DRT.Vx[15] = value; }
    public float V17 { get => GetVx(16); set => DRT.Vx[16] = value; }
    public float V18 { get => GetVx(17); set => DRT.Vx[17] = value; }
    public float V19 { get => GetVx(18); set => DRT.Vx[18] = value; }
    public float V20 { get => GetVx(19); set => DRT.Vx[19] = value; }
    public float V21 { get => GetVx(20); set => DRT.Vx[20] = value; }
    public float V22 { get => GetVx(21); set => DRT.Vx[21] = value; }
    public float V23 { get => GetVx(22); set => DRT.Vx[22] = value; }
    public float V24 { get => GetVx(23); set => DRT.Vx[23] = value; }
    public float V25 { get => GetVx(24); set => DRT.Vx[24] = value; }
    public float V26 { get => GetVx(25); set => DRT.Vx[25] = value; }
    public float V27 { get => GetVx(26); set => DRT.Vx[26] = value; }
    public float V28 { get => GetVx(27); set => DRT.Vx[27] = value; }
    public float V29 { get => GetVx(28); set => DRT.Vx[28] = value; }
    public float V30 { get => GetVx(29); set => DRT.Vx[29] = value; }
    public float V31 { get => GetVx(30); set => DRT.Vx[30] = value; }


    public float? K1 { get => DRT?.K1; set => DRT.K1 = value; }

    public event PropertyChangedEventHandler PropertyChanged;
}

public class TimesheetJoinedRowSet
{
    public TimesheetJoinedRow Plan = null;
    public TimesheetJoinedRow PlanNight = null;
    public TimesheetJoinedRow Fact = null;
    public TimesheetJoinedRow FactNight = null;
    public TimesheetJoinedRow FactOvertime = null;

    public void Clear()
    {
        Plan = null;
        PlanNight = null;
        Fact = null;
        FactNight = null;
        FactOvertime = null;
    }

    public void SetRow(TimesheetJoinedRow row)
    {
        switch (row.XKind1)
        {
            case EKind1.PlanGroupDay:
            case EKind1.PlanIndividualDay:
                Plan = row;
                break;
            case EKind1.PlanGroupaNight:
            case EKind1.PlanIndividualNight:
                PlanNight = row;
                break;
            case EKind1.Fact:
                Fact = row;
                break;
            case EKind1.FactNight:
                FactNight = row;
                break;
            case EKind1.FactOvertime:
                FactOvertime = row;
                break;
        }
    }

} 




