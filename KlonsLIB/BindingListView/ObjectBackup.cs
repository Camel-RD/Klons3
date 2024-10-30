using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Equin.ApplicationFramework
{
    public class ObjectBackup
    {
        Hashtable propstable = null;
        public ObjectBackup(object o) 
        {
            if (o == null) throw new ArgumentNullException("Object is null");
            //PropertyInfo[] properties = (o.GetType()).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var properties = TypeDescriptor.GetProperties(o);
            propstable = new();
            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                if (prop.IsReadOnly) continue;
                if (prop.PropertyType != typeof(string) && 
                    prop.PropertyType.IsAssignableTo(typeof(IEnumerable))) continue;
                IList<int> vg;
                try
                {
                    object value = prop.GetValue(o);
                    propstable.Add(prop.Name, value);
                }
                catch (Exception) { }
            }
        }

        public void Restore(object o)
        {
            if (o == null) throw new ArgumentNullException("Object is null");
            //PropertyInfo[] properties = (o.GetType()).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (propstable == null || propstable.Count == 0) return;
            var properties = TypeDescriptor.GetProperties(o);
            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                if (prop.IsReadOnly) continue;
                if (prop.PropertyType != typeof(string) &&
                    prop.PropertyType.IsAssignableTo(typeof(IEnumerable))) continue;
                object value = propstable[prop.Name];
                if (value == null) continue;
                try
                {
                    prop.SetValue(o, value);
                }
                catch (Exception) { }
            }
            propstable.Clear();
            propstable = null;
        }

        public bool HasChanges(object o)
        {
            if (o == null) throw new ArgumentNullException("Object is null");
            //PropertyInfo[] properties = (o.GetType()).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (propstable == null || propstable.Count == 0) return false;
            var properties = TypeDescriptor.GetProperties(o);
            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                if (prop.IsReadOnly) continue;
                if (prop.PropertyType != typeof(string) &&
                    prop.PropertyType.IsAssignableTo(typeof(IEnumerable))) continue;
                object old_value = propstable[prop.Name];
                try
                {
                    var current_value = prop.GetValue(o);
                    if(!object.Equals(old_value, current_value))
                        return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        public void Clear() => propstable.Clear();
    }

    public class ObjectBackupTable
    {
        Dictionary<object, ObjectBackup> DicBackups = new Dictionary<object, ObjectBackup>();
        List<object> PosList = new();
        public ObjectBackupTable() { }
        public void Backup(object o)
        {
            if (DicBackups.ContainsKey(o))
                DicBackups.Remove(o);
            if (DicBackups.Count > 5 && PosList.Count > 0)
            {
                var o1 = PosList[0];
                PosList.RemoveAt(0);
                DicBackups.Remove(o1);
            }
            DicBackups[o] = new ObjectBackup(o);
            PosList.Add(o);
        }

        public void Restore(object o)
        {
            if (!DicBackups.TryGetValue(o, out var obackup)) return;
            obackup.Restore(o);
            DicBackups.Remove(o);
            PosList.Remove(o);
        }

        public bool HasChanges(object o)
        {
            if (o == null) throw new ArgumentNullException("Object is null");
            if (!DicBackups.TryGetValue(o, out var obackup)) return false;
            return obackup.HasChanges(o);
        }

        public void Remove(object o)
        {
            DicBackups.Remove(o);
            PosList.Remove(o);
        }

        public void Clear()
        {
            foreach (var val in DicBackups.Values)
                val.Clear();
            DicBackups.Clear();
            PosList.Clear();
        }

    }
}
