<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinkDocFasc.ascx.cs" Inherits="SAAdminTool.UserControls.LinkDocFasc" %>
<script language="javascript">
function _<%=ClientID %>_apriRicercaFascicoli() {
			var newUrl;

			url = "<%= RicercaFascicoliLink %>";
			
			var newLeft=(screen.availWidth-615);
			var newTop=(screen.availHeight-704);	
			
			// apertura della ModalDialog
			rtnValue = window.showModalDialog(url, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:yes;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
			if (rtnValue == "Y") {
			    window.document.getElementById('<%= hf_SelectedObject.ClientID %>').value = '1';
			    window.document.getElementById('<%= Page.Form.ClientID %>').submit();
			}
}

function _<%=ClientID %>_apriRicercaDocumenti(){
		    var newLeft=(screen.availWidth-602);
			var newTop=(screen.availHeight-689);	
			var url = "<%= RicercaDocumentiLink %>";
			rtnValue=window.showModalDialog( url,'','dialogWidth:595px;dialogHeight:643px;status:no;dialogLeft:'+newLeft+'; dialogTop:'+newTop+';center:no;resizable:yes;scroll:no;help:no;');				
			if (rtnValue == "Y") {
			    window.document.getElementById('<%= hf_SelectedObject.ClientID %>').value = '1';
			    window.document.getElementById('<%= Page.Form.ClientID %>').submit();
			}
}

function _<%=ClientID %>_apriLinkEsterno(){
   var newLeft=(screen.availWidth-602);
   var newTop=(screen.availHeight-689);
   var url='<%= this.txt_Link.Text %>'
   window.showModalDialog( url,'','dialogWidth:595px;dialogHeight:643px;status:no;dialogLeft:'+newLeft+'; dialogTop:'+newTop+';center:no;resizable:yes;scroll:no;help:no;');
}

function _<%=ClientID %>_reset(){
   window.document.getElementById('<%= hf_Reset.ClientID %>').value = '1';
   window.document.getElementById('<%= Page.Form.ClientID %>').submit();
}

</script>
<asp:HiddenField ID="hf_SelectedObject" runat="server" Value="0" />
<asp:HiddenField ID="hf_Reset" runat="server" Value="0" />
<asp:HiddenField ID="hf_Id" runat="server" />
<asp:Panel ID="pnlLink_Link" Width="100%" runat="server" Visible="false">
<asp:LinkButton ID="hpl_Link" runat="server" Visible="true" Font-Size="XX-Small" Font-Bold="true" Font-Names="Verdana" ForeColor="Blue" /> 
</asp:Panel>
<asp:Panel ID="pnlLink_InsertModify" Width="100%" runat="server" Height="50px" Visible="false">
    <table width="100%">
    <tr>
       <td style="width: 15%">
          <asp:Label CssClass="titolo_scheda" ID="lbl_maschera" runat="server">Descrizione: </asp:Label>
       </td>
       <td style="width: 85%">
          <asp:TextBox ID="txt_Maschera" runat="server" Width="100%" MaxLength="100"></asp:TextBox>
       </td>
    </tr>
    <tr id="tr_interno" runat="server">
       <td style="width: 15%">
          <asp:Label CssClass="titolo_scheda" ID="lbl_oggetto" runat="server" />
       </td>
       <td style="width: 85%">
            <asp:TextBox ID="txt_NomeObj" runat="server" ReadOnly="true" 
                Width="80%"></asp:TextBox>
            <asp:ImageButton ID="btn_Cerca" BorderWidth="0" Width="26px" runat="server" 
                ImageUrl="~/images/proto/RispProtocollo/lentePreview.gif"/>
            <asp:ImageButton ID="btn_Reset" ToolTip="Cancella selezione" BorderWidth="0" Width="26px" runat="server" 
                ImageUrl="~/images/simpleClearFlag.gif"/>
        </td>
    </tr>
    <tr id="tr_esterno" runat="server">
       <td style="width: 15%">
          <asp:Label CssClass="titolo_scheda" ID="lbl_href" runat="server">Link: </asp:Label>
       </td>
       <td style="width: 85%">
          <asp:TextBox ID="txt_Link" runat="server" Width="100%" MaxLength="200"></asp:TextBox>
       </td>
    </tr>
    </table>
</asp:Panel>
