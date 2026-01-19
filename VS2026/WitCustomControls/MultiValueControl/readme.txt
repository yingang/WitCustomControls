Fields associated with multivalue control should have list of suggested values and each value enclosed in square brackets. For example:

      <FIELD name="Triage" refname="Microsoft.VSTS.Common.Triage" type="String" reportable="dimension">
        <HELPTEXT>Status of triaging the bug</HELPTEXT>
        <SUGGESTEDVALUES expanditems="false">
          <LISTITEM value="[Approved]" />
          <LISTITEM value="[Investigate]" />
	      <LISTITEM value="[Rejected]" />
	      <LISTITEM value="[Submit]" />
        </SUGGESTEDVALUES>
      </FIELD>

Then use MultiValueControl as controltype for that field in Form section, for example: 

<Control Type="MultiValueControl" FieldName="Microsoft.VSTS.Common.Triage" Label="Triag&amp;e:" LabelPosition="Left" />


Note:
For searching, MultiValueComboBox value is treated as a string. To search for items that has a specific list item selected in the combo box use the contains operator and dont forget the square bracket when you enter the value. Here are few query example


-To sreach for item with [Approved] checked in Triage field use this clause:
AndOr	Field	Operator	Value
And	Triage	Contains	[Approved]


-To sreach for item with ONLY [Approved] is checked in Triage field use this clause:
AndOr	Field	Operator	Value
And	Triage	=		[Approved]

-To sreach for item with [Approved] and [Investigae] are checked in Triage field use these clauses:
AndOr	Field	Operator	Value
And	Triage	Contains	[Approved]
And	Triage	Contains	[Investigate]


-To sreach for item with ONLY [Approved] and [Investigae] are checked in Triage field use this clause:
AndOr	Field	Operator	Value
And	Triage	=		[Approved];[Investigate]
