<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneFormati.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_FormatiDocumento.GestioneFormati" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../../waiting/WaitingPanel.ascx" TagName="WaitingPanel" TagPrefix="uc3" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
   <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
	
	
	<script type="text/javascript">
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
		
		function ValidateNumericKey()
		{
			var inputKey=event.keyCode;
			var returnCode=true;
			if(inputKey > 47 && inputKey < 58)
				return;
			else
			{
				returnCode=false; 
				event.keyCode=0;
			}
			
			event.returnValue = returnCode;
		}			
		
	    function ConfirmRemoveFileModel()
	    {
	        return confirm("Rimuovere il modello predefinito per il formato corrente?");
	    }
	    
	    function ConfirmRemoveItem(containsDefaultModel)
	    {   
	        var msg = "";
	        if (containsDefaultModel=="True")
	            msg = "Il formato documento contiene un modello di file predefinito.\n" + 
	                  "La cancellazione del formato rimuoverà anche il relativo modello.\n" + 
	                  "Rimuovere il formato selezionato?";
	        else
	            msg = "Rimuovere il formato selezionato?";
	        return confirm(msg);
	    }
	    
	    function ShowUploadFileDialog(codiceAmministrazione, fileType)
	    {
            var args=new Object;
			args.window=window;
			return window.showModalDialog('AssociaModello.aspx?codiceAmministrazione=' + codiceAmministrazione + '&fileType=' + fileType,
					args,
					'dialogWidth:410px;dialogHeight:120px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
	    }
	    
	    function SaveData()
		{
		    ShowWaitPanel("Salva formato file in corso...");
		}
		
		function SetFocus(ctlId)
		{
		    var ctl = document.getElementById(ctlId);
		    if (ctl!=null) ctl.focus();
		}
	
	</script>
    <style type="text/css">
        .style1
        {
            height: 20px;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 20px;
        }
    </style>
</head>
<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Formato documenti" />
        <!-- Gestione del menu a tendina -->
	    <uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
        <uc3:WaitingPanel ID="WaitingPanel1" runat="server" />
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
                    Gestione formato dei documenti</td>
			</tr>
			<tr style="height: 5px">
                <td colspan="2"></td>
            </tr> 						
			<tr>
			    <td vAlign="top" align="left" style="height: 19px; width: 90%" class="pulsanti">
			        <asp:Label ID="lblDescription" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
			    </td>
			    <td vAlign="top" align="right" style="height: 19px; width: 10%;" class="pulsanti">
                    <asp:button id="btnNuovoTipoFile" runat="server" CssClass="testo_btn_p" Text="Nuovo" OnClick="btnNuovoTipoFile_Click"></asp:button>
                </td>
            </tr>   
            <tr style="height: 5px">
                <td colspan="2"></td>
            </tr> 						
			<tr>
			    <td align="center" width="80%" colspan="2">
			        <div id="divGrdTipiFile" style="WIDTH: 100%; HEIGHT: 250px; overflow: auto;">
			            <asp:datagrid id="grdTipiFile" runat="server" AutoGenerateColumns="False" BorderColor="Gray"
							CellPadding="1" BorderWidth="1px" OnItemCommand="grdTipiFile_ItemCommand" OnItemCreated="grdTipiFile_ItemCreated" OnPreRender="grdTipiFile_PreRender">
							<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							<ItemStyle CssClass="bg_grigioN"></ItemStyle>
							<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                            <Columns>
                                <asp:BoundColumn DataField="SystemId" Visible="False"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Description" HeaderText="Descrizione">
                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Center" Width="60%" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Left" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="FileExtension" HeaderText="Formato">
                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Center" Width="10%" />
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Center" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="MaxFileSize" HeaderText="Dim. massima">
                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" HorizontalAlign="Left" />
                                    <HeaderStyle Width="15%" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                </asp:BoundColumn>
                                <asp:TemplateColumn HeaderText="Ammesso gestione documentale">
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
										<asp:Image id="imgFileTypeUsed" runat="server" ImageUrl="../Images/spunta_1.gif"></asp:Image>
                                    </ItemTemplate>                                    
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText=" Ammesso alla firma">
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
										<asp:Image id="imgFileTypeUsedFirma" runat="server" ImageUrl="../Images/spunta_1.gif"></asp:Image>
                                    </ItemTemplate>                                    
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Ammesso conservazione">
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
										<asp:Image id="imgFileTypeUsedConservazione" runat="server" ImageUrl="../Images/spunta_1.gif"></asp:Image>
                                    </ItemTemplate>                                    
                                </asp:TemplateColumn>
                                <asp:TemplateColumn HeaderText="Validazione conservazione">
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
										<asp:Image id="imgFileTypeUsedValidazione" runat="server" ImageUrl="../Images/spunta_1.gif"></asp:Image>
                                    </ItemTemplate>                                    
                                </asp:TemplateColumn>                                                                  
                                <asp:TemplateColumn>
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
						                <cc2:imagebutton id="btnEditItem" runat="server" Width="20px" CommandName="EDIT_ITEM" ImageUrl="../../images/proto/dett_lente.gif"
							                Height="20px" ToolTip="Modifica"></cc2:imagebutton>
					                </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
					                <ItemStyle HorizontalAlign="Center"></ItemStyle>
					                <ItemTemplate>
						                <cc2:imagebutton id="btnDeleteItem" runat="server" Width="20px" CommandName="DELETE_ITEM"  ImageUrl="../Images/cestino.gif"
							                Height="20px" ToolTip="Cancella"></cc2:imagebutton>
					                </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
			            </asp:datagrid>
			        </div>			        
			    </td>
			</tr>
			<tr style="height: 5px">
			    <td colspan="2" style="height: 47px"></td>
			</tr>
			<tr>
			    <td colspan="2" style="height: 198px">
			        <table id="tblDetail" cellpadding="0" cellspacing="5" class="contenitore"  width="100%" runat="server">
                        <tr>
                            <td colspan="2" style="height: 5px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" class="titolo_pnl">
                                            <asp:Label ID="lblPanelTitle" runat="server">Formato documento</asp:Label>
                                        </td>
                                        <td align="right" class="titolo_pnl">
                                            <asp:ImageButton id="btnClosePanel" tabIndex="7" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif" OnClick="btnClosePanel_Click"></asp:ImageButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" class="testo_grigio_scuro" style="height: 20px; width: 100%" colspan="2">
                                 Ammesso gestione documentale:
                            <%--</td>--%>
                            <%--<td align="left" style="color: #666666; " class="style1">--%>
                                <asp:CheckBox ID="chkUseFileType" runat="server" CssClass="testo" 
                                    OnCheckedChanged="chkUseFileType_CheckedChanged" AutoPostBack="True" />
                            <%--</td>--%><asp:Label id="lblFirma" runat="server" align="right" class="testo_grigio_scuro" style="height: 20px;" Text="Ammesso alla firma:"></asp:Label>
                            <%--<td align="right" class="testo_grigio_scuro" style="height: 20px;">--%>
                              <%--  Ammesso per la firma:--%>
                            <%--</td>--%>
                            <%--<td align="left" style="color: #666666; " class="style1">--%>
                                <asp:CheckBox ID="chkUseFileTypeSignature" runat="server" CssClass="testo" OnCheckedChanged="chkUseFileTypeSignature_CheckedChanged" />
                           <%--</td>--%><asp:Label id="lblConservazione" runat="server" align="right" class="testo_grigio_scuro" style="height: 20px;" Text="Ammesso conservazione:"></asp:Label>
                            <%--<td align="right" class="testo_grigio_scuro" style="height: 20px;">--%>
                              <%--  Ammesso per la conservazione:--%>
                            <%--</td>--%>
                            <%--<td align="left" style="color: #666666; " class="style1">--%>
                                <asp:CheckBox ID="chkUseFileTypePreservation" runat="server" CssClass="testo" OnCheckedChanged="chkUseFileTypePreservation_CheckedChanged" />
                            <%--</td>--%><asp:Label id="lblValidazone" runat="server" align="right" class="testo_grigio_scuro" style="height: 20px;" Text="Validazione conservazione:"></asp:Label>
                                <asp:CheckBox ID="chkUseFileTypeValidation" runat="server" CssClass="testo" OnCheckedChanged="chkUseFileTypeValidation_CheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 30%">
                                Descrizione: *</td>
                            <td align="left" style="color: #666666; " class="style1">
                                                    <asp:TextBox ID="txtDescrizione" runat="server" CssClass="testo" Width="350px" MaxLength="255"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 30%">
                                Estensione *:
                            </td>
                            <td align="left" class="style1">
                                <asp:TextBox ID="txtEstensione" runat="server" CssClass="testo" MaxLength="10" Width="50px"></asp:TextBox>
                            </td>                          
                        </tr>
                        <tr>
                            <td style="height: 20px" align="right" class="testo_grigio_scuro">
                                Dim. massima (KB):
                            <td align="left" class="style2">
                                <asp:TextBox ID="txtDimMaxFile" runat="server" CssClass="testo" Width="50px" MaxLength="10" onKeyPress="ValidateNumericKey();"></asp:TextBox>
                                &nbsp;&nbsp;Se file di dim. maggiore:
                                <asp:DropDownList ID="cboMaxFileSizeAlertMode" runat="server" CssClass="testo" Width="130px">
                                    <asp:ListItem Value="None" Text="Nessun messaggio" />
                                    <asp:ListItem Value="Warning" Text="Chiedi conferma" Selected="True" />
                                    <asp:ListItem Value="Error" Text="Blocca" />
                                </asp:DropDownList></td>
                        </tr>        
                        <tr>
                            <td style="height: 20px" align="right" class="testo_grigio_scuro">
                                Formato valido per:
                            </td>
                            <td align="left" class="style1">
                                <asp:DropDownList ID="cboDocumentTypes" runat="server" CssClass="testo" Width="150px">
                                    <asp:ListItem Value="All" Text="Tutti i documenti" Selected="True" />
                                    <asp:ListItem Value="Protocollo" Text="Documenti protocollati" />
                                    <asp:ListItem Value="Grigio" Text="Documenti grigi" />
                                </asp:DropDownList>
                            </td>
                        </tr>
			            <tr>
                            <td colspan="2" style="height: 5px">
                                <table cellpadding="0" cellspacing="0" width="100%">
                                    <tr>
                                        <td align="left" class="titolo_pnl" style="height: 14px">
                                            <asp:Label ID="Label1" runat="server">Modello documento "vuoto"</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
			            </tr>
                        <tr>
                            <td align="right" class="testo_grigio_scuro" style="height: 20px; width: 40%">
                                Seleziona file:
                            </td>
                            <td align="left" valign="middle" class="style1">
                                <INPUT type="file" runat="server" class="testo" id="uploadFile" size="55" name="uploadFile">&nbsp;
                                <cc2:ImageButton ID="btnDeleteFileModel" runat="server" CommandName="DELETE_ITEM"
                                    Height="20px" ImageUrl="../Images/cestino.gif" ToolTip="Elimina modello documento" Width="20px" OnClick="btnDeleteFileModel_Click" ImageAlign="AbsMiddle" /></td>                          
                        </tr>
                        <tr>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td align="left">
                                <asp:button id="btnSave" runat="server" CssClass="testo_btn_p" Text="Salva formato" OnClick="btnSave_Click"></asp:button>
                                <asp:button id="btnClose" runat="server" CssClass="testo_btn_p" Text="Chiudi" OnClick="btnClose_Click"></asp:button></td>
                        </tr>
                    </table>
                </td>
			</tr>
		</table>
    </form>
</body>
</html>
