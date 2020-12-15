
-- lasciare una riga bianca vuota all'inizio e a fine file per evitare efetti di concatenazione tra i vari script
-- inserire SEMPRE il carattere / a fine script altrimenti lo script non va in esecuzione!

begin
-- MEV Fatturazione elettronica by Panici - Iazzetta
-- modifiche Stefano
 
begin

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DOCSADM].[DPA_DATI_FATTURAZIONE]') AND type in (N'U'))

  begin

    CREATE TABLE [DOCSADM].[DPA_DATI_FATTURAZIONE](
	[SYSTEM_ID] [int] IDENTITY(1,1) NOT NULL,
	[CODICE_AMM] [varchar](128) NOT NULL,
	[CODICE_AOO] [varchar](20) NOT NULL,
	[CODICE_UO] [varchar](128) NOT NULL,
	[CODICE_UAC] [varchar](128) NULL,
	[CODICE_CLASSIFICAZIONE] [varchar](20) NULL,
	[VAR_UTENTE_PROPRIETARIO] [varchar](128) NULL,
	[VAR_TIPOLOGIA_DOCUMENTO] [varchar](128) NULL,
	[VAR_RAGIONE_TRASMISSIONE] [varchar](128) NULL,
	[ISTANZA_PITRE] [varchar](512) NULL,
	[MODELLO_TRASMISSIONE] [varchar](128) NULL,
 CONSTRAINT [PK_DPA_DATI_FATTURAZIONE] PRIMARY KEY CLUSTERED 
(
	[SYSTEM_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
       
       
  end 
  END
END
go

