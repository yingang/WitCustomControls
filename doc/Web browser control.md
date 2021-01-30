# Web Browser Control
This control presents a simple web browser control inside the TFS client form. The control is configurable via XML attributes to display any webpage, and can pass in fields to be used as parameters in the URL.

## Installation
In order to use this control, at two files must be present in the deployment folder:

* WebBrowserControl.wicc
* CodePlex.WitCustomControls.dll

The deployment folder is located at the following location under a default client install: C:\ProgramData\Microsoft\Team Foundation\Work Item Tracking\Custom Controls.

## Schema
Below is an example of the "Control" entry which should be added to your work item schema to include this control on your form:

    <Tab Label="Connect">
      <Control Type="WebBrowserControl" URL="http://some.page?param0={0}&amp;param1={1}" Params="fieldreferencename,fieldreferencename" Dock="Fill"/>
    </Tab>
          
Where the URL attribute is a formatted string containing parameter placeholders if needed. The Params attribute is optional, and if omitted, the URL will be used verbatim. If the Params attribute is present, the control will try to look up the values of those fields in the current work item and place the value in the relative positions of the marked parameters in the URL.
