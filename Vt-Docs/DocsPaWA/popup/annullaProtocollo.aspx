<%@ Register TagPrefix="uc1" TagName="CheckInOutController" Src="../CheckInOut/CheckInOutController.ascx" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
   TagPrefix="uct" %>

<%@ Page Language="c#" CodeBehind="annullaProtocollo.aspx.cs" AutoEventWireup="false"
   Inherits="DocsPAWA.popup.annullaProtocollo" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
   <title></title>
   <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
   <meta content="C#" name="CODE_LANGUAGE">
   <meta content="JavaScript" name="vs_defaultClientScript">
   <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
   <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
   <base target="_self">

   <script language="javascript">
			function AbilitaBtn()
			{
			 var maxLength = 1;
			    var strNote = trim(document.getElementById("txt_note_annullamento").value);
			    var strLen = strNote.length;
			    if (parseInt(strLen) >= parseInt(maxLength))
			    {
			     document.getElementById("btn_ok").disabled=false;
			    } 
			    else
			    {
			      document.getElementById("btn_ok").disabled=true;
			    }
			    
			}
		
			function trim(s)
			{
				while (s.substring(0,1) == ' ') 
				{
					s = s.substring(1,s.length);
				}
				while (s.substring(s.length-1,s.length) == ' ') 
				{
					s = s.substring(0,s.length-1);
				}
			return s;
			}
						
			function ConfirmCancelProtocollo()
			{
				if (confirm('Si sta annullando un documento protocollato. Continuare?'))
				{
					annullaProtocollo.deleteConfirmed.value="true";
				}
			}
			
			function ClosePage(retValue)
			{
				window.returnValue=retValue;
				window.close();
			}
			
			function UndoCheckOut()
			{
				UndoCheckOutDocument(false,true);
			}
	
   </script>

</head>
<body ms_positioning="GridLayout">
   <form id="annullaProtocollo" name="annullaProtocollo" method="post" runat="server">
   <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Annulla protocollo" />
   <uc1:CheckInOutController ID="checkInOutController" runat="server"></uc1:CheckInOutController>
   <table class="info" style="width: 362px; height: 139px" width="362" align="center"
      border="0">
      <tr>
         <td class="item_editbox">
            <p class="boxform_item">
               <asp:Label ID="Label" runat="server">Annullamento</asp:Label></p>
         </td>
      </tr>
      <tr>
         <td height="5">
         </td>
      </tr>
      <tr>
         <td height="45" align="center">
            <table cellpadding="0" cellspacing="0" border="0">
               <tr>
                  <td colspan="2">
                     <asp:Label ID="lbl_messageCheckOut" runat="server" CssClass="titolo_scheda" Width="317px"
                        Height="14px" Visible="False">
			               Il documento risulta bloccato.<br>
			               Impossibile annullare il protocollo.
                     </asp:Label>
                     <asp:Label ID="lbl_messageOwnerCheckOut" runat="server" CssClass="titolo_scheda"
                        Width="317px" Height="14px" Visible="False">
			               Il documento risulta bloccato.<br>
			               Tutte le eventuali modifiche effettuate verranno perse.<br>
                     </asp:Label>
                  </td>
               </tr>
               <tr>
                  <td class="titolo_scheda" style="height: 20px" valign="middle" height="20">
                     <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                     <asp:Label ID="LabelCodice" runat="server" CssClass="titolo_scheda">Motivo dell'annullamento&nbsp;*&nbsp;</asp:Label>
                  </td>
                  <td style="height: 20px" valign="middle" align="right">
                  </td>
                  <td style="height: 20px">
                     <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                  </td>
               </tr>
               <tr>
                  <td colspan="3" class="testo_grigio" align="left">
                     <img height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
                     <asp:TextBox ID="txt_note_annullamento" onkeyup="AbilitaBtn()" runat="server" CssClass="testo_grigio"
                        Height="45px" Columns="1" Rows="3" MaxLength="250" Width="350px"></asp:TextBox>
                  </td>
               </tr>
            </table>
         </td>
      </tr>
      <tr>
         <td height="5">
         </td>
      </tr>
      <tr height="30">
         <td valign="middle" align="center" height="30">
            <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:Button>&nbsp;
            <asp:Button ID="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:Button>
         </td>
      </tr>
   </table>
   <input type="hidden" id="deleteConfirmed" runat="server">
   </form>
</body>
</html>
