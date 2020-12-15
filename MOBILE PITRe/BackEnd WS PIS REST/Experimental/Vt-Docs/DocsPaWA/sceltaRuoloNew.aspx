<%@ Page Language="c#" CodeBehind="sceltaRuoloNew.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.sceltaRuoloNew" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="UserControls/NotificationCenterItemList.ascx" tagname="NotificationCenterItemList" tagprefix="uc3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>DOCSPA > Homepage_DPA</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">
    <link href="CSS/NotificationCenter.css" type="text/css" rel="Stylesheet" />
    <script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>

    <script>
		function Body_OnMouseOver() {
			try
			{
				if(top.frames[0].document!=null)
					if(top.frames[0].document.Script!=null)
					if(top.frames[0].document.Script["closeIt"]!=null)
						top.frames[0].document.Script.closeIt();
			}
			catch(exc)
			{;}	
		}		
    </script>

    <script language="javascript" id="chklst_ruoli_Change" event="onchange()" for="chklst_ruoli">
			if (Home_DPA.hd_AutoToDoListValue.value=="true")
			{
				window.document.body.style.cursor='wait';
				WndWaitTrasm();			
			}
			Home_DPA.submit();
    </script>

    <script language="javascript" id="btnCercaTrasmissioni_Click" event="onclick()" for="btnCercaTrasmissioni">
			window.document.body.style.cursor='wait';		
			WndWaitTrasm();
    </script>

    <script language="javascript" type="text/javascript">
		    
		    // Visualizzazione maschera di filtro su trasmissioni in todolist
		    function ShowFiltersDialog()
		    {
		        // Tipologia di oggetto selezionato
		        //var tipoOggetto = document.getElementById('DDLOggettoTab1').value;
		        
//		        var retValue = window.showModalDialog('RicercaTrasm/DialogFiltriRicercaTrasmissioni.aspx?tipoOggetto=' + tipoOggetto,
//		                window.self,
//		                'dialogWidth:700px;dialogHeight:300px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no');
                
                var retValue = null;
                if (navigator.appVersion.indexOf("MSIE 7.")== -1)
                {
                    retValue = window.showModalDialog('RicercaTrasm/DialogFiltriRicercaTrasmissioni.aspx','','dialogWidth:700px;dialogHeight:480px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');
                }
                else
                {
                    retValue = window.showModalDialog('RicercaTrasm/DialogFiltriRicercaTrasmissioni.aspx','','dialogWidth:700px;dialogHeight:480px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');
                }
				
                
                if (retValue == null)
                    retValue = false;
                    
                // Reperimento valore di ritorno del filtro
                document.Home_DPA.hd_performSearchToDoList.value=retValue;

                return retValue;
            }
            
            function ApriListaToDoList()
			{				
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog("popup/listaToDoList.aspx",
										args,
										"dialogWidth:860px;dialogHeight:510px;status:no;resizable:yes;scroll:yes;center:yes;help:no;");						
			    if (retValue)
			    {
			        window.document.Home_DPA.submit();
			    }

			}

            /*
             * Funzione per il ridimensionamento dell'iFrame con le icone del centro notifiche.
             */
			function iFrameResize(height, visibility) { 
                try {
                    document.getElementById('divNotificationCenter').style.visibility = visibility;
                    top.principale.iFrame_sx.frameElement.height = height;
                } 
                catch(ex) {}
            }


			
</script>

</head>
<body bottommargin="0" leftmargin="0" topmargin="5" rightmargin="0" onmouseover="Body_OnMouseOver()">
    <form id="Home_DPA" method="post" runat="server">
    <input id="hd_performSearchToDoList" type="hidden" runat="server" />
    <input type="hidden" name="hd_AutoToDoListValue" id="hd_AutoToDoListValue" runat="server" />
    <table class="info" height="55" width="100%" align="center">
        <tr valign="middle">
            <td width="11%" class="titolo_rosso">
                &nbsp;Scelta ruolo:
            </td>
            <td width="87%" align="left">
                <asp:DropDownList ID="chklst_ruoli" runat="server" Width="455px" CssClass="testo_grigio"
                    AutoPostBack="True">
                </asp:DropDownList>
                &nbsp;
                <cf1:ImageButton ImageAlign="Middle" ID="btn_all_todolist" Height="16" ImageUrl="images/btn_interrogazione.jpg"
                    runat="server" AlternateText="Cerca in tutte le todolist di tutti i ruoli" Visible="false">
                </cf1:ImageButton>
            </td>
            <td width="2%"><asp:ImageButton ID="btn_pred_todolist" Height="16" ImageUrl="images/puntoesclamativo.jpg"
                    runat="server" AlternateText="Esistono documenti predisposti da accettare o visualizzare nella to do list" Visible="false">
                    </asp:ImageButton></td>
        </tr>
        <tr valign="middle">
            <td class="titolo_rosso" width="11%">
                &nbsp;Struttura:
            </td>
            <td width="89%" colspan="2">
                <asp:Label ID="lbl_listaUO" runat="server" CssClass="testo_grigio"></asp:Label>
            </td>
        </tr>
    </table>
    <div id="divNotificationCenter" style="width: 100%; margin-bottom: 5px; margin-top: 5px;">
        <uc3:NotificationCenterItemList ID="ncItems" runat="server" />
    </div>
    <table class="info" height="20" width="100%" align="center">
        <tr>
            <td valign="middle">
                <table border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="titolo_rosso" width="12%">
                            &nbsp;Cose da fare&nbsp;&nbsp;
                        </td>
                        <td valign="middle">
                            <asp:Button ID="btnCercaTrasmissioni" Text="Aggiorna" runat="server" CssClass="pulsante79" ToolTip="Cerca" />
                            <asp:Button ID="btnShowFilters" Text="Filtro" CssClass="pulsante79" runat="server" ToolTip="Filtra trasmissioni" OnClick="btnShowFilters_Click" />
                            <asp:Button ID="btnRemoveFilters" runat="server" Text="Rimuovi Filtro" CssClass="pulsante127" ToolTip="Rimuovi filtro trasmissioni" OnClick="btnRemoveFilters_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td width="0" height="0">
                <cc2:MessageBox Width="0" Height="0" ID="msg_delega" runat="server"></cc2:MessageBox>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
