<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page language="c#" Codebehind="SmistaDoc_Detail.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.smistaDoc.SmistaDoc_Detail"%>
<%@ Register TagPrefix="cc1" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD id="HEAD1" runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<SCRIPT language="javascript">

		    function ShowWaitingPage() {
		        if (document.getElementById('chk_showDoc').checked)
		            top.left.location = 'SmistaDoc_Waiting.htm';
		    }

		    // Visualizzazione popup dettagli firma
		    function ShowMaskDettagliFirma() {
		        var height = screen.availHeight;
		        var width = screen.availWidth;

		        height = (height * 90) / 100;
		        width = (width * 90) / 100;

		        window.showModalDialog('../popup/dettagliFirmaDoc.aspx?SIGNED_DOCUMENT_ON_SESSION=true',
					'', 'dialogWidth:' + width + 'px;dialogHeight:' + height + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
		    }

		    function btn_rifiuta_onclick(trasmissioneAccettata) {


		        var msg = prompt('Per poter rifiutare questa trasmissione, inserire un messaggio di rifiuto', '');

		        if (msg == null) {

		        }

		        if (msg == '') {
		            alert("Attenzione, per poter rifiutare un documento bisogna inserire alcune note di rifiuto");
		        }

		        if ((msg != '' && msg != null && msg != 'null' && msg != 'undefined')) {
		            document.Form1.hd_msg_rifiuto.value = msg;
		        }
		    }

		    function btn_rifiuto_onClick() {

		    }

		    function GetCommentoRifiuto() {
		        var msg = prompt('Per poter rifiutare questa trasmissione, inserire un messaggio di rifiuto', '');

		        if (msg == null) {

		        }

		        if (msg == '') {
		            alert("Attenzione, per poter rifiutare un documento bisogna inserire alcune note di rifiuto");
		        }

		        if ((msg != '' && msg != null && msg != 'null' && msg != 'undefined')) {
		            document.Form1.hd_msg_rifiuto.value = msg;
		        }
		    }

		    function CheckAllDataGridRadio(aspRadioID) {
		        re = new RegExp(':' + aspRadioID + '$')
		        for (i = 0; i < document.forms[0].elements.length; i++) {
		            elm = document.forms[0].elements[i]

		            if (elm.type == 'radio') {
		                if (re.test(elm.name)) {
		                    elm.value = 'optNull'
		                }
		            }
		        }
		    }

		    function ApriFinestraNoteSmistamento(indice, tipo, id, nomeTabella) 
		    {
		        var args = new Object;
		        args.window = window;

		        var viewScadenza = false;
		        
	            if (ControllaAbilitazioneDataScadenza(indice, nomeTabella) == true) {
	                viewScadenza = true;
	            }

	            window.showModalDialog(
	                        "NoteSmistamento.aspx?indice=" + indice + "&tipo=" + tipo + "&id=" + id + "&enableDtaScadenza=" + viewScadenza,
							args,
							"dialogWidth:420px;dialogHeight:210px;status:no;resizable:no;scroll:no;center:yes;help:no;");
		    }

		    function ControllaAbilitazioneDataScadenza(indice, nomeTabella) {
		        var lista_td; //lista delle celle della riga corrente
		        var radios;
		        var radioComp, radioCon, hdCompetenza, hdConoscenza;

		        esito = false;

		        //prendo le righe della tabella passata un input alla function
		        trs = document.getElementById(nomeTabella).tBodies[0].rows;

		        //indice di riga corrente relativo all'immagine selozionata
		        var indNew = parseInt(indice) + 1;

		        //prendo le celle relative alla riga relativa a quella corrente selezionata
		        lista_td = trs[indNew].cells;

		        //prendo i radio button contenuti in tale cella
		        var hdIsEnabledNavigaUO = document.getElementById("hdIsEnabledNavigaUO");

		        if (hdIsEnabledNavigaUO != null && hdIsEnabledNavigaUO != "undefined" && hdIsEnabledNavigaUO.value == "1") //verifica se esiste la colonna con le frecce per la navigazione delle UO
		        {
		            radios = lista_td[2].getElementsByTagName("input");
		            radioComp = document.getElementById(radios[0].id);

		            radios = lista_td[3].getElementsByTagName("input");
		            radioCon = document.getElementById(radios[0].id);
		        }
		        else 
		        {
		            radios = lista_td[1].getElementsByTagName("input");
		            radioComp = document.getElementById(radios[0].id);

		            radios = lista_td[2].getElementsByTagName("input");
		            radioCon = document.getElementById(radios[0].id);		            
		        }

		        hdCompetenza = document.getElementById("hdCheckCompetenza");
		        hdConoscenza = document.getElementById("hdCheckConoscenza");

		        if (radioComp.checked && (hdCompetenza.value == "W"))// verifico se è selezionata competenza
		        {
		            esito = true;
		        }
		        else 
		        {
		            if (radioCon.checked && hdConoscenza.value == "W") 
		            {
		                esito = true;
		            }
		        }
		        return esito;
		    }

		    function ApriRicercaFascicoli(codiceClassifica) {
		        var newUrl;
		        newUrl = "../popup/ricercaFascicoli.aspx?codClassifica=" + codiceClassifica + "&caller=smistamento";
		        var newLeft = (screen.availWidth - 615);
		        var newTop = (screen.availHeight - 710);
		        rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:615px;dialogHeight:700px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
		        if (rtnValue == "Y") {
		            window.document.Form1.submit();
		        }
		    }

		    function ApriTitolario2(qrstr, isFasc) {
		        var newUrl;

		        newUrl = "../popup/titolario.aspx?" + qrstr + "&isFasc=" + isFasc;

		        var newLeft = (screen.availWidth - 650);
		        var newTop = (screen.availHeight - 740);

		        // apertura della ModalDialog
		        rtnValue = window.showModalDialog(newUrl, "", "dialogWidth:650px;dialogHeight:720px;status:no;resizable:no;scroll:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";center:no;help:no;");
		        if (rtnValue != "N") {
		            window.document.Form1.submit();
		        }
		    }

		    function ApriSelezioneSmistamento() {
		        var args = new Object;
		        args.window = window;

		        var retValue = window.showModalDialog("selezioneSmistamento.aspx",
				                     args,
				                      "dialogWidth:600px;dialogHeight:600px;status:no;resizable:no;scroll:no;dialogLeft:0;dialogTop:0;center:yes;help:no;");

		        Form1.hdSelSmista.value = retValue;
		        Form1.submit();
		    }

		    function zoomDocument(link) {
		        var height = screen.availHeight;
		        var width = screen.availWidth;
		        alert(link);
		        window.showModalDialog(link,
					'', 'dialogWidth:' + width + 'px;dialogHeight:' + height + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
		    }

		    function ApriFinestraZoom(pageUrl, targetName) {
		        var maxWidth = window.screen.width;
		        var maxHeight = window.screen.height;

		        // script += "'fullscreen=no,toolbar=no,directories=no,statusbar=no,menubar=no,resizable=yes, scrollbars=auto');";
		        //ie7:
		        //script += "'location=0,resizable=yes');";
		        window.showModalDialog(pageUrl, '', 'dialogWidth:' + maxWidth + 'px;dialogHeight:' + maxHeight + 'px;status:no;resizable:yes;scroll:yes;center:yes;help:no;close:no');
		        //win=window.open(pageUrl,targetName,"fullscreen=no,toolbar=no,"+dimensionWindow+",directories=no,menubar=yes,resizable=yes, scrollbars=yes");
		        win = true;
		        /*add massimo digregorio  
		        * inserito try/catch x evitare errore javascript quando clicco su zoom documento due volte consecutive	
		        */
		    }
		
		</SCRIPT>

		<script language="javascript" id="btn_clearFlag_click" event="onclick()" for="btn_clearFlag">
		    //window.document.body.style.cursor='wait';
			//ShowWaitingPage();
		</script>
		<script language="javascript" id="btn_smista_click" event="onclick()" for="btn_smista">
		    document.getElementById('btn_smista').style.display='none';
            document.getElementById('btn_scarta').disabled=true;
            document.getElementById('btn_AdL').disabled=true;
             document.getElementById('btn_rifiuta').disabled=true;
		    //checkUtenteRuoloSelezionato('grdUOApp');
		    window.document.body.style.cursor='wait';
			ShowWaitingPage()
		</script>
		<script language="javascript" id="btn_scarta_click" event="onclick()" for="btn_scarta">
		    document.getElementById('btn_scarta').style.display='none';
            document.getElementById('btn_smista').disabled=true;
            document.getElementById('btn_AdL').disabled=true;
            document.getElementById('btn_rifiuta').disabled=true;
			window.document.body.style.cursor='wait';
			ShowWaitingPage()
		</script>
		<script language="javascript" id="bnt_AdL_click" event="onclick()" for="btn_AdL">
		    document.getElementById('btn_AdL').style.display='none';
            document.getElementById('btn_smista').disabled=true;
            document.getElementById('btn_scarta').disabled=true;
            document.getElementById('btn_rifiuta').disabled=true;
			window.document.body.style.cursor='wait';
		</script>
		<script language="javascript" id="btn_first_click" event="onclick()" for="btn_first">
			window.document.body.style.cursor='wait';
			var chkShowDoc = window.document.getElementById("chk_showDoc");
			if (chkShowDoc!=null && chkShowDoc.checked)
			{
			    ShowWaitingPage();
			}
		</script>
		<script language="javascript" id="btn_previous_click" event="onclick()" for="btn_previous">
			window.document.body.style.cursor='wait';
			var chkShowDoc = window.document.getElementById("chk_showDoc");
			if (chkShowDoc!=null && chkShowDoc.checked)
			{
			    ShowWaitingPage();
			}
		</script>
		<script language="javascript" id="btn_next_click" event="onclick()" for="btn_next">
			window.document.body.style.cursor='wait';
			var chkShowDoc = window.document.getElementById("chk_showDoc");
			if (chkShowDoc!=null && chkShowDoc.checked)
			{
			    ShowWaitingPage();
			}
		</script>
		<script language="javascript" id="btn_last_click" event="onclick()" for="btn_last">
			window.document.body.style.cursor='wait';
			var chkShowDoc = window.document.getElementById("chk_showDoc");
			if (chkShowDoc!=null && chkShowDoc.checked)
			{
			    ShowWaitingPage();
			}
		</script>	
        
        <style>
        .spazio{
        margin-bottom:5px;
        }
        </style>	
        		
</HEAD>
	<body bottomMargin="1" leftMargin="1" topMargin="1" rightMargin="1">
		<form id="Form1" method="post" runat="server">
		    <asp:ScriptManager ID="scriptManager" runat="server" AsyncPostBackTimeout="3"></asp:ScriptManager>
            <script type="text/javascript">
                var scrollTop;
                var scrollTopBottom;

                Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(BeginRequestHandler);
                Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

                function BeginRequestHandler(sender, args) {
                    var elem = $get("pnlContainerUoAppartenenza");
                    scrollTop = elem.scrollTop;

                    var elemBottom = $get("pnlContainerUoInferiori");
                    scrollTopBottom = elemBottom.scrollTop;

                }

                function EndRequestHandler(sender, args) {
                    var elem = $get("pnlContainerUoAppartenenza");
                    elem.scrollTop = scrollTop;

                    var elemBottom = $get("pnlContainerUoInferiori");
                    elemBottom.scrollTop = scrollTopBottom;
                }
            </script>
            	
			<input id="hd_msg_rifiuto" type="hidden" name="hd_msg_rifiuto" runat="server">
            <asp:HiddenField ID="hdUOapp" runat="server" />
            <asp:HiddenField ID="hdIsEnabledNavigaUO" runat="server" />
            <asp:HiddenField ID="txtNoteInd" runat="server" />
            <asp:HiddenField ID="hdCheckCompetenza" runat="server" /> 
            <asp:HiddenField ID="hdDescrCompetenza" runat="server" /> 
            <asp:HiddenField ID="hdDescrConoscenza" runat="server" /> 
            <asp:HiddenField ID="hdCheckConoscenza" runat="server" />
            <asp:HiddenField ID="hdCheckedUtenti" runat="server" />   
            <asp:HiddenField ID="hdTipoDestCompetenza" runat="server" /> 
            <asp:HiddenField ID="hdTipoDestConoscenza" runat="server" />      
            <asp:HiddenField ID="hdCountNavigaDown" runat="server"  Value="0"/>
            <asp:HiddenField ID="hdSelSmista" runat="server"/>
            <asp:Panel ID="pnlContainer" runat="server" Width="100%" Height="100%" ScrollBars="Auto" BackColor="#ffffff" >
		        <!-- tabella informazioni documento -->
		        <asp:panel ID="pnlHeaderContainer" runat="server" Height="20%" ScrollBars="Auto">
                    <TABLE cellSpacing="1" cellPadding="1" width="100%" border="0">											
				        <TR>
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC">SEGNATURA</TD>
					        <TD class="titolo_rosso" width="80%" bgcolor="#F2F2F2" colspan=5><asp:label id="lbl_segnatura" runat="server" CssClass="menu_1_rosso"></asp:label></TD>
				        </TR>
                        <TR>
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC" id="trTipologyTitle" runat="server">TIPOLOGIA</TD>
					        <TD class="titolo_rosso" width="80%" bgcolor="#F2F2F2" colspan="5" id="tdTipology" runat="server"><asp:label id="lblTipology" runat="server" CssClass="menu_1_rosso" ForeColor="Black" Font-Bold="false"></asp:label></TD>
				        </TR>
                        <TR>
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC" id="segn_repert" runat="server">REPERTORIO</TD>
					        <TD class="titolo_rosso" width="80%" bgcolor="#F2F2F2" colspan=5 id="segn_repert_val" runat="server"><asp:label id="lbl_segn_repertorio" runat="server" CssClass="menu_1_rosso" ForeColor="Red"></asp:label></TD>
				        </TR>
				        <TR>
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC">DATA</TD>
					        <TD class="testo10N" width="28%" bgcolor="#F2F2F2" align=left><asp:label id="lbl_dataCreazione" runat="server" /></TD>
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC">ALLEGATI</TD>
					        <TD class="testo10N" width="6%" bgcolor="#F2F2F2" align=center><asp:label id="lbl_allegati" runat="server" /></TD>											
					        <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC">VERSIONI</TD>
					        <TD class="testo10N" width="6%" bgcolor="#F2F2F2" align=center><asp:label id="lbl_versioni" runat="server" /></TD>
				        </TR>
				        <TR>
					        <TD class="testo_grigio_scuro" width="20%" valign="top" bgcolor="#E5E8EC">MITTENTE</TD>
					        <TD class="testo10N" width="80%" bgcolor="#F2F2F2" colspan=5><asp:label id="lbl_mittente" runat="server"></asp:label></TD>
				        </TR>
				        <TR>
					        <TD class="testo_grigio_scuro" width="20%" valign="top" bgcolor="#E5E8EC">DESTINATARI</TD>
					        <TD class="testo10N" width="80%" bgcolor="#F2F2F2" colspan=5><asp:label id="lbl_destinatario" runat="server"></asp:label></TD>
				        </TR>																																	
				        <TR>
				            <TD class="testo_grigio_scuro" width="20%" bgcolor="#E5E8EC">RAG. TRASM.</TD>
					        <TD class="testo10N" width="80%" bgcolor="#F2F2F2" colspan=5><asp:Label ID="lbl_descRagTrasm" runat="server" /></TD>
																			
				        </TR>
				        <TR>
					        <TD class="testo_grigio_scuro" vAlign="top" width="20%" bgcolor="#E5E8EC">OGGETTO</TD>
					        <TD class="testo10N" width="80%" bgcolor="#F2F2F2" colspan=5><asp:label id="lbl_oggetto" runat="server"></asp:label></TD>
				        </TR>
				        <TR>
					        <TD class="testo_grigio_scuro" vAlign="top" width="20%" bgcolor="#E5E8EC">MITT. TRASM.</TD>
					        <TD class="testo10N" width="80%" bgcolor="#F2F2F2" colspan=5><asp:label id="lbl_mitt_trasm" runat="server"></asp:label></TD>
				        </TR>
			        </TABLE>
		        </asp:panel>
		        <asp:panel ID="Panel1" runat="server" Height="70%" ScrollBars="Auto">
		            <!-- tabella navigazione documenti -->
    		        <asp:panel id="pnl_navigationButtons" Runat="server" Width="100%">
                    <TABLE cellSpacing=0 cellPadding=0 width="100%" align="center" bgColor="#ffffff" border="0">
                        <TR style="height:25px">
                            <TD align="right" width="30%">                             
                                <asp:imagebutton id="btn_first" runat="server" ImageUrl="../images/smistamento/FIRST.bmp"></asp:imagebutton></TD>
                            <TD align="right" width="5%">
                                <asp:imagebutton id="btn_previous" runat="server" ImageUrl="../images/smistamento/PREV.bmp"></asp:imagebutton></TD>
                            <TD class="testo_grigio_scuro" align="center" width="20%">
                                <asp:label id="lbl_contatore" runat="server"></asp:label></TD>
                            <TD align="left" width="5%">
                                <asp:imagebutton id="btn_next" runat="server" ImageUrl="../images/smistamento/NEXT.bmp"></asp:imagebutton></TD>
                            <TD align="left" width="20%">
                                <asp:imagebutton id="btn_last" runat="server" ImageUrl="../images/smistamento/LAST.bmp"></asp:imagebutton></TD>
                            <TD align="left" width="7%">
                                <asp:imagebutton id="btn_clearFlag" runat="server" ImageUrl="../images/clearFlag.gif" AlternateText="Elimina tutte le selezioni"></asp:imagebutton>                              
                            </TD>  
                            <td align="left" width="7%">
                                <asp:ImageButton ID="btn_selezioniSmistamento" runat="server" 
                                    ImageUrl="../images/proto/selezioneSmistamento.jpg" 
                                    ToolTip="visualizza selezioni smistamento" 
                                    onclick="btn_selezioniSmistamento_Click"/>
                            </td>                                     
                        </TR>
                    </TABLE>
		        </asp:panel>
	    	    <!-- datagrid uo appartenenza -->
		        <asp:Panel ID="pnlDataContainer" runat="server" Height="490px" ScrollBars="Auto">
                    <table cellSpacing="0" cellPadding="1"  Border="0" class="contenitore_gray" width="100%" style="margin-bottom:5px;">
                        <tr>							   
                            <td class="testo_grigio_scuro" width="20%" valign="middle">
                                &nbsp;Note generali:
                             </td>
                            <td width="80%">
                                <asp:TextBox id="txtNoteGen" runat="server" Rows=2 TextMode=MultiLine Width="98%" CssClass="testo_grigio"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>							   
                            <td class="testo_grigio_scuro" width="20%" valign="middle">
                                &nbsp;Note individuali:
                            </td>
                            <td width="80%">
                                <asp:TextBox id="txtAreaNoteInd" runat="server" Rows="2" TextMode="MultiLine" Width="98%" ReadOnly=true CssClass="testo_grigio" BackColor="#f2f2f2"></asp:TextBox>
                            </td>
                        </tr>
                    </table>

                    <asp:panel id="pnl_fasc_rapida" Runat="server" Visible="true">    
                        <table class="contenitore_gray" cellspacing="0" cellpadding="0" width="100%" align="center" border="0" style="margin-bottom:5px;">
                            <tr>
                                <td class="testo_grigio_scuro" style="width: 20%">
                                    &nbsp;Fascicolazione:
                                </td>
                                <td style="width: 80%" align="left">
                                    <asp:textbox id="txt_CodFascicolo" runat="server" Width="20%" CssClass="testo_grigio" AutoPostBack="True" ontextchanged="txt_CodFascicolo_TextChanged"></asp:textbox>
				                    <asp:textbox id="txt_DescFascicolo" runat="server" Width="60%" CssClass="testo_grigio"></asp:textbox>
                                    <cc1:imagebutton id="btn_titolario" Runat="server" ImageUrl="../images/proto/ico_titolario_noattivo.gif" AlternateText="Titolario" Tipologia="DO_CLA_TITOLARIO" DisabledUrl="../images/proto/ico_titolario_noattivo.gif" OnClick="btn_titolario_Click"></cc1:imagebutton>
                                    <asp:ImageButton ID="btn_cerca_fasc" runat="server" ImageUrl="../images/proto/ico_fascicolo_trasp.gif" onclick="btn_cerca_fasc_Click" />
                                </td>
                            </tr>
                        </table>  
                    </asp:panel>
                  

                    <asp:Panel ID="pnl_trasm_rapida" runat="server" >
                        <table class="contenitore_gray" cellpadding="0" cellspacing="0" width="100%" border="0" align="center" style="margin-bottom:5px;">
                            <tr>
                                <td style="width:20%" class="testo_grigio_scuro">
                                    &nbsp;Trasmissione rapida:
                                </td>
                                <td style="width:80%">
                                    <asp:DropDownList CssClass="testo_grigio" id="ddl_trasm_rapida" runat="server" AutoPostBack="True" Width="98%" onselectedindexchanged="ddl_trasm_rapida_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel> 

                    
                        <asp:UpdatePanel ID="updatePanelUOApp" runat="server">
                            <ContentTemplate>
                            <asp:Panel ID="pnlContainerUoAppartenenza" runat="server" ScrollBars="Auto" Height="175px" CssClass="spazio">
                                <asp:datagrid id="grdUOApp" runat="server" SkinID="datagrid" Width="100%" BorderWidth="1px" ShowHeader="true" CellPadding="1" BorderColor="Gray" AutoGenerateColumns="False">
		                            <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
		                            <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
		                            <ItemStyle CssClass="bg_grigioN"></ItemStyle>
		                            <HeaderStyle HorizontalAlign="Center" CssClass="testo_biancoN"></HeaderStyle>								  
		                            <Columns>								
			                            <asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
			                            <asp:BoundColumn Visible="False" DataField="TYPE" HeaderText="TYPE"></asp:BoundColumn>
			                            <asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Unit&#224; organizzativa di appartenenza">
				                            <HeaderStyle Width="55%"></HeaderStyle>
				                            <ItemStyle VerticalAlign="Middle"></ItemStyle>
			                            </asp:BoundColumn> 
			                            <asp:ButtonColumn CommandName="navigaUO_up" Text="&lt;img src='../AdminTool/Images/up_trasp.gif' border='0' alt='UO padre'&gt;">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:ButtonColumn>                                                                           
                                        <asp:TemplateColumn Visible="false">
				                            <HeaderStyle Width="25%"></HeaderStyle>
				                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"></ItemStyle>
				                            <ItemTemplate>
                                                <asp:Label runat="server" ID="hd_id" Text= '<%# DataBinder.Eval(Container, "DataItem.PARENT") %>' />
                                                <asp:Label runat="server" ID="hdGerarchia"  Text= '<%# DataBinder.Eval(Container, "DataItem.GERARCHIA") %>' />
                                                <asp:Label runat="server" ID="hd_disablendTrasm"  Text= '<%# DataBinder.Eval(Container, "DataItem.DISABLED_TRASM") %>' />
				                            </ItemTemplate>
			                            </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="COMP">
				                            <HeaderStyle Width="10%"></HeaderStyle>
				                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                            <ItemTemplate>
				                                <asp:CheckBox ID="chkComp" runat="server" TextAlign="Right" Text="" AutoPostBack="true" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged" />
				                            </ItemTemplate>
				                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="CC">
				                            <HeaderStyle Width="10%"></HeaderStyle>
				                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                            <ItemTemplate>
				                                <asp:CheckBox ID="chkCC" runat="server" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged"/>
				                            </ItemTemplate>
				                        </asp:TemplateColumn>
                                        <asp:TemplateColumn HeaderText="Notifica">
				                            <HeaderStyle Width="10%"></HeaderStyle>
				                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                            <ItemTemplate>
				                                <asp:CheckBox ID="chkNotifica" runat="server" TextAlign="Right" AutoPostBack="true" Enabled="false" OnCheckedChanged="OpzioniUoAppartenenza_CheckedChanged" />
				                            </ItemTemplate>
				                        </asp:TemplateColumn>				
                                        <asp:TemplateColumn HeaderText="Note">
				                            <HeaderStyle Width="10%"></HeaderStyle>
				                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                            <ItemTemplate>
				                                <asp:ImageButton ID="imgNote" runat="server" CssClass="pulsante_hand" ImageUrl="../images/smistamento/note.gif" ToolTip="Inserimento dati aggiuntivi" Visible="false" />
				                            </ItemTemplate>
				                        </asp:TemplateColumn>								                
		                            </Columns>								
	                            </asp:datagrid>	
                                 </asp:Panel> 	                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                   
                    
			        <!-- datagrid uo inferiori -->	
                    			
                        <asp:UpdatePanel ID="updatePanelUOInf" runat="server">
                            <ContentTemplate>		
                            <asp:Panel ID="pnlContainerUoInferiori" runat="server" ScrollBars="Auto" Height="115px">			
			                    <asp:datagrid id="grdUOInf" runat="server" SkinID="datagrid" Width="100%" BorderWidth="1px" ShowHeader="true" 
                                        CellPadding="1" BorderColor="Gray"
				                        AutoGenerateColumns="False">
				                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
				                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
				                        <ItemStyle CssClass="bg_grigioN"></ItemStyle>
				                        <HeaderStyle HorizontalAlign="Center" CssClass="testo_biancoN"></HeaderStyle>
				                        <Columns>
					                        <asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID" ></asp:BoundColumn>
					                        <asp:BoundColumn Visible="False" DataField="TYPE" HeaderText="TYPE"></asp:BoundColumn>
					                        <asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Unit&#224; organizzative di livello inferiore">
						                        <HeaderStyle Width="55%"></HeaderStyle>
						                        <ItemStyle VerticalAlign="Middle"></ItemStyle>
					                        </asp:BoundColumn>	
					                        <asp:ButtonColumn CommandName="navigaUO_down" Text="&lt;img src='../AdminTool/Images/down_trasp.gif' border='0' alt='UO livello inferiore'&gt;">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:ButtonColumn>								
                                            <asp:TemplateColumn HeaderText="COMP">
				                                <HeaderStyle Width="10%"></HeaderStyle>
				                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                                <ItemTemplate>
				                                    <asp:CheckBox ID="chkComp" runat="server" TextAlign="Right" Text="" AutoPostBack="true" OnCheckedChanged="OpzioniUoInferiori_CheckedChanged" />
				                                </ItemTemplate>
				                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="CC">
				                                <HeaderStyle Width="10%"></HeaderStyle>
				                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                                <ItemTemplate>
				                                    <asp:CheckBox ID="chkCC" runat="server" TextAlign="Right" AutoPostBack="true" OnCheckedChanged="OpzioniUoInferiori_CheckedChanged"/>
				                                </ItemTemplate>
				                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="Note">
				                                <HeaderStyle Width="10%"></HeaderStyle>
				                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                                <ItemTemplate>
				                                    <asp:ImageButton ID="imgNote" runat="server" CssClass="pulsante_hand" ImageUrl="../images/smistamento/note.gif" ToolTip="Inserimento dati aggiuntivi" Visible="false" />
				                                </ItemTemplate>
				                            </asp:TemplateColumn>
                                            <asp:TemplateColumn HeaderText="">
				                                <HeaderStyle Width="10%"></HeaderStyle>
				                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
				                                <ItemTemplate>
				                                </ItemTemplate>
				                            </asp:TemplateColumn>												                                            
		                            </Columns>
		                        </asp:datagrid>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>						            
                    
                </asp:Panel>
                </asp:panel>
                
                <asp:panel ID="Panel2" runat="server" Height="10%" ScrollBars="Auto">
                    <!-- tabella pulsanti -->			
    		        <TABLE cellSpacing="2" cellPadding="0" width="100%" bgColor="#ffffff" border="0">						  
			        <TR>
				        <TD align="left">
					        <asp:checkbox id="chk_showDoc" CssClass="testo_grigio_scuro" Runat="server" Text="Visual. doc."
						        TextAlign="Right" AutoPostBack="True"  ToolTip="Visualizza il documento (se acquisito)"></asp:checkbox>
					        <asp:checkbox id="chk_mantieniSel" CssClass="testo_grigio_scuro" Runat="server" Text="Mantieni selezione"
						        TextAlign="Right" AutoPostBack="True" ToolTip="Mantiene le selezioni dei destinatari"></asp:checkbox>
                        </TD>
			        </TR>								
			        <TR>
				        <TD align="right">
					        <asp:button id="btn_zoom" runat="server" CssClass="PULSANTE" Width="60px" Text="Zoom" ToolTip="Zoom"></asp:button>
					        <asp:button id="btn_dettFirma" runat="server" CssClass="PULSANTE" Width="75px" Text="Dett. Firma" ToolTip="Visualizza i dettagli della firma"></asp:button>
					        <asp:button id="btn_AdL" runat="server" CssClass="PULSANTE" Width="60px" Text="AdL" ToolTip="Inserisce il documento in AdL" onclick="btn_AdL_Click"></asp:button>
					        <asp:button id="btn_scarta" runat="server" CssClass="PULSANTE" Width="60px" Text="Visto" ToolTip="Imposta il documento come VISTO e lo toglie dalla lista delle COSE DA FARE"></asp:button>
					        <asp:button id="btn_rifiuta" runat="server" CssClass="PULSANTE" Width="60px" Text="Rifiuta" ToolTip="Rifiuta il documento trasmesso" ></asp:button>
					        <asp:button id="btn_smista" runat="server" CssClass="PULSANTE" Width="60px" Text="Smista" ToolTip="Trasmette il documento ai destinatari selezionati e lo toglie dalla lista delle COSE DA FARE"></asp:button>
					        <asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Width="60px" Text="Chiudi" ToolTip="Chiude la pagina di smistamento documenti"></asp:button>
                            <cc1:MessageBox id="MessageBox1" runat="server"></cc1:MessageBox>
                        </TD>					        
			        </TR>
		        </TABLE>						
		        </asp:Panel>
			</asp:Panel>  
		</form>
	</body>
</HTML>
