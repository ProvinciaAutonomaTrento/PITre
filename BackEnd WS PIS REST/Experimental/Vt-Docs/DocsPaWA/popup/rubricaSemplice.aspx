<%@ Page language="c#" Codebehind="rubricaSemplice.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rubricaSemplice" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>rubricaSemplice</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
<!--
	function ChangeCursor(tipo)
		{
			/*
			if(tipo=="default")
				window.document.all("insDest").style.cursor='default';
			if(tipo=="hand")
				window.document.all("insDest").style.cursor='hand';
			*/
		}
		
		function InsertON_OFF(wnd)
		{
			
			if(wnd=="trasm")
				window.document.all("insDest").style.visibility="hidden";
			else
				window.document.all("insDest").style.visibility="visible";	
		}
//-->
		</script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="rubricaSemplice" method="post" runat="server">
			<TABLE id="Table1" class="info" align="center" border="0">
				<TR>
					<td class="item_editbox" colSpan="2">
						<P class="boxform_item"><asp:label id="Label1" runat="server"> Filtro per</asp:label><asp:label id="Label2" runat="server" Visible="False">Rubrica UO esterne</asp:label></P>
					</td>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD class="titolo_scheda" vAlign="center" align="middle" height="26">
						<asp:radiobuttonlist id="rbFiltro" runat="server" cssclass="titolo_scheda" RepeatDirection="Horizontal">
							<asp:ListItem Value="fUO">UO&nbsp;&nbsp;</asp:ListItem>
							<asp:ListItem Value="fRuolo">Ruolo&nbsp;&nbsp;</asp:ListItem>
							<asp:ListItem Value="fUtente">Utente&nbsp;&nbsp;</asp:ListItem>
						</asp:radiobuttonlist></TD>
				</TR>
				<tr>
					<td height="3"></td>
				</tr>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="10" border="0">
						<asp:label id="LabelRubrica" runat="server" CssClass="titolo_scheda">Descrizione&nbsp;</asp:label>
						<asp:textbox id="txt_Rubrica" runat="server" CssClass="testo_grigio" Width="433px" Height="22px"></asp:textbox>&nbsp;
						<asp:imagebutton id="btn_cerca" runat="server" AlternateText="Cerca" ImageUrl="../images/proto/zoom.gif"></asp:imagebutton></TD>
				</TR>
				<tr>
					<td height="3"></td>
				</tr>
				<TR>
					<TD><IMG height="1" src="../images/proto/spaziatore.gif" width="10" border="0">
						<asp:label id="LabelIns" runat="server" CssClass="titolo_scheda">Inserisci&nbsp;</asp:label>&nbsp;
						<IMG id="insDest" onmouseover="ChangeCursor('hand');" style="VISIBILITY: visible" onclick="InserisciCorrispondente()" onmouseout="ChangeCursor('default');" height="18" alt="Aggiungi Corrispondente Esterno" src="../images/proto/aggiungi.gif" border="0" name="insDest">
						<!--<asp:ImageButton id="btn_aggiungi" runat="server" ImageUrl="../images/proto/aggiungi.gif" AlternateText="Aggiungi corrispondente"></asp:ImageButton>--></TD>
				</TR>
				<tr>
					<td height="3"></td>
				</tr>
				<TR>
					<TD align="middle"><asp:listbox id="ListCorrispondenti" runat="server" CssClass="testo_grigio" Width="530px" Height="203px" AutoPostBack="True"></asp:listbox></TD>
				</TR>
				<tr>
					<td height="5"></td>
				</tr>
				<TR>
					<TD align="middle" height="30">
						<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Width="39px" Height="19px" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Width="54px" Height="19px" Text="CHIUDI"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
		<script>
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
	</body>
</HTML>
