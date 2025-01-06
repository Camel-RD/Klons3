﻿using KlonsLIB.Data;
using KlonsLIB.Misc;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace KlonsLIB.Components
{
    public class MyPickRowTextBox : TextBoxWithButton
    {

        private int selectedIndex = -1;
        private object dataSource = null;
        private string dataMember = null;
        private bool syncPosition = true;

        private ListChangedEventHandler listChangedHandler;
        private EventHandler positionChangedHandler;


        public MyPickRowTextBox()
        {
            listChangedHandler = new ListChangedEventHandler(dataManager_ListChanged);
            positionChangedHandler = new EventHandler(dataManager_PositionChanged);            
        }

        //[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        [Category("Data")]
        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }
            set
            {
                if (this.dataSource != value)
                {
                    this.dataSource = value;
                    tryDataBinding();
                }
            }
        }


        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DefaultValue("")]
        public string DataMember
        {
            get { return this.dataMember; }
            set
            {
                if (this.dataMember != value)
                {
                    this.dataMember = value;
                    tryDataBinding();
                }
            }
        }

        [Category("Data")]
        [DefaultValue(null)]
        public string DataPropertyName { get; set; } = null;


        [DefaultValue(true)]
        [Category("Data")]
        public bool SyncPosition
        {
            get { return syncPosition; }
            set { syncPosition = value; }
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            this.tryDataBinding();
            base.OnBindingContextChanged(e);
        }

        private bool IsInPositionChanged = false;
        private bool isInSetSelectedIndex = false;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex == value) return;
                selectedIndex = value;

                if (selectedIndex < 0) selectedIndex = -1;
                if (selectedIndex >= dataManager.Count)
                    selectedIndex = dataManager.Count - 1;

                if (dataManager == null || isInSetSelectedIndex || IsInPositionChanged) return;

                isInSetSelectedIndex = true;

                if (SyncPosition)
                {
                    int k = selectedIndex;
                    if (k == -1) k = 0;
                    try
                    {
                        dataManager.Position = k;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (selectedIndex == -1 && Text != null)
                {
                    // we are skiping this to allow editing of wrong text
                    //Text = null;
                }
                if (selectedIndex > -1)
                {
                    var s = GetText(selectedIndex);
                    if (Text != s) Text = s;
                }

                isInSetSelectedIndex = false;
            }
        }

        private void dataManager_PositionChanged(object sender, EventArgs e)
        {
            if (!SyncPosition) return;
            if (IsInPositionChanged || isInSetSelectedIndex) return;
            IsInPositionChanged = true;
            try
            {
                Text = GetText(dataManager.Position);
                if (SelectedIndex != dataManager.Position)
                    SelectedIndex = dataManager.Position;
            }
            finally
            {
                IsInPositionChanged = false;
            }
        }

        private CurrencyManager dataManager;

        private void tryDataBinding()
        {
            if (this.DataSource == null ||
                base.BindingContext == null)
                return;

            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)
                      base.BindingContext[this.DataSource,
                                          this.DataMember];
            }
            catch (System.ArgumentException)
            {
                // If no CurrencyManager was found
                return;
            }

            if (this.dataManager != cm)
            {
                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged -= listChangedHandler;
                    this.dataManager.PositionChanged -= positionChangedHandler;
                }

                this.dataManager = cm;

                if (this.dataManager != null)
                {
                    this.dataManager.ListChanged += listChangedHandler;
                    this.dataManager.PositionChanged += positionChangedHandler;
                }
            }
        }

        private void dataManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                Text = "";
                selectedIndex = -1;
            }
        }

        protected virtual bool IsDataSourceSortedByDisplayMember()
        {
            if (!((dataSource is BindingSource bs) && bs.IsSorted)) return false;
            if (bs.SortDescriptions.Count == 0) return false;
            if (bs.SortDescriptions[0].PropertyDescriptor.Name != DataPropertyName) return false;
            if (bs.SortDescriptions[0].SortDirection != ListSortDirection.Ascending) return false;
            return true;
        }

        private int FindString(string s)
        {
            if (dataManager == null || s == null) return -1;
            int k = dataManager.List.Count;
            if (string.Compare(s, GetText(k - 1), true) == 0)
                return k - 1;
            if (IsDataSourceSortedByDisplayMember())
                return FindStringSorted(s);
            return FindStringSimple(s);
        }

        private int FindStringExact(string s)
        {
            if (dataManager == null || s == null) return -1;
            int k = dataManager.List.Count;
            if (string.Compare(s, GetText(k - 1), true) == 0)
                return k - 1;
            if (dataSource is BindingSource && (dataSource as BindingSource).IsSorted)
                return FindStringExactSorted(s);
            return FindStringExactSimple(s);
        }

        private int FindStringSimple(string s)
        {

            if (dataManager == null) return -1;
            string s1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                s1 = GetText(i);
                if (s.Length > s1.Length) continue;
                if (MyCompare(s, s1.Substring(0, s.Length)) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindStringExactSimple(string s)
        {
            if (dataManager == null) return - 1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                if (string.Compare(s, GetText(i), true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        int MyCompare(string s1, string s2)
        {
            int ret = string.Compare(s1, s2, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase);
            return ret;
        }

        private int FindStringSorted(string s)
        {
            if (dataManager == null) return -1;
            if (string.IsNullOrEmpty(s)) return -1;
            int k1 = 0;
            int k2 = dataManager.List.Count - 1;
            int khi = k2;
            if (k2 == -1) return -1;
            string s1;
            int m;
            while (k1 <= k2)
            {
                int k3 = (k1 + k2) / 2;
                s1 = GetText(k3);
                m = MyCompare(s, s1);
                if (m == 0) return k3;
                if (m < 0)
                {
                    k2 = k3 - 1;
                }
                else
                {
                    k1 = k3 + 1;
                }
            }
            if (k1 > khi) return -1;
            s1 = GetText(k1);
            if (s1.Length < s.Length) return -1;
            s1 = s1.Substring(0, s.Length);
            m = MyCompare(s, s1);
            if (m == 0) return k1;
            return -1;
        }

        private int FindStringExactSorted(string s)
        {
            if (dataManager == null) return -1;
            if (string.IsNullOrEmpty(s)) return -1;
            int k1 = 0;
            int k2 = dataManager.List.Count - 1;
            int khi = k2;
            if (k2 == -1) return -1;
            string s1;
            while (k1 <= k2)
            {
                int k3 = (k1 + k2) / 2;
                s1 = GetText(k3);
                int m = MyCompare(s, s1);
                if (m == 0) return k3;
                if (m < 0)
                {
                    k2 = k3 - 1;
                }
                else
                {
                    k1 = k3 + 1;
                }
            }
            return -1;
        }

        private string GetText(int nr)
        {
            try
            {
                string s = GetTextA(nr);
                return s ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private PropertyDescriptor dataPropertyDescriptor = null;
        protected PropertyDescriptor GetDataPropertyDescriptor()
        {
            if (DataPropertyName.IsNOE()) return null;
            if (dataPropertyDescriptor == null)
            {
                var pds = ListBindingHelper.GetListItemProperties(DataSource);
                if (pds == null) return null;
                dataPropertyDescriptor = pds[DataPropertyName];
            }
            return dataPropertyDescriptor;
        }


        private string GetTextA(int nr)
        {
            if (dataManager == null || DataPropertyName.IsNOE() ||
                nr < 0 || nr >= dataManager.List.Count) return null;
            var item = dataManager.List[nr];
            if (item == null) return null;
            var pd = GetDataPropertyDescriptor();
            if (pd == null) return null;
            var value = pd.GetValue(item);
            return value?.ToString();
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Text = "";
                OnRowSelectedEvent(-1, null);
                return;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Return)
            {
                int k = FindStringExact(Text);
                if (k > -1)
                {
                    OnRowSelectedEvent(k, Text);
                    e.Handled = true;
                    return;
                }
            }
            base.OnKeyPress(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                doOnTextChangedDelBackKey = true;

            }

            int k = 0;
            if (e.KeyCode == Keys.Up) k = -1;
            if (e.KeyCode == Keys.Down) k = 1;
            if (dataManager != null && k != 0)
            {
                if (SyncPosition)
                    dataManager.Position = dataManager.Position + k;
                else
                    SelectedIndex = SelectedIndex + k;
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                doOnTextChangedDelBackKey = false;

            }
            base.OnKeyUp(e);
        }

        private bool isInOnTextChanged = false;
        private bool doOnTextChangedDelBackKey = false;

        protected override void OnTextChanged(EventArgs e)
        {
            if (IsInPositionChanged || isInSetSelectedIndex || isInOnTextChanged)
            {
                base.OnTextChanged(e);
                return;
            }

            isInOnTextChanged = true;

            string sTypedText = Text;
            int iFoundIndex;

            if (doOnTextChangedDelBackKey)
            {
                doOnTextChangedDelBackKey = false;
                iFoundIndex = FindStringExact(sTypedText);
            }
            else
            {
                iFoundIndex = FindString(sTypedText);
            }
            if (iFoundIndex >= 0 && Text != "")
            {
                string sFoundText = GetText(iFoundIndex);
                string sAppendText = sFoundText.Substring(sTypedText.Length);
                AppendText(sAppendText);
                SelectedIndex = iFoundIndex;
                SelectionStart = sTypedText.Length;
                SelectionLength = sAppendText.Length;
            }
            else
            {
                SelectedIndex = -1;
            }

            isInOnTextChanged = false;
         
            base.OnTextChanged(e);
        }

        public event RowSelectedEventHandler RowSelectedEvent;
        protected void OnRowSelectedEvent(int rownr, string value)
        {
            if (RowSelectedEvent != null)
                RowSelectedEvent(this, new RowSelectedEventArgs(rownr, value));
        }
    }

    public delegate void RowSelectedEventHandler(object sender, RowSelectedEventArgs e);

    public class RowSelectedEventArgs : EventArgs
    {
        public int RowNr { get; private set; }
        public string Value { get; private set; }

        public RowSelectedEventArgs(int rownr, string value)
        {
            RowNr = rownr;
            Value = value;
        }
    }

}
