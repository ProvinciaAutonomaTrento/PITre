<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Page language="c#" Codebehind="EstrazioneLog.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Logs.EstrazioneLog" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuLog" Src="../UserControl/MenuLog.ascx" %>
<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc4" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
        		<script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../../LIBRERIE/rubrica.js"></script>

        <style type="text/css">
            .box_tempo{
                margin:0px;
                padding:0px;
                margin-top:5px;
            }    
        
            .box_tempo fieldset{
                margin:0px;
                padding:0px;
                border:1px solid #666666;
                padding:5px;
            } 
        
            .box_tempo legend{
                font-weight:bold; 
                font-size: 10px; 
                color:#666666; 
                font-family:Verdana
            }       
            
            #button_find fieldset{
                margin:0px;
                padding:0px;
                border:0px;
                margin-top:10px;
                text-align:center;
            }  

            .risultati_logici{
                margin-top:10px;
                text-align:left;
            }

            .ipa_periodo{
                margin:0px;
                padding:0px;
                border-bottom:1px dotted #cccccc;
            }

            .ipa_periodo ul{
                margin:0px;
                padding:0px;
                padding-left:30px;
            }

            .ipa_periodo li{
                margin:0px;
                padding:0px;
                font-weight:bold; 
                font-size: 10px; 
                color:#990000;
                list-style-type: square; 
                text-align:left;
                font-family:Verdana
            }

            .grigio{
                color:#666666;
            }

            .testo_finale{
                color:#666666;
                font-size:10px;
                line-height:18px;
                font-family:Verdana
            }

            .inp2{
                width:20px;
            }   
        </style>

        <script language="javascript">

            //RICERCA RUOLI
            function _ApriRubricaRicercaRuoli() {
                var r = new Rubrica();
                r.MoreParams = "tipo_corr=" + "R";
                r.CallType = r.CALLTYPE_TUTTI_RUOLI;
                var res = r.Apri();
            }

            //RICERCA UO
            function _ApriRubricaRicercaUO() {
                var r = new Rubrica();
                r.MoreParams = "tipo_corr=" + "U";
                r.CallType = r.CALLTYPE_TUTTE_UO;
                var res = r.Apri();
            }

            function showWait(sender, args) {
                // Viene visualizzato il popup di wait    
                this._popup = $find('mdlPopupWait');
                this._popup.show();
            }

            function doWait() {
                var w_width = 300;
                var w_height = 100;
                var t = (screen.availHeight - w_height) / 2;
                var l = (screen.availWidth - w_width) / 2;
                if (document.getElementById("please_wait")) {
                    document.getElementById("please_wait").style.top = t;
                    document.getElementById("please_wait").style.left = l;
                    document.getElementById("please_wait").style.width = w_width;
                    document.getElementById("please_wait").style.height = w_height;
                    //document.getElementById ("please_wait").style.visibility = "visible";
                    document.getElementById("please_wait").style.display = '';

                }
            }

            function MaxCaratteri(Object, MaxLen){
	            return (Object.value.length <= MaxLen);
            }
 

        </script>


        <script language="javascript" id="btnCerca_Click" event="onclick()" for="btn_stampa">
			      window.document.body.style.cursor='wait';
			      doWait();
			
			
    </script>

	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0">
		<form id="Form1" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="36000">
         </asp:ScriptManager>  
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Estrazione Log" />
		    <!-- Gestione del menu a tendina -->
            <uc3:menutendina id="MenuTendina" runat="server"></uc3:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' -->
                        <uc1:testata id="Testata" runat="server"></uc1:testata>
                   </td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20">
                        <asp:label id="lbl_position" runat="server"></asp:label>
                    </td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" height="20">Estrazione log</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
                            <!-- MENUSX -->
								<td width="120" height="100%">
                                    <uc2:menulog id="MenuLog" runat="server"></uc2:menulog>
								</td>
                                <!-- CONTENT -->
                                <td vAlign="top" align="center" width="100%" height="100%">
                                    <br />
                                    <!-- AMMINISTRAZIONE -->
                                    <table cellSpacing="0" cellPadding="0" border="0" style="padding:5px;">
										<tr>
											<td class="pulsanti" align="center" width="840">
                                               
                                               <table width="100%" cellpadding="0" cellspacing="0">
										            <tr>
														<td class="titolo" width="170px;">Amministrazione:</td>
                                                        <td class="testo_grigio_scuro">
                                                            <asp:dropdownlist id="ddl_amministrazioni" runat="server" CssClass="testo" AutoPostBack="True" Width="270px"></asp:dropdownlist>        
                                                        </td>
													</tr>
                                               </table>
                                            </td>
                                           </tr>
                                       </table>   
                                       <br />

                                    <!-- BOX STRUTTURA ORGANIZZATIVA -->
                                    <table cellSpacing="0" cellPadding="0" border="0" style="padding:5px;">
										<tr>
											<td class="pulsanti" align="center" width="840">
                                               
                                               <table width="100%" cellpadding="0" cellspacing="0">
										            <tr>
														<td class="titolo" width="170px;">Struttura Organizzativa:</td>
                                                        <td class="testo_grigio_scuro">
                                                          <asp:radiobuttonlist id="rbl_ruoli" runat="server" CssClass="testo_grigio_scuro" AutoPostBack="True"  RepeatDirection="Horizontal"></asp:radiobuttonlist>         
                                                        </td>
													</tr>
                                                    <asp:Panel ID="pnl_ruolo" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="padding-top:10px;text-align:right;padding-right:10px;vertical-align:bottom;">Scegli un ruolo:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:10px;text-align:left">
                                                            <asp:textbox id="txt_codRuolo" runat="server" AutoPostBack="True"  CssClass="testo" Width="75px"></asp:textbox>&nbsp;
													        <asp:textbox id="txt_descRuolo" runat="server"  CssClass="testo" Width="220px"></asp:textbox>&nbsp;
                                                            <asp:Image id="btn_Rubrica_Ruolo" runat="server" ToolTip="Seleziona un ruolo nella rubrica" ImageUrl="../../images/proto/rubrica.gif" AlternateText="Seleziona un ruolo nella rubrica"></asp:Image> 
                                                            <INPUT id="hd_systemIdCorrRuolo" type="hidden" size="1" name="hd_systemIdCorrRuolo" runat="server">
                                                        </td>
                                                    </tr>
                                                    </asp:Panel> 
                                                    <asp:Panel ID="pnl_uo" runat="server" Visible="false">
                                                    <tr>
                                                    <td class="testo_grigio_scuro" style="padding-top:10px;text-align:right;padding-right:10px;vertical-align:bottom;">Scegli una UO:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:10px;text-align:left">
                                                            <asp:textbox id="txt_codUo" runat="server" AutoPostBack="True"  CssClass="testo" Width="75px"></asp:textbox>&nbsp;
													        <asp:textbox id="txt_descUo" runat="server" CssClass="testo" Width="220px"></asp:textbox>&nbsp;
                                                            <asp:Image id="btn_Rubrica_Uo" runat="server" ToolTip="Seleziona una uo nella rubrica" ImageUrl="../../images/proto/rubrica.gif" AlternateText="Seleziona una uo nella rubrica"></asp:Image> 
                                                            <INPUT id="hd_systemIdCorrUO" type="hidden" size="1" name="hd_systemIdCorrUO" runat="server">
                                                        </td> 
                                                    </tr>  
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="padding-top:10px;text-align:right;padding-right:10px;">Considera sottoposti:</td>
                                                        <td style="padding-top:10px;"><asp:CheckBox ID="chk_sottoposti" runat="server" CssClass="testo" AutoPostBack="true"/></td>
                                                    </tr>                                                  
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnl_rf" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:right;padding-right:10px;vertical-align:bottom;padding-bottom:1px;">Scegli un RF:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:left;padding-bottom:1px;">
                                                            <asp:DropDownList ID="ddl_rf" runat="server" Width="232px" CssClass="testo" AutoPostBack="true">
                                                             </asp:DropDownList>&nbsp; <asp:Label runat="server" ID="lbl_rf" class="testo_grigio_scuro"></asp:Label>
                                                        </td> 
                                                    </tr>                                                       
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnl_aoo" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:right;padding-right:10px;vertical-align:bottom;padding-bottom:1px;">Scegli una AOO:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:left;padding-bottom:1px;">
                                                            <asp:DropDownList ID="ddl_aoo" runat="server" Width="232px" CssClass="testo" AutoPostBack="true">
                                                             </asp:DropDownList>
                                                        </td> 
                                                    </tr>                                                       
                                                    </asp:Panel>
                                                    <asp:Panel ID="pnl_tipo_ruolo" runat="server" Visible="false">
                                                    <tr>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:right;padding-right:10px;vertical-align:bottom;padding-bottom:1px;">Scegli un tipo ruolo:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:16px;text-align:left;padding-bottom:1px;">
                                                            <asp:DropDownList ID="ddl_tp" runat="server" Width="232px" CssClass="testo" AutoPostBack="true">
                                                             </asp:DropDownList>
                                                        </td> 
                                                    </tr>                                                       
                                                    </asp:Panel>
                                               </table>
                                            </td>
                                           </tr>
                                       </table>   
                                       <br />
                                        <!-- BOX TIPO DI EVENTO -->
                                     <table cellSpacing="0" cellPadding="0" border="0" style="padding:5px;">
									    <tr>
										    <td class="pulsanti" align="center" width="840">
                                                <table width="100%" cellpadding="0" cellspacing="0">
										            <tr>
														<td class="titolo" width="170px">Tipo di evento:</td>
													</tr>
                                                 </table>
                                                 <asp:Panel ID="pnl_tipo_evento" runat="server" Visible="false">
                                                 <table width="100%" cellpadding="0" cellspacing="0">
										            <tr>
                                                        <td class="testo_grigio_scuro" width="60px;" style="padding-top:7px;padding-left:5px;">Oggetto:</td>
													    <td class="testo_grigio_scuro" style="padding-top:7px;padding-left:5px;width:220px;">
                                                            <asp:dropdownlist id="ddl_oggetto" runat="server" CssClass="testo" Width="213px" AutoPostBack="True">
															<asp:ListItem Value="null" Selected="True">Seleziona...</asp:ListItem>
															</asp:dropdownlist></td>
                                                        <td class="testo_grigio_scuro" width="55px;" style="padding-top:7px;padding-left:5px;">Azione:</td>
                                                        <td class="testo_grigio_scuro" style="padding-top:7px;padding-left:5px;">
                                                            <asp:dropdownlist id="ddl_azione" runat="server" CssClass="testo" Width="355px" AutoPostBack="true">
                                                            </asp:dropdownlist>
                                                        </td>
													</tr>
                                                 </table>
                                                 </asp:Panel>
                                                 <asp:Panel id="pnl_no_log" runat="server" Visible="false">
                                                     <table width="100%" cellpadding="0" cellspacing="0">
										                 <tr>
														    <td style="color:#990000;text-align:center;font-weight:bold;padding-top:7px;"><asp:Label ID="lbl_errore" runat="server"></asp:Label></td>
													    </tr>
                                                    </table>
                                                 </asp:Panel>
                                            </td>
                                        </tr>
                                      </table> 
                                      <br />
                                        <!-- BOX TEMPO -->
                                     <table cellSpacing="0" cellPadding="0" border="0" style="padding:5px;">
									    <tr>
										    <td class="pulsanti" align="center" width="840">
                                                <table width="100%" cellpadding="0" cellspacing="0">
										            <tr>
														<td class="titolo" width="170px;">Intervallo temporale:</td>
													</tr>
                                                 </table>
                                                    <asp:Panel ID="pnl_iop" GroupingText="Intervallo di osservazione principale (IOP)" runat="server" CssClass="box_tempo">
                                                    <table width="100%" cellpadding="0" cellspacing="0">   
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="100px;" style="padding-top:10px;padding-right:5px;text-align:right;">Data inizio:</td>
                                                            <td style="padding-top:11px;width:100px;"><uc4:Calendario id="txt_data_da" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress='#MaxCaratteri(this, 0);'/></td>
                                                            <td style="padding-top:8px;width:11"><asp:ImageButton ID="eliminaDataDa" runat="server" ImageUrl="../Images/noData.gif" width="16" height="16" border="0" alt="Elimina la data DA" /></td>
                                                            <td class="testo_grigio_scuro" width="100px;" style="padding-top:10px;padding-right:5px;text-align:right;">Data fine:</td>         
                                                            <td style="padding-top:11px;width:100px;"><uc4:Calendario id="txt_data_a" runat="server" Visible="true" PaginaChiamante="esportalog" CSS="testo" onkeypress="return MaxCaratteri(this, 0);"/></td>
                                                            <td style="padding-top:8px;"><asp:ImageButton ID="eliminaDataA" runat="server" ImageUrl="../Images/noData.gif" width="16" height="16" border="0" alt="Elimina la data A" /></td>
                                                            
                                                        </tr>
                                                    </table>
                                                        <div class="risultati_logici">
                                                            <asp:Panel ID="pnl_risultati_iop" CssClass="testo_grigio_scuro" Visible="false" runat="server">
                                                                <asp:label id="lbl_quanto_iop" runat="server"></asp:label>
                                                            </asp:Panel>
                                                        </div>
                                                    </asp:Panel>

                                                    <asp:Panel ID="pnl_ipa" GroupingText="Intervallo periodico di analisi (IPA)" runat="server" CssClass="box_tempo" Visible="false">
                                                       <div class="ipa_periodo">
                                                            <ul>
                                                               <li><span class="grigio">Periodicità</span></li>
                                                           </ul>
                                                       </div>
                                                       <table width="100%" cellpadding="0" cellspacing="0">   
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="35px;" style="padding-left:20px;padding-top:5px;">Tipo</td>
                                                            <td class="testo_grigio_scuro" style="padding-top:5px;"> 
                                                                <asp:dropdownlist id="ddl_periodo_ipa" runat="server" CssClass="testo" Width="100px" autopostback="True">
                                                                </asp:dropdownlist>
                                                            </td>
                                                        </tr>
                                                        </table>
                                                        <asp:Panel ID="pnl_periodo_ipa" runat="server" Visible="false">
                                                            <div class="ipa_periodo">
                                                                <ul>
                                                                   <li><span class="grigio">Ampiezza</span></li>
                                                               </ul>
                                                           </div>
                                                           <table width="100%" cellpadding="0" cellspacing="0">   
                                                            <tr>
                                                                <td class="testo_grigio_scuro" width="35px;" style="padding-left:20px;padding-top:5px;">Tipo</td>
                                                                <td class="testo_grigio_scuro" style="padding-top:5px;width:105px;"> 
                                                                    <asp:dropdownlist id="ddl_ampiezza_ipa" runat="server" CssClass="testo" Width="100px" autopostback="True">
                                                                    </asp:dropdownlist>
                                                               </td>
                                                                 <td class="testo_grigio_scuro" width="35px;" style="padding-right:10px;padding-top:5px;"><asp:Label ID="lbl_numero" Visible="false" runat="server" Text="Numero"></asp:Label>&nbsp;</td>
                                                                 <td class="testo_grigio_scuro" style="padding-top:5px;"> 
                                                                    <asp:TextBox ID="txt_numeroUnita" runat="server" CssClass="testo" Width="40px" Visible="false" AutoPostBack="true"></asp:TextBox>&nbsp;
                                                                 </td>
                                                            </tr>
                                                          </table>
                                                          <asp:Panel ID="pnl_risultati_iap" CssClass="testo_grigio_scuro" Visible="false" runat="server">
                                                              <div class="risultati_logici">
                                                                    <asp:label id="lbl_quanto_iap" runat="server"></asp:label>
                                                              </div>
                                                          </asp:Panel>

                                                        </asp:Panel>
                                                    </asp:Panel>

                                                    <asp:Panel ID="pnl_iagg" GroupingText="Intervalli di aggregazione (IAGG)" runat="server" CssClass="box_tempo" Visible="false">
                                                        <table width="100%" cellpadding="0" cellspacing="0">   
                                                        <tr>
                                                            <td class="testo_grigio_scuro" width="35px;" style="padding-left:20px;padding-top:5px;">Tipo</td>
                                                            <td class="testo_grigio_scuro" style="padding-top:5px;"> 
                                                                <asp:dropdownlist id="ddl_iagg" runat="server" CssClass="testo" Width="100px" autopostback="True">
                                                                </asp:dropdownlist>
                                                            </td>
                                                        </tr>
                                                        </table>
                                                        <asp:Panel ID="pnl_risultati_iagg" CssClass="testo_grigio_scuro" Visible="false" runat="server">
                                                              <div class="risultati_logici">
                                                                    <asp:label id="lbl_quanto_iag" runat="server"></asp:label>
                                                              </div>
                                                          </asp:Panel>
                                                    </asp:Panel>

                                                    <asp:Panel ID="pnl_finale"  GroupingText="Descrizione esportazione log" runat="server" CssClass="box_tempo" Visible="false">
                                                      <table width="100%" cellpadding="0" cellspacing="0">   
                                                        <tr>
                                                            <td class="testo_finale" style="padding-left:20px;padding-top:5px;">
                                                            <asp:Label ID="lbl_finale" runat="server"></asp:Label>
                                                            </td>
                                                        </tr>
                                                      </table>
                                                    </asp:Panel>
                                              
                                            </td>
                                        </tr>
                                      </table> 
                                      <div id="button_find">
                                          <fieldset>
                                             <asp:button id="btn_stampa" runat="server" CssClass="testo_btn" Text="Stampa" Enabled="false" OnClientClick="showWait();"></asp:button>
                                          </fieldset>
                                       </div>                          
                                      </td>
                                   </tr>
                                    </table>
                                </td>
                            </tr>
						</table>
						<!-- FINE CORPO CENTRALE -->
					</td>				
				</tr>
			</table>
                                    <!-- PopUp Wait-->
                                    <div id="please_wait" style="display: none; border-right: #000000 2px solid; border-top: #000000 2px solid;
                                        border-left: #000000 2px solid; border-bottom: #000000 2px solid; position: absolute;
                                        background-color: #d9d9d9;">
                                        <table cellspacing="0" cellpadding="0" width="350px" border="0">
                                            <tr>
                                                <td valign="middle" style="font-weight: bold; font-size: 12pt; font-family: Verdana"
                                                    align="center" width="350px" height="90px">
                                                    Attendere, prego...
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    
            <iframe id="iframeVisUnificata" style="width:0px; height:0px;" scrolling="auto" frameborder="0" runat="server" visible="true"></iframe>  
		</form>
	</body>
</HTML>
