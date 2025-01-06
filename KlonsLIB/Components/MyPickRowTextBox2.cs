using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using KlonsLIB.Data;
using KlonsLIB.Misc;

namespace KlonsLIB.Components
{
    public class MyPickRowTextBox2 : TextBoxWithButton
    {

        private int selectedIndex = -1;
        private object dataSource = null;
        private string dataMember = null;
        private string valueMember = "";
        private string displayMember = "";
        private bool syncPosition = true;
        private PropertyDescriptor valuePropertyDescriptor = null;
        private PropertyDescriptor displayPropertyDescriptor = null;

        private ListChangedEventHandler listChangedHandler;
        private EventHandler positionChangedHandler;


        public MyPickRowTextBox2()
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
                return dataSource;
            }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    tryDataBinding();
                }
            }
        }


        [Category("Data")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DefaultValue("")]
        public string DataMember
        {
            get { return dataMember; }
            set
            {
                if (dataMember != value)
                {
                    dataMember = value;
                    tryDataBinding();
                }
            }
        }

        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Category("Data")]
        public string ValueMember
        {
            get { return valueMember; }
            set
            {
                valueMember = value;
                valuePropertyDescriptor = null;
            }
        }

        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Category("Data")]
        public string DisplayMember 
        { 
            get { return displayMember; }
            set
            {
                displayMember = value;
                displayPropertyDescriptor = null;
            }
        }

        [DefaultValue(true)]
        [Category("Data")]
        public bool SyncPosition
        {
            get { return syncPosition; }
            set { syncPosition = value; }
        }

        [DefaultValue(true)]
        [Category("Data")]
        public bool LimitToList { get; set; } = true;

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
                    Text = null;
                }
                if (selectedIndex > -1)
                {
                    var s = GetDisplayText(selectedIndex);
                    if (Text != s) Text = s;
                }

                isInSetSelectedIndex = false;

                OnSelectedIndexChanged();
            }
        }

        private void dataManager_PositionChanged(object sender, EventArgs e)
        {
            if (!SyncPosition) return;
            if (IsInPositionChanged || isInSetSelectedIndex) return;
            IsInPositionChanged = true;
            try
            {
                Text = GetDisplayText(dataManager.Position);
                if (SelectedIndex != dataManager.Position)
                    SelectedIndex = dataManager.Position;
            }
            finally
            {
                IsInPositionChanged = false;
            }
        }

        public event EventHandler SelectedIndexChanged;

        protected virtual void OnSelectedIndexChanged()
        {
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        private CurrencyManager dataManager;

        private void tryDataBinding()
        {
            if (DataSource == null || base.BindingContext == null)
                return;
            
            CurrencyManager cm;
            try
            {
                cm = (CurrencyManager)base.BindingContext[DataSource, DataMember];
            }
            catch (ArgumentException)
            {
                // If no CurrencyManager was found
                return;
            }

            if (dataManager != cm)
            {
                if (dataManager != null)
                {
                    dataManager.ListChanged -= listChangedHandler;
                    dataManager.PositionChanged -= positionChangedHandler;
                    valuePropertyDescriptor = null;
                    displayPropertyDescriptor = null;
                }

                dataManager = cm;

                if (dataManager != null)
                {
                    dataManager.ListChanged += listChangedHandler;
                    dataManager.PositionChanged += positionChangedHandler;
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
            if (bs.SortDescriptions[0].PropertyDescriptor.Name != displayMember) return false;
            if (bs.SortDescriptions[0].SortDirection != ListSortDirection.Ascending) return false;
            return true;
        }

        private int FindString(string s)
        {
            if (dataManager == null || s == null) return -1;
            int k = dataManager.List.Count;
            if (string.Compare(s, GetDisplayText(k - 1), true) == 0)
                return k - 1;
            if (IsDataSourceSortedByDisplayMember())
                return FindStringSorted(s);
            return FindStringSimple(s);
        }

        private int FindStringExact(string s)
        {
            if (dataManager == null || string.IsNullOrEmpty(s)) return -1;
            int k = dataManager.List.Count;
            if (string.Compare(s, GetDisplayText(k - 1), true) == 0)
                return k - 1;
            if (IsDataSourceSortedByDisplayMember())
                return FindStringExactSorted(s);
            return FindStringExactSimple(s);
        }

        private int FindStringSimple(string s)
        {

            if (dataManager == null) return -1;
            string s1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                s1 = GetDisplayText(i);
                if (s.Length > s1.Length) continue;
                if (string.Compare(s, s1.Substring(0, s.Length), true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindStringExactSimple(string s)
        {
            if (dataManager == null) return -1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                if (string.Compare(s, GetDisplayText(i), true) == 0)
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
                s1 = GetDisplayText(k3);
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
            s1 = GetDisplayText(k1);
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
                s1 = GetDisplayText(k3);
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

        public int FindValueSimple(object value)
        {
            if (dataManager == null) return -1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                if (object.Equals(value, GetValue(i)))
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindItem(object item)
        {
            if (dataManager == null) return -1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                var listitem = dataManager.List[i];
                if (object.Equals(item, listitem))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool GetDisplayTextX(object value, out string displaytext)
        {
            displaytext = null;
            int k = FindValueSimple(value);
            if (k == -1) return false;
            displaytext = GetDisplayText(k);
            return true;
        }

        private int FindValueAsString(string value)
        {
            if (string.IsNullOrEmpty(value)) return -1;
            if (dataManager == null) return -1;
            for (int i = 0; i < dataManager.List.Count; i++)
            {
                var s = GetDisplayText(i);
                if (s == null) continue;
                if (value == s)
                    return i;
            }
            return -1;
        }

        public string GetDisplayText(int nr)
        {
            try
            {
                string s = GetDisplayTextA(nr);
                return s ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        private string GetDisplayTextA(int nr)
        {
            if (dataManager == null || nr == -1 || nr >= dataManager.List.Count) return null;
            object o = ((IMyObjectView)dataManager.List[nr])?.Object;
            if (o == null) return null;
            var pd = GetDisplayPropertyDescriptor();
            if (pd == null) return null;
            var ret = pd.GetValue(o);
            return ret?.ToString();
        }

        protected PropertyDescriptor GetValuePropertyDescriptor()
        {
            if (ValueMember.IsNOE()) return null;
            if (valuePropertyDescriptor == null)
            {
                var pds = ListBindingHelper.GetListItemProperties(DataSource);
                if (pds == null) return null;
                valuePropertyDescriptor = pds[ValueMember];
            }
            return valuePropertyDescriptor;
        }

        protected PropertyDescriptor GetDisplayPropertyDescriptor()
        {
            if (DisplayMember.IsNOE()) return null;
            if (displayPropertyDescriptor == null)
            {
                var pds = ListBindingHelper.GetListItemProperties(DataSource);
                if (pds == null) return null;
                displayPropertyDescriptor = pds[DisplayMember];
            }
            return displayPropertyDescriptor;
        }

        private object GetValue(int nr)
        {
            if (dataManager == null || nr == -1 || nr >= dataManager.List.Count) return null;
            try
            {
                object o = dataManager.List[nr];
                if (o == null) return null;
                var pd = GetValuePropertyDescriptor();
                if (pd == null) return null;
                var ret = pd.GetValue(o);
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem
        {
            get
            {
                if (dataManager == null || SelectedIndex == -1 || SelectedIndex >= dataManager.List.Count) return null;
                var rv = dataManager.List[SelectedIndex];
                return rv;
            }
            set
            {
                if (dataManager == null || dataManager.List.Count == 0) return;
                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }
                int k = FindItem(value);
                SelectedIndex = k;
            }
        }

        [
            DefaultValue(null),
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Bindable(true),
            Category("Data")
        ]
        public object SelectedValue
        {
            get
            {
                if (SelectedIndex == -1) return null;
                var o = GetValue(SelectedIndex);
                if (o == null) return null;
                return o;
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = -1;
                    if(Text != null) 
                        Text = null;
                    return;
                }
                int k = FindValueSimple(value);
                SelectedIndex = k;
            }
        }

        [
            DefaultValue(null),
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Bindable(true),
            Category("Data")
        ]
        public object SelectedValueAsString
        {
            get
            {
                if (SelectedIndex == -1) return null;
                var o = GetValue(SelectedIndex);
                if (o == null) return null;
                return o.ToString();
            }
            set
            {
                if (value == null)
                {
                    SelectedIndex = -1;
                    return;
                }
                int k = FindValueAsString(value as string);
                selectedIndex = k;
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            if (ReadOnly)
            {
                base.OnPreviewKeyDown(e);
                return;
            }
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
            if (e.KeyChar == (char)Keys.Return)
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
            if (e.KeyCode == Keys.F4)
            {
                OnButtonClicked(EventArgs.Empty);
                e.Handled = true;
            }

            if (ReadOnly)
            {
                base.OnKeyDown(e);
                return;
            }

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                doOnTextChangedDelBackKey = true;
            }

            int k = 0;
            if (e.KeyCode == Keys.Up) k = -1;
            if (e.KeyCode == Keys.Down) k = 1;
            if (dataManager != null && k != 0)
            {
                base.OnKeyDown(e);
                if (e.Handled) return;
                if (SyncPosition)
                    dataManager.Position = dataManager.Position + k;
                else
                    SelectedIndex = SelectedIndex + k;
                e.Handled = true;
                return;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (ReadOnly)
            {
                base.OnKeyUp(e);
                return;
            }
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
            if (iFoundIndex >= 0 && !Text.IsNOE())
            {
                string sFoundText = GetDisplayText(iFoundIndex);
                string sAppendText = sFoundText.Substring(sTypedText.Length);
                AppendText(sAppendText);
                SelectedIndex = iFoundIndex;
                SelectionStart = sTypedText.Length;
                SelectionLength = sAppendText.Length;
            }
            else
            {
                if (LimitToList)
                {
                    SelectedIndex = -1;
                }
                else
                {
                    selectedIndex = -1;
                }
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


}
