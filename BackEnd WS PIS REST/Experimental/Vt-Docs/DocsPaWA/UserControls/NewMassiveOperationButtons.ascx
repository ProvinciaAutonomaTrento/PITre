<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewMassiveOperationButtons.ascx.cs"
    Inherits="DocsPAWA.UserControls.NewMassiveOperationButtons" %>
<%@ Register Src="~/UserControls/DocumentConsolidation.ascx" TagName="DocumentConsolidation"
    TagPrefix="uc2" %>
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
    // L'URL della finestra per la rimozione massiva dall'ADL
    var _removeAdlMassivo = '<%=GetMassiveRemoveWorkingAreaURL%>';
    // L'URL della finestra per l'esportazione dei risultati della ricerca
    var _exportResult = '<%=GetExportResultURL%>';
    // L'URL della finestra per la creazione del documento da inoltrare
    var _inoltro = '<%=GetInoltraMassivoURL %>';
    // L'URL della finestra per l'inserimento massivo di documenti in area di conservazione
    var _conservazione = '<%=GetMassiveStorageURL %>';
    // L'URL della finestra per la personalizzazione delle griglie
    var _grids = '<%=UrlToGridManagement %>';
    // L'URL della funzione per l'eliminazione delle versioni
    var _elVersioni='<%=GetEliminaVersioniURL %>';
    //Serve per il refresh delle pagine dopo la modifica della personalizzazione
    var _pagina_chiamante ='<%=PAGINA_CHIAMANTE%>';
    //Serve per il refresh delle pagine numero dei risultati
    var _num_result ='<%=NUM_RESULT%>';
    //L'URL della finestra per ritornare al default delle griglie
  //  var _gridsDefault ='<%=UrlToDefaultGrid %>';
        //L'URL della finestra per le grigle preferite
    var _gridspreferred = '<%=UrlPreferredGrid %>';
    //L'URL della finestra per salvare le griglie
    var _gridsave = '<%=UrlSaveGrid %>';


    // Funzione per l'apertura del popup di fascicolazione massiva
    function OpenFascicolaMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        showModal(_fascicolazioneMassivaUrl, 'FascicolazioneMassiva', newLeft,newTop);
        window.document.getElementById('<%= hfOperationDone.ClientID %>').value = '1';

    }

    // Funzione per l'apertura del popup di trasmissione massiva
    function OpenTrasmissioneMassiva() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 450);
        var newTop = (screen.availHeight / 2 - 400);
        var dialogArgs=new Object();
		dialogArgs.window = window;
        dialogArgs.title='TrasmissioneMassiva';
        // Apertura della ModalDialog
        window.showModalDialog(_trasmissioneMassivaUrl, dialogArgs, 'dialogWidth:900px;dialogHeight:540px;status:no;resizable:yes;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
       // top.principale.iFrame_dx.document.location = '<%= PageToReload %>';
       window.document.getElementById('<%= Page.Form.ClientID %>').submit();
    }

    // Funzione per l'apertura del popup di timestamp massivo
    function OpenTimestampMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 75);

        // Apertura della ModalDialog
        showModal(_timestampMassivoUrl,'TimestampMassivo',newLeft,newTop);

    }

    // Funzione per la firma digitale di più documenti
    function OpenFirmaMassivaDocumenti() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 525);
        var newTop = (screen.availHeight / 2 - 450);
        var dialogArgs=new Object();
		dialogArgs.window = window;
        dialogArgs.title='FirmaMassivaDocumenti';
        // Apertura della ModalDialog
        window.showModalDialog(_firmaMassivaDocumentiUrl, 
                dialogArgs, 
                "dialogWidth:430px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no", '');
        reload();
    }

    // Funzione per l'apertura del popup di conversione massiva
    function OpenConversioneMassivo() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        showModal(_convertiMassivaDocumentiUrl,'ConversioneMassivo',newLeft,newTop);

    }

    // Funzione per l'apertura del popup per lo spostamento di documenti / fascicoli nell'ADL
    function OpenADLMassiva(objType) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);

        // Apertura della ModalDialog
        showModal(_adlMassivo + "?objType=" + objType,'ADLMassivo',newLeft,newTop);
        reload();
    }

        // Funzione per l'apertura del popup per la rimozione di documenti / fascicoli dall'ADL
    function OpenRemoveADLMassiva(objType) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);

        //Apertura della ModalDialog
        showModal(_removeAdlMassivo + "?objType=" + objType,'ADLMassivo',newLeft,newTop);
        reload();
    }

    // Funzione per l'apertura del popup per l'esportazione dei risultati della ricerca
    function OpenExportDialog(obj) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 85);
        var newTop = (screen.availHeight / 2 - 60);
        var dialogArgs=new Object();
		dialogArgs.window = window;
        dialogArgs.title='Export';
        // Apertura della ModalDialog
        window.showModalDialog(
                 _exportResult + "?export=" + obj + "&fromMassiveOperation=1",
                dialogArgs,
                'dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;');

    }

    // Funzione per l'apertura del popup per l'inoltro massivo
    function OpenInoltraDialog() {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight / 2 - 75);

        // Apertura della ModalDialog
        var a = showModal(
                _inoltro,
                'InoltroMassivo', 
                newLeft,
                newTop);
        if(a)
            top.principale.document.location = '../documento/gestioneDoc.aspx?tab=protocollo';
    }

    function OpenGrids(gridType, gridId, templateId, ricADL) {
        var newLeft = (screen.availWidth / 2 - 450);
        var newTop = (screen.availHeight / 2 - 300);
        
        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        var retval = window.showModalDialog(
            _grids + "?gridType=" + gridType + "&gridId=" + gridId + "&templateId=" + templateId + "&tabRes=" + _pagina_chiamante,
            'GridCustomization',
            'dialogWidth:950px;dialogHeight:750px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', ''); 
           
        if(retval !=null){
            if(retval=="estesa" || retval=="completa" || retval=="completamento" || retval=="veloce" || retval=="StampaReg" || retval=="StampaRep"){
                top.principale.document.iFrame_sx.location='tabGestioneRicDoc.aspx?gridper=' + gridType + '&tab=' + retval + adlPar + "&numRes=" + _num_result;
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
            }
            if(retval=="fascicoli")
            {
                top.principale.document.iFrame_sx.location='gestioneRicFasc.aspx?gridper=' + gridType + adlPar + "&numRes=" + _num_result;
                top.principale.iFrame_dx.document.location = '../waitingpage.htm';
            }
            if(retval=="docInFasc")
            {
                top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx';        
            }
        }
  }  

    function ReturnDefaultGrid(gridType, ricADL){
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight / 2 - 75);
        
        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        var retval = window.showModalDialog(_gridsDefault+ "?gridType=" + gridType + "&tabRes="+ _pagina_chiamante, 'GridDefault','dialogWidth:400px;dialogHeight:200px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', ''); 
        if(retval!=null){
            if(retval=="estesa" || retval=="completa" || retval=="completamento"){
                    top.principale.document.iFrame_sx.location='tabGestioneRicDoc.aspx?gridper=' + gridType + '&tab=' + retval + adlPar+ "&numRes=" + _num_result;
                }
                if(retval=="fascicoli")
                {
                    top.principale.document.iFrame_sx.location='gestioneRicFasc.aspx?gridper=' + gridType + adlPar+ "&numRes=" + _num_result;
                    top.principale.iFrame_dx.document.location = '../waitingpage.htm';
                }
            }
    }

    // Funzione per l'apertura del popup di inserimento in area di conservazione massivo
    function OpenStorageMassivo(objectType) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Il path da richiamare sarà _conservazione + ?objType=<objectType>
        var path = this._conservazione + "?objType=" + objectType;

        // Apertura della ModalDialog
        showModal(path, 'ConservazioneMassiva', newLeft,newTop);

        reload();
    }

    // Funzione per l'apertura del popup per l'inoltro massivo
    function OpenEliminaVersioni(objectType) {
        // Calcolo della posizione della finestra in modo che sia posizionata 
        // al centro dello schermo
        var newLeft = (screen.availWidth / 2 - 175);
        var newTop = (screen.availHeight / 2 - 60);

        // Il path da richiamare sarà _conservazione + ?objType=<objectType>
        var path = this._elVersioni + "?objType=" + objectType;

        // Apertura della ModalDialog
        showModal(path,
                'EliminaVersioni', 
                newLeft,
                newTop);
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

    function showModal(url,label,newLeft,newTop){
        //Apertura della ModalDialog
        var dialogArgs=new Object();
		dialogArgs.window = window;
        dialogArgs.title=label;
        var a=window.showModalDialog(
                url,
                dialogArgs,
                'dialogWidth:420px;dialogHeight:150px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', '');
        return a;
    }

    function reload(reloadGrid){
        <%=JSFunction%>
        window.document.getElementById('<%= hfOperationDone.ClientID %>').value = '1';
		window.document.getElementById('<%= Page.Form.ClientID %>').submit();
    }

    function OpenPreferredGrids(gridType, ricADL){
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight - 704);
        
        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        

        var retval = window.showModalDialog(_gridspreferred+ "?gridType=" + gridType + "&tabRes="+ _pagina_chiamante, 'GridDefault','dialogWidth:700px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', ''); 
        if(retval!=null){
            if(retval=="estesa" || retval=="completa" || retval=="completamento" || retval=="veloce" || retval=="StampaReg" || retval=="StampaRep"){
                    top.principale.document.iFrame_sx.location='tabGestioneRicDoc.aspx?gridper=' + gridType + '&tab=' + retval + adlPar  + "&numRes=" + _num_result;
                    top.principale.iFrame_dx.document.location = '../waitingpage.htm';
                }
                if(retval=="fascicoli")
                {
                    top.principale.document.iFrame_sx.location='gestioneRicFasc.aspx?gridper=' + gridType + adlPar  + "&numRes=" + _num_result;
                    top.principale.iFrame_dx.document.location = '../waitingpage.htm';
                }
                if(retval=="docInFasc")
                {
                    top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx';        
                }
            }
    }

     function OpenSaveGrid(gridType, ricADL){
        var newLeft = (screen.availWidth / 2 - 225);
        var newTop = (screen.availHeight - 704);
        
        var adlPar = '';
        if (ricADL != undefined && ricADL == 'ricADL')
            adlPar = "&ricADL=1";

        

        var retval = window.showModalDialog(_gridsave+ "?gridType=" + gridType + "&tabRes="+ _pagina_chiamante, 'GridDefault','dialogWidth:600px;dialogHeight:450px;status:no;resizable:no;scroll:no;dialogLeft:' + newLeft + ';dialogTop:' + newTop + ';center:no;help:no;', ''); 
        if(retval!=null){
            if(retval=="estesa" || retval=="completa" || retval=="completamento" || retval=="veloce" || retval=="StampaReg" || retval=="StampaRep"){
                    top.principale.document.iFrame_sx.location='tabGestioneRicDoc.aspx?save=si&gridper=' + gridType + '&tab=' + retval + adlPar  + "&numRes=" + _num_result;
                    top.principale.iFrame_dx.document.location = '../waitingpage.htm';
                }
                if(retval=="fascicoli")
                {
                    top.principale.document.iFrame_sx.location='gestioneRicFasc.aspx?gridper=' + gridType + adlPar  + "&numRes=" + _num_result;
                    top.principale.iFrame_dx.document.location = '../waitingpage.htm';
                }
                if(retval=="docInFasc")
            {
                top.principale.document.iFrame_dx.location='tabPulsantiDoc.aspx';        
            }
            }
    }

</script>
<div style="float: left; width: 100%;">
    <div style="float: left;">
        <!--parametro che indica l'esecuzione di una operazione massiva-->
        <asp:HiddenField ID="hfOperationDone" runat="server" Value="0" />
        <asp:HiddenField ID="hd_pag_chiam" runat="server" />
        <asp:HiddenField ID="hd_pag_num" runat="server" />
        <!-- Firma massiva di documenti-->
        <asp:ImageButton ID="btnSign" runat="server" AlternateText="Firma" Visible="false"
            ToolTip="Firma tutti i documenti selezionati" ImageUrl="~/images/bottoniera/icon_p7m.gif"
            OnClick="btnSign_Click" Style="width: 15px" />
        &nbsp;
        <!-- Fascicolazione massiva -->
        <asp:ImageButton ID="btnFascicola" runat="server" AlternateText="Fascicola" Visible="false"
            ToolTip="Fascicola i documenti selezionati" ImageUrl="~/images/classificaADL.gif"
            OnClick="btnFascicola_Click" />
        &nbsp;
        <!-- Trasmissione massiva -->
        <asp:ImageButton ID="btnTransmit" runat="server" AlternateText="Trasmetti" ImageUrl="~/images/bottoniera/IcoTrasmissioniMultiple.gif"
            OnClick="btnTransmit_Click" Style="height: 16px; width: 16px" />
        &nbsp;
        <!-- Metti in area di lavoro -->
        <asp:ImageButton ID="btnWorkingArea" runat="server" AlternateText="Area di lavoro"
            Visible="false" ImageUrl="~/images/proto/area_new.gif" OnClick="btnWorkingArea_Click" />
        &nbsp;
        <!--Togli dall'area di lavoro-->
        <asp:ImageButton ID="btnRemoveWorkingArea" runat="server" AlternateText="Rimuovi dall'area di lavoro"
            Visible="false" ImageUrl="~/images/proto/cancella_area_lavoro.gif" ToolTip="Rimuovi dall'area di lavoro"
            OnClick="btnRemoveWorkingArea_Click" />
        &nbsp;
        <!-- Esportazione massiva -->
        <asp:ImageButton ID="btnExport" runat="server" AlternateText="Esporta risultati"
            ImageUrl="~/images/proto/export_1.gif" OnClick="btnExport_Click" />
        &nbsp;
        <!-- Associazione timespamp massivo -->
        <asp:ImageButton ID="btnTimeStamp" runat="server" AlternateText="Timestamp" Visible="false"
            ToolTip="Assegna timestamp ai documenti selezionati" ImageUrl="~/App_Themes/ImgComuni/timestamp.jpg"
            OnClick="btnTimeStamp_Click" />
        &nbsp;
        <!-- Conversione PDF massiva -->
        <asp:ImageButton ID="btnConvert" runat="server" Visible="false" AlternateText="Converti"
            ToolTip="Converti in PDF i documenti selezionati" ImageUrl="~/images/bottoniera/icon_pdf.gif"
            OnClick="btnConvert_Click" />
        &nbsp;
        <!-- Inoltra massivo -->
        <asp:ImageButton ID="btnInoltra" runat="server" Visible="false" AlternateText="Inoltra"
            ToolTip="Crea un documento da inoltrare con i documenti selezionati" ImageUrl="~/images/proto/inoltra.gif"
            Style="height: 16px" OnClick="btnInoltra_Click" />
        &nbsp;
        <!-- Elimina versioni -->
        <asp:ImageButton ID="btnEliminaVersioni" runat="server" Visible="false" AlternateText="Elimina Versioni"
            ToolTip="Elimina le vecchie versioni dei documenti selezionati" ImageUrl="~/images/simpleClearFlag2.gif"
            OnClick="btnElVersioni_Click" Style="height: 16px" />
        &nbsp;
        <uc2:DocumentConsolidation ID="documentConsolidationCtrl" runat="server" Visible="false">
        </uc2:DocumentConsolidation>
        &nbsp;
        <!-- Inserimento in area di conservazione -->
        <asp:ImageButton ID="btnStorage" runat="server" Visible="false" AlternateText="Area di conservazione"
            ToolTip="Inserisci i documenti selezionati nell'area di conservazione" ImageUrl="~/images/proto/conservazione_d.gif"
            OnClick="btnStorage_Click" />
        &nbsp;
        <!-- Seleziona / Deseleziona tutti -->
        <asp:HiddenField ID="hfSelectedFromGrid" runat="server" Value="0" />
    </div>
    <div style="float: right;">
        <asp:CheckBox ID="chkSelectDeselectAll" runat="server" Checked="false" Text="Seleziona tutti"
            CssClass="titolo_real" AutoPostBack="true" OnCheckedChanged="chkSelectDeselectAll_CheckedChanged"
            Enabled="false" />
    </div>
