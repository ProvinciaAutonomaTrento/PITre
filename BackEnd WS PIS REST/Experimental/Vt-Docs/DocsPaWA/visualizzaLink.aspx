<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="visualizzaLink.aspx.cs"
    Inherits="DocsPAWA.visualizzaLink" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DOCSPA > visualizzaLink</title>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <link href="/CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <link rel="shortcut icon" href="/images/favicon.ico" type="image/x-icon" />

    <script type="text/javascript">
    
        // Funzione per l'apertura del visualizzatore
        function openViewer(urlToOpen) {
            // Si apre la finestra
	        //var newWindow = window.open(urlToOpen, '', 'location=0;');
	        // Si effettua un ritorno alla pagina precedente
	        //history.go(-1);
	        
	        // Se l'oggetto history non contiene elementi...
	        if(history.length == 0)
	            openWindowsAndCloseThis(urlToOpen);
	        else
	        {
	        
	            // ...altrimenti si apre una nuova finestra...
	            
	            var newWindow = window.open(urlToOpen, '', "width=800px,height=600px,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no");
	            // ...si effettua un history back per tornare alla pagina precedente
                //history.back(); 
	            // ...e si dà il focus alla finestra aperta
	            newWindow.focus();
	        }
	        
	    }
	    
	    // Funzione che si occupa di determinare la versione di internet explorer
	    function getInternetExplorerVersion() {
            var rv = -1;
            if (navigator.appName.lastIndexOf('Explorer') != -1) {
                var ua = navigator.userAgent;
                var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
                if (re.exec(ua) != null)
                    rv = parseFloat(RegExp.$1);
            }
            return rv;
            
		}
		
		// Funzione che si occupa di redirezionare l'utente verso la pagina 
		// di login
		function redirectToLogin(url)
		{
	        var pageAddress = url;
	        var newUrl = pageAddress;
	        //var newWindow = window.showModelessDialog(url, '_blank', 'location=0');
		    
		    openWindowsAndCloseThis(newUrl);

		}
		
		
		// Funzione richiamata al load della pagina per impostarne le dimensioni e la
		// posizione
		function load()
        {
	        var maxWidth=800;
	        var maxHeight=700;

	        //window.resizeTo(maxWidth,maxHeight);
        	
	        //var newLeft=(screen.availWidth-maxWidth)/2;
	        //var newTop=(screen.availHeight-maxHeight)/2;
	        //window.moveTo(newLeft,newTop);
	
        }
        
        function openWindowsAndCloseThis(urlToOpen)
        {
            var version = getInternetExplorerVersion();
		    
            if (version > -1 && version < 7) {
                var newWindow = window.open(urlToOpen, '_blank', "width=800px,height=600px,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes");
                window.opener = null;
                window.close();
                return;
            }

            if (version > -1 && version >= 7) {
                window.open(null, '_self', null);
                window.close();
                window.open(urlToOpen, '_blank', "width=800px,height=600px,toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no, center=yes");
                return;
            }
        }

		
		
    </script>

</head>
<body onload="load()">
    <form id="frmVisualizzaLink" runat="server">
    <div style="z-index: 101; left: 71px; position: absolute; top: 111px" class="testo_red">
        <asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
