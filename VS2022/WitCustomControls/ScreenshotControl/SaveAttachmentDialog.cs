using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodePlex.WitCustomControls.Screenshot
{
    public partial class SaveAttachmentDialog : Form
    {
        #region Private Members

        private Image _image = null;

        #endregion

        #region Constructors

        public SaveAttachmentDialog(Image image)
        {
            _image = image;
            InitializeComponent();
        }

        #endregion

        #region Public Properties

        public string AttachmentName
        {
            get { return nameTextBox.Text; }
        }

        public string Comment
        {
            get { return commentTextBox.Text; }
        }

        #endregion

        #region Event Handlers

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void PreviewPictureBox_Click(object sender, EventArgs e)
        {
            ViewAttachmentDialog viewDialog = new ViewAttachmentDialog(PreviewPictureBox.Image);
            viewDialog.ShowDialog();
        }

        private void SaveAttachmentDialog_Load(object sender, EventArgs e)
        {
            this.PreviewPictureBox.Image = _image;
        }

        private void CancelSaveButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            OKButton.Enabled = (nameTextBox.Text.Length > 0); 
        }

#endregion	
    }
}