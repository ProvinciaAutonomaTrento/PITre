<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Page language="c#" Codebehind="tabFascListaDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicolo.tabFascListaDoc" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register  TagPrefix="uc2" TagName="IconeRicDoc" Src="../UserControls/IconeRicerca.ascx"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
        <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script type="text/javascript">

		    function ShowDialogSearchDocuments() {
		        var retValue = window.showModalDialog('../ricercaDoc/FiltriRicercaDocumenti.aspx?prov=fasc',
												'',
												'dialogWidth:650px;dialogHeight:280px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');

		        tabFasc.txtFilterDocumentsRetValue.value = retValue;

		        if (retValue)
		            ShowWaitCursor()
		    }

		    function confirmDel() {

		        var agree = confirm("Il documento verrà rimosso dal fascicolo. Continuare?");
		        if (agree) {
		            document.getElementById("txt_confirmDel").value = "si";
		            document.forms[0].submit();
		            return true;
		        }

		    }
		    function ShowWaitCursor() {
		        window.document.body.style.cursor = "wait";
		    }

		    // Script eseguito in fase di cambio pagina griglia
		    function WaitGridPagingAction() {
		        ShowWaitCursor();

		        var ctl = document.getElementById("lbl_docProtocollati");
		        if (ctl != null)
		            ctl.innerHTML = "Ricerca in corso...";
		    }

		    function StampaRisultatoRicerca() {

		        var args = new Object;
		        args.window = window;
		        window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=docInfasc",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");
		    }
		
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="tabFasc" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Lista Documenti in Fascicolo" />
        	<INPUT id="txtFilterDocumentsRetValue" type="hidden" runat="server">
			<input id="txt_confirmDel" type="hidden" name="txt_confirmDel" runat="server" />
					<table style=" height:100%; vertical-align:top;" height="100%" cellSpacing="0" cellPadding="0" width="95%" align="right" border="0" >
				<tr>
					<td height="2"></td>
				</tr>
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="90%" runat="server">
								    <asp:label id="titolo" CssClass="titolo" Runat="server"></asp:label></td>
								<td class="testo_grigio_scuro" style="HEIGHT: 19px" vAlign="middle" align="right" width="5%">
					                <asp:imagebutton id="btn_stampa" Runat="server" ImageUrl="../images/proto/export.gif" AlternateText="Esporta il risultato della ricerca"></asp:imagebutton></td>
				            </tr>
				        </table>
				    </td>
				</tr>
				<tr id="tr_gridDocProt" height="100%" runat="server">
					<td valign="top">
                        <asp:datagrid SkinID="datagrid" id="dt_Prot" runat="server"
                                        Width="100%" Visible="false" 
                                        PageSize="15" AllowCustomPaging="True" 
                                        OnItemCommand="dt_Prot_ItemCommand"
                                        OnPageIndexChanged="dt_Prot_PageIndexChanged"
                                        OnItemDataBound="dt_Prot_ItemDataBound"
                                        OnSelectedIndexChanged="dt_Prot_SelectedIndexChanged"
                                        OnPreRender="dt_Prot_PreRender"
										AllowPaging="True" HorizontalAlign="Center" BorderColor="Gray" BorderWidth="1px" CellPadding="1" 
                                        AutoGenerateColumns="False" BorderStyle="Inset" AllowSorting="True">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
										<HeaderStyle Height="20px" ForeColor="White" CssClass="bg_GrigioH"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText="Doc.">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="10%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<asp:LinkButton Text='<%#DataBinder.Eval(Container, "DataItem.descrDoc")%>' runat="server" ID="link_dettaglio" CommandName="Select" ToolTip="Vai alla scheda del documento" ></asp:LinkButton>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="segnatura">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>' ID="Label2">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>' ID="Textbox2">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="data">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' ID="Label3">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' ID="Textbox3">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="registro">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="15%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' ID="Label4">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.registro") %>' ID="Textbox4">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="tipo Prot">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="3%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<asp:Label id="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProt") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipoProt") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="oggetto">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="20%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.oggetto")) %>' ID="Label6">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.oggetto") %>' ID="Textbox6">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="chiave">
												<HeaderStyle Font-Bold="True"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>' ID="Label7">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>' ID="Textbox7">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="mitt/dest">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="30%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Left"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString_MittDest(DataBinder.Eval(Container, "DataItem.mittdest")) %>' ID="Label8">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.mittdest") %>' ID="Textbox8">
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Data arch.">
												<HeaderStyle Font-Bold="True" HorizontalAlign="Center" Width="12%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center"></ItemStyle>
												<ItemTemplate>
													<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dta_archiviazione") %>' ID="lblDtaArchiviazione">
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dta_archiviazione") %>' ID="txtDtaArchiviazione">
													</asp:TextBox>
												</EditItemTemplate>
                                            </asp:TemplateColumn>
											<asp:BoundColumn Visible="False" DataField="dta_annulla">
												<HeaderStyle Font-Bold="True"></HeaderStyle>
											</asp:BoundColumn>
											<asp:TemplateColumn Visible="False" HeaderText="Num Prot">
									            <HeaderStyle Wrap="False"></HeaderStyle>
									            <ItemTemplate>
										            <asp:Label id="Label77" runat="server" Text='<%# getSegnatura((string) DataBinder.Eval(Container, "DataItem.segnatura")) %>'>
										            </asp:Label>
									            </ItemTemplate>
									            <EditItemTemplate>
										            <asp:TextBox id="Textbox77" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.segnatura") %>'>
										            </asp:TextBox>
									            </EditItemTemplate>
								            </asp:TemplateColumn>
									            <asp:TemplateColumn Visible="False" HeaderText="idProfile">
											            <ItemTemplate>
												            <asp:Label id="lbl_idprofile" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>' >
												            </asp:Label>
											            </ItemTemplate>	
								              </asp:TemplateColumn>
                                                <asp:TemplateColumn Visible="False" HeaderText="docNumber">
											            <ItemTemplate>
												            <asp:Label id="lbl_docnumber" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.docNumber") %>' >
												            </asp:Label>
											            </ItemTemplate>	
								              </asp:TemplateColumn>
								            <asp:TemplateColumn Visible="true">
								                <HeaderStyle Wrap="false" Width="5px" />
								                <ItemTemplate>
								                <uc2:IconeRicDoc ID="UserControlRic" runat="server" 
                                                        ACQUISITA_IMG='<%# DataBinder.Eval(Container, "DataItem.cha_img") %>' 
                                                        DOC_NUMBER='<%# DataBinder.Eval(Container, "DataItem.docNumber") %>' 
                                                        FIRMATO='<%# DataBinder.Eval(Container, "DataItem.cha_firmato") %>' 
                                                        IN_CONSERVAZIONE='<%# DataBinder.Eval(Container, "DataItem.in_conservazione") %>' 
                                                        ID_PROFILE='<%# DataBinder.Eval(Container, "DataItem.idProfile") %>' 
                                                        IN_ADL='<%# DataBinder.Eval(Container, "DataItem.inAdl") %>' 
                                                        PAGINA_CHIAMANTE="tabFascListaDoc" />
								                </ItemTemplate>
								            </asp:TemplateColumn>
										</Columns>
										<PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages" Position="TopAndBottom"></PagerStyle>
									</asp:datagrid>
					</td>
				</tr>
			</table>
			<uc1:datagridpagingwait id="dt_Prot_pagingWait" runat="server"></uc1:datagridpagingwait></form>
	</body>
</HTML>

