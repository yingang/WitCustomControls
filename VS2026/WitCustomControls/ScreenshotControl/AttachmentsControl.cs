using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CodePlex.WitCustomControls.Screenshot
{
    public partial class AttachmentsControl : WitCustomControlBase
    {
        #region Win32 Imports for clipboard handling

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        
        IntPtr _nextClipboardViewer;

        protected override void WndProc(ref Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    CaptureButton.Enabled = Clipboard.ContainsImage();
                    captureToolStripMenuItem.Enabled = Clipboard.ContainsImage();
                    SendMessage(_nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                case WM_CHANGECBCHAIN:
                    if (m.WParam == _nextClipboardViewer)
                        _nextClipboardViewer = m.LParam;
                    else
                        SendMessage(_nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion

        #region Private Members

        private List<TempAttachment> _tempAttachments = new List<TempAttachment>();

        #endregion

        #region Constructors

        public AttachmentsControl()
        {
            InitializeComponent();
            EnableControls();
            _nextClipboardViewer = SetClipboardViewer(this.Handle);
        }

        #endregion

        #region IWorkItemControl Overrides

        public override void Clear()
        {
            AttachmentsListView.Items.Clear();
        }

        public override void InvalidateDatasource()
        {
            ClearTempFiles();
            RefreshAttachments();
        }

        #endregion

        #region Event Handlers

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e); ResizeColumns();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenAttachment();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddAttachment();
        }

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            CaptureAttachment();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteAttachment();
        }

        protected override void OnFieldChanged(object sender, WorkItemEventArgs e)
        {
            if (e.Object is AttachmentCollection)
            {
                RefreshAttachments();
            }
        }

        private void AttachmentsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void AttachmentsListView_DoubleClick(object sender, EventArgs e)
        {
            OpenAttachment();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAttachment();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAttachment();
        }

        private void captureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CaptureAttachment();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteAttachment();
        }
        
        #endregion

        #region Private Methods

        private void ResizeColumns()
        {
            this.NameColumnHeader.Width = panel2.Width - (50 + 90);
            this.SizeColumnHeader.Width = 50;
            this.CommentsColumnHeader.Width = 80;
        }

        private void OpenAttachment()
        {
            try
            {
                ListViewItem lvi = AttachmentsListView.SelectedItems[0];
                Attachment a = (Attachment)lvi.Tag;

                if (a.IsSaved)
                {
                    Process.Start(a.Uri.AbsoluteUri);
                }
                else
                {
                    Process.Start(a.Uri.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddAttachment()
        {
            try
            {
                AddAttachmentDialog dlg = new AddAttachmentDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _workItem.Attachments.Add(dlg.Attachment);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CaptureAttachment()
        {
            try
            {
                TempAttachment tempAttachment = null;
                CaptureHelper.SaveCapture(ref _workItem, ref tempAttachment);
                _tempAttachments.Add(tempAttachment);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteAttachment()
        {
            try
            {
                ListViewItem lvi = AttachmentsListView.SelectedItems[0];
                Attachment a = (Attachment)lvi.Tag;
                string message = string.Format("Are you sure you want to remove '{0}'?", a.Name);
                if (MessageBox.Show(message) == DialogResult.OK)
                {
                    _workItem.Attachments.Remove(a);
                    EnableControls();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearTempFiles()
        {
            // Remove capture files
            if (_tempAttachments != null)
            {
                foreach (TempAttachment ta in _tempAttachments)
                {
                    try
                    {
                        ta.Dispose();
                    }
                    catch
                    {
                    }
                }
                _tempAttachments.Clear();
            }
        }

        private void RefreshAttachments()
        {
            try
            {
                if (_workItem != null && _workItem.Attachments != null)
                {
                    AttachmentsListView.Items.Clear();
                    foreach (Attachment a in _workItem.Attachments)
                    {
                        ListViewItem lvi = new ListViewItem(a.Name);
                        lvi.Tag = a;
                        string formattedLength;
                        if (a.Length < 1024)
                            formattedLength = a.Length.ToString();
                        else if (a.Length < (1024 * 1024))
                            formattedLength = string.Format("{0} KB", a.Length / 1024);
                        else
                            formattedLength = string.Format("{0} MB", a.Length / (1024 * 1024));
                        lvi.SubItems.Add(formattedLength);
                        lvi.SubItems.Add(a.Comment);
                        AttachmentsListView.Items.Add(lvi);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EnableControls()
        {
            bool enable = (AttachmentsListView.SelectedItems.Count > 0);
            OpenButton.Enabled = enable;
            DeleteButton.Enabled = enable;
            openToolStripMenuItem.Enabled = enable;
            deleteToolStripMenuItem.Enabled = enable;
        }

        #endregion
    }
}
