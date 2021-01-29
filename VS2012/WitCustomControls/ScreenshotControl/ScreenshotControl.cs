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

namespace CodePlex.WitCustomControls.Screenshot
{
    public partial class ScreenshotControl : WitCustomControlBase
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

        private Button CaptureButton;
    
        #endregion

        #region Constructors

        public ScreenshotControl()
        {
            InitializeComponent();
            _nextClipboardViewer = SetClipboardViewer(this.Handle);
        }

        #endregion

        #region IWorkItemControl Overrides

        private TempAttachment _tempAttachment = null;

        public override void InvalidateDatasource()
        {
            if (_tempAttachment != null)
            {
                _tempAttachment.Dispose();
                _tempAttachment = null;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            this.CaptureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CaptureButton
            // 
            this.CaptureButton.Location = new System.Drawing.Point(3, 1);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(75, 23);
            this.CaptureButton.TabIndex = 0;
            this.CaptureButton.Text = "Capture...";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // ScreenshotControl
            // 
            this.Controls.Add(this.CaptureButton);
            this.Name = "ScreenshotControl";
            this.Size = new System.Drawing.Size(80, 25);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        private void CaptureButton_Click(object sender, EventArgs e)
        {
            CaptureHelper.SaveCapture(ref _workItem, ref _tempAttachment);
        }

        #endregion 
    }
}
