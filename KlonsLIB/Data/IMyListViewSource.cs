using KlonsLIB.BindingListView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.Data
{
    public interface IMyListViewSource
    {
        IMyBindingListView GetBindingListView();
    }
}
