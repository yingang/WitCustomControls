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

namespace CodePlex.WitCustomControls.MultiValueCustomControl
{
    public class ResourceStrings
    {
        public static string EmptyValue = "{0} contains a value that is not supported by the multiple value control because it is either empty or does not properly separate its values by enclosing them square brackets. The string representing multiple values should be of the form “[value 1];[value 2]”.";
        public static string MultiValueControl = "Multiple Value Control";
        public static string NoSuggestedValues = "{0} does not have any values to list in the multiple value control. See “Work Item Tracking Multiple Value Control.doc” for details on defining a field to support the multiple value control.";
        public static string StringTooLong = "The set of values selected by the multiple value control in {0} exceeds the 255 character limit.";
        public static string WrongType = "The data type of field {0} is not “string.” See “Work Item Tracking Multiple Value Control.doc” for details on defining a field to support the multiple value control.";
        public static string Tooltip = "{1}" + Environment.NewLine + "[Field Name: {0}]";
        public static string TooltipNoInfo = "[Field Name: {0}]";
    }

    public sealed partial class MultiValueControl : UserControl, IWorkItemControl, IWorkItemToolTip
    {
        // the data are stored in 3 places: WorkItem Field, CheckedListBox and ComboBox.Text
        // if one of them changes then we update the other two.

        private enum ChangeSource
        {
            None,
            CheckedListBox,
            TextInput,
            WorkItemField
        }


        private ChangeSource m_changeSource = ChangeSource.None;

        private bool m_fieldIsLoaded = false;

        private MultiValueCombo m_multiValueCombo = new MultiValueCombo();
        private const int MaximumLenght = 255;
        private const int MaximunLines = 10;

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

        private void UpdateData(ChangeSource source)
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
                        break;

                    case ChangeSource.WorkItemField:
                        string FieldValue = this.m_workItem.Fields[m_fieldName].Value.ToString();
                        ValidateFieldValue();
                        this.m_multiValueCombo.Text = FieldValue;
                        SetCheckedItem(FieldValue);
                        break;

                    case ChangeSource.TextInput:
                        if (IsTextValid(this.m_multiValueCombo.Text))
                        {
                            this.m_workItem.Fields[m_fieldName].Value = this.m_multiValueCombo.Text;
                            SetCheckedItem(this.m_multiValueCombo.Text);
                        }
                        break;

                    case ChangeSource.None:
                        Debug.Fail("when updating a souce should be set");
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
            AllowedValuesCollection allowedValues = this.m_workItem.Fields[m_fieldName].AllowedValues;
            if (allowedValues.Count == 0 || this.m_workItem.Fields[m_fieldName].IsLimitedToAllowedValues)
            {
                throw new Exception(string.Format(ResourceStrings.NoSuggestedValues, this.m_workItem.Fields[m_fieldName].Name));
            }

            m_multiValueCombo.CheckedListBox.Items.Clear();

            bool IsValueValid = true;

            //populate the list
            for (int i = 0; i < allowedValues.Count; i++)
            {
                if (allowedValues[i].Trim().Length > 2 && allowedValues[i].StartsWith("[", StringComparison.OrdinalIgnoreCase) &&
                    allowedValues[i].EndsWith("]", StringComparison.OrdinalIgnoreCase))
                {
                    m_multiValueCombo.CheckedListBox.Items.Add(allowedValues[i]);
                }
                else
                {
                    IsValueValid = false;
                }
            }

            if (!IsValueValid && !this.m_fieldIsLoaded) //to only display once during initial load
            {
                throw new Exception(string.Format(ResourceStrings.EmptyValue, this.m_workItem.Fields[m_fieldName].Name));
            }
            this.m_fieldIsLoaded = true;
        }

        private void SetCheckedItem(string text)
        {
            //display the value
            AllowedValuesCollection allowedValues = this.m_workItem.Fields[m_fieldName].AllowedValues;

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

        private long m_lastTimeStamp;

        public MultiValueCombo()
        {
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

                // becuase the way its hacked winfroms can send the show/hide drop down mulitple times, 
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
}