--Script di inserimento della micro funzione DO_STAMPA_SEGN_AUTOMATICA in DPA_ANAGRAFICA_FUNZIONI

BEGIN

declare @Cnt Int

SELECT @Cnt = COUNT(*)  
  FROM DOCSADM.DPA_ANAGRAFICA_FUNZIONI
  WHERE COD_FUNZIONE='DO_STAMPA_SEGN_AUTOMATICA'

  If (@Cnt = 0 )  -- inserisco la microfunzione non esistente
    begin
		INSERT  INTO DOCSADM.DPA_ANAGRAFICA_FUNZIONI
		( COD_FUNZIONE,VAR_DESC_FUNZIONE
		,CHA_TIPO_FUNZ,DISABLED )
		Values
		( 'DO_STAMPA_SEGN_AUTOMATICA',
		'Abilita il pulsante Segnatura Elettronica per la Firma Automatica di un documento',
		NULL,
		'N'
		)
	End   
END



