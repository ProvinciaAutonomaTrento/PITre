<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="rubricaDT.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rubricaDT" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title>Rubrica</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
		
		function ChangeCursor(tipo)
		{			
			if(tipo=="default")
				window.document.all("insDest").style.cursor='default';
			if(tipo=="hand")
				window.document.all("insDest").style.cursor='hand';			
		}
		
		function InsertON_OFF(wnd)
		{
			
			if(wnd=="proto")
				if (window.document.all("insDest")!=null)
					window.document.all("insDest").style.visibility="visible";
					
			else
				if (window.document.all("insDest")!=null)
					window.document.all("insDest").style.visibility="hidden";	
		}
function ApriInsert(wnd,target,firstime)
{
	if(window!=null && window.document!=null &&window.document.frames[0]!=null && window.document.frames[0].document!=null && window.document.frames[0].document.datagridRubrica!=null && window.document.frames[0].document.datagridRubrica.hiddenField!=null)
		{
		window.document.frames[0].document.datagridRubrica.hiddenField.value='noexp';
		window.document.frames[0].document.datagridRubrica.hiddenField2.value='T';
		window.document.frames[0].document.datagridRubrica.submit();
		}
	else
	//DatagridRubrica.aspx?wnd="+wnd+"&target="+target+"&firstime="+firstime
		{
		window.document.all("IFrame_dt").src="datagridRubrica.aspx?wnd="+wnd+"&target="+target+"&firstime="+firstime+"";
		}
}
		</script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
  </HEAD>
	<body onload="InsertON_OFF('<%=_wnd%>');">
		<form id="rubrica" method="post" runat="server">
			<table class="info" width="550" align="center" border="0">
				<TR>
					<td class="item_editbox" colSpan="2">
						<P class="boxform_item"><asp:label id="Label" runat="server">Rubrica</asp:label></P>
					</td>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
						<asp:label id="lbl_tipoCorr" runat="server" CssClass="titolo_scheda" Visible="False">Tipo Corrispondente</asp:label></TD>
					<TD>
						<span id="tipoCorrispondenteAreaReal" runat=server>
							<asp:radiobuttonlist id="rbl_tipoCorr" runat="server" CssClass="testo_grigio" Visible="False" RepeatDirection="Horizontal"
								Width="190px">
								<asp:ListItem Value="I">Interno&nbsp;&nbsp;</asp:ListItem>
								<asp:ListItem Value="E">Esterno&nbsp;&nbsp;</asp:ListItem>
								<asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
							</asp:radiobuttonlist>
						</span>
						<span id="tipoCorrispondenteAreaFake" class="testo_grigio" runat=server>
							<INPUT checked type="radio">Interno&nbsp;&nbsp;
							<INPUT disabled="true" type="radio">Esterno&nbsp;&nbsp;	
							<INPUT disabled="true" type="radio">Tutti
						</span>
						</TD>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
						<asp:label id="lblUO" runat="server" CssClass="titolo_scheda">UO</asp:label></TD>
					<TD><asp:dropdownlist id="DropDownList1" runat="server" CssClass="testo_grigio" Width="118px" Height="16px">
							<asp:ListItem Value="C">Codice</asp:ListItem>
							<asp:ListItem Value="D" Selected="True">Descrizione</asp:ListItem>
						</asp:dropdownlist>&nbsp;<asp:textbox id="TextUO" runat="server" CssClass="testo_grigio" Width="280px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
						<asp:label id="lblRuolo" runat="server" CssClass="titolo_scheda">Ruolo</asp:label></TD>
					<TD><asp:dropdownlist id="DropDownList2" runat="server" CssClass="testo_grigio" Width="118px">
							<asp:ListItem Value="D" Selected="True">Descrizione</asp:ListItem>
						</asp:dropdownlist>&nbsp;<asp:textbox id="TextRuolo" runat="server" CssClass="testo_grigio" Width="280px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
						<asp:label id="lblUtente" runat="server" CssClass="titolo_Scheda">Persona</asp:label></TD>
					<TD><asp:dropdownlist id="DropDownList3" runat="server" CssClass="testo_grigio" Width="117px">
							<asp:ListItem Value="N">Nome</asp:ListItem>
							<asp:ListItem Value="C" Selected="True">Cognome</asp:ListItem>
						</asp:dropdownlist>&nbsp;<asp:textbox id="TextUtente" runat="server" CssClass="testo_grigio" Width="280px"></asp:textbox></TD>
				</TR>
				<tr align="center" height="40">
					<td colspan="2">
						<table align="center">
							<tr>
								<td><IMG height="1" src="../images/proto/spaziatore.gif" width="5" border="0">
									<asp:label id="lbl_ricerca" runat="server" CssClass="titolo_scheda" Height="15px">Ricerca</asp:label>&nbsp;
									<asp:imagebutton id="btn_ricerca" runat="server" AlternateText="Avvia Ricerca" BorderWidth="0px"
										BorderStyle="Solid" BorderColor="Transparent" ImageUrl="../images/proto/zoom.gif" BackColor="Transparent"
										ForeColor="Transparent"></asp:imagebutton>&nbsp;&nbsp;&nbsp;
								</td>
								<asp:Panel ID="pnl_ins" Runat="server">
          <TD>
<asp:label id=lbl_inserimento runat="server" CssClass="titolo_scheda" Height="15px">Inserisci</asp:label>&nbsp; 
            <IMG id=insDest onmouseover="ChangeCursor('hand');" 
            style="VISIBILITY: visible" 
            onclick="ApriInsert('<%=_wnd%>','<%=_target%>','<%=_firstime%>');" 
            onmouseout="ChangeCursor('default');" height=18 
            alt="Aggiungi Corrispondente Esterno" 
            src="../images/proto/aggiungi.gif" border=0 name=insDest> 
          </TD>
								</asp:Panel>
							</tr>
						</table>
					</td>
				</tr>
				<TR>
					<TD align="center" colSpan="2">
						<asp:label id="lbl_message" runat="server" CssClass="testo_grigio" Width="334px" Height="15px"
							Font-Bold="True"></asp:label></TD>
				</TR>
				<TR>
					<TD align="center" colSpan="2">
						<cc2:iframewebcontrol id="IFrame_dt" runat="server" Scrolling="auto" iWidth="100%" iHeight="220" Frameborder="0"
							Marginheight="0" Marginwidth="10"></cc2:iframewebcontrol></TD>
				</TR>
				<!--TR>
					<td class="item_editbox" colSpan="2">
						<P class="boxform_item"><asp:label id="id_lbl_dettCorr" Visible="true" Runat="server">Dettagli Corrispondente</asp:label></td>
				</tr-->
				<TR align="center">
					<TD align="center" colSpan="2">
						<cc2:iframewebcontrol id="IFrame_info" runat="server" Marginwidth="10" Marginheight="0" Frameborder="0"
							iHeight="100" iWidth="100%" Scrolling="auto"></cc2:iframewebcontrol>
					</TD>
				</TR>
				<TR>
					<TD align="center" colSpan="2">
						<INPUT id="h_corrispondenti" style="WIDTH: 46px; HEIGHT: 22px" type="hidden" size="2" name="h_corrispondenti"
							runat="server"><asp:textbox id="TextBox1" runat="server" CssClass="testo_grigio" Visible="False" Width="34px"
							Height="21px"></asp:textbox>&nbsp;&nbsp; <INPUT class=pulsante id=buttonOK onclick="submitClose('<%=_wnd%>','<%=_oggTrasm%>');"type=button value=Ok>&nbsp;
						<asp:button id="btn_Chiudi" runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button></TD>
				</TR>
			</table>
		</form>
	</body>
</HTML>
