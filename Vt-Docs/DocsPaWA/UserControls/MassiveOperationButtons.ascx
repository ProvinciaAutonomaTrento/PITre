<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MassiveOperationButtons.ascx.cs"
    Inherits="DocsPAWA.UserControls.MassiveOperationButtons" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="~/UserControls/DocumentConsolidation.ascx" TagName="DocumentConsolidation" TagPrefix="uc2" %>
<script type="text/javascript" language="javascript">

    // L'URL della finestra di fascicolazione massiva
    var _fascicolazioneMassivaUrl = '<%=GetFascicolazioneMassivaURL %>';
    // L'URL della finestra di trasmissione massiva
    var _trasmissioneMassivaUrl = '<%=GetTrasmissioneMassivaURL %>';
    // L'URL della finestra di applicazione del time stamp massiva
    var _timestampMassivoUrl = '<%=GetTimestampMassivoURL %>';
    // L'URL della finestra di firma digitale documenti
    var _firmaMassivaDocumentiUrl = '<%=GetFirmaMassivaDocumentiURL%>';
    // L'URL della finestra di conversione massiva di documenti
    var _convertiMassivaDocumentiUrl = '<%=GetConversioneMassivaDocumentiURL%>';
    // L'URL della finestra per lo spostamento massivo in ADL
    var _adlMassivo = '<%=GetMassiveWorkingAreaURL%>';
    // L'URL della finestra per l'esportazione dei risultati della ricerca
    var _exportResult = '<%=GetExportResultURL%>';
    // L'URL della funzione per la creazione del documento da inoltrare
    var _inoltro = '<%=GetInoltraMassivoURL %>';
    // L'URL della funzione per l'eliminazione delle versioni
    var _elVersioni='<%=GetEliminaVersioniURL %>';

    // Funzione per l'apertura del popup di fascicolazione massiva
    function OpenFascicolaMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        window.showModalDialog(_fascicolazioneMassivaUrl, 'FascicolazioneMassiva', 'dialogWidth:500px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

    }

    // Funzione per l'apertura del popup di trasmissione massiva
    function OpenTrasmissioneMassiva() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 450);
        var newTop = (screen.availHeight / 2 - 400);

        // Apertura della ModalDialog
        window.showModalDialog(_trasmissioneMassivaUrl, 'TrasmissioneMassiva', 'dialogWidth:900px;dialogHeight:800px;status:no;resizable:yes;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

    }

    // Funzione per l'apertura del popup di timestamp massivo
    function OpenTimestampMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 75);

        // Apertura della ModalDialog
        window.showModalDialog(_timestampMassivoUrl,
                'TimestampMassivo', 
                'dialogWidth:500px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

    }

    // Funzione per la firma digitale di più documenti
    function OpenFirmaMassivaDocumenti() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 525);
        var newTop = (screen.availHeight / 2 - 450);

        // Apertura della ModalDialog
        window.showModalDialog(_firmaMassivaDocumentiUrl, 
                'FirmaMassivaDocumenti', 
                "dialogWidth:500px;dialogHeight:450px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no", '');
    }

    // Funzione per l'apertura del popup di conversione massiva
    function OpenConversioneMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        window.showModalDialog(
                _convertiMassivaDocumentiUrl,
                'ConversioneMassivo',
                'dialogWidth:500px;dialogHeight:500px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

    }

    // Funzione per l'apertura del popup per lo spostamento di documenti / fascicoli nell'ADL
    function OpenADLMassiva(objType) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        window.showModalDialog(
                _adlMassivo + "?objType=" + objType,
                'ADLMassivo',
                'dialogWidth:500px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');

    }

    // Funzione per l'apertura del popup per l'esportazione dei risultati della ricerca
    function OpenExportDialog(obj) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        window.showModalDialog(
                _exportResult + "?export=" + obj + "&fromMassiveOperation=1",
                'Export',
                'dialogWidth:500px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;');

    }

    // Funzione per l'apertura del popup per l'inoltro massivo
    function OpenInoltraDialog() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight / 2 - 75);

        // Apertura della ModalDialog
        var a = window.showModalDialog(
                _inoltro,
                'InoltroMassivo', 
                'dialogWidth:500px;dialogHeight:250px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;',
                '');
        
        if(a)
            top.principale.iFrame_sx.document.location = '<%=UrlToDocumentPage %>';
    }

    // Funzione per l'apertura del popup per l'inoltro massivo
    function OpenEliminaVersioniDialog() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight / 2 - 75);

        // Apertura della ModalDialog
        var a = window.showModalDialog(
                _elVersioni,
                'EliminaVersioni', 
                'dialogWidth:450px;dialogHeight:150px;status:no;resizable:yes;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;',
                '');
        
        if(a)
            top.principale.iFrame_sx.document.location = '<%=UrlToDocumentPage %>';
    }
        

    // Numero di check rimanenti per poter visualizzare il testo "Seleziona Tutti"
    var toSelectAll = <%=SelectNumberToSelectAll %>;
   
    // Funzione registrata all'evento on client click dei checkbox del datagrid
    // Si occupa di gestire il pulsante seleziona, seleziona tutto lato javascript
    function ceckSelectDeselectAll(checkBoxId, selectDeselectAll, hiddenField) { 
        
        if(checkBoxId.checked)
            toSelectAll--;
        else
            toSelectAll++;
            
        if(toSelectAll == 0)
        {
            
            selectDeselectAll.checked = true;
            selectDeselectAll.nextSibling.innerHTML = "Deseleziona tutti.";

        }
        else
        {
            
            selectDeselectAll.checked = false;
            selectDeselectAll.nextSibling.innerHTML = "Seleziona tutti.";
        }

        hiddenField.value = "1";
        
    }

</script>
<!-- Firma massiva di documenti-->
<asp:ImageButton ID="btnSign" runat="server" AlternateText="Firma" Visible="false"
    ToolTip="Firma tutti i documenti selezionati"
    ImageUrl="~/images/bottoniera/icon_p7m.gif" OnClick="btnSign_Click" />
&nbsp;
<!-- Fascicolazione massiva -->
<asp:ImageButton ID="btnFascicola" runat="server" AlternateText="Fascicola" Visible="false"
    ToolTip="Fascicola i documenti selezionati" 
    ImageUrl="~/images/classificaADL.gif" onclick="btnFascicola_Click" />
&nbsp;
<!-- Trasmissione massiva -->
<asp:ImageButton ID="btnTransmit" runat="server" AlternateText="Trasmetti"
    ImageUrl="~/images/bottoniera/IcoTrasmissioniMultiple.gif" 
    onclick="btnTransmit_Click" />
&nbsp;
<!-- Metti in area di lavoro -->
<asp:ImageButton ID="btnWorkingArea" runat="server" AlternateText="Area di lavoro"
    Visible="false" OnClick="btnWorkingArea_Click"
    ImageUrl="~/images/proto/area_new.gif" />
&nbsp;
<!-- Esportazione massiva -->
<asp:ImageButton ID="btnExport" runat="server" AlternateText="Esporta risultati" OnClick="btnExport_Click"
    ImageUrl="~/images/proto/export_1.gif" />
&nbsp;
<!-- Associazione timespamp massivo -->
<asp:ImageButton ID="btnTimeStamp" runat="server" AlternateText="Timestamp" Visible="false"
    ToolTip="Assegna timestamp ai documenti selezionati" 
    ImageUrl="~/App_Themes/ImgComuni/timestamp.jpg" onclick="btnTimeStamp_Click" />
&nbsp;
<!-- Conversione PDF massiva -->
<asp:ImageButton ID="btnConvert" runat="server" Visible="false" AlternateText="Converti"
    ToolTip="Converti in PDF i documenti selezionati" 
    ImageUrl="~/images/bottoniera/icon_pdf.gif" OnClick="btnConvert_Click" 
    style="height: 16px" />
&nbsp;
<!-- Inoltra massivo -->
<asp:ImageButton ID="btnInoltra" runat="server" Visible="false" AlternateText="Inoltra"
    ToolTip="Crea un documento da inoltrare con i documenti selezionati" 
    ImageUrl="~/images/proto/inoltra.gif" OnClick="btnInoltra_Click" 
    style="height: 16px" />
&nbsp;
<!-- Elimina versioni -->
<asp:ImageButton ID="btnEliminaVersioni" runat="server" Visible="false" AlternateText="Elimina Versioni"
    ToolTip="Elimina le vecchie versioni dei documenti selezionati" 
    ImageUrl="~/images/proto/cancella.gif" OnClick="btnElVersioni_Click" 
    style="height: 16px" />
&nbsp;
<uc2:DocumentConsolidation id="documentConsolidationCtrl" runat="server" Visible="false"></uc2:DocumentConsolidation>
&nbsp;
<!-- Seleziona / Deseleziona tutti -->
<asp:HiddenField ID="hfSelectedFromGrid" runat="server" Value="0" />
<asp:CheckBox ID="chkSelectDeselectAll" runat="server" Checked="false" Text="Seleziona tutti" CssClass="titolo_real"
    AutoPostBack="true" OnCheckedChanged="chkSelectDeselectAll_CheckedChanged" />

