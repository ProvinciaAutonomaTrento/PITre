<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDocumenti.aspx.cs"
    Inherits="DocsPAWA.Import.Documenti.ImportDocumenti" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=Titolo %></title>
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
                //this._contentType = args.get_contentType();
                // ...il primo controllo viene fatto sulla dimensione
                // del file che deve essere maggiore di zero
                if (!(this._fileSize > 0))
                    validParam = false;

                // ...il secondo controllo va fatto sul content
                // type che deve essere application/vnd.ms-excel
//                if (this._contentType != 'application/vnd.ms-excel')
//                    validParam = false;
                // Cancellazione del contenuto di tutti i data grid
                divGenerali.innerHTML = "";
                if (document.getElementById("divArrivo") != null) divArrivo.innerHTML = "";
                divPartenza.innerHTML = "";
                divInterni.innerHTML = "";
                divGrigi.innerHTML = "";
                if(document.getElementById("divAllegati")!=null) divAllegati.innerHTML = "";
                // Viene impostata a zero la percentuale della progress bar
//                if(manualPB2)
//                    manualPB2.setPercentage('0');

                // Se ci sono stati errori, viene visualizzato un messaggio e vengono
                // disabilitati i pulsanti "Crea documenti", altrimenti vengono
                // abilitati ii pulsanti "Crea documenti".
                if (!validParam) {
                    alert("Selezionare un file 'Cartella di Excel' valido o assicurarsi che il file non sia aperto in nessun altro programma.");
                    $get("btnCreaDocumenti").disabled = true;

                }
                else
                    $get("btnCreaDocumenti").disabled = false;

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
            $get("btnCreaDocumenti").disabled = true;

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
            var report = dialogArguments.window.open('../ReportGenerator/ExportReport.aspx', 'report', null, null);
            report.opener = window;
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
    <form id="frmImportaDocumenti" runat="server" enctype="multipart/form-data">
    <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="36000">
    </asp:ScriptManager>

    <script type="text/javascript">
        //Sys.WebForms.PageRequestManager.getInstance().add_endRequest(updateProgressBar);
    </script>

    <table class="tabella">
        <tr>
            <td style="text-align: center" class="titolo_rosso">
                <%=Header %>
            </td>
        </tr>
        <tr style="height: 10px">
            <td align="right">
                <asp:ImageButton ID="help" runat="server" AlternateText="Aiuto?" SkinID="btnHelp" OnClientClick="OpenHelp('StampaUnione')" />&nbsp;&nbsp;
                <asp:HyperLink ID="hlLink" runat="server" Target="_blank" CssClass="testo_grigio_scuro" ForeColor="Blue">Scarica modello</asp:HyperLink>
                &nbsp;&nbsp;
                 <asp:HyperLink ID="hlLink_preg" runat="server" Target="_blank" CssClass="testo_grigio_scuro" ForeColor="Blue" Visible = false>Pregressi</asp:HyperLink>
                 &nbsp;
            </td>
        </tr>
        <tr class="titolo_real">
            <td>
                Nome file:&nbsp;
                <cc1:AsyncFileUpload ID="fileUploader" runat="server" CssClass="pulsante" Width="500px" PersistedStoreType="Session"
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
                <asp:UpdatePanel ID="upReport" runat="server">
                    <ContentTemplate>
                        <cc1:TabContainer ID="trReport" runat="server" Height="270px" BorderStyle="Solid"
                            BorderColor="#810d06" BorderWidth="1px" ScrollBars="Auto" Visible="true">
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Generale" runat="server" ID="tpGenerale">
                                <HeaderTemplate>
                                    Generale
                                </HeaderTemplate>
                                <ContentTemplate>
                                    <div id="divGenerali">
                                        <asp:DataGrid ID="grdGenerale" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
                                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></HeaderStyle>
                                            <FooterStyle HorizontalAlign="Center" CssClass="testo_grigio_scuro"></FooterStyle>
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Ordinale">
                                                    <ItemStyle HorizontalAlign="Center" Width="3%" />
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
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Documenti non protocollati" runat="server" ID="tpGrigi">
                                <ContentTemplate>
                                    <div id="divGrigi">
                                        <asp:DataGrid ID="grdGrigi" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
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
                            <cc1:TabPanel ScrollBars="Auto" HeaderText="Allegati" runat="server" ID="tpAllegati">
                                <ContentTemplate>
                                    <div id="divAllegati">
                                        <asp:DataGrid ID="grdAllegati" SkinID="datagrid" runat="server" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
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
                        <asp:AsyncPostBackTrigger ControlID="btnCreaDocumenti" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr style="height: 10px">
            <td align="center">
                <asp:UpdatePanel ID="upScarica" runat="server" UpdateMode="Conditional">
                   <ContentTemplate>
                      <asp:HyperLink ID="hlScaricaDoc" runat="server" CssClass="testo_grigio_scuro" ForeColor="Blue"  Visible="false">Scarica documenti</asp:HyperLink>
                   </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr align="center">
            <td class="testo_grigio_scuro" style="padding: 5px;" align="center">
                <asp:UpdatePanel ID="upButtons" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Button CssClass="pulsante" ID="btnCreaDocumenti" runat="server" Text="CREA DOCUMENTI"
                            OnClientClick="showWait();" OnClick="btnCreaDocumenti_Click" />
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
<script>
    var input =
document.getElementById('fileUploader').childNodes.item(1);
    if (input != null) {
        var nome_inputNew =
input.firstChild.attributes.getNamedItem("name").value;
        var id_inputNew =
input.firstChild.attributes.getNamedItem("id").value;
        input.removeChild(input.firstChild);
        var newinput = document.createElement('input');
        newinput.setAttribute("id", id_inputNew);
        newinput.setAttribute("name", nome_inputNew);
        newinput.setAttribute("type", "file");
        newinput.setAttribute("style", "width: 500px;");
        input.appendChild(newinput);
    }
</script>
</html>
