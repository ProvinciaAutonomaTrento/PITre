<%@ Page language="c#" Codebehind="SendDocResponse.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.Interoperabilita.SendDocResponse" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript">
			
			function CloseWindow()
			{
				window.close();
			}
			
			function WaitSendMail()
			{
				window.document.body.style.cursor="wait";
			
				var panel=document.getElementById("panelGridCheckResponse");
				panel.style.visibility="hidden";
				
				panel=document.getElementById("panelWaitSendMail");
				panel.style.visibility="visible";
			}
			
		</script>
</HEAD>
	<body bottomMargin="5" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="frmSendDocumentResponse" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Esito spedizione documento" />
			<TABLE class="info" id="tblContainer" cellSpacing="0" cellPadding="5" width="900" height="455" align="center"
				border="0" runat="server">
				<TR>
					<TD class="item_editbox">
						<P class="boxform_item"><asp:label id="lblTitle" runat="server">
							Esito spedizione documento
						</asp:label></P>
					</TD>
				</TR>
				<TR>
					<TD class="item_editbox">
						<TABLE class="info_grigio" id="tblSendDetails" cellSpacing="0" cellPadding="2" width="100%"
							align="center" border="0" runat="server">
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblSendDateTime" runat="server">Data e ora spedizione:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtSendDateTime" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblSegnatura" runat="server">Segnatura:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtSegnatura" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblRegistro" runat="server">Registro:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtRegistro" runat="server"></asp:label></TD>
							</TR>
							<TR>
								<TD class="titolo_scheda" vAlign="top" width="25%"><asp:label id="lblSendResponse" runat="server">Esito spedizione:</asp:label></TD>
								<TD class="titolo_scheda" vAlign="top" width="75%"><asp:label id="txtSendResponse" runat="server"></asp:label></TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
				<TR vAlign="middle">
					<TD class="item_editbox" align="right">
						<DIV class="titolo_scheda" id="panelWaitSendMail" style="FONT-SIZE: small; VISIBILITY: hidden; WIDTH: 100%"
							align="center" runat="server">Spedizione in corso ai destinatari selezionati, 
							attendere prego...</DIV>
						<DIV id="panelGridCheckResponse" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 250px" align="right"
							runat="server">
							<asp:CheckBox cssclass="titolo_scheda" id="chkSelectUnselectAll" runat="server" Text="Seleziona/deseleziona tutti"
								AutoPostBack="True" TextAlign="Left" Checked="True"></asp:CheckBox>
							<asp:datagrid id="grdSendMailResponse" runat="server" SkinID="datagrid" AutoGenerateColumns="False" BorderColor="Gray"
								CellPadding="1" BorderWidth="1px" Width="100%" OnItemDataBound="grdSendMailResponse_ItemDataBound">
<SelectedItemStyle CssClass="bg_grigioS">
</SelectedItemStyle>

<AlternatingItemStyle CssClass="bg_grigioA">
</AlternatingItemStyle>

<ItemStyle CssClass="bg_grigioN">
</ItemStyle>

<HeaderStyle HorizontalAlign="Center" CssClass="testo_biancoN" BackColor="#810D06">
</HeaderStyle>

<Columns>
<asp:BoundColumn DataField="MailAddress" HeaderText="">
<HeaderStyle Width="30%">
</HeaderStyle>

<ItemStyle VerticalAlign="Top">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn DataField="Adressee" HeaderText="Destinatario/i">
<HeaderStyle HorizontalAlign="Center" Width="50%">
</HeaderStyle>

<ItemStyle VerticalAlign="Top">
</ItemStyle>
</asp:BoundColumn>
<asp:BoundColumn Visible="False" DataField="SendResultBoolean"></asp:BoundColumn>
<asp:BoundColumn DataField="SendResult" HeaderText="Esito">
<HeaderStyle Width="17%">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Top">
</ItemStyle>
</asp:BoundColumn>
<asp:TemplateColumn>
<HeaderStyle Width="2%">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Top">
</ItemStyle>

<ItemTemplate>
											<asp:CheckBox id="chkSendMail" runat="server" ToolTip="Seleziona per rispedire"></asp:CheckBox>
										
</ItemTemplate>
</asp:TemplateColumn>
<asp:BoundColumn Visible=False DataField="NoInterop" HeaderText="NoInterop">
<HeaderStyle Width="1%">
</HeaderStyle>

<ItemStyle HorizontalAlign="Center" VerticalAlign="Top">
</ItemStyle>
</asp:BoundColumn>
</Columns>
							</asp:datagrid>
						</DIV>
					</TD>
				</TR>
				<TR>
					<TD class="item_editbox" align="center">&nbsp;
						<asp:button id="btnClose" runat="server" Width="80px" Text="CHIUDI" CssClass="PULSANTE" Height="19px"></asp:button>&nbsp;
						<asp:button id="btnSendMail" runat="server" Width="80px" Text="SPEDISCI" CssClass="PULSANTE"
							Height="19px"></asp:button></TD>
				</TR>
			</TABLE>
		</form>
	</body>
</HTML>
