<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SpedizioneDocumento.aspx.cs" Inherits="DocsPAWA.Spedizione.SpedizioneDocumento" %>
<%@ Register src="../waiting/WaitingPanel.ascx" tagname="WaitingPanel" tagprefix="uc2" %>
<%@ Register src="Destinatari.ascx" tagname="Destinatari" tagprefix="uc4" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Spedizione documento</title>
    <base target = "_self" />    
    <LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script type ="text/javascript">
        function btnSpedisci_onClientClick() {
            var btnSped = document.getElementById('btnSpedisci');
            btnSped.disabled = true;
            __doPostBack(btnSped.name, '');
            ShowWaitPanel('Spedizione in corso...'); 
        }
        function CloseWaitPanel() {
            CloseWait();
        }
        function closeDialog() { window.returnValue = document.getElementById('txtReturnValue').value; window.close(); }
    </script> 
    <style type="text/css">
        .pulsante92
        {}
    </style>
</head>
<body leftMargin="5" topMargin="5" bottomMargin="5" rightMargin="5">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scrManager" runat="server" AsyncPostBackTimeOut="3600"></asp:ScriptManager>
        <input id="txtReturnValue" runat="server" type="hidden" value="False" />
        <uc2:WaitingPanel ID="waitingPanel" runat="server" />
        <cc2:MessageBox ID="msgSpedisci" runat="server" OnGetMessageBoxResponse="msgSpedisci_messageBoxResponse" />
        <table class="contenitore"  id="tblContainer" border="0" style="width: 100%; height: 100%">
            <tr>
                <td class="item_editbox">
				    <p class="boxform_item">
					    <asp:Label id="lblTitle" runat="server">Spedizione documento</asp:Label>
				    </p>
			    </td>
            </tr>
            <tr>
                <td>
                    <br />
                    <asp:UpdatePanel ID="updPanelDestInterni" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table class="info_grigio" id="tblDestinatariInterni" style="width:95%;" align="center">
                                <tr>
                                    <td>
                                        <uc4:Destinatari ID="listaDestinatariInterni" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSpedisci" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>                
                    <br />
                    <asp:UpdatePanel ID = "updPanelRF" UpdateMode="Conditional" ChildrenAsTriggers="true" runat="server">
                        <ContentTemplate>
                            <table class="info_grigio" id="Table1" style="width:95%;" align="center" style="margin:0px;">
                                <tr id="trRegistriRF" runat="server">

                                    <td width="4%"></td>
                                    <td width="20%">
                                        <asp:Label ID = "lblRegistriRF" runat = "server" text = "Registro / RF mittente:" CssClass="testo_grigio_scuro"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="cboRegistriRF" runat="server" 
                                            CssClass="testo_grigio_scuro" Width="300px" 
                                            onselectedindexchanged="cboRegistriRF_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                    </td>
                                    <td width="35%" style="padding-left:10px;">
                                        <asp:Label ID = "lblCasella" runat = "server" text = "Elenco caselle Registro/RF Mittente:" CssClass="testo_grigio_scuro"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="ddl_caselle" runat="server" 
                                            CssClass="testo_grigio_scuro" Width="300px" Enabled="false"
                                            onselectedindexchanged="ddl_caselle_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                                    </td>
                                    <td width="15%">
                                        <asp:Label ID = "lblRicevutaPec" runat = "server" text = "Tipo Ricevuta PEC:" CssClass="testo_grigio_scuro"></asp:Label>
                                        <br />
                                        <asp:DropDownList ID="cboTipoRicevutaPec" runat="server" Enabled="false"
                                            CssClass="testo_grigio_scuro" Width="120px">
                                            <asp:ListItem Value ="C">Completa</asp:ListItem>
                                            <asp:ListItem Value ="B">Breve</asp:ListItem>
                                            <asp:ListItem Value ="S">Sintetica</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td width="26%"> </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdatePanel ID="updPanelDestEsterni" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <table class="info_grigio" id="tblDestinatariEsterni" style="width:95%;" align="center">
                        <tr>
                            <td>
                                <uc4:Destinatari ID="listaDestinatariInteroperanti" runat="server" />
                                <br />
                                <uc4:Destinatari ID="listaDestinatatiInteropSempl" runat="server" />
                                <br />
                                <uc4:Destinatari ID="listaDestinatariNonInteroperanti" runat="server" />
                            </td>
                        </tr>
                    </table>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSpedisci" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
          </tr>
           <tr align="center">
                <td>
					<cc2:Messagebox Width="1" Height="1" CssClass="testo_grigio"  id="msg_SpedizioneAutomatica" runat="server"></cc2:Messagebox>
                    <asp:UpdatePanel ID="updPanelPulsanti" runat="server" UpdateMode="Conditional" >
                        <ContentTemplate>
                            <br />
                            <asp:Button ID = "btnSpedisci" runat = "server" TabIndex = "1" Text = "Spedisci" CssClass="pulsante92" onclick="btnSpedisci_Click"  OnClientClick="btnSpedisci_onClientClick();"
                                Width="100px" />
                            &nbsp;                                
                            <asp:Button ID = "btnChiudi" runat = "server" TabIndex = "2" Text = "Chiudi" CssClass="pulsante92" OnClientClick="closeDialog();"/>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="cboRegistriRF" EventName="SelectedIndexChanged"/>
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
      </table>
    </form>
</body>
</html>
