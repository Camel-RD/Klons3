using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equin.ApplicationFramework;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace KlonsLIB.Data
{
    public class ObservableListSource<T> : ObservableCollection<T>, IListSource where T : class
    {
        private IBindingList _bindingList;
        bool IListSource.ContainsListCollection => false;

        IList IListSource.GetList()
        {
            return _bindingList ?? (_bindingList = new MyObservableBackedBindingList<T>(this));
        }

        public BindingList<T> GetBindingList()
        {
            return (this as IListSource).GetList() as BindingList<T>;
        }
    }
}
