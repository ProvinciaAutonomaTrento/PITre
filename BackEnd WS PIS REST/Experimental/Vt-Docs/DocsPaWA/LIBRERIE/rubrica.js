
function Rubrica_Apri()
{	
	var w_width = screen.availWidth-40;
	var w_height = screen.availHeight-5;
	var do_scroll = "no";
	var navapp = navigator.appVersion.toUpperCase();
	var opts = "dialogHeight:" + w_height + "px;dialogWidth:" + w_width + "px;center:yes;help:no;resizable:yes;status:no;unadorned:yes";
	var params = "calltype=" + this.CallType;
	//correzione per apertura rubrica con HTTPS:// 
	var newLoc = window.top.document.location.toString();
    var newParts = newLoc.split("/");
    var appoLoc = newParts[0];	
    var loc = window.top.document.location.toString().substring(appoLoc.length); 
	//fine correzione
	var parts = loc.split("/");
	var res = "/";

	// Variabile utilizzata per indicare che la pagina chiamante è una pagina
	// delle azioni massive
	var senderIsMassiveOperation = false;
	
	if (screen.availWidth < 1000)
	{
		do_scroll = "yes";
	}
		
	if (this.MoreParams != null) {
		this.MoreParams = this.MoreParams + "&do_scroll=" + do_scroll;
		params = params + "&" + this.MoreParams;
	}
	else
	{
		params = params + "&do_scroll=" + do_scroll;
	}	
	
	for (i = 1; i < (parts.length - 1); i++)
	{
		res = res + parts[i] + "/";
    }
	
	if (res != null) 
	{
	    //Apertura rubrica per protocollo
		if (res.indexOf("/popup") < 0)
		{
			urlRubrica = res + "popup/rubrica/Rubrica.aspx"
	    }
		else
		{
			urlRubrica = res + "rubrica/Rubrica.aspx"
        }

		//Apertura rubrica dalla cartella "admintool"
		if(res.toLowerCase().indexOf("admintool") >= 0)
		{
			//old: var str1 = res.toLowerCase().split("admintool");
			
			var str1 = res.substring(0,res.toLowerCase().indexOf("admintool"))
			
			urlRubrica = str1 + "popup/rubrica/Rubrica.aspx";
		}
		
		//Apertura rubrica dalla cartella "documento"
		if(res.toLowerCase().indexOf("documento") >= 0)
		{
				//old: var str1 = res.toLowerCase().split("admintool");
			
			var str1 = res.substring(0,res.toLowerCase().indexOf("documento"))
			urlRubrica = str1 + "popup/rubrica/Rubrica.aspx";

        }

        // Apertura rubrica dalla cartella "MassiveOperation"
        if (res.toLowerCase().indexOf("massiveoperation") >= 0) {
            var str1 = res.substring(0, res.toLowerCase().indexOf("massiveoperation"))
            urlRubrica = str1 + "popup/rubrica/Rubrica.aspx";

            senderIsMassiveOperation = true;

        }

        // Apertura rubrica dalla pagina delle impostazioni del repertorio
        if (res.toLowerCase().indexOf("modifyrepertoriosettings") >= 0) {
            var str1 = res.substring(0, res.toLowerCase().indexOf("modifyrepertoriosettings"))
            urlRubrica = str1 + "rubrica/Rubrica.aspx";

            senderIsMassiveOperation = true;

        }
		
		var res = window.showModalDialog (urlRubrica + "?" + params, window, opts);

		//window.open(urlRubrica + "?" + params);
		// Se non si proviene dalla pagina delle azioni massive o se moreParams e moreParams non contiene ajaxPage, 
        // si effettua il submit del chiamante
		if (!senderIsMassiveOperation && !(this.MoreParams && this.MoreParams.indexOf("ajaxPage") != -1))
		    window.document.forms[0].submit();
	}
	else
	{
		alert ("Errore! Impossibile aprire la rubrica.");
	}
}

function Rubrica()
{
    this.MoreParams = null;
}


new Rubrica();
// Costanti
Rubrica.prototype.CALLTYPE_PROTO_IN = 0;
Rubrica.prototype.CALLTYPE_PROTO_IN_INT = 1;
Rubrica.prototype.CALLTYPE_PROTO_OUT = 2;
Rubrica.prototype.CALLTYPE_TRASM_INF = 3;
Rubrica.prototype.CALLTYPE_TRASM_SUP = 4;
Rubrica.prototype.CALLTYPE_TRASM_ALL = 5;
Rubrica.prototype.CALLTYPE_MANAGE = 6;
Rubrica.prototype.CALLTYPE_PROTO_OUT_MITT = 7;
Rubrica.prototype.CALLTYPE_PROTO_INT_MITT = 8;
Rubrica.prototype.CALLTYPE_PROTO_INGRESSO = 9;
Rubrica.prototype.CALLTYPE_UFFREF_PROTO = 10;
Rubrica.prototype.CALLTYPE_GESTFASC_LOCFISICA = 11;
Rubrica.prototype.CALLTYPE_GESTFASC_UFFREF = 12;
Rubrica.prototype.CALLTYPE_EDITFASC_LOCFISICA = 13;
Rubrica.prototype.CALLTYPE_EDITFASC_UFFREF = 14;
Rubrica.prototype.CALLTYPE_NEWFASC_LOCFISICA = 15;
Rubrica.prototype.CALLTYPE_NEWFASC_UFFREF = 16;
Rubrica.prototype.CALLTYPE_RICERCA_MITTDEST = 17;
Rubrica.prototype.CALLTYPE_RICERCA_UFFREF = 18;
Rubrica.prototype.CALLTYPE_RICERCA_MITTINTERMEDIO = 19;
Rubrica.prototype.CALLTYPE_RICERCA_ESTESA = 20;
Rubrica.prototype.CALLTYPE_RICERCA_COMPLETAMENTO = 21;
Rubrica.prototype.CALLTYPE_RICERCA_TRASM = 22;
Rubrica.prototype.CALLTYPE_PROTO_INT_DEST = 23;
Rubrica.prototype.CALLTYPE_FILTRIRICFASC_UFFREF = 24;
Rubrica.prototype.CALLTYPE_FILTRIRICFASC_LOCFIS = 25;
Rubrica.prototype.CALLTYPE_RICERCA_DOCUMENTI = 26;
Rubrica.prototype.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT = 27;
Rubrica.prototype.CALLTYPE_LISTE_DISTRIBUZIONE = 28;
Rubrica.prototype.CALLTYPE_TRASM_PARILIVELLO = 29;
Rubrica.prototype.CALLTYPE_MITT_MODELLO_TRASM = 30;
Rubrica.prototype.CALLTYPE_MODELLI_TRASM_ALL = 31;
Rubrica.prototype.CALLTYPE_MODELLI_TRASM_INF = 32;
Rubrica.prototype.CALLTYPE_MODELLI_TRASM_SUP = 33;
Rubrica.prototype.CALLTYPE_MODELLI_TRASM_PARILIVELLO = 34;
Rubrica.prototype.CALLTYPE_DEST_MODELLO_TRASM = 35;
Rubrica.prototype.CALLTYPE_ORGANIGRAMMA=36;
Rubrica.prototype.CALLTYPE_ORGANIGRAMMA_TOTALE_PROTOCOLLO=37;
Rubrica.prototype.CALLTYPE_STAMPA_REG_UO=38;
Rubrica.prototype.CALLTYPE_ORGANIGRAMMA_TOTALE=39;
Rubrica.prototype.CALLTYPE_ORGANIGRAMMA_INTERNO=40;
Rubrica.prototype.CALLTYPE_RICERCA_TRASM_TODOLIST=41;
Rubrica.prototype.CALLTYPE_RUOLO_REG_NOMAIL=42;
Rubrica.prototype.CALLTYPE_UTENTE_REG_NOMAIL=43;
Rubrica.prototype.CALLTYPE_PROTO_USCITA_SEMPLIFICATO=44;
Rubrica.prototype.CALLTYPE_PROTO_OUT_MITT_SEMPLIFICATO=45;
Rubrica.prototype.CALLTYPE_CORR_INT=46;
Rubrica.prototype.CALLTYPE_CORR_EST=47;
Rubrica.prototype.CALLTYPE_CORR_INT_EST=48;
Rubrica.prototype.CALLTYPE_CORR_NO_FILTRI=49;
Rubrica.prototype.CALLTYPE_RICERCA_CREATOR=50;
Rubrica.prototype.CALLTYPE_ESTERNI_AMM=51;
Rubrica.prototype.CALLTYPE_PROTO_OUT_ESTERNI=52;
Rubrica.prototype.CALLTYPE_PROTO_IN_ESTERNI=53;
Rubrica.prototype.CALLTYPE_RUOLO_RESP_REG = 54;
Rubrica.prototype.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO = 55;
Rubrica.prototype.CALLTYPE_MITT_MULTIPLI = 56;
Rubrica.prototype.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO = 57;
Rubrica.prototype.CALLTYPE_RICERCA_UO_RUOLI_SOTTOPOSTI = 58;
Rubrica.prototype.CALLTYPE_TUTTI_RUOLI = 59;
Rubrica.prototype.CALLTYPE_TUTTE_UO = 60;
Rubrica.prototype.CALLTYPE_CORR_INT_NO_UO = 61;
Rubrica.prototype.CALLTYPE_REPLACE_ROLE = 62;
Rubrica.prototype.CALLTYPE_DEST_FOR_SEARCH_MODELLI = 63;
Rubrica.prototype.CALLTYPE_FIND_ROLE = 64;
Rubrica.prototype.CALLTYPE_OWNER_AUTHOR = 65;
Rubrica.prototype.CALLTYPE_RUOLO_RESP_REPERTORI = 66;
Rubrica.prototype.CALLTYPE_RICERCA_CORRISPONDENTE = 67;
Rubrica.prototype.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO = 68;
Rubrica.prototype.Apri = Rubrica_Apri;
