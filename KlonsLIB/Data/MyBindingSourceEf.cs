using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using KlonsLIB.Components;
using System;
using System.Collections;
using KlonsLIB.ObjectSelector;
using Equin.ApplicationFramework;
using KlonsLIB.BindingListView;
using KGySoft.ComponentModel;
using Microsoft.EntityFrameworkCore;
using ItemPropertyRestrictions = KlonsLIB.Data.MyDbContext.ItemPropertyRestrictions;
using KlonsLIB.Misc;
using KlonsLIB.Forms;
using static KlonsLIB.Data.MyDbContext;

namespace KlonsLIB.Data;

public class MyBindingSourceEf : BindingSource
{
    public MyBindingSourceEf() : base()
    {
    }

    public MyBindingSourceEf(IContainer container) : this()
    {
        if (container == null)
        {
            throw new ArgumentNullException("container");
        }
        container.Add(this);
    }

    [DefaultValue(null)]
    [Category("Misc")]
    public DataGridView UseDataGridView { get; set; } = null;


    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new object DataSource
    {
        get { return base.DataSource; }
        set
        {
            if (value == DataSource) return;
            if (value is null)
            {
                SetDataSourceFromBindingListView(null);
            }
            else if (value is IBindingListView bl_value)
            {
                SetDataSourceFromBindingListView(bl_value);
            }
            else
            {
                throw new InvalidOperationException("DataSource is of wrong type");
            }
        }
    }

    private KlonsDbSetSelector _MyDataSource = KlonsDbSetSelector.Empty;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBindingListView BindingListView { get; protected set; } = null;

    [Category("Data")]
    [DefaultValue(false)]
    public bool AutoSaveOnDelete { get; set; } = false;

    [Category("My")]
    public event MyItemPropertyChangedventHandler ListItemPropertyChanged;

    [Category("My")]
    public event ItemRmoveEventHandler ListItemRemoved;

    [Category("My")]
    public event ItemRmoveEventHandler ListItemRemoving;


    [Category("Data")]
    [RefreshProperties(RefreshProperties.All)]
    public KlonsDbSetSelector MyDataSource
    {
        get => _MyDataSource;
        set
        {
            if (_MyDataSource.Equals(value)) return;
            _MyDataSource = new KlonsDbSetSelector(value?.DataSource, value?.DataMember);
            RecreateLists();
        }
    }

    public void RecreateLists()
    {
        var dbsetbl = KlonsDataModuleEF.St.GetDbSetBindingList(_MyDataSource.DataSource, _MyDataSource.DataMember);
        if (dbsetbl == null)
        {
            if (_MyDataSource.DataMember?.Value != null && _MyDataSource.DataSource?.Value != null)
                _MyDataSource = KlonsDbSetSelector.Empty;
            SetDataSourceFromBindingList(null);
            return;
        }
        SetDataSourceFromBindingList(dbsetbl);
    }

    //we do this to avoid issue when Sort is set
    public void KillLists()
    {
        var tp = GetItemType();
        if (tp == null) return;
        var tp_bp = typeof(BindingList<>).MakeGenericType([tp]);
        var empty_list = Activator.CreateInstance(tp_bp) as IBindingList;
        SetDataSourceFromBindingList(empty_list);
    }

    public void SetMyDataSourceWithoutBinding(string datasourcename, string datamemnber)
    {
        _MyDataSource = new KlonsDbSetSelector(datasourcename, datamemnber);
    }

    public void SetDataSourceFromBindingList(IBindingList new_datasource)
    {
        if (new_datasource == null)
        {
            SetDataSourceFromBindingListView(null);
            return;
        }
        if (_MyDataSource == null)
            throw new InvalidOperationException("MyDataSource not set");
        var tp = new_datasource.GetType();
        if (!tp.IsGenericType ||
            !tp.IsAssignableTo(typeof(BindingList<>).MakeGenericType(tp.GenericTypeArguments)))
            throw new ArgumentException("BindingList<> expected");
        var tp_blv = typeof(BindingListView<>);
        object new_blv = null;
        bool rt = Reflector.TryCreateInstanceByType(tp_blv, tp.GenericTypeArguments, ReflectionWays.Auto, false, out new_blv);
        if (!rt)
            throw new InvalidOperationException("Failed to create BindingListView<>");
        var source_lists = new List<object> { new_datasource };
        var imyblv = new_blv as IMyBindingListView;
        imyblv.HookItemEvents = true;
        imyblv.SourceLists = source_lists;
        imyblv.NewItemsList = new_datasource;
        SetDataSourceFromBindingListView(new_blv as IBindingListView);
    }

    public void SetDataSourceFromBindingListView(IBindingListView new_datasource)
    {
        if ((_MyDataSource == null || _MyDataSource.IsNotSet) && new_datasource != null)
            throw new InvalidOperationException("MyDataSource not set");
        if (new_datasource is not null && new_datasource is not IMyBindingListView)
            throw new ArgumentException("IMyBindingListView expected");
        var current_datarasource = DataSource;
        if (current_datarasource != null && current_datarasource is IMyBindingListView imblv)
        {
            UnHookBindingListView();
            imblv.KillView();
        }
        if (new_datasource == null)
        {
            base.DataSource = null;
            BindingListView = null;
            return;
        }
        base.DataSource = new_datasource;
        BindingListView = new_datasource;
        HookBindingListView();
    }

    private void Blv_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
    {
        ListItemPropertyChanged?.Invoke(this, e);
    }

    protected void HookBindingListView()
    {
        var blv = BindingListView as IMyBindingListView;
        blv.ListItemPropertyChanged += Blv_ListItemPropertyChanged;
    }

    protected void UnHookBindingListView()
    {
        if (BindingListView == null) return;
        var blv = (IMyBindingListView)BindingListView;
        blv.ListItemPropertyChanged -= Blv_ListItemPropertyChanged;
    }

    public T GetCurrentItem<T>() where T : class
    {
        var ov = (Current as ObjectView<T>);
        return ov?.Object;
    }

    public T GetItem<T>(int k) where T : class
    {
        var ov = this[k] as ObjectView<T>;
        return ov?.Object;
    }

    public int IndexOfItem<T>(T item) where T : class
    {
        if (item == null) return -1;
        for (int i = 0; i < Count; i++)
        {
            var t = GetItem<T>(i);
            if (t == item)
                return i;
        }
        return -1;
    }

    public bool SelectItem<T>(T item) where T : class
    {
        int k = IndexOfItem(item);
        if (k == -1) return false;
        Position = k;
        return true;
    }

    public Type GetItemType()

    {
        if (BindingListView == null) return null;
        var tp = BindingListView.GetType();
        if (tp.GenericTypeArguments.Length == 0) return null;
        var rt = tp.GenericTypeArguments[0];
        return rt;
    }

    public PropertyDescriptorCollection GetItemProperties()
    {
        if (BindingListView == null) return null;
        return CurrencyManager.GetItemProperties();
    }

    public MyDbContext GetMyDbContext() => KlonsDataModuleEF.St.GetDataSet(_MyDataSource?.DataSource);

    public ItemRestrictions GetItemPropertiesRestrictions()
    {
        var ctx = GetMyDbContext();
        if (ctx == null) return ItemRestrictions.Default;
        var tp = GetItemType();
        if (tp == null) return ItemRestrictions.Default;
        var ret = ctx.GetItemPropertiesRestrictions(tp);
        return ret;
    }

    public ItemPropertyRestrictions GetItemPropertysRestrictions(string propname)
    {
        var ctx = GetMyDbContext();
        if (ctx == null) return ItemPropertyRestrictions.Default;
        var tp = GetItemType();
        if (tp == null) return ItemPropertyRestrictions.Default;
        var ret = ctx.GetItemPropertyRestrictions(tp, propname);
        return ret;
    }

    public void Fill()
    {
        KillLists();
        try
        {
            var dbset = KlonsDataModuleEF.St.GetDbSet(_MyDataSource?.DataSource, _MyDataSource?.DataMember) ??
                throw new InvalidOperationException("DbSet not found");
            var q = dbset as IQueryable;
            foreach (var _ in q) { }
        }
        finally
        {
            RecreateLists();
        }
    }

    public EBsSaveResult SaveDbContext()
    {
        if (BindingListView == null) return EBsSaveResult.SaveSkipped;
        var ctx = GetMyDbContext() ??
            throw new InvalidOperationException("DbContext not found");
        var rt = ctx.Save();
        return rt;
    }

    public bool HasChanges(bool inthisdbset = true)
    {
        if (BindingListView == null) return false;
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        bool rt = ctx.HasChanges();
        if (rt || !inthisdbset) return rt;
        var tp = GetItemType() ?? throw new InvalidOperationException("ItemType not found");
        rt = ctx.HasChanges(tp);
        return rt;
    }
    
    public void SetFilter<T>(List<Predicate<T>> pred_list)
    {
        if (pred_list == null || pred_list.Count == 0)
        {
            RemoveFilter();
            return;
        }
        var pred_filter = Utils.JoinPredicatesWitnAnd(true, pred_list.ToArray());
        SetFilter(pred_filter);
    }

    public void SetFilter<T>(Predicate<T> filter)
    {
        var blv = BindingListView as BindingListView<T>;
        if (filter == null)
        {
            blv.RemoveFilter();
            return;
        }
        var itemfilter = BindingListView<T>.CreateItemFilter(filter);
        blv.Filter = itemfilter;
    }
    
    public void SetFilter<T>(params Predicate<T>[] filters)
    {
        var filter = Utils.JoinPredicatesWitnAnd(true, filters);
        SetFilter(filter);
    }

    public class GetStatsData
    {
        public int ItmemsAdded = 0;
        public int ItmemsDeleted = 0;
        public int ItmemsModified = 0;

        public string Format()
        {
            var msg = $"jauni: {ItmemsAdded}, dzēsti: {ItmemsDeleted}, laboti: {ItmemsModified}";
            return msg;
        }

        public override string ToString() => Format();
    }

    public GetStatsData GetStats()
    {
        var tp = GetItemType() ?? throw new InvalidOperationException("ItemType not found"); 
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        var stats_data = new GetStatsData();
        (stats_data.ItmemsAdded, stats_data.ItmemsDeleted, stats_data.ItmemsModified) = ctx.GetStats(tp);
        return stats_data;
    }

    public void ShowStats()
    {
        var stats_data = GetStats();
        var msg = stats_data.Format();
        if (MyMainFormBase.MyInstance == null) return;
        MyMainFormBase.MyInstance.ShowInfo(msg);
    }

    public bool CanRemoveCurrent()
    {
        if (!((IBindingList)this).AllowRemove)
            return false;

        if (Position < 0 || Position >= Count)
            return false;

        return true;
    }

    public bool IsItemDetached(object item)
    {
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        var entry = ctx.Entry(item);
        if (entry == null) return true;
        return entry.State == EntityState.Detached;
    }

    public void DeleteFromContext(object value)
    {
        if (value == null) return;
        object item = value;
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        ListItemRemoving?.Invoke(this, new MyItemRemovedEventArgs(item));
        ctx.Remove(item);
        //if (AutoSaveOnDelete)
        {
            ctx.Save();
        }
        ListItemRemoved?.Invoke(this, new MyItemRemovedEventArgs(item));
    }

    public override void Remove(object value)
    {
        if (value == null) return;
        object item = value;
        ListItemRemoving?.Invoke(this, new MyItemRemovedEventArgs(item));
        base.Remove(value);
        if (AutoSaveOnDelete)
        {
            //SaveItem(e.Item);
        }
        ListItemRemoved?.Invoke(this, new MyItemRemovedEventArgs(item));
    }

    public override void RemoveAt(int index)
    {
        object item = ((IList)this)[index];
        ListItemRemoving?.Invoke(this, new MyItemRemovedEventArgs(item));
        base.RemoveAt(index);
        if (AutoSaveOnDelete)
        {
            //SaveItem(e.Item);
        }
        ListItemRemoved?.Invoke(this, new MyItemRemovedEventArgs(item));
    }

    public override PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
    {
        object ds = ListBindingHelper.GetList(this.DataSource);

        if (ds is ITypedList && !String.IsNullOrEmpty(this.DataMember))
        {
            return ListBindingHelper.GetListItemProperties(ds, this.DataMember, listAccessors);
        }
        else
        {
            if (List is IBindingListView blv)
                return ListBindingHelper.GetListItemProperties(List, listAccessors);
            else
                return ListBindingHelper.GetListItemProperties(BindingListView, listAccessors);
        }
    }


    protected override void OnListChanged(ListChangedEventArgs e)
    {
        if (!this.RaiseListChangedEvents || !(this as ISupportInitializeNotification).IsInitialized)
        {
            return;
        }
        //Debug.Print("MyBindingSource2.OnListChanged {0}: {1} {2}", this.Name2, e.NewIndex, e.ListChangedType);
        base.OnListChanged(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            UnHookBindingListView();
        var mblv = BindingListView as IMyBindingListView;
        base.Dispose(disposing);
        if (disposing && mblv != null)
        {
            mblv.KillView();
            BindingListView = null;
        }
    }
}

public class MyItemPropertyChangedEventArgs : EventArgs
{
    public object Item { get; private set; }
    public string PropertyName { get; private set; }
    public MyItemPropertyChangedEventArgs(object item, string propertyName)
    {
        Item = item;
        PropertyName = propertyName;
    }
}
public delegate void MyItemPropertyChangedventHandler(object sender, MyItemPropertyChangedEventArgs e);

public class MyItemRemovedEventArgs : EventArgs
{
    public object Item { get; private set; }

    public MyItemRemovedEventArgs(object item)
    {
        this.Item = item;
    }
}
public delegate void ItemRmoveEventHandler(object sender, MyItemRemovedEventArgs e);

public enum EBsSaveResult
{
    Saved, SaveSkipped, Error, RolledBack, RollBackFailed
}

