// Funzione per l'apertura della finestra sfoglia per cartelle
function PerformSelectFolder() {
    var folder = ShellWrappers_BrowseForFolder("Scegliere la cartella in cui esportare il fascicolo");

    if (folder != "") {
        var fso = FsoWrapper_CreateFsoObject();
        if (fso.FolderExists(folder)) {
            frmExpFasc.txtFolderPath.value = folder;
        }
    } else {
        return;
    }
}

// Esporta il fascicolo
function esporta() {

    try {
        // Se è stato selezionato un folder...
        if(frmExpFasc.txtFolderPath.value != "") {
            // ...visualizza la schermata di wait...
            ShowWaitingPage("L'esportazione può richiedere alcuni minuti...");
            // ...esporta il fascicolo
            frmExpFasc.hdResult.value = ExportPrj(frmExpFasc.txtFolderPath.value);

        }
        else {
            alert("Impostare una cartella di destinazione.");
        }
    }
    catch (e) {
        // ...chiude il form di wait
        CloseWaitingPage();
        alert(e.message.toString());
    }
    // ...chiude il form di wait
    CloseWaitingPage();
}

function lanciaVisPdf() {
    var w = window.screen.availWidth;
    var h = window.screen.availHeight;
    var dimensionWindow = "width=" + w + ",height=" + h;
    window.showModalDialog('visPdfReportFrame.aspx', '', 'dialogWidth:' + w + ';dialogHeight:' + h + ';status:no;resizable:yes;scroll:no;center:no;help:no;close:no;top:' + 0 + ';left:' + 0);
}

var wndAttendi;

function ShowWaitingPage(msg) {
    wndAttendi = window.open('tempPage.aspx', '_blank', "location=0,toolbar=0,scrollbars=0,resizable=0,width=350,height=10,left=350,top=350");
}

function CloseWaitingPage() {
    if (wndAttendi != null)
        wndAttendi.close();
}