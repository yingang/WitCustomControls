using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace CodePlex.WitCustomControls.Screenshot
{
    internal class TempAttachment : IDisposable
    {
        #region  Private Members 
        
        private bool _isTempFile;
        private string _tempFileName;
        private Attachment _attachment;

        #endregion

        #region  Constructors 
        
        public TempAttachment(string attachmentName, string comment)
        {
            this._attachment = new Attachment(attachmentName, comment);
            this._isTempFile = false;
        }

        public TempAttachment(Image image, string attachmentName, string comment)
        {
            string tempFileName = string.Format(@"{0}\{1}.jpg",
                                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        attachmentName);
            image.Save(tempFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            this._attachment = new Attachment(tempFileName, comment);
            this._tempFileName = tempFileName;
            this._isTempFile = true;
        }

        #endregion

        #region Public Properties 
        
        public Attachment Attachment
        {
            get
            {
                return _attachment;
            }
        }
      
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_isTempFile)
            {
                File.Delete(_tempFileName);
            }
        }

        #endregion
    }
}
