using System;
using System.Security;
using System.Security.Permissions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.Helpers;
using System.Threading;

namespace CodePlex.WitCustomControls.MultiValueCustomControl
{
    public sealed partial class MultiValueControl : UserControl, IWorkItemControl, IWorkItemToolTip
    {
        // the data are stored in 3 places: WorkItem Field, CheckedListBox and ComboBox.Text
        // if one of them changes then we update the other two.

        internal enum ChangeSource
        {
            None,
            CheckedListBox,
            TextInput,
            WorkItemField,
            AutoCompleteTextBox,
            CheckedListBoxAutoComplete
        }

        internal enum Behavior
        {
            Undefined,
            CheckedListBox,
            AutoComplete
        }

        private ChangeSource m_changeSource = ChangeSource.None;

        private bool m_fieldIsLoaded = false;

        private MultiValueCombo m_multiValueCombo = new MultiValueCombo();
        private const int MaximumLenght = 255;
        private const int MaximunLines = 10;

        //URL of external data source for list of values
        private string dataSource;

        //Property value for behavior between CheckListBox or AutoComplete behavior
        Behavior controlBehavior = Behavior.Undefined;

        //Keep the values to avoid useless queries
        private List<string> extendedValues;

        private Label m_label;
        private ToolTip m_tooltip;

        #region IWorkItemToolTip

        private void UpdateTooltip()
        {
            // Set the tooltip on the associated label
            if (m_label != null && m_tooltip != null && m_fieldName != null && m_workItem != null)
            {
                Field field = m_workItem.Fields[m_fieldName];

                string format = (field.FieldDefinition.HelpText != null && field.FieldDefinition.HelpText.Length > 0)
                    ? ResourceStrings.Tooltip
                    : ResourceStrings.TooltipNoInfo;
                m_tooltip.SetToolTip(m_label, String.Format(CultureInfo.CurrentCulture, format, field.FieldDefinition.Name, field.FieldDefinition.HelpText));
            }
        }

        Label IWorkItemToolTip.Label
        {
            get
            {
                return m_label;
            }
            set
            {
                m_label = value;
                UpdateTooltip();
            }
        }

        ToolTip IWorkItemToolTip.ToolTip
        {
            get
            {
                return m_tooltip;
            }
            set
            {
                m_tooltip = value;
                UpdateTooltip();
            }
        }

        #endregion

        public MultiValueControl()
        {
            InitializeComponent();

            m_multiValueCombo.Dock = DockStyle.Top;
            m_multiValueCombo.CheckedListBox.CheckOnClick = true;
            m_multiValueCombo.CheckedListBox.BorderStyle = BorderStyle.None;
            m_multiValueCombo.CheckedListBox.Sorted = true;
            m_multiValueCombo.CheckedListBox.IntegralHeight = true;
            m_multiValueCombo.ContextMenu = new ContextMenu();
            m_multiValueCombo.CheckedListBox.MaximumSize = new Size(0, MaximunLines * m_multiValueCombo.CheckedListBox.Font.Height);
            m_multiValueCombo.CheckedListBox.SelectedIndexChanged += new EventHandler(this.checkedListBox_SelectedIndexChanged);
            m_multiValueCombo.CheckedListBox.ItemCheck += new ItemCheckEventHandler(this.checkedListBox_ItemCheck);
            m_multiValueCombo.CheckedListBox.KeyUp += new KeyEventHandler(checkedListBox_KeyUp);
            m_multiValueCombo.TextUpdate += new EventHandler(CheckedListBox_TextChanged);

            m_multiValueCombo.TextBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            m_multiValueCombo.CheckedListBoxAutoComplete.ItemCheck += new ItemCheckEventHandler(this.CheckedListBoxAutoComplete_ItemCheck);

            this.Controls.Add(m_multiValueCombo.AutoCompletePanel);

            this.Controls.Add(m_multiValueCombo);
        }

        private void MultiValueControl_Resize(object sender, EventArgs e)
        {
            if (m_multiValueCombo.CheckedListBox.Items.Count > MaximunLines)
            {
                m_multiValueCombo.CheckedListBox.MinimumSize = new System.Drawing.Size(this.Width, MaximunLines * m_multiValueCombo.CheckedListBox.Font.Height);
            }
            else
            {
                m_multiValueCombo.CheckedListBox.MinimumSize = new System.Drawing.Size(this.Width, m_multiValueCombo.CheckedListBox.Items.Count * m_multiValueCombo.CheckedListBox.Font.Height);
            }
        }

        private void CheckedListBox_TextChanged(object sender, EventArgs e)
        {
            UpdateData(ChangeSource.TextInput);
        }

        private void checkedListBox_ItemCheck(object sender, EventArgs e)
        {
            UpdateData(ChangeSource.CheckedListBox);
        }

        private void checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateData(ChangeSource.CheckedListBox);
        }

        private void checkedListBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateData(ChangeSource.CheckedListBox);
        }

        private void textBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!this.m_multiValueCombo.CheckedListBoxAutoComplete.Items.Contains(m_multiValueCombo.TextBox.Text))
                {
                    UpdateData(ChangeSource.AutoCompleteTextBox);
                }
            }
        }

        private void CheckedListBoxAutoComplete_ItemCheck(System.Object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                ClearUnchecked cu = new ClearUnchecked(m_multiValueCombo.CheckedListBoxAutoComplete);
                e.NewValue = CheckState.Checked;
                System.Threading.Thread oThread = new Thread(cu.DoIt);
                oThread.Start(this);
            }
        }

        internal void UpdateData(ChangeSource source)
        {
            if (m_changeSource != ChangeSource.None || source == ChangeSource.None)
            {
                return;
            }

            try
            {
                m_changeSource = source;

                if (m_workItem == null || m_workItem.Fields[m_fieldName] == null || m_workItem.Fields[m_fieldName].FieldDefinition == null)
                {
                    return;
                }

                switch (m_changeSource)
                {
                    case ChangeSource.CheckedListBox:
                        string value = GetValueFromCheckedListBox();
                        this.m_multiValueCombo.Text = value;
                        this.m_workItem.Fields[m_fieldName].Value = value;
                        SetAutoCompleteCheckedItem(this.m_multiValueCombo.Text);
                        break;

                    case ChangeSource.WorkItemField:
                        string FieldValue = this.m_workItem.Fields[m_fieldName].Value.ToString();
                        ValidateFieldValue();
                        this.m_multiValueCombo.Text = FieldValue;
                        SetCheckedItem(FieldValue);
                        SetAutoCompleteCheckedItem(FieldValue);
                        break;

                    case ChangeSource.TextInput:
                        if (IsTextValid(this.m_multiValueCombo.Text))
                        {
                            this.m_workItem.Fields[m_fieldName].Value = this.m_multiValueCombo.Text;
                            SetCheckedItem(this.m_multiValueCombo.Text);
                            SetAutoCompleteCheckedItem(this.m_multiValueCombo.Text);
                        }
                        break;

                    case ChangeSource.AutoCompleteTextBox:
                        //If text match one of the values and not already in the CheckListBox
                        if (m_multiValueCombo.TextBox.AutoCompleteCustomSource.Contains(m_multiValueCombo.TextBox.Text)) // && !checkedListBox.Items.Contains(m_multiValueCombo.TextBox.Text))
                        {
                            if (m_workItem.Fields[m_fieldName].FieldDefinition.FieldType == FieldType.String && m_workItem.Fields[m_fieldName].Value.ToString().Length + m_multiValueCombo.TextBox.Text.Length + 3 > MaximumLenght)
                            {
                                MessageBox.Show(string.Format(ResourceStrings.StringTooLong, this.m_workItem.Fields[m_fieldName].Name), ResourceStrings.MultiValueControl, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                m_multiValueCombo.CheckedListBoxAutoComplete.Items.Add(m_multiValueCombo.TextBox.Text, true);

                                string textBoxValue = GetValueFromCheckedListBoxAutoComplete();
                                this.m_multiValueCombo.Text = textBoxValue;
                                this.m_workItem.Fields[m_fieldName].Value = textBoxValue;
                                SetCheckedItem(this.m_multiValueCombo.Text);
                                this.m_multiValueCombo.TextBox.Text = "";
                            }
                        }
                        break;

                    case ChangeSource.CheckedListBoxAutoComplete:

                        string autoCompleteValue = GetValueFromCheckedListBoxAutoComplete();
                        this.m_multiValueCombo.Text = autoCompleteValue;
                        this.m_workItem.Fields[m_fieldName].Value = autoCompleteValue;
                        SetCheckedItem(this.m_multiValueCombo.Text);
                        this.m_multiValueCombo.TextBox.Text = "";
                        break;

                    case ChangeSource.None:
                        Debug.Fail("when updating a source should be set");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ResourceStrings.MultiValueControl, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                m_changeSource = ChangeSource.None;
            }
        }

        private string GetValueFromCheckedListBoxAutoComplete()
        {
            //Return the formatted string with the selected values.  ie: [value1];[value2];[value3]
            string text = String.Empty;

            for (int index = 0; index <= m_multiValueCombo.CheckedListBoxAutoComplete.Items.Count - 1; index++)
            {

                if (text.Length > 0) text += ";";


                text += "[" + m_multiValueCombo.CheckedListBoxAutoComplete.Items[index] + "]";
            }

            return text;
        }

        private string GetValueFromCheckedListBox()
        {
            string text = String.Empty;
            foreach (Object checkedItem in this.m_multiValueCombo.CheckedListBox.CheckedItems)
            {
                if (text.Trim().Length != 0)
                {
                    text = text + ";";
                }
                text = text + checkedItem.ToString();
            }
            return text;
        }

        private bool IsTextValid(string text)
        {
            // todo text the value of the field here
            // currently this will select whatever it can from the input text, if the text is invalid, nothing will get selected
            return true;
        }

        private void ValidateFieldValue()
        {
            if (m_workItem.Fields[m_fieldName].FieldDefinition.FieldType != FieldType.String)
            {
                //this control only applys for string type.
                throw new Exception(string.Format(ResourceStrings.WrongType, this.m_workItem.Fields[m_fieldName].Name));
            }

            //display the value
            if (this.ExtendedValues.Count == 0 || this.m_workItem.Fields[m_fieldName].IsLimitedToAllowedValues)
            {
                throw new Exception(string.Format(ResourceStrings.NoSuggestedValues, this.m_workItem.Fields[m_fieldName].Name));
            }

            this.m_multiValueCombo.CheckedListBox.Items.Clear();

            bool IsValueValid = true;

            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();

            if (this.BehaviorMode == Behavior.AutoComplete)
            {
                m_multiValueCombo.Visible = false;
                m_multiValueCombo.AutoCompletePanel.Visible = true;
            }
            else
            {
                m_multiValueCombo.Visible = true;
                m_multiValueCombo.AutoCompletePanel.Visible = false;
            }

            //populate the list
            for (int i = 0; i < this.ExtendedValues.Count; i++)
            {
                var value = this.ExtendedValues[i];

                if (value.Trim().Length > 2 && value.StartsWith("[", StringComparison.OrdinalIgnoreCase) &&
                    value.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                {
                    this.m_multiValueCombo.CheckedListBox.Items.Add(value);

                    if (value.Length >= 0)
                    {
                        autoComplete.Add(value.Substring(1, value.Length - 2));
                    }
                }
                else
                {
                    IsValueValid = false;
                }

                if (IsValueValid)
                {
                    if (value.Length >= 0)
                    {
                        autoComplete.Add(value);
                    }
                }
            }

            this.m_multiValueCombo.TextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.m_multiValueCombo.TextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
            this.m_multiValueCombo.TextBox.AutoCompleteCustomSource = autoComplete;

            if (!IsValueValid && !this.m_fieldIsLoaded) //to only display once during initial load
            {
                throw new Exception(string.Format(ResourceStrings.EmptyValue, this.m_workItem.Fields[m_fieldName].Name));
            }
            this.m_fieldIsLoaded = true;
        }

        private void SetCheckedItem(string text)
        {
            //display the value
            //AllowedValuesCollection allowedValues = this.m_workItem.Fields[m_fieldName].AllowedValues;

            //mark the items checked according to the existing combo box text.
            for (int i = 0; i < m_multiValueCombo.CheckedListBox.Items.Count; i++)
            {
                if (text.IndexOf(m_multiValueCombo.CheckedListBox.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) != -1)
                {
                    m_multiValueCombo.CheckedListBox.SetItemChecked(i, true);
                }
                else
                {
                    m_multiValueCombo.CheckedListBox.SetItemChecked(i, false);
                }
            }
        }

        private void SetAutoCompleteCheckedItem(string text) {

            string[] itemList = text.Split(';');

            foreach (string item in itemList)
            {
                string tempItem=item.Trim();

                if (!string.IsNullOrWhiteSpace(tempItem) && tempItem.Length>2 && tempItem.StartsWith("[", StringComparison.OrdinalIgnoreCase) &&
                                    tempItem.EndsWith("]", StringComparison.OrdinalIgnoreCase)) {
                    string strippedItem = tempItem.Substring(1, tempItem.Length - 2);
                    if (!string.IsNullOrWhiteSpace(tempItem) && !m_multiValueCombo.CheckedListBoxAutoComplete.Items.Contains(strippedItem))
                    {
                        m_multiValueCombo.CheckedListBoxAutoComplete.Items.Add(strippedItem, true);
                    }
                }
            }
        }

        private string DataSource
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.dataSource))
                {
                    StringDictionary properties = ((IWorkItemControl)this).Properties;
                    this.dataSource = (properties.ContainsKey("MultiValueDataProvider") ? properties["MultiValueDataProvider"] : "");
                }
                return this.dataSource;
            }
        }

        private Behavior BehaviorMode
        {
            get
            {
                if (this.controlBehavior==Behavior.Undefined)
                {
                    this.controlBehavior = Behavior.CheckedListBox;
                    StringDictionary properties = ((IWorkItemControl)this).Properties;
                    string controlBehavior = (properties.ContainsKey("Behavior") ? properties["Behavior"] : "");

                    if (!string.IsNullOrWhiteSpace(controlBehavior) && controlBehavior.ToLower() == "autocomplete")
                    {
                        this.controlBehavior = Behavior.AutoComplete;
                    }

                }
                return this.controlBehavior;
            }
        }

        private List<string> ExtendedValues
        {
            get
            {
                if (this.extendedValues == null)
                {
                    AllowedValuesCollection allowedValues = this.m_workItem.Fields[this.m_fieldName].AllowedValues;
                    this.extendedValues = new List<string>();
                    for (int i = 0; i < allowedValues.Count; i++)
                    {
                        this.extendedValues.Add(allowedValues[i]);
                    }
                    List<string> remoteData = this.GetData();
                    foreach (string item in remoteData)
                    {
                        if (!this.extendedValues.ConvertAll<string>((string s) => s.ToLower()).Contains(item.ToLower()))
                        {
                            this.extendedValues.Add(item);
                        }
                    }
                }
                return this.extendedValues;
            }
        }

        private bool IsDataSourceDefined()
        {
            return !string.IsNullOrWhiteSpace(this.DataSource);
        }

        public List<string> GetData()
        {
            List<string> coll = new List<string>();
            if (this.IsDataSourceDefined())
            {
                string result = null;
                string URL = string.Format("{0}", this.DataSource);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL);
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                    {
                        result = stream.ReadToEnd();
                        stream.Close();
                        webResponse.Close();
                    }
                }
                coll = Json.Decode<List<string>>(result);
                for (int i = 0; i < coll.Count; i++)
                {
                    coll[i] = string.Format("{0}", coll[i]);
                }
            }
            return coll;
        }

        #region IWorkItemControl Members

        private EventHandlerList m_events;

        private EventHandlerList DataSourceEvents
        {
            get
            {
                if (m_events == null)
                {
                    m_events = new EventHandlerList();
                }
                return m_events;
            }
        }

        private static object EventBeforeUpdateDatasource = new object();
        event EventHandler IWorkItemControl.BeforeUpdateDatasource
        {
            add { DataSourceEvents.AddHandler(EventBeforeUpdateDatasource, value); }
            remove { DataSourceEvents.RemoveHandler(EventBeforeUpdateDatasource, value); }
        }

        event EventHandler IWorkItemControl.AfterUpdateDatasource
        {
            add { DataSourceEvents.AddHandler(EventBeforeUpdateDatasource, value); }
            remove { DataSourceEvents.RemoveHandler(EventBeforeUpdateDatasource, value); }
        }

        void IWorkItemControl.Clear()
        {
        }

        void IWorkItemControl.FlushToDatasource()
        {
            UpdateData(ChangeSource.CheckedListBox);
        }

        void IWorkItemControl.InvalidateDatasource()
        {
            UpdateData(ChangeSource.WorkItemField);
        }

        private StringDictionary m_properties;

        StringDictionary IWorkItemControl.Properties
        {
            get
            {
                return m_properties;
            }
            set
            {
                m_properties = value;
            }
        }

        private bool m_readOnly;

        bool IWorkItemControl.ReadOnly
        {
            get
            {
                return m_readOnly;
            }
            set
            {
                m_readOnly = value;
            }
        }

        void IWorkItemControl.SetSite(IServiceProvider serviceProvider)
        {
        }

        private WorkItem m_workItem;
        object IWorkItemControl.WorkItemDatasource
        {
            get
            {
                return m_workItem;
            }
            set
            {
                m_workItem = (WorkItem)value;
            }
        }

        private string m_fieldName;
        string IWorkItemControl.WorkItemFieldName
        {
            get
            {
                return m_fieldName;
            }
            set
            {
                m_fieldName = value;
            }
        }

        #endregion
    }

    #region MultiValueCombo
    public class MultiValueCombo : ComboBox
    {
        ToolStripControlHost checkedListHost;
        ToolStripDropDown dropDown;
        CheckedListBox checkedListBox;
        CheckedListBox checkedListBoxAutoComplete;
        TextBox textBox;
        Panel panel;

        private long m_lastTimeStamp;

        public MultiValueCombo()
        {

            textBox = new TextBox();
            textBox.Dock = DockStyle.Top;
            textBox.ContextMenu = new ContextMenu();
            textBox.Size = new System.Drawing.Size(120, 20);

            checkedListBoxAutoComplete = new CheckedListBox();
            checkedListBoxAutoComplete.Dock = DockStyle.Top;
            checkedListBoxAutoComplete.ContextMenu = new ContextMenu();
            checkedListBoxAutoComplete.CheckOnClick = false;
            checkedListBoxAutoComplete.BorderStyle = BorderStyle.Fixed3D;
            checkedListBoxAutoComplete.IntegralHeight = true;
            checkedListBoxAutoComplete.Sorted = true;

            panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.AutoSize = true;
            panel.Controls.Add(checkedListBoxAutoComplete);
            panel.Controls.Add(textBox);
            panel.Visible = false;

            checkedListBox = new CheckedListBox();
            checkedListBox.KeyDown += new KeyEventHandler(checkedListBox_KeyDown);
            checkedListBox.LostFocus += new EventHandler(checkedListBox_LostFocus);

            checkedListHost = new ToolStripControlHost(checkedListBox);
            checkedListHost.Dock = DockStyle.Fill;
            checkedListHost.Margin = new Padding(0);
            checkedListHost.Padding = new Padding(0);

            // create drop down and add listbox to it
            dropDown = new ToolStripDropDown();
            dropDown.AutoClose = false;
            dropDown.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            dropDown.Items.Add(checkedListHost);
            this.DropDownHeight = 1;
        }

        private bool IsDroppedDown
        {
            get { return dropDown.Visible; }
            set
            {
                Debug.Assert(this.IsHandleCreated, "Shouldn't be setting the IsDroppedDown before the handle is created");

                // because the way its hacked winfroms can send the show/hide drop down mulitple times, 
                // we only want to respond once when the user clicks, and we ignore the duplicate messages
                // remember the last time a dropdown was shown/hidden, so that we can ignore messages that are fired right after it.
                m_lastTimeStamp = DateTime.Now.Ticks;

                if (value)
                {
                    ShowDropDown();
                }
                else
                {
                    HideDropDown();
                }

                AsyncHide();
            }
        }

        private void checkedListBox_LostFocus(object sender, System.EventArgs e)
        {
            IsDroppedDown = false;
        }

        public Panel AutoCompletePanel
        {
            get { return panel; }
        }

        public TextBox TextBox
        {
            get { return textBox; }
        }

        public CheckedListBox CheckedListBoxAutoComplete
        {
            get { return checkedListBoxAutoComplete; }
        }

        public CheckedListBox CheckedListBox
        {
            get { return checkedListHost.Control as CheckedListBox; }
        }
        /// <summary>
        /// Hides the default dropdown.
        /// </summary>
        /// <param name="node"></param>
        protected void AsyncHide()
        {
            // Workaround!  Clicking on the toggle to hide the dropdown
            // has the side effect of enabling the default dropdown.  It doesn't
            // appear to be possible to do this synchronously.
            HideDelegate d = delegate()
            {
                base.DroppedDown = false;
            };
            this.BeginInvoke(d);
        }

        private void ShowDropDown()
        {
            if (!dropDown.Visible)
            {
                // fire the OnDropDown event. The combobox itself never fires this event, because the real dropdown is never displayed.
                OnDropDown(EventArgs.Empty);

                dropDown.Show(this, 0, this.Height);
            }
            checkedListBox.Focus();
        }

        private void HideDropDown()
        {

            if (dropDown.Visible)
            {
                dropDown.Close();
            }
        }

        private const int WM_USER = 0x0400,
                          WM_REFLECT = WM_USER + 0x1C00,
                          WM_COMMAND = 0x0111,
                          CBN_DROPDOWN = 7;

        public static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }
        private void checkedListBox_KeyDown(Object sender, KeyEventArgs e)
        {
            // CAUTION! The order of processing the keys is important (the Keys.Constants are
            // bit masks with different combinations)
            if (e.KeyCode == Keys.Tab)
            {
                IsDroppedDown = false;
            }

            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                // toggle item check state
                if (checkedListBox.SelectedIndex != -1)
                    checkedListBox.SetItemChecked(checkedListBox.SelectedIndex, !checkedListBox.GetItemChecked(checkedListBox.SelectedIndex));
            }
            else if (e.KeyCode == Keys.F4 || e.KeyCode == Keys.Escape)
            {
                IsDroppedDown = false;
            }
            else if (e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                IsDroppedDown = false;
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (WM_REFLECT + WM_COMMAND))
            {
                int msg = HIWORD((int)m.WParam);
                if (msg == CBN_DROPDOWN)
                {
                    if (m_lastTimeStamp + 1000000 < DateTime.Now.Ticks)
                    {
                        // toggle the drop down
                        IsDroppedDown = !IsDroppedDown;
                    }

                    return;
                }
            }
            base.WndProc(ref m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dropDown != null)
                {
                    dropDown.Dispose();
                    dropDown = null;
                }
            }
            base.Dispose(disposing);
        }
        private delegate void HideDelegate();
    }

    #endregion

    //We must do the removal from the checklistbox in a separate thread because of a bug in the CheckListBox control when
    //you try to remove an item from inside the ItemCheck event.
    public class ClearUnchecked
    {
        private int _selectedIndex;
        private CheckedListBox _checkedListBox;

        public ClearUnchecked(CheckedListBox checkedListBox)
        {
            _selectedIndex = checkedListBox.SelectedIndex;
            _checkedListBox = checkedListBox;
        }

        public void DoIt(object parameter)
        {
            MultiValueControl control = (MultiValueControl)parameter;

            string selectedValue = this._checkedListBox.SelectedItem.ToString();

            if (selectedValue != null && _checkedListBox.Items.IndexOf(selectedValue) != -1)
            {
                int index = _checkedListBox.Items.IndexOf(selectedValue);

                _checkedListBox.SelectedIndex = index - 1;

                _checkedListBox.Items.RemoveAt(index);

                _checkedListBox.Invalidate();

                control.UpdateData(MultiValueControl.ChangeSource.CheckedListBoxAutoComplete);
            }

        }
    }

    public class ResourceStrings
    {
        public static string EmptyValue = "{0} contains a value that is not supported by the multiple value control because it is either empty or does not properly separate its values by enclosing them square brackets. The string representing multiple values should be of the form “[value 1];[value 2]?";
        public static string MultiValueControl = "Multiple Value Control";
        public static string NoSuggestedValues = "{0} does not have any values to list in the multiple value control. See “Work Item Tracking Multiple Value Control.doc?for details on defining a field to support the multiple value control.";
        public static string StringTooLong = "The set of values selected by the multiple value control in {0} exceeds the 255 character limit.";
        public static string WrongType = "The data type of field {0} is not “string.?See “Work Item Tracking Multiple Value Control.doc?for details on defining a field to support the multiple value control.";
        public static string Tooltip = "{1}" + Environment.NewLine + "[Field Name: {0}]";
        public static string TooltipNoInfo = "[Field Name: {0}]";
        public static string ErrorLoadingValues = "Unable to load values from the configured URL for field {0}.  Message: {1}";

    }
}
