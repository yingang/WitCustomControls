# Screenshot Controls

The Capture control can be added to any work item tracking type definition. Typically you would provide a label and then the Capture button, similar to the following image:

![](Screenshot%20controls_ScreenshotControl.jpg)

And here’s a cool feature – if the clipboard doesn’t contain an image then the Capture button is disabled. It will become enable as soon as the user presses “print screen” or “alt-gr print-screen”.

![](Screenshot%20controls_DisabledScreenshotControl.jpg)

When the button is pressed a “Save As Attachment” dialog is shown. The dialog lets the user enter the attachment name and comment as usual. Pressing OK will create a new attachment to the work item.

![](Screenshot%20controls_SaveAsDialog.jpg)

It is also possible to click on the preview image to bring up a larger window with the screen shot:

![](Screenshot%20controls_EnlargeDialog.jpg)

To make the capture control even more useful, I’ve also implemented an alternative “File Attachment” control. This control works just like the standard control but also provides the Capture feature.

![](Screenshot%20controls_AttachmentsControl.jpg)

## Schema
To use the control first create a work item type that contains the image. In essence the only thing that needs to be done is to include a new control in the `<FORM>` section in the work item XML. The following examples shows the syntax for the ScreenShot control and the FileAttachment control respectively:

    <Control Type="ScreenshotControl" Label="Screenshot:" LabelPosition="Left" />

    <Tab Label="File Attachments">
        <Control Type="CaptureAttachmentsControl" LabelPosition="Top" />
    </Tab>

After that the control and its associated .wicc files need to be deployed locally on each machine using the control (and no, the TFS web client will currently not handle custom controls). Team Explorer searches for custom controls in folder “Microsoft\Team Foundation\Work Item Tracking\Custom Controls” under Environment.SpecialFolder.CommonApplicationData folder first, then under Environment.SpecialFolder.LocalApplicationData.

So, that was it. Hopefully these little controls will fill part of the gap when it comes to handling screenshots in TFS work items.

## Installation
In order to use this control, at two files must be present in the deployment folder:

* AttachmentsControl.wicc
* ScreenshotControl.wicc
* CodePlex.WitCustomControls.dll

The deployment folder is located at the following location under a default client install: C:\ProgramData\Microsoft\Team Foundation\Work Item Tracking\Custom Controls.

Note in the 2010 release I renamed the control to CaptureAttachmentsControl. because if its has the same name it will override the builtin control. The main reason because the builtin control in 2010 has support for images and you may want to just use the builtin version as they provide similar funtionality. You can drag drop pretty much anything to the attachments control and it will create a file and attach it. You can also copy/paste images. To take a screenshot, hit the PrtScn key then Ctrl+V in the built-in attachments control. 
