<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs"
  Inherits="ControlExamples._default"
%>
<%--   ClientIDMode="static" Predictable --%>
<asp:content id='content1' contentplaceholderid='PageContent' runat='server'>
<h1>TEST</h1>

<div class="row">
<div class="col-md-6">
<fieldset>
<div>
<kuujinbo:textbox id="tb00" label="textbox: [required='True' ValidationGroup='k00']"
  required="True" ValidationGroup="k00" 
  runat="server"
/>
</div>

<div>
<kuujinbo:textbox id="tbregex00" label="textbox: : [required='True' ValidationGroup='k00' regex='^\w+$']"
  required="True" ValidationGroup="k00" regex="^\w+$"
  runat="server"
/>
</div>

<div>
<kuujinbo:dropdownlist ID="dl00" label="dropdownlist: [required='True' ValidationGroup='k00']" 
  required="True" ValidationGroup="k00" 
  runat="server"
>
<asp:ListItem Text="Select..." Value="" />
<asp:ListItem Text="00" />
<asp:ListItem Text="01" />
<asp:ListItem Text="02" />
<asp:ListItem Text="03" />
</kuujinbo:dropdownlist>
</div>

<div>
<kuujinbo:radiobuttonlist ID="rbl00" label="radiobuttonlist: [required='True' ValidationGroup='k00' RepeatDirection='Horizontal' RepeatLayout='Flow']"
  required="True" ValidationGroup="k00" 
  RepeatDirection="Horizontal" RepeatLayout="Flow"
  runat="server" 
>
  <asp:ListItem Text="00" />
  <asp:ListItem Text="01" />
  <asp:ListItem Text="02" />
  <asp:ListItem Text="03" />
</kuujinbo:radiobuttonlist>
</div>

<div>
<kuujinbo:checkboxlist ID="cbl00" label="checkboxlist: [required='True' ValidationGroup='k00' RepeatDirection='Horizontal' RepeatLayout='Flow']"
  required="True" ValidationGroup="k00"  
  RepeatDirection="Horizontal" RepeatLayout="Flow"
  runat="server"
>
  <asp:ListItem Text="00" />
  <asp:ListItem Text="01" />
  <asp:ListItem Text="02" />
  <asp:ListItem Text="03" />
</kuujinbo:checkboxlist>
</div>

<div>
<kuujinbo:textbox id="textA" label="textbox: [required='True' ValidationGroup='k00' TextMode='MultiLine']"
  Columns="40" Rows="4" MaxLength="76" TextMode="MultiLine"
  required="True" ValidationGroup="k00" 
  runat="server"
/>
</div>

<div>
<kuujinbo:button ID="b00" ValidationGroup="k00" runat="server" />
</div>
</fieldset>
</div>


<div class="col-md-6">
<fieldset>

<div>
<kuujinbo:checkboxlist ID="cbl01" label="checkboxlist: [required='True' ValidationGroup='k01']"
  required="True" ValidationGroup="k01"  
  runat="server"
>
  <asp:ListItem Text="00" />
  <asp:ListItem Text="01" />
  <asp:ListItem Text="02" />
  <asp:ListItem Text="03" />
</kuujinbo:checkboxlist>
</div>
    
<div>
<kuujinbo:radiobuttonlist ID="rbl01" label="radiobuttonlist: [required='True' ValidationGroup='k01']"
  required="True" ValidationGroup="k01" 
  runat="server"
>
  <asp:ListItem Text="00" />
  <asp:ListItem Text="01" />
  <asp:ListItem Text="02" />
  <asp:ListItem Text="03" />
</kuujinbo:radiobuttonlist>
</div>

<div>
<kuujinbo:checkbox ID="cbcb01" Text="I agree" 
  required="True" ValidationGroup="k01" 
  runat="server" 
/>
</div>

<div>
<kuujinbo:datebox id="db0000" label="datebox: : [required='True' ValidationGroup='k01']"
  MinDate="-40y"
  placeholder="Please enter a valid date"
  required="True" ValidationGroup="k01" 
  runat="server"
/>
</div>

<div>
<kuujinbo:button ID="b01" ValidationGroup="k01" runat="server"  />
</div>
</fieldset>
</div>

</div>
<%--<div>
<kuujinbo:checkbox ID="Checkbox1" Label="I agree" 
  required="True"
  runat="server" 
/>
</div>--%>
</asp:content>