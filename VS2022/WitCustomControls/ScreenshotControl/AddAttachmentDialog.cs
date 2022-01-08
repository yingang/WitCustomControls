using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CodePlex.WitCustomControls.Screenshot
{
    public partial class AddAttachmentDialog : Form
    {
        #region Private Members

        private Attachment _attachment = null;
        
        #endregion

        #region Constructors 

        public AddAttachmentDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Properties 

        public Attachment Attachment
        {
            get { return _attachment; }
            set { _attachment = value; }
        }

        #endregion

        #region Event Handlers

        private void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                _attachment = new Attachment(this.AttachmentTextBox.Text.Trim(), this.CommentTextBox.Text.Trim());
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CancelAddButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "All Files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AttachmentTextBox.Text = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

#endregion

    }
}