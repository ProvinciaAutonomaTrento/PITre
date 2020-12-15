<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneContatore.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_ProfDinamica.GestioneContatore" %>
<%@ Register TagPrefix="MB" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat = "server">
    <title></title>
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet" />
    <base target="_self" />

    <script language="javascript">
        function ValidateNumKey() {
            var inputKey = event.keyCode;
            var returnCode = true;
            if (inputKey > 47 && inputKey < 58) {
                return;
            }
            else {
                returnCode = false; event.keyCode = 0;
            }
            event.returnValue = returnCode;
        }        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione Contatori Tipologia Documenti" />

    <table width="100%">
		<tr>
			<td class="titolo"  align="center" bgColor="#e0e0e0" height="20">
				<asp:Label id="lbl_titolo" runat="server"></asp:Label>				
			</td>
			<td align="right" bgColor="#e0e0e0" width="30%" style="padding-right:13px;">
			    <asp:Button id="btnChiudi" runat="server" Text="Chiudi" CssClass="testo_btn_p_large" OnClientClick="window.close();"></asp:Button>             
			</td>
		</tr>
		<tr>
		    <td colspan="2" style="height:10px; border-bottom:2px #810D06 solid; text-align:center;">
                <p class="testo_grigio_scuro_grande">Il valore che assumerà il contatore, è il numero successivo a quello inserito.</p>
            </td>
		</tr>
        <tr>
            <td colspan="2">
                <p class="testo_grigio_scuro_grande">Elenco contatori in uso :</p>
                <div id="div_listaContatori" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 225px; width:100%; border:1px #810D06 solid;">
                <asp:DataGrid id="dgContatori" runat="server" AutoGenerateColumns="False" Width="100%" OnItemCommand="dgContatori_OnItemCommand">
			        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
			        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					<ItemStyle CssClass="bg_grigioN"></ItemStyle>
					<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                    <Columns>
						<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SYSTEM_ID" ReadOnly="true"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="ID_OGG" HeaderText="ID_OGG" ReadOnly="true"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="ID_TIPOLOGIA" HeaderText="ID_TIPOLOGIA" ReadOnly="true"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="ID_AOO" HeaderText="ID_AOO" ReadOnly="true"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="ID_RF" HeaderText="ID_RF" ReadOnly="true"></asp:BoundColumn>
                        <asp:BoundColumn Visible="False" DataField="ABILITATO" HeaderText="ABILITATO" ReadOnly="true"></asp:BoundColumn>

                        <asp:BoundColumn DataField="ANNO" HeaderText="ANNO" ReadOnly="true">
                            <HeaderStyle Width="10%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        
                        <asp:BoundColumn DataField="CODICE_RF_AOO" HeaderText="COD. RF/AOO" ReadOnly="true">
                            <HeaderStyle Width="15%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>
                        
                        <asp:BoundColumn DataField="DESC_RF_AOO" HeaderText="DESC. RF/AOO" ReadOnly="true">
                            <HeaderStyle Width="45%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
                        </asp:BoundColumn>

                        <asp:TemplateColumn HeaderText="CONTATORE">
                            <HeaderStyle Width="10%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
                            <EditItemTemplate>
                                <asp:TextBox ID="txt_valore" runat="server" CssClass="testo" onKeyPress="ValidateNumKey();" MaxLength="8" Text='<%# DataBinder.Eval(Container, "DataItem.VALORE") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_valore" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.VALORE") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:TemplateColumn HeaderText="SOTTOCONTATORE">
                            <HeaderStyle Width="10%"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
                            <EditItemTemplate>
                                <asp:TextBox ID="txt_valoreSc" runat="server" CssClass="testo" onKeyPress="ValidateNumKey();" MaxLength="8" Text='<%# DataBinder.Eval(Container, "DataItem.VALORE_SC") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lbl_valoreSc" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.VALORE_SC") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>

                        <asp:EditCommandColumn ButtonType="LinkButton" CancelText="Annulla" EditText="Modifica" UpdateText="Salva">
                            <HeaderStyle Width="10%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Left" />
                        </asp:EditCommandColumn>
                        
                        <asp:ButtonColumn HeaderText="Elimina" CommandName="Delete" Text="&lt;img src='../Images/cestino.gif' border='0' /&gt;">
                        	<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center" CssClass="img_delete"></ItemStyle>                                                        
						</asp:ButtonColumn>
					</Columns>
				</asp:DataGrid>
                </div>                
            </td>
        </tr>
        <asp:Panel ID="pnl_addContatori_RF" Visible="false" runat="server">
        <tr>
            <td style="width:30%;">
                <p class="testo_grigio_scuro_grande">Aggiungi un contatore di RF :</p>                         
            </td>
            <td style="width:70%;">
                <input id="txt_systemIdRF" name="txt_systemIdRF" type="hidden" runat="server" />
                <%--<asp:TextBox ID="txt_systemIdRF" runat="server" Visible="false"></asp:TextBox>--%>
                <asp:TextBox ID="txt_codRF" runat="server" CssClass="testo" Width="10%" OnTextChanged="txt_codRF_OnTextChanged" AutoPostBack="true"></asp:TextBox>
                <asp:TextBox ID="txt_descRF" runat="server" CssClass="testo" Width="80%" ReadOnly="true" BackColor="Control"></asp:TextBox>
                <asp:ImageButton ID="btn_addContatori_RF" runat="server" ImageUrl="../Images/aggiungi.gif" ToolTip="Aggiungi contatore" OnClick="btn_addContatore_Click"/>
            </td>
        </tr>        
        </asp:Panel>        
        <asp:Panel ID="pnl_addContatori_AOO" Visible="false" runat="server">
        <tr>
            <td style="width:30%;">
                <p class="testo_grigio_scuro_grande">Aggiungi un contatore di AOO :</p>                         
            </td>
            <td style="width:70%;">
                <input id="txt_systemIdAoo" name="txt_systemIdAoo" type="hidden" runat="server" />
                <%--<asp:TextBox ID="txt_systemIdAoo" runat="server" Visible="false"></asp:TextBox>--%>
                <asp:TextBox ID="txt_codAoo" runat="server" CssClass="testo" Width="10%" OnTextChanged="txt_codAoo_OnTextChanged" AutoPostBack="true"></asp:TextBox>
                <asp:TextBox ID="txt_descAoo" runat="server" CssClass="testo" Width="80%" ReadOnly="true" BackColor="Control"></asp:TextBox>
                <asp:ImageButton ID="btn_addContatori_AOO" runat="server" ImageUrl="../Images/aggiungi.gif" ToolTip="Aggiungi contatore" OnClick="btn_addContatore_Click"/>
            </td>
        </tr>        
        </asp:Panel>        
        <asp:Panel ID="pnl_addContatori_T" Visible="false" runat="server">
        <tr>
            <td style="width:30%;">
                <p class="testo_grigio_scuro_grande">Aggiungi un contatore di tipologia :</p>                         
            </td>
            <td style="width:70%;">
                <asp:ImageButton ID="btn_addContatori_T" runat="server" ImageUrl="../Images/aggiungi.gif" ToolTip="Aggiungi contatore" OnClick="btn_addContatore_Click"/>
            </td>
        </tr>        
        </asp:Panel>        
    </table>

    <!--Message Box-->
    <MB:messagebox id="msg_Elimina" runat="server"></MB:messagebox>

    </form>
</body>
</html>
