<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Page language="c#" Codebehind="tabRisultatiRicFasc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.ricercaFascicoli.tabRisultatiRicFasc" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagName="MassiveOperation" TagPrefix="MassiveOperation" Src="~/UserControls/MassiveOperationButtons.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title>DOCSPA > tabRisultatiRicFasc</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function StampaRisultatoRicerca()
			{		
				var args=new Object;
				args.window=window;
				window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=fasc",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}			
		</script>
		<script language="javascript">
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";
				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
		</script>
		<script language=javascript>
	    function ApriModalDialogNewADL()
			{					
				var val;
				val=window.confirm("Vuoi eliminare il fascicolo dall'area di Lavoro ?");
				
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
	</HEAD>
	<body text="#660066" bottomMargin="0" vLink="#ff3366" aLink="#cc0066" link="#660066" leftMargin="0"
		topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="tabRisultatiRicFasc" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="99%" align="center">
				<tr>
					<td height="1"><uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="80%" runat="server">
								    <asp:Label ID="msgADL" runat="server" Text="AREA DI LAVORO - " CssClass="titolo" Font-Bold=true Visible=false></asp:Label>
								    <asp:label id="titolo" CssClass="titolo_real" Runat="server"></asp:label>
								</td>
								<td class="testo_grigio_scuro" style="height: 19px" valign="middle" align="right" width="5%">
                                    <asp:ImageButton ID="btn_archivia" runat="server" ImageUrl="../images/proto/btn_archivia.gif" AlternateText="Inserisci tutti i fascicoli in archivio di deposito"></asp:ImageButton>
                                </td>
								<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%">&nbsp;&nbsp;
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
                    <td height="2"> <asp:Label class="testo_red" ID="lbl_messaggio" runat="server" Visible=false ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <MassiveOperation:MassiveOperation ID="moButtons" runat="server" 
                            CheckBoxControlId="chkSelected" DataGridId="DataGrid1" EnableViewState="true" 
                            HiddenFieldControlId="hfIdProject" ObjType="P" Visible="true" />
                    </td>
                </tr>
				<tr id="trBody" runat="server">
					<td vAlign="middle">
					    <asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" AllowCustomPaging="True" BorderStyle="Inset" AutoGenerateColumns="False"
							Width="100%" CellPadding="1" BorderWidth="1px" BorderColor="Gray" HorizontalAlign="Center" AllowPaging="True" OnItemCreated="DataGrid1_ItemCreated1">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
                            <Columns>
                                <asp:TemplateColumn>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelected" runat="server" />
                                        <asp:HiddenField ID="hfIdProject" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.Id") %>' />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" SortExpression="Stato" HeaderText="Stato">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="contatore">
                                    <ItemTemplate>
                                        <asp:Label ID="Label9" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.contatore") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Tipo" HeaderText="Tipo">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="CodClass" HeaderText="CodClass">
									<HeaderStyle Width="5%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left"></ItemStyle>
									<ItemTemplate>
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Label20">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Textbox1">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<%--<asp:TemplateColumn SortExpression="Codice" HeaderText="Codice">
									<HeaderStyle Width="10%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>--%>
							    <asp:TemplateColumn HeaderText="Codice">
									<HeaderStyle Wrap="False" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
									<asp:LinkButton Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>' runat="server" ID="link_dettaglio" CommandName="Dettaglio" ToolTip="Dettaglio" ForeColor="Black" ></asp:LinkButton>
									</ItemTemplate>	
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Descrizione" HeaderText="Descrizione">
									<HeaderStyle Width="50%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label4" runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.Descrizione")) %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Apertura">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Chiusura">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Chiave">
									<ItemTemplate>
										<asp:Label ID="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False" HeaderText="Codleg">
									<ItemTemplate>
										<asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdLegislatura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdLegislatura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Dett">
									<HeaderStyle Width="5px"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" ImageUrl="../images/proto/folder2.gif"
											CommandName="Select" AlternateText="Dettaglio"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="ADL" Visible=false >
								    <HeaderStyle Wrap="False" HorizontalAlign="Center" Width="5px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                    <ItemTemplate>
                                        <cc1:imagebutton id="btnDelADL" Runat="server" Width="18px" Height="17" ImageUrl="../images/proto/cancella.gif"
											CommandName="delADL" AlternateText="Elimina questo fascicolo in 'Area di lavoro'" OnClientClick="ApriModalDialogNewADL();"></cc1:imagebutton>
                                    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Area cons.">
								    <HeaderStyle Wrap="false" HorizontalAlign="center" Width="5px"/>
								    <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"/>
								    <ItemTemplate>
								        <cc1:ImageButton ID="img_area_lavoro" runat="server" Width="18px" Height="17px" ImageUrl="../images/proto/conservazione_d.gif" 
								        CommandName="AreaConservazione" AlternateText="Inserisci questo fascicolo in 'Area di conservazione'" Tipologia="DO_GET_ADL" /> 
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="false" HeaderText="in conservazione">
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="inConservazione" runat="server" 
                                            Text='<%# DataBinder.Eval(Container, "DataItem.inConservazione") %>'></asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible=false HeaderText="Trasf. deposito">
								    <HeaderStyle Wrap="false" HorizontalAlign="center" Width="5px"/>
								    <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"/>
								    <ItemTemplate>
								        <cc1:ImageButton ID="img_Archivio" runat="server" ImageUrl="../images/proto/btn_archiviaSC.gif" 
								        CommandName="InArchivio" AlternateText="Inserisci il fascicolo in archivio di deposito"/> 
								    </ItemTemplate>
                                </asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr>
					<td vAlign="middle" align="center"><input id="hd1" type="hidden" value="null" name="hd1" runat="server"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
