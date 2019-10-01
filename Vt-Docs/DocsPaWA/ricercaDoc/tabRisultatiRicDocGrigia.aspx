<%@ Page language="c#" Codebehind="tabRisultatiRicDocGrigia.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaDoc.tabRisultatiRicDocGrigia" %>
<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title>DOCSPA > tabRisultatiRicDocGrigia</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
				<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript">
			function StampaRisultatoRicerca()
			{				
				var args=new Object;
				args.window=window;
				window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=fasc",
										args,
										"dialogWidth:450px;dialogHeight:250px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}			
		</script>
		<script language="javascript">
		function ApriModalDialog()
		{
				
			var val;
				val=window.confirm("Vuoi inserire il documento nell'area di Lavoro ?");
			
			if(val)
			{
			window.document.forms[0].hd1.value='Yes';
		
			
			}
			else
			{
			window.document.forms[0].hd1.value='No';
			}
		
		
		}
		
		function StampaRisultatoRicerca()
			{				
				var args=new Object;
				args.window=window;
				window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=doc",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}
		</script>
		
		<script language="javascript">
        function ApriModalDialogNewADL()
		{
				
			var val;
				val=window.confirm("Vuoi eliminare il documento dall'area di Lavoro ?");
			
			if(val)
			{
			window.document.forms[0].hd1.value='Yes';
		
			
			}
			else
			{
			window.document.forms[0].hd1.value='No';
			}
		
		
		}
		
		
		</script>
		<script language="javascript">
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";
				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
		</script>
	</HEAD>
	<body text="#660066" vLink="#ff3366" aLink="#cc0066" link="#660066" MS_POSITIONING="GridLayout">
		<form id="tabRisultatiRicDocGrigia" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" align="center">
				<tr>
					<td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
							<td id="Td2" align="left" height="90%" runat="server">
							     <asp:Label ID="msgADL" runat="server" Text="AREA DI LAVORO - " CssClass="titolo" Font-Bold=true Visible=false></asp:Label>
								<asp:label id="titolo" CssClass="titolo" Runat="server"></asp:label></td>
								<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%">									
								</td>
								<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%">
								    <asp:imagebutton id="btn_stampa" Runat="server" ImageUrl="../images/proto/export.gif" AlternateText="Esporta il risultato della ricerca"></asp:imagebutton>									
								</td>
								<td vAlign="middle" align="center" width="5%">
									<asp:ImageButton id="insertAllADL" Runat="server" ImageUrl="../images/proto/area_new.gif" ToolTip="Inserisci tutti i documenti in 'Area di lavoro'" Width=18px Height=17px></asp:ImageButton>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr id="trBody" runat="server">
					<TD vAlign="middle">
						<asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" AllowCustomPaging="True"
							PageSize="20" AllowPaging="True" HorizontalAlign="Center" BorderColor="Gray" CellPadding="1"
							AutoGenerateColumns="False" BorderStyle="Inset" OnItemCreated="DataGrid1_ItemCreated">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
							    <asp:BoundColumn DataField = "idProfile" Visible = "false"></asp:BoundColumn>
							    <asp:BoundColumn DataField = "docNumber" Visible = "false"></asp:BoundColumn>
								<asp:TemplateColumn  HeaderText="Descrizione">
									<HeaderStyle Wrap="False" Width="15%" Font-Bold="False" Font-Italic="False" 
                                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False" 
                                        HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lblDescrizione" runat="server" Text='<%# this.GetDescrizione((DocsPAWA.DocsPaWR.InfoDocumento) Container.DataItem) %>'>
										</asp:Label>
									</ItemTemplate>
								    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Tipo">
									<HeaderStyle Width="5%" Font-Bold="False" Font-Italic="False" 
                                        Font-Overline="False" Font-Strikeout="False" Font-Underline="False" 
                                        HorizontalAlign="Center"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id="lblTipo" runat="server" Text='<%# this.GetTipo((DocsPAWA.DocsPaWR.InfoDocumento) Container.DataItem) %>'>
										</asp:Label>
									</ItemTemplate>
								    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Oggetto">
									<HeaderStyle HorizontalAlign="Center" Width="70%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.Oggetto")) %>'>
										</asp:Label>
									</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="dett">
									<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="btnDettaglio" runat="server" CommandName="Select" ImageUrl="../images/proto/dettaglio.gif" BorderWidth="1px" BorderColor="Gray" AlternateText='<%# DataBinder.Eval(Container, "DataItem.docNumber") %>' ToolTip="Vai alla scheda del documento" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="VIS">
									<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="btnVis" runat="server" CommandName="Vis" ImageUrl="../images/proto/dett_lente_doc.gif" ToolTip="Visualizza immagine documento" />
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="ADL">
									<HeaderStyle Wrap="False" HorizontalAlign="Center" Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<cc1:imagebutton id="btnAreaLavoro" Runat="server" Width="18px" Height="17" CommandName="Area" ImageUrl="../images/proto/ins_area.gif"
											AlternateText="Inserisci questo documento in 'Area di lavoro'" Tipologia="DO_GET_ADL"></cc1:imagebutton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField = "inAdl" Visible = "false"></asp:BoundColumn>
								<asp:BoundColumn DataField= "acquisitaImmagine" Visible="false"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></TD>
				</tr>
				<tr>
					<td height="5"></td>
				</tr>
				<tr>
					<td align="center"><input id="hd1" type="hidden" value="null" name="hd1" runat="server"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
