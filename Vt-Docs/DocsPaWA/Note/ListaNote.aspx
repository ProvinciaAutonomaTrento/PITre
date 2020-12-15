<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaNote.aspx.cs" Inherits="DocsPAWA.Note.ListaNote" %>
<%@ Register src="DettaglioNota.ascx" tagname="DettaglioNota" tagprefix="uc1" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Note</title>
    <base target = "_self" />    
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <script type ="text/javascript">
        function CloseDialog() { window.returnValue = document.getElementById('txtReturnValue').value; window.close();}
        function ConfirmDelete() { return confirm("Cancellare la nota selezionata?"); }
        function SetFocus(controlId) { try { document.getElementById(controlId).focus(); } catch (e) {} }
    </script> 
</head>
    <body>
        <form id="frmNote" runat="server">
            <div align = "center" runat = "server" style="overflow:auto; height:100%; width:100%" >
                <input type = "hidden" id = "txtReturnValue" runat = "server" value = "false" />
                <table class="contenitore" id="tblContainer" style ="width: 100%; height: 100%">
                    <tr>
                        <td class="item_editbox">
						    <p class="boxform_item">
							    <asp:Label id="Label1" runat="server">Note utente o di ruolo</asp:Label>
						    </p>
					    </td>
                    </tr>
                    <tr>
                        <td></td>
                    </tr>
                    <tr>
                        <td>
                            <table class="info_grigio" id="tblGrdNote" width="95%" height = "100%" >
                                <tr>
                                    <td>
                                        <asp:Label ID = "lblMessaggi" runat = "server" CssClass = "testo_grigio_scuro_grande" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div align ="center" style ="overflow:auto; height: 280px" id="divGrdNote" >
                                            <asp:datagrid id="grdNote" runat="server" SkinID="datagrid" Width="95%" BorderWidth="1px" 
                                                BorderColor="Gray" CellPadding="1"
                                                    AutoGenerateColumns="False" BorderStyle="Inset" 
                                                OnItemCommand="grdNote_ItemCommand" TabIndex="5" >
                                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                                    <PagerStyle Mode="NumericPages" />
                                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                                    <ItemStyle Height="20px" CssClass="bg_grigioN"></ItemStyle>			
                                                    <Columns>
                                                        <asp:BoundColumn DataField="Id" HeaderText="Id" Visible="False">
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="Testo" HeaderText="Note">
                                                            <HeaderStyle Width="40%" />
                                                            <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Left" />
                                                        </asp:BoundColumn>
                                                        <asp:TemplateColumn HeaderText="Utente">
                                                            <ItemTemplate>
                                                                <asp:Label ID = "lblUtente" runat = "server" Text = '<%# this.GetDescrizioneUtenteCreatore((DocsPAWA.DocsPaWR.InfoNota) Container.DataItem) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateColumn>
                                                        <asp:BoundColumn DataField="DataCreazione" HeaderText="Data" DataFormatString="{0:d}">
                                                            <HeaderStyle Width="15%" />
                                                        </asp:BoundColumn>
                                                        <asp:BoundColumn DataField="TipoVisibilita" HeaderText="Visibilità">
                                                            <HeaderStyle Width="10%" />
                                                        </asp:BoundColumn>
                                                        <asp:TemplateColumn>
                                                            <ItemTemplate>
                                                                <cc1:ImageButton ID="btnDelete" runat="server" ImageAlign = "Middle" ToolTip = "Rimuovi nota" OnClientClick = "return ConfirmDelete()" ImageUrl="../Images/Proto/cancella.gif" CommandName="Delete" Visible = <%# this.CanDeleteNota((DocsPAWA.DocsPaWR.InfoNota) Container.DataItem) %> />
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="5%" />
                                                        </asp:TemplateColumn>
                                                        <asp:TemplateColumn>
                                                            <ItemTemplate>
                                                                <cc1:ImageButton ID="btnSelect" runat="server" CommandName="Select"  ImageAlign = "Middle" ImageUrl = "../images/proto/ico_riga.gif" ToolTip = "Seleziona nota" />
                                                            </ItemTemplate>
                                                            <HeaderStyle Width="5%" />
                                                        </asp:TemplateColumn>
                                                    </Columns>
                                                    <HeaderStyle Wrap="False" Font-Bold="True" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            </asp:datagrid>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="100px">
                           <table class="info_grigio" id="tblNote" width="95%" >
                                <tr>
                                    <td class="testo_grigio" vAlign="middle" width = "100%" align = "center">
                                        <asp:RadioButtonList ID="rblTipiVisibilita" runat="server" 
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="testo_grigio" 
                                            TabIndex="4">
                                            <asp:ListItem Value = "Personale">Personale</asp:ListItem>
                                            <asp:ListItem Value = "Ruolo">Ruolo</asp:ListItem>
                                            <asp:ListItem Value = "RF">RF</asp:ListItem>
                                            <asp:ListItem Value = "Tutti" Selected = "True">Tutti</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="testo_grigio" vAlign="top" align="center">
                                            <asp:textbox id="txtNote" runat="server" 
                                                Width="360" Height="50px" CssClass="testo_grigio" TextMode="MultiLine"></asp:textbox>
                                    </td>
                                </tr>
                                <tr>
		                            <td colspan="2" align="right" class="testo_grigio">
			                            caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	                            </tr>
                                <tr>
                                    <td class="testo_grigio" vAlign="top" align="right">
                                        <br />
                                    </td>
                                </tr>
                            </table>   
                        </td>
                    </tr>     
                    <tr>
                        <td>
                            <br />
                            <asp:Button ID = "btnNuovo" runat = "server" TabIndex = "1" 
                                onclick="btnNuovo_Click" Text = "Nuovo" CssClass="pulsante" />
                                &nbsp;                                
                            <asp:Button ID = "btnSave" runat = "server" TabIndex = "2" 
                                onclick="btnSalva_Click" Text = "Conferma" CssClass="pulsante" />
                                &nbsp;                                
                            <asp:Button ID = "btnChiudi" runat = "server" TabIndex = "3" 
                                Text = "Chiudi" CssClass="pulsante" />
                        </td>
                    </tr>
                </table>      
            </div>
        </form>
    </body>
</html>