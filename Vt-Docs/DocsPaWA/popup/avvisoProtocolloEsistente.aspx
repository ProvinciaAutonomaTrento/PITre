<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="avvisoProtocolloEsistente.aspx.cs" Inherits="DocsPAWA.popup.avvisoProtocolloEsistente" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
		<title></title>
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	<base target="_self"></base>
	<script language="javascript" type="text/javascript">
	    function ApriFinestraVisibilita(IdProf)
        {
   	    var newLeft=(screen.availWidth-605);
	    var newTop=(screen.availHeight-640);
	    var value_IdProf = document.getElementById(IdProf);
	    //win=window.open("visibilitaDocumento.aspx?VisFrame="+value_IdProf.value,"Visibilita","width=588,height=450,top="+newTop+",left="+newLeft+",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no"); 
	    window.showModalDialog("../popup/visibilitaDocumento.aspx?VisFrame="+value_IdProf.value, "Visibilita","dialogWidth:700px;dialogHeight:505px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";");   
	    return false;
        }
	</script>
	<script type="text/javascript" language="javascript" id="CloseWindow">
     function CloseIt()
        {
            window.returnValue = 'chiudi'; 
            window.close();
            return false;
  
        }
    </script>
    <script type="text/javascript" language="javascript" id="protocollo">
        function protocolla()
        {
            window.returnValue = 'protocolla'; 
            window.close();
        }
    </script>
</head>
<body>
    <form id="fAvviso" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Avviso di protocollo già esistente" />
    <div>
        <input type="hidden" id="IdProf" runat="server" />
        <input type="hidden" id="modelloTrasm" runat="server" />
        <table cellSpacing="2" cellPadding="4" >
        <TR valign="top">
            <td colspan="12" class="menu_1_rosso_minuscolo" align="center" ><asp:Label ID="lbl_messaggio" runat="server"></asp:Label></td>
        </TR>
         <tr valign="top"><td></td></tr>
        <TR valign="top">
           <TD class="titolo_scheda" width="16%" >
			<asp:Label id="lblTitleNumProtocollo" runat="server" Width="100%">N. Prot.:</asp:Label></TD>
		<TD class="testo_grigio" width="20%">
			<asp:Label id="lblNumProtocollo" runat="server" Width="100%"></asp:Label></TD>
		<TD class="titolo_scheda" width="6%">
			<asp:Label id="lblTitleData" runat="server" Width="100%">Data:</asp:Label></TD>
		<TD class="testo_grigio" width="32%">
			<asp:Label id="lblData" runat="server" Width="100%"></asp:Label></TD>
		<TD class="titolo_scheda" width="4%">
			<asp:Label id="lblTitleIdDocumento" runat="server" Width="100%">ID:</asp:Label></TD>
		<TD class="testo_grigio" width="18%">
			<asp:Label id="lblIdDocumento" runat="server" Width="100%"></asp:Label></TD>
		<TD class="testo_grigio" width="2%" valign="bottom">
			<input id="Image1" width=19 height=17 type="image" src="..\images\proto\ico_visibilita2.gif" align="absmiddle" alt="Mostra la Visibilità del Documento" onclick="ApriFinestraVisibilita('IdProf')"/>
		</TD>
		<td class="testo_grigio" width="2%" valign="bottom">
		    <cc1:imagebutton id="btn_VisDoc" ImageAlign="AbsMiddle"  Runat="server" AlternateText="Visualizza immagine documento" ImageUrl="../images/proto/dett_lente_doc.gif"></cc1:imagebutton>
		</td>
	</TR>
	<TR valign="top">
		<TD class="titolo_scheda" width="18%" height="16">
			<asp:Label id="lblTitleSegnatura" runat="server" Width="100%">Segnatura:</asp:Label></TD>
		<TD class="testo_grigio" colspan="6">
			<asp:TextBox id="txtSegnatura" runat="server" BorderStyle="None" Width="325px" CssClass="testo_red"
				BackColor="#fafafa" ReadOnly="True"></asp:TextBox>
		</TD>
	</TR>
	<TR valign="top">
		<TD class="titolo_scheda" width="18%">
			<asp:Label id="lblOggetto" runat="server" Width="100%">Ufficio:</asp:Label></TD>
		<TD class="titolo_scheda" colspan="7">
			<asp:TextBox id="txtOggetto" runat="server" BorderStyle="None" Width="325px"
				CssClass="testo_grigio" BackColor="#fafafa" ReadOnly="True"></asp:TextBox>
		</TD>
	</TR>
	<tr valign="middle">
	<td colspan="13" ></td>
	</tr>
	<tr valign="top"> 
		    <td align="center" colspan="13" class="titolo_scheda" width="18%">  
            <input id="btn_protocolla" type="button" value="Protocolla" class="pulsante" onclick="protocolla()" runat="server"/>&nbsp;&nbsp;
            <input id="btn_chiudi" type="button" value="Chiudi" class="pulsante" onclick="CloseIt()"/>
		    </td>
	</tr>
        </table>
    </div>
    </form>
</body>
</html>
