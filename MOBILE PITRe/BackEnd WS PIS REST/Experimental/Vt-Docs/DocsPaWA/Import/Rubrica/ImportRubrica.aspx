<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportRubrica.aspx.cs"
    Inherits="DocsPAWA.Import.Rubrica.ImportRubrica" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Importazione corrispondenti</title>
    <link href="../../CSS/docspa_30.css" rel="stylesheet" type="text/css" />
    <!-- Inclusione codice delle librerie per la progress bar -->
    <script type="text/javascript" src="../../LIBRERIE/ProgressBar/prototype/prototype.js"></script>
    <script type="text/javascript" src="../../LIBRERIE/ProgressBar/progress/jsProgressBarHandler.js"></script>
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


            // Se a richiamare la funzione è stato l'uploader, viene nascosta
            // la progress bar altrimenti viene mostrata
            //            if (sender != null && sender.get_id() == 'fileUploader') {
            //                progressBar.style.visibility = 'hidden';
            //                
            //            }
            //            else {
            //                progressBar.style.visibility = '';
            //            }

        }

        // Funzione utilizzata per nascondere la schermata di
        // waiting
        function hideWait(sender, args) {
            // Viene nascosto il pannello di wait
            this._popup = $find('mdlPopupWait');
            this._popup.hide();

            // Se il sender dell'evento è il fileUploader...
            if (sender != null && sender.get_id() == 'fileUploader') {
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
                if (!(this._fileSize > 0))
                    validParam = false;

                // ...il secondo controllo va fatto sul content
                // type che deve essere application/vnd.ms-excel
                if (this._contentType != 'application/vnd.ms-excel')
                    validParam = false;

                // Cancellazione del contenuto di tutti i data grid
                divGenerale.innerHTML = "";
                divInseriti.innerHTML = "";
                divModificati.innerHTML = "";
                divCancellati.innerHTML = "";

                // Viene impostata a zero la percentuale della progress bar
                //                if(manualPB2)
                //                    manualPB2.setPercentage('0');

                // Se ci sono stati errori, viene visualizzato un messaggio e vengono
                // disabilitati i pulsanti "Importa Corrispondenti", altrimenti vengono
                // abilitati ii pulsanti "Importa Corrispondenti".
                if (!validParam) {
                    alert("Selezionare un file 'Cartella di Excel' valido o assicurarsi che il file non sia aperto in nessun altro programma.");
                    $get("btnImportaCorrispondenti").disabled = true;

                }
                else
                    $get("btnImportaCorrispondenti").disabled = false;

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
            // Al page load vengono disattivati i pulsanti "Crea documenti"
            $get("btnImportaCorrispondenti").disabled = true;

        }
        
    </script>
    <script type="text/javascript">
        //        document.observe('dom:loaded', function() {

        //            manualPB2 = new JS_BRAMUS.jsProgressBar(
        //								$('progressBar'),
        //								0,
        //								{

        //								    barImage: Array(
        //								        '../../images/progressBar/percentImage_back4.png',
        //										'../../images/progressBar/percentImage_back3.png',
        //										'../../images/progressBar/percentImage_back2.png',
        //										'../../images/progressBar/percentImage_back1.png'
        //									),

        //								    onTick: function(pbObj) {

        //								        return true;
        //								    }
        //								}
        //							);
        //        }, false);

        //        function updateProgressBar(sender, args) {

        //            if (manualPB2) {
        //                // Calcolo dell'entità dell'incremento
        //                var incVal = frmImportaDocumenti.hfTargetPerc.value - manualPB2.getPercentage();

        //                if (incVal < 0)
        //                    incVal = incVal * -1;

        //                if (frmImportaDocumenti.hfTargetPerc.value < manualPB2.getPercentage())
        //                    manualPB2.setPercentage('-' + manualPB.getPercentage());

        //                manualPB2.setPercentage('+' + incVal);

        //                frmImportaDocumenti.hfTargetPerc.value = manualPB2.getPercentage();
        //            }
        //        }

        // Funzione per l'apertura della finestra in cui visualizzare il report
        function openReport() {
            var report = window.open('../ReportGenerator/ExportReport.aspx', 'report', null, null);

            report.focus();
        }
    </script>
</head>
<body>
    <form id="frmImportaCorrispondenti" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="36000">
    </asp:ScriptManager>
    <script type="text/javascript">
        //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(updateProgressBar);
    </script>
    <table class="tabella">
        <tr>
            <td style="text-align: center" class="titolo_rosso">
                Importazione corrispondenti da foglio excel
            </td>
        </tr>
        <tr style="height: 10px">
            <td align="right">
                <a href="ImportRubrica.xls" target="_blank" class="testo_grigio9">Scarica modello</a>
            </td>
        </tr>
        <tr class="titolo_real">
            <td>
                Nome file:&nbsp;
                <cc1:AsyncFileUpload ID="fileUploader" runat="server" CssClass="pulsante" Width="500px"
                    BackColor="WhiteSmoke" OnClientUploadStarted="showWait" OnClientUploadComplete="hideWait"
                    CompleteBackColor="WhiteSmoke" ErrorBackColor="WhiteSmoke" OnClientUploadError="uploadingError" />
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
                <asp:UpdatePanel ID="upReport" runat="server">
                    <ContentTemplate>
                        <cc1:TabContainer ID="trReport" runat="server" Height="270px" BorderStyle="Solid"
                            BorderColor="#810d06" BorderWidth="1px" ScrollBars="Auto" Visible="true">
                            <cc1:TabPanel ScrollBars="Auto" runat="server" HeaderText="Generale"
                                ID="tpGenerale">
                                <ContentTemplate>
                                    <div id="divGenerale">
                                        <asp:GridView ID="grdGenerale" runat="server" CellPadding="4" ForeColor="#333333"
                                            GridLines="None" AutoGenerateColumns="false" Font-Size="8pt" Width="100%">
                                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateField HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dettagli" ItemStyle-Width="45%" >
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Corrispondenti inseriti" runat="server" ID="tpInseriti">
                                <ContentTemplate>
                                    <div id="divInseriti">
                                        <asp:GridView ID="grdInseriti" runat="server" CellPadding="4" ForeColor="#333333"
                                            GridLines="None" AutoGenerateColumns="false" Font-Size="8pt" Width="100%">
                                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateField HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Corrispondenti modificati" runat="server" ID="tpModificati">
                                <ContentTemplate>
                                    <div id="divModificati">
                                        <asp:GridView ID="grdModificati" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                                            AutoGenerateColumns="false" Font-Size="8pt" Width="100%">
                                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateField HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Corrispondenti cancellati" runat="server" ID="tpCancellati">
                                <ContentTemplate>
                                    <div id="divCancellati">
                                        <asp:GridView ID="grdCancellati" runat="server" CellPadding="4" ForeColor="#333333"
                                            GridLines="None" AutoGenerateColumns="false" Font-Size="8pt" Width="100%">
                                            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                            <AlternatingRowStyle BackColor="White" />
                                            <Columns>
                                                <asp:BoundField DataField="Message" HeaderText="Messaggio" ItemStyle-Width="45%" />
                                                <asp:TemplateField HeaderText="Risultato">
                                                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Dettagli" ItemStyle-Width="45%">
                                                    <ItemTemplate>
                                                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((DocsPAWA.DocsPaWR.AddressBookImportResult) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </ContentTemplate>
                            </cc1:TabPanel>
                        </cc1:TabContainer>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnImportaCorrispondenti" EventName="Click" />
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
                        <asp:Button CssClass="pulsante" ID="btnImportaCorrispondenti" runat="server" Text="IMPORTA CORRISPONDENTI"
                            OnClientClick="showWait();" onclick="btnImportaCorrispondenti_Click" />
                        &nbsp;
                        <asp:Button CssClass="pulsante" ID="btnChiudi" runat="server" Text="CHIUDI" OnClientClick="javascript:window.close();" />
                        &nbsp;
                        <asp:Button CssClass="pulsante" ID="btnEsportaReport" runat="server" Text="ESPORTA REPORT PDF"
                            Enabled="false" OnClientClick="openReport();" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <!-- PopUp Wait-->
    <cc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
        BackgroundCssClass="modalBackground" />
    <div id="Wait" runat="server" style="display: none; font-weight: bold; font-size: xx-large;
        font-family: Arial; text-align: center;">
        <span style="font-family: Arial; font-size: small; text-align: center;">
            <asp:UpdatePanel ID="pnlUP" runat="server">
                <ContentTemplate>
                    <input id="hfTargetPerc" runat="server" type="hidden" value="0" />
                    <br />
                    <asp:Label ID="lblInfo" runat="server">Importazione in corso...</asp:Label><br />
                </ContentTemplate>
            </asp:UpdatePanel>
            (L'operazione può richiedere diversi minuti.)</span><br />
        <br />
        <%--<span style="font-family: Arial; font-size: small; text-align: center;" visible="true"
            id="progressBar">[ Inizializzazione barra di avanzamento ]</span>--%>
    </div>
    </form>
</body>
</html>