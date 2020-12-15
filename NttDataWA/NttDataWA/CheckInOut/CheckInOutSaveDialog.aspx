<%@ Page Language="c#" CodeBehind="CheckInOutSaveDialog.aspx.cs" AutoEventWireup="false"
    Inherits="NttDataWA.CheckInOut.CheckInOutSaveDialog" %>

<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc2" %>
<%@ Register Src="../FormatiDocumento/SupportedFileTypeController.ascx" TagName="SupportedFileTypeController"
    TagPrefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>
        <%=this.DialogTitle %></title>
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../Css/docspa_30.css" type="text/css" rel="stylesheet">
    <base target="_self">

    <script type="text/javascript">        
        // Questa funzione, richiamata al load del body, rende invisibili i
        // radio button se il parametro visOpt nella query string è pari a 0
        // e disabilita i controlli della pagina se il radio button copia 
        // link su clipboard o copia link su file system è abilitato
        function inizializza() {
            // Per default non devo visualizzare i tre radio button
            var visualizza = "0";

            // Recupero il parametro visOpt dalla query string
            var qst = window.location.search.substr(1);
            var dati = qst.split(/\&/);
            var valore = '';
            for (var i=0; i<dati.length; i++) {
                var tmp = dati[i].split(/\=/);
                if (tmp[0] == "visOpt") {
                    visualizza = tmp[1];
                }
            }
            
            // Se vis opt è pari a 0...
            if(visualizza == "0") {
                // ...nascondo il div opzioni...
                opzioni.style.display = "none";   
            }
            
            // Se il radio button copia link in clipboard è selezionato...
            if(frmSaveDialog.rbClipboard.checked)
                // ...richiama la funzione SelezionaTipoSalvataggio passando 
                // come parametro clipboard
                SelezionaTipoSalvataggio("clipboard");
                
            // Se il radio button copia link a scheda documento in clipboard è selezionato...
            if(frmSaveDialog.rbClipboardSD.checked)
                // ...richiama la funzione SelezionaTipoSalvataggio passando 
                // come parametro clipboard
                SelezionaTipoSalvataggio("clipboardSD");

            // Se il radio button salva link su filesystem è selezionato...
            if(frmSaveDialog.rbSalvaUrl.checked)
                // ...richiama la funzione SelezionaTipoSalvataggio passando 
                // come parametro creaFileUrl
                SelezionaTipoSalvataggio("creaFileUrl");
                
            // Se il radio button salva link a scheda su filesystem è selezionato...
            if(frmSaveDialog.rbSalvaUrlSD.checked)
                // ...viene richiamata la funzione SelezionaTipoSalvataggio passando 
                // come parametro creaFileUrlSD
                SelezionaTipoSalvataggio("creaFileUrlSD");    
                
             if (frmSaveDialog.rbFileZip.checked) {
                SelezionaTipoSalvataggio('zip');
            }   
        }
        
        // Questa funzione si occupa di abilitare/disabilitare i controlli in base
        // al tipo di salvataggio richiesto
        function SelezionaTipoSalvataggio(tipo) {
            switch(tipo)
            {
                case "clipboard":   // Copia del link a immagine del documento su clipboard
	                // Selezione del radio button di copia su clipboard e deselezionamento
	                // degli altri
		            frmSaveDialog.rbFileSystem.checked = false;
    	            frmSaveDialog.rbClipboard.checked = true;
    	            frmSaveDialog.rbSalvaUrl.checked = false;
    	            frmSaveDialog.rbClipboardSD.checked = false;
                    frmSaveDialog.rbSalvaUrlSD.checked = false;
                    frmSaveDialog.rbFileZip.checked = false;
    	            
    	            document.getElementById("txtFolderPath").disabled = true;
    	            document.getElementById("btnBrowseForFolder").disabled = true;
    	            document.getElementById("txtFileName").disabled = true;
    	            estensioneURL.style.display = "none";
                    estensione.style.display = "block";
                    estensioneZip.style.display = "none";

                    //setSessionValue("DownloadZipPackageDocument", "0");
    	            
    	            break;
    	            
    	        case "clipboardSD": // Copia del link a scheda documento su clipboard
    	            // Selezione del radio button di copia link a scheda su clipboard e 
    	            // deselezionamento degli altri
		            frmSaveDialog.rbFileSystem.checked = false;
    	            frmSaveDialog.rbClipboard.checked = false;
    	            frmSaveDialog.rbSalvaUrl.checked = false;
    	            frmSaveDialog.rbClipboardSD.checked = true;
                    frmSaveDialog.rbSalvaUrlSD.checked = false;
                    frmSaveDialog.rbFileZip.checked = false;
    	            
    	            document.getElementById("txtFolderPath").disabled = true;
    	            document.getElementById("btnBrowseForFolder").disabled = true;
    	            document.getElementById("txtFileName").disabled = true;
    	            estensioneURL.style.display = "none";
                    estensione.style.display = "block";
                    estensioneZip.style.display = "none";

                    //setSessionValue("DownloadZipPackageDocument", "0");
    	            
    	            break;
    	            
    	        case "creaFileUrl": // Salvataggio del link a immagine documento su filesystem
    	            // Selezione del radio button di creazione file url e deselezionamento
    	            // degli altri
    	            frmSaveDialog.rbFileSystem.checked = false;
	                frmSaveDialog.rbClipboard.checked = false;
	                frmSaveDialog.rbSalvaUrl.checked = true;
	                frmSaveDialog.rbClipboardSD.checked = false;
                    frmSaveDialog.rbSalvaUrlSD.checked = false;
                    frmSaveDialog.rbFileZip.checked = false;
	                
    	            document.getElementById("txtFolderPath").disabled = false;
    	            document.getElementById("btnBrowseForFolder").disabled = false;
    	            document.getElementById("txtFileName").disabled = false;
    	            estensione.style.display = "none";
                    estensioneURL.style.display = "block";
                    estensioneZip.style.display = "none";

                    //setSessionValue("DownloadZipPackageDocument", "0");
    	            break;
    	            
    	        case "creaFileUrlSD": // Salvataggio  del link alla scheda documento su filesystem
    	            // Selezione del radio button di creazione file url a scheda e deselezionamento
    	            // degli altri
    	            frmSaveDialog.rbFileSystem.checked = false;
	                frmSaveDialog.rbClipboard.checked = false;
	                frmSaveDialog.rbSalvaUrl.checked = false;
	                frmSaveDialog.rbClipboardSD.checked = false;
                    frmSaveDialog.rbSalvaUrlSD.checked = true;
                    frmSaveDialog.rbFileZip.checked = false;
	                
    	            document.getElementById("txtFolderPath").disabled = false;
    	            document.getElementById("btnBrowseForFolder").disabled = false;
    	            document.getElementById("txtFileName").disabled = false;
    	            estensione.style.display = "none";
                    estensioneURL.style.display = "block";
                    estensioneZip.style.display = "none";

                   //setSessionValue("DownloadZipPackageDocument", "0");

                    break;

                case "zip":
                    frmSaveDialog.rbFileSystem.checked = false;
	                frmSaveDialog.rbClipboard.checked = false;
	                frmSaveDialog.rbSalvaUrl.checked = false;
	                frmSaveDialog.rbClipboardSD.checked = false;
                    frmSaveDialog.rbSalvaUrlSD.checked = false;
                    frmSaveDialog.rbFileZip.checked = true;

                    document.getElementById("txtFolderPath").disabled = false;
    	            document.getElementById("btnBrowseForFolder").disabled = false;
    	            document.getElementById("txtFileName").disabled = false;
    	            estensioneURL.style.display = "none";
                    estensione.style.display = "none";
                    estensioneZip.style.display = "block";

                    //setSessionValue("DownloadZipPackageDocument", "1"); // imposta variabile di sessione per evitare porcate nel codice !
                    break;
    	            
    	        default:    // Salvataggio del documento su filesystem
    	            // Selezione del radio button salva su file system e deselezionamento
    	            // degli altri
	                frmSaveDialog.rbFileSystem.checked = true;
	                frmSaveDialog.rbClipboard.checked = false;
	                frmSaveDialog.rbSalvaUrl.checked = false;
	                frmSaveDialog.rbClipboardSD.checked = false;
                    frmSaveDialog.rbSalvaUrlSD.checked = false;
                    frmSaveDialog.rbFileZip.checked = false;
	                // Abilito la text box del path
    	            document.getElementById("txtFolderPath").disabled = false;
    	            // Abilito il bottone del browse for folder
    	            document.getElementById("btnBrowseForFolder").disabled = false;
    	            // Abilito la casella di testo per del filename
    	            document.getElementById("txtFileName").disabled = false;
    	            // Nascondo la div estensioneURL
    	            estensioneURL.style.display = "none";
    	            // Visualizzo la div estensione
                    estensione.style.display = "block";
                    
                    estensioneZip.style.display = "none";

                    //setSessionValue("DownloadZipPackageDocument", "0");
    	            
    	            break;

    	    }
	        
        }

        

        // ajax function that looks a bit like jQuery $.ajax
        // minimal code for what I need; not dummy proof, no error handling ...
        // feel free to extend this
        //var http_request = new XMLHttpRequest();
        //function ajax(options) {
        //  http_request.open(options.type || 'GET', options.url, true);
        //  http_request.send(options.data || null);
        //  http_request.onreadystatechange = function() {
        //    if (http_request.readyState == 4) {
        //      if (http_request.status == 200) {
        //        var type = options.dataType || '';
        //        switch (type.toLowerCase()) {
        //          default: 
        //            // options.success(http_request.responseText);
        //            break;
        //          case 'json': 
        //            // options.success(JSON.parse(http_request.responseText));
        //            break;
        //        }
        //      }
        //    }
        //  }
        //}

        //function setSessionValue(key, value) {
        //    console.log("set value in session from js");
        //    ajax({
        //        type: "POST",
        //        url: "../handler/SessionHandler.ashx",
        //        data: { sessionKey: key, sessionValue: value },
        //        // DO NOT SET CONTENT TYPE to json
        //        // contentType: "application/json; charset=utf-8", 
        //        // DataType needs to stay, otherwise the response object
        //        // will be treated as a single string
        //        dataType: "json"
        //    });
        //}
	    
	    // Funzione per il salvataggio su filesystem del link al documento
	    function CreateLinkFile() {
	        // Il path completo
	        var path = document.getElementById("txtFolderPath").value + "\\" + document.getElementById("txtFileName").value + ".URL";
	        
	        // Il path per l'immagine documento
	        var linkID = "<%= this.Link %>";
	        
	        // Il path per la scheda del documento
	        var linkSD ="<%= this.LinkSD %>";
	        
	        // L'oggetto per accedere al filesystem client
            var fso = FsoWrapper_CreateFsoObject();
            //alert(linkSD);
	        // Creo un file in path
	        var stream=fso.OpenTextFile(path,2,true);
	        stream.WriteLine("[InternetShortcut]");
	        if(frmSaveDialog.rbSalvaUrlSD.checked)
	            stream.WriteLine("URL=" + linkSD);
	        else
	            stream.WriteLine("URL=" + linkID);
	            
	        stream.WriteLine("IconFile=" + "<%= this.FileIcona %>");
	        stream.WriteLine("IconIndex=1");
	        stream.WriteLine("[{000214A0-0000-0000-C000-000000000046}]");
	        stream.WriteLine("HotKey=0");
	        stream.WriteLine("Prop3=19,2");
	        // Chiudo lo stream
	        FsoWrapper_CloseFsoStreamObject(stream);
	    }
	    
		
		// Impostazione del focus su un controllo
		function SetControlFocus(controlID)
		{	
			try
			{
				var control=document.getElementById(controlID);
				
				if (control!=null)
				{
					control.focus();
				}
			}
			catch (e)
			{}
		}
		
		// Ripristino dei dati persistiti sul client
		function RestoreClientData()
		{	
			try
			{
				var fso=FsoWrapper_CreateFsoObject();
				
				var path=fso.GetSpecialFolder(2).Path + "\\CheckInOutSaveDialog.txt";
				
				if (fso.FileExists(path))
				{
					var stream=fso.OpenTextFile(path,1,false)

					if (!stream.AtEndOfStream)
						frmSaveDialog.txtFolderPath.value=stream.ReadLine();
					
					FsoWrapper_CloseFsoStreamObject(stream);
				}
			}
			catch (ex)
			{
			    alert(ex.message.toString());
			}
			
			if (frmSaveDialog.txtFolderPath.value!="")
			{	
				frmSaveDialog.txtFirstInvalidControlID.value="txtFileName";
				frmSaveDialog.submit();
			}
		}
		
		// Momorizzazione dei dati immessi sul client
		function PersistClientData()
		{
			try
			{
				var fso=FsoWrapper_CreateFsoObject();

				var path=fso.GetSpecialFolder(2).Path + "\\CheckInOutSaveDialog.txt";

				var stream=fso.OpenTextFile(path,2,true)
				stream.WriteLine(frmSaveDialog.txtFolderPath.value);
				FsoWrapper_CloseFsoStreamObject(stream);
			}
			catch (ex)
			{
			    alert(ex.message.toString());
			}
        }

        function CreateZipFile() {
            // Creazione FIle zip
            // Alessandro Aiello 03/05/2019
        }
		
		// Creazione oggetto activex con gestione errore
		function CreateObject(objectType)
		{
			try
			{
				return new ActiveXObject(objectType);
			}
			catch (ex)
			{
				alert("Oggetto '" + objectType + "' non istanziato");
			}	
		}	
		
		// Validazione dati immessi
		function Validate()
		{
			var retValue=true;
			var fso=FsoWrapper_CreateFsoObject();	
			var validationMessage="";
			
			var selectedFolder=frmSaveDialog.txtFolderPath.value;
			if (selectedFolder=="")
			{
				validationMessage="Cartella non immessa";
				frmSaveDialog.txtFirstInvalidControlID.value="txtFolderPath";
				retValue=false;
			}
			else
			{		
			    if (!fso.FolderExists(selectedFolder))
			    {
				    // Cartella non esistente, viene richiesto all'utente se crearla o meno
				    if (confirm("Cartella non presente.\nSi desidera crearla?"))
					    fso.CreateFolder(selectedFolder);
				    else
				    {
				        validationMessage="Cartella non valida";
				        frmSaveDialog.txtFirstInvalidControlID.value="txtFolderPath";
					    retValue=false;
                    }
			    }
			}

			var fileName=frmSaveDialog.txtFileName.value;
			if (fileName=="")
			{
				if (validationMessage!="")
					validationMessage+="\n";
				validationMessage="Nome file non immesso";
				
				frmSaveDialog.txtFirstInvalidControlID.value="txtFileName";
				retValue=false;
			}
			else
			{
                var re = new RegExp("[\\\/\:\*\?\"\<\>]+");
				if (fileName.match(re))
				{
			        validationMessage = "Il nome del file contiene dei caratteri non ammessi";    				
				    frmSaveDialog.txtFirstInvalidControlID.value="txtFileName";
				    retValue = false;
                }
			}
			
			var fileExtension = GetFileExtension();
			if (fileExtension=="")
			{
			    if (validationMessage!="")
					validationMessage+="\n";
				validationMessage="Formato file non immesso";
				
				if (document.getElementById("cboFileTypes")!=null)
				{
				    var cboFileTypes=document.getElementById("cboFileTypes").value;
				    frmSaveDialog.txtFirstInvalidControlID.value="cboFileTypes";
				    retValue=false;
				}
			}

            
			if (!retValue)
			{
				// Visualizzazione messaggio di errore
				alert(validationMessage);
			}
			else
			{
			    var fullName=GetSelectedFilePath();
			    
			    // Validazione formato file immesso
			    if (SF_ValidateFileFormat(fullName))
			    {
				    if (fso.FileExists(fullName))
					    retValue=confirm("Il file immesso risulta già presente.\nSi desidera sovrascriverlo?");
				}
				else
				{
					retValue = false;
		    	}
            }
			return retValue;
		}

		// Chiusura pagina 						
		function ClosePage(retValue)
        {
			if (retValue)
            {

			    // Se è stato richiesto il salvataggio su clipboard...
                if (frmSaveDialog.rbClipboard.checked || frmSaveDialog.rbClipboardSD.checked) {
                    var link = "<%= this.Link %>";
                    var linkSD = "<%= this.LinkSD %>";

                    // ...scrivo nel campo nascosto txtLink, il link 
                    // da copiare negli appunti...
                    frmSaveDialog.txtLink.value = frmSaveDialog.rbClipboard.checked ? link : linkSD;
                    // ...copio il testo negli appunti
                    
                   
                    

                    //* ALESSANDRO AIELLO 10-06-2019
                    //* INIZIO
                    var ua = window.navigator.userAgent;
                    var msie = ua.indexOf("MSIE ");
                    if (msie > 0 && msie < 9) // If Internet Explorer, return version number
                    {
                        Copied = frmSaveDialog.txtLink.createTextRange();
                        Copied.execCommand("Copy");
                    }
                    else  // If another browser, return 0
                    {
                        var aux = document.createElement("div");
                        aux.setAttribute("contentEditable", true);
                        aux.innerHTML = frmSaveDialog.txtLink.value;
                        document.body.appendChild(aux);
                        window.getSelection().selectAllChildren(aux);
                        document.execCommand("copy");
                        document.body.removeChild(aux);
                    }

                    //* FINE


                    alert("Testo copiato negli appunti");
                    window.close();

                } else if (frmSaveDialog.rbFileZip.checked) {
                    //console.log("Creo ZIP");
                    if (Validate()) {
                       //CreateZipFile();
                        PersistClientData();
            				
			            // Validazione dati effettuata
			            window.returnValue=GetSelectedFilePath();
                            
                        window.close();
                    }
                } else   {
                    // Se è stato richiesto il salvataggio di un file con il link...
                    if(frmSaveDialog.rbSalvaUrl.checked || frmSaveDialog.rbSalvaUrlSD.checked) {
                        // ...richiedo validazione dati e salvataggio file
                        if(Validate()) {
                            CreateLinkFile();
                            
                            alert("Link su file creato");
                            
                            window.close();
                        }
                    } else {
		                if (Validate())
		                {	
			                PersistClientData();
            				
			                // Validazione dati effettuata
			                window.returnValue=GetSelectedFilePath();
            			
			                window.close();
		                }
		            }
				}
			}
			else 
			{
				window.returnValue="";
				
				window.close();
			}
		}
		
		// Reperimento percorso correntemente selezionato
		function GetSelectedFilePath()
		{
			var selectedPath=frmSaveDialog.txtFolderPath.value;
			
			if (selectedPath!="")
			{	
				var fso=FsoWrapper_CreateFsoObject();
			
				var folder=fso.GetFolder(selectedPath);
				
				if (folder!=null)
				{
					selectedPath=folder.Path;
					
					if (selectedPath.charAt(selectedPath.length - 1)!="\\")
						selectedPath=selectedPath + "\\";

		            selectedPath = selectedPath + GetFileName();
				}
			}

			return selectedPath;

        }

        function GetFileName() {
            var fileName = "";

            var ext = GetFileExtension();

            if (ext.toUpperCase() == "P7M")
                fileName = frmSaveDialog.txtFileName.value + "<%=this.GetP7mFileExtensions()%>";
            else
                fileName = frmSaveDialog.txtFileName.value + "." + GetFileExtension();            
            //console.log("File Name: " + fileName);
            return fileName;
        }

		// Reperimento dell'estensione del file
		function GetFileExtension()
		{
			var fileType="";
            if (frmSaveDialog.rbFileZip.checked === true) {
                fileType = "zip";
            } else if (document.getElementById("cboFileTypes") != null)
				fileType=document.getElementById("cboFileTypes").value;
			else if (document.getElementById("lblFileType")!=null)
                fileType = document.getElementById("lblFileType").innerHTML;
            //console.log("File Extension: " + fileType);
			return fileType;
		}			
					
		
		function PerformSelectFolder()
		{
			var folder=ShellWrappers_BrowseForFolder("Salva documento");
			
			if (folder!="")
			{
				var fso=FsoWrapper_CreateFsoObject();
				if (fso.FolderExists(folder))
				{
					frmSaveDialog.txtFolderPath.value=folder;
					
					frmSaveDialog.txtFirstInvalidControlID.value="txtFileName";
				}
			}
		}
	
    </script>

</head>
<body bottommargin="1" leftmargin="1" topmargin="2" rightmargin="1" ms_positioning="GridLayout"
    onload="javascript:inizializza();">
    <form id="frmSaveDialog" method="post" runat="server">
    <input type="hidden" id="txtLink" runat="server" />
    <input type="hidden" id="txtFirstInvalidControlID" runat="server" />
    <uc1:SupportedFileTypeController ID="supportedFileTypeController" runat="server" />
    <uc2:ShellWrapper ID="shellWrapper" runat="server" />
    <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
    <table class="info" id="tblContainer" cellspacing="0" cellpadding="2" width="370"
        align="center" border="0" runat="server">
        <tr>
            <td align="center" colspan="2">
                <asp:Label ID="lblTitle" runat="server" CssClass="titolo_scheda"><%=this.DialogTitle %> </asp:Label>
            </td>
        </tr>
        <tr id="opzioni">
            <td colspan="2" class="testo_grigio">
                <div>
                    <div onclick="javascript:SelezionaTipoSalvataggio('filesystem')">
                        <input id="rbFileSystem" type="radio" runat="server" checked="true" />Salva documento su file system
                    </div>
                    <br />
                    <div onclick="javascript:SelezionaTipoSalvataggio('zip')">
                        <input id="rbFileZip" type="radio" runat="server" />Salva documento e allegati (.zip) su file system
                    </div>
                    <br />
                    <div onclick="javascript:SelezionaTipoSalvataggio('clipboard')">
                        <input id="rbClipboard" type="radio" runat="server" />Copia link a immagine documento su clipboard</div>
                    <br />
                    <div onclick="javascript:SelezionaTipoSalvataggio('clipboardSD')">
                        <input id="rbClipboardSD" type="radio" runat="server" />Copia link a scheda documento su clipboard</div>
                    <br />
                    <div onclick="javascript:SelezionaTipoSalvataggio('creaFileUrl')">
                        <input id="rbSalvaUrl" type="radio" runat="server" />Salva link a immagine documento su filesystem
                    </div>
                    <br />
                    <div onclick="javascript:SelezionaTipoSalvataggio('creaFileUrlSD')">
                        <input id="rbSalvaUrlSD" type="radio" runat="server" />Salva link a scheda documento su filesystem</div>
                    <br />
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;<asp:Label ID="lblFolderPath" runat="server" CssClass="titolo_scheda">Cartella di destinazione: *</asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;<asp:TextBox ID="txtFolderPath" runat="server" CssClass="testo_grigio" Width="322px"></asp:TextBox>&nbsp;
                <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." CssClass="PULSANTE">
                </asp:Button>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;<asp:Label ID="lblFileName" runat="server" CssClass="titolo_scheda">Nome file: *</asp:Label>
            </td>
        </tr>
        <tr>
            <td width="240">
                &nbsp;<asp:TextBox ID="txtFileName" runat="server" CssClass="testo_grigio" Width="98%"></asp:TextBox>
            </td>
            <td width="130">
                <div id="estensioneZip" style="display: none; width: 50%;">
                        <p class="titolo_scheda">.zip</p>
                    </div>
                <div id="estensione">
                    .<asp:Label ID="lblFileType" runat="server" CssClass="titolo_scheda" Width="50%"></asp:Label>
                    
                    <asp:DropDownList ID="cboFileTypes" runat="server" CssClass="testo_grigio" Width="50%">
                    </asp:DropDownList>
                </div>
                <div id="estensioneURL" style="display:none;">
                    .<asp:Label ID="lblUrl" runat="server" CssClass="titolo_scheda" Width="50%">URL</asp:Label>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" colspan="2">
                <asp:Button ID="btnOk" runat="server" Text="     OK     " CssClass="PULSANTE"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Annulla" CssClass="PULSANTE"></asp:Button>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
