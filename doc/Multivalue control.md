# MultiValue Control
A ComboBox Control to accept and show multiple values for a field by showing a list of checkboxes. 

![](Multivalue%20control_multivaluecontrol.jpg)

## TFS Installation
To install on a TFS instance, download the control and do the following:

1. Open a web browser and navigate to the TFS instance.
1. Go to "Server settings" under the white gear and select the "Legacy Extensions" tab.
1. Click "Install" and select the zip file "CodePlex.WitCustomControls.MultiValueControl" which is located inside the downloaded folder "WitCustomControlSetup2015-1.3.4.6."

## Visual Studio Installation
To install for a Visual Studio instance, download the control and do the following:

1. Open the downloaded zip.
1. Execute the Windows installer package "WitCustomControlSetup2015."

## Using the Control
In order to use this control, two files must be present in the deployment folder:

* MultiValueControl.wicc
* CodePlex.WitCustomControls.dll

The deployment folder is located at the following location under a default client install: `<ApplicationData>\Microsoft\Team Foundation\Work Item Tracking\Custom Controls`.

## Schema
Fields associated with multivalue control should have list of suggested values and each value enclosed in square brackets. For example:

      <FIELD name="Triage" refname="Microsoft.VSTS.Common.Triage" type="String" reportable="dimension">
        <HELPTEXT>Status of triaging the bug</HELPTEXT>
        <SUGGESTEDVALUES expanditems="false">
          <LISTITEM value="{"[Approved](Approved)"}" />
          <LISTITEM value="{"[Investigate](Investigate)"}" />
          <LISTITEM value="{"[Rejected](Rejected)"}" />
          <LISTITEM value="{"[Submit](Submit)"}" />
        </SUGGESTEDVALUES>
      </FIELD>

Then use MultiValueControl as controltype for that field in Form section, for example: 

  `{" <Control Type="MultiValueControl" FieldName="Microsoft.VSTS.Common.Triage" Label="Triag&amp;e:" LabelPosition="Left" /> "}`

## Querying
For quering work item based on MultiValueControl field, the control value is treated as a string. To search for items that has a specific list item selected in the MultiValueContorl use the contains operator and don't forget the square bracket when you enter the value. If you are searching for a specific list item selected and only that item is selected use the = operator. If sreaching for item that has more than one list item selected use multipe 'contains' clauses for that field.