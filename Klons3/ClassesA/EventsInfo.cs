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
    public class EventsInfo0
    {
        public static KlonsData MyData => KlonsData.St;

        public PeriodInfo HireFire = null;
        public Dictionary<A_POSITIONS, PeriodInfo> Positions = null;
        public PeriodInfo Vacations = null;
        public PeriodInfo SickDays = null;

        public bool IsOK { get; protected set; } = false;

        public virtual string AnalizeDLEvents(A_PERSONS dr_person)
        {
            IsOK = false;

            var table_persons = KlonsData.St.DbContextA.BL_A_PERSONS;
            if (dr_person == null)
                throw new ArgumentException("No person.");

            HireFire = new PeriodInfo();
            Positions = new Dictionary<A_POSITIONS, PeriodInfo>();
            Vacations = new PeriodInfo();
            SickDays = new PeriodInfo();

            var drs_events = dr_person.Events;

            //
            // atlaists / pieņemts
            //
            var drsn_hirefire = drs_events.Where(d =>
                    Utils.IN(d.IDN, (int)EEventId.Pieņemts, (int)EEventId.Atlaists)
            ).OrderBy(d => d.DATE1).ToArray();

            if (drsn_hirefire.Length == 0)
            {
                return "OK";
            }

            var pi_hirefire = new PeriodInfo();

            var rt1 = pi_hirefire.ReadStartEndList(drsn_hirefire,
                isStartItem: it => it.IDN == (int)EEventId.Pieņemts,
                getItemDate: it => it.DATE1);

            switch (rt1)
            {
                case PeriodInfo.ERetReadStartEndList.BadDates:
                    return "Pieņemts/atlaists notikumiem ir nekorekti datumi.";
                case PeriodInfo.ERetReadStartEndList.BadStart:
                case PeriodInfo.ERetReadStartEndList.BadEnd:
                    return "Pieņemts/atlaists notikumiem ir nekorekta secība.";
            }

            //
            // amati
            //
            var drsn_position = drs_events.Where(d =>
            {
                return
                    d.Person == dr_person &&
                    Utils.IN(d.IDN, (int)EEventId.Piešķirts_amats, (int)EEventId.Atbrīvots_no_amata);
            }).OrderBy(d => d.DATE1).ToArray();

            var dic_pos = Utils.BreakListInGroups(drsn_position, d => d.Position);
            var dic_pos_pi = new Dictionary<A_POSITIONS, PeriodInfo>();
            foreach (var kv in dic_pos)
            {
                var pi = new PeriodInfo();
                dic_pos_pi[kv.Key] = pi;
                rt1 = pi.ReadStartEndList(kv.Value.ToArray(),
                    isStartItem: it => it.IDN == (int)EEventId.Piešķirts_amats,
                    getItemDate: it => it.DATE1);

                switch (rt1)
                {
                    case PeriodInfo.ERetReadStartEndList.BadDates:
                        return "Piešķirts/atņemts amats notikumiem ir nekorekti datumi.";
                    case PeriodInfo.ERetReadStartEndList.BadStart:
                    case PeriodInfo.ERetReadStartEndList.BadEnd:
                        return "Piešķirts/atņemts amats notikumiem ir nekorekta secība.";
                }

                if (!pi_hirefire.ListContainsList(pi))
                    return "Piešķirts/atņemts amats notikums atlaistam darbiniekam.";

            }

            //
            //  atvaļinājumi  
            //
            var drsn_vac = drs_events.Where(d =>
            {
                return d.Person == dr_person &&
                    d.IDN >= 100 &&
                    d.IDN < 200;
            }).OrderBy(d => d.DATE1).ToArray();

            var pi_vac = new PeriodInfo();

            var rt2 = pi_vac.ReadPeriodList(drsn_vac,
                getItemDate1: it => it.DATE1,
                getItemDate2: it => it.DATE2.Value);

            switch (rt2)
            {
                case PeriodInfo.ERetReadPeriodList.BadDates:
                    return "Atvaļinājuma notikumam beigu datums \nir mazāks par sākuma datumu.";
                case PeriodInfo.ERetReadPeriodList.PeriodsOverlap:
                    return "Atvaļinājuma notikumai pārklājas";
            }

            foreach (var pi in pi_vac.LinkedPeriods)
            {
                pi.PeriodId = EPeriodId.Atvaļinājums;
                var dr_notikumi_r = pi.Item1 as A_EVENTS;
                pi.EEventId = (EEventId)dr_notikumi_r.IDN;
            }

            //
            //  slimo
            //
            var pi_sick = new PeriodInfo();

            var drsn_sick = drs_events.Where(d =>
            {
                return d.Person == dr_person &&
                    d.IDN >= 200 &&
                    d.IDN < 300;
            }).OrderBy(d => d.DATE1).ToArray();

            rt2 = pi_sick.ReadPeriodList(drsn_sick,
                getItemDate1: it => it.DATE1,
                getItemDate2: it => it.DATE2.Value);

            switch (rt2)
            {
                case PeriodInfo.ERetReadPeriodList.BadDates:
                    return "Slimības notikumam beigu datums \nir mazāks par sākuma datumu.";
                case PeriodInfo.ERetReadPeriodList.PeriodsOverlap:
                    return "Slimības notikumai pārklājas";
            }

            foreach (var pi in pi_sick.LinkedPeriods)
            {
                pi.PeriodId = EPeriodId.Slimo;
                var dr_notikumi_r = pi.Item1 as A_EVENTS;
                pi.EEventId = (EEventId)dr_notikumi_r.IDN;
            }
            
            //
            //  check
            //

            if (!pi_hirefire.ListContainsList(pi_vac))
                return "Atvaļinājuma notikums atlaistam darbiniekam.";

            if (!pi_hirefire.ListContainsList(pi_sick))
                return "Slimības notikums atlaistam darbiniekam.";

            //if (pi_vac.ListContainsList(pi_sick))
            //    return "Slimības notikums pārklājas ar atvaļinājumu.";

            if (pi_hirefire.LinkedPeriods.Count > 0 &&
                dic_pos_pi.Count == 0)
                return "Darbiniekam nav piešķirts amats";

            if (pi_hirefire.LinkedPeriods.Count > 0)
            {
                int ct = pi_hirefire.LinkedPeriods.Count;
                var dt2 = pi_hirefire.LinkedPeriods[ct - 1].DateLast;
                bool posok = false;
                if (dt2 == DateTime.MaxValue)
                {
                    foreach (var pi in dic_pos_pi.Values)
                    {
                        int ct1 = pi.LinkedPeriods.Count;
                        if (ct1 == 0) continue;
                        if(pi.LinkedPeriods[ct1-1].DateLast == DateTime.MaxValue)
                        {
                            posok = true;
                            break;
                        }
                    }
                    if (!posok)
                        return "Darbiniekam nav amata.";
                }
                else
                {
                    foreach (var pi in dic_pos_pi.Values)
                    {
                        int ct1 = pi.LinkedPeriods.Count;
                        if (ct1 == 0) continue;
                        var dt3 = pi.LinkedPeriods[ct1 - 1].DateLast;
                        if (dt3 == DateTime.MaxValue)
                            return "Darbinieks nav atbrīvots no amata.";
                        if (dt3 == dt2)
                        {
                            posok = true;
                            break;
                        }
                    }
                    if (!posok)
                        return "Darbiniekam nav amata.";
                }
            }

            HireFire = pi_hirefire;
            Positions = dic_pos_pi;
            Vacations = pi_vac;
            SickDays = pi_sick;

            IsOK = true;
            return "OK";

        }
    }


    public class EventsInfo : EventsInfo0
    {
        public EPeriodId[] DayIds = new EPeriodId[31];
        public EEventId[] DayIdsA = new EEventId[31];

        public Dictionary<A_POSITIONS, EPeriodId[]> PositionDayIds = new Dictionary<A_POSITIONS, EPeriodId[]>();
        public Dictionary<A_POSITIONS, EEventId[]> PositionDayIdsA = new Dictionary<A_POSITIONS, EEventId[]>();


        public string ProcessData(A_PERSONS dr_person, DateTime dt1, DateTime dt2)
        {
            IsOK = false;
            var ret = AnalizeDLEvents(dr_person);
            if (ret != "OK") return ret;
            ret = MarkDays(dt1, dt2);
            IsOK = ret == "OK";
            return ret;
        }

        private void ClearA(EPeriodId[] ids)
        {
            for (int i = 0; i < DayIds.Length; i++)
            {
                ids[i] = EPeriodId.Nav_pieņets;
            }
        }

        private void ClearB(EEventId[] ids)
        {
            for (int i = 0; i < DayIds.Length; i++)
            {
                ids[i] = EEventId.None;
            }
        }

        public void ClearDays()
        {
            ClearA(DayIds);
            ClearB(DayIdsA);
            PositionDayIds = new Dictionary<A_POSITIONS, EPeriodId[]>();
            PositionDayIdsA = new Dictionary<A_POSITIONS, EEventId[]>();
        }

        public string MarkDays(DateTime dt1, DateTime dt2)
        {
            ClearDays();

            if (HireFire == null)
                return "Darbieneiks nav pieņemts darbā.";

            foreach (var pv in Positions)
            {
                var dids = new EPeriodId[31];
                var didsa = new EEventId[31];
                ClearA(dids);
                ClearB(didsa);
                PositionDayIds[pv.Key] = dids;
                PositionDayIdsA[pv.Key] = didsa;
            }

            foreach (var pv in Positions)
            {
                pv.Value.MarkDates(DayIds, dt1, dt2, EPeriodId.Ir_pieņemts);
                var posdayids = PositionDayIds[pv.Key];
                pv.Value.MarkDates(posdayids, dt1, dt2, EPeriodId.Ir_pieņemts);
            }

            Vacations.MarkDates(DayIds, dt1, dt2, EPeriodId.Atvaļinājums);
            SickDays.MarkDates(DayIds, dt1, dt2, EPeriodId.Slimo);

            foreach (var pv in Positions)
            {
                var posdayids = PositionDayIds[pv.Key];
                Vacations.MarkDates(posdayids, dt1, dt2, EPeriodId.Atvaļinājums);
                SickDays.MarkDates(posdayids, dt1, dt2, EPeriodId.Slimo);
            }

            foreach (var pi in Vacations.LinkedPeriods)
                pi.EEventId = (pi.Item1 as A_EVENTS).EventCode;
            foreach (var pi in SickDays.LinkedPeriods)
                pi.EEventId = (pi.Item1 as A_EVENTS).EventCode;

            foreach (var pv in Positions)
            {
                pv.Value.MarkDates(DayIdsA, dt1, dt2, EEventId.Pieņemts);
                var posdayidsa = PositionDayIdsA[pv.Key];
                pv.Value.MarkDates(posdayidsa, dt1, dt2, EEventId.Pieņemts);
            }

            Vacations.MarkDatesA(DayIdsA, dt1, dt2, d => d.EEventId);
            SickDays.MarkDatesA(DayIdsA, dt1, dt2, d => d.EEventId);

            foreach (var pv in Positions)
            {
                var posdayidsa = PositionDayIdsA[pv.Key];
                Vacations.MarkDatesA(posdayidsa, dt1, dt2, d => d.EEventId);
                SickDays.MarkDatesA(posdayidsa, dt1, dt2, d => d.EEventId);
            }

            return "OK";
        }

        public override string AnalizeDLEvents(A_PERSONS dr_person)
        {
            ClearDays();
            return base.AnalizeDLEvents(dr_person);
        }
    }

}
