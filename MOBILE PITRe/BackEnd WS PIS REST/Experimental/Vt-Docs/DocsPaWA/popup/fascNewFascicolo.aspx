<%@ Page language="c#" Codebehind="fascNewFascicolo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.fascNewFascicolo" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../Note/DettaglioNota.ascx" tagname="DettaglioNota" tagprefix="uc1" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head runat="server">
		<title></title>
        <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
        <meta content="C#" name="CODE_LANGUAGE">
        <meta content="JavaScript" name="vs_defaultClientScript">
        <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
        <LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
        <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
        <script language="javascript" src="../CSS/ETcalendar.js"></script>
        <script language="javascript" src="../LIBRERIE/rubrica.js"></script>
 
        <script language=javascript id=btn_salva_click event=onclick() for=btn_salva>
			//Controllo anche i campi obbligatori prima di lanciare la modale!!!
			checkFields();
			if(document.getElementById("abilitaModaleVis").value == "true" && document.getElementById("fieldsOK").value == "true")
			{
			    AvvisoVisibilita();
			}
			document.getElementById('btn_salva').style.display='none';
			document.getElementById('btn_salva_disabled').style.display='';
			window.document.body.style.cursor='wait';
		</script>

        <script language=javascript>
			function createFascicolo()
			{					
				window.navigate = '../ricercaFascicoli/tabRisultatiRicFasc.aspx?newFasc=1';
			}

			function _ApriRubrica(target)
			{
				var r = new Rubrica();
				
				switch (target) {
					// Gestione fascicoli - Locazione fisica
					case "nf_locfisica":
						r.CallType = r.CALLTYPE_NEWFASC_LOCFISICA;
						break;
						
					// Gestione fascicoli - Ufficio referente
					case "nf_uffref":
						r.CallType = r.CALLTYPE_NEWFASC_UFFREF;
						break;							
				}
				var res = r.Apri(); 		
			}	
			
			function nascondi()
			{
				document.getElementById('btn_salva_disabled').style.display='none';
			}
			
			function clearSelezioneEsclusiva(id, numeroDiScelte)
			{
				numeroDiScelte--;
				while(numeroDiScelte >= 0)
				{
					var elemento = id+"_"+numeroDiScelte;
					document.getElementById(elemento).checked = false;
					numeroDiScelte--;
				}
			}
			
            function AvvisoVisibilita()
            {
                var newLeft=(screen.availWidth-500);
		        var newTop=(screen.availHeight-500);	
			    var newUrl;
    		
		        newUrl="../popup/estensioneVisibilita.aspx";
    		    
	            if(IsCheckBoxRequired())
	            {
	                retValue=window.showModalDialog(newUrl,"","dialogWidth:306px;dialogHeight:188px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:yes;help:no;");
    	            
	                if(retValue == 'NO')
	                {
	                    document.getElementById("estendiVisibilita").value = "true";
	                }
	                else
	                {
	                    document.getElementById("estendiVisibilita").value = "false";
	                }
	            }
            }
            
            function IsCheckBoxRequired()
            {
	            if (document.getElementById("chkPrivato").checked == true)
	            {
		            return true;
	            } 
	            else
	            {
		            return false;
	            }
            }

            //Bisogna aggiungere eventualmente gli altri controlli!

            function checkFields() {
                document.getElementById("fieldsOK").value = "false";
                if (document.getElementById("txt_descFascicolo").value != '')
                    document.getElementById("fieldsOK").value = "true";

                if (document.getElementById("isCodFascFree").value == "true" && document.getElementById("txt_codice").value == '')

                    document.getElementById("fieldsOK").value = "false";

            }
            function checkFields_old()
            {
                if(document.getElementById("txt_descFascicolo").value != '' &&  document.getElementById("isCodFascFree").value=="true" && document.getElementById("txt_codice").value != '')
                {
                    document.getElementById("fieldsOK").value = "true";
                }
                else
                {
                    document.getElementById("fieldsOK").value = "false";
                }
            }
        
		</script>

        <style>
            .tabfasc{
                background-color:#fafafa;
                border:1px solid #eaeaea;
            }
        </style>
        <base target="_self"/>
    </head>
    <body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout" style="background-color:#eaeaea;">
        <form id="insertNewFascicolo" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
    	<asp:HiddenField ID="estendiVisibilita" runat="server" />
 		<asp:HiddenField ID="abilitaModaleVis" runat="server" />
 		<asp:HiddenField ID="fieldsOK" runat="server" />
 		<asp:HiddenField ID="isCodFascFree" runat="server" />
 		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Inserimento nuovo fascicolo" />
       <table class="info" width="99%" align="center" border="0" style="background-color:#f7f7f7;">
        <tr>
            <td class="item_editbox" height="20">
                <p class="boxform_item">Inserimento nuovo fascicolo</p></td>
        </tr>
        <tr>
            <td vAlign="top" align="left">
                <table class="tabfasc" width="100%">
                    <tr>
			            <td class="titolo_scheda"><asp:label id="lbl_codice" Runat="server">Codice</asp:label>
                            <asp:textbox id="txt_codice" runat="server" Width="90px" CssClass="testo_grigio" ReadOnly="True"></asp:textbox>
                        </td>
				    </tr>
                    <tr>
                        <td>
                            <asp:panel id="pnl_registri" Runat="server">
							    <asp:label id="lbl_registro" runat="server" Width="53px" CssClass="titolo_scheda">Registro</asp:label>
							    <asp:dropdownlist id="ddl_registri" runat="server" Width="115px" CssClass="testo_grigio" AutoPostBack="True"></asp:dropdownlist>
					        </asp:panel>
                        </td>
                    </tr>
                </table>
                <table class="tabfasc" width="100%" style="margin-top:5px;">
				    <tr>
					    <td class="titolo_scheda" >Descrizione *</td>
				    </tr>
                    <tr>
					    <td class="testo_grigio"><asp:textbox id="txt_descFascicolo" runat="server" Width="655px" CssClass="testo_grigio" Height="37px"
							    Rows="3" TextMode="MultiLine"></asp:textbox>
                       </td>
				    </tr>
                    <tr>      
                       <td class="testo_grigio" style="padding-left:505px;">
                        caratteri disponibili: <input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />
                       </td>
                    </tr>
                </table>
                <table class="tabfasc" width="100%" style="margin-top:5px;">
				    <tr>
					    <td>
                            <uc1:DettaglioNota ID="dettaglioNota" runat="server" TipoOggetto = "Fascicolo" 
                                Width = "655px" Rows = "3" PAGINA_CHIAMANTE="fascNewFascicolo"/>
                        </td>
				    </tr>
                </table>
                <table class="tabfasc" width="100%" style="margin-top:5px;">
				<tr>
				    <td class="titolo_scheda" >
					    <asp:CheckBox ID="chkFascicoloCartaceo" TextAlign="left" runat="server" Text="Cartaceo" />

					    <asp:CheckBox ID="chkPrivato" TextAlign="left" runat="server" Text="Privato" toltip="Fascicolo creato con visibilità limitata al solo ruolo e utente proprietario"/>

					    <asp:CheckBox ID="chkControllato" TextAlign="left" runat="server" Text="Impone visibilità" Checked="true"/>
					</td>
				</tr>	
                </table>
				<table class="tabfasc" width="100%" style="margin-top:5px;">					
				    <tr>
					    <td class="titolo_scheda" width="120px;">Collocazione Fisica</td>
                        <td> <asp:textbox id="txt_LFCod" runat="server" Width="100px" CssClass="testo_grigio" AutoPostBack="True"></asp:textbox>
					    <asp:textbox id="txt_LFDesc" runat="server" Width="390px" CssClass="testo_grigio" ReadOnly="True"></asp:textbox>&nbsp;<asp:image id="btnRubricaF" runat="server" ImageUrl="../images/proto/rubrica.gif" ImageAlign="AbsBottom"></asp:image>
					    </td>
				    </tr>
                </table>
                <table class="tabfasc" width="100%" style="margin-top:5px;">	
				<tr>
					<td class="titolo_scheda" width="120px;"><asp:Label ID="lbl_dta_LF" runat="server"></asp:Label></td>
					<td><uc3:Calendario id="txt_LFDTA" runat="server" Visible="true" /></td>
				</tr>
                </table>
				<asp:panel id="pnl_uff_ref" Runat="server" Visible="false">
                <table class="tabfasc" width="100%" style="margin-top:5px;">
					<tr>
						<td class="titolo_scheda">Ufficio Referente *</td>
					</tr>
					<tr>
						<td class="titolo_scheda">
							<asp:TextBox id="txt_cod_uffRef" runat="server" Width="100px" CssClass="testo_grigio" AutoPostBack="True"></asp:TextBox>&nbsp;
							<asp:TextBox id="txt_desc_UffRef" runat="server" Width="240px" CssClass="testo_grigio" ReadOnly="True"></asp:TextBox>&nbsp;
							<asp:Image id="btn_Rubrica_ref" runat="server" ImageUrl="../images/proto/rubrica.gif" ImageAlign="AbsBottom"></asp:Image>
                        </td>
					</tr>
                </table>
				</asp:panel>
                
                <table class="tabfasc" width="100%" style="margin-top:5px;">
	        	    <tr>
			            <td align="center"><asp:label id="lbl_nota" CssClass="testo_grigio_light" Runat="server"><u>Nota</u>: L'operazione di inserimento potrebbe richiedere alcuni secondi</asp:label></td>
		            </tr>
				</table>
			</td>
		</tr>							
	</table>
				
	<asp:Panel id="panel_profDinamica" runat="server" Visible="false">
	<table width="99%" class="info" align="center" style="margin-top:5px;background-color:#f7f7f7;">
        <tr>
            <td>
                <table class="tabfasc" width="100%" style="margin-top:5px;">
	                <tr>
	                    <td class="titolo_scheda">&nbsp;Tipologia fascicolo :&nbsp;&nbsp;</td>
	                    <td style="padding-top:5px"><asp:DropDownList ID="ddl_tipologiaFasc" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="350px" ></asp:DropDownList></td>
	                </tr>
                </table>
	            <asp:panel id="Panel_DiagrammiStato" Runat="server" Visible="false">
                 <table class="tabfasc" width="100%" style="margin-top:5px;">
			        <tr>
				        <td class="titolo_scheda">&nbsp;Stati :&nbsp;&nbsp;</td>
				        <td><asp:DropDownList id="ddl_statiSuccessivi" runat="server" Width="350px" CssClass="testo_grigio"></asp:DropDownList></td>				
			        </tr>
                  </table>			
		        </asp:panel>
		        <asp:panel id="Panel_DataScadenza" Runat="server" Visible="false" >
                    <table class="tabfasc" width="100%" style="margin-top:5px;">
		                <tr>
		                    <td class="titolo_scheda">&nbsp;Data Scadenza :&nbsp;&nbsp;</td>
				            <td><cc1:DateMask id="txt_dataScadenza" runat="server" CssClass="testo_grigio" Width="75px"></cc1:DateMask></td>
			            </tr>	
                    </table>																												   								    									    									
		        </asp:panel>
                 <table class="tabfasc" width="100%" style="margin-top:5px;">
	            <tr>	                
                    <td><asp:panel id="panel_Contenuto" runat="server" Width="100%" Height="215" ScrollBars="Auto"></asp:panel></td>
	            </tr>	
                </table>    
            </td>
        </tr>
	</table>
	</asp:Panel>
				
	<table width="100%">
	    <tr>
		    <td align="center">
			    <asp:button id="btn_salva" runat="server" CssClass="pulsante_hand" Text="Inserisci" Width="80px"></asp:button>&nbsp;
			    <asp:button id="btn_salva_disabled" runat="server" CssClass="pulsante_hand" Text="Inserisci" EnableViewState="False" Enabled="False" Width="80px"></asp:button>&nbsp;
		        <asp:button id="btn_annulla" runat="server" CssClass="pulsante_hand" Text="Chiudi" Width="80px"></asp:button>
		    </td>
        </tr> 
    </table>   	
</form>

<script language="javascript">		
	esecuzioneScriptUtente();
</script>

</body>
</HTML>
