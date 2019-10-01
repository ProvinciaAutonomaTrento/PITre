<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TestataDocumento.ascx.cs" Inherits="DocsPAWA.documento.TestataDocumento" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
<TABLE class="info_grigio" id="tblDocumentoGrigio" cellSpacing="1" cellPadding="0" width="398px"
	border="0">
    <TR valign="middle" id="trStatoConsolidamento" runat="server">
        <TD class="titolo_scheda" colspan="6" align="center">
            <asp:Label id="lblStatoConsolidamento"  CssClass="testo_red"  runat="server" Width="100%"></asp:Label>
        </TD>
    </TR>
	<TR valign="middle">
		<TD class="titolo_scheda" width="5%"> 
			<asp:Label id="lblTitleNumProtocollo" runat="server" Width="100%">N. Prot.:</asp:Label></TD>
		<TD class="testo_grigio" width="23%">
			<asp:Label id="lblNumProtocollo" runat="server" Width="100%"></asp:Label></TD>
		<TD class="titolo_scheda" width="5%">
			<asp:Label id="lblTitleData" runat="server" Width="100%">Data:</asp:Label></TD>
		<TD class="testo_grigio" width="32%">
			<asp:Label id="lblData" runat="server" Width="100%"></asp:Label></TD>
		<TD class="titolo_scheda" width="5%">
			<asp:Label id="lblTitleIdDocumento" runat="server" Width="100%">ID:</asp:Label></TD>
		<TD class="testo_grigio" width="25%">
			<asp:Label id="lblIdDocumento" runat="server" Width="100%"></asp:Label></TD>
        <td width="5%">&nbsp;</td>
	</TR>
	<TR valign="middle">	
		<TD class="titolo_scheda" width="5%" style="height: 16px">
			<asp:Label id="lblTitleSegnatura" runat="server">Segn.:</asp:Label></TD>
		<TD class="testo_grigio" colspan="5" style="height: 16px">
			<asp:TextBox id="txtSegnatura" runat="server" BorderStyle="None" width="100%" CssClass="testo_red"
				BackColor="#fafafa" ReadOnly="True"></asp:TextBox>
		</TD>
		</td>
        <td width="5%"><cc1:imagebutton id="btn_log" Runat="server" Width="19px" AlternateText="Mostra Storia Documento"
			DisabledUrl="../images/proto/storia.gif" Height="17px"
		    ImageUrl="../images/proto/storia.gif"></cc1:imagebutton></td>

	</TR>
    <tr valign="middle">
    	<TD class="titolo_scheda" width="5%">
		    <asp:Label id="lblPrivato" runat="server" Width="100%">Privato:</asp:Label></TD>
		<TD class="testo_grigio" width="5%">
		    <asp:checkbox id="chkPrivato" Runat="server" Enabled="false" CssClass="titolo_scheda" TextAlign="Left" Checked="False"></asp:checkbox>
		</TD>
		<TD class="titolo_scheda" width="5%">
		    <asp:Label id="lblVisibilita" runat="server" Width="100%">Vis.:</asp:Label></TD>
		</TD>
		<TD class="testo_grigio" colspan="4">
			<cc1:imagebutton id="btnVisibilita" Width="19px" Height="17px" Tipologia="DO_PRO_VISIBILITA" ImageUrl="../images/proto/ico_visibilita2.gif"
				DisabledUrl="../images/proto/ico_visibilita2.gif" Runat="server" AlternateText="Mostra visibilità"></cc1:imagebutton>&nbsp;
		</TD>		    
	</tr>	
	<TR valign="top">
		<TD class="titolo_scheda" width="5%">
			<asp:Label id="lblOggetto" runat="server" Width="100%">Oggetto:</asp:Label></TD>
		<TD class="titolo_scheda" colspan="6">
			<asp:TextBox id="txtOggetto" runat="server" TextMode="MultiLine" BorderStyle="None" Width="325px"
				CssClass="testo_grigio" BackColor="#fafafa" ReadOnly="True"></asp:TextBox>
		</TD>
	</TR>
	<tr valign="middle">
		<TD class="titolo_scheda" width="5%">
			<asp:Label id="lblAnnullamento" runat="server" Width="100%">Annullato il:</asp:Label>
		</TD>
		<TD class="testo_grigio" colspan="6">
			<asp:Label id="lblDataAnnullamento" runat="server" CssClass="testo_red" Width="100%"></asp:Label>
		</TD>
	</tr>
	<asp:Panel id="pnl_protoTitolario" runat="server" Visible="false">
	<tr valign="middle">
		<TD class="titolo_scheda" colspan="7" width="100%">
			<asp:Label id="lbl_titleProtoTitolario" runat="server" Width="20%"></asp:Label>&nbsp;&nbsp;
			<asp:Label id="lbl_protoTitolario" runat="server" width="70%" CssClass="testo_grigio"></asp:Label>
		</TD>
	</tr>
	</asp:Panel>	
</TABLE>
