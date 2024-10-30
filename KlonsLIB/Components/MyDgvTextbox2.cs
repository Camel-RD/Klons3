using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using KlonsLIB.Misc;
using Microsoft.DotNet.DesignTools.Protocol.Values;

namespace KlonsLIB.Components
{
    public interface IMyDgvTextboxEditingControlEvents2
    {
        void OnButtonClicked(MyDgvTextboxEditingControl2 control);
    }

    [ToolboxItem(false)]
    public class MyDgvTextboxEditingControl2 : MyPickRowTextBox2, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged = false;
        private int rowIndex;

        public bool LimitToList { get; set; }

        public MyDgvTextboxEditingControl2()
        {
            LimitToList = true;
            BorderStyle = BorderStyle.None;
            ShowButton = true;
            SyncPosition = false;
        }

        public object EditingControlFormattedValue
        {
            get
            {
                if (LimitToList && SelectedValue == null)
                    return "";
                if (SelectedValue == null || SelectedIndex < 0) return null;
                var displayvalue = GetDisplayText(SelectedIndex);
                var text = Text;
                return displayvalue;
                
            }
            set
            {
                string valueStr = value as string;
                Text = valueStr;
                if (string.Compare(valueStr, Text, true, CultureInfo.CurrentCulture) != 0)
                {
                    SelectedIndex = -1;
                }
                return;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                        return true;

                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.F4:
                case Keys.Enter:
                    return true;

                case Keys.Left:
                    return SelectionStart + SelectionLength > 0;

                case Keys.Right:
                    return SelectionStart + SelectionLength < Text.Length;
            }
            return !dataGridViewWantsInputKey;

            /*    
            if (((((keyData & Keys.KeyCode) != Keys.Down) && ((keyData & Keys.KeyCode) != Keys.Up)) && (!base.DroppedDown || ((keyData & Keys.KeyCode) != Keys.Escape))) && ((keyData & Keys.KeyCode) != Keys.Enter))
            {
                return !dataGridViewWantsInputKey;
            }
            return true;
            */
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {

        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridView; }
            set { dataGridView = value; }
        }

        public bool EditingControlValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value; }
        }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        protected override void OnSelectedIndexChanged()
        {
            base.OnSelectedIndexChanged();
            if (valueChanged) return;
            valueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            valueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnTextChanged(e);
        }

        protected override void OnButtonClicked(EventArgs e)
        {
            base.OnButtonClicked(e);
            var ievents = dataGridView?.FindForm() as IMyDgvTextboxEditingControlEvents2;
            if (ievents == null) return;
            ievents.OnButtonClicked(this);
        }
    }


    public class MyDgvTextboxCell2 : DataGridViewTextBoxCell
    {
        private static Type defaultFormattedValueType = typeof(string);
        private static Type defaultValueType = typeof(object);

        private MyDgvTextboxEditingControl2 m_EditingControl = null;
        public MyDgvTextboxColumn2 TemplateDGVColumn = null;
        private object m_DataSource;
        private string m_DisplayMember;
        private string m_ValueMember;
        private bool m_limitToList = true;
        private PropertyDescriptor m_ValueMemberProperty = null;
        private PropertyDescriptor m_DisplayMemberProperty = null;
        private CurrencyManager m_DataManager = null;
        
        private byte flags;
        private const byte FLAG_dataSourceInitializedHookedUp = 0x01;



        public MyDgvTextboxCell2() : base()
        {
        }

        public override object Clone()
        {
            MyDgvTextboxCell2 cell = (MyDgvTextboxCell2)base.Clone();
            cell.DataSource = DataSource;
            cell.DisplayMember = DisplayMember;
            cell.ValueMember = ValueMember;
            cell.LimitToList = LimitToList;
            return cell;
        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {

            m_EditingControl = base.DataGridView.EditingControl as MyDgvTextboxEditingControl2;
            if (m_EditingControl == null) return;
            m_EditingControl.BorderStyle = BorderStyle.None;

            m_EditingControl.DataSource = null;
            m_EditingControl.ValueMember = null;
            m_EditingControl.DisplayMember = null;

            m_EditingControl.BindingContext = DataGridView?.BindingContext;
            m_EditingControl.DataSource = m_DataSource;
            m_EditingControl.DisplayMember = m_DisplayMember;
            m_EditingControl.ValueMember = m_ValueMember;
            m_EditingControl.LimitToList = LimitToList;
            
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            //can skip, is set in base with Text = ...
            //m_EditingControl.Text = (initialFormattedValue as string);
        }

        public override object ParseFormattedValue(object formattedValue,
            DataGridViewCellStyle cellStyle,
            TypeConverter formattedValueTypeConverter,
            TypeConverter valueTypeConverter)
        {
            var vt = ValueType;
            if (formattedValue == null) return null;

            if (formattedValueTypeConverter == null)
            {
                if (DataSource == null)
                {
                    return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
                }
                if (DisplayMemberProperty != null)
                {
                    formattedValueTypeConverter = DisplayMemberProperty.Converter;
                }
                else if (ValueMemberProperty != null)
                {
                    formattedValueTypeConverter = ValueMemberProperty.Converter;
                }
            }
            int rowindex = DataGridView.CurrentRow?.Index ?? -1;
            if (formattedValue == null || ((FormattedValueType != null && !FormattedValueType.IsAssignableFrom(formattedValue.GetType())) && formattedValue != System.DBNull.Value))
            {
                if (formattedValue == null)
                {
                    return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
                }
                if (DataGridView != null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                        new FormatException("Invalid cell value"), 
                            ColumnIndex,
                            rowindex, 
                            DataGridViewDataErrorContexts.Parsing);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }
                return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            }

            string strValue = formattedValue as String;
            if ((DataManager != null && (ValueMemberProperty != null || DisplayMemberProperty != null)) ||
                !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember))
            {
                object value;
                if (LookupValueByFormattedValue(rowindex, formattedValue, out value))
                {
                    return value;
                }
                else 
                { 
                    if (formattedValue == null)
                    {
                        value = null;
                    }
                    else if (DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException("Invalid cell value"), ColumnIndex,
                            rowindex, DataGridViewDataErrorContexts.Parsing);
                        RaiseDataError(dgvdee);
                        if (dgvdee.ThrowException)
                        {
                            throw dgvdee.Exception;
                        }

                        if (OwnsEditingControl(rowindex))
                        {
                            ((IDataGridViewEditingControl)m_EditingControl).EditingControlValueChanged = true;
                            DataGridView.NotifyCurrentCellDirty(true);
                        }
                    }
                }
                return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            }
            else
            {
                return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
            }
        }

        public override Type EditType
        {
            get { return typeof(MyDgvTextboxEditingControl2); }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
        }

        /*
        public override Type FormattedValueType
        {
            get
            {
                // we return string for the formatted type
                return defaultFormattedValueType;
            }
        }
        */


        private bool OwnsEditingControl(int rowIndex)
        {
            return (((rowIndex != -1) && (m_EditingControl != null)) &&
                    (rowIndex == m_EditingControl.EditingControlRowIndex));
        }

        public object DataSource
        {
            get { return m_DataSource; }
            set
            {
                if (value != null && !(value is IList || value is IListSource))
                {
                    throw new ArgumentException("Bad data source");
                }

                if (DataSource != value)
                {
                    DataManager = null;
                    UnwireDataSource();

                    m_DataSource = value;
                    WireDataSource(value);

                    try
                    {
                        InitializeDisplayMemberPropertyDescriptor(DisplayMember);
                    }
                    catch (Exception exception)
                    {
                        Debug.Assert(DisplayMember != null && DisplayMember.Length > 0);
                        DisplayMemberInternal = null;
                    }

                    try
                    {
                        InitializeValueMemberPropertyDescriptor(ValueMember);
                    }
                    catch (Exception exception)
                    {
                        Debug.Assert(ValueMember != null && ValueMember.Length > 0);
                        ValueMemberInternal = null;
                    }

                    if (value == null)
                    {
                        DisplayMemberInternal = null;
                        ValueMemberInternal = null;
                    }

                    if (OwnsEditingControl(RowIndex))
                    {
                        m_EditingControl.DataSource = value;
                        InitializeComboBoxText();
                    }
                    else
                    {
                        OnCommonChange();
                    }
                }
            }
        }

        private void InitializeComboBoxText()
        {
            Debug.Assert(m_EditingControl != null);
            ((IDataGridViewEditingControl)m_EditingControl).EditingControlValueChanged = false;
            int rowIndex = ((IDataGridViewEditingControl)m_EditingControl).EditingControlRowIndex;
            Debug.Assert(rowIndex > -1);
            DataGridViewCellStyle dataGridViewCellStyle = GetInheritedStyle(null, rowIndex, false);
            m_EditingControl.Text = (string)GetFormattedValue(GetValue(rowIndex), rowIndex, ref dataGridViewCellStyle, null, null, DataGridViewDataErrorContexts.Formatting);
        }

        internal void OnCommonChange()
        {
            if (DataGridView != null && !DataGridView.IsDisposed && !DataGridView.Disposing)
            {
                if (RowIndex == -1)
                {
                    DataGridView.InvalidateColumn(ColumnIndex);
                }
                else
                {
                    DataGridView.InvalidateCell(ColumnIndex, RowIndex);
                }
            }
        }



        private string DisplayMemberInternal
        {
            set
            {
                InitializeDisplayMemberPropertyDescriptor(value);
                if ((value != null && value.Length > 0))
                {
                    m_DisplayMember = value;
                }
            }
        }

        private string ValueMemberInternal
        {
            set
            {
                InitializeValueMemberPropertyDescriptor(value);
                if ((value != null && value.Length > 0))
                {
                    m_ValueMember = value;
                }
            }
        }


        public string DisplayMember
        {
            get { return m_DisplayMember; }
            set
            {
                DisplayMemberInternal = value;
                if (OwnsEditingControl(base.RowIndex))
                {
                    if (m_EditingControl != null)
                    {
                        m_EditingControl.DisplayMember = value;
                    }
                }
                else
                {
                    OnCommonChange();
                }
            }
        }

        public string ValueMember
        {
            get { return m_ValueMember; }
            set
            {
                ValueMemberInternal = value;
                if (OwnsEditingControl(base.RowIndex))
                {
                    if (m_EditingControl != null)
                    {
                        m_EditingControl.ValueMember = value;
                    }
                }
                else
                {
                    OnCommonChange();
                }
            }
        }

        private PropertyDescriptor ValueMemberProperty
        {
            get
            {
                return m_ValueMemberProperty;
            }
            set
            {
                m_ValueMemberProperty = value;
            }
        }

        private PropertyDescriptor DisplayMemberProperty
        {
            get
            {
                return m_DisplayMemberProperty;
            }
            set
            {
                m_DisplayMemberProperty = value;
            }
        }

        private CurrencyManager DataManager
        {
            get
            {
                return GetDataManager(DataGridView);
            }
            set
            {
                m_DataManager = value;
            }
        }

        private CurrencyManager GetDataManager(DataGridView dataGridView)
        {
            CurrencyManager cm = m_DataManager;
            if (cm == null && DataSource != null && dataGridView != null && dataGridView.BindingContext != null && !(DataSource == Convert.DBNull))
            {
                ISupportInitializeNotification dsInit = DataSource as ISupportInitializeNotification;
                if (dsInit != null && !dsInit.IsInitialized)
                {
                    if ((flags & FLAG_dataSourceInitializedHookedUp) == 0x00)
                    {
                        dsInit.Initialized += new EventHandler(DataSource_Initialized);
                        flags |= (byte)FLAG_dataSourceInitializedHookedUp;
                    }
                }
                else
                {
                    cm = (CurrencyManager)dataGridView.BindingContext[DataSource];
                    DataManager = cm;
                    InitializeDisplayMemberPropertyDescriptor(DisplayMember);
                    InitializeValueMemberPropertyDescriptor(ValueMember);
                }
            }
            return cm;
        }
        
        private void DataSource_Initialized(object sender, EventArgs e)
        {
            Debug.Assert(sender == DataSource);
            Debug.Assert(DataSource is ISupportInitializeNotification);
            Debug.Assert((flags & FLAG_dataSourceInitializedHookedUp) != 0x00);

            ISupportInitializeNotification dsInit = DataSource as ISupportInitializeNotification;
            if (dsInit != null)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }
            flags = (byte)(flags & ~FLAG_dataSourceInitializedHookedUp);
            InitializeDisplayMemberPropertyDescriptor(DisplayMember);
            InitializeValueMemberPropertyDescriptor(ValueMember);
        }


        private void InitializeDisplayMemberPropertyDescriptor(string displayMember)
        {
            if (DataManager != null)
            {
                if (String.IsNullOrEmpty(displayMember))
                {
                    DisplayMemberProperty = null;
                }
                else
                {
                    BindingMemberInfo displayBindingMember = new BindingMemberInfo(displayMember);
                    DataManager = DataGridView.BindingContext[DataSource, displayBindingMember.BindingPath] as CurrencyManager;
                    PropertyDescriptorCollection props = DataManager.GetItemProperties();
                    PropertyDescriptor displayMemberProperty = props.Find(displayBindingMember.BindingField, true);
                    if (displayMemberProperty == null)
                    {
                        throw new ArgumentException($"Field [{displayMember}] not found");
                    }
                    else
                    {
                        DisplayMemberProperty = displayMemberProperty;
                    }
                }
            }
        }

        private void InitializeValueMemberPropertyDescriptor(string valueMember)
        {
            if (DataManager != null)
            {
                if (String.IsNullOrEmpty(valueMember))
                {
                    ValueMemberProperty = null;
                }
                else
                {
                    BindingMemberInfo valueBindingMember = new BindingMemberInfo(valueMember);
                    DataManager = DataGridView.BindingContext[DataSource, valueBindingMember.BindingPath] as CurrencyManager;
                    PropertyDescriptorCollection props = DataManager.GetItemProperties();
                    PropertyDescriptor valueMemberProperty = props.Find(valueBindingMember.BindingField, true);
                    if (valueMemberProperty == null)
                    {
                        throw new ArgumentException($"Field [{valueMember}] not found");
                    }
                    else
                    {
                        ValueMemberProperty = valueMemberProperty;
                    }

                    if ((DataManager.List is IBindingList) && ((IBindingList)DataManager.List).SupportsSearching
                        && ValueMemberProperty != null)
                    {
                        ((IBindingList)DataManager.List).AddIndex(ValueMemberProperty);
                    }
                }
            }
        }

        private void DataSource_Disposed(object sender, EventArgs e)
        {
            Debug.Assert(sender == DataSource, "How can we get dispose notification from anything other than our DataSource?");
            DataSource = null;
        }

        private void WireDataSource(object dataSource)
        {
            IComponent component = dataSource as IComponent;
            if (component != null)
            {
                component.Disposed += new EventHandler(DataSource_Disposed);
            }
        }

        private void UnwireDataSource()
        {
            IComponent component = DataSource as IComponent;
            if (component != null)
            {
                component.Disposed -= new EventHandler(DataSource_Disposed);
            }
            ISupportInitializeNotification dsInit = DataSource as ISupportInitializeNotification;
            if (dsInit != null && (flags & FLAG_dataSourceInitializedHookedUp) != 0x00)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
                flags = (byte)(flags & ~FLAG_dataSourceInitializedHookedUp);
            }
        }


        protected override object GetFormattedValue(
            object value,
            int rowIndex,
            ref DataGridViewCellStyle cellStyle,
            TypeConverter valueTypeConverter,
            TypeConverter formattedValueTypeConverter,
            DataGridViewDataErrorContexts context)
        {
            if (valueTypeConverter == null)
            {
                if(DataSource == null)
                {
                    return base.GetFormattedValue(null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
                }
                if (ValueMemberProperty != null)
                {
                    valueTypeConverter = ValueMemberProperty.Converter;
                }
                else if (DisplayMemberProperty != null)
                {
                    valueTypeConverter = DisplayMemberProperty.Converter;
                }
            }

            if (value == null || ((ValueType != null && !ValueType.IsAssignableFrom(value.GetType())) && value != System.DBNull.Value))
            {
                if (value == null)
                {
                    return base.GetFormattedValue(null, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
                }
                if (DataGridView != null)
                {
                    DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                        new FormatException("Invalid cell value"), ColumnIndex,
                        rowIndex, context);
                    RaiseDataError(dgvdee);
                    if (dgvdee.ThrowException)
                    {
                        throw dgvdee.Exception;
                    }
                }
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }

            String strValue = value as String;
            if ((DataManager != null && (ValueMemberProperty != null || DisplayMemberProperty != null)) ||
                !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember))
            {
                object displayValue;
                if (!LookupDisplayValue(rowIndex, value, out displayValue))
                {
                    if (value == System.DBNull.Value)
                    {
                        displayValue = System.DBNull.Value;
                    }
                    else if (strValue != null && String.IsNullOrEmpty(strValue) && DisplayType == typeof(String))
                    {
                        displayValue = String.Empty;
                    }
                    else if (DataGridView != null)
                    {
                        DataGridViewDataErrorEventArgs dgvdee = new DataGridViewDataErrorEventArgs(
                            new ArgumentException("Invalid cell value"), ColumnIndex,
                            rowIndex, context);
                        RaiseDataError(dgvdee);
                        if (dgvdee.ThrowException)
                        {
                            throw dgvdee.Exception;
                        }

                        if (OwnsEditingControl(rowIndex))
                        {
                            ((IDataGridViewEditingControl)m_EditingControl).EditingControlValueChanged = true;
                            DataGridView.NotifyCurrentCellDirty(true);
                        }
                    }
                }
                //return displayValue;
                return base.GetFormattedValue(displayValue, rowIndex, ref cellStyle, DisplayTypeConverter, formattedValueTypeConverter, context);
            }
            else
            {
                return base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
            }
        }

        private Type DisplayType
        {
            get
            {
                if (DisplayMemberProperty != null)
                {
                    return DisplayMemberProperty.PropertyType;
                }
                else if (ValueMemberProperty != null)
                {
                    return ValueMemberProperty.PropertyType;
                }
                else
                {
                    return defaultFormattedValueType;
                }
            }
        }

        private TypeConverter DisplayTypeConverter
        {
            get
            {
                return TypeDescriptor.GetConverter(DisplayType);
            }
        }

        private bool LookupDisplayValue(int rowIndex, object value, out object displayValue)
        {
            Debug.Assert(value != null);
            Debug.Assert(ValueMemberProperty != null || DisplayMemberProperty != null ||
                         !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember));

            object item = null;
            if (DisplayMemberProperty != null || ValueMemberProperty != null)
            {
                item = ItemFromComboBoxDataSource(ValueMemberProperty != null ? ValueMemberProperty : DisplayMemberProperty, value);
            }
            if (item == null)
            {
                displayValue = null;
                return false;
            }

            displayValue = GetItemDisplayValue(item);
            return true;
        }

        private bool LookupValueByFormattedValue(int rowIndex, object displayValue, out object value)
        {
            Debug.Assert(displayValue != null);
            Debug.Assert(ValueMemberProperty != null || DisplayMemberProperty != null ||
                         !string.IsNullOrEmpty(ValueMember) || !string.IsNullOrEmpty(DisplayMember));

            object item = null;
            if (DisplayMemberProperty != null || ValueMemberProperty != null)
            {
                var prop = DisplayMemberProperty != null ? DisplayMemberProperty : ValueMemberProperty;
                item = ItemFromComboBoxDataSource(prop, displayValue);
            }
            if (item == null)
            {
                value = null;
                return false;
            }
            value = GetItemValue(item);
            return true;
        }

        internal object GetItemDisplayValue(object item)
        {
            Debug.Assert(item != null);
            bool displayValueSet = false;
            object displayValue = null;
            if (DisplayMemberProperty != null)
            {
                displayValue = DisplayMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (ValueMemberProperty != null)
            {
                displayValue = ValueMemberProperty.GetValue(item);
                displayValueSet = true;
            }
            else if (!string.IsNullOrEmpty(DisplayMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(DisplayMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    displayValue = propDesc.GetValue(item);
                    displayValueSet = true;
                }
            }
            else if (!string.IsNullOrEmpty(ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    displayValue = propDesc.GetValue(item);
                    displayValueSet = true;
                }
            }
            if (!displayValueSet)
            {
                displayValue = item;
            }
            return displayValue;
        }

        internal object GetItemValue(object item)
        {
            Debug.Assert(item != null);
            bool valueSet = false;
            object value = null;
            if (ValueMemberProperty != null)
            {
                value = ValueMemberProperty.GetValue(item);
                valueSet = true;
            }
            else if (!string.IsNullOrEmpty(ValueMember))
            {
                PropertyDescriptor propDesc = TypeDescriptor.GetProperties(item).Find(ValueMember, true /*caseInsensitive*/);
                if (propDesc != null)
                {
                    value = propDesc.GetValue(item);
                    valueSet = true;
                }
            }
            if (!valueSet)
            {
                value = item;
            }
            return value;
        }

        private object ItemFromComboBoxDataSource(PropertyDescriptor property, object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Debug.Assert(property != null);
            Debug.Assert(DataManager != null);
            object item = null;

            if ((DataManager.List is IBindingList) && ((IBindingList)DataManager.List).SupportsSearching)
            {
                int index = ((IBindingList)DataManager.List).Find(property, key);
                if (index != -1)
                {
                    item = DataManager.List[index];
                }
            }
            else
            {
                for (int i = 0; i < DataManager.List.Count; i++)
                {
                    object itemTmp = DataManager.List[i];
                    object value = property.GetValue(itemTmp);
                    if (key.Equals(value))
                    {
                        item = itemTmp;
                        break;
                    }
                }
            }
            return item;
        }

        public bool LimitToList
        {
            get { return m_limitToList; }
            set
            {
                m_limitToList = value;
                if (OwnsEditingControl(base.RowIndex))
                    if (m_EditingControl != null)
                    {
                        m_EditingControl.LimitToList = value;
                    }
            }
        }

        public void ScaleColumn(SizeF factor)
        {
            if (OwnsEditingControl(base.RowIndex))
                if (m_EditingControl != null)
                {
                    //m_EditingControl.ScaleControlA(factor);
                }
        }



    }


    public class MyDgvTextboxColumn2 : DataGridViewColumn
    {
        public MyDgvTextboxColumn2() : base(new MyDgvTextboxCell2())
        {
            TextBoxCellTemplateA.TemplateDGVColumn = this;
        }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(MyDgvTextboxCell2)))
                {
                    throw new InvalidCastException("Must be a MyDgvTextboxCell");
                }
                base.CellTemplate = value;
            }
        }

        private MyDgvTextboxCell2 TextBoxCellTemplateA
        {
            get { return (MyDgvTextboxCell2)CellTemplate; }
        }

        private void CheckCellTemplate()
        {
            if (TextBoxCellTemplateA == null)
            {
                throw new InvalidOperationException("DataGridViewColumn_CellTemplateRequired");
            }
        }

        private void ApplyToCells(Action<MyDgvTextboxCell2> act)
        {
            if (DataGridView == null) return;
            DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
            int rowCount = dataGridViewRows.Count;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                MyDgvTextboxCell2 dataGridViewCell = dataGridViewRow.Cells[Index] as MyDgvTextboxCell2;
                if (dataGridViewCell != null)
                {
                    act(dataGridViewCell);
                }
            }
        }


        [
            DefaultValue(null),
            Category("Data"),
            RefreshProperties(RefreshProperties.Repaint),
            AttributeProvider(typeof(IListSource)),
        ]
        public object DataSource
        {
            get
            {
                CheckCellTemplate();
                return TextBoxCellTemplateA.DataSource;
            }
            set
            {
                CheckCellTemplate();
                TextBoxCellTemplateA.DataSource = value;
                ApplyToCells(cell => cell.DataSource = value);
            }
        }

        [
            DefaultValue(""),
            Category("Data"),
            TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef1.SystemDesign),
            Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef1.SystemDesign,
                typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string DisplayMember
        {
            get
            {
                CheckCellTemplate();
                return TextBoxCellTemplateA.DisplayMember;
            }
            set
            {
                CheckCellTemplate();
                TextBoxCellTemplateA.DisplayMember = value;
                ApplyToCells(cell => cell.DisplayMember = value);
            }
        }

        [
            DefaultValue(""),
            Category("Data"),
            TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef1.SystemDesign),
            Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef1.SystemDesign,
                typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string ValueMember
        {
            get
            {
                CheckCellTemplate();
                return TextBoxCellTemplateA.ValueMember;
            }
            set
            {
                CheckCellTemplate();
                TextBoxCellTemplateA.ValueMember = value;
                ApplyToCells(cell => cell.ValueMember = value);
            }
        }

        [DefaultValue(true),
         Category("Behavior")]
        public bool LimitToList
        {
            get
            {
                CheckCellTemplate();
                return TextBoxCellTemplateA.LimitToList;
            }
            set
            {
                CheckCellTemplate();
                if (LimitToList != value)
                {
                    TextBoxCellTemplateA.LimitToList = value;
                    ApplyToCells(cell => cell.LimitToList = value);
                }
            }
        }

        public void ScaleColumn(SizeF factor)
        {
            TextBoxCellTemplateA.ScaleColumn(factor);
            if (base.DataGridView != null)
            {
                DataGridViewRowCollection rows = base.DataGridView.Rows;
                int count = rows.Count;
                for (int i = 0; i < count; i++)
                {
                    MyDgvTextboxCell2 cell = rows.SharedRow(i).Cells[base.Index] as MyDgvTextboxCell2;
                    if (cell != null)
                    {
                        cell.ScaleColumn(factor);
                    }
                }
            }
        }

        public event EventHandler ButtonClicked;
    }
}
