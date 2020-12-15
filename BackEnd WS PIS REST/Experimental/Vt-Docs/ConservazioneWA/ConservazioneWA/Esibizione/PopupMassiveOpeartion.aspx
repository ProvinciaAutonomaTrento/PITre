<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PopupMassiveOpeartion.aspx.cs"
    Inherits="ConservazioneWA.Esibizione.PopupMassiveOpeartion" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Conservazione.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        .tabAlert
        {
            margin: 0px;
            padding: 0px;
            border-collapse: collapse;
            border-left: 3px solid #666666;
            border-right: 3px solid #666666;
            border-bottom: 3px solid #666666;
            background-color: #ffffff;
        }
        
        .tabAlert td
        {
            padding: 2px;
        }
        
        .tabAlert caption
        {
            font-size: 14px;
            text-align: center;
            background-color: #3399FF;
            border-left: 3px solid #666666;
            border-right: 3px solid #666666;
            border-top: 3px solid #666666;
            padding: 2px;
            border-bottom: 1px solid #666666;
        }
        
        .bg_grigioS
        {
            background-color: #4b4b4b;
            font-weight: normal;
            font-size: 10px;
            color: white;
            text-indent: 0px;
            font-family: Verdana;
        }
        
        .bg_grigioA
        {
            background-color: #f2f2f2;
            font-weight: normal;
            font-size: 10px;
            color: #000000;
            text-indent: 0px;
            font-family: Verdana;
        }
        
        .bg_grigioN
        {
            background-color: #d9d9d9;
            font-weight: normal;
            font-size: 10px;
            color: #000000;
            text-indent: 0px;
            font-family: Verdana;
        }
        
        .testo_grigio_scuro
        {
            font-weight: bold;
            font-size: 14px;
            color: #666666;
            font-family: Verdana;
        }
    </style>
    <script language="javascript" type="text/javascript">

        function showWait(sender, args) {
            // Viene visualizzato il popup di wait    
            this._popup = $find('mdlPopupWait');
            this._popup.show();
        }

        function closeAlert(sender, args) {
            this._alert = $find('mpeMessage');
            this._alert.hide();
        }

        // Funzione per l'apertura della finestra del report
        function openReport() {
            var report = dialogArguments.window.open('../Import/ReportGenerator/ExportReport.aspx', 'report', null, null);
            report.opener = window;
            report.focus();
        }

        function resize() {
            var maxWidth = "420px";
            var maxHeight = "400px";
            window.dialogHeight = maxHeight;
            window.showModalDialog;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000">
    </asp:ScriptManager>
    <table width="100%">
        <tr align="center">
            <td>
                <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <asp:Panel ID="pnlReport" runat="server" Width="90%" BorderStyle="Solid" BorderColor="#810d06"
                            BorderWidth="1px" ScrollBars="Auto" Visible="false" >
                            <asp:DataGrid ID="grdReport" runat="server" Height="70%" Width="100%" BorderWidth="1px" ShowHeader="true"
                                CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                <Columns>
                                    <asp:BoundColumn HeaderText="ID" DataField="ObjId">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn HeaderText="ID Istanza" DataField="IstId">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn HeaderText="Oggetto" DataField="Obj">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn HeaderText="Esito" DataField="Result">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundColumn>
                                    <asp:BoundColumn HeaderText="Dettagli" DataField="Details">
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </asp:BoundColumn>
                                </Columns>
                            </asp:DataGrid>
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--<asp:Button ID="btnConferma" CssClass="pulsante99" runat="server" Text="Conferma"
                            ToolTip="Conferma" OnClick="btnConfermaMP_Click" OnClientClick="showWait();" />--%>
                        <asp:Button ID="btnChiudi" CssClass="pulsante69" runat="server" Text="Chiudi" ToolTip="Annulla"
                            OnClientClick="self.close();" />
                        <%--<asp:Button ID="btnExportReport" runat="server" Text="Esporta report" CssClass="pulsante127" OnClientClick="openReport();"
                            ToolTip="Esporta report" Enabled="false" />--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
