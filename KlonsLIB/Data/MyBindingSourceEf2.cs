using Equin.ApplicationFramework;
using KlonsLIB.BindingListView;
using KlonsLIB.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KlonsLIB.Data.MyBindingSourceEf;
using static KlonsLIB.Data.MyDbContext;

namespace KlonsLIB.Data;

public class MyBindingSourceEf2 : BindingSource, ISupportInitialize
{
    [Category("My")]
    public event MyItemPropertyChangedventHandler ListItemPropertyChanged;

    [Category("My")]
    public event ItemRmoveEventHandler ListItemRemoved;

    [Category("My")]
    public event ItemRmoveEventHandler ListItemRemoving;

    [DefaultValue(null)]
    [Category("Misc")]
    public DataGridView UseDataGridView { get; set; } = null;

    [Category("Data")]
    [DefaultValue(false)]
    public bool AutoSaveOnDelete { get; set; } = false;

    public MyBindingSourceEf2() : base()
    {
    }

    public MyBindingSourceEf2(IContainer container) : this()
    {
        if (container == null)
            throw new ArgumentNullException("container");
        container.Add(this);
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

    public Type GetItemType()
    {
        if (DataSource == null) return null;
        var tpo = ListBindingHelper.GetListItemType(DataSource, DataMember);
        if (!tpo.IsGenericType || tpo.GetGenericTypeDefinition() != typeof(ObjectView<>))
            throw new InvalidOperationException($"{nameof(MyBindingSourceEf2)} is intended to use with BindingListView as data source");
        var rt = tpo.GenericTypeArguments[0];
        return rt;
    }


    public PropertyDescriptorCollection GetItemProperties()
    {
        if (List is not IBindingListView) return null;
        return CurrencyManager.GetItemProperties();
    }

    public MyDbContext GetMyDbContext() => (DataSource as MyBindingSourceEf)?.GetMyDbContext();
    /*
    public MyDbContext GetMyDbContext()
    {
        if (DataSource == null) return null;
        if (DataSource is not MyBindingSourceEf bsef)
            throw new InvalidOperationException("Datasource not supported");
        return bsef.GetMyDbContext();
    }
    */

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

    public GetStatsData GetStats()
    {
        var tp = GetItemType() ?? throw new InvalidOperationException("ItemType not found");
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        var stats_data = new GetStatsData();
        (stats_data.ItmemsAdded, stats_data.ItmemsDeleted, stats_data.ItmemsModified) = ctx.GetStats(tp);
        return stats_data;
    }

    private object _innerFilter = null;

    protected void InnerApplyFilter()
    {
        var blv = List as IMyBindingListView;
        if (blv == null) return;
        if (_innerFilter == null)
        {
            blv.RemoveFilter();
            return;
        }
        blv.SetFilter(_innerFilter);
    }

    public void SetFilter<T>(Predicate<T> filter)
    {
        var tp = GetItemType() ?? throw new InvalidOperationException("ItemType not found");
        if (tp != typeof(T)) throw new InvalidOperationException("Expected predicate type " + tp.Name);
        _innerFilter = BindingListView<T>.CreateItemFilter(filter);
        InnerApplyFilter();
    }

    public new void RemoveFilter()
    {
        _innerFilter = null;
        InnerApplyFilter();
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

    public void SetFilter<T>(params Predicate<T>[] filters)
    {
        var filter = Utils.JoinPredicatesWitnAnd(true, filters);
        SetFilter(filter);
    }

    public void Fill()
    {
        var ctx = GetMyDbContext();
        if (ctx == null) return;
        var tp = GetItemType();
        if (tp == null) return;
        var dbset = ctx.GetDbSetByType(tp);
        if (dbset == null) return;
        var q = dbset as IQueryable;
        foreach (var _ in q) { }
    }

    public EBsSaveResult SaveDbContext()
    {
        if (DataSource == null) return EBsSaveResult.SaveSkipped;
        var ctx = GetMyDbContext() ??
            throw new InvalidOperationException("DbContext not found");
        var rt = ctx.Save();
        return rt;
    }

    public bool HasChanges(bool inthisdbset = true)
    {
        if (DataSource == null) return false;
        var ctx = GetMyDbContext() ?? throw new InvalidOperationException("DbContext not found");
        bool rt = ctx.HasChanges();
        if (rt || !inthisdbset) return rt;
        var tp = GetItemType() ?? throw new InvalidOperationException("ItemType not found");
        rt = ctx.HasChanges(tp);
        return rt;
    }

    private void Blv_ListItemPropertyChanged(object sender, MyItemPropertyChangedEventArgs e)
    {
        if (sender != List) return;
        ListItemPropertyChanged?.Invoke(this, e);
    }

    protected void HookBindingListView(IList list)
    {
        if (list is not IMyBindingListView blv) return;
        blv.ListItemPropertyChanged += Blv_ListItemPropertyChanged;
    }

    protected void UnHookBindingListView(IList list)
    {
        if (list is not IMyBindingListView blv) return;
        blv.ListItemPropertyChanged += Blv_ListItemPropertyChanged;
    }


    IList _currentList = null;

    protected override void OnListChanged(ListChangedEventArgs e)
    {
        if (e.ListChangedType == ListChangedType.Reset && List != _currentList)
        {
            if (_currentList != null)
            {
                UnHookBindingListView(_currentList);
                if (_currentList is IMyBindingListView blv)
                    blv.RemoveFilter();
            }
            _currentList = List;
            if (_currentList != null)
            {
                HookBindingListView(_currentList);
                InnerApplyFilter();
            }
        }

        if (!this.RaiseListChangedEvents || !(this as ISupportInitializeNotification).IsInitialized)
        {
            return;
        }
        //Debug.Print("MyBindingSource2.OnListChanged {0}: {1} {2}", this.Name2, e.NewIndex, e.ListChangedType);
        base.OnListChanged(e);
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

    public override void Remove(object value)
    {
        if (value == null) return;
        object item = value;
        ListItemRemoving?.Invoke(this, new MyItemRemovedEventArgs(item));
        base.Remove(value);
        ListItemRemoved?.Invoke(this, new MyItemRemovedEventArgs(item));
    }

    public override void RemoveAt(int index)
    {
        object item = ((IList)this)[index];
        ListItemRemoving?.Invoke(this, new MyItemRemovedEventArgs(item));
        base.RemoveAt(index);
        ListItemRemoved?.Invoke(this, new MyItemRemovedEventArgs(item));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_currentList != null)
            {
                UnHookBindingListView(_currentList);
                if (_currentList is IMyBindingListView blv)
                    blv.RemoveFilter();
                _currentList = null;
            }
        }
        base.Dispose(disposing);
    }

}
