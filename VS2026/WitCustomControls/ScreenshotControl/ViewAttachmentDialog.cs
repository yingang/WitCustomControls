using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CodePlex.WitCustomControls.Screenshot
{
    public partial class ViewAttachmentDialog : Form
    {
        #region Private Members
        
        private Image _image = null;

        #endregion

        #region Constructors

        public ViewAttachmentDialog(Image image)
        {
            _image = image;
            InitializeComponent();
        }

        #endregion

        #region Event Handlers
        
        private void mainPictureBox_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewAttachmentDialog_Load(object sender, EventArgs e)
        {
            this.Size = _image.Size;
            this.mainPictureBox.Image = _image;
        } 

        #endregion
    }
}