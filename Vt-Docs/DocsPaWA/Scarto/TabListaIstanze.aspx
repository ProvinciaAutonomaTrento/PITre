<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="TabListaIstanze.aspx.cs" Inherits="DocsPAWA.Scarto.TabListaIstanze" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA > TabListaIstanze</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="javascript">
			function StampaRisultatoRicerca()
			{		
				var args=new Object;
				args.window=window;
				window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=scarto",
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
			}			
		</script>
	</HEAD>
	<body text="#660066" bottomMargin="0" vLink="#ff3366" aLink="#cc0066" link="#660066" leftMargin="0"
		topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="TabListaIstanze" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="100%" align="center">
				
				<tr id="trHeader" runat="server">
					<td class="pulsanti">
						<table width="100%">
							<tr id="tr1" runat="server">
								<td id="Td2" align="left" height="80%" runat="server">
								    <asp:label id="titolo" CssClass="titolo" Runat="server">Lista istanze di scarto</asp:label>
								</td>
                        	</tr>
						</table>
					</td>
				</tr>
				
                <tr><td height="5"></td></tr>
                <tr>
                    <td>
                        <table class="info" runat="server" width="100%">
                            <tr>
                                <td>&nbsp; <asp:Label CssClass="testo_grigio_scuro" ID="lbl_descrizione" runat="server" Text="Inserisci descrizione *"></asp:Label></td>
                                <td align=left><asp:TextBox ID="txt_descrizione" runat="server" Width="440px" CssClass="testo_grigio" Height="18px" MaxLength="250"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>&nbsp; <asp:Label CssClass="testo_grigio_scuro" ID="lbl_note" runat="server" Text="Inserisci note"></asp:Label></td>
                                <td align=left><asp:TextBox ID="txt_note" runat="server" CssClass="testo_grigio" Width="440px" Height="18px" MaxLength="500"></asp:TextBox></td>                                    
                            </tr>
                            <tr id="ric_aut_lbl" runat="server" visible="false">
                                <td class="testo_grigio_scuro">&nbsp; Estremi richiesta</td>
                                <td align=left><asp:Label ID="lbl_estr_richiesta" runat="server" CssClass="testo_grigio" Width="440px" Height="18px"></asp:Label></td>                                    
                            </tr>
                            <tr id="autor_lbl" runat="server" visible="false">
                                <td class="testo_grigio_scuro">&nbsp; Estremi provvedimento</td>
                                <td align=left><asp:Label ID="lbl_estr_autorizzazione" runat="server" CssClass="testo_grigio" Width="440px" Height="18px"></asp:Label></td>                                    
                            </tr>
                             <tr id="dataScarto" runat="server" visible="false">
                                <td class="testo_grigio_scuro">&nbsp; Data scarto</td>
                                <td align=left><asp:Label ID="lbl_data_scarto" runat="server" CssClass="testo_grigio" Width="440px" Height="18px"></asp:Label></td>                                    
                            </tr>
                            <tr>
                                <td colspan=2 align= right><asp:button id="btn_salva" runat="server" CssClass="pulsante" Width="80" Text="Salva"></asp:button>&nbsp;&nbsp;</TD>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr><td height="5"></td></tr>
                <asp:Panel ID="pnl_statiOp" runat="server" Visible="false">
                <tr>
                    <td>
                        <table id="Table1" class="info" runat="server" width="100%" border=0>
                            <tr>
                                <td valign=top class="testo_grigio_scuro" width="20px">&nbsp; Stato </td>
                                <td valign=bottom align="left" width="165px"><asp:label id="lbl_stato" CssClass="testo_grigio" Runat="server" Width="165px" AutoPostBack="True"></asp:label></td>    
                                <td valign=top class="testo_grigio_scuro" width="40px" id="op" runat=server>&nbsp; Operazione </td>
                                <td valign=bottom align="left" width="280px">
                                    <asp:DropDownList ID="ddl_operazioni" runat="server" CssClass="testo_grigio" Width="280px" AutoPostBack="True"></asp:DropDownList>
                                    </td>    
                                
                            </tr>
                            <tr><td height="4"></td></tr>
                            <tr id="report" runat=server visible=false>
                                <td colspan="4" class="testo_grigio_scuro" vAlign="middle" align="right">
                                    <asp:imagebutton id="btn_report" Runat="server" AlternateText="Genera report" ImageUrl="../images/proto/export.gif"></asp:imagebutton>&nbsp;&nbsp;
								</td>
                            </tr>
                            <tr id="richAut" runat="server" visible="false">
                                <td class="testo_grigio_scuro" colspan=4>Estremi richiesta di autorizzazione * &nbsp;
                                    <asp:TextBox ID=txt_ricAut runat=server CssClass="testo_grigio" width="266px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="regAut" runat="server" visible="false">
                                <td class="testo_grigio_scuro" colspan=4>Estremi del provvedimento di autorizzazione *
                                    <asp:TextBox ID="txt_autor" runat="server" CssClass="testo_grigio" width="266px"></asp:TextBox>
                                </td>
                            </tr>
                            
                            <tr>
                                <td colspan="4" align="right"><asp:button id="btn_esegui" runat="server" CssClass="pulsante" Width="60" Text="Esegui"></asp:button>&nbsp;&nbsp;</TD>
                            </tr>
                        </table>
                    </td>    
                </tr>  
                </asp:Panel>      
                <tr><td height="5"></td></tr>
                <tr>
                    <td height="2"> <asp:Label class="testo_red" ID="lbl_messaggio" runat="server" Visible=false ></asp:Label>
                    </td>
                </tr>
				<tr id="trBody" runat="server">
					<td vAlign="middle">
					    <asp:datagrid id="dg_fasc" runat="server" SkinID="datagrid" AllowCustomPaging="True" BorderStyle="Inset" AutoGenerateColumns="False"
							Width="100%" CellPadding="1" BorderWidth="1px" BorderColor="Gray" HorizontalAlign="Center" AllowPaging="True">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle"></ItemStyle>
							<HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg"></HeaderStyle>
							<Columns>
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
										<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Label3">
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.CodClass") %>' ID="Textbox3">
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Codice" HeaderText="Codice">
									<HeaderStyle Width="10%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn SortExpression="Descrizione" HeaderText="Descrizione">
									<HeaderStyle Width="50%"></HeaderStyle>
									<ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
									<ItemTemplate>
										<asp:Label ID="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
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
								<asp:TemplateColumn HeaderText="Mesi cons.">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiCons") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiCons") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Mesi chiusura">
									<HeaderStyle Width="10px"></HeaderStyle>
									<ItemTemplate>
										<asp:Label ID="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiChiusura") %>'>
										</asp:Label>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox ID="TextBox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.MesiChiusura") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="false" >
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="lbl_Chiave" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn HeaderText="Area scarto">
								    <HeaderStyle Wrap="false" HorizontalAlign="center" Width="5px"/>
								    <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle"/>
								    <ItemTemplate>
								        <cc1:ImageButton ID="btn_elimina" runat="server"  ImageUrl="../images/proto/cancella.gif" 
								        CommandName="EliminaAreaScarto" AlternateText="Elimina questo fascicolo in 'Area di scarto'"/> 
								    </ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn Visible="false" HeaderText="in scarto">
								    <HeaderStyle Wrap="false"  Width="5px"/>
								    <ItemTemplate>
								        <asp:Label ID="lbl_inScarto" runat="server"  Text='<%# DataBinder.Eval(Container, "DataItem.InScarto") %>'>
								        </asp:Label>
								    </ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" Position="TopAndBottom" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
								Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr>
					<td><cc2:messageBox id="MsgBox_Scarta" runat="server"></cc2:messagebox>
                </td></tr>
								    
			</table>
			
        </form>
	</body>
</HTML>


