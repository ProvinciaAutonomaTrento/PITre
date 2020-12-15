<%@ Register TagPrefix="uc1" TagName="TestataDocumento" Src="TestataDocumento.ascx" %>
<%@ Page language="c#" Codebehind="docVersioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.docVersioni" %>
<%@ Register TagPrefix="uc1" TagName="CheckInOutController" Src="../CheckInOut/CheckInOutController.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc2" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link id="idLinkCss" href="" type="text/css" rel="stylesheet" />
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
</HEAD>
	<body>
		<form id="docVersioni" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Versioni" />
		<uc2:AclDocumento id="aclDocumento" runat="server"></uc2:AclDocumento>
		
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="398px" align="center"
				border="0">
				<tr vAlign="top">
					<TD height="5">
						<uc1:TestataDocumento id="TestataDocumento" runat="server"></uc1:TestataDocumento></TD>
				</tr>
				<tr vAlign="top">
					<td align="left">
						<!-- TABELLA Versioni -->
						<table class="contenitore" height="99%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<TR>
								<TD height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
								</TD>
							</TR>
							<tr>
								<td align="center"><asp:Label id="lbl_message" runat="server" CssClass="testo_msg_grigio" Height="20px" Width="343px"
										Visible="False">Versioni non trovate</asp:Label>
								</td>
							</tr>
							<TR>
								<TD height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR vAlign="top">
								<TD align="center">
									<asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True"
										CellPadding="1" Width="100%" BorderColor="Gray" OnItemCreated="DataGrid1_ItemCreated">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText="Vers.">
												<HeaderStyle Width="15%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codice") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Note">
												<HeaderStyle Width="50%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.note") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.note") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Data">
												<HeaderStyle Width="20%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="Firmatari">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<asp:ImageButton id="ImageButton2" runat="server" Visible="False" ImageUrl="../images/proto/matita.gif" AlternateText="Elenco Firmatari" CommandName="Firma" CausesValidation="False"></asp:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="chiave">
												<HeaderStyle Width="5%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>' ID="Label4">
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn>
												<HeaderStyle Width="20px"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<cc1:ImageButton id="ImageButton1" runat="server" BorderWidth="0px" ImageUrl="../images/proto/ico_riga.gif" AlternateText="Seleziona" CommandName="Select"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
											Mode="NumericPages"></PagerStyle>
									</asp:datagrid>
								</TD>
							</TR>
						</table>
						<!-- FINE TABELLA VERSIONI -->
					</td>
				</tr>
				<tr height="10%">
					<TD>
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD><cc1:ImageButton id="btn_nuovaVersione" runat="server" AlternateText="Nuova versione" Thema="btn_" SkinId="nuovo_Attivo"
										DisabledUrl="../images/bottoniera/btn_nuovo_nonattivo.gif" Tipologia="DO_VER_NUOVA"></cc1:ImageButton></TD>
								<TD><cc1:imagebutton id="btn_aggiungiAreaLav" DisabledUrl="../images/bottoniera/btn_area_nonattivo.gif"
										Thema="btn_" SkinId="area_attivo" AlternateText="Aggiungi ad Area di lavoro"
										Runat="server" Tipologia="DO_ADD_ADL"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_modiVersione" DisabledUrl="../images/bottoniera/btn_modifica_nonattivo.gif"
										Thema="btn_" SkinId="modifica_attivo" AlternateText="Modifica dati versione"
										Runat="server" Tipologia="DO_VER_MODIFICA"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_rimuoviVersione" DisabledUrl="../images/bottoniera/btn_rimuovi_nonattivo.gif"
										Thema="btn_" SkinId="rimuovi_Attivo" AlternateText="Rimuovi versione" Runat="server"
										Tipologia="DO_VER_RIMUOVI"></cc1:imagebutton></TD>
							</TR>
						</TABLE>
						<!--FINE BOTTONIERA -->
					</TD>
				</tr>
			</table>
		</form>
	</body>
</HTML>

