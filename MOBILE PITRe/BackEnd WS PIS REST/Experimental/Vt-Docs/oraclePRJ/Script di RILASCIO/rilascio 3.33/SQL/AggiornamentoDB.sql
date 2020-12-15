
CREATE TABLE [DPA_FATTURA](
	[idAmministrazione] [varchar](100) NULL,
	[templateXML] [nvarchar](4000) NULL,
	[formato_trasmissione] [varchar](255) NULL,
	[esigibilita_IVA] [varchar](50) NULL,
	[condizioni_pagamento] [varchar](255) NULL
) ON [PRIMARY]

ALTER TABLE [DPA_DETT_GLOBALI]
ADD [VAR_COD_IPA] [varchar](50) NULL

INSERT INTO [DPA_FATTURA] (idAmministrazione,templateXML,formato_trasmissione,esigibilita_IVA,condizioni_pagamento)
SELECT SYSTEM_ID, 
'<?xml version="1.0" encoding="UTF-8"?><p:FatturaElettronica versione="1.0" xmlns:ds="http://www.w3.org/2000/09/xmldsig#" xmlns:p="http://www.fatturapa.gov.it/sdi/fatturapa/v1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><FatturaElettronicaHeader><DatiTrasmissione><IdTrasmittente><IdPaese></IdPaese><IdCodice></IdCodice></IdTrasmittente><ProgressivoInvio></ProgressivoInvio><FormatoTrasmissione></FormatoTrasmissione><CodiceDestinatario></CodiceDestinatario><ContattiTrasmittente><Telefono></Telefono><Email></Email></ContattiTrasmittente></DatiTrasmissione><CedentePrestatore><DatiAnagrafici><IdFiscaleIVA><IdPaese></IdPaese><IdCodice></IdCodice></IdFiscaleIVA><Anagrafica><Denominazione></Denominazione></Anagrafica><RegimeFiscale></RegimeFiscale></DatiAnagrafici><Sede><Indirizzo></Indirizzo><NumeroCivico></NumeroCivico><CAP></CAP><Comune></Comune><Provincia></Provincia><Nazione></Nazione></Sede><IscrizioneREA><Ufficio></Ufficio><NumeroREA></NumeroREA><CapitaleSociale></CapitaleSociale><SocioUnico></SocioUnico><StatoLiquidazione></StatoLiquidazione></IscrizioneREA><RiferimentoAmministrazione></RiferimentoAmministrazione></CedentePrestatore><CessionarioCommittente><DatiAnagrafici><IdFiscaleIVA><IdPaese></IdPaese><IdCodice></IdCodice></IdFiscaleIVA><Anagrafica><Denominazione></Denominazione></Anagrafica></DatiAnagrafici><Sede><Indirizzo></Indirizzo><NumeroCivico></NumeroCivico><CAP></CAP><Comune></Comune><Provincia></Provincia><Nazione></Nazione></Sede></CessionarioCommittente></FatturaElettronicaHeader><FatturaElettronicaBody><DatiGenerali><DatiGeneraliDocumento><TipoDocumento></TipoDocumento><Divisa></Divisa><Data></Data><Numero></Numero><ImportoTotaleDocumento></ImportoTotaleDocumento></DatiGeneraliDocumento><DatiOrdineAcquisto><IdDocumento></IdDocumento><CodiceCUP></CodiceCUP><CodiceCIG></CodiceCIG></DatiOrdineAcquisto><DatiContratto><IdDocumento></IdDocumento><CodiceCUP></CodiceCUP><CodiceCIG></CodiceCIG></DatiContratto></DatiGenerali><DatiBeniServizi></DatiBeniServizi><DatiPagamento><CondizioniPagamento></CondizioniPagamento><DettaglioPagamento><ModalitaPagamento></ModalitaPagamento><DataRiferimentoTerminiPagamento></DataRiferimentoTerminiPagamento><GiorniTerminiPagamento></GiorniTerminiPagamento><ImportoPagamento></ImportoPagamento><IstitutoFinanziario></IstitutoFinanziario><IBAN></IBAN><BIC></BIC></DettaglioPagamento></DatiPagamento></FatturaElettronicaBody></p:FatturaElettronica>',
'SDI10',
'I',
'TP01'
from DPA_AMMINISTRA

CREATE TABLE [DPA_SERVIZI_ESTERNI](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[DESCRIZIONE] [varchar](255) NULL,
	[SERVIZIO] [varchar](500) NULL
) ON [PRIMARY]


CREATE TABLE [DPA_PARAMETRI_SERVIZIO](
	[ID_SERVIZIO] [int] NOT NULL,
	[ID_PARAMETRO] [int] NOT NULL
) ON [PRIMARY]

CREATE TABLE [DPA_PARAMETRO_SERVIZI](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[DESCRIZIONE] [varchar](255) NOT NULL,
	[TIPO_VALORE] [varchar](255) NULL,
	[POSIZIONE] [int] NOT NULL DEFAULT 1
) ON [PRIMARY]

CREATE TABLE [DPA_STATI_SERVIZI](
	[ID_SERVIZIO] [int] NOT NULL,
	[ID_STATO] [int] NOT NULL
) ON [PRIMARY]

CREATE TABLE [DPA_PROFILE_FATTURA](
	[DOCNUMBER] [varchar](100) NOT NULL,
	[IDSDI] [varchar](100) NOT NULL,
	[DIAGRAMMA] [varchar](100) NOT NULL
) ON [PRIMARY]

ALTER TABLE [DOCSADM].[DPA_FATTURA] ADD idTipoAtto varchar(100)