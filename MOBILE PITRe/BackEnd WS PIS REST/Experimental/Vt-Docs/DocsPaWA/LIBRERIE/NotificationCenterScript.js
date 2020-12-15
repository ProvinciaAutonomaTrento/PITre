/*
 * Funzione per l'inizializzazione della finestra
 */
function initializeWindow() {
    // Associazione dell'evento change ai menù a tendina che prevedono possibilità di descidere
    // se scegliere per valore singolo o per intervallo
    document.getElementById('ddlProtoNum').onChange = showIntervalInfo('ProtoNum');
    document.getElementById('ddlRecDate').onchange = showIntervalInfo('RecDate');
}

/*
* Funzione per apertura della pagina di ricerca del centro notifiche, per l'analisi del risultato ottenuto
* e per il redirect alla pagina di dettaglio del protocollo
*/
function openSearchWindow() {
    var retVal = window.showModalDialog('NotificationCenter/NotificationCenterSearch.aspx', null, 'dialogWidth:800px; dialogHeight:500px; center:yes;');
    //var retVal = window.open('NotificationCenter/NotificationCenterSearch.aspx');
    if (retVal != null) {
        // Redirect verso la pagina di protocollo
        top.principale.location = 'documento/GestioneDoc.aspx?tab=protocollo&from=newRicDoc&idProfile=' + retVal + '&protoType=A';
        
    }
}

/*
 * Funzione per la gestione della visualizzazione delle label "Da" e "A" relative ad un filtro a seconda del tipo di ricerca
 * selezionata fra "Singolo valore" e "Intervallo"
 */
function showIntervalInfo(filterName) {
    // Recupero del valore selezionato per il menù a tendina
    var selectedVal = document.getElementById('ddl' + filterName).value;

    // Pulizia dei valori delle text box
    document.getElementById('txt' + filterName + 'From').value = '';
    document.getElementById('txt' + filterName + 'To').value = '';
    
    // Se value è S viene visualizzata solo la prima text box e vengono nascoste entrambe le label
    // altrimenti vengono visualizzate label e text box
    if (selectedVal == 'S') {
        document.getElementById('lbl' + filterName + 'From').style.visibility = 'hidden';
        document.getElementById('lbl' + filterName + 'To').style.visibility = 'hidden';
        document.getElementById('txt' + filterName + 'To').style.visibility = 'hidden';

    }
    else {
        document.getElementById('lbl' + filterName + 'From').style.visibility = '';
        document.getElementById('lbl' + filterName + 'To').style.visibility = '';
        document.getElementById('txt' + filterName + 'To').style.visibility = '';

    }
}

/*
 * Funzione per la pulizia dei filtri di ricerca
 */
function clearFilters() {
    // Tutti i menù a tendina devono avere il primo elemento selezionato
    document.getElementById('ddlProtoNum').selectedIndex = 1;
    document.getElementById('ddlRecDate').selectedIndex = 1;
    document.getElementById('ddlEvent').selectedIndex = 0;

    // Opportuna visualizzazione delle caselle di testo in base ai valori selezionati
    // nei vari menu a tendina
    showIntervalInfo('ProtoNum');
    showIntervalInfo('RecDate');

}