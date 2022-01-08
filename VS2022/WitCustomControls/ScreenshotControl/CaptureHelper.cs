using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Windows.Forms;
using System.Drawing;

namespace CodePlex.WitCustomControls.Screenshot
{
    internal class CaptureHelper
    {
        #region Public Methods

        public static void SaveCapture(ref WorkItem workItem, ref TempAttachment tempAttachment)
        {
            if (Clipboard.ContainsImage())
            {
                if (workItem != null)
                {
                    Image image = Clipboard.GetImage();
                    SaveAttachmentDialog dialog = new SaveAttachmentDialog((Image)image.Clone());
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            tempAttachment = new TempAttachment(image, dialog.AttachmentName, dialog.Comment);
                            workItem.Attachments.Add(tempAttachment.Attachment);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No internal reference to TFS WorkItem was set.");
                }
            }
            else
            {
                MessageBox.Show("Cannot create attachment: no image available in clipboard.");
            }
        }

        #endregion
    }
}
