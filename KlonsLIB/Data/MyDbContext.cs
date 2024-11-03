using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KGySoft.ComponentModel;
using KlonsLIB.Forms;
using System.Data;
using System.Collections;
using EntityEntry = Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using FirebirdSql.Data.FirebirdClient;
using KlonsLIB.Misc;
using System.Collections.ObjectModel;

namespace KlonsLIB.Data;

public class MyDbContext : DbContext
{
    protected readonly string _connectionString;
    protected readonly string _userName;

    public MyDbContext(DbContextOptions options) : base(options)
    {
    }

    public MyDbContext(string connectionString, string username)
    {
        _connectionString = connectionString;
        _userName = username;
    }

    public static void OnConfiguring(DbContextOptionsBuilder optionsBuilder, string connectionstring, string username)
    {
        optionsBuilder.UseFirebird(connectionstring);
        optionsBuilder.LogTo(x => Debug.WriteLine(x), LogLevel.Information).EnableSensitiveDataLogging();
        var connection_interceptor = new KlonsDbConnectionInterceptor(username);
        optionsBuilder.AddInterceptors(connection_interceptor);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseFirebird(_connectionString);
        optionsBuilder.LogTo(x => Debug.WriteLine(x), LogLevel.Information).EnableSensitiveDataLogging();
        var connection_interceptor = new KlonsDbConnectionInterceptor(_userName);
        optionsBuilder.AddInterceptors(connection_interceptor);
    }

    public virtual void InitDbSetList()
    {
        DbSetLists = GetDbSetProperties()
            .ToDictionary(x => x.Name, x => GetBindingListFromDbSet(x.GetValue(this)));
    }

    public virtual void InitBindingListMembers()
    {
        if (DbSetLists.Count == 0)
            throw new InvalidOperationException("DbSetLists not initialized");
        var bl_propinfo = GetBindingListProperties();
        foreach (var blkv in DbSetLists)
        {
            var nm = "BL_" + blkv.Key;
            var prop = bl_propinfo.Find(x => x.Name == nm);
            if (prop == null) continue;
            prop.SetValue(this, blkv.Value);
        }
    }

    public Dictionary<string, IBindingList> DbSetLists { get; private set; } = new Dictionary<string, IBindingList>();

    public List<string> GetDbSetNames()
    {
        return GetDbSetProperties().Select(x => x.Name).ToList();
    }

    public object GetDbSetByName(string name)
    {
        if (name == null) return null;
        var prop = GetDbSetProperties().FirstOrDefault(x => x.Name == name);
        return prop?.GetValue(this);
    }

    public object GetDbSetByType(Type itemtype)
    {
        if (itemtype == null) return null;
        var prop = GetDbSetProperties().FirstOrDefault(x =>
            x.PropertyType.IsGenericType && x.PropertyType.GenericTypeArguments[0] == itemtype
        );
        return prop?.GetValue(this);
    }

    public DbSet<T> GetDbSet<T>() where T:class 
        => (DbSet<T>)GetDbSetByType(typeof(T));

    public static bool IsDbSet(object o)
    {
        return o != null && o.GetType().Name == "InternalDbSet`1";
    }

    public static IBindingList GetBindingListFromDbSet(object dbset)
    {
        if (!IsDbSet(dbset)) return null;
        var prop_local = dbset.GetType().GetProperty("Local");
        var obj_local = prop_local.GetValue(dbset);
        var meth_tobl = obj_local.GetType().GetMethod("ToBindingList");
        var obj_bl = meth_tobl.Invoke(obj_local, null);
        return obj_bl as IBindingList;
    }

    public IBindingList GetBindingListByName(string tablename)
    {
        if (tablename == null) return null;
        if (!DbSetLists.TryGetValue(tablename, out var ret)) return null;
        return ret;
    }

    public BindingList<T> GetBindingListByType<T>()
    {
        var tablename = typeof(T).Name;
        if (!DbSetLists.TryGetValue(tablename, out var ret)) return null;
        return ret as BindingList<T>;
    }

    public List<object> GetDbSets()
    {
        var ret = GetDbSetProperties()
            .Select(x => x.GetValue(this))
            .ToList();
        return ret;
    }

    public List<PropertyInfo> GetDbSetProperties()
    {
        return GetDbSetProperties(GetType());
    }
    public static DbSetList GetFakeList(Type ctx_type)
    {
        if (ctx_type == null) return null;
        var ret = new DbSetList();
        var props = GetDbSetProperties(ctx_type);
        var tp_blv = typeof(BindingList<>);
        foreach (var prop in props)
        {
            var item = new DbSetListItem();
            item.Name = prop.Name;
            item.ItemType = prop.PropertyType.GenericTypeArguments[0];
            object new_bl = null;
            bool rt = Reflector.TryCreateInstanceByType(
                tp_blv,
                prop.PropertyType.GenericTypeArguments,
                ReflectionWays.Auto,
                false,
                out new_bl);
            if (!rt) return null;
            item.BindingList = new_bl as IBindingList;
            ret.Add(prop.Name, item);
        }
        return ret;
    }

    public static List<PropertyInfo> GetDbSetProperties(Type ctx_type)
    {
        return ctx_type.GetProperties()
            .Where(x => x.PropertyType.IsGenericType &&
                typeof(DbSet<>).IsAssignableFrom(x.PropertyType.GetGenericTypeDefinition()))
            .ToList();
    }

    public List<PropertyInfo> GetBindingListProperties()
    {
        return GetType().GetProperties()
            .Where(x => x.PropertyType.IsGenericType &&
                typeof(BindingList<>).IsAssignableFrom(x.PropertyType.GetGenericTypeDefinition()))
            .ToList();
    }

    public void SetDbUserName(FbConnection connection, string username)
    {
        using (var cm = new FbCommand())
        {
            cm.Connection = connection;
            cm.CommandText = "\"SP_SET_USERNAME\"";
            cm.CommandType = CommandType.StoredProcedure;
            var param = new FbParameter();
            param.ParameterName = "USERNAME";
            param.DbType = DbType.String;
            param.Size = 15;
            param.IsNullable = false;
            cm.Parameters.Add(param);
            cm.Parameters[0].Value = username;
            var ret = cm.ExecuteNonQuery();
        }
    }

    public class DbSetList : Dictionary<string, DbSetListItem> { }

    public class DbSetListItem
    {
        public string Name;
        public Type ItemType;
        public IBindingList BindingList;
    }

    public virtual string FormatEntriesList(IList<EntityEntry> list)
    {
        var sb = new StringBuilder();
        bool isfirstitem = true;
        foreach (var item in list)
        {
            if (isfirstitem) isfirstitem = false;
            else sb.Append("\n\n");
            var s = (item.Entity as EntityBase).GetInfoView();
            sb.Append(s);
            sb.Append("\n");
            var state = item.State switch
            {
                EntityState.Added => ":jauns ieraksts",
                EntityState.Deleted => ":dzēsts ierakst",
                EntityState.Modified => ":labots ierakst",
                EntityState.Unchanged => ":nemainīts ieraksts",
                _=> ":?"
            };
            sb.Append(state);
        }
        return sb.ToString();
    }

    public virtual string FormatSaveException(Exception ex)
    {
        var sb = new StringBuilder();
        List<EntityEntry> bad_entries = null;
        if (ex is DbUpdateConcurrencyException exc)
        {
            sb.AppendLine("[Datu bāzes konkurences kļūda]");
            bad_entries = exc.Entries.Take(50).ToList();
        }
        else if (ex is DbUpdateException exu)
        {
            sb.AppendLine("[Datu saglabāšanas kļūda]");
            bad_entries = exu.Entries.Take(50).ToList();
        }
        else
        {
            sb.AppendLine("[Datu saglabāšanas kļūda]");
        }
        var fex = ex;
        int k = 1;
        while (fex != null)
        {
            sb.Append($"[{k++}]: ");
            sb.AppendLine(fex.Message);
            sb.AppendLine();
            fex = fex.InnerException;
        }
        if (bad_entries != null && bad_entries.Count > 0)
        {
            var formatted_errors = FormatEntriesList(bad_entries.ToList());
            sb.AppendLine("\n[KĻŪDAINIE IERAKSTI]\n");
            sb.AppendLine(formatted_errors);
        }
        var changed_entries = GetChangedEntries()
            .Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached)
            .ToList();
        if (changed_entries.Count > 0)
        {
            var formatted_changes = FormatEntriesList(changed_entries);
            sb.AppendLine("\n[LABOTIE IERAKSTI]\n");
            sb.AppendLine(formatted_changes);
        }
        return sb.ToString();
    }

    public string LastExceptionFormatted { get; protected set; } = null;

    public virtual EBsSaveResult Save(bool canrollback = true)
    {
        if (!HasChanges()) 
            return EBsSaveResult.SaveSkipped;
        try
        {
            SaveChanges();
        }
        catch (Exception ex)
        {
            var formatted_exception = FormatSaveException(ex);
            LastExceptionFormatted = formatted_exception;
            var myex = new MyException("Neizdevās saglabāt izmaiņas.", ex);
            myex.ExtraData = formatted_exception;
            var has_deleted_entries = HasDeletedEntities();
            Form_Error2.EPromptState promptstate = Form_Error2.EPromptState.None;
            if (has_deleted_entries)
                promptstate = Form_Error2.EPromptState.WillRollBack;
            else if (canrollback)
                promptstate = Form_Error2.EPromptState.CanRollBack;
            var rt = Form_Error2.ShowException(MyMainFormBase.MyInstance, myex, promptstate);
            if (has_deleted_entries || rt == Form_Error2.EFormErrorResult.DoRollBack)
            {
                var rt2 = RollbackChanges();
                return rt2 ? EBsSaveResult.RolledBack : EBsSaveResult.RollBackFailed;
            }
            return EBsSaveResult.Error;
        }
        return EBsSaveResult.Saved;
    }

    public virtual bool DeleteItem(object item)
    {
        try
        {
            var entry = Entry(item);
            if (entry == null)
                throw new InvalidOperationException("Entry not found");
            Remove(entry);
        }
        catch (Exception ex)
        {
            var myex = new MyException("Neizdevās dzēst ierakstu.", ex);
            Form_Error.ShowException(MyMainFormBase.MyInstance, myex);
            return false;
        }
        return true;
    }

    public bool HasChanges()
    {
        return ChangeTracker.HasChanges();
    }

    public bool HasDeletedEntities()
    {
        bool ret = GetEntriesForState(deleted: true).Any();
        return ret;
    }

    public class ItemPropertyRestrictions
    {
        public static ItemPropertyRestrictions Default { get; } = new ItemPropertyRestrictions();
        public string PropertyName;
        public bool AllowNull = true;
        public bool IsKey = false;
        public int MaxLength = 0;
    }

    Dictionary<string, ItemRestrictions> ItemPropertiesRestrictions = [];

    public class ItemRestrictions : Dictionary<string, ItemPropertyRestrictions> 
    {
        public static readonly ItemRestrictions Default = [];
    }

    public ItemRestrictions GetItemPropertiesRestrictions(Type itemtype)
    {
        if (ItemPropertiesRestrictions.TryGetValue(itemtype.Name, out var ret)) 
            return ret;
        ret = new ItemRestrictions();
        var entitytype = this.Model.FindEntityType(itemtype);
        if (entitytype == null) return null;
        
        var props = entitytype.GetDeclaredProperties().ToList();
        
        var navis = entitytype.GetNavigations()
            .Where(x => 
                !x.IsCollection && 
                x.ForeignKey != null &&
                x.ForeignKey.Properties.Count == 1)
            .Select(x => (name: x.Name, baseprop: x.ForeignKey.Properties[0]))
            .ToList();

        foreach (var prop in props)
        {
            var restr = new ItemPropertyRestrictions()
            {
                PropertyName = prop.Name,
                AllowNull = prop.IsNullable,
                IsKey = prop.IsKey(),
                MaxLength = prop.GetMaxLength() ?? 0
            };
            ret[restr.PropertyName] = restr;
        }
        foreach (var navi in navis)
        {
            var restr = new ItemPropertyRestrictions()
            {
                PropertyName = navi.name,
                AllowNull = navi.baseprop.IsNullable,
                IsKey = navi.baseprop.IsKey(),
                MaxLength = 0
            };
            ret[restr.PropertyName] = restr;
        }
        ItemPropertiesRestrictions[itemtype.Name] = ret;
        return ret;
    }

    public ItemPropertyRestrictions GetItemPropertyRestrictions(Type itemtype, string propname)
    {
        var restrictions = GetItemPropertiesRestrictions(itemtype);
        
        if (restrictions == null)
            return ItemPropertyRestrictions.Default;

        if (restrictions.TryGetValue(propname, out var restriction))
            return restriction;

        return ItemPropertyRestrictions.Default;
    }

    public void RejectChanges()
    {
        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                case EntityState.Deleted:
                    entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
            }
        }
    }

    public bool RollbackChanges()
    {
        try
        {
            var rt = RollbackChangesA();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    protected bool RollbackChangesA()
    {
        var oldBehavoir = ChangeTracker.QueryTrackingBehavior;
        var oldAutoDetect = ChangeTracker.AutoDetectChangesEnabled;

        // this is the key - disable change tracking logic so EF does not check that there were exception in on of tracked entities
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;

        var entries = ChangeTracker.Entries().ToList();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Reload();
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified; //Revert changes made to deleted entity.
                    entry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
            }
        }

        ChangeTracker.QueryTrackingBehavior = oldBehavoir;
        ChangeTracker.AutoDetectChangesEnabled = oldAutoDetect;
        
        return true;
    }

    public void RemoveFromContext(object item)
    {
        var entry = Entry(item);
        Remove(item);
        entry.State = EntityState.Detached;
    }

    public void RemoveFromContextRange(IEnumerable items)
    {
        foreach (var item in items)
        {
            RemoveFromContext(item);
        }
    }

    public void LoadTable(IQueryable table)
    {
        foreach (var item in table) { }
    }
    
    public void CopyEntry(object item_from, object item_to, bool checktypematch = true, bool skipnavigations = false)
    {
        if (item_from == null)
            throw new ArgumentNullException(nameof(item_from));
        if (item_to == null)
            throw new ArgumentNullException(nameof(item_to));
        if (checktypematch && item_to.GetType() != item_from.GetType())
            throw new ArgumentException("Types must match");
        var entity_modeltype = Model.FindEntityType(item_to.GetType()) ??
            throw new InvalidOperationException("DbContext entity not found");

        var pds_from = TypeDescriptor.GetProperties(item_from);
        var tp_to = item_to.GetType();
        for (int i = 0; i < pds_from.Count; i++)
        {
            var pd_from = pds_from[i];
            var pd_to = tp_to.GetProperty(pd_from.Name);
            if (pd_to == null) continue;
            var prop_setter = pd_to.GetSetMethod();
            if (prop_setter == null) continue;
            var entity_prop = entity_modeltype.FindProperty(pd_from.Name);
            if (entity_prop != null)
            {
                if (entity_prop.IsPrimaryKey()) continue;
                if (entity_prop.ValueGenerated != Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never) continue;
                var value_from = pd_from.GetValue(item_from);
                if (value_from != null && value_from is ICollection) continue;
                pd_to.SetValue(item_to, value_from);
            }
            else if (!skipnavigations)
            {
                var entity_navprop = entity_modeltype.FindNavigation(pd_from.Name);
                if (entity_navprop != null)
                {
                    if (entity_navprop.IsCollection) continue;
                    var value_from = pd_from.GetValue(item_from);
                    if (value_from != null && value_from is IList) continue;
                    pd_to.SetValue(item_to, value_from);
                }
            }
        }
    }

    public bool ListHasChanges(IEnumerable items)
    {
        if (!HasChanges()) return false;
        var changes = GetChangedEntries().ToList();
        foreach (var item in items)
        {
            if (changes.Contains(item))
                return true;
        }
        return false;
    }

    public IStateManager GetStateManager() => this.GetService<IStateManager>();

    public bool IsItemDeleted(object items)
    {
        if (!HasChanges()) return false;
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(deleted: true)
            .Where(e => e.Entity == items)
            .Any();
    }

    public (int added, int deleted, int modified) GetStats()
    {
        var state_manager = this.GetService<IStateManager>();
        int added = state_manager.GetCountForState(added: true);
        int deleted = state_manager.GetCountForState(deleted: true);
        int modified = state_manager.GetCountForState(modified: true);
        return (added, deleted, modified);
    }

    public IEnumerable<EntityEntry> GetEntriesForState(
        bool added = false, bool modified = false, bool deleted = false)
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: added, modified: modified, deleted: deleted)
            .Select(e => new EntityEntry(e));
    }

    public IEnumerable<EntityEntry<TEntity>> GetEntriesForState<TEntity>(
        bool added = false, bool modified = false, bool deleted = false) where TEntity : class
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: added, modified: modified, deleted: deleted)
            .Where(e => e.Entity is TEntity)
            .Select(e => new EntityEntry<TEntity>(e));
    }

    public IEnumerable<EntityEntry<TEntity>> GetChangedEntries<TEntity>() where TEntity : class
    {
        return GetEntriesForState<TEntity>(added: true, modified: true, deleted: true);
    }

    public IEnumerable<EntityEntry> GetChangedEntries(Type entitytype)
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: true, modified: true, deleted: true)
            .Where(e => e.EntityType.ClrType == entitytype)
            .Select(e => new EntityEntry(e));
    }

    public IEnumerable<EntityEntry> GetChangedEntries()
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: true, modified: true, deleted: true)
            .Select(e => new EntityEntry(e));
    }

    public bool HasChanges<TEntity>()
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: true, modified: true, deleted: true)
            .Where(e => e.Entity is TEntity)
            .Any();
    }

    public bool HasChanges(Type entitytype)
    {
        var state_manager = this.GetService<IStateManager>();
        return state_manager.GetEntriesForState(added: true, modified: true, deleted: true)
            .Where(e => e.EntityType.ClrType == entitytype)
            .Any();
    }

    public (int added, int deleted, int modified) GetStats(Type entitytype)
    {
        int added = 0, deleted = 0, modified = 0;
        foreach (var entry in GetChangedEntries(entitytype))
        {
            if (entry.Entity.GetType() != entitytype) continue;
            switch (entry.State)
            {
                case EntityState.Added:
                    added++;
                    break;
                case EntityState.Deleted:
                    deleted++;
                    break;
                case EntityState.Modified:
                    modified++;
                    break;
            }
        }
        return (added, deleted, modified);
    }

    /// <summary>
    /// Fast DbSet clear, because State = Detached for large entity count is slow
    /// Context should be saved before
    /// DbSet shouldn be referenced by other DbSets with navigations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public void ClearLocalDbSet<T>() where T:class
    {
        var state_manager = GetStateManager();

        ClearIdentityMap();
        ClearReferenceMap();
        ClearLocalView();

        void ClearIdentityMap()
        {
            var fi_identitymaps = Utils.GetField(state_manager.GetType(), "_identityMaps");
            var identitymaps = (Dictionary<IKey, IIdentityMap>)fi_identitymaps.GetValue(state_manager);
            var entoty_type = Model.FindEntityType(typeof(T));
            var key = entoty_type.GetKeys().Single();
            bool rt = identitymaps.TryGetValue(key, out var identitymap);
            if (!rt) return;
            identitymap.Clear();
        }

        void ClearReferenceMap()
        {
            var fi_referencemap = Utils.GetField(state_manager.GetType(), "_entityReferenceMap");
            var referencemap = (EntityReferenceMap)fi_referencemap.GetValue(state_manager);
            if (referencemap == null) return;
            var fi_unchangedreferencemap = Utils.GetField(referencemap.GetType(), "_unchangedReferenceMap");
            var unchangedreferencemap = (Dictionary<object, InternalEntityEntry>)fi_unchangedreferencemap.GetValue(referencemap);
            if (unchangedreferencemap == null || unchangedreferencemap.Count == 0) return;
            var entity_type = Model.FindEntityType(typeof(T));
            var kv1 = unchangedreferencemap.Where(x => x.Value.EntityType != entity_type);
            var new_unchangedreferencemap = new Dictionary<object, InternalEntityEntry>(kv1);
            fi_unchangedreferencemap.SetValue(referencemap, new_unchangedreferencemap);
            unchangedreferencemap.Clear();
        }

        void ClearLocalView()
        {
            var dbset = (DbSet<T>)GetDbSetByType(typeof(T));
            if (dbset == null) return;
            var localview = dbset.Local;
            var tp_localview = localview.GetType();
            var fi_observable = Utils.GetField(tp_localview, "_observable");
            var _observable = (ObservableCollection<T>)fi_observable.GetValue(localview);
            if (_observable != null && _observable.Count > 0)
            {
                var fi_items = Utils.GetField(_observable.GetType(), "items");
                var items = (ICollection<T>)fi_items.GetValue(_observable);
                items.Clear();
            }
            var fi_bindingList = Utils.GetField(tp_localview, "_bindingList");
            var _bindingList = (ObservableBackedBindingList<T>)fi_bindingList.GetValue(localview);
            if (_bindingList != null && _bindingList.Count > 0)
            {
                var fi_items = Utils.GetField(_bindingList.GetType(), "items");
                var items = (ICollection<T>)fi_items.GetValue(_bindingList);
                items.Clear();
            }
            var fi_countchanges = Utils.GetField(tp_localview, "_countChanges");
            fi_countchanges.SetValue(localview, 0);
            var fi_count = Utils.GetField(tp_localview, "_count");
            fi_count.SetValue(localview, (int?)0);
        }
    }

    /*void dsfasf<T>(Expression<Func<T, object?>> foreignKeyExpression)
    {
        dsfasf<Form>(x => x.Icon);
        foreignKeyExpression.GetMemberAccessList()
    }*/

    public void UpdateEntryLists<T>(IList<T> list_from, IList<T> list_to,
        Func<T, int> keyselector)
    {
        var dic_to = list_to.ToDictionary(x => keyselector(x));
        var items_to_update = list_from.Where(x => dic_to.ContainsKey(keyselector(x))).ToList();
        foreach (var item_from in items_to_update)
        {
            var item_to = dic_to[keyselector(item_from)];
            CopyEntry(item_from, item_to, false, true);
            var entry_to = Entry(item_to).GetInfrastructure();
            entry_to.SetEntityState(EntityState.Unchanged, true);
        }
    }

    public void UpdateEntryLists<T>(IList<T> list_from, IList<T> list_to,
        Func<T, (int, int)> keyselector)
    {
        var dic_to = list_to.ToDictionary(x => keyselector(x));
        var items_to_update = list_from.Where(x => dic_to.ContainsKey(keyselector(x))).ToList();
        foreach (var item_from in items_to_update)
        {
            var item_to = dic_to[keyselector(item_from)];
            CopyEntry(item_from, item_to, false, true);
            var entry_to = Entry(item_to).GetInfrastructure();
            entry_to.SetEntityState(EntityState.Unchanged, true);
        }
    }

    public void MergeEntryLists<T>(IList<T> list_from, IList<T> list_to, 
        Func<T, int> keyselector)
    {
        var dic_from = list_from.ToDictionary(x => keyselector(x));
        var dic_to = list_to.ToDictionary(x => keyselector(x));
        var items_to_add = list_from.Where(x => !dic_to.ContainsKey(keyselector(x))).ToList();
        var items_to_remove = list_to.Where(x => !dic_from.ContainsKey(keyselector(x))).ToList();
        var items_to_update = list_from.Where(x => dic_to.ContainsKey(keyselector(x))).ToList();
        foreach (var item_from in items_to_update)
        {
            var item_to = dic_to[keyselector(item_from)];
            CopyEntry(item_from, item_to, false, true);
            var entry_to = Entry(item_to).GetInfrastructure();
            entry_to.SetEntityState(EntityState.Unchanged, true);
        }
        foreach (var item in items_to_add)
        {
            Attach(item);
        }
        foreach (var item in items_to_remove)
        {
            var entry = Entry(item);
            Remove(item);
            entry.State = EntityState.Detached;
        }
    }

    public void MergeEntryLists<T>(IList<T> list_from, IList<T> list_to,
        Func<T, (int, int)> keyselector)
    {
        var dic_from = list_from.ToDictionary(x => keyselector(x));
        var dic_to = list_to.ToDictionary(x => keyselector(x));
        var items_to_add = list_from.Where(x => !dic_to.ContainsKey(keyselector(x))).ToList();
        var items_to_remove = list_to.Where(x => !dic_from.ContainsKey(keyselector(x))).ToList();
        var items_to_update = list_from.Where(x => dic_to.ContainsKey(keyselector(x))).ToList();
        foreach (var item_from in items_to_update)
        {
            var item_to = dic_to[keyselector(item_from)];
            CopyEntry(item_from, item_to, false, true);
            var entry_to = Entry(item_to).GetInfrastructure();
            entry_to.SetEntityState(EntityState.Unchanged, true);
        }
        foreach (var item in items_to_add)
        {
            Attach(item);
        }
        foreach (var item in items_to_remove)
        {
            var entry = Entry(item);
            Remove(item);
            entry.State = EntityState.Detached;
        }
        
    }



    public interface IMergeTask
    {
        Type GetItemType();
        void AddNewItems();
        void RemoveItems();
        void UpdateItems();
    }

    public class MergeTask<T> : IMergeTask where T : class
    {
        Func<T, object> KeySelector = null;
        MyDbContext Ctx = null;
        List<T> ItemsCurrent = null;
        List<T> ItemsNew = null;
        List<T> ItemsToAdd = null;
        List<T> ItemsToRemoce = null;
        List<T> ItemsToUpdate = null;
        Dictionary<object, T> DicCurrent = null;
        Dictionary<object, T> DicNew = null;
        public MergeTask(MyDbContext ctx, Func<T, object> keyselector)
        {
            Ctx = ctx;
            KeySelector = keyselector;
            var dbset = Ctx.GetDbSet<T>();
            ItemsCurrent = dbset.Local.ToList();
            ItemsNew = dbset.AsNoTracking().ToList();
            DicCurrent = ItemsCurrent.ToDictionary(x => KeySelector(x));
            DicNew = ItemsNew.ToDictionary(x => KeySelector(x));
            ItemsToAdd = ItemsNew.Where(x => !DicCurrent.ContainsKey(keyselector(x))).ToList();
            ItemsToRemoce = ItemsCurrent.Where(x => !DicNew.ContainsKey(keyselector(x))).ToList();
            ItemsToUpdate = ItemsNew.Where(x => DicCurrent.ContainsKey(keyselector(x))).ToList();
        }

        public void Merge()
        {
            AddNewItems();
            UpdateItems();
            RemoveItems();
        }

        public Type GetItemType() => typeof(T);

        public void AddNewItems()
        {
            Ctx.AttachRange(ItemsToAdd);
        }

        public void RemoveItems()
        {
            foreach (var item in ItemsToRemoce)
            {
                var entry = Ctx.Entry(item);
                Ctx.Remove(item);
                entry.State = EntityState.Unchanged;
            }
        }

        public void UpdateItems()
        {
            foreach (var item_from in ItemsToUpdate)
            {
                var item_to = DicCurrent[KeySelector(item_from)];
                Ctx.CopyEntry(item_from, item_to, false, true);
                var entry_to = Ctx.Entry(item_to).GetInfrastructure();
                entry_to.SetEntityState(EntityState.Unchanged, true);
            }
        }
    }

    //stores, items, items_ps, docs, rows
    public class MergeTaskList
    {
        List<IMergeTask> MergeTasks = new List<IMergeTask>();
        MyDbContext Ctx = null;
        public MergeTaskList(MyDbContext ctx)
        {
            Ctx = ctx;
        }

        public void AddTask<T>(Func<T, object> keyselector) where T:class
        {
            var mergetask = new MergeTask<T>(Ctx, keyselector);
            MergeTasks.Add(mergetask);
        }

        public string ValidateList()
        {
            if (MergeTasks.Count == 0) return "Ok";
            var tp_list = MergeTasks.Select(x => x.GetItemType()).ToList();
            var etp_list = new List<IEntityType>();
            for (int i = 1; i < MergeTasks.Count; i++)
            {
                var tp = tp_list[i];
                var etp = Ctx.Model.FindEntityType(tp);
                if (etp == null)
                    return $"EntityType {tp.Name} not found";
                etp_list.Add(etp);
            }
            for (int i = 1; i < MergeTasks.Count; i++)
            {
                var tp1 = tp_list[i];
                var etp1 = etp_list[i];
                for (int j = 0; j < i; j++)
                {
                    var tp2 = tp_list[j];
                    var etp2 = etp_list[j];
                    var pkey = etp2.FindPrimaryKey();
                    var fks = pkey.GetReferencingForeignKeys().ToList();
                    foreach(var fk in fks)
                    {
                        if (fk.DeclaringEntityType == etp1)
                            return $"{tp1.Name} depends on {tp2.Name}";
                    }
                }
            }
            return "Ok";
        }

        public void Merge()
        {
            AddNewItems();
            UpdateItems();
            RemoveItems();
        }

        public void AddNewItems()
        {
            for (int i = 0; i < MergeTasks.Count; i++)
            {
                MergeTasks[i].AddNewItems();
            }
        }

        public void RemoveItems()
        {
            for (int i = MergeTasks.Count - 1; i >= 0; i--)
            {
                MergeTasks[i].RemoveItems();
            }
        }

        public void UpdateItems()
        {
            for (int i = 0; i < MergeTasks.Count; i++)
            {
                MergeTasks[i].UpdateItems();
            }
        }

    }


}

