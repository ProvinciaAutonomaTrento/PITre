<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- SEZIONE RAPPORTO DI VERSAMENTO -->
  <xsl:template match="RapportoVersamento">
    <html>
      <head>
        <style type="text/css">
          #container {font-family: sans-serif; margin-left: auto; margin-right: auto; max-width: 1280px; min-width: 930px; padding: 0;  }
          .tab {margin: 0 0 20px 0; width: 90%; background-color: #F2F2F2; font-size: 12px;}
          .tab .head1 {width: 20%; font-weight: bold; padding: 0 0 10px 0; font-size: 18px;}
          .tab .head2 {width: 20%; font-weight: bold; padding: 10px 0 5px 0; font-size: 16px;}
          .tab .head3 {width: 20%; font-weight: bold; padding: 5px 0 5px 0; font-size: 12px;}
          .tdleft 	{width: 20%;}
          .tdleft2	{width: 20%; padding: 0 0 0 10px;}
          .tdleft3	{width: 20%; padding: 0 0 0 25px;}
        </style>
      </head>
      <body>
        <div id="container">
          <table class="tab">
            <tr>
              <td class="head1" colspan="2">Rapporto di versamento</td>
            </tr>
          </table>

          <table class="tab">
            <tr>
              <td class="head1">Esito Generale</td>
            </tr>
            <tr>
              <td class="tdleft">Codice esito:</td>
              <td>
                <xsl:value-of select="EsitoGenerale/CodiceEsito"/>
              </td>
            </tr>
            <td class="tdleft">Codice errore:</td>
            <td>
              <xsl:value-of select="EsitoGenerale/CodiceErrore"/>
            </td>
            <tr>
            </tr>
            <td class="tdleft">Messaggio di errore:</td>
            <td>
              <xsl:value-of select="EsitoGenerale/MessaggioErrore"/>
            </td>
            <tr>

            </tr>
          </table>
          <!-- WARNING ULTERIORI -->
          <xsl:if test="WarningUlteriori">
            <table class="tab">
              <tr>
                <td class="head1" colspan="2">Warning Ulteriori</td>
              </tr>
              <xsl:apply-templates select="WarningUlteriori/Warning"/>
            </table>
          </xsl:if>
          <!-- VERSATORE -->
          <table class="tab">
            <tr>
              <td class="head1">Versatore</td>
            </tr>
            <tr>
              <td class="tdleft">Ambiente</td>
              <td>
                <xsl:value-of select="Versatore/Ambiente"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Ente</td>
              <td>
                <xsl:value-of select="Versatore/Ente"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Struttura</td>
              <td>
                <xsl:value-of select="Versatore/Struttura"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">UserID</td>
              <td>
                <xsl:value-of select="Versatore/UserID"/>
              </td>
            </tr>
          </table>
          <!-- SIP -->
          <table class="tab">
            <tr>
              <td class="head1">Pacchetto di versamento</td>
            </tr>
            <tr>
              <td class="head2">Dati generali pacchetto</td>
            </tr>
            <tr>
              <td class="tdleft">URN Indice pacchetto</td>
              <td>
                <xsl:value-of select="SIP/URNIndiceSIP"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Hash Indice pacchetto</td>
              <td>
                <xsl:value-of select="SIP/HashIndiceSIP"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Algoritmo Hash Indice pacchetto</td>
              <td>
                <xsl:value-of select="SIP/AlgoritmoHashIndiceSIP"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Encoding Hash Indice pacchetto</td>
              <td>
                <xsl:value-of select="SIP/EncodingHashIndiceSIP"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Data Versamento</td>
              <td>
                <xsl:value-of select="SIP/DataVersamento"/>
              </td>
            </tr>
            <tr>
              <td class="head2">Dettagli Unità Documentaria</td>
            </tr>
            <tr>
              <td class="head3" colspan="2">Chiave</td>
            </tr>
            <tr>
              <td class="tdleft2">Numero</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/Chiave/Numero"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft2">Anno</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/Chiave/Anno"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft2">Tipo Registro</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/Chiave/TipoRegistro"/>
              </td>
            </tr>
            <tr>
              <td class="head3">Tipologia Unità Documentaria</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/TipologiaUnitaDocumentaria"/>
              </td>
            </tr>
            <!-- Documento Principale -->
            <tr>
              <td class="head3" colspan="2">Documento Principale</td>
            </tr>
            <tr>
              <td class="tdleft2">Chiave</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/DocumentoPrincipale/ChiaveDoc"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft2">ID Documento</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/DocumentoPrincipale/IDDocumento"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft2">Tipo Documento</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/DocumentoPrincipale/TipoDocumento"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft2">Firmato Digitalmente</td>
              <td>
                <xsl:value-of select="SIP/UnitaDocumentaria/DocumentoPrincipale/FirmatoDigitalmente"/>
              </td>
            </tr>
            <xsl:apply-templates select="SIP/UnitaDocumentaria/DocumentoPrincipale/Componenti/Componente"/>
            <!-- Allegati -->
            <xsl:for-each select="SIP/UnitaDocumentaria/Allegato">
              <tr>
                <td class="head3" colspan="2">Allegato</td>
              </tr>
              <tr>
                <td class="tdleft2">Chiave</td>
                <td>
                  <xsl:value-of select="ChiaveDoc"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">ID Documento</td>
                <td>
                  <xsl:value-of select="IDDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Tipo Documento</td>
                <td>
                  <xsl:value-of select="TipoDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Firmato Digitalmente</td>
                <td>
                  <xsl:value-of select="FirmatoDigitalmente"/>
                </td>
              </tr>
              <xsl:apply-templates select="Componenti/Componente"/>
            </xsl:for-each>
            <!-- Annessi -->
            <xsl:for-each select="SIP/UnitaDocumentaria/Annesso">
              <tr>
                <td class="head3" colspan="2">Annesso</td>
              </tr>
              <tr>
                <td class="tdleft2">Chiave</td>
                <td>
                  <xsl:value-of select="ChiaveDoc"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">ID Documento</td>
                <td>
                  <xsl:value-of select="IDDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Tipo Documento</td>
                <td>
                  <xsl:value-of select="TipoDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Firmato Digitalmente</td>
                <td>
                  <xsl:value-of select="FirmatoDigitalmente"/>
                </td>
              </tr>
              <xsl:apply-templates select="Componenti/Componente"/>
            </xsl:for-each>
            <!-- Annotazioni -->
            <xsl:for-each select="SIP/UnitaDocumentaria/Annotazione">
              <tr>
                <td class="head3" colspan="2">Annotazione</td>
              </tr>
              <tr>
                <td class="tdleft2">Chiave</td>
                <td>
                  <xsl:value-of select="ChiaveDoc"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">ID Documento</td>
                <td>
                  <xsl:value-of select="IDDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Tipo Documento</td>
                <td>
                  <xsl:value-of select="TipoDocumento"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft2">Firmato Digitalmente</td>
                <td>
                  <xsl:value-of select="FirmatoDigitalmente"/>
                </td>
              </tr>
              <xsl:apply-templates select="Componenti/Componente"/>
            </xsl:for-each>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="//Componente">
    <tr>
      <td class="tdleft2" colspan="2">Componente:</td>
    </tr>
    <tr>
      <td class="tdleft3">URN</td>
      <td>
        <xsl:value-of select="URN"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft3">Hash</td>
      <td>
        <xsl:value-of select="Hash"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft3">Algoritmo Hash</td>
      <td>
        <xsl:value-of select="AlgoritmoHash"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft3">Encoding</td>
      <td>
        <xsl:value-of select="Encoding"/>
      </td>
    </tr>
  </xsl:template>
  <xsl:template match="Warning">
    <tr>
      <td class="head3" colspan="2">Warning</td>
    </tr>
    <tr>
      <td class="tdleft2">Codice</td>
      <td>
        <xsl:value-of select="CodiceWarning"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft2">Descrizione</td>
      <td>
        <xsl:value-of select="DescrizioneWarning"/>
      </td>
    </tr>
  </xsl:template>
  <!-- SEZIONE STATO CONSERVAZIONE -->
  <xsl:template match="StatoConservazione">
    <html>
      <head>
        <style type="text/css">
          #container {font-family: sans-serif; margin-left: auto; margin-right: auto; max-width: 1280px; min-width: 930px; padding: 0;  }
          .tab {margin: 0 0 20px 0; width: 90%; background-color: #F2F2F2; font-size: 12px;}
          .tab .head1 {width: 20%; font-weight: bold; padding: 0 0 10px 0; font-size: 18px;}
          .tab .head2 {width: 20%; font-weight: bold; padding: 10px 0 5px 0; font-size: 16px;}
          .tab .head3 {width: 20%; font-weight: bold; padding: 5px 0 5px 0; font-size: 12px;}
          .tab .head4 {width: 20%; font-weight: bold; padding: 5px 0 5px 0; font-size: 11px;}
          .tdleft 	{width: 20%;}
          .tdleft2	{width: 20%; padding: 0 0 0 10px;}
          .tdleft3	{width: 20%; padding: 0 0 0 25px;}
        </style>
      </head>
      <body>
        <div id="container">
          <table class="tab">
            <tr>
              <td colspan="2" class="head1">Chiave</td>
            </tr>
            <tr>
              <td class="tdleft">Numero</td>
              <td>
                <xsl:value-of select="UnitaDocumentaria/Chiave/Numero"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Anno</td>
              <td>
                <xsl:value-of select="UnitaDocumentaria/Chiave/Anno"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Tipo Registro</td>
              <td>
                <xsl:value-of select="UnitaDocumentaria/Chiave/TipoRegistro"/>
              </td>
            </tr>
          </table>
          <table class="tab">
            <tr>
              <td colspan="2" class="head1">Stato di conservazione</td>
            </tr>
            <tr>
              <td colspan="2" class="tdleft">
                <xsl:value-of select="UnitaDocumentaria/StatoConservazioneUD"/>
              </td>
            </tr>
          </table>
          <xsl:if test="UnitaDocumentaria/Volumi">
            <table class="tab">
              <tr>
                <td colspan="2" class="head1">Volumi</td>
              </tr>
              <xsl:for-each select="UnitaDocumentaria/Volumi/Volume">
                <tr>
                  <td class="head2" colspan="2">Volume</td>
                </tr>
                <tr>
                  <td class="tdleft">ID</td>
                  <td>
                    <xsl:value-of select="IdVolume"/>
                  </td>
                </tr>
                <tr>
                  <td class="tdleft">Nome</td>
                  <td>
                    <xsl:value-of select="StatoVolume"/>
                  </td>
                </tr>
                <xsl:if test="DataAperturaVolume">
                  <tr>
                    <td class="tdleft">Data Apertura</td>
                    <td>
                      <xsl:value-of select="DataAperturaVolume"/>
                    </td>
                  </tr>
                </xsl:if>
                <xsl:if test="DataChiusuraVolume">
                  <tr>
                    <td class="tdleft">Data Chiusura</td>
                    <td>
                      <xsl:value-of select="DataChiusuraVolume"/>
                    </td>
                  </tr>
                </xsl:if>
                <xsl:if test="DataFirmaVolume">
                  <tr>
                    <td class="tdleft">Data Firma</td>
                    <td>
                      <xsl:value-of select="DataFirmaVolume"/>
                    </td>
                  </tr>
                </xsl:if>
                <tr>
                  <td class="head3" colspan="2">Documenti contenuti:</td>
                </tr>
                <xsl:apply-templates select="DocumentiContenuti"/>
              </xsl:for-each>
            </table>
          </xsl:if>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="//DocumentoContenuto">
    <tr>
      <td class="head4" colspan="2">Documento</td>
    </tr>
    <tr>
      <td class="tdleft2">URN</td>
      <td>
        <xsl:value-of select="URNDocumento"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft2">Data versamento</td>
      <td>
        <xsl:value-of select="DataVersamento"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft2">Tipo conservazione</td>
      <td>
        <xsl:value-of select="TipoConservazione"/>
      </td>
    </tr>
  </xsl:template>
  <!-- SEZIONE ESITO VERSAMENTO -->
  <xsl:template match="EsitoVersamento">
    <html>
      <head>
        <style type="text/css">
          #container {font-family: sans-serif; margin-left: auto; margin-right: auto; max-width: 1280px; min-width: 930px; padding: 0;  }
          .tab {margin: 0 0 20px 0; width: 90%; background-color: #F2F2F2; font-size: 12px;}
          .tab .head1 {width: 20%; font-weight: bold; padding: 0 0 10px 0; font-size: 18px;}
          .tab .head2 {width: 20%; font-weight: bold; padding: 0 0 5px 0; font-size: 16px;}
          .tab .head3 {width: 20%; font-weight: bold; padding: 5px 0 5px 0; font-size: 12px;}
          .tab .head4 {width: 20%; font-weight: bold; padding: 5px 0 5px 0; font-size: 11px;}
          .tdleft 	{width: 20%;}
          .tdleft2	{width: 20%; padding: 0 0 0 10px;}
          .tdleft3	{width: 20%; padding: 0 0 0 25px;}
        </style>
      </head>
      <body>
        <div id="container">
          <table class="tab">
            <tr>
              <td class="head1" colspan="2">Esito generale</td>
            </tr>
            <tr>
              <td class="tdleft">Codice esito</td>
              <td>
                <xsl:value-of select="EsitoGenerale/CodiceEsito"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Codice errore</td>
              <td>
                <xsl:value-of select="EsitoGenerale/CodiceErrore"/>
              </td>
            </tr>
            <tr>
              <td class="tdleft">Messaggio di errore</td>
              <td>
                <xsl:value-of select="EsitoGenerale/MessaggioErrore"/>
              </td>
            </tr>
          </table>
          <xsl:if test="ErroriUlteriori">
            <table class="tab">
            </table>
          </xsl:if>
          <xsl:if test="WarningUlteriori">
            <table class="tab">
            </table>
          </xsl:if>
          <xsl:if test="EsitoChiamataWS">
            <table class="tab">
              <tr>
                <td class="head2" colspan="2">Esito chiamata WS</td>
              </tr>
              <tr>
                <td class="tdleft">Versione WS corretta</td>
                <td>
                  <xsl:value-of select="EsitoChiamataWS/VersioneWSCorretta"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft">Credenziali operatore</td>
                <td>
                  <xsl:value-of select="EsitoChiamataWS/CredenzialiOperatore"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft">File attesi ricevuti</td>
                <td>
                  <xsl:value-of select="EsitoChiamataWS/FileAttesiRicevuti"/>
                </td>
              </tr>
            </table>
          </xsl:if>
          <xsl:if test="EsitoXSD">
            <table class="tab">
              <tr>
                <td class="head2" colspan="2">Esito XSD</td>
              </tr>
              <tr>
                <td class="tdleft">Codice esito</td>
                <td>
                  <xsl:value-of select="EsitoXSD/CodiceEsito"/>
                </td>
              </tr>
              <xsl:if test="EsitoXSD/ControlloStrutturaXML">
                <tr>
                  <td class="tdleft">Controllo struttura XML</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/ControlloStrutturaXML"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="EsitoXSD/UnivocitaIDComponenti">
                <tr>
                  <td class="tdleft">Univocità ID componenti</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/UnivocitaIDComponenti"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="EsitoXSD/UnivocitaIDDocumenti">
                <tr>
                  <td class="tdleft">Univocità ID documenti</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/UnivocitaIDDocumenti"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="EsitoXSD/CorrispondenzaAllegatiDichiarati">
                <tr>
                  <td class="tdleft">Corrispondenza allegati dichiarati</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/CorrispondenzaAllegatiDichiarati"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="EsitoXSD/CorrispondenzaAnnessiDichiarati">
                <tr>
                  <td class="tdleft">Corrispondenza annessi dichiarati</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/CorrispondenzaAnnessiDichiarati"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="EsitoXSD/CorrispondenzaAnnotazioniDichiarate">
                <tr>
                  <td class="tdleft">Corrispondenza annotazioni dichiarate</td>
                  <td>
                    <xsl:value-of select="EsitoXSD/CorrispondenzaAnnotazioniDichiarate"/>
                  </td>
                </tr>
              </xsl:if>
            </table>
          </xsl:if>
          <xsl:if test="Configurazione">
            <table class="tab">
              <tr>
                <td class="head2" colspan="2">Configurazione</td>
              </tr>
              <tr>
                <td class="tdleft">Tipo conservazione</td>
                <td>
                  <xsl:value-of select="Configurazione/TipoConservazione"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft">Forza accettazione</td>
                <td>
                  <xsl:value-of select="Configurazione/ForzaAccettazione"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft">Forza conservazione</td>
                <td>
                  <xsl:value-of select="Configurazione/ForzaConservazione"/>
                </td>
              </tr>
              <tr>
                <td class="tdleft">Forza collegamento</td>
                <td>
                  <xsl:value-of select="Configurazione/ForzaCollegamento"/>
                </td>
              </tr>
            </table>
          </xsl:if>
          <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria">
            <table class="tab">
              <tr>
                <td class="head2" colspan="2">Esito unità documentaria</td>
              </tr>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/CodiceEsito">
                <tr>
                  <td class="tdleft">Codice esito</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/CodiceEsito"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/IdentificazioneVersatore">
                <tr>
                  <td class="tdleft">Identificazione versatore</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/IdentificazioneVersatore"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/UnivocitaChiave">
                <tr>
                  <td class="tdleft">Univocità chiave</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/UnivocitaChiave"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/VerificaTipologiaUD">
                <tr>
                  <td class="tdleft">Verifica tipologia UD</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/VerificaTipologiaUD"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/CorrispondenzaDatiSpecifici">
                <tr>
                  <td class="tdleft">Corrispondenza dati specifici</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/CorrispondenzaDatiSpecifici"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/PresenzaUDCollegate">
                <tr>
                  <td class="tdleft">Presenza UD collegate</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/PresenzaUDCollegate"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/EsitoUnitaDocumentaria/VerificaFirmeUnitaDocumentaria">
                <tr>
                  <td class="tdleft">Verifica firme UD</td>
                  <td>
                    <xsl:value-of select="UnitaDocumentaria/EsitoUnitaDocumentaria/VerificaFirmeUnitaDocumentaria"/>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/DocumentoPrincipale">
                <xsl:for-each select="//DocumentoPrincipale">
                  <tr>
                    <td class="head3" colspan="2">Documento principale</td>
                  </tr>
                  <xsl:call-template name="EsitoDoc"/>
                </xsl:for-each>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/Allegati">
                <xsl:for-each select="Allegato">
                  <tr>
                    <td class="head3" colspan="2">Allegato</td>
                  </tr>
                  <xsl:call-template name="EsitoDoc"/>
                </xsl:for-each>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/Annessi">
                <xsl:for-each select="//Annesso">
                  <tr>
                    <td class="head3" colspan="2">Annesso</td>
                  </tr>
                  <xsl:call-template name="EsitoDoc"/>
                </xsl:for-each>
              </xsl:if>
              <xsl:if test="UnitaDocumentaria/Annotazione">
                <xsl:for-each select="Annotazione">
                  <tr>
                    <td class="head3" colspan="2">Annotazione</td>
                  </tr>
                  <xsl:call-template name="EsitoDoc"/>
                </xsl:for-each>
              </xsl:if>
            </table>
          </xsl:if>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="EsitoDoc">
    <tr>
      <td class="tdleft">Chiave</td>
      <td>
        <xsl:value-of select="ChiaveDoc"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft">ID documento</td>
      <td>
        <xsl:value-of select="IDDocumento"/>
      </td>
    </tr>
    <tr>
      <td class="tdleft">Tipo documento</td>
      <td>
        <xsl:value-of select="TipoDocumento"/>
      </td>
    </tr>
    <xsl:if test="FirmatoDigitalmente">
      <tr>
        <td class="tdleft">Firmato digitalmente</td>
        <td>
          <xsl:value-of select="FirmatoDigitalmente"/>
        </td>
      </tr>
    </xsl:if>
    <tr>
      <td class="tdleft" colspan="2">Esito documento</td>
    </tr>
    <tr>
      <td class="tdleft2">Codice esito</td>
      <td>
        <xsl:value-of select="EsitoDocumento/CodiceEsito"/>
      </td>
    </tr>
    <xsl:if test="EsitoDocumento/VerificaTipoDocumento">
      <tr>
        <td class="tdleft2">Verifica tipo documento</td>
        <td>
          <xsl:value-of select="EsitoDocumento/VerificaTipoDocumento"/>
        </td>

      </tr>
    </xsl:if>
    <xsl:if test="EsitoDocumento/CorrispondenzaDatiSpecifici">
      <tr>
        <td class="tdleft2">Corrispondenza dati specifici</td>
        <td>
          <xsl:value-of select="EsitoDocumento/CorrispondenzaDatiSpecifici"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="EsitoDocumento/VerificaTipoStruttura">
      <tr>
        <td class="tdleft2">Verifica tipo struttura</td>
        <td>
          <xsl:value-of select="EsitoDocumento/VerificaTipoStruttura"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="EsitoDocumento/VerificaFirmeDocumento">
      <tr>
        <td class="tdleft2">Verifica firme documento</td>
        <td>
          <xsl:value-of select="EsitoDocumento/VerificaFirmeDocumento"/>
        </td>
      </tr>
    </xsl:if>
    <tr>
      <td class="tdleft2">Univocità ordine presentazione</td>
      <td>
        <xsl:value-of select="EsitoDocumento/UnivocitaOrdinePresentazione"/>
      </td>
    </tr>
    <xsl:if test="Componenti">
      <xsl:for-each select="Componenti/Componente">
        <tr>
          <td class="tdleft" colspan="2">Componente</td>
        </tr>
        <xsl:call-template name="Comp"/>
        <xsl:if test="SottoComponenti">
          <xsl:for-each select="SottoComponenti/SottoComponente">
            <tr>
              <td class="tdleft" colspan="2">SottoComponente</td>
              <xsl:call-template name="Comp"/>
            </tr>
          </xsl:for-each>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
  <xsl:template name="Comp">
    <tr>
      <td class="tdleft2">Ordine presentazione</td>
      <td>
        <xsl:value-of select="OrdinePresentazione"/>
      </td>
    </tr>
    <xsl:if test="TipoComponente">
      <tr>
        <td class="tdleft2">Tipo componente</td>
        <td>
          <xsl:value-of select="TipoComponente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="TipoSottoComponente">
      <tr>
        <td class="tdleft2">Tipo sottocomponente</td>
        <td>
          <xsl:value-of select="TipoSottoComponente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="Hash">
      <tr>
        <td class="tdleft2">Hash</td>
        <td>
          <xsl:value-of select="Hash"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="AlgoritmoHash">
      <tr>
        <td class="tdleft2">Algoritmo hash</td>
        <td>
          <xsl:value-of select="AlgoritmoHash"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="Encoding">
      <tr>
        <td class="tdleft2">Encoding</td>
        <td>
          <xsl:value-of select="Encoding"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="FormatoRappresentazione">
      <tr>
        <td class="tdleft2">Formato rappresentazione</td>
        <td>
          <xsl:value-of select="FormatoRappresentazione"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="FormatoRappresentazioneEsteso">
      <tr>
        <td class="tdleft2">Formato rappresentazione esteso</td>
        <td>
          <xsl:value-of select="FormatoRappresentazioneEsteso"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="IdoneitaFormato">
      <tr>
        <td class="tdleft2">Idoneità formato</td>
        <td>
          <xsl:value-of select="IdoneitaFormato"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="FirmatoDigitalmente">
      <tr>
        <td class="tdleft2">Firmato digitalmente</td>
        <td>
          <xsl:value-of select="FirmatoDigitalmente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="EsitoComponente">
      <tr>
        <td class="tdleft2" colspan="2">Esito componente</td>
      </tr>
      <xsl:for-each select="EsitoComponente">
        <xsl:call-template name="VerificaEsitoComp"/>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="EsitoSottoComponente">
      <tr>
        <td class="tdleft2" colspan="2">Esito componente</td>
      </tr>
      <xsl:for-each select="EsitoSottoComponente">
        <xsl:call-template name="VerificaEsitoComp"/>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
  <xsl:template name="VerificaEsitoComp">
    <xsl:if test="CodiceEsito">
      <tr>
        <td class="tdleft3">Codice esito</td>
        <td>
          <xsl:value-of select="CodiceEsito"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="VerificaTipoComponente">
      <tr>
        <td class="tdleft3">Verifica tipo componente</td>
        <td>
          <xsl:value-of select="VerificaTipoComponente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="CorrispondenzaDatiSpecifici">
      <tr>
        <td class="tdleft3">Corrispondenza dati specifici</td>
        <td>
          <xsl:value-of select="CorrispondenzaDatiSpecifici"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="VerificaTipoSupportoComponente">
      <tr>
        <td class="tdleft3">Verifica tipo supporto componente</td>
        <td>
          <xsl:value-of select="VerificaTipoSupportoComponente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="VerificaNomeComponente">
      <tr>
        <td class="tdleft3">Verifica nome componente</td>
        <td>
          <xsl:value-of select="VerificaNomeComponente"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="VerificaAmmissibilitaFormato">
      <tr>
        <td class="tdleft3">Verifica ammissibilità formato</td>
        <td>
          <xsl:value-of select="VerificaAmmissibilitaFormato"/>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="VerificaRiconoscimentoFormato">
      <tr>
        <td class="tdleft3">Verifica riconoscimento formato</td>
        <td>
          <xsl:value-of select="VerificaRiconoscimentoFormato"/>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>