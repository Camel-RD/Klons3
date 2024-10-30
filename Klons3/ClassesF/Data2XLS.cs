using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlonsF.Classes;
using KlonsLIB.Forms;
using KlonsLIB.Data;
using KlonsLIB.Misc;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Klons3.ModelsF;
using Klons3;
using System.ComponentModel;
using KlonsLIB;
using ItemPropertyRestrictions = KlonsLIB.Data.MyDbContext.ItemPropertyRestrictions;

namespace KlonsF.Classes;

public class RowsData
{
    private KlonsData MyData => KlonsData.St;

    public List<F_OPSD> OPSdRows = null;
    public List<F_OPS> OPSRows = null;
    public List<F_ACP21> AcP1Rows = null;
    public List<F_ACP23> AcP3Rows = null;
    public List<F_ACP24> AcP4Rows = null;
    public List<F_ACP25> AcP5Rows = null;
    public List<F_PERSONS> PersonsRows = null;
    public List<F_DOCTYP> DocTypRows = null;
    public List<F_CURRENCY> CurrencyRows = null;

    public int TotalRowCount { get; private set; } = 0;

    public RowsData()
    {
    }

    public void GetRows()
    {
        var ds = MyData.DbContextF;

        OPSdRows = ds.F_OPSD.Local
            .OrderBy(d => d.DETE)
            .ThenBy(d => d.ID)
            .ToList();

        OPSRows = ds.F_OPS.Local
            .OrderBy(d => d.DOCRow.DETE)
            .ThenBy(d => d.DOCRow.ID)
            .ThenBy(d => d.ID)
            .ToList();

        var e11 = OPSRows.Select(d => d.AC11Row).Distinct();
        var e21 = OPSRows.Select(d => d.AC21Row).Distinct();
        var e12 = OPSRows.Where(d => d.AC12 != null).Select(d => d.AC12Row).Distinct();
        var e22 = OPSRows.Where(d => d.AC22 != null).Select(d => d.AC22Row).Distinct();
        var e13 = OPSRows.Where(d => d.AC13 != null).Select(d => d.AC13Row).Distinct();
        var e23 = OPSRows.Where(d => d.AC23 != null).Select(d => d.AC23Row).Distinct();
        var e14 = OPSRows.Where(d => d.AC14 != null).Select(d => d.AC14Row).Distinct();
        var e24 = OPSRows.Where(d => d.AC24 != null).Select(d => d.AC24Row).Distinct();
        var e15 = OPSRows.Where(d => d.AC15 != null).Select(d => d.AC15Row).Distinct();
        var e25 = OPSRows.Where(d => d.AC25 != null).Select(d => d.AC25Row).Distinct();
        var epers1 = OPSdRows.Where(d => d.CLID != null).Select(d => ds.F_PERSONS.Find(d.CLID)).Distinct();
        var epers2 = OPSdRows.Where(d => d.CLID2 != null).Select(d => ds.F_PERSONS.Find(d.CLID2)).Distinct();
        var edoctyp = OPSdRows.Where(d => d.DOCTYP != null).Select(d => ds.F_DOCTYP.Find(d.DOCTYP)).Distinct();
        var ecurr = OPSRows.Where(d => d.CUR != "EUR").Select(d => ds.F_CURRENCY.Find(d.CUR, d.DOCRow.DETE)).Where(d => d != null).Distinct();


        AcP1Rows = e11
            .Union(e21)
            .Union(e12)
            .Union(e22)
            .OrderBy(d => d.AC)
            .ToList();

        AcP3Rows = e13
            .Union(e23)
            .OrderBy(d => d.IDX)
            .ToList();

        AcP4Rows = e14
            .Union(e24)
            .OrderBy(d => d.IDX)
            .ToList();

        AcP5Rows = e15
            .Union(e25)
            .OrderBy(d => d.IDX)
            .ToList();

        PersonsRows = epers1
            .Union(epers1)
            .OrderBy(d => d.CLID)
            .ToList();

        DocTypRows = edoctyp
            .OrderBy(d => d.ID)
            .ToList();

        CurrencyRows = ecurr
            .OrderBy(d => d.DETE)
            .ThenBy(d => d.ID)
            .ToList();
    }

    public void GetRowsFull()
    {
        var ds = MyData.DbContextF;
        OPSdRows = ds.F_OPSD.Local
            .OrderBy(d => d.DETE)
            .ThenBy(d => d.ID)
            .ToList();

        OPSRows = ds.F_OPS.Local
            .OrderBy(d => d.DOCRow.DETE)
            .ThenBy(d => d.DOCRow.ID)
            .ThenBy(d => d.ID)
            .ToList();

        AcP1Rows = ds.F_ACP21.Local
            .OrderBy(d => d.AC)
            .ToList();

        AcP3Rows = ds.F_ACP23.Local
            .OrderBy(d => d.IDX)
            .ToList();

        AcP4Rows = ds.F_ACP24.Local
            .OrderBy(d => d.IDX)
            .ToList();

        AcP5Rows = ds.F_ACP25.Local
            .OrderBy(d => d.IDX)
            .ToList();

        PersonsRows = ds.F_PERSONS.Local
            .OrderBy(d => d.CLID)
            .ToList();

        DocTypRows = ds.F_DOCTYP.Local
            .OrderBy(d => d.ID)
            .ToList();

        var ecurr = OPSRows.Where(d => d.CUR != "EUR")
            .Select(d => ds.F_CURRENCY.Find(d.CUR, d.DOCRow.DETE))
            .Where(d => d != null)
            .Distinct();
        CurrencyRows = ecurr
            .OrderBy(d => d.DETE)
            .ThenBy(d => d.ID)
            .ToList();
    }

    public void GetTotalRowCount()
    {
        TotalRowCount = OPSRows.Count + AcP1Rows.Count + AcP3Rows.Count +
            AcP4Rows.Count + AcP5Rows.Count + PersonsRows.Count +
            DocTypRows.Count + CurrencyRows.Count;
    }

    public List<InfoRow> GetCounts()
    {
        var ret = new List<InfoRow>()
        {
            new InfoRow("Dokumenti", OPSdRows.Count),
            new InfoRow("Ieraksti", OPSRows.Count),
            new InfoRow("Kontu plāns", AcP1Rows.Count),
            new InfoRow("Darijumu žurnāla pazīmes", AcP3Rows.Count),
            new InfoRow("Papildpazīmes", AcP4Rows.Count),
            new InfoRow("PVN pazīmes", AcP5Rows.Count),
            new InfoRow("Personas", PersonsRows.Count),
            new InfoRow("Dokumentu veidi", DocTypRows.Count),
            new InfoRow("Valūtas kursi", CurrencyRows.Count),
        };
        return ret;
    }

    public List<InfoRow> GetCountsNone()
    {
        var ret = new List<InfoRow>();
        string[] titles = { "Ieraksti", "Kontu plāns", "Darijumu žurnāla pazīmes"
                ,"Papildpazīmes", "PVN pazīmes", "Personas","Dokumentu veidi","Valūtas kursi"};
        for (int i = 0; i < titles.Length; i++)
        {
            var ir = new InfoRow(titles[i], 0);
            ret.Add(ir);
        }
        return ret;
    }

    public void ExportToXLS(WorkBookConfig wbc)
    {
        IEnumerable<EntityBase>[] rowsets = { OPSRows, AcP1Rows, AcP3Rows
                ,AcP4Rows, AcP5Rows, PersonsRows, DocTypRows, CurrencyRows};

        wbc.RowsDone = 0;
        for (int i = 0; i <= 7; i++)
        {
            wbc.SheetsConfig[i].MakeSheet(rowsets[i]);
            if (wbc.Cancel) return;
        }

    }

}

public enum ESheetTask
{
    Ignore, AddNew, DoAll
}

public class InfoRow
{
    public InfoRow(string caption, int count)
    {
        Caption = caption;
        Count = count;
    }

    public string Caption { get; set; } = null;
    public int Count { get; set; } = 0;
    public int CountNew { get; set; } = 0;
    public int CountExisting { get; set; } = 0;
    public int CountChanging { get; set; } = 0;
    public int CountBad { get; set; } = 0;
    public int Task { get; set; } = 2;

}

public enum EFieldType
{
    Int, Int16, Single, Double, Decimal, String, Date, DateTime, Bool
}

public enum ERowKind
{
    Ignore, New, Existing, Changing
}

public interface IFieldHelper
{
    Type GetRowType();
    Type GetFieldType(string fieldname);
    bool GetAllowNull(string fieldname);
    int GetMaxLength(string fieldname);
    object GetFieldValue(string fieldname, object dr);
    void SetFieldValue(string fieldname, object dr, object value);
}

public class FieldConfig
{
    public int Index = 0;
    public string Caption = "";
    public string FieldName = "";
    public EFieldType FieldType = EFieldType.String;
    public int MaxLength = 0;
    public int WidthInChars = 10;
    public ICellStyle CellStyle = null;
    public bool AllowNull = false;

    public FieldConfig()
    {

    }

    public FieldConfig(string caption, string fieldname)
    {
        Caption = caption;
        FieldName = fieldname;
    }

    public IFieldHelper FieldHelper = null;

    public void UpdateField()
    {
        Type tp = FieldHelper.GetFieldType(FieldName);
        if (tp.IsGenericType) 
            tp = Nullable.GetUnderlyingType(tp);
        if (tp == typeof(int)) FieldType = EFieldType.Int;
        else if (tp == typeof(Int16)) FieldType = EFieldType.Int16;
        else if (tp == typeof(Single)) FieldType = EFieldType.Single;
        else if (tp == typeof(double)) FieldType = EFieldType.Double;
        else if (tp == typeof(decimal)) FieldType = EFieldType.Decimal;
        else if (tp == typeof(string)) FieldType = EFieldType.String;
        else if (tp == typeof(DateTime)) FieldType = EFieldType.Date;
        else if (tp == typeof(bool)) FieldType = EFieldType.Bool;
        else
            throw new Exception("Bad field type.");

        MaxLength = tp == typeof(string) ? FieldHelper.GetMaxLength(FieldName) : 0;
        AllowNull = FieldHelper.GetAllowNull(FieldName);
    }

    public void SetCellStyle(WorkBookConfig wbc)
    {
        switch (FieldType)
        {
            case EFieldType.Date:
                CellStyle = wbc.CellStyleDate;
                break;
            case EFieldType.DateTime:
                CellStyle = wbc.CellStyleDateTime;
                break;
            case EFieldType.Decimal:
                CellStyle = wbc.CellStyleMoney;
                break;
        }
    }

}

public class BaseFieldHelper : IFieldHelper
{
    public Type ItemType;
    public PropertyDescriptorCollection Properties;
    public Dictionary<string, ItemPropertyRestrictions> Restrictions;
    
    public BaseFieldHelper(Type itemtype)
    {
        ItemType = itemtype;
        Properties = TypeDescriptor.GetProperties(ItemType);
        Restrictions = KlonsData.St.DbContextF.GetItemPropertiesRestrictions(ItemType);
    }

    public bool GetAllowNull(string fieldname)
    {
        if (Restrictions.TryGetValue(fieldname, out var restrictions))
            return restrictions.AllowNull;
        return ItemPropertyRestrictions.Default.AllowNull;
    }

    public Type GetFieldType(string fieldname)
    {
        var prop = Properties[fieldname];
        if (prop == null)
            throw new InvalidOperationException($"Property not found {fieldname}");
        return prop.PropertyType;
    }

    public int GetMaxLength(string fieldname)
    {
        if (Restrictions.TryGetValue(fieldname, out var restrictions))
            return restrictions.MaxLength;
        return ItemPropertyRestrictions.Default.MaxLength;
    }

    public Type GetRowType()
    {
        return ItemType;
    }

    public virtual object GetFieldValue(string fieldname, object dr)
    {
        var prop = Properties[fieldname];
        if (prop == null)
            throw new InvalidOperationException($"Property not found {fieldname}");
        return prop.GetValue(dr);
    }

    public virtual void SetFieldValue(string fieldname, object dr, object value)
    {
        var prop = Properties[fieldname];
        if (prop == null)
            throw new InvalidOperationException($"Property not found {fieldname}");
        prop.SetValue(dr, value);
    }
}

public class OpsdFieldHelper : BaseFieldHelper
{
    public OpsdFieldHelper(Type itemtype) : base(itemtype)
    {
    }

    public override object GetFieldValue(string fieldname, object dr)
    {
        var prop = Properties[fieldname];
        if (prop == null)
            throw new InvalidOperationException($"Property not found {fieldname}");
        if (dr is not F_OPS dr_ops)
            throw new InvalidOperationException($"Wrong row type");
        return prop.GetValue(dr_ops.DOCRow);
    }

    public override void SetFieldValue(string fieldname, object dr, object value)
    {
        var prop = Properties[fieldname];
        if (prop == null)
            throw new InvalidOperationException($"Property not found {fieldname}");
        prop.SetValue(dr, value);
    }
}

public class SheetConfig
{
    public WorkBookConfig WBC { get; protected set; } = null;
    public List<FieldConfig> FieldsConfig = new List<FieldConfig>();

    public string SheetName { get; set; } = "";
    public int RowCount { get; set; } = 0;
    public int RowCountNew { get; set; } = 0;
    public int RowCountExisting { get; set; } = 0;
    public int RowCountChanging { get; set; } = 0;
    public int RowCountBad { get; set; } = 0;
    public ESheetTask Task { get; set; } = ESheetTask.AddNew;

    public IFieldHelper FieldHelperForAddSheetField = null;

    public SheetConfig(WorkBookConfig wbc)
    {
        WBC = wbc;
        Init();
    }

    public virtual FieldConfig AddSheetField(string caption, string fieldname,
        int width = 10,
        IFieldHelper fieldhelper = null)
    {
        var ret = new FieldConfig(caption, fieldname);
        ret.Index = FieldsConfig.Count;
        ret.WidthInChars = width;
        if (fieldhelper == null)
            fieldhelper = FieldHelperForAddSheetField;
        if (fieldhelper == null)
            throw new InvalidOperationException("fieldhelper is null");
        ret.FieldHelper = fieldhelper;
        FieldsConfig.Add(ret);
        return ret;
    }

    public void UpdateFields()
    {
        foreach (var fc in FieldsConfig)
        {
            fc.UpdateField();
        }
    }

    public virtual void Init()
    {
    }

    public void SetCellValue(ICell cell, FieldConfig fc, object value)
    {
        if (value == DBNull.Value || value == null)
            cell.SetCellType(CellType.Blank);
        else
        {
            switch (fc.FieldType)
            {
                case EFieldType.Int:
                    cell.SetCellValue((double)(int)value);
                    break;
                case EFieldType.Int16:
                    cell.SetCellValue((double)(Int16)value);
                    break;
                case EFieldType.Single:
                    cell.SetCellValue((double)(Single)value);
                    break;
                case EFieldType.Double:
                    cell.SetCellValue((double)value);
                    break;
                case EFieldType.Decimal:
                    cell.SetCellValue((double)(decimal)value);
                    break;
                case EFieldType.String:
                    string s = (value == DBNull.Value) ? null : (string)value;
                    cell.SetCellValue(s);
                    break;
                case EFieldType.DateTime:
                case EFieldType.Date:
                    cell.SetCellValue((DateTime)value);
                    break;
                case EFieldType.Bool:
                    cell.SetCellValue((bool)value);
                    break;
            }
        }
    }

    public string GetCellValue(ICell cell, FieldConfig fc, out object value)
    {
        value = null;
        if (cell.CellType == CellType.Error || cell.CellType == CellType.Unknown)
            return "Kļūdaina vērtība.";

        if (cell.CellType == CellType.Blank)
        {
            if (!fc.AllowNull) return "Nav norādīta vērtība.";
            value = null;
            return "OK";
        }

        switch (fc.FieldType)
        {
            case EFieldType.Int:
            case EFieldType.Int16:
            case EFieldType.Single:
            case EFieldType.Double:
            case EFieldType.Decimal:
                if (cell.CellType != CellType.Numeric)
                    return "Kļudains vērtības tips.";
                break;
            case EFieldType.String:
                if (cell.CellType != CellType.String)
                    return "Kļudains vērtības tips.";
                break;
            case EFieldType.DateTime:
            case EFieldType.Date:
                if (cell.CellType != CellType.Numeric)
                    return "Kļudains vērtības tips.";
                break;
            case EFieldType.Bool:
                if (cell.CellType != CellType.Boolean)
                    return "Kļudains vērtības tips.";
                break;
        }
        try
        {

            switch (fc.FieldType)
            {
                case EFieldType.Int:
                    value = (int)cell.NumericCellValue;
                    break;
                case EFieldType.Int16:
                    value = (Int16)cell.NumericCellValue;
                    break;
                case EFieldType.Single:
                    value = (Single)cell.NumericCellValue;
                    break;
                case EFieldType.Double:
                    value = (double)cell.NumericCellValue;
                    break;
                case EFieldType.Decimal:
                    value = (decimal)cell.NumericCellValue;
                    break;
                case EFieldType.String:
                    string s = cell.StringCellValue.Zn();
                    value = s;
                    if (s != null && fc.MaxLength > 0 && s.Length > fc.MaxLength)
                        return "Teksta garums pārsniedz atļauto.";
                    break;
                case EFieldType.DateTime:
                case EFieldType.Date:
                    value = cell.DateCellValue;
                    break;
                case EFieldType.Bool:
                    value = cell.BooleanCellValue;
                    break;
            }
        }
        catch (Exception)
        {
            return "Kļūdaina vērtība.";
        }
        return "OK";
    }

    public string GetBlancCellValue(FieldConfig fc, ref object value)
    {
        if (!fc.AllowNull) return "Nav norādīta vērtība.";
        if (fc.FieldType == EFieldType.String) value = null;
        else value = DBNull.Value;
        return "OK";
    }

    public virtual void SetCellStyle()
    {
        foreach (var fc in FieldsConfig)
        {
            fc.SetCellStyle(WBC);
        }
    }

    public void MakeSheet(IEnumerable<EntityBase> rows)
    {
        ISheet sh1 = WBC.WB.CreateSheet(SheetName);
        var row0 = sh1.CreateRow(0);

        for (int i = 0; i < FieldsConfig.Count; i++)
        {
            var fc = FieldsConfig[i];
            var cell = row0.CreateCell(i);
            cell.CellStyle = WBC.CellStyleCations;
            cell.SetCellValue(FieldsConfig[i].Caption);
            sh1.SetColumnWidth(i, fc.WidthInChars * 256);

            if (fc.CellStyle != null)
                sh1.SetDefaultColumnStyle(i, fc.CellStyle);
        }

        int ct = 1;
        foreach (var dr in rows)
        {
            var row = sh1.CreateRow(ct);
            ct++;
            for (int i = 0; i < FieldsConfig.Count; i++)
            {
                var fc = FieldsConfig[i];
                object value = null;
                value = fc.FieldHelper.GetFieldValue(fc.FieldName, dr);
                var cell = row.CreateCell(i);
                SetCellValue(cell, fc, value);
            }
            WBC.RowsDone++;
            WBC.OnProgress();
            if (WBC.Cancel) return;
        }

        sh1.CreateFreezePane(0, 1);
    }

    public int GetFieldIndex(string fieldname, int startindex = 0)
    {
        for (int i = startindex; i < FieldsConfig.Count; i++)
            if (FieldsConfig[i].FieldName == fieldname) return i;
        return -1;
    }

    public bool GetFieldIndex(string field1, string field2, out int ind1, out int ind2)
    {
        ind1 = 0;
        ind2 = FieldsConfig.Count - 1;
        if (field1 != null) ind1 = GetFieldIndex(field1);
        if (ind1 == -1) return false;
        if (field2 != null) ind2 = GetFieldIndex(field2, ind1 + 1);
        if (ind2 == -1) return false;
        return true;
    }

    public virtual bool EqualsDataRowFields(EntityBase row, object[] vr, int ind1 = 0, int ind2 = -1)
    {
        if (ind2 == -1) ind2 = FieldsConfig.Count - 1;
        if (ind1 < 0 || ind1 >= FieldsConfig.Count || ind2 < 0 || ind2 >= FieldsConfig.Count || ind2 < ind1)
            throw new ArgumentException("Bad index.");
        for (int i = ind1; i <= ind2; i++)
        {
            var fc = FieldsConfig[i];
            object drval = fc.FieldHelper.GetFieldValue(fc.FieldName, row);
            object vval = vr[i];
            if (drval == DBNull.Value) drval = null;
            if (vval == DBNull.Value) vval = null;
            if (!object.Equals(drval, vval)) return false;
        }
        return true;
    }

    public virtual bool EqualsDataRowFieldsA(EntityBase row, object[] vr, string field1 = null, string field2 = null)
    {
        if(!GetFieldIndex(field1, field2, out int ind1, out int ind2))
            throw new ArgumentException("Bad field.");
        return EqualsDataRowFields(row, vr, ind1, ind2);
    }

    public virtual void SetDataRowFields(EntityBase row, object[] vr, int ind1 = 0, int ind2 = -1)
    {
        if (ind2 == -1) ind2 = FieldsConfig.Count - 1;
        if (ind1 < 0 || ind1 >= FieldsConfig.Count || ind2 < 0 || ind2 >= FieldsConfig.Count || ind2 < ind1)
            throw new ArgumentException("Bad index.");

        if (EqualsDataRowFields(row, vr, ind1, ind2)) return;

        for (int i = ind1; i <= ind2; i++)
        {
            var fc = FieldsConfig[i];
            fc.FieldHelper.SetFieldValue(fc.FieldName, row, vr[i]);
        }
    }

    public virtual void SetDataRowFieldsA(EntityBase row, object[] vr, string field1 = null, string field2 = null)
    {
        if (!GetFieldIndex(field1, field2, out int ind1, out int ind2))
            throw new ArgumentException("Bad field.");

        SetDataRowFields(row, vr, ind1, ind2);
    }

    public virtual ERowKind GetRowKind(object[] valuerow)
    {
        return ERowKind.Ignore;
    }

    public virtual void Prepare()
    {

    }

    public virtual void UseRow(object[] valuerow)
    {

    }

    public int GetFirstCount()
    {
        ISheet sh1 = WBC.WB.GetSheet(SheetName);
        if (sh1 == null) return 0;
        return sh1.PhysicalNumberOfRows - 1;
    }

    public List<CellError> TestSheet()
    {
        RowCount = 0;
        RowCountNew = 0;
        RowCountExisting = 0;
        RowCountChanging = 0;
        RowCountBad = 0;

        var ret = new List<CellError>();
        ISheet sh1 = WBC.WB.GetSheet(SheetName);
        if (sh1 == null)
        {
            ret.Add(new CellError(SheetName, 0, 0, "Nav tādas lapas."));
            return ret;
        }
        if (sh1.PhysicalNumberOfRows == 0)
        {
            ret.Add(new CellError(SheetName, 0, 0, "Nav datu."));
            return ret;
        }

        RowCount = sh1.PhysicalNumberOfRows - 1;

        var row0 = sh1.GetRow(0);
        if (row0.Cells.Count != FieldsConfig.Count)
        {
            ret.Add(new CellError(SheetName, 0, 0, "Nekorekts kolonu skaits."));
            return ret;
        }

        for (int i = 0; i < FieldsConfig.Count; i++)
        {
            var fc = FieldsConfig[i];
            var cell = row0.Cells[i];
            if (cell.CellType != CellType.String || cell.StringCellValue != fc.Caption)
                ret.Add(new CellError(SheetName, 0, i, "Nekorekts kolonnas nosaukums."));
        }

        if (ret.Count > 0) return ret;

        var valuerow = new object[FieldsConfig.Count];

        for (int i = 1; i < sh1.PhysicalNumberOfRows; i++)
        {
            var row = sh1.GetRow(i);
            if (row == null)
            {
                ret.Add(new CellError(SheetName, i, 0, "Tukša rinda."));
                return ret;
            }

            bool badrow = false;
            for (int j = 0; j < FieldsConfig.Count; j++)
            {
                object value = null;
                var fc = FieldsConfig[j];
                var cell = row.GetCell(j);
                string er = null;
                if (cell == null)
                    er = GetBlancCellValue(fc, ref value);
                else
                    er = GetCellValue(cell, fc, out value);
                valuerow[j] = value;
                if (er != "OK")
                {
                    badrow = true;
                    ret.Add(new CellError(SheetName, i, j, er));
                }
            }

            if (badrow)
            {
                RowCountBad++;
            }
            else
            {
                var rowkind = GetRowKind(valuerow);
                switch (rowkind)
                {
                    case ERowKind.New:
                        RowCountNew++;
                        break;
                    case ERowKind.Existing:
                        RowCountExisting++;
                        break;
                    case ERowKind.Changing:
                        RowCountChanging++;
                        break;
                }
            }

            WBC.RowsDone++;
            WBC.OnProgress();
            if (WBC.Cancel) return ret;
        }

        return ret;
    }

    public string ReadSheet()
    {
        ISheet sh1 = WBC.WB.GetSheet(SheetName);
        var row0 = sh1.GetRow(0);
        var valuerow = new object[FieldsConfig.Count];

        Prepare();

        for (int i = 1; i < sh1.PhysicalNumberOfRows; i++)
        {
            var row = sh1.GetRow(i);
            for (int j = 0; j < FieldsConfig.Count; j++)
            {
                object value = null;
                var fc = FieldsConfig[j];
                var cell = row.GetCell(j);
                string er = null;
                if (cell == null)
                    er = GetBlancCellValue(fc, ref value);
                else
                    er = GetCellValue(cell, fc, out value);
                valuerow[j] = value;
                if (er != "OK")
                    return $"Sheet:{SheetName}, Row:{i}, col:{j}, {er}";
            }

            try
            {
                UseRow(valuerow);
            }
            catch (Exception ex)
            {
                return $"Sheet:{SheetName}, Row:{i}, {ex.Message}";
            }

            WBC.RowsDone++;
            WBC.OnProgress();
            if (WBC.Cancel) return "OK";
        }

        return "OK";
    }
}

public class CellError
{
    public string SheetName { get; set; } = null;
    public int RowNr { get; set; } = -1;
    public int ColNr { get; set; } = -1;
    public string Error { get; set; } = null;

    public CellError(string sheetname, int rownr, int colnr, string error)
    {
        SheetName = sheetname;
        RowNr = rownr;
        ColNr = colnr;
        Error = error;
    }
}

public class OPSSheetConfig : SheetConfig
{
    public OPSSheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Ieraksti";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_OPS));

        AddSheetField("Dok.Id.", "DOCID", 6);
        AddSheetField("Ier.Id.", "ID", 6);

        FieldHelperForAddSheetField = new OpsdFieldHelper(typeof(F_OPSD));

        AddSheetField("Nr.", "ZNR", 6);
        AddSheetField("Datums", "DETE", 11);
        AddSheetField("Dok.veids", "DOCTYP", 8);
        AddSheetField("Sērija", "DOCST", 6);
        AddSheetField("Dok.numurs", "DOCNR", 12);
        AddSheetField("Persona", "CLID", 15);
        AddSheetField("Apraksts", "DESCR", 50);
        AddSheetField("Summa", "SUMM", 11);
        AddSheetField("PVN", "PVN", 11);
        AddSheetField("Nor.persona", "CLID2", 15);
        AddSheetField("Nr.2", "NRX", 5);
        AddSheetField("Datums2", "DT2", 11);
        AddSheetField("Lietotājs", "ZU", 12);
        var fzdt = AddSheetField("Labojuma laiks", "ZDT", 18);

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_OPS));

        AddSheetField("Debets", "AC11", 10);
        AddSheetField("D.n.p.", "AC12", 10);
        AddSheetField("D.iin", "AC13", 5);
        AddSheetField("D.kat.", "AC14", 10);
        AddSheetField("D.pvn", "AC15", 6);
        AddSheetField("Kredits", "AC21", 10);
        AddSheetField("K.n.p.", "AC22", 10);
        AddSheetField("K.iin", "AC23", 5);
        AddSheetField("K.kat.", "AC24", 10);
        AddSheetField("K.pvn", "AC25", 6);
        AddSheetField("Summa", "SUMMC", 11);
        AddSheetField("Valūta", "CUR", 5);
        AddSheetField("EUR", "SUMM", 11);
        AddSheetField("Daudzums", "QV", 8);
        AddSheetField("Apraksts", "DESCR", 20);

        UpdateFields();
        fzdt.FieldType = EFieldType.DateTime;
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        return ERowKind.New;
    }

    private int lastXLSDocId = 0;
    private bool haveLastDocId = false;
    F_OPSD current_opsd_row = null;

    public override void Prepare()
    {
        haveLastDocId = false;
        current_opsd_row = null;
    }

    public override void UseRow(object[] valuerow)
    {
        int docid = (int)valuerow[0];
        if (!haveLastDocId || docid != lastXLSDocId)
        {
            current_opsd_row = new F_OPSD();
            SetDataRowFieldsA(current_opsd_row, valuerow, "ZNR", "DT2");
            current_opsd_row.ZU = KlonsData.St.CurrentUserName;
            current_opsd_row.ZDT = DateTime.Now;
            current_opsd_row.ID = default;
            KlonsData.St.DbContextF.F_OPSD.Add(current_opsd_row);

            lastXLSDocId = docid;
            haveLastDocId = true;
        }

        var dr_ops = new F_OPS();
        SetDataRowFieldsA(dr_ops, valuerow, "DOCID", "ID");
        SetDataRowFieldsA(dr_ops, valuerow, "AC11", "DESCR");
        dr_ops.ID = default;
        dr_ops.DOCID = current_opsd_row.ID;
        dr_ops.DOCRow = current_opsd_row;
        dr_ops.ZDT = DateTime.Now;
        current_opsd_row.OPSRows.Add(dr_ops);
    }

}

public class AC1SheetConfig : SheetConfig
{
    public AC1SheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Kontu plāns";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_ACP21));

        AddSheetField("Konts", "AC", 10);
        AddSheetField("Nosaukums", "NAME", 50);
        AddSheetField("Paz.np.", "ID1", 8);
        AddSheetField("Paz.2", "ID2", 8);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP21;
        string ac = (string)valuerow[0];
        var dr = table.Find(ac);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP21;
        string ac = (string)valuerow[0];
        var dr = table.Find(ac);
        if(dr == null)
        {
            dr = new F_ACP21();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }

}

public class AC3SheetConfig : SheetConfig
{
    public AC3SheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Darijumu pazīmes";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_ACP23));

        AddSheetField("Kods", "IDX", 5);
        AddSheetField("Nosaukums", "NAME", 50);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        return ERowKind.Ignore;
    }

}

public class AC4SheetConfig : SheetConfig
{
    public AC4SheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Papildpazīmes";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_ACP24));

        AddSheetField("Kods", "IDX", 10);
        AddSheetField("Nosaukums", "NAME", 50);
        AddSheetField("Mērv.", "UNIT", 8);
        AddSheetField("Cena", "PRICE", 8);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP24;
        string idx = (string)valuerow[0];
        var dr = table.Find(idx);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP24;
        string idx = (string)valuerow[0];
        var dr = table.Find(idx);
        if (dr == null)
        {
            dr = new F_ACP24();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }

}

public class AC5SheetConfig : SheetConfig
{
    public AC5SheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "PVN pazīmes";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_ACP25));

        AddSheetField("Kods", "IDX", 8);
        AddSheetField("Nosaukums", "NAME", 50);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP25;
        string idx = (string)valuerow[0];
        var dr = table.Find(idx);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_ACP25;
        string idx = (string)valuerow[0];
        var dr = table.Find(idx);
        if (dr == null)
        {
            dr = new F_ACP25();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }

}

public class PersonsSheetConfig : SheetConfig
{
    public PersonsSheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Personas";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_PERSONS));

        AddSheetField("Kods", "CLID", 15);
        AddSheetField("Nosaukums", "NAME", 50);
        AddSheetField("Reģ.nr.", "REGNR", 13);
        AddSheetField("PVN reģ.nr.", "PVNREGNR", 16);
        AddSheetField("Veids", "TP", 5);
        AddSheetField("Fp/Jp", "TP2", 6);
        AddSheetField("PVN", "TP3", 5);
        AddSheetField("Jur.adrese", "ADDR", 30);
        AddSheetField("Fiz.adrese", "ADDR2", 30);
        AddSheetField("ATK", "ATK", 10);
        AddSheetField("Bankas kods", "BANKID", 10);
        AddSheetField("Bankas nosaukums", "BANK", 20);
        AddSheetField("Bankas konts", "BANKACC", 20);
        AddSheetField("Telefons", "PHONE", 10);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_PERSONS;
        string clid = (string)valuerow[0];
        var dr = table.Find(clid);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_PERSONS;
        string clid = (string)valuerow[0];
        var dr = table.Find(clid);
        if (dr == null)
        {
            dr = new F_PERSONS();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }

}


public class DocTypSheetConfig : SheetConfig
{
    public DocTypSheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Dokumentu veidi";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_DOCTYP));

        AddSheetField("Kods", "ID", 8);
        AddSheetField("Nosaukums", "NAME", 50);
        AddSheetField("Kods2", "ID1", 8);
        AddSheetField("PVN paz", "PVNPAZ", 8);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_DOCTYP;
        string id = (string)valuerow[0];
        var dr = table.Find(id);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_DOCTYP;
        string id = (string)valuerow[0];
        var dr = table.Find(id);
        if (dr == null)
        {
            dr = new F_DOCTYP();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }
}

public class CurrencySheetConfig : SheetConfig
{
    public CurrencySheetConfig(WorkBookConfig wbc) : base(wbc)
    {
    }

    public override void Init()
    {
        SheetName = "Valūtas kursi";

        FieldHelperForAddSheetField = new BaseFieldHelper(typeof(F_CURRENCY));

        AddSheetField("Kods", "ID", 6);
        AddSheetField("Datums", "DETE", 11);
        AddSheetField("Kurs", "RATE", 10);

        UpdateFields();
    }

    public override ERowKind GetRowKind(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_CURRENCY;
        string id = (string)valuerow[0];
        DateTime dt = (DateTime)valuerow[1];
        var dr = table.Find(id, dt);
        if (dr == null) return ERowKind.New;
        if (EqualsDataRowFields(dr, valuerow)) return ERowKind.Existing;
        return ERowKind.Changing;
    }

    public override void UseRow(object[] valuerow)
    {
        var table = KlonsData.St.DbContextF.F_CURRENCY;
        string id = (string)valuerow[0];
        DateTime dt = (DateTime)valuerow[1];
        var dr = table.Find(id, dt);
        if (dr == null)
        {
            dr = new F_CURRENCY();
            SetDataRowFields(dr, valuerow);
            table.Add(dr);
        }
        else
        {
            if (Task != ESheetTask.DoAll) return;
            SetDataRowFields(dr, valuerow);
        }
    }

}

public class WorkBookConfig
{
    public IWorkbook WB { get; protected set; }
    public Stream Stream = null;

    public List<SheetConfig> SheetsConfig = null;

    public ICellStyle CellStyleCations = null;
    public ICellStyle CellStyleMoney = null;
    public ICellStyle CellStyleDate = null;
    public ICellStyle CellStyleDateTime = null;

    public bool Cancel = false;
    public int RowsDone = 0;

    public int RowCount = 0;
    public int RowCountNew = 0;
    public int RowCountExisting = 0;
    public int RowCountChanging = 0;
    public int RowCountBad = 0;

    public bool ImportDone = false;

    public event EventHandler ProgressChanged;

    public WorkBookConfig()
    {
        WB = new HSSFWorkbook();
        InitSheetsConfig();
    }

    public WorkBookConfig(Stream sr)
    {
        WB = new HSSFWorkbook(sr);
        Stream = sr;
        InitSheetsConfig();
    }

    public void InitSheetsConfig()
    {
        SheetsConfig = new List<SheetConfig>()
        {
            new OPSSheetConfig(this),
            new AC1SheetConfig(this),
            new AC3SheetConfig(this),
            new AC4SheetConfig(this),
            new AC5SheetConfig(this),
            new PersonsSheetConfig(this),
            new DocTypSheetConfig(this),
            new CurrencySheetConfig(this),
        };
    }

    public void InitCellStyles()
    {
        if (WB == null) throw new Exception("No workbook.");
        var dataformat = WB.CreateDataFormat();
        var boldfont = WB.CreateFont();
        boldfont.IsBold = true;

        CellStyleCations = WB.CreateCellStyle();
        CellStyleMoney = WB.CreateCellStyle();
        CellStyleDate = WB.CreateCellStyle();
        CellStyleDateTime = WB.CreateCellStyle();

        CellStyleCations.SetFont(boldfont);
        CellStyleMoney.DataFormat = dataformat.GetFormat("# ##0.00;-# ##0.00");
        CellStyleDate.DataFormat = dataformat.GetFormat("yyyy.MM.dd");
        CellStyleDateTime.DataFormat = dataformat.GetFormat("yyyy.MM.dd HH:mm");

        foreach (var shc in SheetsConfig)
        {
            shc.SetCellStyle();
        }
    }

    public void OnProgress()
    {
        if (ProgressChanged != null)
            ProgressChanged(this, new EventArgs());
    }

    public int GetFirstCount()
    {
        int ret = 0;
        foreach (var shc in SheetsConfig)
        {
            ret += shc.GetFirstCount();
        }
        return ret;
    }

    public void GetCount()
    {
        RowCount = 0;
        RowCountNew = 0;
        RowCountExisting = 0;
        RowCountChanging = 0;
        RowCountBad = 0;
        foreach (var sh in SheetsConfig)
        {
            if (sh.Task == ESheetTask.Ignore) continue;
            RowCount += sh.RowCount;
            RowCountNew += sh.RowCountNew;
            RowCountExisting += sh.RowCountExisting;
            RowCountChanging += sh.RowCountChanging;
            RowCountBad += sh.RowCountBad;
        }
    }

    public int GetUpdateCount()
    {
        int ct = 0;
        foreach (var sh in SheetsConfig)
        {
            if (sh.Task == ESheetTask.Ignore) continue;
            ct += sh.RowCountNew;
            if (sh.Task == ESheetTask.DoAll) ct += sh.RowCountChanging;
        }
        return ct;
    }

    public List<InfoRow> GetCountA()
    {
        var ret = new List<InfoRow>();
        string[] titles = { "Ieraksti", "Kontu plāns", "Darijumu žurnāla pazīmes"
                ,"Papildpazīmes", "PVN pazīmes", "Personas","Dokumentu veidi","Valūtas kursi"};
        for (int i = 0; i < titles.Length; i++)
        {
            var shc = SheetsConfig[i];
            var ir = new InfoRow(titles[i], shc.RowCount)
            {
                CountNew = shc.RowCountNew,
                CountExisting = shc.RowCountExisting,
                CountChanging = shc.RowCountChanging,
                CountBad = shc.RowCountBad
            };
            ret.Add(ir);
        }
        return ret;
    }

    public List<CellError> TestXLS()
    {
        var ret = new List<CellError>();
        RowsDone = 0;
        for (int i = 0; i <= 7; i++)
        {
            var shc = SheetsConfig[i];
            var er = shc.TestSheet();
            ret.AddRange(er);
            if (Cancel) return ret;
        }
        return ret;
    }

    public string ImportXLS()
    {
        RowsDone = 0;
        int[] ord = { 1, 2, 3, 4, 5, 6, 7, 0 };
        for (int i = 0; i <= 7; i++)
        {
            int j = ord[i];
            var shc = SheetsConfig[j];
            if (shc.Task == ESheetTask.Ignore) continue;
            string er = null;
            try
            {
                er = shc.ReadSheet();
            }
            catch (Exception ex)
            {
                return $"Sheet:{shc.SheetName}, {ex.ToString()}";
            }
            if (er != "OK") return er;
            if (Cancel) return "OK";
        }
        return "OK";
    }

}
