<%@ Control Language="c#" AutoEventWireup="false" Codebehind="MenuTendina.ascx.cs" Inherits="Amministrazione.UserControl.MenuTendina" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<!--
    NOTA:
    questi campi nascosti indicano le voci di menù presenti nella tendina.
    
    Inserire tanti campi nascosti quanti sono le voci del menù che vanno inserite nella tendina.
    
    Il valore del campo nascosto dovrà essere:
        - "1" DEFAULT ! indica se la voce di menù dove essere abilitata (cioè visualizzata)...
		- "0" altrimenti.
		
	Questa gestione permette di visualizzare o meno le voci di menù agli utenti amministratori
	di tipo USER ADMIN
-->
<input type="hidden" name="hd_abilita_gestione_log" id="hd_abilita_gestione_log" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_log_amm" id="hd_abilita_gestione_log_amm" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_grafica" id="hd_abilita_gestione_grafica" runat="server" value="1" />
<input type="hidden" name="hd_abilita_sblocca_doc" id="hd_abilita_sblocca_doc" runat="server" value="1" />
<input type="hidden" name="hd_abilita_tipi_doc" id="hd_abilita_tipi_doc" runat="server" value="1" />
<input type="hidden" name="hd_abilita_tipi_fasc" id="hd_abilita_tipi_fasc" runat="server" value="1" />
<input type="hidden" name="hd_abilita_diagrammi_stato" id="hd_abilita_diagrammi_stato" runat="server" value="1" />
<input type="hidden" name="hd_abilita_liste_distribuzione" id="hd_abilita_liste_distribuzione" runat="server" value="1" />
<input type="hidden" name="hd_abilita_modelli_trasm" id="hd_abilita_modelli_trasm" runat="server" value="1" />
<input type="hidden" name="hd_abilita_formati_documenti" id="hd_abilita_formati_documenti" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_rf" id="hd_abilita_gestione_rf" runat="server" value="1" />
<input type="hidden" name="hd_import_oggettario" id="hd_import_oggettario" runat="server" value="1" />
<input type="hidden" name="hd_mezzo_spedizione" id="hd_mezzo_spedizione" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_news" id="hd_abilita_gestione_news" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_chiaviConfigurazione" id="hd_abilita_gestione_chiaviConfigurazione" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_deleghe" id="hd_abilita_gestione_deleghe" runat="server" value="1" />
<input type="hidden" name="hd_abilita_verifica_doc" id="hd_abilita_verifica_doc" runat="server" value="1" />
<input type="hidden" name="hd_gestione_password" id="hd_gestione_password" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_cache" id="hd_abilita_gestione_cache" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_qualifiche" id="hd_abilita_gestione_qualifiche" runat="server" value="1" />
<input type="hidden" name="hd_abilita_gestione_importazione" id="hd_abilita_gestione_importazione" runat="server" value="1" />
<input type="hidden" name="hd_import_pregressi" id="hd_import_pregressi" runat="server" value="1" />
<input type="hidden" name="hd_gestione_asserzioni" id="hd_gestione_asserzioni" runat="server" value="1" />
<input type="hidden" name="hd_info_documento" id="hd_info_documento" runat="server" value="1" />
<input type="hidden" name="hd_sblocca_doc_stato_finale" id="hd_sblocca_doc_stato_finale" runat="server" value="1" />
<input type="hidden" name="hd_gestione_pregressi" id="hd_gestione_pregressi" runat="server" value="1" />
<!--
	NOTA 2:	
	questi campi nascosti servono a visualizzare alcune voci del menù a tendina.
	
	Inserire tanti campi nascosti quanti sono le voci del menù a tendina che necessitano della gestione 
	della visualizzazione tramite "key" sul web.config.		
	
	Il valore del campo nascosto dovrà essere:	
		- "1" se la voce di menù dove essere visualizzata...
		- "0" altrimenti.
-->
<input type="hidden" name="hd_prof_dinamic" id="hd_prof_dinamic" runat="server" />
<input type="hidden" name="hd_prof_dinamicFasc" id="hd_prof_dinamicFasc" runat="server" />
<input type="hidden" name="hd_liste_distr" id="hd_liste_distr" runat="server" /> 
<input type="hidden" name="hd_diagra_stato" id="hd_diagra_stato" runat="server" /> 
<input type="hidden" name="hd_formati_documenti" id="hd_formati_documenti" runat="server" /> 
<input type="hidden" name="hd_gestione_rf" id="hd_gestione_rf" runat="server" />
<input type="hidden" name="hd_gestione_rubrica_comune" id="hd_gestione_rubrica_comune" runat="server"/>
<input type="hidden" name="hd_gestione_news" id="hd_gestione_news" runat="server" />
<input type="hidden" name="hd_gestione_Docum_Stato_Finale" id="hd_gestione_Docum_Stato_Finale" runat="server" />
<input type="hidden" name="hd_gestione_indisponibilita" id="hd_gestione_indisponibilita" runat="server"  />
<input type="hidden" name="hd_gestione_struttura_sottofascicoli" id="hd_gestione_struttura_sottofascicoli" runat="server" value="1" />

<!-- Autenticazione Sistemi Esterni R.1 -->
<input type="hidden" name="hd_gestione_sistemi_esterni" id="hd_gestione_sistemi_esterni" runat="server" value="1"/>


