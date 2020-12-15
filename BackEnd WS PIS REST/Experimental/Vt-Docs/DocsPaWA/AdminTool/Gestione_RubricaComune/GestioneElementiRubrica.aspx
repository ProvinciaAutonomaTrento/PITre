<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneElementiRubrica.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_RubricaComune.GestioneElementiRubrica" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../UserControl/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register tagprefix="uc3" src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider"  %>
<%@ Register TagPrefix="uc4" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc5" Src="../../waiting/WaitingPanel.ascx" TagName="WaitingPanel" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script type="text/javascript" src="../CSS/caricamenujs.js"></script>
	<title runat = "server"></title>
    <script type ="text/javascript">
	    function apriPopup() 
	    {
			hlp = window.open('../help.aspx?from=FD','','width=450,height=500,scrollbars=YES');
		}				
		function cambiaPwd() {
			cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
		}	
		function ClosePopUp()
		{	
			if(typeof(cambiapass) != 'undefined')
			{
				if(!cambiapass.closed)
					cambiapass.close();
			}
			if(typeof(hlp) != 'undefined')
			{
				if(!hlp.closed)
					hlp.close();
			}				
		}

		function verificaCF() {
		    var cf = document.getElementById('txtCodiceFiscale').value;
		    if (cf.length == 16)
		        return confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?');

		    return true;
		}

        function ShowMessage(message) { alert(message); }
        function ConfirmDelete() { return confirm("Si conferma la rimozione dell'elemento dalla rubrica?"); }
        function SetFocus(ctlId) {
            var ctl = document.getElementById(ctlId); if (ctl != null && ctl.style.visibility != '') ctl.focus();
        }
    </script> 
</head>
<body>
    <form id="frmGestioneElementiRubrica" runat="server">
        <asp:ScriptManager ID="scrManager" runat="server" />
        <uc3:AppTitleProvider ID="titleProvider" runat="server" PageName="Gestione Rubrica Comune" />
        <uc2:menutendina id="menuTendina" runat="server"></uc2:menutendina>
        <uc5:WaitingPanel ID="waitingPanel" runat="server" />
	    <table cellSpacing="1" cellPadding="0" width="100%" border="0">
            <tr>
	            <td colspan="2">
			        <uc1:testata id="Testata" runat="server"></uc1:testata>
			     </td>
		    </tr>
		    <tr>
				<td colspan="2" class="testo_grigio_scuro" bgColor="#c0c0c0" height="20">
				    <asp:label id="lbl_position" runat="server"></asp:label>
				</td>
			</tr>
			<tr>
				<td colspan="2" class="titolo" style="HEIGHT: 20px" align="center" bgColor="#e0e0e0" height="34">
                    Gestione Rubrica Comune
                </td>
			</tr>
			<tr style="height: 5px">
                <td colspan="2"></td>
            </tr>
            <tr>
                 <td align="center"  colspan="2">
                    <table width="85%" border = "0" cellpadding = "0" cellspacing = "0">
                        <tr>
                            <td vAlign="top" colspan = "2" align = "left"  class="pulsanti">
			                    <asp:Label ID = "lblOggettiVisualizzati" runat = "server" CssClass="titolo"></asp:Label>
                            </td>
                        </tr>
		                <tr>
		                    <td valign = "top" align = "left" width = "90%" class="pulsanti">
		                        <asp:DropDownList ID = "cboFiltri" runat = "server" AutoPostBack="true" 
                                    Width = "100px" CssClass="testo" 
                                    onselectedindexchanged="cboFiltri_SelectedIndexChanged">
                                    <asp:ListItem Value = "" Text = "Tutti"></asp:ListItem>    
                                    <asp:ListItem Value = "Codice" Text = "Codice" Selected = "True"></asp:ListItem>    
                                    <asp:ListItem Value = "Descrizione" Text = "Descrizione"></asp:ListItem>    
                                    <asp:ListItem Value = "Citta" Text = "Città"></asp:ListItem>    
                                </asp:DropDownList>
		                        <asp:TextBox ID = "txtFiltro" runat = "server" Width = "200px" CssClass="testo"></asp:TextBox>
		                        <asp:button id="btnFiltro" runat="server" CssClass="testo_btn_p" Text="Cerca" 
                                    onclick="btnFiltro_Click" Width="50px" />
		                    </td>
			                <td vAlign="top" align="right" style="height: 19px;" width = "10%" class="pulsanti">
                                <asp:button id="btnInsert" runat="server" CssClass="testo_btn_p" Text="Nuovo" 
                                    onclick="btnInsert_Click"></asp:button>
                            </td>
                        </tr>                           
                    </table>
                    <div id="divGrdElementiRubrica" style="OVERFLOW: auto; WIDTH: 100%; HEIGHT: 200px">
	                    <asp:datagrid id="grdElementiRubrica" runat="server" 
                            AutoGenerateColumns="False" 
                            BorderColor="Gray"
					        CellPadding="1"
					        BorderWidth="1px" 
					        onitemcommand="grdElementiRubrica_ItemCommand" 
					        AllowCustomPaging="True" 
                            AllowPaging="True" 
                            Width="85%"
                            PageSize="6" onpageindexchanged="grdElementiRubrica_PageIndexChanged" 
                            onitemcreated="grdElementiRubrica_ItemCreated"
                            onsortcommand="grdElementiRubrica_SortCommand" >
					        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
					        <PagerStyle Mode="NumericPages" CssClass="bg_grigioN" />
					        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					        <ItemStyle CssClass="bg_grigioN"></ItemStyle>
					        <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                            <Columns>
                                <asp:BoundColumn DataField="Id" HeaderText="Id" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Codice" HeaderText="Codice">
                                    <HeaderStyle Width="20%" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Left" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
                                    <HeaderStyle Width="50%" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Left" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="UtenteCreatore" HeaderText="Creato da" 
                                    Visible="False">
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="DataCreazione" HeaderText="Creato il" DataFormatString="{0:d}">
                                    <HeaderStyle Width="10%" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="DataUltimaModifica" HeaderText="Mod. il" 
                                    DataFormatString="{0:d}">
                                    <HeaderStyle Width="10%" />
                                </asp:BoundColumn>
                                <asp:TemplateColumn>
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
			                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
			                        <ItemTemplate>
				                        <uc4:imagebutton id="btnEditItem" runat="server" CommandName="EDIT_COMMAND"
                                            ImageUrl="../../images/proto/dett_lente.gif" 
                                            ToolTip="Seleziona elemento" />
			                        </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
			                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
			                        <ItemTemplate>
				                        <uc4:imagebutton id="btnDeleteItem" runat="server" CommandName="DELETE_COMMAND"
                                            ImageUrl="../Images/cestino.gif" ToolTip="Cancella elemento" OnClientClick = "return ConfirmDelete()" />
			                        </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                         </asp:datagrid>
                    </div>
                </td>
            </tr>	
			<tr>
			    <td colspan="2"></td>
			</tr>
			<tr>
			    <td colspan="2" align = "center">
			        <table id="tblDetail" cellpadding="0" cellspacing="5" class="contenitore" width="85%" runat="server">
                        <tr>
                            <td colspan="4" style="height: 5px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" class="titolo_pnl">
                                            <asp:Label ID="lblPanelTitle" runat="server">Elemento Rubrica</asp:Label>
                                        </td>
                                        <td align="right" class="titolo_pnl">
                                            <asp:ImageButton id="btnClosePanel" tabIndex="7" runat="server" ToolTip="Chiudi" OnClick = "btnClosePanel_OnClick" ImageUrl="../Images/cancella.gif"></asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Codice: *
                            </td>
                            <td align="left" style="width: 690px; color: #666666; height: 20px">
                                <asp:TextBox ID="txtCodice" runat="server" CssClass="testo" Width="180px" MaxLength = "50"></asp:TextBox>
                            </td>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Telefono:
                            </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                                    <asp:TextBox ID="txtTelefono" runat="server" 
                                    CssClass="testo" Width="200px" MaxLength="255"></asp:TextBox>
                            </td>                          
                        </tr>
                        <tr>
                            <td align="right"  class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Descrizione: *</td>
                            <td align="left" style="width: 690px; color: #666666; height: 20px">
                                <asp:TextBox ID="txtDescrizione" runat="server" CssClass="testo" Width="300px" 
                                    MaxLength="255"></asp:TextBox></td>
                                <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Fax:
                                </td>
                                <td align="left" style="width: 612px; height: 20px" valign="middle">
                                                    <asp:TextBox ID="txtFax" runat="server" 
                                    CssClass="testo" Width="200px" MaxLength="20"></asp:TextBox>
                                </td>      
                            </tr>     
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Cod. Fiscale:
                            </td>
                            <td align="left" style="width: 690px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtCodiceFiscale" runat="server" 
                                    CssClass="testo" Width="180px" MaxLength="16"></asp:TextBox>
                            </td>    
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Partita Iva:
                            </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                 <asp:TextBox ID="txtPartitaIva" runat="server" 
                                    CssClass="testo" Width="200px" MaxLength="11"></asp:TextBox>
                            </td>                          
                        </tr>                              
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Indirizzo:
                            </td>
                            <td align="left" style="width: 690px; height: 20px">
                                                    <asp:TextBox ID="txtIndirizzo" runat="server" 
                                    CssClass="testo" Width="300px" MaxLength="255"></asp:TextBox>
                            </td>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Codice Amm:</td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtAmministrazione" runat="server" 
                                    CssClass="testo" Width="200px" MaxLength="100"></asp:TextBox>
                            </td>               
                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Città:
                            </td>
                            <td align="left" style="width: 690px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtCitta" runat="server" 
                                    CssClass="testo" Width="180px" MaxLength="50"></asp:TextBox>
                            </td>  
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                &nbsp;Codice AOO:
                                </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtAOO" runat="server" 
                                    CssClass="testo" Width="200px" MaxLength="100"></asp:TextBox>
                            </td>                          

                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Provincia:
                            </td>
                            <td align="left" style="width: 690px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtProvincia" runat="server" 
                                    CssClass="testo" Width="30px" MaxLength="2"></asp:TextBox>
                            </td>    
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Tipo:
                            </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                <asp:DropDownList ID="ddlCorrType" runat="server"
                                    CssClass="testo" Width="200px">
                                    
                                </asp:DropDownList>    
                            </td>                          
                        </tr>                        
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Cap:
                            </td>
                            <td align="left" valign="middle" width="690">
                                <asp:TextBox ID="txtCap" runat="server" 
                                    CssClass="testo" Width="50px" MaxLength="5"></asp:TextBox>
                            </td>   
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                <asp:Label ID="lblUrl" runat="server" >Url:</asp:Label>
                            </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtUrl" runat="server"
                                    CssClass="testo" Width="200px" MaxLength="4000"></asp:TextBox>
                            </td>                       
                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                Nazione:
                            </td>
                            <td align="left" style="width: 690px; height: 20px" valign="middle">
                                <asp:TextBox ID="txtNazione" runat="server" 
                                    CssClass="testo" Width="180px" MaxLength="50"></asp:TextBox>
                            </td>                          
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                &nbsp;</td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                &nbsp;</td>       
                        </tr>
                        <%-- Gestione multicasella --%>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                <asp:Label ID="lblEmail" runat="server" >Email:</asp:Label>
                            </td>
                            <td align="left" style="width: 690px; height: 20px" valign="middle">
                                <asp:UpdatePanel ID="updPanel1" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtEmail" runat="server" CssClass="testo" Width="300px"></asp:TextBox>
                                        <asp:ImageButton AlternateText="Aggiungi casella di posta" ImageAlign="AbsMiddle" OnClick="imgAggiungiCasella_Click"
                                            ID="imgAggiungiCasella" runat="server" ToolTip="Aggiungi casella di posta" ImageUrl="~/images/proto/aggiungi.gif" />
                                     </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>              
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 20%">
                                <asp:Label ID="lblNote" runat="server" >Note E-mail:</asp:Label>
                            </td>
                            <td align="left" style="width: 612px; height: 20px" valign="middle">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtNote" runat="server" CssClass="testo" Width="200px" MaxLength="20"></asp:TextBox>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="left">
                                <asp:UpdatePanel ID="updPanelMail" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div id="divGridViewCaselle" runat="server" style="width:100%;overflow-y:scroll;overflow-x:hidden;padding-left:20%; max-height:80px;">
                                            <asp:GridView  ID="gvCaselle" runat="server" Width="80%" AutoGenerateColumns="False" OnRowDataBound="gvCaselle_RowDataBound"
                                                CellPadding="1" BorderWidth="1px" BorderColor="Gray"
                                                >
                                                <SelectedRowStyle CssClass="bg_grigioS"></SelectedRowStyle>
                                                <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                                                <RowStyle CssClass="bg_grigioN"></RowStyle>
                                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>                                                                                      
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Email" ShowHeader="true" >
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="70%"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:TextBox AutoPostBack="true" OnTextChanged="txtEmailCorr_TextChanged" CssClass="testo" style="width:98%;margin:1px;" ID="txtEmailCorr" runat="server" Text ='<%# ((RubricaComune.Proxy.Elementi.EmailInfo)Container.DataItem).Email %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                            <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle" Width="20%"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:TextBox AutoPostBack="true" MaxLength="20" CssClass="testo" OnTextChanged="txtNoteMailCorr_TextChanged" style="width:96%;" ID="txtNoteMailCorr" runat="server" Text ='<%# ((RubricaComune.Proxy.Elementi.EmailInfo)Container.DataItem).Note %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="*" ShowHeader="true" >
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:RadioButton ID="rdbPrincipale" runat="server" AutoPostBack="true" OnCheckedChanged="rdbPrincipale_ChecekdChanged" Checked='<%# (((RubricaComune.Proxy.Elementi.EmailInfo)Container.DataItem).Preferita) %>' />
                                                            </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    <asp:TemplateField HeaderText="" ShowHeader="true">
                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></ItemStyle>
                                                            <ItemTemplate>
                                                                <asp:ImageButton ID="imgEliminaCasella" runat="server"  OnClick="imgEliminaCasella_Click" AutoPostBack="true" ImageUrl="~/images/proto/cancella.gif" />
                                                            </ItemTemplate>
                                                    </asp:TemplateField> 
                                                    </Columns>
                                            </asp:GridView>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="imgAggiungiCasella" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            
                            </td>
                        </tr>    

                        <tr>
                            <td  style="height: 20px; width:20%;" align="right" class="testo_grigio_scuro">
                                &nbsp;
                            </td>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 690px">
                                &nbsp;
                            </td>
                            <td align="left" style="width: 20%; height: 20px" valign="middle">
                                &nbsp;
                            </td>       
                           <td align="left" style="width: 612px; height: 20px" valign="middle">
                                &nbsp;
                           </td>
                        </tr>      
                                        
                        <tr align="center">
                            <td colspan="4">
                                <asp:button id="btnSave" runat="server" CssClass="testo_btn_p" Text="Salva"  OnClientClick = "return verificaCF();" 
                                     OnClick = "btnSave_Click"></asp:button>&nbsp;
                                <asp:button id="btnClose" runat="server" CssClass="testo_btn_p" Text="Chiudi" OnClick = "btnClosePanel_OnClick"></asp:button>
                            </td>
                        </tr>
                    </table>
                </td>
			</tr>                                                    

		</table>	
    </form>
</body>
</html>