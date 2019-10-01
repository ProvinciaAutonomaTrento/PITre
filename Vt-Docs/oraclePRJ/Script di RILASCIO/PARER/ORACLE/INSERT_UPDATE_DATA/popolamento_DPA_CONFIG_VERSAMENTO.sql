DECLARE

CurrAmmId NUMBER;
type AmmList IS table OF Number(10);
myAmmList AmmList;


BEGIN

	SELECT system_id BULK COLLECT INTO myAmmList 
	FROM dpa_amministra;
	
	FOR Amm IN 1 .. myAmmList.COUNT LOOP
	
		CurrAmmId := myAmmList(Amm);
		
		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'P',
		'<?xml version="1.0" encoding="UTF-8"?> <UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> <Intestazione> <Versione></Versione> <Versatore> <Ambiente></Ambiente> <Ente></Ente> <Struttura></Struttura> <UserID></UserID> </Versatore> <Chiave> <Numero></Numero> <Anno></Anno> <TipoRegistro></TipoRegistro> </Chiave> <TipologiaUnitaDocumentaria></TipologiaUnitaDocumentaria> </Intestazione> <Configurazione> <TipoConservazione></TipoConservazione> <ForzaAccettazione></ForzaAccettazione> <ForzaConservazione></ForzaConservazione> <ForzaCollegamento></ForzaCollegamento> <SimulaSalvataggioDatiInDB></SimulaSalvataggioDatiInDB> </Configurazione> <ProfiloArchivistico> <FascicoloPrincipale> <Classifica></Classifica> <Fascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </Fascicolo> <SottoFascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </SottoFascicolo> </FascicoloPrincipale> <FascicoliSecondari></FascicoliSecondari> </ProfiloArchivistico> <ProfiloUnitaDocumentaria> <Oggetto></Oggetto> <Data></Data> </ProfiloUnitaDocumentaria> <DatiSpecifici> <VersioneDatiSpecifici></VersioneDatiSpecifici> <NumeroProtocollo></NumeroProtocollo> <AnnoProtocollazione></AnnoProtocollazione> <TipoRegistroProtocollo></TipoRegistroProtocollo> <SegnaturaProtocollo></SegnaturaProtocollo> <TipoProtocollo></TipoProtocollo> <CodiceRegistro></CodiceRegistro> <DescrizioneRegistro></DescrizioneRegistro> <Mittente></Mittente> <MezzoDiSpedizioneMittente></MezzoDiSpedizioneMittente> <ProtocolloMittente></ProtocolloMittente> <DataProtocolloMittente></DataProtocolloMittente> <DataArrivo></DataArrivo> <OraArrivo></OraArrivo> <Destinatari></Destinatari> <TipologiaDocumentalePITre></TipologiaDocumentalePITre> <DataCreazioneProfiloDocumento></DataCreazioneProfiloDocumento> <UtenteCreatore></UtenteCreatore> <RuoloCreatore></RuoloCreatore> <UOCreatrice></UOCreatrice> </DatiSpecifici> <DocumentiCollegati></DocumentiCollegati> <NumeroAllegati></NumeroAllegati> <NumeroAnnessi></NumeroAnnessi> <NumeroAnnotazioni></NumeroAnnotazioni> <DocumentoPrincipale> <IDDocumento></IDDocumento> <TipoDocumento></TipoDocumento> <ProfiloDocumento> <Descrizione></Descrizione> </ProfiloDocumento> <StrutturaOriginale> <TipoStruttura></TipoStruttura> <Componenti> <Componente> <ID></ID> <OrdinePresentazione></OrdinePresentazione> <TipoComponente></TipoComponente> <TipoSupportoComponente></TipoSupportoComponente> <NomeComponente></NomeComponente> <FormatoFileVersato></FormatoFileVersato> <HashVersato></HashVersato> <IDComponenteVersato></IDComponenteVersato> <RiferimentoTemporale></RiferimentoTemporale> <DescrizioneRiferimentoTemporale></DescrizioneRiferimentoTemporale> </Componente> </Componenti> </StrutturaOriginale> </DocumentoPrincipale> <Allegati></Allegati> <Annessi></Annessi> <Annotazioni></Annotazioni> </UnitaDocumentaria>',
		'1.6'
		);

		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'G',
		'<?xml version="1.0" encoding="UTF-8"?> <UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> <Intestazione> <Versione></Versione> <Versatore> <Ambiente></Ambiente> <Ente></Ente> <Struttura></Struttura> <UserID></UserID> </Versatore> <Chiave> <Numero></Numero> <Anno></Anno> <TipoRegistro></TipoRegistro> </Chiave> <TipologiaUnitaDocumentaria></TipologiaUnitaDocumentaria> </Intestazione> <Configurazione> <TipoConservazione></TipoConservazione> <ForzaAccettazione></ForzaAccettazione> <ForzaConservazione></ForzaConservazione> <ForzaCollegamento></ForzaCollegamento> <SimulaSalvataggioDatiInDB></SimulaSalvataggioDatiInDB> </Configurazione> <ProfiloArchivistico> <FascicoloPrincipale> <Classifica></Classifica> <Fascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </Fascicolo> <SottoFascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </SottoFascicolo> </FascicoloPrincipale> <FascicoliSecondari></FascicoliSecondari> </ProfiloArchivistico> <ProfiloUnitaDocumentaria> <Oggetto></Oggetto> <Data></Data> </ProfiloUnitaDocumentaria> <DatiSpecifici> <VersioneDatiSpecifici></VersioneDatiSpecifici> <TipologiaDocumentalePITre></TipologiaDocumentalePITre> <DataCreazioneProfiloDocumento></DataCreazioneProfiloDocumento> <UtenteCreatore></UtenteCreatore> <RuoloCreatore></RuoloCreatore> <UOCreatrice></UOCreatrice> </DatiSpecifici> <DocumentiCollegati></DocumentiCollegati> <NumeroAllegati></NumeroAllegati> <NumeroAnnessi></NumeroAnnessi> <NumeroAnnotazioni></NumeroAnnotazioni> <DocumentoPrincipale> <IDDocumento></IDDocumento> <TipoDocumento></TipoDocumento> <ProfiloDocumento> <Descrizione></Descrizione> </ProfiloDocumento> <StrutturaOriginale> <TipoStruttura></TipoStruttura> <Componenti> <Componente> <ID></ID> <OrdinePresentazione></OrdinePresentazione> <TipoComponente></TipoComponente> <TipoSupportoComponente></TipoSupportoComponente> <NomeComponente></NomeComponente> <FormatoFileVersato></FormatoFileVersato> <HashVersato></HashVersato> <IDComponenteVersato></IDComponenteVersato> <RiferimentoTemporale></RiferimentoTemporale> <DescrizioneRiferimentoTemporale></DescrizioneRiferimentoTemporale> </Componente> </Componenti> </StrutturaOriginale> </DocumentoPrincipale> <Allegati></Allegati> <Annessi></Annessi> <Annotazioni></Annotazioni> </UnitaDocumentaria>',
		'1.6'
		);

		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'R',
		'<?xml version="1.0" encoding="UTF-8"?> <UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> <Intestazione> <Versione></Versione> <Versatore> <Ambiente></Ambiente> <Ente></Ente> <Struttura></Struttura> <UserID></UserID> </Versatore> <Chiave> <Numero></Numero> <Anno></Anno> <TipoRegistro></TipoRegistro> </Chiave> <TipologiaUnitaDocumentaria></TipologiaUnitaDocumentaria> </Intestazione> <Configurazione> <TipoConservazione></TipoConservazione> <ForzaAccettazione></ForzaAccettazione> <ForzaConservazione></ForzaConservazione> <ForzaCollegamento></ForzaCollegamento> <SimulaSalvataggioDatiInDB></SimulaSalvataggioDatiInDB> </Configurazione> <ProfiloArchivistico> <FascicoloPrincipale> <Classifica></Classifica> <Fascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </Fascicolo> <SottoFascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </SottoFascicolo> </FascicoloPrincipale> <FascicoliSecondari></FascicoliSecondari> </ProfiloArchivistico> <ProfiloUnitaDocumentaria> <Oggetto></Oggetto> <Data></Data> </ProfiloUnitaDocumentaria> <DatiSpecifici> <VersioneDatiSpecifici></VersioneDatiSpecifici> <SegnaturaRepertorio></SegnaturaRepertorio> <CodiceRegistro_REP></CodiceRegistro_REP> <DescrizioneRegistro_REP></DescrizioneRegistro_REP> <CodiceRF_REP></CodiceRF_REP> <DescrizioneRF_REP></DescrizioneRF_REP> <NumeroProtocollo></NumeroProtocollo> <AnnoProtocollazione></AnnoProtocollazione> <DataProtocollazione></DataProtocollazione> <TipoRegistroProtocollo></TipoRegistroProtocollo> <SegnaturaProtocollo></SegnaturaProtocollo> <TipoProtocollo></TipoProtocollo> <CodiceRegistro_PROT></CodiceRegistro_PROT> <DescrizioneRegistro_PROT></DescrizioneRegistro_PROT> <CodiceRF_PROT></CodiceRF_PROT> <DescrizioneRF_PROT></DescrizioneRF_PROT> <Mittente></Mittente> <MezzoDiSpedizioneMittente></MezzoDiSpedizioneMittente> <ProtocolloMittente></ProtocolloMittente> <DataProtocolloMittente></DataProtocolloMittente> <DataArrivo></DataArrivo> <OraArrivo></OraArrivo> <Destinatari></Destinatari> <TipologiaDocumentalePITre></TipologiaDocumentalePITre> <DataCreazioneProfiloDocumento></DataCreazioneProfiloDocumento> <UtenteCreatore></UtenteCreatore> <RuoloCreatore></RuoloCreatore> <UOCreatrice></UOCreatrice> </DatiSpecifici> <DocumentiCollegati></DocumentiCollegati> <NumeroAllegati></NumeroAllegati> <NumeroAnnessi></NumeroAnnessi> <NumeroAnnotazioni></NumeroAnnotazioni> <DocumentoPrincipale> <IDDocumento></IDDocumento> <TipoDocumento></TipoDocumento> <ProfiloDocumento> <Descrizione></Descrizione> </ProfiloDocumento> <StrutturaOriginale> <TipoStruttura></TipoStruttura> <Componenti> <Componente> <ID></ID> <OrdinePresentazione></OrdinePresentazione> <TipoComponente></TipoComponente> <TipoSupportoComponente></TipoSupportoComponente> <NomeComponente></NomeComponente> <FormatoFileVersato></FormatoFileVersato> <HashVersato></HashVersato> <IDComponenteVersato></IDComponenteVersato> <RiferimentoTemporale></RiferimentoTemporale> <DescrizioneRiferimentoTemporale></DescrizioneRiferimentoTemporale> </Componente> </Componenti> </StrutturaOriginale> </DocumentoPrincipale> <Allegati></Allegati> <Annessi></Annessi> <Annotazioni></Annotazioni> </UnitaDocumentaria>',
		'1.7'
		);

		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'S',
		'<?xml version="1.0" encoding="UTF-8"?> <UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> <Intestazione> <Versione></Versione> <Versatore> <Ambiente></Ambiente> <Ente></Ente> <Struttura></Struttura> <UserID></UserID> </Versatore> <Chiave> <Numero></Numero> <Anno></Anno> <TipoRegistro></TipoRegistro> </Chiave> <TipologiaUnitaDocumentaria></TipologiaUnitaDocumentaria> </Intestazione> <Configurazione> <TipoConservazione></TipoConservazione> <ForzaAccettazione></ForzaAccettazione> <ForzaConservazione></ForzaConservazione> <ForzaCollegamento></ForzaCollegamento> <SimulaSalvataggioDatiInDB></SimulaSalvataggioDatiInDB> </Configurazione> <ProfiloArchivistico> <FascicoloPrincipale> <Classifica></Classifica> <Fascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </Fascicolo> <SottoFascicolo> <Identificativo></Identificativo> <Oggetto></Oggetto> </SottoFascicolo> </FascicoloPrincipale> <FascicoliSecondari></FascicoliSecondari> </ProfiloArchivistico> <ProfiloUnitaDocumentaria> <Oggetto></Oggetto> <Data></Data> </ProfiloUnitaDocumentaria> <DatiSpecifici> <VersioneDatiSpecifici></VersioneDatiSpecifici> <TipoRegistro></TipoRegistro> <CodiceRegistro_PROT></CodiceRegistro_PROT> <DescrizioneRegistro_PROT></DescrizioneRegistro_PROT> <CodiceRegistro_REP></CodiceRegistro_REP> <DescrizioneRegistro_REP></DescrizioneRegistro_REP> <CodiceRF_REP></CodiceRF_REP> <DescrizioneRF_REP></DescrizioneRF_REP> <RuoloResponsabileRegistro></RuoloResponsabileRegistro> <FrequenzaDiStampa></FrequenzaDiStampa> <PrimoElementoRegistrato></PrimoElementoRegistrato> <DataPrimaRegistrazione></DataPrimaRegistrazione> <UltimoElementoRegistrato></UltimoElementoRegistrato> <DataUltimaRegistrazione></DataUltimaRegistrazione> <DataCreazioneProfiloDocumento></DataCreazioneProfiloDocumento> <UtenteCreatore></UtenteCreatore> <RuoloCreatore></RuoloCreatore> <UOCreatrice></UOCreatrice> </DatiSpecifici> <DocumentiCollegati></DocumentiCollegati> <NumeroAllegati></NumeroAllegati> <NumeroAnnessi></NumeroAnnessi> <NumeroAnnotazioni></NumeroAnnotazioni> <DocumentoPrincipale> <IDDocumento></IDDocumento> <TipoDocumento></TipoDocumento> <ProfiloDocumento> <Descrizione></Descrizione> </ProfiloDocumento> <StrutturaOriginale> <TipoStruttura></TipoStruttura> <Componenti> <Componente> <ID></ID> <OrdinePresentazione></OrdinePresentazione> <TipoComponente></TipoComponente> <TipoSupportoComponente></TipoSupportoComponente> <NomeComponente></NomeComponente> <FormatoFileVersato></FormatoFileVersato> <HashVersato></HashVersato> <IDComponenteVersato></IDComponenteVersato> <RiferimentoTemporale></RiferimentoTemporale> <DescrizioneRiferimentoTemporale></DescrizioneRiferimentoTemporale> </Componente> </Componenti> </StrutturaOriginale> </DocumentoPrincipale> <Allegati></Allegati> <Annessi></Annessi> <Annotazioni></Annotazioni> </UnitaDocumentaria>',
		'1.7'
		);

		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'F',
		'<?xml version="1.0" encoding="UTF-8"?><UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><Intestazione><Versione/><Versatore><Ambiente/><Ente/><Struttura/><UserID/></Versatore><Chiave><Numero/><Anno/><TipoRegistro/></Chiave><TipologiaUnitaDocumentaria/></Intestazione><Configurazione><TipoConservazione/><ForzaAccettazione/><ForzaConservazione/><ForzaCollegamento/><SimulaSalvataggioDatiInDB/></Configurazione><ProfiloArchivistico><FascicoloPrincipale><Classifica/><Fascicolo><Identificativo/><Oggetto/></Fascicolo><SottoFascicolo><Identificativo/><Oggetto/></SottoFascicolo></FascicoloPrincipale><FascicoliSecondari/></ProfiloArchivistico><ProfiloUnitaDocumentaria><Oggetto/><Data/></ProfiloUnitaDocumentaria><DatiSpecifici><VersioneDatiSpecifici/><SegnaturaRepertorio/><NumeroRepertorio/><DataRepertorio/><CodiceRegistro_REP/><DescrizioneRegistro_REP/><CodiceRF_REP/><DescrizioneRF_REP/><NumeroProtocollo/><AnnoProtocollazione/><DataProtocollazione/>		<TipoRegistroProtocollo/><SegnaturaProtocollo/><TipoProtocollo/><CodiceRegistro_PROT/><DescrizioneRegistro_PROT/><CodiceRF_PROT/><DescrizioneRF_PROT/><Mittente/><MezzoDiSpedizioneMittente/><ProtocolloMittente/><DataProtocolloMittente/><DataArrivo/><OraArrivo/><NumeroEmissione/><DataEmissione/><DenominazioneMittente/><PartitaIvaMittente/><CodiceFiscaleMittente/><CIG/><CUP/><IdentificativoSdI/><AliquotaIvaReverseCharge/><IvaTotaleReverseCharge/><TipologiaDocumentalePITre/><DataCreazioneProfiloDocumento/><UtenteCreatore/><RuoloCreatore/><UOCreatrice/></DatiSpecifici><DocumentiCollegati/><NumeroAllegati/><NumeroAnnessi/><NumeroAnnotazioni/><DocumentoPrincipale><IDDocumento/><TipoDocumento/><ProfiloDocumento><Descrizione/></ProfiloDocumento><StrutturaOriginale><TipoStruttura/><Componenti><Componente><ID/><OrdinePresentazione/><TipoComponente/><TipoSupportoComponente/><NomeComponente/><FormatoFileVersato/><HashVersato/><IDComponenteVersato/><RiferimentoTemporale/><DescrizioneRiferimentoTemporale/></Componente></Componenti></StrutturaOriginale></DocumentoPrincipale><Allegati/><Annessi/><Annotazioni/></UnitaDocumentaria>',
		'1.1'
		);
		
		
		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml, cha_vers_dati
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'L',
		'<?xml version="1.0" encoding="UTF-8"?><UnitaDocumentaria xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><Intestazione><Versione/><Versatore><Ambiente/><Ente/><Struttura/><UserID/></Versatore><Chiave><Numero/><Anno/><TipoRegistro/></Chiave><TipologiaUnitaDocumentaria/></Intestazione><Configurazione><TipoConservazione/><ForzaAccettazione/><ForzaConservazione/><ForzaCollegamento/><SimulaSalvataggioDatiInDB/></Configurazione><ProfiloArchivistico><FascicoloPrincipale><Classifica/><Fascicolo><Identificativo/><Oggetto/></Fascicolo><SottoFascicolo><Identificativo/><Oggetto/></SottoFascicolo></FascicoloPrincipale><FascicoliSecondari/></ProfiloArchivistico><ProfiloUnitaDocumentaria><Oggetto/><Data/></ProfiloUnitaDocumentaria><DatiSpecifici><VersioneDatiSpecifici/><SegnaturaRepertorio/><NumeroRepertorio/><DataRepertorio/><CodiceRegistro_REP/><DescrizioneRegistro_REP/><CodiceRF_REP/><DescrizioneRF_REP/><NumeroProtocollo/><AnnoProtocollazione/><DataProtocollazione/><TipoRegistroProtocollo/><SegnaturaProtocollo/><TipoProtocollo/><CodiceRegistro_PROT/><DescrizioneRegistro_PROT/><CodiceRF_PROT/><DescrizioneRF_PROT/><Mittente/><MezzoDiSpedizioneMittente/><ProtocolloMittente/><DataProtocolloMittente/><DataArrivo/><OraArrivo/><DenominazioneMittente/><PartitaIvaMittente/><CodiceFiscaleMittente/><IdentificativoSdI/><TipologiaDocumentalePITre/><DataCreazioneProfiloDocumento/><UtenteCreatore/><RuoloCreatore/><UOCreatrice/></DatiSpecifici><DocumentiCollegati/><NumeroAllegati/><NumeroAnnessi/><NumeroAnnotazioni/><DocumentoPrincipale><IDDocumento/><TipoDocumento/><ProfiloDocumento><Descrizione/></ProfiloDocumento><StrutturaOriginale><TipoStruttura/><Componenti><Componente><ID/><OrdinePresentazione/><TipoComponente/><TipoSupportoComponente/><NomeComponente/><FormatoFileVersato/><HashVersato/><IDComponenteVersato/><RiferimentoTemporale/><DescrizioneRiferimentoTemporale/></Componente></Componenti></StrutturaOriginale></DocumentoPrincipale><Allegati/><Annessi/><Annotazioni/></UnitaDocumentaria>',
		'1.1'
		);
		
		

		INSERT INTO dpa_config_versamento
		(
		system_id, id_amm, cha_tipo, var_template_xml
		)
		VALUES
		(
		seq.NEXTVAL, CurrAmmId, 'M',
		'<?xml version="1.0" encoding="utf-8"?> <Documento IDdocumento="" DataCreazione="" Oggetto="" Tipo="" LivelloRiservatezza="">   <SoggettoProduttore>     <Amministrazione CodiceAmministrazione="" DescrizioneAmministrazione="" />     <GerarchiaUO></GerarchiaUO>     <Creatore CodiceRuolo="" DescrizioneRuolo="" CodiceUtente="" DescrizioneUtente="" />   </SoggettoProduttore>   <Registrazione CodiceAOO="" DescrizioneAOO="" CodiceRF="" DescrizioneRF="" SegnaturaProtocollo="" NumeroProtocollo="" TipoProtocollo="" DataProtocollo="" OraProtocollo="" SegnaturaEmergenza="" NumeroProtocolloEmergenza="" DataProtocolloEmergenza="">     <ProtocolloMittente Protocollo="" Data="" MezzoSpedizione="" />     <Protocollista CodiceUtente="" DescrizioneUtente="" CodiceRuolo="" DescrizioneRuolo="" UOAppartenenza="" />   </Registrazione>   <ContestoArchivistico>     <Classificazione CodiceClassificazione="" TitolarioDiRiferimento="" />     <DocumentoCollegato IDdocumento="" DataCreazione="" Oggetto="" SegnaturaProtocollo="" NumeroProtocollo="" DataProtocollo="" />      </ContestoArchivistico>   <Tipologia NomeTipologia=""></Tipologia>   <Allegati></Allegati>    <File Formato="" Dimensione="" Impronta="" AlgoritmoHash="">   <MarcaTemporale NumeroSerie="" Data="" Ora="" SNCertificato="" DataInizioValidità="" DataFineValidità="" ImprontaDocumentoAssociato="" TimeStampingAuthority="" CodiceFiscale="" />   </File> </Documento>'
		);

	END LOOP;
	
	COMMIT;

EXCEPTION WHEN OTHERS THEN ROLLBACK;
	
END;