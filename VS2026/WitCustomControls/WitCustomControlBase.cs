using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CodePlex.WitCustomControls
{
    public class WitCustomControlBase : UserControl, IWorkItemControl
    {
        #region IWorkItemControl Members

        public event EventHandler AfterUpdateDatasource;
        public event EventHandler BeforeUpdateDatasource;

        public virtual void Clear()
        {
        }

        public virtual void FlushToDatasource()
        {
        }

        public virtual void InvalidateDatasource()
        {
        }

        protected StringDictionary _properties;
        public StringDictionary Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        protected bool _readOnly = false;
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                _readOnly = value;
            }
        }

        protected IServiceProvider _serviceProvider = null;
        public void SetSite(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected WorkItem _workItem = null;
        public object WorkItemDatasource
        {
            get
            {
                return _workItem;
            }
            set
            {
                if (value == null && _workItem != null)
                {
                    _workItem.FieldChanged -= OnFieldChanged;
                }

                _workItem = (WorkItem)value;

                if (_workItem != null)
                {
                    _workItem.FieldChanged += new WorkItemFieldChangeEventHandler(OnFieldChanged);
                }
            }
        }

        protected string _workItemFieldName = null;
        public string WorkItemFieldName
        {
            get
            {
                return _workItemFieldName;
            }
            set
            {
                _workItemFieldName = value;
            }
        }

        protected virtual void OnFieldChanged(object sender, WorkItemEventArgs e)
        {
        }

        #endregion
    }
}
