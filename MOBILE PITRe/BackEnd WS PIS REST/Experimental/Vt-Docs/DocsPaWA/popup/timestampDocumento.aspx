<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="timestampDocumento.aspx.cs" Inherits="DocsPAWA.popup.timestampDocumento" validateRequest="false"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"> 

<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet"> 
    <base target="_self" />
    <style type="text/css">
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }        
    </style>       
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeOut="3600"></asp:ScriptManager>
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Elenco Timestamp Documento" />

    <script language="javascript" type="text/javascript">
       
        function showWait() {
            this._popup = $find('mdlPopupWait');
            this._popup.show();
        }

        function hideWait() {
            this._popup = $find('mdlPopupWait');
            this._popup.hide();
        }        
    </script>      

    <table width="100%" border="0">
        <tr>
            <td align="left" style="font-weight:bold; font-family:Arial; font-size:smaller;">
                <asp:label id="lbl_timestamp" runat="server" Font-Bold="True" Width="100%">Timestamps associati al documento</asp:label>
            </td>
			<td align="right">
                <asp:Button id="btn_creaTsd" runat="server" Text="Crea TSD" CssClass="pulsante127" ToolTip="Crea un TSD dal TSR Associato" OnClick="btn_creaTsd_Click" OnClientClick="showWait();" Enabled="false"></asp:Button>
                <asp:Button id="btn_associaTimestamp" runat="server" Text="Ass.Timestamp" CssClass="pulsante127" ToolTip="Associa timestamp" OnClick="btn_associaTimestamp_Click" OnClientClick="showWait();" Enabled="false"></asp:Button>
                <%--<asp:Button id="btn_chiudi" runat="server" Text="Chiudi" CssClass="pulsante69" OnClientClick="window.opener.top.principale.iFrame_dx.document.location='../documento/tabDoc.aspx';window.close();" ToolTip="Chiudi"></asp:Button>--%>
                <asp:Button id="btn_chiudi" runat="server" Text="Chiudi" CssClass="pulsante69" OnClientClick="window.close();" ToolTip="Chiudi"></asp:Button>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="border-bottom:thin #e0e0e0 solid;">
            <DIV id="div_timestamps" align="center" runat="server" style="OVERFLOW: auto; HEIGHT: 228px; width:100%">
                <asp:DataGrid id="dg_timestamp" runat="server" AutoGenerateColumns="False" Width="100%" onitemcommand="dg_timestamp_ItemCommand">
                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
					<ItemStyle CssClass="bg_grigioN"></ItemStyle>
					<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
					<Columns>
						<asp:BoundColumn Visible="False" DataField="SYSTEM_ID" HeaderText="SYSTEM_ID"></asp:BoundColumn>
						<asp:BoundColumn DataField="NUMERO_DI_SERIE"  HeaderText="Numero di Serie" ItemStyle-HorizontalAlign="Left">
							<HeaderStyle Width="30%" HorizontalAlign="Left"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
						</asp:BoundColumn>						
                        <asp:BoundColumn DataField="DTA_CREAZIONE"  HeaderText="Data Creazione" ItemStyle-HorizontalAlign="Left">
							<HeaderStyle Width="30%" HorizontalAlign="Left"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
						</asp:BoundColumn>
                        <asp:BoundColumn DataField="DTA_SCADENZA"  HeaderText="Data Scadenza" ItemStyle-HorizontalAlign="Left">
							<HeaderStyle Width="30%" HorizontalAlign="Left"></HeaderStyle>
						    <ItemStyle HorizontalAlign="Left" />
						</asp:BoundColumn>
                        <asp:TemplateColumn HeaderText="Dettagli">
							<ItemTemplate>
								<asp:ImageButton ID="imgBtn_dettagli" runat="server" CommandName="Dettagli" ImageUrl="~/Images/proto/dett_lente.gif" ToolTip="Dettagli timestamp" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:TemplateColumn>
                        <asp:TemplateColumn HeaderText="Salva">
							<ItemTemplate>
								<asp:ImageButton ID="imgBtn_salva" runat="server" CommandName="Salva" ImageUrl="~/Images/proto/salva.gif" ToolTip="Salva timestamp" />
							</ItemTemplate>
							<HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
							<ItemStyle HorizontalAlign="Center"></ItemStyle>
						</asp:TemplateColumn>
					</Columns>
				</asp:DataGrid>
            </DIV>            
            </td>
        </tr>        
        <asp:Panel ID="pnl_dettagli" runat="server" Visible="false">
        <tr>
            <td colspan="2">
                <table width="100%" border="0">
                    <tr>
                        <td width="30%" style="font-weight:bold; font-family:Arial; font-size:smaller;">Soggetto :</td>
                        <td style="font-family:Arial; font-size:smaller;"><asp:label id="lbl_soggetto" runat="server" Width="100%"/></td>
                    </tr>    
                    <tr>
                        <td style="font-weight:bold; font-family:Arial; font-size:smaller;">Paese :</td>
                        <td style="font-family:Arial; font-size:smaller;"><asp:label id="lbl_paese" runat="server" Width="100%"/></td>
                    </tr>
                    <tr>
                        <td style="font-weight:bold; font-family:Arial; font-size:smaller;">S. N. Certificato :</td>
                        <td style="font-family:Arial; font-size:smaller;"><asp:label id="lbl_certificato" runat="server" Width="100%"/></td>
                    </tr>
                    <tr>    
                        <td style="font-weight:bold; font-family:Arial; font-size:smaller;">Algortimo di HASH :</td>
                        <td style="font-family:Arial; font-size:smaller;"><asp:label id="lbl_algoritmo" runat="server" Width="100%"/></td>
                    </tr>
                    <tr>    
                        <td style="font-weight:bold; font-family:Arial; font-size:smaller;">Nr. di serie :</td>
                        <td style="font-family:Arial; font-size:smaller;"><asp:label id="lbl_numeroSerie" runat="server" Width="100%"/></td>
                    </tr>
                </table>
            </td>
        </tr>
        </asp:Panel>        
    </table>

    <!-- PopUp Wait-->
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait" BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display:none; font-weight:bold; font-size:xx-large; font-family:Arial;">Attendere prego ...</div>

    </form>
</body>
</html>
