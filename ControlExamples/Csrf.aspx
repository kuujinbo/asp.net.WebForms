<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Csrf.aspx.cs" Inherits="ControlExamples.Csrf" %>

<asp:content id='content1' contentplaceholderid='PageContent' runat='server'>

<h1><%= Session.SessionID %></h1>

<div>
<kuujinbo:datebox id="db0000" label="Datebox"
required="True" runat="server"
/>
</div>

<div>
<kuujinbo:button ID="b01" runat="server" />
</div>

</asp:content>