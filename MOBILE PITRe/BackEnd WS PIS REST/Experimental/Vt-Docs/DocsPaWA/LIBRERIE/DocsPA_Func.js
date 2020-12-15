function AttendiClassifica() {
    window.document.body.style.cursor = 'default';
    top.principale.iFrame_dx.document.location = "../AttendiClassifica.htm";

}
function MyDialogArguments() {
    this.Sender = null;
    this.StringValue = "";
}

function OpenMyDialog(sVal) {
    //var dlgArgs;
    //dlgArgs.window=window;
    dialogArgs = new MyDialogArguments();
    dialogArgs.StringValue = sVal;
    dialogArgs.Sender = window;
    var screeny = window.screen.availHeight;
    var screenx = window.screen.availWidth;



    var retVal = window.showModalDialog(sVal, dialogArgs, 'dialogWidth:' + screenx + 'px;dialogHeight:' + screeny + 'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
}

//modifica del 12/05/2209
function setfocusconboragioni(id) {
    if ((document.getElementById(id) == 'undefined') ||
     (document.getElementById(id) == null) || (document.getElementById(id).value == '-1'))
        document.getElementById(id).focus();
}
//fine modifica del 12/05/2009

function EndAttendiClassifica() {
    top.principale.iFrame_dx.document.location = "../tabdoc.aspx";
}
function ShowWaitingDonwloadPage() {
    top.principale.iFrame_dx.document.iFrameDoc.location = "../waitingDownloadpage.htm";
}
function WndWait() {


    top.principale.iFrame_dx.document.location = "../waitingPage.htm";
}
function WndWaitTrasm() {


    try { top.principale.iFrame_dx.document.location = "waitingPage.htm"; }
    catch (e) { }
}
function WndWaitStampaReg() {


    top.principale.iFrame_dettagli.document.location = "../../waitingPageReg.htm";
}

function wndAlt() {
    alert('Attenzione: \nVisualizza Old \nrichiede la selezione di un \nMittente/Destinatario');
}

function ApriFrame(pagina, target) {
    win = window.open(pagina, target, "toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
    win.focus();
}

function ApriFinestra(form, pagina, titolo) {
    win = window.open(pagina, titolo, "width=450,height=150, scrollbars=yes");
    win.focus();
}

function ChangeCursorT(tipo, name) {
    try {
        var oggetto = window.document.all(name);
        if (tipo == "default")
            oggetto.style.cursor = 'default';
        if (tipo == "hand")
            oggetto.style.cursor = 'hand';
    }
    catch (exc)
	{ ; }
}

function RefreshDestro(pagina) {
    try {
        alert("start refresh dx");
        top.principale.iFrame_dx.document.location = "'" + pagina + "'";
        alert("end refresh dx");
    }
    catch (e) {
        alert(e);
    }
}

function RefreshSinistro(pagina) {
    try {
        alert("start refresh sx");
        top.principale.iFrame_sx.document.location = "'" + pagina + "'";
        alert("end refresh sx");
    }
    catch (e) {
        alert(e);
    }
}

function ApriFinestraGen(pagina, titolo, param) {
    win = window.open(pagina, titolo, param);
    win.focus();
}

function ApriFinestraCatenaDoc() {

    win = window.open("../popup/catenaDocumento.aspx", "CatenaDocumentale", "width=580,height=390,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");

    win.focus();
}


//per i cookie
function setCookie(cookieName, cookieValue, cookiePath, cookieExpires) {
    cookieValue = escape(cookieValue);
    if (cookieExpires == "") {
        var nowDate = new Date();
        nowDate.setMonth(nowDate.getMonth() + 6);
        cookieExpires = nowDate.toLocaleString();
    }
    if (cookiePath != "") {
        cookiePath = "Path=" + cookiePath;
    }
    document.cookie = cookieName + "=" + cookieValue + " ; expires=" + cookieExpires + cookiePath;
}

function getCookieValue(cookieName) {
    var cookieValue = document.cookie;
    var cookieSrartsAt = cookieValue.indexOf(" " + cookieName + "=");
    if (cookieSrartsAt == -1) {
        cookieSrartsAt = cookieValue.indexOf(cookieName + "=");
    }
    if (cookieSrartsAt == -1) {
        cookieSrartsAt = null;
    }
    else {
        cookieSrartsAt = cookieValue.indexOf("=", cookieSrartsAt) + 1;
        var cookieEndsAt = cookieValue.indexOf(";" + cookieSrartsAt);
        if (cookieEndsAt == -1) {
            cookieEndsAt = cookieValue.length;
        }
        cookieValue = unescape(cookieValue.substring(cookieSrartsAt, cookieEndsAt));
    }
    return cookieValue;
}

// oggettario.aspx
function ApriOggettario(wnd) {
    var left = (screen.availWidth - 595);
    var top = (screen.availHeight - 620);
    win = window.open("../popup/oggettario.aspx?wnd=" + wnd, "Oggettario", "width=565,height=380,top=" + top + ",left=" + left + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");

    win.focus();
}

// oggettario.aspx
function ApriModalOggettario(wnd) {
    var left = (screen.availWidth - 595);
    var top = (screen.availHeight - 620);
    var args = new Object;
    args.window = window;

    window.showModalDialog('../popup/oggettario.aspx?wnd=' + wnd, args, 'dialogWidth:575px;dialogHeight:420px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
}

// acquisisciDocumento.aspx
function ApriAcquisisciDocumento() {
    var args = new Object;
    args.window = window;

    return window.showModalDialog('../popup/acquisisciDocumento.aspx', args, 'dialogWidth:425px;dialogHeight:290px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
}

//Tasto LogOut
function ReturnLoginPage() {
    window.open('login.aspx', '_top');
}

//ddl
function CreaDescr(descr) {
    document.writeln("<DIV ID='div1' class='titolo_biancoDiv'>" + descr + "</DIV>");
}
// rubrica.aspx

function DocsPa_FuncJS_WaitWindows() {
    DocsPa_FuncJS_WaitPopup(top, top.principale.document.all("iFrame_dx"));
}

function DocsPa_FuncJS_WaitForExecuting(win) {
    DocsPa_FuncJS_WaitPopup(top, win);
}

function DocsPa_FuncJS_WaitPopup(winInWaiting, winInLoading) {
    try {
        top.waitingManagerJS_startWaiting(winInWaiting, winInLoading);
    }
    catch (exc)
	{ ; }
}
function ApriRubrica(wnd, target) {
    var top = 80;
    var left = 300;
    var height = 600;
    try {
        if (window.screen.availWidth < 1000) {
            top = 40;
            left = 150;
            height = 450;
        }

        if (target != "") {
            win = window.open("../popup/rubricaDT.aspx?wnd=" + wnd + "&target=" + target, "Rubrica", " top=" + top + ",left=" + left + ", width=600,height=" + height + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
        }
        else
            win = window.open("../popup/rubricaDT.aspx?wnd=" + wnd, "Rubrica", "top=" + top + ", left=" + left + ", width=600,height=" + height + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
        DocsPa_FuncJS_WaitPopup(win, null);
        win.focus();
    }
    catch (exc)
	{ ; }
}

function ApriRubricaDaRicFasc(wnd, target) {
    var top = 80;
    var left = 300;
    var height = 600;
    try {
        if (window.screen.availWidth < 1000) {
            top = 40;
            left = 150;
            height = 450;
        }
        if (target != "") {
            win = window.open("../popup/rubricaDT.aspx?wnd=" + wnd + "&target=" + target + "&clear=1", "Rubrica", " top=" + top + ",left=" + left + ", width=600,height=" + height + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
        }
        DocsPa_FuncJS_WaitPopup(win, null);
        win.focus();
    }
    catch (exc)
	{ ; }
}
function ApriRubricaSemplice(wnd, target) {
    try {
        if (target != "") {
            win = window.open("../popup/rubricaSemplice.aspx?wnd=" + wnd + "&target=" + target, "Rubrica", " top=80,left=300, width=600,height=400,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
        }
        DocsPa_FuncJS_WaitPopup(win, null);
        win.focus();
    }
    catch (exc)
	{ ; }
}

function InserisciCorrispondente() {
    win = window.open("../popup/InsDest.aspx", "InserimentoCorr", " top=80,left=300, width=600,height=580,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
}


//dettagli cor
/*function ApriFinestraCor(url,indexCor,tipoCorr)
{
//controllo selezione corrispondente  
var tipoCor = url.substr(url.length-1,1);
var newLeft=(screen.availWidth-602);
var newTop=(screen.availHeight-600);	
				
if (tipoCor == "M" || tipoCor == "I")
{
if(indexCor =="")
{ 
alert("Dati sul Mittente non trovati");
return;
}
var urlPar = url + "&indexCor=" + indexCor;			
window.open(urlPar,"DettagliCor","toolbar=no,location=no,directories=no, scrollbars=yes,status=no,top="+newTop+",left="+newLeft+"resizable=yes,copyhistory=no, width=470, height=510");

}
else if (tipoCor == "D" || tipoCor == "C")
{
if (indexCor == "-1")
{
alert("Destinatario non selezionato");
return;
}
var urlPar = url + "&indexCor=" + indexCor;			
window.open(urlPar,"DettagliCor","toolbar=no,location=no,directories=no, scrollbars=yes,top="+newTop+",left="+newLeft+"status=no,resizable=yes,copyhistory=no, width=470, height=530");

}
}*/

//storiaModifiche
function ApriFinestraStoriaMod(tipoObj) {
    var left = (screen.availWidth - 598);
    var top = (screen.availHeight - 620);
    var urlPar = "../popup/storiaObj.aspx?" + "tipo=" + tipoObj;
    window.open(urlPar, "StoriaModifiche", "toolbar=no,location=no,directories=no, status=no,scrollbars=no,resizable=no,copyhistory=no,top=" + top + ",left=" + left + ", width=580, height=350");
}

//dettagli Firmatari
function ApriFinestraFirmatari(url, codInfo) {
    var urlPar = url + "?codInfo=" + codInfo;
    window.open(urlPar, "Firmatari", "toolbar=no,location=no,directories=no, status=no,scrollbars=yes,resizable=yes,copyhistory=no, width=500, height=270");
}

function ApriFinestraSceltaFirma(url) {
    window.open(url, "SceltaFirma", "toolbar=no,location=no,directories=no, status=no,scrollbars=yes,resizable=yes,copyhistory=no, width=500, height=240");
}

//annullaProtocollo.aspx
function ApriFinestraAnnullaProto() {
    var args = new Object;
    args.window = window;

    var retValue = window.showModalDialog("../popup/annullaProtocollo.aspx",
				args,
				"dialogWidth:450px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no");

    return retValue;
}

//rimuoviProfilo.aspx
function ApriFinestraRimuoviProfilo(nomeForm) {
    var retValue = false;
    try {
        var newLeft = ((screen.availWidth - 360) / 2);
        var newTop = ((screen.availHeight - 220) / 2);
        var retValue = window.showModalDialog("../popup/rimuoviProfilo.aspx?prov=" + nomeForm, "RimuoviProfilo", "dialogWidth:360px;dialogHeight:220px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
        if (retValue == null)
            retValue = false;
        if (retValue == true && nomeForm != "") {
            if (nomeForm == "toDoList") {
                top.principale.iFrame_dx.document.location = '../TodoList/toDoList.aspx?type=D&tiporic=R&home=Y';
            }
        }
    }
    catch (e) { }
    return retValue;
}

function ApriFinestraRimuoviAllegato() {
    var retValue = false;
    try {
        var url = "../popup/rimuoviAllegato.aspx";
        var retValue = window.showModalDialog(url,
	                        null, 'dialogWidth:360px;dialogHeight:150px;status:no;resizable:no;scroll:no;help:no;close:no');

        if (retValue == null)
            retValue = false;
    }
    catch (e) { }
    return retValue;
}

//rimuoviVisibilita.aspx
function ApriFinestraRimuoviVisibilita() {
    win = window.open("../popup/rimuoviVisibilita.aspx", "RimuoviVisibilita", "width=360,height=220,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
    win.focus();

    //var newLeft=(screen.availWidth-605);
    //var newTop=(screen.availHeight-640);
    //window.showModalDialog("../popup/rimuoviVisibilita.aspx", "RimuoviVisibilita","dialogWidth:360px;dialogHeight:220px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";");

}




//rimuoviVersione.aspx
function ApriFinestraRimuoviVersione(versioneSelezionata) {
    var args = new Object;
    args.window = window;

    var retValue = window.showModalDialog("../popup/rimuoviVersione.aspx?versioneSelezionata=" + versioneSelezionata,
				args,
				"dialogWidth:360px;dialogHeight:180px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no");

    return retValue;
}

//paroleChiave.aspx
function ApriFinestraParoleChiave(wnd) {
    var left = (screen.availWidth - 595);
    var top = (screen.availHeight - 620);
    win = window.open("../popup/paroleChiave.aspx?wnd=" + wnd, "ParoleChiave", "width=480,height=380,top=" + top + ",left=" + left + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
    win.focus();
}

//NewparoleChiave.aspx
function ApriFinestraParoleChiaveAdvanced(wnd) {
    var left = (screen.availWidth - 595);
    var top = (screen.availHeight - 620);
    win = window.open("../popup/newParoleChiave.aspx?wnd=" + wnd, "ParoleChiave", "width=520,height=356,top=" + top + ",left=" + left + ",toolbar=no,directories=no,menubar=no,resizable=no, scrollbars=no");
    win.focus();
}

//visibilitaDocumento.aspx
//function ApriFinestraVisibilita()
//{
//   	var newLeft=(screen.availWidth-605);
//	var newTop=(screen.availHeight-640);
//	win=window.open("../popup/visibilitaDocumento.aspx","Visibilita","width=588,height=450,top="+newTop+",left="+newLeft+",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no"); 
//	win.focus(); 
//	//win_position(win);
// 	
//}

//storiaVisibilitaDocumento.aspx
function ApriFinestraStoriaVisibilita(tipoObj) {
    var newLeft = (screen.availWidth - 605);
    var newTop = (screen.availHeight - 640);
    var urlPar = "../popup/storiaVisibilitaDocumento.aspx?" + "tipo=" + tipoObj;
    window.showModalDialog(urlPar, "Storia visibilita", "dialogWidth:588px;dialogHeight:505px;status:no;resizable:no;scroll:yes;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
}

//visualizzaLog.aspx
function ApriFinestraLog(tipoObj) {
    var newLeft = (screen.availWidth - 605);
    var newTop = (screen.availHeight - 640);
    var urlPar = "../popup/visualizzaLog.aspx?" + "tipo=" + tipoObj;
    window.showModalDialog(urlPar, "Storia log", "dialogWidth:600px;dialogHeight:525px;status:no;resizable:no;scroll:auto;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
}

//visibilitaDocumento.aspx
function ApriFinestraVisibilita() {
    var left = (screen.availWidth - 605);
    var top = (screen.availHeight - 670);
    //window.showModalDialog("../popup/visibilitaDocumento.aspx", "Visibilita", "dialogWidth:700px;dialogHeight:620px;status:no;resizable:no;scroll:auto;center:no;help:no;close:no;dialogLeft:" + left + ";dialogTop:" + top + ";");
    window.showModalDialog("../popup/visibilitaDocumento.aspx", "Visibilita", "dialogWidth:770px;dialogHeight:620px;status:no;resizable:no;scroll:auto;center:no;help:no;close:no;dialogLeft:" + left + ";dialogTop:" + top + ";");
    document.forms[0].submit();
    //win=window.open("../popup/visibilitaDocumento.aspx", "Visibilita","width=588,height=560,top="+top+",left="+left+",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no"); 
    //win.focus();
}

//visibilitaFascicolo.aspx
function ApriFinestraVisibilitaFasc() {
    var newLeft = (screen.availWidth - 605);
    var newTop = (screen.availHeight - 670);
    //window.showModalDialog("../popup/visibilitaFascicolo.aspx", "Visibilita", "dialogWidth:588px;dialogHeight:620px;status:no;resizable:no;scroll:auto;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
    window.showModalDialog("../popup/visibilitaFascicolo.aspx", "Visibilita", "dialogWidth:770px;dialogHeight:620px;status:no;resizable:no;scroll:auto;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
    document.forms[0].submit();
}

//finestra area di lavoro


function ApriFinestraADL(pagina) {
    win = window.open(pagina, "AreaLavoro", "width=680,height=450, scrollbars=yes");
    win.focus();
}

//gestioneFascicolo
//visibilitaDocumento.aspx
function ApriFinestraModificaFasc(profilazione) {
    if (profilazione == "1") {
        if (screen.height < 800) {
            rtnValue = window.showModalDialog("../popup/modificaFascicolo.aspx", "ModificaFascicolo", "dialogWidth:700px;dialogHeight:800px;status:no;resizable:no;scroll:yes;help:no;dialogLeft:500;dialogTop:30;center:no;");
        }
        else {
            rtnValue = window.showModalDialog("../popup/modificaFascicolo.aspx", "ModificaFascicolo", "dialogWidth:700px;dialogHeight:800px;status:no;resizable:no;scroll:auto;help:no;dialogLeft:500;dialogTop:30;center:no;");
        }
    }
    else {
        if (screen.height < 800) {
            returnValue = window.showModalDialog("../popup/modificaFascicolo.aspx", "ModificaFascicolo", 'dialogWidth:430px;dialogHeight:420px;status:no;resizable:no;scroll:yes;center:yes;help:no;close:yes;');
        }
        else {
            returnValue = window.showModalDialog("../popup/modificaFascicolo.aspx", "ModificaFascicolo", 'dialogWidth:430px;dialogHeight:420px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:yes;');
        }
    }

    window.parent.document.forms[0].submit();
}

function ApriFinestraGestAllegato(versionId) {
    var left = (screen.availWidth - 594);
    var top = (screen.availHeight - 545);

    url = "../popup/gestioneAllegato.aspx";

    if (versionId != "")
        url = url + "?versionId=" + versionId;

    return window.showModalDialog(url, null, 'dialogWidth:408px;dialogHeight:190px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
}

//gestioneVersioni.aspx
function ApriFinestraGestVersioni(index) {
    var left = (screen.availWidth - 594);
    var top = (screen.availHeight - 545);

    url = "../popup/gestioneVersioni.aspx";
    if (index != "") {
        url = url + "?index=" + index;
    }
    win = window.open(url, "Gestione_Versioni", "width=428,height=205,top=" + top + ",left=" + left + ",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
    win.focus();
}

//trasmissioniXRisposta.aspx
function ApriFinestraTrasmXRisp() {
    win = window.open("../popup/trasmissioniXRisp.aspx", "Trasmissioni", "width=700,height=500,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
    win.focus();
}

// gestione rgistri
function ApriFinestraGestioneReg(tipo) {
    win = window.open("../../popup/gestioneRegistro.aspx?tipo=" + tipo, "Registro", "width=500,height=400,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
    win.focus();
}

// Modifica Password registri
function ApriFinestraModificaPassReg() {
    win = window.open("../../popup/modificaPassRegistro.aspx", "Registro", "width=460,height=270,toolbar=no,directories=no,menubar=no,resizable=no, scrollbars=no");
    win.focus();
}

// oggettario.aspx
function setOggetto(wnd, desc) {
    var oggetto_sel = "";
    if (desc != "") {
        if (wnd == "ric_E") {
            window.opener.document.f_Ricerca_E.txt_oggetto.value = desc; //descOggetto;
        }
        else if (wnd == "ric_C") {
            window.opener.document.f_Ricerca_C.txt_ogg_C.value = desc; //descOggetto;
        }
        else if (wnd == "ric_CT") {
            window.opener.document.f_Ricerca_Compl.txt_oggetto.value = desc; //descOggetto;
        }
        else if (wnd == "ric_G") {
            window.opener.document.ricDocGrigia.txt_oggetto.value = desc; //descOggetto;
        }
    }
    window.close();
}

//rubrica.aspx
function setCorrispondenti(wnd) {
    var rubrica_sel = "";
    for (i = 0; i < document.rubrica.rb_Corr.length; i++) {
        if (document.rubrica.rb_Corr[i].checked == "1") {
            rubrica_sel = document.rubrica.rb_Corr[i]
        }
    }
    if (wnd == "ric_E") {
        window.opener.document.f_Ricerca_E.txt_codMit_E.value = rubrica_sel.value;
        window.opener.document.f_Ricerca_E.txt_descrMit_E.value = rubrica_sel.text;
    }
    else if (wnd == "ric_C") {
        window.opener.document.f_Ricerca_C.txt_codMit_E.value = rubrica_sel.value;
        window.opener.document.f_Ricerca_C.txt_descrMit_E.value = rubrica_sel.text;
    }
    else if (wnd == "ric_CT") {
        window.opener.document.f_Ricerca_CT.txt_codMit_E.value = rubrica_sel.value;
        window.opener.document.f_Ricerca_CT.txt_descrMit_E.value = rubrica_sel.text;
    }
    window.close();
}

function CaricaCertificati(select) {
    var store = new ActiveXObject("CAPICOM.Store");
    store.Open(2, "MY", 0);
    var cert = null;
    for (i = 1; i <= store.Certificates.Count; i++) {
        cert = store.Certificates(i);
        var option = document.createElement("OPTION");
        select.options.add(option);
        props = cert.SubjectName.split(",");
        for (j = 0; j < props.length; j++) {
            if (props[j].substr(0, 1) == " ")
                props[j] = props[j].substr(1);
            if (props[j].substr(0, 3) == "CN=")
                option.innerText = props[j].substr(3);
        }
        option.Value = i;
    }
}

function ApriFinestraNewFolder(page) {
    win = window.open("../popup/fascNewFolder.aspx?page=" + page, "NewFolder", "width=450,height=190,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
    win.focus();
}

function ApriFinestraNewFasc() {
    if (screen.height < 800) {
        win = window.open("../popup/fascNewFascicolo.aspx", "NewFascicolo", "width=420,height=450,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
    }
    else {
        win = window.open("../popup/fascNewFascicolo.aspx", "NewFascicolo", "width=420,height=450,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=auto");
    }
    win.focus();
}

function ApriFinestraNewFascDadoc(val) {
    if (screen.height < 800) {
        win = window.open("../popup/fascNewFascicolo.aspx?val=" + val, "NewFascicolo", "width=420,height=450,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes");
    }
    else {
        win = window.open("../popup/fascNewFascicolo.aspx?val=" + val, "NewFascicolo", "width=420,height=450,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=auto");
    }
    win.focus();
}

// gestione inserimento nuovo fascicolo / nodo titolario
function ApriFinestraNewFascNewTit(val, from, page, profilazione, idTitolario) {
    var newUrl;
    newUrl = "../popup/" + page + "?val=" + val + "&from=" + from + "&idTit=" + idTitolario;

    if (from == "docClassifica") {
        top.principale.iFrame_dx.document.location = 'tabDoc.aspx';
    }

    if (page == "fascNewFascicolo.aspx") {
        if (from == "docClassifica" || from == "docProtocollo" || from == "docProfilo" || from == "protoSempl")
            newUrl = newUrl + "&newFasc=1&isDoc=1";
        else
            newUrl = newUrl + "&newFasc=1&isDoc=0";
        if (profilazione == "1") {
            //newUrl = newUrl + "&newFasc=1";
            if (screen.height < 800) {
                rtnValue = window.showModalDialog(newUrl, "insertNewFasc", "dialogWidth:700px;dialogHeight:800px;status:no;resizable:no;scroll:yes;help:no;dialogLeft:500;dialogTop:30;center:no;");
            }
            else {
                rtnValue = window.showModalDialog(newUrl, "insertNewFasc", "dialogWidth:700px;dialogHeight:800px;status:no;resizable:no;scroll:auto;help:no;dialogLeft:500;dialogTop:30;center:no;");
            }
            //rtnValue = window.open(newUrl,"","dialogWidth:420px;dialogHeight:500px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 
        }
        else {
            //newUrl = newUrl + "&newFasc=1";
            if (screen.height < 800) {
                rtnValue = window.showModalDialog(newUrl, "insertNewFasc", "dialogWidth:700px;dialogHeight:550px;status:no;resizable:no;scroll:auto;help:no;dialogLeft:500;dialogTop:30;center:no;");
            }
            else {
                rtnValue = window.showModalDialog(newUrl, "insertNewFasc", "dialogWidth:700px;dialogHeight:550px;status:no;resizable:no;scroll:auto;help:no;dialogLeft:500;dialogTop:30;center:no;");
            }
            //rtnValue = window.open(newUrl,"","dialogWidth:420px;dialogHeight:410px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 
        }
    }
    else {
        if (screen.height < 800) {
            rtnValue = window.showModalDialog(newUrl, "insertNewTit", "dialogWidth:420px;dialogHeight:450px;status:no;resizable:no;scroll:yes;center:yes;help:no;");
        }
        else {
            rtnValue = window.showModalDialog(newUrl, "insertNewTit", "dialogWidth:420px;dialogHeight:450px;status:no;resizable:no;scroll:auto;center:yes;help:no;");
        }
        //rtnValue = window.open(newUrl,"","dialogWidth:420px;dialogHeight:410px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 	    	    
    }


    if (rtnValue != "N") {
        if (from == "ricercaFascicoli") {
            window.document.fascicoli_sx.submit();

        }
        if (from == "docProtocollo") {

            window.document.docProtocollo.submit();

        }
        if (from == "docProfilo") {

            window.document.docProfilo.submit();
        }
        if (from == "protoSempl") {

            window.document.frmProtIngresso.submit();
        }


    }

    return rtnValue;
}

// titolario.aspx
function ApriTitolario(qrstr, isFasc) {
    var newUrl;

    newUrl = "../popup/titolario.aspx?" + qrstr + "&isFasc=" + isFasc;

    var newLeft = (screen.availWidth - 650);
    var newTop = (screen.availHeight - 740);

    // apertura della ModalDialog
    rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:650px;dialogHeight:720px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");

    if (rtnValue != "N") {
        if (isFasc == "gestFasc") {
            window.document.fascicoli_sx.submit();
        }
        if (isFasc == "gestClass") {
            window.document.docClassifica.submit();
        }
        if (isFasc == "gestArchivio") {
            window.document.opzioniArchivio.submit();
        }
        if (isFasc == "gestScarto") {
            window.document.opzioniScarto.submit();
        }
        if (isFasc == "gestDoc") {
            window.document.docProfilo.submit();
        }
        if (isFasc == "gestProt") {
            window.document.docProtocollo.submit();
        }
        // Mev Ospedale Maggiore Policlinico
        if (isFasc == "gestRiclassFasc") {
            window.document.fascDocumenti.submit();
        }
        // End Mev
    }
}

//ApriClassifica - ProponiClassifica.aspx NB: usato solo per prototipo
//gara 

function ApriClassifica() {
    var newUrl;
    newUrl = "../popup/ProponiClassifica.aspx";
    win = window.open(newUrl, "Titolario", "width=700,height=400,toolbar=no,directories=no,menubar=no,resizable=no, scrollbars=no");
    win.focus();
}



function ApriFinestraMassimizzata(pageUrl, targetName) {
    var maxWidth = window.screen.width;
    var maxHeight = window.screen.height;

    // script += "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
    //ie7:
    //script += "'location=0,resizable=yes');";

    var dimensionWindow = "width=" + maxWidth + ",height=" + maxHeight;
    win = window.open(pageUrl, targetName, "fullscreen=no,t oolbar=no,directories=no,menubar=yes,resizable=yes, scrollbars=yes," + dimensionWindow + "');");
    //win=window.open(pageUrl,targetName,"fullscreen=no,toolbar=no,"+dimensionWindow+",directories=no,menubar=yes,resizable=yes, scrollbars=yes"); 
    win.moveTo(0, 0);
    /*add massimo digregorio  
    * inserito try/catch x evitare errore javascript quando clicco su zoom documento due volte consecutive	
    */
    try {
        win.focus();
    }
    catch (e) {
    }
}

function ApriFinestraSpedisci() {
    win = window.open("../popup/docSpedisci.aspx", "Spedisci", "width=277,height=142,top=50,left=100,toolbar=no,directories=no,menubar=no,resizable=no, scrollbars=no");
    win.focus();
}

//scelta tipoSpedizione - doc partenza

function ApriFinestraSceltaTipoSped(editMode) {
        var url = "../popup/sceltaTipoSpedizione.aspx?&editMode=" + editMode;
        window.showModalDialog(url, "TipoSpedizione", "dialogWidth:450px;dialogHeight:300px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:yes;");
}

//templateTrasmissioni.aspx
function ApriFinestraListaTemplate(tipo) {
    //win=window.open("../popup/templateTrasmissioni.aspx?tipoObj=" + tipo,"Template","width=610,height=440,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=yes"); 
    win = window.showModalDialog("../popup/templateTrasmissioni_frame.aspx?tipoObj=" + tipo, "Template", 'dialogWidth:610px;dialogHeight:540px;status:no;resizable:yes;scroll:auto;center:yes;help:no;close:yes;');
    //var newLeft=(screen.availWidth-630);
    //var newTop=(screen.availHeight-593);
    //win.moveTo(newLeft,newTop);
    //win.focus(); 
}

function confermaCaricamentoTitolario() {
    var retValue = false;
    if (confirm('Attenzione: Si sta per visualizzare tutto il titolario. L\'operazione potrebbe richiedere qualche minuto. Procedere comunque?')) {
        retValue = true;
    }
    return retValue;
}

function submitClose(wnd, oggettoTrasm) {
    //per la webform DatagridRubrica
    try {
        if (true /*wnd != 'FiltriFascLF' && wnd != 'fascUffRef'*/) {
            window.document.frames[0].document.datagridRubrica.hiddenField.value = 'noexp';
            window.document.frames[0].document.datagridRubrica.hiddenField2.value = 'F';
            window.document.frames[0].document.datagridRubrica.submit();
        }
        else {
            window.close();
        }
    } catch (e) { }

    //per la finistra chiamante 			
    //if (wnd!=null && (wnd=="proto" || wnd=="protoS")) 
    //{
    //window.opener.document.forms[0].submit();
    //}
    //else 
    //if (wnd!=null && (wnd=="ric_E" || wnd=="ric_C" || wnd=="ric_CT" || wnd=="ric_Trasm"))
    //	{
    //window.opener.document.forms[0].submit();
    //	}
    //	else
    //	if (wnd!=null && wnd=="trasm")
    //	{
    //if(oggettoTrasm!=null && oggettoTrasm=="DOC")	
    //	window.opener.top.principale.iFrame_dx.document.trasmDatiTrasm_dx.submit();
    //else
    //	window.opener.top.principale.iFrame_dx.document.trasmDatiFascTras_dx.submit();
    //	}
}


function win_position(win) {
    var newLeft = (screen.availWidth - 570);
    var newTop = (screen.availHeight - 560);
    win.moveTo(newLeft, newTop);
}

//Funzioni per disabilitare i tasti
function addKeyHandler(element) {
    element._keyObject = new Array();
    element._keyObject["keydown"] = new Array();
    element._keyObject["keyup"] = new Array();
    element._keyObject["keypress"] = new Array();

    element.addKeyDown = function (keyCode, action) {
        element._keyObject["keydown"][keyCode] = action;
    }

    element.removeKeyDown = function (keyCode) {
        element._keyObject["keydown"][keyCode] = null;
    }

    element.addKeyUp = function (keyCode, action) {
        element._keyObject["keyup"][keyCode] = action;
    }

    element.removeKeyUp = function (keyCode) {
        element._keyObject["keyup"][keyCode] = null;
    }

    element.addKeyPress = function (keyCode, action) {
        element._keyObject["keypress"][keyCode] = action;
    }

    element.removeKeyPress = function (keyCode) {
        element._keyObject["keypress"][keyCode] = null;
    }

    function handleEvent() {
        var type = window.event.type;
        var code = window.event.keyCode;

        if (element._keyObject[type][code] != null)
            element._keyObject[type][code]();
    }

    element.onkeypress = handleEvent;
    element.onkeydown = handleEvent;
    element.onkeyup = handleEvent;
}


function BackPressDisable() {
    //alert('hai spinto back');
    window.history.go(window.history.length - 1);
    window.event.keyCode = 0;
}
//Fine Funzioni per disabilitare i tasti

function loadvisualizzaDoc(id, docnumber, idprofile) {
    win = window.open("../documento/docVisualizzaFrame.aspx?id=" + id + "&docnumber=" + docnumber + "&idprofile=" + idprofile, "_blank", "width=800px,height=600px,top=50,left=100,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes");
    win.focus();
}

function loadVisualizzatoreDocModal(id, docnumber, idprofile) {
    var args = new Object;
    args.window = window;

    window.showModalDialog("../documento/docVisualizzaFrame.aspx?id=" + id + "&docnumber=" + docnumber + "&idprofile=" + idprofile,
	            args,
	            'dialogWidth:' + screen.availWidth + 'px;dialogHeight:' + screen.availHeight + 'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
}

function loadVisualizzatoreDocModalFromInterop(id, docnumber, idprofile) {
    var args = new Object;
    args.window = window;

    window.showModalDialog("../../documento/docVisualizzaFrame.aspx?id=" + id + "&docnumber=" + docnumber + "&idprofile=" + idprofile,
	            args,
	            'dialogWidth:' + screen.availWidth + 'px;dialogHeight:' + screen.availHeight + 'px;status:no;resizable:yes;scroll:no;center:yes;help:no;close:no');
}

function calcTesto(f, l, nomeCampo, campoCaratteri) {


    StrLen = f.value.length
    if (StrLen > l) {
        f.value = f.value.substring(0, l)
        CharsLeft = 0
        window.alert("Lunghezza " + nomeCampo + " eccessiva: " + StrLen + " caratteri, max " + l);
    } else {
        CharsLeft = l - StrLen
    }
    campoCaratteri.value = CharsLeft





}
