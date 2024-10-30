using Equin.ApplicationFramework;
using KlonsLIB.BindingListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.Data
{
    public class ObservableListViewSource<T> : ObservableListSource<T>, IMyListViewSource where T : class
    {
        private BindingListView<T> _bindingListView;
        
        public BindingListView<T> GetBindingListView()
        {
            if (_bindingListView == null)
            {
                var bl = GetBindingList();
                _bindingListView = new BindingListView<T>(bl);
            }
            return _bindingListView;
        }

        IMyBindingListView IMyListViewSource.GetBindingListView()
        {
            return GetBindingListView();
        }
    }
}
