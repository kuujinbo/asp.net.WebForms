<%@ Page Language="C#" AutoEventWireup="false" Inherits="kuujinbo.asp.net.WebForms.encryptFile" Codebehind="encryptFile.aspx.cs" %>
<asp:content id='content1' contentplaceholderid='PageContent' runat='server'>

<p><strong>Base64 encryption key (DO NOT LOSE)</strong>
<asp:Literal ID='ecKey' runat='server' />
</p>

<div class='tbMargin8'>
<kuujinbo:radiobuttonlist ID='actionType' runat='server' required='True'
  RepeatDirection='Horizontal' RepeatLayout='Flow'
>
<asp:ListItem Text='Encrypt' />
<asp:ListItem Text='Decrypt' />
</kuujinbo:radiobuttonlist>
</div>

<div class='tbMargin8'>
<kuujinbo:textbox ID='encryptionKey' runat='server' required='True'
  label='Base64 Encryption Key' Width='440px'
/>
</div>

<div class='tbMargin8'>
<span class='bRed'><asp:Literal ID='noFile' runat='server' EnableViewState='false' /></span>
<asp:FileUpload ID='uploader' runat='server' />
</div>


<div class='tbMargin8'>
<kuujinbo:button ID='processUpload' runat='server' 
  text='Submit'
  oncommand='process'
  width='120px'
/>
</div>

<p class='boldRed'>
<asp:Literal ID='errorMessage' runat='server' EnableViewState='false' />
</p>


</asp:content>