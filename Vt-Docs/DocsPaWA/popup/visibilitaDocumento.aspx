<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="visibilitaDocumento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.visibilitaDocumento" EnableEventValidation="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self" />
		<%Response.Expires=-1;%>
		<script language="javascript">
            function calcTesto(f,l) {
 
			  // riconosce il tipo di browser collegato
			  
			  if (document.all) {
			    var IE = true;
			  }
			  if (document.layers) {
			    var NS = true;
			  }
			  
			  // versione per NetScape
			
			  if (NS) {
			    StrLen = f.value.length
			    if (StrLen > l ) {
			        f.value = f.value.substring(0, l);
			      CharsLeft = 0;
			      window.alert("Lunghezza NOTE eccessiva: " + StrLen + " caratteri, max " + l);
			    } else {
			        CharsLeft = l - StrLen;
			    }
			    //document.visibilitaDocumento.clTesto.value = CharsLeft
			  }
			  
			  // versione per Internet Explorer
			  
			  if (IE) {
			      var maxLength = l;
			      var strLen = document.visibilitaDocumento.txt_note.value.length;
			    if (document.visibilitaDocumento.txt_note.value.length > maxLength) {
			        document.visibilitaDocumento.txt_note.value = document.visibilitaDocumento.txt_note.value.substring(0, maxLength);
			      cleft = 0;
			      window.alert("Lunghezza NOTE eccessiva: " + strLen + " caratteri, max " + maxLength);
			    } else {
			    cleft = maxLength - strLen;
			    }
			    //document.visibilitaDocumento.clTesto.value = cleft
			  }
            }

		</script>
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2">
		<form id="visibilitaDocumento" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Visibilità documento" />
			<TABLE id="Table1" class="info" width="100%" align="center" border="0">
				<TR>
					<TD class="item_editbox">
                        <P class="boxform_item">
							<asp:label id="Label2" runat="server">Visibilità documento</asp:label>
					        </P>
					</TD>
				</TR>
				<tr>
				    <td>
				        <asp:label id="lbl_InArchivio" CssClass="testo_red" runat="server" Visible="false">Attenzione, il documento è presente in archivio di deposito.</asp:label>
                        <asp:label id="lbl_atipicita" CssClass="testo_red" runat="server" Visible="false">Atipicità Documento :</br></asp:label>
				    </td>
				</tr>
				<tr>
                    <td align="right">
				        <cc2:imagebutton ImageAlign="Middle" id="btn_storia_visibilita" Runat="server" Width="19px" AlternateText="Mostra storia revoche"
							    DisabledUrl="../images/proto/storia.gif" ImageUrl="../images/proto/storia.gif"></cc2:imagebutton>&nbsp;
					</td>
				</tr>
                <tr>
                    <%--pulsante cestino per il seleziona tutti--%>
                    <td align="left">
				        <cc2:imagebutton ImageAlign="Middle" id="CestinoMassiva" Runat="server" AlternateText="Rimuovi"
							    ImageUrl="../images/proto/b_elimina.gif" OnClick="RimuoviElementiSelezionati"></cc2:imagebutton>&nbsp;
                    <%--pulsante ripristina per il seleziona tutti--%>
                        <cc2:imagebutton ImageAlign="Middle" id="RipristinoMass" Runat="server" AlternateText="Ripristina"
							    ImageUrl="../images/proto/ico_risposta.gif" OnClick="RipristinaElementiSelezionati"></cc2:imagebutton>&nbsp;
                    <%--checkbox seleziona tutti--%>
                        <asp:CheckBox id="CheckBoxSelezionaTutti" runat="server" CommandName="SelectAll" Text = "Seleziona / Deseleziona Tutti" 
                        OnCheckedChanged="SelectAll" AutoPostBack="true" CssClass="testo_grigio_scuro">
                                        </asp:CheckBox>&nbsp;
					</td>
                </tr>					    
				<TR>
					<TD align="center" valign="top">
					    <!--<div class="div_Visibilita" id="divVis" style="OVERFLOW: auto; WIDTH: 667px; ">-->
                        <div class="div_Visibilita" id="divVis" style="OVERFLOW: auto; WIDTH: 740px; ">
						<asp:datagrid id="DataGrid1" Width="100%" SkinID="datagrid" runat="server" BorderColor="Gray" BorderWidth="1px"
							CellPadding="1" HorizontalAlign="Center" AllowPaging="True" AutoGenerateColumns="False" OnPreRender="DataGrid1_PreRender" OnPageIndexChanged="Datagrid1_pager" OnSelectedIndexChanged="Datagrid1_SelectedIndex" OnItemCreated="DataGrid1_ItemCreated" SelectedItemStyle-BorderStyle="Inset">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="Maroon"></HeaderStyle>
							<Columns>
                            	<asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
                                        <asp:CheckBox id="CheckBox1" CssClass='<%# DataBinder.Eval(Container, "DataItem.idCorr") + "_" + DataBinder.Eval(Container, "DataItem.diritti") + "_" + DataBinder.Eval(Container, "DataItem.TipoDiritto") %>' runat="server" CommandName="Select" AutoPostBack="true" OnCheckedChanged="verificaCheckSelectAll" Checked='<%# DataBinder.Eval(Container, "DataItem.Checked") %>'>
                                        </asp:CheckBox>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Ruolo/Utente">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox2 runat="server" CssClass="testo_grigio" Text='<%# DataBinder.Eval(Container, "DataItem.ruolo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
                                        <asp:ImageButton ID="ibRoleHistory" runat="server" Width="19px" AlternateText="Mostra storia ruolo"
							                ImageUrl="../images/proto/storia.gif" Visible='<%# DataBinder.Eval(Container, "DataItem.ShowHistory") %>'
                                            OnClientClick='<%# DocsPAWA.popup.RoleHistory.GetScriptToOpenWindow(DataBinder.Eval(Container, "DataItem.IdCorrGlobbRole").ToString(), String.Empty) %>' />
									</ItemTemplate>
									<EditItemTemplate>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Diritti">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.diritti") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox3 runat="server" CssClass="testo_grigio" Text='<%# DataBinder.Eval(Container, "DataItem.diritti") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Dettagli">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="ImageButton1" runat="server" BorderWidth="1px" CssClass="testo_grigio" CommandName="Select"
											ImageUrl="../images/proto/dettaglio.gif"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="False">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemTemplate>
										<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codiceRub") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.codiceRub") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="tipo">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id=Label5 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox4 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="dataFine Ruolo" Visible=False>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label6" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dtaFine") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dtaFine") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>                               
								<asp:TemplateColumn HeaderText="Rimuovi">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="btn_Rimuovi" runat="server" CssClass="testo_grigio" CommandName="Rimuovi"  AlternateText='<%# DataBinder.Eval(Container, "DataItem.note") %>'
											ImageUrl="../images/proto/b_elimina.gif"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="rimosso" Visible=False>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.rimosso") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.rimosso") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="note" Visible=False>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.note") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.note") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="idCorr" Visible=False>
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idCorr") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.idCorr") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
                                <asp:BoundColumn HeaderText="Tipo diritto" DataField="TipoDiritto">
                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Note Acquisizione" Visible="true">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.noteSecurity") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.noteSecurity") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Data Diritto/Revoca" Visible="true">
									<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:Label id="Label11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dtaInsSecurity") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id="Textbox11" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dtaInsSecurity") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>                                
							</Columns>
							<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
						</div>
					</TD>
				</TR>				
				<TR>
					<TD height="10"></TD>
				</TR>
				<tr>
				    <td>
				        <asp:panel id="pnl_descRimuovi" Runat="server" visible=false>
				            <TABLE id="Table2" class="info" width="100%" align="center" border="0" >
				                <TR>
					                <td class="item_editbox">
						                <P class="boxform_item">
							                Rimuovi Visibilita'</P>
					                </td>
				                </TR>
				                <TR>
					                <TD vAlign="middle" align="center">
						                <asp:label id="lbl_result" runat="server" CssClass="testo_red"></asp:label></TD>
				                </TR>
				                <TR>
				                    <TD vAlign="middle">&nbsp;&nbsp;
				                        <asp:label id="lbl_note" valign="top" runat="server" CssClass="titolo_scheda">Note</asp:label>&nbsp;
						                <asp:textbox  id="txt_note" runat="server" Width="520px" CssClass="testo_grigio" TextMode="MultiLine" onkeyup="calcTesto(this,'128')" MaxLength="5"></asp:textbox>			
				                    </TD>
				                </TR>
				                <TR height="40">
					                <TD vAlign="middle" align="center" height="40">
						                <asp:button id="btn_okDesc" runat="server" CssClass="PULSANTE" Text="OK"></asp:button>&nbsp;
						                <asp:button id="btn_annulla" runat="server" CssClass="PULSANTE" Text="ANNULLA"></asp:button>
					                </TD>
				                </TR>
				            </TABLE>
				        </asp:panel>
				    </td>
				</tr>
				<TR>
					<TD align="center" id="dettaglio" runat="server">
						<DIV style="OVERFLOW: auto; HEIGHT: 49px; background-color: #F4F4F4" id="DIV1">
							<asp:label id="lb_dettagli" runat="server" CssClass="testo_grigio" Visible="False"></asp:label>
						</DIV>
					</TD>
				</TR>
				<TR>
					<TD height="2"></TD>
				</TR>
				<TR>
					<TD align="center"><asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Text="Chiudi"></asp:button></TD>
				</TR>
				<TR>
					<TD height="2"></TD>
				</TR>
					<TR >
					    <cc1:messagebox id="Msg_Rimuovi" runat="server"></cc1:messagebox>
					</TR>
				
					<TR >
					    <cc1:messagebox id="Msg_Ripristina" runat="server"></cc1:messagebox>
					</TR>
				
			</TABLE>
		</form>
	</body>
</HTML>

