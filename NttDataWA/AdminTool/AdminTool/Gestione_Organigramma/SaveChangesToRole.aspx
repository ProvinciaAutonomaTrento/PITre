<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveChangesToRole.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.SaveChangesToRole" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Modifica ruolo</title>
    <style type="text/css">
        /* Stile della bottoniera */
        .buttons-style
        {
            text-align: right;
            margin: 5px;
        }
        
        .datagrid
        {
            background-color: White;
            border-color: #DEDFDE;
            border-style: none;
            border-width: 1px;
            padding: 4;
            color: Black;
            width: 96%;
        }
        
        .datagrid_alternating
        {
            background-color: White;
            font-size: smaller;
        }
        
        .datagrid_footer
        {
            background-color: #CCCC99;
        }
        
        .datagrid_header
        {
            background-color: #6B696B;
            font-weight: bold;
            color: White;
        }
        
        .datagrid_item
        {
            background-color: #F7F7DE;
            font-size: smaller;
        }
        
        .operationReport 
        {
            overflow:scroll;
            height: 200px;
            
        }
        
        .header-style
        {
		}
    .title-style {
		text-align: center;
		font-family: Verdana, Geneva, Tahoma, sans-serif;
		font-size: x-small;
		color: #000000;
		font-weight: bold;
		text-transform: uppercase;
		background: #C0C0C0;
	}
	
	.external-style
	{
		padding: 3px;
		border: 1px #000000 solid;
		color: #000000;
		width: 95%;
		margin: 5px;
	}
	
	.button-style
	{
		font-family: Arial, Helvetica, sans-serif;
		font-size: x-small;
		font-weight: bold;
		text-transform: uppercase;
	}

    .report-style {
		overflow: scroll;
		height: 200px;
		text-align: left;
	}

    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
    <script type="text/javascript" language="javascript">
        // Funzione che verrà chiamata dal server quando sarà pronta la risposta
        function analyzeResult(retVal) {

            // Se retVal è definita e lunga 0, è la fase di inizializzazione,
            // quindi viene chiamato direttamente il server e viene disabilitato il pulsante
            // avvia
            if (retVal != undefined && retVal === '') {
                callServer();
                document.getElementById('btnMove').disabled = 'disabled';
            }

            // Se la retVal è valorizzato, il primo carattere è un codice identificativo
            // che può assumere i seguenti valori:
            // 0 - L'elaborazione è terminata -> Viene abilitato il pulsante per l'esportazione della reportica e viene visualizzato
            //                                   il report finale
            // 1 - L'elaborazione deve continuare -> Viene visualizzato il report parziale e viene richiamata la procedura server
            // 2 - Bisogna mostrare una richiesta all'amministratore -> Viene visualizzata la richiesta e, se l'amministratore
            //                                                          clicca su ok, si continua, altrimenti si interrompe 
            //                                                          il processo
            if (retVal != undefined && retVal.length > 0) {

                var opCode = retVal.substring(0, 1);
                var report = retVal.substring(1);
                var message = undefined;

                // Se il report contiene || significa che bisogna visualizzare un messaggio
                if (report.indexOf('||') > -1) {
                    message = report.substring(0, report.indexOf('||'));
                    report = report.substring(report.indexOf('||') + 2);
                }

                switch (opCode) {
                    case "0":
                        operationReport.innerHTML = report;
                        document.getElementById('btnExportReport').disabled = '';
                        break;
                    case "1":
                        operationReport.innerHTML = report;
                        callServer();
                        break;
                    case "2":
                        var adminResp = window.confirm(message);

                        // Se la risposta è positiva, si continua con l'azione
                        // altrimenti viene interrotta l'azione e si chiude la finestra
                        if (adminResp) {
                            operationReport.innerHTML = report;
                            callServer();
                        }
                        else
                            self.close();


                        break;

                }

                // Scorrimento fino al fondo del div
                try {
                    var objDiv = operationReport;
                    objDiv.scrollIntoView
                    objDiv.scrollTop = objDiv.scrollHeight;

                } catch (e) {

                }

            }

        }

    </script>
    <script type="text/javascript">
        function WebForm_CallbackComplete_SyncFixed() {
            // SyncFix: the original version uses "i" as global thereby resulting in javascript errors when "i" is used elsewhere in consuming pages
            for (var i = 0; i < __pendingCallbacks.length; i++) {
                callbackObject = __pendingCallbacks[i];
                if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
                    // the callback should be executed after releasing all resources
                    // associated with this request.
                    // Originally if the callback gets executed here and the callback
                    // routine makes another ASP.NET ajax request then the pending slots and
                    // pending callbacks array gets messed up since the slot is not released
                    // before the next ASP.NET request comes.
                    // FIX: This statement has been moved below
                    // WebForm_ExecuteCallback(callbackObject);
                    if (!__pendingCallbacks[i].async) {
                        __synchronousCallBackIndex = -1;
                    }
                    __pendingCallbacks[i] = null;

                    var callbackFrameID = "__CALLBACKFRAME" + i;
                    var xmlRequestFrame = document.getElementById(callbackFrameID);
                    if (xmlRequestFrame) {
                        xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
                    }

                    // SyncFix: the following statement has been moved down from above;
                    WebForm_ExecuteCallback(callbackObject);
                }
            }
        }

        window.onload = function Onload() {
            if (typeof (WebForm_CallbackComplete) == "function") {
                // set the original version with fixed version
                WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed;
            }
        }
    </script>
    <div class="external-style">
	    <div class="title-style">
	    	Modifica ruolo     
	    </div>
        <asp:UpdatePanel ID="upButtons" runat="server">
            <ContentTemplate>
	            <div class="buttons-style">
	                <button id="btnMove" name="btnMove" type="button" value="Avvia" class="button-style" onclick="analyzeResult('');" >
	                    Avvia</button>
	                &nbsp;
                    <button type="button" disabled="disabled" id="btnExportReport" name="btnExportReport" class="button-style" onclick="<%= ReportScript %>">Esporta report</button>
	                &nbsp;
                    <asp:Button runat="server" Text="Chiudi" CssClass="button-style" 
                        ID="btnClose" onclick="btnClose_Click" />
	            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
	    <div id="operationReport" class="report-style">
	        <asp:DataGrid ID="dgReport" runat="server" AutoGenerateColumns="False"
	            CssClass="datagrid" AlternatingItemStyle-CssClass="datagrid_alternating" FooterStyle-CssClass="datagrid_footer"
	            HeaderStyle-CssClass="datagrid_header" ItemStyle-CssClass="datagrid_item">
	            <Columns>
	                <asp:BoundColumn HeaderText="Operazione" DataField="Description" />
	                <asp:TemplateColumn ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Center">
	                    <ItemTemplate>
	                        <asp:Image runat="server" ID="imgIcon" ImageUrl="<%# ((SAAdminTool.DocsPaWR.SaveChangesToRoleReport)Container.DataItem).ImageUrl %>" />
	                    </ItemTemplate>
	                </asp:TemplateColumn>
	            </Columns>
	        </asp:DataGrid>
	    </div>
	</div>
    </form>
</body>
</html>
