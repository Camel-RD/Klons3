using KlonsA.Classes;
using System;
using System.Collections.Generic;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace Klons3.ModelsA;

public interface IDLRowVXIndexer 
{
    float this[int k] { get; set; }
}

public interface IDLRowDXIndexer
{
    short this[int k] { get; set; }
}

public interface IDLRowDXPlanIndexer
{
    EDayPlanId this[int k] { get; set; }
}

public interface IDLRowDXFactIndexer
{
    EDayFactId this[int k] { get; set; }
}

public partial class A_TIMESHEET : EntityBase, IDLRowDXPlanIndexer, IDLRowDXFactIndexer, IDLRowVXIndexer, IDLRowDXIndexer
{
    private readonly float[] _Vx = new float[31];
    private readonly short[] _Dx =  new short[31];

    public IDLRowVXIndexer Vx => this;
    public IDLRowDXIndexer Dx => this;
    public IDLRowDXPlanIndexer DxPlan => this;
    public IDLRowDXFactIndexer DxFact => this;

    [PropertyChanged.DoNotNotify]
    [PropertyChanging.DoNotNotify]
    float IDLRowVXIndexer.this[int k]
    {
        get { return _Vx[k]; }
        set
        {
            if (_Vx[k] == value) return;
            OnPropertyChanging($"V{k + 1}");
            _Vx[k] = value;
            OnPropertyChanged($"V{k + 1}");
        }
    }

    [PropertyChanged.DoNotNotify]
    [PropertyChanging.DoNotNotify]
    short IDLRowDXIndexer.this[int k]
    {
        get { return _Dx[k]; }
        set
        {
            if (_Dx[k] == value) return;
            OnPropertyChanging($"D{k + 1}");
            _Dx[k] = value;
            OnPropertyChanged($"D{k + 1}");
        }
    }

    [PropertyChanged.DoNotNotify]
    [PropertyChanging.DoNotNotify]
    EDayFactId IDLRowDXFactIndexer.this[int k]
    {
        get { return (EDayFactId)Dx[k]; }
        set
        {
            if (Dx[k] == (short)value) return;
            OnPropertyChanging($"D{k + 1}");
            Dx[k] = (short)value;
            OnPropertyChanged($"D{k + 1}");
        }
    }

    [PropertyChanged.DoNotNotify]
    [PropertyChanging.DoNotNotify]
    EDayPlanId IDLRowDXPlanIndexer.this[int k]
    {
        get { return (EDayPlanId)Dx[k]; }
        set
        {
            if (Dx[k] == (short)value) return;
            OnPropertyChanging($"D{k + 1}");
            Dx[k] = (short)value;
            OnPropertyChanged($"D{k + 1}");
        }
    }

    public void SetAllVx(float[] vx)
    {
        for (int i = 0; i < 31; i++)
            Vx[i] = vx[i];
    }
        
    public float SumV() => _Vx.Sum();

    public EKind1 XKind1
    {
        get { return (EKind1)KIND1; }
        set { KIND1 = (short)value; }
    }

    public static bool IsVx(string sv)
    {
        if (sv.IsNOE() || sv.Length < 2 || sv[0] != 'V') return false;
        var sx = sv[1..];
        if (!int.TryParse(sx, out int x)) return false;
        if (x < 1 || x > 31) return false;
        return true;
    }

    public int ID { get; set; }
    public int? IDP { get; set; }
    public int? IDL { get; set; }
    public short? SNR { get; set; }
    public int? PERID { get; set; }
    public int? AMID { get; set; }
    public int? YR { get; set; } = 2020;
    public int? MT { get; set; } = 1;
    public short? KIND1 { get; set; }
    public short? KIND2 { get; set; }


    #region Vx and Dx
    public float V1 { get => _Vx[0]; set => _Vx[0] = value; }
    public float V2 { get => _Vx[1]; set => _Vx[1] = value; }
    public float V3 { get => _Vx[2]; set => _Vx[2] = value; }
    public float V4 { get => _Vx[3]; set => _Vx[3] = value; }
    public float V5 { get => _Vx[4]; set => _Vx[4] = value; }
    public float V6 { get => _Vx[5]; set => _Vx[5] = value; }
    public float V7 { get => _Vx[6]; set => _Vx[6] = value; }
    public float V8 { get => _Vx[7]; set => _Vx[7] = value; }
    public float V9 { get => _Vx[8]; set => _Vx[8] = value; }
    public float V10 { get => _Vx[9]; set => _Vx[9] = value; }
    public float V11 { get => _Vx[10]; set => _Vx[10] = value; }
    public float V12 { get => _Vx[11]; set => _Vx[11] = value; }
    public float V13 { get => _Vx[12]; set => _Vx[12] = value; }
    public float V14 { get => _Vx[13]; set => _Vx[13] = value; }
    public float V15 { get => _Vx[14]; set => _Vx[14] = value; }
    public float V16 { get => _Vx[15]; set => _Vx[15] = value; }
    public float V17 { get => _Vx[16]; set => _Vx[16] = value; }
    public float V18 { get => _Vx[17]; set => _Vx[17] = value; }
    public float V19 { get => _Vx[18]; set => _Vx[18] = value; }
    public float V20 { get => _Vx[19]; set => _Vx[19] = value; }
    public float V21 { get => _Vx[20]; set => _Vx[20] = value; }
    public float V22 { get => _Vx[21]; set => _Vx[21] = value; }
    public float V23 { get => _Vx[22]; set => _Vx[22] = value; }
    public float V24 { get => _Vx[23]; set => _Vx[23] = value; }
    public float V25 { get => _Vx[24]; set => _Vx[24] = value; }
    public float V26 { get => _Vx[25]; set => _Vx[25] = value; }
    public float V27 { get => _Vx[26]; set => _Vx[26] = value; }
    public float V28 { get => _Vx[27]; set => _Vx[27] = value; }
    public float V29 { get => _Vx[28]; set => _Vx[28] = value; }
    public float V30 { get => _Vx[29]; set => _Vx[29] = value; }
    public float V31 { get => _Vx[30]; set => _Vx[30] = value; }


    public short D1 { get => _Dx[0]; set => _Dx[0] = value; }
    public short D2 { get => _Dx[1]; set => _Dx[1] = value; }
    public short D3 { get => _Dx[2]; set => _Dx[2] = value; }
    public short D4 { get => _Dx[3]; set => _Dx[3] = value; }
    public short D5 { get => _Dx[4]; set => _Dx[4] = value; }
    public short D6 { get => _Dx[5]; set => _Dx[5] = value; }
    public short D7 { get => _Dx[6]; set => _Dx[6] = value; }
    public short D8 { get => _Dx[7]; set => _Dx[7] = value; }
    public short D9 { get => _Dx[8]; set => _Dx[8] = value; }
    public short D10 { get => _Dx[9]; set => _Dx[9] = value; }
    public short D11 { get => _Dx[10]; set => _Dx[10] = value; }
    public short D12 { get => _Dx[11]; set => _Dx[11] = value; }
    public short D13 { get => _Dx[12]; set => _Dx[12] = value; }
    public short D14 { get => _Dx[13]; set => _Dx[13] = value; }
    public short D15 { get => _Dx[14]; set => _Dx[14] = value; }
    public short D16 { get => _Dx[15]; set => _Dx[15] = value; }
    public short D17 { get => _Dx[16]; set => _Dx[16] = value; }
    public short D18 { get => _Dx[17]; set => _Dx[17] = value; }
    public short D19 { get => _Dx[18]; set => _Dx[18] = value; }
    public short D20 { get => _Dx[19]; set => _Dx[19] = value; }
    public short D21 { get => _Dx[20]; set => _Dx[20] = value; }
    public short D22 { get => _Dx[21]; set => _Dx[21] = value; }
    public short D23 { get => _Dx[22]; set => _Dx[22] = value; }
    public short D24 { get => _Dx[23]; set => _Dx[23] = value; }
    public short D25 { get => _Dx[24]; set => _Dx[24] = value; }
    public short D26 { get => _Dx[25]; set => _Dx[25] = value; }
    public short D27 { get => _Dx[26]; set => _Dx[26] = value; }
    public short D28 { get => _Dx[27]; set => _Dx[27] = value; }
    public short D29 { get => _Dx[28]; set => _Dx[28] = value; }
    public short D30 { get => _Dx[29]; set => _Dx[29] = value; }
    public short D31 { get => _Dx[30]; set => _Dx[30] = value; }

    #endregion

    public float? K1 { get; set; }


    public DateTime TS { get; set; }
    public virtual A_TIMESHEET_LISTS_R TimeSheetListRow { get; set; }
    public virtual A_TIMEPLAN_LIST TimePlan { get; set; }
    public virtual A_POSITIONS Position { get; set; }
    public virtual A_PERSONS Person { get; set; }


    public override string GetInfoView()
    {
        string[] darba_laiks_k1 = [
            "grupas plāns", 
            "grupas plāns naktij",
            "ind. plāns", 
            "ind. plāns naktij", 
            "nostrādāts",
            "nostrādāts virst.",
            "nostrādāts naktī"];
        string ret = "";
        if (XKind1 == EKind1.PlanGroupDay || XKind1 == EKind1.PlanGroupaNight)
        {
            ret =
                $""""
                [Darba laika uzskaites rinda] 
                Veids: {(KIND1 == null ? "?" : darba_laiks_k1[KIND1.Value])}
                Lapas kods: {TimePlan?.CODE}
                Lapas nosaukums: {TimePlan?.DESCR}
                Gads: {YR}
                Mēnesis: {MT}
                Stundas: {K1}
                ...
                """";
        }
        else
        {
            ret =
                $""""
                [Darba laika uzskaites rinda] 
                Veids: {(KIND1 == null ? "?" : darba_laiks_k1[KIND1.Value])}
                Lapas Nr.: {TimeSheetListRow?.Timesheet?.SNR}
                Lapas gads: {TimeSheetListRow?.Timesheet?.YR}
                Lapas mēnesis: {TimeSheetListRow?.Timesheet?.MT}
                Lapas apraksts: {TimeSheetListRow?.Timesheet?.DESCR}
                Rindas Nr.: {TimeSheetListRow?.SNR}
                Persona: {Person?.YNAME}
                Amats: {Position?.TITLE}
                Gads: {YR}
                Mēnesis: {MT}
                Stundas: {K1}
                ...
                """"; 
        }
        return ret;
    }


}