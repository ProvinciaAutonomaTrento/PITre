<%@ Page language="c#" Codebehind="rubrica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.rubrica" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body onblur="self.focus()" MS_POSITIONING="GridLayout">
		<form id="rubrica" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Rubrica" />
			<TABLE id="Table2" style="WIDTH: 634px; HEIGHT: 240px" cellSpacing="0" cellPadding="0"
				width="634" align="center" border="0">
				<TR>
					<TD colSpan="3" height="3"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
				</TR>
				<TR>
					<TD></TD>
					<TD width="6" height="6"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
					<TD style="HEIGHT: 15px" vAlign="top" align="left" width="2%" bgColor="#d9d9d9" rowSpan="1"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
					<td vAlign="middle" align="center" width="48%" bgColor="#d9d9d9" height="15" rowSpan="1"><asp:label id="Label1" runat="server" CssClass="menu_grigio_popup">Rubrica</asp:label></td>
				</TR>
				<TR>
					<TD style="WIDTH: 306px" vAlign="top" align="left" width="306" bgColor="#d9d9d9" height="15"><IMG height="6" src="../images/proto/angolo_bn_sx.gif" width="6" border="0"></TD>
					<TD class="menu_1_grigio" vAlign="middle" align="center" bgColor="#d9d9d9" colSpan="3"
						height="15"></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 126px" bgColor="#d9d9d9" colSpan="5">
						<TABLE id="Table1" style="WIDTH: 594px; HEIGHT: 108px" cellSpacing="1" cellPadding="0"
							width="594" align="center" border="0">
							<TR>
								<TD class="testo_grigio" style="WIDTH: 91px"><asp:label id="lbl_tipoCorr" runat="server" Visible="False">Tipo Corrispondente</asp:label></TD>
								<TD class="testo_grigio" style="WIDTH: 433px"><asp:radiobuttonlist id="rbl_tipoCorr" runat="server" CssClass="testo_grigio" Visible="False" RepeatDirection="Horizontal"
										Width="190px">
										<asp:ListItem Value="I">Interno&nbsp;&nbsp;</asp:ListItem>
										<asp:ListItem Value="E">Esterno&nbsp;&nbsp;</asp:ListItem>
										<asp:ListItem Value="T" Selected="True">Tutti</asp:ListItem>
									</asp:radiobuttonlist></TD>
								<TD class="testo_grigio"></TD>
							</TR>
							<TR>
								<TD class="testo_grigio" style="WIDTH: 91px"><asp:label id="lblUO" runat="server" CssClass="testo_grigio">UO</asp:label></TD>
								<TD style="WIDTH: 433px"><asp:dropdownlist id="DropDownList1" runat="server" CssClass="testo_grigio" Width="118px" Height="16px">
										<asp:ListItem Value="C">Codice</asp:ListItem>
										<asp:ListItem Value="D" Selected="True">Descrizione</asp:ListItem>
									</asp:dropdownlist><asp:textbox id="TextUO" runat="server" CssClass="testo_grigio" Width="300px"></asp:textbox></TD>
								<TD class="testo_grigio"></TD>
							</TR>
							<TR>
								<TD class="testo_grigio" style="WIDTH: 91px"><asp:label id="lblRuolo" runat="server" CssClass="testo_grigio">Ruolo</asp:label></TD>
								<TD style="WIDTH: 433px"><asp:dropdownlist id="DropDownList2" runat="server" CssClass="testo_grigio" Width="118px">
										<asp:ListItem Value="D" Selected="True">Descrizione</asp:ListItem>
									</asp:dropdownlist><asp:textbox id="TextRuolo" runat="server" CssClass="testo_grigio" Width="300px"></asp:textbox></TD>
								<TD class="testo_grigio"></TD>
							</TR>
							<TR>
								<TD style="WIDTH: 91px"><asp:label id="lblUtente" runat="server" CssClass="testo_grigio">Persona</asp:label></TD>
								<TD style="WIDTH: 433px"><asp:dropdownlist id="DropDownList3" runat="server" CssClass="testo_grigio" Width="117px">
										<asp:ListItem Value="N">Nome</asp:ListItem>
										<asp:ListItem Value="C" Selected="True">Cognome</asp:ListItem>
									</asp:dropdownlist><asp:textbox id="TextUtente" runat="server" CssClass="testo_grigio" Width="300px"></asp:textbox></TD>
								<TD><asp:imagebutton id="btn_ricerca" runat="server" ImageUrl="../images/proto/zoom.gif"></asp:imagebutton></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 19px" align="center" bgColor="#d9d9d9" colSpan="5"><asp:label id="lbl_message" runat="server" CssClass="menu_grigio_popup" Width="594px" Height="15px"></asp:label></TD>
				</TR>
				<TR>
					<TD style="HEIGHT: 19px" bgColor="#d9d9d9" colSpan="5"><asp:table id="tbl_Corr" runat="server"></asp:table></TD>
				</TR>
				<TR>
					<TD align="center" bgColor="#d9d9d9" colSpan="5" height="30"><INPUT id="h_corrispondenti" style="WIDTH: 46px; HEIGHT: 22px" type="hidden" size="2" name="h_corrispondenti"
							runat="server">
						<asp:textbox id="TextBox1" runat="server" CssClass="testo_grigio" Visible="False" Width="34px"
							Height="21px"></asp:textbox><asp:button id="btn_Ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						<asp:button id="btn_Chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:button><asp:hyperlink id="HyperLink1" runat="server" NavigateUrl="datagridRubrica.aspx">HyperLink</asp:hyperlink></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
