using KlonsLIB.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KlonsLIB.BindingListView
{
    public  interface IMyBindingListView
    {
        void KillView();
        public IList SourceLists { get; set; }
        public IList NewItemsList { get; set; }
        public bool HookItemEvents { get; set; }
        public Type ItemType { get;}
        public void SuspendAutoFilterAndSort();
        public void ResumeAutoFilterAndSort();
        public void SetFilter(object ofilter);
        public void RemoveFilter();

        public event MyItemPropertyChangedventHandler ListItemPropertyChanged;
    }
}
