<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format" xmlns:ns0="http://www.infotn.it/LIQ/FlussoDocumentiInfotn">
	<xsl:decimal-format name="european" decimal-separator="," grouping-separator="." NaN="0,00"/>
	<xsl:variable name="color">#0000C0</xsl:variable>
	<xsl:template match="/">
		<html>
			<xsl:call-template name="HTMLPageHeader"/>
			<body bgcolor="#FFFFFF">
				<center>
					<div>
						<tr>
							<!-- <img src="logo_PAT0.png" width="38" height="59" alt="Logo" border="0"/>-->
							<img src="../xml/liquidazione/logo_PAT0.png" alt="Logo" border="0"/>
						</tr>
						<br/>
						<!-- FINE TITOLO -->
						<xsl:variable name="funzCode" select=".//ns0:Estremi_Titolo/ns0:Funz_Codice[1]"/>
						<xsl:choose>
							<xsl:when test="$funzCode='LQ'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:Liquidazione"/>
							</xsl:when>
							<xsl:when test="$funzCode='RS'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:Richiesta_Storno"/>
							</xsl:when>
							<xsl:when test="$funzCode='ST' or $funzCode='SR' or $funzCode='SN'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:Storno">
									<xsl:with-param name="funzCode" select="$funzCode"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$funzCode='RL'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:Riduzione_Liquidazione"/>
							</xsl:when>
							<xsl:when test="$funzCode='NR'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:NotaRettifica"/>
							</xsl:when>
							<xsl:when test="$funzCode='IN'">
								<xsl:apply-templates select=".//ns0:Documenti/ns0:Integrazione"/>
							</xsl:when>
						</xsl:choose>
					</div>
				</center>
			</body>
		</html>
	</xsl:template>
	<xsl:template name="HTMLPageHeader">
		<head>
			<title>Liquidazione</title>
			<style Type="text/css">
					body { font-family:Tahoma, Arial, sans-serif;
		 	 		font_size:8pt; font-weight:bold;
		 			line-height:140%}
				.breakhere {page-break-after: always}
				.intestazione {font-family: Times New Roman, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 12pt; color: #000000; text-align: center}
				.titolo {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:10pt; font-weight:normal; color: #000000; text-align: center}
				.titolo1 {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:10pt; font-weight:bold; color: #000000; text-align: center}
 				.testo {font-family: CG Times, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 9pt; text-align: justify}
 				.testo_sx{font-family: CG Times, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 9pt; text-align: left;}
 				.testo_bold {font-family: CG Times, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 9pt;font-weight:bold; text-align: justify;}
				.totale {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:10pt; font-weight:bold; color: #000000; text-align: left}
				.totale_poste_sx {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:9pt; font-weight:normal; color: #000000; text-align: left}
				.totale_poste_sx_bold {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:9pt; font-weight:bold; color: #000000; text-align: left}
				.totale1 {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:10pt; font-weight:bold; color: #000000; text-align: right}
 				.intest_anagrafica {font-family: Verdana, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 9pt; text-align: center}
 				.anagrafica {font-family: Verdana, Verdana, Arial, Helvetica, sans-serif;
 					font-size: 9pt; text-align: center; font-weight:bold; font-style:italic}
				.dest_diverso {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:8pt; font-weight:bold; color: #000000; text-align: left}
 				.ritenuta {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:9pt; font-weight:normal; color: #000000; text-align: left;
					 text-decoration: underline}
 				.colore_sfondo {background: #000000 }
  				.oggetto {font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
					font-size:8pt; font-weight:bold; color: #000000; text-align: left; font-style:italic}
				
					#tableDatiPosta {
						font-family: Verdana, Tahoma, Arial, Helvetica, sans-serif;
						font-size:9pt;
						width:640px;
						border:1px solid black;
						border-bottom:2px solid black;
						border-top:2px solid black;
					}
					#tableDatiPosta td {
						border-left:1px solid black;
						border-right:1px solid black
					}
					#colDati {
						
					}
					#colImporti {
						width:140px;
						background-color:#A9A9A9;
						font-size: 9pt;
						text-align: right;
					}
					#etichetta_poste {
						font-weight:bold;
						text-align: center;
						background-color:#A9A9A9;
						height:30px;
					}
					#etichetta_poste td {
						border-bottom:2px solid black;
					}
					.totale_poste_dx {
						text-align: right
					}
					.desc_ogg {
						font-weight:bold;
					}
					.num_poste {
						font-weight:bold;
						text-align: center;
					}
					.intestazione_importo {
						border-left:0px solid;
						text-align:center;
					}
					@media print {
						#Stampa {
							display:none;
						}
					}
		</style>
			<SCRIPT LANGUAGE="javascript">
    			function AzioneStampa() {
					print();
    			}
				function Chiudi() {
					window.close();
    			}
			</SCRIPT>
		</head>
	</xsl:template>
	<!-- ///////////////////     INIZIO LIQUIDAZIONE      /////////////////// -->
	<!--Inizio Template che scrive i dati della posta di liquidazione-->
	<xsl:template name="DatiPostaLiquidazione">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500" style="border-right:0px solid;">
			  	  DATI POSTA DI LIQUIDAZIONE
			    </td>
				<td class="intestazione_importo" style="border-left:0px solid;">IMPORTO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="num_poste">
					<xsl:text>Posta di liquidazione n. </xsl:text>
					<xsl:value-of select="ns0:Estremi_Liquidazione/ns0:Estremi_Posta/ns0:Num_Doc"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto, ' 0', '') != ''">
				<tr>
					<td class="totale_poste_sx" colspan="2">
						<br/>
						<xsl:choose>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="AG"'>Atto Gestionale</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="L"'>Legge</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="LP"'>Legge Provinciale</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPGP"'>Decreto Presidente Giunta Pr.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPP"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPE"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DEL"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLB"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DET"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DTD"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLGS"'>Decreto Legislativo</xsl:when>							
						</xsl:choose>
						<xsl:text> </xsl:text>
						<xsl:value-of select="number(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto)"/>
						<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Data_Atto, ' 0', '') != ''">
							<xsl:text> del </xsl:text>
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Provvedimenti_Base/ns0:Imp_Data_Atto"/>
								</xsl:with-param>
							</xsl:call-template>
						</xsl:if>
						<xsl:if test='ns0:Provvedimenti_Base/ns0:Imp_Struttura != ""'>
							<xsl:value-of select="concat(' ( ', ns0:Provvedimenti_Base/ns0:Imp_Struttura, ' )')"/>
						</xsl:if>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<!-- *** sposto il numero di archiviazione dopo "totale posta .." *** -->
			<tr>
				<td width="300" class="totale_poste_sx" style="border-right:0px solid">
					<xsl:text>Impegno n. </xsl:text>
					<font class="num_poste">
						<xsl:value-of select="concat(number(ns0:Impegno/ns0:Imp_Num), ' ', ns0:Impegno/ns0:Imp_Posizione)"/>
					</font>
				</td>
				<td width="200" class="totale_poste_sx" style="border-left:0px solid">
					<xsl:text>Anno </xsl:text>
					<xsl:value-of select="concat(ns0:Impegno/ns0:Fin_Anno, ' ', ns0:Impegno/ns0:Imp_Tipo_Comp)"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="desc_ogg" colspan="2">
					<xsl:text>OGGETTO: </xsl:text>
					<font class="oggetto">
						<xsl:value-of select="ns0:Impegno/ns0:Imp_Oggetto"/>
					</font>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<!-- *** inizio - aggiunta della voce visto inoltre ***-->
			<xsl:if test='ns0:Estremi_Doc_Supporto != ""'>
				<tr>
					<td width="500" class="totale_poste_sx" colspan="2">
						<br/>
						<xsl:text>Visto inoltre</xsl:text>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<!-- *** fine - aggiunta della voce visto inoltre *** -->
			<xsl:for-each select="ns0:Estremi_Doc_Supporto">
				<xsl:apply-templates/>
			</xsl:for-each>
			<xsl:call-template name="Beneficiario">
				<xsl:with-param name="bShowImporto" select="true()"/>
			</xsl:call-template>
			<!-- *** Descr_Fissa in poste *** -->
			<tr>	    
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:text>  </xsl:text>
					<xsl:value-of select="ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<!-- *** fine Descr_Fissa in poste *** -->
			<xsl:if test='ns0:Altro_Beneficiario/ns0:Ben_Cognome_Nome != ""'>
				<xsl:call-template name="Altro_Beneficiario"/>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<br/>
					<xsl:call-template name="Pagamento"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto">
				<tr>
					<td width="500" class="totale_poste_sx" colspan="2">
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test="translate(ns0:Pagamento/ns0:Pag_DataScadenza, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  Scadenza </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_DataScadenza"/>
							</xsl:with-param>
						</xsl:call-template>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Pagabile in </xsl:text>
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> Da versare sul capitolo d'entrata </xsl:text>
						<xsl:value-of select="concat(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr, '/', ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_AnnoCapEntr)"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_FlagCopertura="1"'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Mandato a copertura</xsl:text>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:call-template name="Ritenute"/>
			<tr>
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:text> ANFI:</xsl:text>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="1"'>
						<xsl:text> Si</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="0"'>
						<xsl:text> No</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> SIOPE:</xsl:text>
						<xsl:value-of select="ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="E"'>
						<xsl:text>Pagamento esente dal bollo</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="I"'>
						<xsl:text>Pagamento non soggetto al bollo per applicazione dell'IVA</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="S"'>
						<xsl:text>Pagamento soggetto al bollo</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<font class="desc_ogg">
						<xsl:text>TOTALE POSTA DI LIQUIDAZIONE n. </xsl:text>
						<xsl:value-of select="ns0:Estremi_Liquidazione/ns0:Estremi_Posta/ns0:Num_Doc"/>
					</font>
				</td>
				<td>
					<br/>
					<font class="desc_ogg">
						<xsl:call-template name="FormattaEuro">
							<xsl:with-param name="importo">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoLordo"/>
							</xsl:with-param>
						</xsl:call-template>
					</font>
				</td>
			</tr>
			<xsl:call-template name="addEmptyRow"/>
			<!-- *** aggiungo numero di archiviazione *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="num_poste">
							<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<!--Fine Template che scrive i dati della posta di liquidazione-->
	<xsl:key name="cod_Rit" match="ns0:Lista_Codici_Ritenute/ns0:Codici_Ritenute" use="ns0:Rit_tipo"/>
	<!-- Inizio Template Ritenute -->
	<xsl:template name="Ritenute">
		<tr>
			<td width="500" class="desc_ogg" colspan="2">
				<br/>
				<br/>
				<xsl:text>Ritenute applicate: </xsl:text>
				<xsl:if test='ns0:Beneficiario_Poste/ns0:Rit_Applicate="0"'>
					<xsl:text>No</xsl:text>
				</xsl:if>
				<xsl:if test='ns0:Beneficiario_Poste/ns0:Rit_Applicate="1"'>
					<xsl:text>Si</xsl:text>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
			<br/>
			<br/>
		</tr>
		<xsl:for-each select="ns0:Lista_Codici_Ritenute/ns0:Codici_Ritenute[ns0:Rit_tipo='10']">
			<tr>
				<td width="500" colspan="2">
					<xsl:text> </xsl:text>
					<font class="ritenuta">
						<xsl:text>Ritenuta</xsl:text>
					</font>
					<font class="desc_ogg">
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Rit_Irpef"/>
						<xsl:text> Cat.Red. </xsl:text>
						<xsl:value-of select="../../ns0:Beneficiario_Poste/ns0:Reddito_IRPEF/ns0:Rit_IrpefCat"/>
					</font>
					<font class="totale_poste_sx">
						<xsl:text> </xsl:text>
						<xsl:value-of select="../../ns0:Beneficiario_Poste/ns0:Reddito_IRPEF/ns0:Rit_IrpefCat_Descr"/>
					</font>
				</td>
				<td>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Rit_ImportoRitenute"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td width="300" class="totale_poste_sx" style="border-right:0px solid">
					<xsl:text> Base Imponibile</xsl:text>
				</td>
				<td width="200" class="totale_poste_dx" style="border-left:0px solid">
					<xsl:text/>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Rit_IrpefImpBase"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="300" class="totale_poste_sx" style="border-right:0px solid">
					<xsl:text> Importo esente</xsl:text>
				</td>
				<td width="200" class="totale_poste_dx" style="border-left:0px solid">
					<xsl:text/>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Rit_IrpefImpEsente"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
		</xsl:for-each>
		<xsl:for-each select="ns0:Lista_Codici_Ritenute/ns0:Codici_Ritenute[ns0:Rit_tipo!='10']">
			<tr>
				<td width="500" colspan="2">
					<xsl:text> </xsl:text>
					<font class="ritenuta">
						<xsl:text>Ritenuta</xsl:text>
					</font>
					<font class="desc_ogg">
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Rit_Irpef"/>
					</font>
					<font class="totale_poste_sx">
						<xsl:text> </xsl:text>
						<xsl:value-of select="ns0:Rit_Irpef_Descr"/>
					</font>
				</td>
				<td>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Rit_ImportoRitenute"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
		</xsl:for-each>
	</xsl:template>
	<!-- Fine Template Ritenute -->
	<!-- Inizio Template visto -->
	<xsl:template match="ns0:Estremi_Evidenza">
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:value-of select="ns0:Ev_Ordinamento"/>
				<xsl:text>. </xsl:text>
				<xsl:value-of select="ns0:Evidenza"/>
				<xsl:if test='ns0:Ev_Numero != ""'>
					<xsl:text> n. </xsl:text>
					<xsl:value-of select="ns0:Ev_Numero"/>
				</xsl:if>
				<xsl:if test="translate(ns0:Ev_Data, ' 0', '') != ''">
					<xsl:text> di data </xsl:text>
					<xsl:call-template name="FormattaData">
						<xsl:with-param name="data">
							<xsl:value-of select="ns0:Ev_Data"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
				<xsl:text> </xsl:text>
				<xsl:if test="ns0:Ev_Note != ''">
					<xsl:value-of select="concat('(', ns0:Ev_Note, ')')"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
	<!-- Fine Template visto -->
	<!-- Inizio Template Pagamento -->
	<xsl:template name="Pagamento">
		<xsl:text>  </xsl:text>
		<xsl:value-of select="ns0:Pagamento/ns0:Pag_Tipologia/ns0:Pag_Descr"/>
		<!-- *** aggiunto if sul numero di conto *** -->
        <xsl:choose>
			<xsl:when test='((count(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Caratteri_Controllo)= "0" or ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Caratteri_Controllo = "") and (count(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Coordinate_Iban)= "0" or ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Coordinate_Iban = ""))'>
			    <xsl:text> </xsl:text> 
				<xsl:if test='(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCB != "" or ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCP != "" or ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCEstero != "")'>
				    <xsl:text>n. </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCB"/>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCP"/>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCEstero"/>
				</xsl:if>
				<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ContoTesoreria"/>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "" and ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "00000"'>
					<xsl:text> ABI </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI"/>
				</xsl:if>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CAB != "" and ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "00000"'>
					<xsl:text> CAB </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CAB"/>
				</xsl:if>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CIN != ""'>
					<xsl:text> CIN </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CIN"/>
				</xsl:if>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CodiceSwift != ""'>
					<xsl:text> Codice Swift </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CodiceSwift"/>
				</xsl:if> 
			</xsl:when>
		    <xsl:otherwise>
				<!-- Visualizzazione nel caso di documenti di Liquidazione (LQ) Nota di Rettifica (NR) e 
		        Riduzione Liquidazione (RL) con Pag_Caratteri_Controllo e Pag_Coordinate_Iban  diversi da null-->
		    	<xsl:text>  </xsl:text>
		    	<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Codice_Paese"/>
		    	<xsl:text>  </xsl:text>
		    	<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Caratteri_Controllo"/>
		    	<xsl:text>  </xsl:text>
		    	<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CIN"/>			
		    	<xsl:text>  </xsl:text> 
		    	<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "" and ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "00000"'>
					<xsl:text>  </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI"/>
				</xsl:if>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CAB != "" and ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ABI != "00000"'>
					<xsl:text>  </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CAB"/>
				</xsl:if>   	
				<xsl:text>  </xsl:text>
				<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCB"/>
				<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCP"/>
				<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_NumCCEstero"/>
				<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ContoTesoreria"/>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Coordinate_Iban != ""'>
					<xsl:text> IBAN: </xsl:text>
					<xsl:value-of select="ns0:agamento/ns0:Pag_Estremi/ns0:Pag_Coordinate_Iban"/>
				</xsl:if>
				<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CodiceSwift != ""'>
					<xsl:text> Codice Swift </xsl:text>
					<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CodiceSwift"/>
				</xsl:if>		    
		    </xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<!-- Fine Template Pagamento -->
	<!-- Inizio Template Beneficiario-->
	<xsl:template name="Beneficiario">
		<xsl:param name="bShowImporto" select="false()"/>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<br/>
				<xsl:text>A: </xsl:text>
				<font class="desc_ogg">
					<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Cognome_Nome"/>
					<xsl:text> (Cod.</xsl:text>
					<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Creditore"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Gruppo_Conti"/>
					<xsl:text>)</xsl:text>
				</font>
			</td>
			<td>
				<br/>
				<xsl:if test="$bShowImporto">
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoNetto"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
			</td>
		</tr>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Indirizzo"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_CAP"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Localita"/>
				<xsl:text> </xsl:text>
				<xsl:if test='ns0:Beneficiario_Poste/ns0:Ben_Paese != ""'>
					<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Paese"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<xsl:if test="translate(ns0:Beneficiario_Poste/ns0:Ben_Luogo_Nascita, ' 0', '') != ''or translate(ns0:Beneficiario_Poste/ns0:Ben_Data_Nascita, ' 0', '') != ''">
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<xsl:text>  Nato </xsl:text>
					<xsl:if test='ns0:Beneficiario_Poste/ns0:Ben_Luogo_Nascita != ""'>
						<xsl:text>a </xsl:text>
						<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Luogo_Nascita"/>
					</xsl:if>
					<xsl:if test="translate(ns0:Beneficiario_Poste/ns0:Ben_Data_Nascita, ' 0', '') != ''">
						<xsl:text> il </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Data_Nascita"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
		</xsl:if>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  C.F. </xsl:text>
				<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_CFPIVA"/>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
	<!-- Fine Template Beneficiario -->
	<!-- Inizio Template Altro Beneficiario-->
	<xsl:template name="Altro_Beneficiario">
		<tr>
			<td width="500" class="dest_diverso" colspan="2">
				<br/>
				<xsl:text>Destinatario pagamento diverso:  </xsl:text>
				<br/>
				<font class="desc_ogg">
					<xsl:text>  </xsl:text>
					<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Cognome_Nome"/>
					<xsl:text> (Cod.</xsl:text>
					<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Creditore"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Gruppo_Conti"/>
					<xsl:text>)</xsl:text>
				</font>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Indirizzo"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_CAP"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Localita"/>
				<xsl:text> </xsl:text>
				<xsl:if test='ns0:Altro_Beneficiario/ns0:Ben_Paese != ""'>
					<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Paese"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<xsl:if test="translate(ns0:Altro_Beneficiario/ns0:Ben_Luogo_Nascita, ' 0', '') != ''or translate(ns0:Altro_Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<xsl:text>  Nato </xsl:text>
					<xsl:if test='ns0:Altro_Beneficiario/ns0:Ben_Luogo_Nascita != ""'>
						<xsl:text>a </xsl:text>
						<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Luogo_Nascita"/>
					</xsl:if>
					<xsl:if test="translate(ns0:Altro_Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
						<xsl:text> il </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_Data_Nascita"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
		</xsl:if>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  C.F. </xsl:text>
				<xsl:value-of select="ns0:Altro_Beneficiario/ns0:Ben_CFPIVA"/>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
	<!-- Fine Template Altro Beneficiario -->
	<xsl:template name="FormattaData">
		<xsl:param name="data"/>
		<xsl:param name="formatMask">DD/MM/YYYY</xsl:param>
		<xsl:if test="$data != ''">
			<xsl:choose>
				<xsl:when test="not(contains($data, '/'))">
					<xsl:choose>
						<xsl:when test="contains($data, '-')">
							<xsl:value-of select="substring($data, 9, 2)"/>
							<xsl:text>/</xsl:text>
							<xsl:value-of select="substring($data, 6, 2)"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="substring($data, 7, 2)"/>
							<xsl:text>/</xsl:text>
							<xsl:value-of select="substring($data, 5, 2)"/>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>/</xsl:text>
					<xsl:value-of select="substring($data, 1, 4)"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$data"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<xsl:template match="ns0:Documenti/ns0:Riduzione_Liquidazione" name="startRiduzioneLiquidazione">
		<table>
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Testata/ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
		</table>
		<!-- INIZIO TITOLO -->
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo">
					<xsl:value-of select="concat(ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO -->
		<br/>
		<!-- INIZIO TITOLO1 -->
		<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo1">
					<xsl:value-of select="concat(ns0:Testata/ns0:Estremi_Titolo/ns0:Funz_Descr, ' N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
					<!-- *** andare a capo dopo numero documento *** -->
					<br/>
					<xsl:text>ALLA LIQUIDAZIONE N. </xsl:text>
					<xsl:value-of select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='01']/ns0:Estremi_Doc/ns0:Num_Doc"/>
					<br/>
					<xsl:text>DI DATA </xsl:text>
					<xsl:call-template name="FormattaData">
						<xsl:with-param name="data" select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='01']/ns0:Estremi_Doc/ns0:Data_Doc"/>
					</xsl:call-template>
					<br/>
					<xsl:text>CAPITOLO </xsl:text>
					<xsl:value-of select="ns0:Posta/ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO1 -->
		<br/>
		<!-- INIZIO TESTO -->
		<table>
			<tr>
				<td width="640" class="testo">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
			</tr>
		</table>
		<!-- FINE TESTO -->
		<br/>
		<!-- INIZIO TOTRIDUZIONELIQUIDAZIONE -->
		<table>
			<tr>
				<td width="440" class="totale">
					<xsl:value-of select="concat('TOTALE ', ns0:Testata/ns0:Estremi_Titolo/ns0:Funz_Descr, ' N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
				</td>
				<td width="200" class="totale1">
					<xsl:text>EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo" select="-ns0:Posta/ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoRiduzione"/>
					</xsl:call-template>
				</td>
			</tr>
		</table>
		<!-- FINE TOTRIDUZIONELIQUIDAZIONE -->
		<br/>
		<!-- INIZIO ANAGRAFICA -->
		<table>
			<tr>
				<td width="214" class="intest_anagrafica">
					<xsl:text>Indicazione dell'Estensore</xsl:text>
				</td>
				<td width="300" class="intest_anagrafica">
					<xsl:text>Funzionario responsabile proposto</xsl:text>
				</td>
				<td width="126"/>
			</tr>
			<tr>
				<td class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Estensore"/>
				</td>
				<td class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Funzionario"/>
				</td>
				<td class="anagrafica"/>
			</tr>
		</table>
		<!-- FINE ANAGRAFICA -->
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
		<br/>
		<table>
			<tr>
				<td width="320" class="totale_poste_sx_bold">
					<xsl:text>Poste di liquidazione oggetto di riduzione:</xsl:text>
				</td>
				<td width="320" class="totale_poste_sx_bold">
					<xsl:text>Importo liquidazione originale:</xsl:text>
				</td>
			</tr>
			<tr>
				<td width="320" class="totale_poste_sx">
					<xsl:for-each select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='01']/ns0:Estremi_Posta">
						<div>
							<xsl:text>n. </xsl:text>
							<xsl:value-of select="ns0:Num_Doc"/>
							<xsl:if test="translate(ns0:Data_Doc, ' 0', '') != ''">
								<xsl:text> di data </xsl:text>
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data" select="ns0:Data_Doc"/>
								</xsl:call-template>
							</xsl:if>
						</div>
					</xsl:for-each>
				</td>
				<td width="320" class="totale_poste_sx">
					<xsl:text>N. LIQ. </xsl:text>
					<xsl:value-of select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='01']/ns0:Estremi_Doc/ns0:Num_Doc"/>
					<xsl:text> EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo" select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='01']/ns0:Importo_PosteCollegate"/>
					</xsl:call-template>
				</td>
			</tr>
		</table>
		<br/>
		<!--MOTIVO-->
		<!-- *** solo se valorizzato *** -->
		<xsl:if test='ns0:Testata/ns0:Testo_richiesta != ""'>
			<table>
				<tr>
					<td width="640" class="testo">
						<span class="desc_ogg">Motivo: </span>
						<span>
							<xsl:value-of select="ns0:Testata/ns0:Testo_richiesta"/>
						</span>
					</td>
				</tr>
			</table>
		</xsl:if>
		<!--MOTIVO-->
		<br/>
		<!--DATA-->
		<table>
			<tr>
				<td width="640" class="testo_sx">
					<font class="testo_bold">
						Data:
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data" select="ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
						</xsl:call-template>
					</font>
				</td>
			</tr>
		</table>
		<!--DATA-->
		<!-- 	***********************************************************************************	-->
		<!-- INIZIO DATI LIQUIDAZIONE -->
		<xsl:for-each select="ns0:Posta">
			<xsl:call-template name="DatiPostaRiduzioneLiquidazione"/>
		</xsl:for-each>
		<!-- FINE DATI LIQUIDAZIONE -->
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<xsl:template name="DatiPostaRiduzioneLiquidazione">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500" style="border-right:0px solid;">
			  	  DATI DI RIDUZIONE DELLA LIQUIDAZIONE
			    </td>
				<td class="intestazione_importo" style="border-left:0px solid;">IMPORTO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="num_poste">
					<xsl:text>Posta di liquidazione di riduzione n. </xsl:text>
					<xsl:value-of select="ns0:Estremi_Liquidazione/ns0:Estremi_Posta/ns0:Num_Doc"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto, ' 0', '') != ''">
				<tr>
					<td class="totale_poste_sx" colspan="2">
						<br/>
						<xsl:choose>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="AG"'>Atto Gestionale</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="L"'>Legge</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="LP"'>Legge Provinciale</xsl:when>							
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPGP"'>Decreto Presidente Giunta Pr.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPP"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPE"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DEL"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLB"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DET"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DTD"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLGS"'>Decreto Legislativo</xsl:when>						
						</xsl:choose>
						<xsl:text> </xsl:text>
						<xsl:value-of select="number(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto)"/>
						<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Data_Atto, ' 0', '') != ''">
							<xsl:text> del </xsl:text>
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Provvedimenti_Base/ns0:Imp_Data_Atto"/>
								</xsl:with-param>
							</xsl:call-template>
						</xsl:if>
						<xsl:if test="ns0:Provvedimenti_Base/ns0:Imp_Struttura !=''">
							<xsl:value-of select="concat(' (', ns0:Provvedimenti_Base/ns0:Imp_Struttura, ')')"/>
						</xsl:if>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<!-- *** sposto il numero di archiviazione dopo "totale posta .." *** -->
			<tr>
				<td width="300" class="totale_poste_sx" style="border-right:0px solid">
					<xsl:text>Impegno n. </xsl:text>
					<font class="num_poste">
						<xsl:value-of select="concat(number(ns0:Impegno/ns0:Imp_Num), ' ', ns0:Impegno/ns0:Imp_Posizione)"/>
					</font>
				</td>
				<td width="200" class="totale_poste_sx" style="border-left:0px solid">
					<xsl:text>Anno </xsl:text>
					<xsl:value-of select="concat(ns0:Impegno/ns0:Fin_Anno, ' ', ns0:Impegno/ns0:Imp_Tipo_Comp)"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="desc_ogg" colspan="2">
					<xsl:text>OGGETTO: </xsl:text>
					<font class="oggetto">
						<xsl:value-of select="ns0:Impegno/ns0:Imp_Oggetto"/>
					</font>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:call-template name="Beneficiario"/>
			<tr>
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:value-of select="ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test='ns0:Altro_Beneficiario/ns0:Ben_Cognome_Nome != ""'>
				<xsl:call-template name="Altro_Beneficiario"/>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<br/>
					<xsl:call-template name="Pagamento"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto != ''">
				<tr>
					<td width="500" class="totale_poste_sx" colspan="2">
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test="translate(ns0:Pagamento/ns0:Pag_DataScadenza, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  Scadenza </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_DataScadenza"/>
							</xsl:with-param>
						</xsl:call-template>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Pagabile in </xsl:text>
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> Da versare sul capitolo d'entrata </xsl:text>
						<xsl:value-of select="concat(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr, '/', ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_AnnoCapEntr)"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_FlagCopertura="1"'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Mandato a copertura</xsl:text>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:call-template name="Ritenute"/>
			<tr>
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:text> ANFI:</xsl:text>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="1"'>
						<xsl:text> Si</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="0"'>
						<xsl:text> No</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> SIOPE:</xsl:text>
						<xsl:value-of select="ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="E"'>
						<xsl:text>Pagamento esente dal bollo</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="I"'>
						<xsl:text>Pagamento non soggetto al bollo per applicazione dell'IVA</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="S"'>
						<xsl:text>Pagamento soggetto al bollo</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<font class="desc_ogg">
						<xsl:text>TOTALE IMPORTO ORIGINALE</xsl:text>
					</font>
				</td>
				<td>
					<br/>
					<font class="desc_ogg">
						<xsl:call-template name="FormattaEuro">
							<xsl:with-param name="importo">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoOriginale"/>
							</xsl:with-param>
						</xsl:call-template>
					</font>
				</td>
			</tr>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<font class="desc_ogg">
						<xsl:text>TOTALE IMPORTO RIDUZIONE</xsl:text>
					</font>
				</td>
				<td>
					<br/>
					<font class="desc_ogg">
						<xsl:call-template name="FormattaEuro">
							<xsl:with-param name="importo">
								<xsl:value-of select="-ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoRiduzione"/>
							</xsl:with-param>
						</xsl:call-template>
					</font>
				</td>
			</tr>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<font class="desc_ogg">
						<xsl:text>TOTALE IMPORTO RIDOTTO</xsl:text>
					</font>
				</td>
				<td>
					<br/>
					<font class="desc_ogg">
						<xsl:call-template name="FormattaEuro">
							<xsl:with-param name="importo">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoLordo"/>
							</xsl:with-param>
						</xsl:call-template>
					</font>
				</td>
			</tr>
			<xsl:call-template name="addEmptyRow"/>
			<!-- *** aggiungo numero di archiviazione *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="num_poste">
							<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<xsl:template match="ns0:Documenti/ns0:NotaRettifica" name="startNotaRettifica">
		<table>
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Testata/ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
		</table>
		<!-- INIZIO TITOLO -->
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo">
					<xsl:value-of select="concat(ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO -->
		<br/>
		<!-- INIZIO TITOLO1 -->
		<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo1">
					<xsl:value-of select="concat(ns0:Testata/ns0:Estremi_Titolo/ns0:Funz_Descr, ' N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
					<!-- *** andare a capo dopo numero documento *** -->
					<br/>
					<xsl:choose>
						<xsl:when test="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Funz_Codice = 'LQ'">
							<xsl:text>ALLA LIQUIDAZIONE N. </xsl:text>
						</xsl:when>
						<xsl:when test="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Funz_Codice = 'NR'">
							<xsl:text>ALLA NOTA DI RETTIFICA N. </xsl:text>
						</xsl:when>
					</xsl:choose>
					<xsl:value-of select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Estremi_Doc/ns0:Num_Doc"/>
					<br/>
					<xsl:text>DI DATA </xsl:text>
					<xsl:call-template name="FormattaData">
						<xsl:with-param name="data" select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Estremi_Doc/ns0:Data_Doc"/>
					</xsl:call-template>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO1 -->
		<br/>
		<!-- INIZIO TESTO -->
		<table>
			<tr>
				<td width="640" class="testo">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
			</tr>
		</table>
		<!-- FINE TESTO -->
		<br/>
		<!-- INIZIO TOTNOTARETTIFICA -->
		<table>
			<tr>
				<td width="440" class="totale">
					<xsl:value-of select="concat('TOTALE ', ns0:Testata/ns0:Estremi_Titolo/ns0:Funz_Descr, ' N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
				</td>
				<td width="200" class="totale1">
					<xsl:text>EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo" select="ns0:Testata/ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td class="totale_poste_sx">
					<xsl:value-of select="concat('(Segue elenco con n. ', ns0:Testata/ns0:Riepilogo_Poste/ns0:Num_Poste, ' poste di liquidazione)')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TOTNOTARETTIFICA -->
		<br/>
		<!-- INIZIO ANAGRAFICA -->
		<table>
			<tr>
				<td width="214" class="intest_anagrafica">
					<xsl:text>Indicazione dell'Estensore</xsl:text>
				</td>
				<td width="300" class="intest_anagrafica">
					<xsl:text>Funzionario responsabile proposto</xsl:text>
				</td>
				<td width="126"/>
			</tr>
			<tr>
				<td class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Estensore"/>
				</td>
				<td class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Funzionario"/>
				</td>
				<td class="anagrafica"/>
			</tr>
		</table>
		<!-- FINE ANAGRAFICA -->
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
		<br/>
		<table>
			<tr>
				<td width="640" class="totale_poste_sx_bold">
					<xsl:text>Poste di liquidazione oggetto di rettifica:</xsl:text>
				</td>
			</tr>
			<tr>
				<td width="640" class="totale_poste_sx">
					<xsl:for-each select="ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Estremi_Posta">
						<div>
							<xsl:text>n. </xsl:text>
							<xsl:value-of select="ns0:Num_Doc"/>
							<xsl:if test="translate(ns0:Data_Doc, ' 0', '') != ''">
								<xsl:text> di data </xsl:text>
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data" select="ns0:Data_Doc"/>
								</xsl:call-template>
							</xsl:if>
						</div>
					</xsl:for-each>
				</td>
			</tr>
		</table>
		<br/>
		<!--MOTIVO-->
		<!-- *** solo se valorizzato *** -->
		<xsl:if test='ns0:Testata/ns0:Testo_richiesta != ""'>
			<table>
				<tr>
					<td width="640" class="testo">
						<span class="desc_ogg">Motivo: </span>
						<span>
							<xsl:value-of select="ns0:Testata/ns0:Testo_richiesta"/>
						</span>
					</td>
				</tr>
			</table>
		</xsl:if>
		<!--MOTIVO-->
		<br/>
		<xsl:if test="ns0:Testata/ns0:Estremi_Doc_Riferimento != ''">
			<table width="640" cellspacing="0" cellpadding="2">
				<tr height="40">
					<td width="200" class="testo">
						<span class="desc_ogg">Dati oggetto di rettifica:</span>
					</td>
					<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Fin_Capitolo, ' 0', '') != ''">
						<td width="440" class="testo">
							<span class="desc_ogg">Capitolo: </span>
							<span>
								<xsl:value-of select="ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Fin_Capitolo"/>
							</span>
						</td>
					</xsl:if>
				</tr>
				<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Num, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">Impegno n.: </span>
							<span>
								<xsl:value-of select="concat(number(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Num), ' - ', ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Posizione)"/>
							</span>
						</td>
					</tr>
				</xsl:if>
				<xsl:variable name="finAnno" select="ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Fin_Anno"/>
				<xsl:if test="translate($finAnno, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">Anno: </span>
							<span>
								<xsl:value-of select="$finAnno"/>
							</span>
							<span class="desc_ogg">
								<xsl:choose>
									<xsl:when test="$finAnno = ns0:Testata/ns0:Documenti_Collegati/ns0:Elenco_Documenti[ns0:Tipo_Estremo='02']/ns0:Estremi_Doc/ns0:Anno_Doc"> Competenza</xsl:when>
									<xsl:otherwise> Residuo</xsl:otherwise>
								</xsl:choose>
							</span>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Num_Atto, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">
								<xsl:choose>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="AG"'>Atto Gestionale</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="L"'>Legge</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="LP"'>Legge Provinciale</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DPGP"'>Decreto Presidente Giunta Pr.</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DPP"'>Decreto urg.</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DPE"'>Decreto urg.</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DEL"'>Delibera</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DLB"'>Delibera</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DET"'>Determinazione</xsl:when>
									<xsl:when test='ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Tipo="DTD"'>Determinazione</xsl:when>
									<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLGS"'>Decreto Legislativo</xsl:when>									
								</xsl:choose>
							</span>
							<span>
								<xsl:value-of select="number(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Num_Atto)"/>
								<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Data_Atto, ' 0', '') != ''">
									<xsl:text> di data </xsl:text>
									<xsl:call-template name="FormattaData">
										<xsl:with-param name="data" select="ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Data_Atto"/>
									</xsl:call-template>
								</xsl:if>
								<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Struttura, ' 0', '') != ''">
									<xsl:value-of select="concat('(', ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Imp_Struttura, ')')"/>
								</xsl:if>
							</span>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text1_mnd, ' 0', '') != '' or translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text2_mnd, ' 0', '') != '' or translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text3_mnd, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">Oggetto: </span>
							<span>
								<xsl:value-of select="concat(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text1_mnd, ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text2_mnd, ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Text3_mnd)"/>
							</span>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_CapitoloEntr, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">Capitolo di entrata: </span>
							<span>
								<xsl:value-of select="concat(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_CapitoloEntr, '/', ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_AnnoCapEntr)"/>
							</span>
						</td>
					</tr>
				</xsl:if>
				<xsl:if test="translate(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_Accentr, ' 0', '') != ''">
					<tr height="40">
						<td width="200"/>
						<td width="440" class="testo">
							<span class="desc_ogg">Accertamento n.: </span>
							<span>
								<xsl:value-of select="concat(ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_Accentr, ' - ', ns0:Testata/ns0:Estremi_Doc_Riferimento/ns0:Pag_PosAccentr)"/>
							</span>
						</td>
					</tr>
				</xsl:if>
			</table>
		</xsl:if>
		<table>
			<tr>
				<td width="640" class="testo_sx">
					<font class="testo_bold">
						Data:
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data" select="ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
						</xsl:call-template>
					</font>
				</td>
			</tr>
		</table>
		<!-- 	***********************************************************************************	-->
		<!-- INIZIO DATI LIQUIDAZIONE -->
		<xsl:for-each select="ns0:Posta">
			<xsl:call-template name="DatiPostaNotaRettifica"/>
		</xsl:for-each>
		<!-- FINE DATI LIQUIDAZIONE -->
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<xsl:template name="DatiPostaNotaRettifica">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500" style="border-right:0px solid;">
			  	  DATI RETTIFICATIVI DELLA LIQUIDAZIONE
			    </td>
				<td class="intestazione_importo" style="border-left:0px solid;">IMPORTO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="num_poste">
					<xsl:text>Posta di liquidazione rettificativa n. </xsl:text>
					<xsl:value-of select="ns0:Estremi_Liquidazione/ns0:Estremi_Posta/ns0:Num_Doc"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto, ' 0', '') != ''">
				<tr>
					<td class="totale_poste_sx" colspan="2">
						<br/>
						<xsl:choose>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="AG"'>Atto Gestionale</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="L"'>Legge</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="LP"'>Legge Provinciale</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPGP"'>Decreto Presidente Giunta Pr.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPP"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DPE"'>Decreto urg.</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DEL"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLB"'>Delibera</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DET"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DTD"'>Determinazione</xsl:when>
							<xsl:when test='ns0:Provvedimenti_Base/ns0:Imp_Tipo="DLGS"'>Decreto Legislativo</xsl:when>
						</xsl:choose>
						<xsl:text> </xsl:text>
						<xsl:value-of select="number(ns0:Provvedimenti_Base/ns0:Imp_Num_Atto)"/>
						<xsl:if test="translate(ns0:Provvedimenti_Base/ns0:Imp_Data_Atto, ' 0', '') != ''">
							<xsl:text> del </xsl:text>
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Provvedimenti_Base/ns0:Imp_Data_Atto"/>
								</xsl:with-param>
							</xsl:call-template>
						</xsl:if>
						<xsl:if test='ns0:Provvedimenti_Base/ns0:Imp_Struttura != ""'>
							<xsl:value-of select="concat(' (', ns0:Provvedimenti_Base/ns0:Imp_Struttura, ')')"/>
						</xsl:if>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<tr>
				<td width="500" colspan="2">
					<xsl:text>Capitolo </xsl:text>
					<font class="num_poste">
						<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
					</font>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="300" class="totale_poste_sx" style="border-right:0px solid">
					<xsl:text>Impegno n. </xsl:text>
					<font class="num_poste">
						<xsl:value-of select="concat(number(ns0:Impegno/ns0:Imp_Num), ' ', ns0:Impegno/ns0:Imp_Posizione)"/>
					</font>
				</td>
				<td width="200" class="totale_poste_sx" style="border-left:0px solid">
					<xsl:text>Anno </xsl:text>
					<xsl:value-of select="concat(ns0:Impegno/ns0:Fin_Anno, ' ', ns0:Impegno/ns0:Imp_Tipo_Comp)"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="desc_ogg" colspan="2">
					<xsl:text>OGGETTO: </xsl:text>
					<font class="oggetto">
						<xsl:value-of select="ns0:Impegno/ns0:Imp_Oggetto"/>
					</font>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:call-template name="Beneficiario">
				<xsl:with-param name="bShowImporto" select="true()"/>
			</xsl:call-template>
			<tr>
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:value-of select="ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test='ns0:Altro_Beneficiario/ns0:Ben_Cognome_Nome != ""'>
				<xsl:call-template name="Altro_Beneficiario"/>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<br/>
					<xsl:call-template name="Pagamento"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto">
				<tr>
					<td width="500" class="totale_poste_sx" colspan="2">
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Istituto"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test="translate(ns0:Pagamento/ns0:Pag_DataScadenza, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  Scadenza </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_DataScadenza"/>
							</xsl:with-param>
						</xsl:call-template>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Pagabile in </xsl:text>
						<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_DivisaConversione"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> Da versare sul capitolo d'entrata </xsl:text>
						<xsl:value-of select="concat(ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_CapitoloEntr, '/', ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_AnnoCapEntr)"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito != ""'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Descr_Accredito"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:if test='ns0:Pagamento/ns0:Pag_FlagCopertura="1"'>
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<xsl:text>  Mandato a copertura</xsl:text>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<xsl:call-template name="Ritenute"/>
			<tr>
				<td width="500" class="dest_diverso" colspan="2">
					<br/>
					<xsl:text> ANFI:</xsl:text>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="1"'>
						<xsl:text> Si</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Int_Anfi="0"'>
						<xsl:text> No</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<xsl:if test="translate(ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE, ' 0', '') != ''">
				<tr>
					<td width="500" class="dest_diverso" colspan="2">
						<br/>
						<xsl:text> SIOPE:</xsl:text>
						<xsl:value-of select="ns0:Dati_Codice_Siope/ns0:SIOPE/ns0:Codice_SIOPE"/>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="E"'>
						<xsl:text>Pagamento esente dal bollo</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="I"'>
						<xsl:text>Pagamento non soggetto al bollo per applicazione dell'IVA</xsl:text>
					</xsl:if>
					<xsl:if test='ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_Bollo="S"'>
						<xsl:text>Pagamento soggetto al bollo</xsl:text>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<tr>
				<td width="500" class="totale_poste_dx" colspan="2">
					<br/>
					<font class="desc_ogg">
						<xsl:value-of select="concat('TOTALE POSTA DI LIQUIDAZIONE RETTIFICATIVA N. ', ns0:Estremi_Liquidazione/ns0:Estremi_Posta/ns0:Num_Doc)"/>
					</font>
				</td>
				<td>
					<br/>
					<font class="desc_ogg">
						<xsl:call-template name="FormattaEuro">
							<xsl:with-param name="importo">
								<xsl:value-of select="ns0:Pagamento/ns0:Pag_Estremi/ns0:Pag_ImportoLordo"/>
							</xsl:with-param>
						</xsl:call-template>
					</font>
				</td>
			</tr>
			<xsl:call-template name="addEmptyRow"/>
			<!-- *** aggiungo numero di archiviazione *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="num_poste">
							<xsl:value-of select="ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<xsl:template match="ns0:Documenti/ns0:Liquidazione" name="startLiquidazione">
		<table>
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Testata/ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
		</table>
		<!-- INIZIO TITOLO -->
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo">
					<xsl:value-of select="concat(ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Testata/ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO -->
		<br/>
		<!-- INIZIO TITOLO1 -->
		<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo1">
					<xsl:choose>
						<xsl:when test='ns0:Testata/ns0:Dati_DocLIQ/ns0:Tipo_Liquidazione="01"'>
							<xsl:value-of select="concat('LIQUIDAZIONE N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
							<!-- *** andare a capo dopo numero documento *** -->
							<br/>
							<xsl:text> E RICHIESTA EMISSIONE TITOLO DI SPESA</xsl:text>
							<br/>
							<xsl:text>CAPITOLO </xsl:text>
							<xsl:value-of select="ns0:Posta/ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
						</xsl:when>
						<xsl:when test='ns0:Testata/ns0:Dati_DocLIQ/ns0:Tipo_Liquidazione="02"'>
							<xsl:value-of select="concat('DOCUMENTO N. ', ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc)"/>
							<br/>
							<xsl:text> DI INOLTRO LIQUIDAZIONI SPESE DI RAPPRESENTANZA</xsl:text>
							<br/>
							<xsl:text>CAPITOLO </xsl:text>
							<xsl:value-of select="ns0:Posta/ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
						</xsl:when>
					</xsl:choose>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO1 -->
		<br/>
		<!-- INIZIO TESTO -->
		<table>
			<tr>
				<td width="640" class="testo">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Descr_Fissa"/>
				</td>
			</tr>
		</table>
		<!-- FINE TESTO -->
		<br/>
		<!-- INIZIO TOTLIQUIDAZIONE -->
		<table>
			<tr>
				<td width="320" class="totale">
					<xsl:text>TOTALE LIQUIDAZIONE N. </xsl:text>
					<xsl:value-of select="ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
				</td>
				<td width="320" class="totale1">
					<xsl:text>EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Testata/ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td class="totale_poste_sx">
					<xsl:value-of select="concat('(Segue elenco con n. ', ns0:Testata/ns0:Riepilogo_Poste/ns0:Num_Poste, ' poste di liquidazione)')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TOTLIQUIDAZIONE -->
		<br/>
		<!-- INIZIO ANAGRAFICA -->
		<table>
			<tr>
				<td width="180" class="intest_anagrafica">
					<xsl:text>Indicazione dell'Estensore</xsl:text>
				</td>
				<td width="240" class="intest_anagrafica">
					<xsl:text>Funzionario responsabile proposto</xsl:text>
				</td>
				<xsl:if test="ns0:Testata/ns0:Dati_DocLIQ/ns0:Delegato">
				<td width="220" class="intest_anagrafica">Delegato proposto</td>
				</xsl:if>
			</tr>
			<tr>
				<td width="180" class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Estensore"/>
				</td>
				<td width="240" class="anagrafica">
					<xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Funzionario"/>
				</td>
				<td width="220" class="anagrafica">
  				   <xsl:value-of select="ns0:Testata/ns0:Dati_DocLIQ/ns0:Delegato"/>
				</td>
			</tr>
		</table>
		<!-- FINE ANAGRAFICA -->
		<br/>
		<table>
			<tr>
				<td width="640" class="testo_sx">
					Data:
					<font class="testo_bold">
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data" select="ns0:Testata/ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
						</xsl:call-template>
					</font>
				</td>
			</tr>
		</table>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
		<table>
			<br/>
			<!-- *** inizio - Aggiunta della voce "visto:" *** -->
			<xsl:if test='ns0:Testata/ns0:Estremi_Doc_Supporto != ""'>
				<tr>
					<td width="640" class="totale_poste_sx">
						<xsl:text>Visto:</xsl:text>
					</td>
				</tr>
			</xsl:if>
			<!-- *** fine - Aggiunta della voce "visto:" *** -->
			<xsl:for-each select="ns0:Testata/ns0:Estremi_Doc_Supporto/ns0:Estremi_Evidenza">
				<tr>
					<td width="640" class="totale_poste_sx">
						<xsl:value-of select="ns0:Ev_Ordinamento"/>
						<xsl:text>. </xsl:text>
						<xsl:value-of select="ns0:Evidenza"/>
						<xsl:if test='ns0:Ev_Numero != ""'>
							<xsl:text> n. </xsl:text>
							<xsl:value-of select="ns0:Ev_Numero"/>
						</xsl:if>
						<xsl:if test="translate(ns0:Ev_Data, ' 0', '') != ''">
							<xsl:text> di data </xsl:text>
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Ev_Data"/>
								</xsl:with-param>
							</xsl:call-template>
						</xsl:if>
						<xsl:text> </xsl:text>
						<xsl:if test="ns0:Ev_Note != ''">
							<xsl:value-of select="concat('(', ns0:Ev_Note, ')')"/>
						</xsl:if>
					</td>
				</tr>
			</xsl:for-each>
		</table>
		<!-- 	***********************************************************************************	-->
		<!-- INIZIO DATI LIQUIDAZIONE -->
		<xsl:for-each select="ns0:Posta">
			<xsl:call-template name="DatiPostaLiquidazione"/>
		</xsl:for-each>
		<!-- FINE DATI LIQUIDAZIONE -->
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<!-- ///////////////////     FINE LIQUIDAZIONE      /////////////////// -->
	<!-- ///////////////////     INIZIO STORNO      /////////////////// -->
	<!-- INIZIO STORNO TEMPLATE -->
	<xsl:template match="ns0:Documenti/ns0:Storno" name="startStorno">
		<xsl:param name="funzCode"/>
		<xsl:variable name="title">
			<xsl:choose>
				<xsl:when test="$funzCode = 'ST'">LIQUIDAZIONE</xsl:when>
				<xsl:when test="$funzCode = 'SR'">RIDUZIONE DI LIQUIDAZIONE</xsl:when>
				<xsl:when test="$funzCode = 'SN'">NOTIFICA RETTIFICA</xsl:when>
			</xsl:choose>
		</xsl:variable>
		<table border="0">
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
			<tr>
				<td>
					<!-- INIZIO TITOLO -->
					<table bordercolor="#000000" cellspacing="0" width="100%" border="1">
						<tr>
							<td class="titolo">
								<xsl:value-of select="concat(ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
							</td>
						</tr>
					</table>
					<br/>
					<!-- FINE TITOLO -->
				</td>
			</tr>
			<xsl:choose>
				<!-- STORNO INTERA LIQUIDAZIONE -->
				<xsl:when test='ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc="0000000000"'>
					<tr>
						<td>
							<xsl:call-template name="titolo1_storno_totale">
								<xsl:with-param name="title" select="$title"/>
							</xsl:call-template>
						</td>
					</tr>
					<tr>
						<td>
							<xsl:call-template name="info_storno"/>
						</td>
					</tr>
					<tr>
						<td/>
					</tr>
				</xsl:when>
				<!-- STORNO PARZIALE -->
				<xsl:otherwise>
					<tr>
						<td>
							<xsl:call-template name="titolo1_storno_parziale">
								<xsl:with-param name="title" select="$title"/>
							</xsl:call-template>
						</td>
					</tr>
					<tr>
						<td>
							<xsl:call-template name="info_storno"/>
						</td>
					</tr>
					<tr>
						<td>
							<xsl:call-template name="DatiPostaLiquidazioneStornata"/>
						</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<!-- FINE STORNO TEMPLATE -->
	<!-- INIZIO titolo1_storno_parziale TEMPLATE   -->
	<xsl:template name="titolo1_storno_parziale">
		<xsl:param name="title"/>
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td class="titolo1">
					<xsl:value-of select="concat('STORNO N. ', ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc, ' DELLA ', $title, ' N. ', ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc)"/>
					<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
						<!-- *** "di data" deve andare a capo *** -->
						<br/>
						<xsl:text>  DI DATA  </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<br/>
					<xsl:text>  LIMITATAMENTE ALLA POSTA SOTTO INDICATA </xsl:text>
					<xsl:if test="translate(ns0:Posizione_Finanziaria/ns0:Fin_Capitolo, ' 0', '') != ''">
						<br/>
						<xsl:text>CAPITOLO </xsl:text>
						<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
					</xsl:if>
				</td>
			</tr>
		</table>
	</xsl:template>
	<!-- INIZIO titolo1_storno_totale TEMPLATE   -->
	<xsl:template name="titolo1_storno_totale">
		<xsl:param name="title"/>
		<table bordercolor="#000000" cellspacing="0" width="100%" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo1">
					<xsl:value-of select="concat('STORNO N. ', ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc, ' DELLA ', $title, ' N. ', ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc)"/>
					<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
						<!-- *** "di data" deve andare a capo *** -->
						<br/>
						<xsl:text>  DI DATA  </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:if test="translate(ns0:Posizione_Finanziaria/ns0:Fin_Capitolo, ' 0', '') != ''">
						<br/>
						<xsl:text>CAPITOLO </xsl:text>
						<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
					</xsl:if>
				</td>
			</tr>
		</table>
	</xsl:template>
	<!-- INIZIO info_storno TEMPLATE   -->
	<xsl:template name="info_storno">
		<table border="0">
			<tr>
				<td width="640" class="testo" colspan="2" height="70">
					<xsl:value-of select="ns0:Dati_Doc/ns0:Descr_Fissa"/>
				</td>
			</tr>
			<!-- INIZIO TOTLIQUIDAZIONE -->
			<tr>
				<td width="320" class="totale" height="40" valign="bottom">
					<xsl:text>TOTALE LIQUIDAZIONE N.</xsl:text>
					<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
				</td>
				<td width="320" class="totale1" height="40" valign="bottom">
					<xsl:text>EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td width="320" class="totale">
					<xsl:text>TOTALE STORNO  </xsl:text>
				</td>
				<td width="320" class="totale1">
					<!-- *** aggiungo il segno meno *** -->
					<xsl:text>EURO -</xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Beneficiario/ns0:Pag_ImportoLordo"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<!-- FINE TOTLIQUIDAZIONE -->
			<!-- INIZIO ANAGRAFICA -->
			<tr>
				<td width="640" colspan="2">
					<table border="0">
						<tr>
							<td width="214" class="intest_anagrafica">
								<br/>
								<br/>
								<xsl:text>Indicazione dell'Estensore</xsl:text>
							</td>
							<td width="300" class="intest_anagrafica">
								<br/>
								<br/>
								<xsl:if test='ns0:Dati_Doc/ns0:Funzionario != ""'>
									<xsl:text>Funzionario responsabile proposto</xsl:text>
								</xsl:if>
							</td>
							<td width=""/>
						</tr>
						<tr>
							<td class="anagrafica">
								<br/>
								<br/>
								<xsl:value-of select="ns0:Dati_Doc/ns0:Estensore"/>
							</td>
							<td class="anagrafica">
								<br/>
								<br/>
								<xsl:value-of select="ns0:Dati_Doc/ns0:Funzionario"/>
							</td>
							<td class="anagrafica">
								<br/>
								<br/>
							</td>
						</tr>
						<tr>
							<td class="anagrafica">
								<br/>
								<br/>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<!-- FINE ANAGRAFICA -->
			<br/>
			<br/>
			<tr>
				<!-- aggiungo un'altra tabella -->
				<td>
					<table>
						<tr>
							<td width="640" class="testo_sx">
								Data:
								<font class="testo_bold">
									<xsl:call-template name="FormattaData">
										<xsl:with-param name="data" select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
									</xsl:call-template>
								</font>
							</td>
						</tr>
					</table>
					<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
					<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
				</td>
				<td width="126"/>
			</tr>
		</table>
	</xsl:template>
	<!--Inizio Template che scrive i dati della posta di liquidazione stornata-->
	<xsl:template name="DatiPostaLiquidazioneStornata">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500">DATI POSTA DI LIQUIDAZIONE STORNATA</td>
				<td>IMPORTO STORNATO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="testo_sx">
					<font class="testo_bold">
						<xsl:text>Numero della posta di liquidazione stornata</xsl:text>
						<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc"/>
					</font>
					<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc, ' 0', '') != ''">
						<xsl:text> di data </xsl:text>
						<font class="testo_bold">
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc"/>
								</xsl:with-param>
							</xsl:call-template>
						</font>
					</xsl:if>
				</td>
				<td>
					<!-- *** aggiungo il segno meno ***-->
					<xsl:text>-</xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Beneficiario/ns0:Pag_ImportoLordo"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<!-- *** sposto numero di archiviazione alla fine *** -->
			<xsl:call-template name="Beneficiario_Storno"/>
			<!-- *** numero di archiviazione *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="testo_bold">
							<xsl:value-of select="ns0:Ulteriori_Dati/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<!--Fine Template che scrive i dati della posta di liquidazione-->
	<!-- Inizio Template Beneficiario_Storno-->
	<xsl:template name="Beneficiario_Storno">
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<br/>
				<xsl:text>A: </xsl:text>
				<font class="desc_ogg">
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Cognome_Nome"/>
					<xsl:text> (Cod.</xsl:text>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Creditore"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Gruppo_Conti"/>
					<xsl:text>)</xsl:text>
				</font>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Indirizzo"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_CAP"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Localita"/>
				<xsl:text> </xsl:text>
				<xsl:if test='ns0:Beneficiario/ns0:Ben_Paese != ""'>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Paese"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<xsl:if test="translate(ns0:Beneficiario/ns0:Ben_Luogo_Nascita, ' 0', '') != ''or translate(ns0:Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<xsl:text>  Nato </xsl:text>
					<xsl:if test='ns0:Beneficiario/ns0:Ben_Luogo_Nascita != ""'>
						<xsl:text>a </xsl:text>
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Luogo_Nascita"/>
					</xsl:if>
					<xsl:if test="translate(ns0:Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
						<xsl:text> il </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Data_Nascita"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
		</xsl:if>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  C.F. </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_CFPIVA"/>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
	<!-- Fine Template Beneficiario_Storno-->
	<!-- ///////////////////     FINE STORNO      /////////////////// -->
	<!-- ///////////////////     INIZIO RICHIESTA STORNO      /////////////////// -->
	<!-- inizio Template  startRichiestaStorno  -->
	<xsl:template match="ns0:Documenti/ns0:Richiesta_Storno" name="startRichiestaStorno">
		<table>
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
		</table>
		<!-- INIZIO TITOLO -->
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo">
					<xsl:value-of select="concat(ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO -->
		<br/>
		<xsl:choose>
			<xsl:when test='ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc="0000000000"'>
				<!--CASO TUTTE LE POSTE -->
				<!-- INIZIO TITOLO1 -->
				<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
					<tr>
						<td width="640" colspan="2" class="titolo1">
            RICHIESTA DI STORNO N. <xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
							<xsl:text> DELLA LIQUIDAZIONE N. </xsl:text>
							<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
							<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
								<!-- *** "di data" deve andare a capo *** -->
								<br/>
								<xsl:text>  DI DATA  </xsl:text>
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data">
										<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
									</xsl:with-param>
								</xsl:call-template>
							</xsl:if>
							<br/>
							<xsl:text>CAPITOLO </xsl:text>
							<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
						</td>
					</tr>
				</table>
				<!-- FINE TITOLO1 -->
				<!-- INIZIO TESTO -->
				<table>
					<tr>
						<td width="640" class="testo">
							<br/>
							<br/>
							<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Descr_Fissa"/>
						</td>
					</tr>
				</table>
				<!-- FINE TESTO -->
				<!-- INIZIO TOTLIQUIDAZIONE -->
				<table>
					<tr>
						<td width="320" class="totale">
							<br/>
							<br/>
							<xsl:text>TOTALE LIQUIDAZIONE N. </xsl:text>
							<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
						</td>
						<td width="320" class="totale1">
							<br/>
							<br/>
							<xsl:text>EURO </xsl:text>
							<xsl:call-template name="FormattaEuro">
								<xsl:with-param name="importo">
									<xsl:value-of select="ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
								</xsl:with-param>
							</xsl:call-template>
						</td>
					</tr>
					<tr>
						<td width="320" class="totale">
							<xsl:text>TOTALE RICHIESTA STORNO N. </xsl:text>
							<xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
						</td>
						<td width="320" class="totale1">
							<xsl:text>EURO </xsl:text>
							<xsl:call-template name="FormattaEuro">
								<xsl:with-param name="importo">
									<xsl:value-of select="ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
								</xsl:with-param>
							</xsl:call-template>
						</td>
					</tr>
				</table>
				<!-- FINE TOTLIQUIDAZIONE -->
				<br/>
				<!-- INIZIO ANAGRAFICA -->
				<table>
					<tr>
						<td width="214" class="intest_anagrafica">
							<br/>
							<br/>
							<xsl:text>Indicazione dell'Estensore</xsl:text>
						</td>
						<td width="300" class="intest_anagrafica">
							<br/>
							<br/>
							<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Funzionario != ""'>
								<xsl:text>Funzionario responsabile proposto</xsl:text>
							</xsl:if>
						</td>
						<td width="126"/>
					</tr>
					<tr>
						<td class="anagrafica">
							<br/>
							<br/>
							<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Estensore"/>
						</td>
						<td class="anagrafica">
							<br/>
							<br/>
							<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Funzionario != ""'>
								<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Funzionario"/>
							</xsl:if>
						</td>
						<td class="anagrafica"/>
					</tr>
				</table>
				<!-- FINE ANAGRAFICA -->
				<br/>
				<table>
					<tr>
						<td width="640" class="testo_sx">
							Data:
							<font class="testo_bold">
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data" select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
								</xsl:call-template>
							</font>
						</td>
					</tr>
				</table>
				<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
				<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
				<br/>
				<!--MOTIVO-->
				<!-- *** solo se valorizzato *** -->
				<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Motivo != ""'>
					<table>
						<tr>
							<td width="160" class="testo">
								<xsl:text>Motivo della richiesta :</xsl:text>
							</td>
							<td width="480" class="testo">
								<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Motivo"/>
							</td>
						</tr>
					</table>
				</xsl:if>
				<!--MOTIVO-->
				<br/>
			</xsl:when>
			<xsl:otherwise>
				<!--CASO UNICA POSTA -->
				<!-- INIZIO TITOLO1 -->
				<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
					<tr>
						<td width="640" colspan="2" class="titolo1">
RICHIESTA DI STORNO N. <xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
							<xsl:text> DELLA LIQUIDAZIONE N. </xsl:text>
							<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
							<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
								<!-- *** "di data" deve andare a capo *** -->
								<br/>
								<xsl:text>  DI DATA  </xsl:text>
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data">
										<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
									</xsl:with-param>
								</xsl:call-template>
							</xsl:if>
							<!-- *** "limitatamente" deve andare a capo *** -->
							<br/>
							<xsl:text>LIMITATAMENTE ALLA POSTA SOTTO INDICATA</xsl:text>
							<br/>
							<xsl:text>CAPITOLO </xsl:text>
							<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
						</td>
					</tr>
				</table>
				<!-- FINE TITOLO1 -->
				<!-- INIZIO TESTO -->
				<table>
					<tr>
						<td width="640" class="testo">
							<br/>
							<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Descr_Fissa"/>
						</td>
					</tr>
				</table>
				<!-- FINE TESTO -->
				<br/>
				<!-- INIZIO TOTLIQUIDAZIONE -->
				<table>
					<tr>
						<td width="320" class="totale">
							<br/>
							<xsl:text>TOTALE LIQUIDAZIONE N. </xsl:text>
							<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
						</td>
						<td width="320" class="totale1">
							<br/>
							<xsl:text>EURO </xsl:text>
							<xsl:call-template name="FormattaEuro">
								<xsl:with-param name="importo">
									<xsl:value-of select="ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
								</xsl:with-param>
							</xsl:call-template>
						</td>
					</tr>
					<tr>
						<td width="320" class="totale">
							<xsl:text>TOTALE RICHIESTA DI STORNO N. </xsl:text>
							<xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
						</td>
						<td width="320" class="totale1">
							<xsl:text>EURO </xsl:text>
							<xsl:call-template name="FormattaEuro">
								<xsl:with-param name="importo">
									<xsl:value-of select="ns0:Beneficiario/ns0:Pag_ImportoLordo"/>
								</xsl:with-param>
							</xsl:call-template>
						</td>
					</tr>
				</table>
				<!-- FINE TOTLIQUIDAZIONE -->
				<!-- INIZIO ANAGRAFICA -->
				<table>
					<tr>
						<td width="214" class="intest_anagrafica">
							<br/>
							<br/>
							<xsl:text>Indicazione dell'Estensore</xsl:text>
						</td>
						<td width="300" class="intest_anagrafica">
							<br/>
							<br/>
							<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Funzionario != ""'>
								<xsl:text>Funzionario responsabile proposto</xsl:text>
							</xsl:if>
						</td>
						<td width="126"/>
					</tr>
					<tr>
						<td class="anagrafica">
							<br/>
							<br/>
							<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Estensore"/>
						</td>
						<td class="anagrafica">
							<br/>
							<br/>
							<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Funzionario != ""'>
								<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Funzionario"/>
							</xsl:if>
						</td>
						<td class="anagrafica"/>
					</tr>
				</table>
				<!-- FINE ANAGRAFICA -->
				<br/>
				<br/>
				<table>
					<tr>
						<td width="640" class="testo_sx">
							Data:
							<font class="testo_bold">
								<xsl:call-template name="FormattaData">
									<xsl:with-param name="data" select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
								</xsl:call-template>
							</font>
						</td>
					</tr>
				</table>
				<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
				<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
				<br/>
				<br/>
				<xsl:call-template name="DatiRichiestaStorno"/>
			</xsl:otherwise>
		</xsl:choose>
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<!-- fine Template  startRichiestaStorno   -->
	<!--INIZIO TEMPLATE CHE SCRIVE I DATI DELLA RICHIESTA DI STORNO-->
	<xsl:template name="DatiRichiestaStorno">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500" style="border-right:0px solid;">DATI POSTA DI LIQUIDAZIONE </td>
				<td class="intestazione_importo" style="border-left:0px solid;">IMPORTO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="testo_sx">
					<font class="testo_bold">
						<xsl:text>Numero della posta di liquidazione che si propone di bloccare </xsl:text>
						<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc"/>
					</font>
					<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc, ' 0', '') != ''">
						<xsl:text>di data </xsl:text>
						<font class="testo_bold">
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc"/>
								</xsl:with-param>
							</xsl:call-template>
						</font>
					</xsl:if>
				</td>
				<td>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Beneficiario/ns0:Pag_ImportoLordo"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<!-- *** sposto numero archiviazione alla fine *** -->
			<xsl:call-template name="Beneficiario_RichiestaStorno"/>
			<!-- *** numero archiviazione *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<br/>
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="testo_bold">
							<xsl:value-of select="ns0:Ulteriori_Dati/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<!-- Inizio Template Beneficiario_RichiestaStorno-->
	<xsl:template name="Beneficiario_RichiestaStorno">
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<br/>
				<xsl:text>A: </xsl:text>
				<font class="desc_ogg">
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Cognome_Nome"/>
					<xsl:text> (Cod.</xsl:text>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Creditore"/>
					<xsl:text> </xsl:text>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Gruppo_Conti"/>
					<xsl:text>)</xsl:text>
				</font>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Indirizzo"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_CAP"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Localita"/>
				<xsl:text> </xsl:text>
				<xsl:if test='ns0:Beneficiario/ns0:Ben_Paese != ""'>
					<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Paese"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<xsl:if test="translate(ns0:Beneficiario/ns0:Ben_Luogo_Nascita, ' 0', '') != ''or translate(ns0:Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<xsl:text>  Nato </xsl:text>
					<xsl:if test='ns0:Beneficiario/ns0:Ben_Luogo_Nascita != ""'>
						<xsl:text>a </xsl:text>
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Luogo_Nascita"/>
					</xsl:if>
					<xsl:if test="translate(ns0:Beneficiario/ns0:Ben_Data_Nascita, ' 0', '') != ''">
						<xsl:text> il </xsl:text>
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data">
								<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Data_Nascita"/>
							</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
		</xsl:if>
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:text>  C.F. </xsl:text>
				<xsl:value-of select="ns0:Beneficiario/ns0:Ben_CFPIVA"/>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
		<xsl:if test='ns0:Dati_Doc_Motivo/ns0:Motivo != ""'>
			<!--MOTIVO-->
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<br/>
					<xsl:text>Motivo della richiesta :</xsl:text>
					<xsl:value-of select="ns0:Dati_Doc_Motivo/ns0:Motivo"/>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<!--MOTIVO-->
		</xsl:if>
	</xsl:template>
	<!-- Fine Template Beneficiario_RichiestaStorno-->
	<!--FINE TEMPLATE CHE SCRIVE I DATI DELLA RICHIESTA DI STORNO-->
	<!-- ///////////////////     FINE RICHIESTA STORNO    /////////////////// -->
	<!-- ///////////////////     INIZIO INTEGRAZIONE      /////////////////// -->
	<!--INIZIO TEMPLATE INTEGRAZIONE-->
	<xsl:template match="ns0:Documenti/ns0:Integrazione" name="startIntegrazione">
		<table>
			<tr>
				<td width="640" colspan="2" class="intestazione">
					<xsl:value-of select="ns0:Ente/ns0:Ente_Descr"/>
				</td>
			</tr>
			<tr>
				<td height="15"/>
			</tr>
		</table>
		<!-- INIZIO TITOLO INTEGRAZIONE-->
		<table bordercolor="#000000" cellspacing="0" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo">
					<xsl:value-of select="concat(ns0:Struttura_Emittente/ns0:Strutt_Descr, ' (', ns0:Struttura_Emittente/ns0:Strutt_Codice, ')')"/>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO INTEGRAZIONE-->
		<br/>
		<!-- INIZIO TITOLO1 INTEGRAZIONE-->
		<table bordercolor="#000000" cellspacing="0" cellpadding="2" width="640" border="1">
			<tr>
				<td width="640" colspan="2" class="titolo1">
					<xsl:choose>
						<!-- scelta integrazione riferita a TUTTE le poste  -->
						<xsl:when test='ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc = "0000000000"'>
							<xsl:call-template name="titolo1_integrazione_totale"/>
						</xsl:when>
						<!-- scelta integrazione parziale riferita alla singola posta -->
						<xsl:otherwise>
							<xsl:call-template name="titolo1_integrazione_parziale"/>
						</xsl:otherwise>
					</xsl:choose>
				</td>
			</tr>
		</table>
		<!-- FINE TITOLO1 INTEGRAZIONE-->
		<br/>
		<table>
			<!-- INIZIO TESTO INTEGRAZIONE -->
			<tr>
				<td width="640" class="testo">
					<xsl:value-of select="ns0:Dati_Doc/ns0:Descr_Fissa"/>
				</td>
			</tr>
		</table>
		<!-- FINE TESTO INTEGRAZIONE-->
		<br/>
		<!-- INIZIO TOTLIQUIDAZIONE -->
		<table>
			<tr>
				<td width="350" class="totale">
					<!-- *** tolgo la parola originale dopo TOTALE LIQUIDAZIONE *** -->
					<xsl:text>TOTALE LIQUIDAZIONE  N. </xsl:text>
					<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
				</td>
				<!-- il tot liquidazione ci deve essere se si riferisce a tutte le poste altrimenti no???-->
				<td width="290" class="totale1">
					<xsl:text>EURO </xsl:text>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Riepilogo_Poste/ns0:Importo_Poste"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
		</table>
		<br/>
		<!-- FINE TOTLIQUIDAZIONE -->
		<!-- INIZIO ANAGRAFICA INTEGRAZIONE -->
		<table>
			<tr>
				<td width="214" class="intest_anagrafica">
					<xsl:text>Indicazione dell'Estensore</xsl:text>
				</td>
				<td width="300" class="intest_anagrafica">
					<xsl:if test='ns0:Dati_Doc/ns0:Funzionario != ""'>
						<xsl:text>Funzionario responsabile proposto</xsl:text>
					</xsl:if>
				</td>
				<td width="126"/>
			</tr>
			<tr>
				<td class="anagrafica">
					<br/>
					<br/>
					<xsl:value-of select="ns0:Dati_Doc/ns0:Estensore"/>
				</td>
				<td class="anagrafica">
					<br/>
					<br/>
					<xsl:value-of select="ns0:Dati_Doc/ns0:Funzionario"/>
				</td>
				<td class="anagrafica">
					<br/>
					<br/>
				</td>
			</tr>
		</table>
		<!-- FINE ANAGRAFICA INTEGRAZIONE-->
		<br/>
		<br/>
		<table>
			<tr>
				<td width="640" class="testo_sx">
							Data:
							<font class="testo_bold">
						<xsl:call-template name="FormattaData">
							<xsl:with-param name="data" select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Data_Doc"/>
						</xsl:call-template>
					</font>
				</td>
			</tr>
		</table>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:Visto"/>
		<xsl:apply-templates select="ns0:HistoryInfoTestata/ns0:FirmaInvio"/>
		<!-- INIZIO DATI  INTEGRATIVI -->
		<xsl:choose>
			<!-- scelta dati integrativi riferita a TUTTE le poste  -->
			<xsl:when test='ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc = "0000000000"'>
				<xsl:call-template name="DatiIntegrativiPosteTotali"/>
			</xsl:when>
			<!-- scelta dati integrativi parziale riferita alla singola posta -->
			<xsl:otherwise>
				<xsl:call-template name="DatiIntegrativiSingolaPosta"/>
			</xsl:otherwise>
		</xsl:choose>
		<!-- FINE DATI INTEGRATIVI -->
		<table>
			<tr>
				<td height="50"/>
			</tr>
		</table>
	</xsl:template>
	<!--FINE TEMPLATE INTEGRAZIONE-->
	<!-- TEMPLATE DatiIntegrativiPosteTotali -->
	<xsl:template name="DatiIntegrativiPosteTotali">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="3" width="640">
		    		DATI INTEGRATIVI VALIDI PER TUTTE LE POSTE DELLA LIQUIDAZIONE
				</td>
			</tr>
			<!-- *** inizio - aggiunta della voce visto inoltre ***-->
			<tr>
				<td width="500" class="totale_poste_sx" colspan="3">
					<xsl:text>Visto inoltre</xsl:text>
				</td>
			</tr>
			<!-- *** fine - aggiunta della voce visto inoltre *** -->
			<!--elenco evidenze ordinamento-->
			<xsl:for-each select="ns0:Estremi_Doc_Supporto/ns0:Estremi_Evidenza">
				<xsl:call-template name="EvidenzaIntegrazioneSenzaImporto"/>
				<br/>
			</xsl:for-each>
			<!--fine elenco evidenze ordinamento-->
		</table>
	</xsl:template>
	<!-- TEMPLATE DatiIntegrativiSingolaPosta-->
	<xsl:template name="DatiIntegrativiSingolaPosta">
		<table id="tableDatiPosta" cellspacing="0" cellpadding="2">
			<xsl:call-template name="addColgroup"/>
			<tr id="etichetta_poste">
				<td colspan="2" width="500" style="border-right:0px solid;">
		    		DATI INTEGRATIVI DELLA POSTA DI LIQUIDAZIONE
		    </td>
				<td class="intestazione_importo" style="border-left:0px solid;">IMPORTO</td>
			</tr>
			<tr>
				<td colspan="2" width="500" class="testo_sx">
					<font class="testo_bold">
						<xsl:text>Numero della posta di liquidazione oggetto di integrazione n.</xsl:text>
						<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Num_Doc"/>
					</font>
					<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc, ' 0', '') != ''">
						<xsl:text>di data </xsl:text>
						<font class="testo_bold">
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data">
									<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Posta/ns0:Data_Doc"/>
								</xsl:with-param>
							</xsl:call-template>
						</font>
					</xsl:if>
					<!-- *** spostare numero di archiviazione alla fine dei dati integrativi *** -->
					<br/>
				</td>
				<td>
					<xsl:call-template name="FormattaEuro">
						<xsl:with-param name="importo">
							<xsl:value-of select="ns0:Beneficiario/ns0:Pag_ImportoLordo"/>
						</xsl:with-param>
					</xsl:call-template>
				</td>
			</tr>
			<!--elenco evidenze ordinamento-->
			<!-- *** inizio - aggiunta della voce visto inoltre ***-->
			<xsl:if test='ns0:Estremi_Doc_Supporto != ""'>
				<tr>
					<td width="500" class="totale_poste_sx" colspan="2">
						<br/>
						<xsl:text>Visto inoltre</xsl:text>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
			<!-- *** fine - aggiunta della voce visto inoltre *** -->
			<xsl:for-each select="ns0:Estremi_Doc_Supporto/ns0:Estremi_Evidenza">
				<xsl:call-template name="EvidenzaIntegrazione"/>
				<br/>
			</xsl:for-each>
			<!--fine elenco evidenze ordinamento-->
			<!-- Info beneficiario-->
			<tr>
				<td width="500" class="totale_poste_sx" colspan="2">
					<br/>
					<xsl:text>A: </xsl:text>
					<font class="desc_ogg">
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Cognome_Nome"/>
						<xsl:text> (Cod.</xsl:text>
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Creditore"/>
						<xsl:value-of select="ns0:Beneficiario_Poste/ns0:Ben_Gruppo_Conti"/>
						<xsl:text>)</xsl:text>
						<br/>
					</font>
					<font class="totale_poste_sx">
						<xsl:text>  </xsl:text>
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Indirizzo"/>
						<xsl:if test='ns0:Beneficiario/ns0:Ben_Paese != ""'>
							<xsl:text>(</xsl:text>
							<xsl:value-of select="ns0:Beneficiario/ns0:Ben_Paese"/>
							<xsl:text>)</xsl:text>
						</xsl:if>
						<br/>
						<xsl:text>  C.F.</xsl:text>
						<xsl:value-of select="ns0:Beneficiario/ns0:Ben_CFPIVA"/>
					</font>
				</td>
				<xsl:call-template name="addEmptyCell"/>
			</tr>
			<!-- fine Info beneficiario-->
			<!-- *** aggiungo il numero di archiviazione  *** -->
			<xsl:if test='ns0:Ulteriori_Dati_Poste/ns0:Num_Archiviazione != ""'>
				<tr>
					<td width="500" class="totale_poste_dx" colspan="2">
						<xsl:text>n. di archiviazione </xsl:text>
						<font class="testo_bold">
							<xsl:value-of select="ns0:Ulteriori_Dati/ns0:Num_Archiviazione"/>
						</font>
					</td>
					<xsl:call-template name="addEmptyCell"/>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
	<!-- fine template dati integrativi-->
	<!-- TEMPLATE EVIDENZA INTEGRAZIONE-->
	<xsl:template name="EvidenzaIntegrazione">
		<tr>
			<td width="500" class="totale_poste_sx" colspan="2">
				<xsl:value-of select="ns0:Ev_Ordinamento"/>
				<xsl:text>. </xsl:text>
				<xsl:value-of select="ns0:Evidenza"/>
				<xsl:if test='ns0:Ev_Numero != ""'>
					<xsl:text> n.</xsl:text>
					<xsl:value-of select="ns0:Ev_Numero"/>
				</xsl:if>
				<xsl:if test="translate(ns0:Ev_Data, ' 0', '') != ''">
					<xsl:text> di data </xsl:text>
					<xsl:call-template name="FormattaData">
						<xsl:with-param name="data">
							<xsl:value-of select="ns0:Ev_Data"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
				<xsl:text> </xsl:text>
				<xsl:if test="ns0:Ev_Note != ''">
					<xsl:value-of select="concat('(', ns0:Ev_Note, ')')"/>
				</xsl:if>
			</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
	<!-- ***  template senza importo ***-->
	<xsl:template name="EvidenzaIntegrazioneSenzaImporto">
		<tr>
			<td width="500" class="totale_poste_sx" colspan="3">
				<xsl:value-of select="ns0:Ev_Ordinamento"/>
				<xsl:text>. </xsl:text>
				<xsl:value-of select="ns0:Evidenza"/>
				<xsl:if test='ns0:Ev_Numero != ""'>
					<xsl:text> n.</xsl:text>
					<xsl:value-of select="ns0:Ev_Numero"/>
				</xsl:if>
				<xsl:if test="translate(ns0:Ev_Data, ' 0', '') != ''">
					<xsl:text> di data </xsl:text>
					<xsl:call-template name="FormattaData">
						<xsl:with-param name="data">
							<xsl:value-of select="ns0:Ev_Data"/>
						</xsl:with-param>
					</xsl:call-template>
				</xsl:if>
				<xsl:text> </xsl:text>
				<xsl:if test="ns0:Ev_Note != ''">
					<xsl:value-of select="concat('(', ns0:Ev_Note, ')')"/>
				</xsl:if>
			</td>
		</tr>
	</xsl:template>
	<!--*** fine template senza importo *** -->
	<xsl:template name="titolo1_integrazione_totale">
     INTEGRAZIONE N.
     	<xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
		<xsl:text>  ALLA LIQUIDAZIONE N. </xsl:text>
		<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
		<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
			<!-- *** "di data" deve andare a capo *** -->
			<br/>
			<xsl:text>  DI DATA  </xsl:text>
			<xsl:call-template name="FormattaData">
				<xsl:with-param name="data">
					<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
		<br/>
		<xsl:text>CAPITOLO </xsl:text>
		<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
	</xsl:template>
	<xsl:template name="titolo1_integrazione_parziale">
     INTEGRAZIONE N.
     <xsl:value-of select="ns0:Estremi_Titolo/ns0:Estremi_Doc/ns0:Num_Doc"/>
		<xsl:text>  ALLA LIQUIDAZIONE N. </xsl:text>
		<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Num_Doc"/>
		<xsl:if test="translate(ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc, ' 0', '') != ''">
			<!-- *** "di data" deve andare a capo *** -->
			<br/>
			<xsl:text>  DI DATA  </xsl:text>
			<xsl:call-template name="FormattaData">
				<xsl:with-param name="data">
					<xsl:value-of select="ns0:Liquidazione_Collegata/ns0:Estremi_Doc/ns0:Data_Doc"/>
				</xsl:with-param>
			</xsl:call-template>
		</xsl:if>
		<br/>
		<xsl:text>LIMITATAMENTE ALLA POSTA SOTTO INDICATA</xsl:text>
		<br/>
		<xsl:text>CAPITOLO </xsl:text>
		<xsl:value-of select="ns0:Posizione_Finanziaria/ns0:Fin_Capitolo"/>
	</xsl:template>
	<!-- ///////////////////     FINE INTEGRAZIONE      /////////////////// -->
	<xsl:template name="FormattaEuro">
		<xsl:param name="importo"/>
		<xsl:value-of select="format-number($importo div 100, '###.##0,00', 'european')"/>
	</xsl:template>
	<xsl:template name="estract_NOME_COGNOME">
		<xsl:param name="cn"/>
		<xsl:if test="translate($cn, ' &#160;', '') != ''">
			<xsl:choose>
				<!-- se ci sono almeno 2 "/" -->
				<xsl:when test="(string-length($cn) - 2) >= string-length(translate($cn, '/', ''))">
					<xsl:value-of select="concat(substring-before($cn,'/'), ' ', substring-before(substring-after($cn,'/'),'/'))"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$cn"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
	<xsl:template match="ns0:Visto">
		<xsl:if test="./@vistatore != ''">
			<br/>
			<table>
				<tr>
					<td width="640" class="testo_sx">
						Vistatore:
						<font class="testo_bold">
							<xsl:call-template name="estract_NOME_COGNOME">
								<xsl:with-param name="cn" select="./@vistatore"/>
							</xsl:call-template>
						</font>
					</td>
				</tr>
				<tr>
					<td width="640" class="testo_sx">
						Data visto:
						<font class="testo_bold">
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data" select="./@data"/>
							</xsl:call-template>
						</font>
					</td>
				</tr>
				<xsl:if test="./@urgenza > 0">
					<tr>
						<td width="640" class="testo_sx">
							Tipo visto:
							<font class="testo_bold">
								<xsl:choose>
									<xsl:when test="./@urgenza = 2">URGENTE</xsl:when>
									<xsl:otherwise>NON URGENTE</xsl:otherwise>
								</xsl:choose>
							</font>
						</td>
					</tr>
				</xsl:if>
			</table>
		</xsl:if>
	</xsl:template>
	<xsl:template match="ns0:FirmaInvio">
		<xsl:if test="./@firmatario != ''">
			<br/>
			<table>
				<tr>
					<td width="640" class="testo_sx">
						Indicazione del firmatario:
					</td>
				</tr>
				<tr>
					<td width="640" class="testo_sx">
						<font class="testo_bold">
							<xsl:value-of select="./@titolo"/>
						</font>
					</td>
				</tr>
				<tr>
					<td width="640" class="testo_sx">
						<font class="testo_bold">
							<xsl:call-template name="estract_NOME_COGNOME">
								<xsl:with-param name="cn" select="./@firmatario"/>
							</xsl:call-template>
						</font>
					</td>
				</tr>
				<tr>
					<td width="640" class="testo_sx">
						Data firma:
						<font class="testo_bold">
							<xsl:call-template name="FormattaData">
								<xsl:with-param name="data" select="./@data"/>
							</xsl:call-template>
						</font>
					</td>
				</tr>
			</table>
		</xsl:if>
	</xsl:template>
	<xsl:template name="addColgroup">
		<colgroup>
			<col id="colDati"/>
			<col/>
			<col id="colImporti"/>
		</colgroup>
	</xsl:template>
	<xsl:template name="addEmptyCell">
		<td>&#160;</td>
	</xsl:template>
	<xsl:template name="addEmptyRow">
		<tr style="height:35px;">
			<td width="500" colspan="2">&#160;</td>
			<xsl:call-template name="addEmptyCell"/>
		</tr>
	</xsl:template>
</xsl:stylesheet>
