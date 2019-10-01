<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportRDE.aspx.cs" Inherits="DocsPAWA.Import.RDE.ImportRDE" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Importazione Registro Documenti di Emergenza</title>
    <link href="../../CSS/docspa_30.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tabella
        {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #800000;
        }
        .modalBackground
        {
            background-color: Gray;
            filter: alpha(opacity=50);
            opacity: 0.5;
        }
        .text
        {
            height: 8px;
        }
    </style>

    <script type="text/javascript">
        // Funzione utilizzata per mostrare la schermata
        // di waiting
        function showWait(sender, args) {
            // Viene visualizzato il popup di wait    
            this._popup = $find('mdlPopupWait');
            this._popup.show();
            
        }
        
        // Funzione utilizzata per nascondere la schermata di
        // waiting
        function hideWait(sender, args) {
            // Viene nascosto il pannello di wait
            this._popup = $find('mdlPopupWait');
            this._popup.hide();
            
            // Se il sender dell'evento è il fileUploader...
            if(sender != null && sender.get_id() == 'fileUploader')
            {
                // ...booleano utilizzato per verificare se
                // ci sono problemi con il file (estensione o
                // content type non validi
                var validParam = true;
                
                // ...prelevamento di fileName, del fileSize e del contentType
                this._fileName = args.get_fileName();
                this._fileSize = args.get_length();
                this._contentType = args.get_contentType();
                    
                // ...il primo controllo viene fatto sulla dimensione
                // del file che deve essere maggiore di zero
                if(!(this._fileSize > 0))
                    validParam = false;
                
                // ...il secondo controllo va fatto sul content
                // type che deve essere application/vnd.ms-excel
                if(this._contentType != 'application/vnd.ms-excel')
                    validParam = false;
                    
                // Cancellazione del contenuto di tutti i data grid
                divGenerale.innerHTML = "";
                divArrivo.innerHTML = "";
                divPartenza.innerHTML = "";
                try {
                    divInterni.innerHTML = "";
                }
                catch (e) { }

                // Se ci sono stati errori, viene visualizzato un messaggio e viene
                // disabilitato il pulsante "Crea documenti", altrimenti viene
                // abilitato il pulsante "Crea RDE"
                if(!validParam)
                {
                    alert("Selezionare un file 'Cartella di Excel' valido o accertarsi che il file non sia in uso in qualche altro processo.");
                    $get("btnCreaDocumenti").disabled = true;
                }
                else
                    $get("btnCreaRDE").disabled = false;
                    
            }
            
        }
        
        // Funzione per la segnalazione di errori durante l'uploading
        // del file
        function uploadingError(sender, args) {
           // In caso di errore viene visualizzato un messaggio
           // e viene nascosto il div di attesa
           alert("Si è verificato un errore dutante l'upload del file."); 
           
           hideWait();
        }
        
        function pageLoad() {
            // Al page load viene disattivato il pulsante "Crea RDE"
            $get("btnCreaRDE").disabled = true;

        }

        // Funzione per l'apertura della finestra in cui visualizzare il report
        function openReport() {
            var report = window.open('../ReportGenerator/ExportReport.aspx', 'report', null, null);

            report.focus();
        }

        function OpenHelp(from) {
            var pageHeight = 600;
            var pageWidth = 800;
            //alert(from);
            var posTop = (screen.availHeight - pageHeight) / 2;
            var posLeft = (screen.availWidth - pageWidth) / 2;

            var newwin = window.showModalDialog('../../Help/Manuale.aspx?from=' + from,
								'',
								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:no;scroll:yes;dialogLeft:' + posLeft + ';dialogTop:' + posTop + ';center:yes;help:no');

        }							

    </script>

</head>
<body>
    <form id="frmImportaRDE" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="36000">
    </asp:ScriptManager>
    <table class="tabella">
        <tr>
            <td style="text-align: center" class="titolo_rosso">
                Importazione Registro Documenti Emergenza (<asp:Literal ID="ltlAdministrationInfo" runat="server" />)
            </td>
        </tr>
        <tr style="height: 10px">
            <td align="right">
                <asp:ImageButton ID="help" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('ImportRDE')" />&nbsp;&nbsp;
                <asp:HyperLink ID="hlLink" runat="server" Target="_blank" CssClass="testo_grigio_scuro" ForeColor="Blue">Scarica modello</asp:HyperLink>
            </td>
        </tr>
        <tr class="titolo_real">
            <td>
                Nome file:&nbsp;
                <cc1:AsyncFileUpload ID="fileUploader" runat="server" CssClass="pulsante" Width="500px"
                    BackColor="WhiteSmoke" OnClientUploadStarted="showWait" OnClientUploadComplete="hideWait"
                    CompleteBackColor="WhiteSmoke" ErrorBackColor="WhiteSmoke" OnClientUploadError="uploadingError" OnUploadedComplete="fileUploader_UploadedComplete" />
                <br />
            </td>
        </tr>
        <tr style="height: 8px">
            <td>
            </td>
        </tr>
        <tr class="titolo_real">
            <td>
                Report
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <cc1:TabContainer ID="trReport" runat="server" Height="270px" BorderStyle="Solid"
                            BorderColor="#810d06" BorderWidth="1px" ScrollBars="Auto" Visible="true">
                            <cc1:TabPanel ScrollBars="Auto" runat="server" HeaderText="Generale" ID="TabPanel1">
                                <ContentTemplate>
                                    <div id="divGenerale">
                                        <asp:DataGrid ID="grdGenerale" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                            <FooterStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></FooterStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Ordinale" ItemStyle-Width="3%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateColumn HeaderText="Risultato" ItemStyle-Width="7%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" runat="server" HeaderText="Documenti in arrivo" ID="tpArrivo">
                                <ContentTemplate>
                                    <div id="divArrivo">
                                        <asp:DataGrid ID="grdArrivo" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                            <FooterStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></FooterStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Ordinale" ItemStyle-Width="3%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateColumn HeaderText="Risultato" ItemStyle-Width="7%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" runat="server" HeaderText="Documenti in partenza"
                                ID="tpPartenza">
                                <ContentTemplate>
                                    <div id="divPartenza">
                                        <asp:DataGrid ID="grdPartenza" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                            <FooterStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></FooterStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Ordinale" ItemStyle-Width="3%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateColumn HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Documenti interni" runat="server" ID="tpInterni">
                                <ContentTemplate>
                                    <div id="divInterni">
                                        <asp:DataGrid ID="grdInterni" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                            <FooterStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></FooterStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Ordinale" ItemStyle-Width="3%">
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlOrdinalNumber" runat="server" Text="<%#this.GetOrdinalNumber((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:BoundColumn DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateColumn HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="7%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                        </cc1:TabContainer>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCreaRDE" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr align="center">
            <td class="testo_grigio_scuro" style="padding: 5px;" align="center">
                <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button CssClass="pulsante" ID="btnCreaRDE" runat="server" Text="AVVIA RDE"
                            OnClientClick="showWait();" onclick="btnCreaRDE_Click" />
                        &nbsp;
                        <asp:Button CssClass="pulsante" ID="btnChiudi" runat="server" Text="CHIUDI" OnClientClick="javascript:window.close();" />
                        &nbsp;
                        <asp:Button CssClass="pulsante" ID="btnEsportaReport" runat="server" Text="ESPORTA REPORT PDF" Enabled="false"
                            OnClientClick="openReport();" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <!-- PopUp Wait-->
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display: none; font-weight: bold; font-size: xx-large;
        font-family: Arial; text-align:center;">
        Attendere prego ...<br />
        <span style="font-family:Arial; font-size:small; text-align:center;">(L'operazione può richiedere diversi minuti.)</span>
    </div>
    </form>
</body>
</html>
