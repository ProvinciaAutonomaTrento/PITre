--Script di inserimento della micro funzione FIRMA_HSM in DPA_ANAGRAFICA_FUNZIONI

BEGIN

declare @Cnt Int

SELECT @Cnt = COUNT(*)  
  FROM DOCSADM.DPA_ANAGRAFICA_FUNZIONI
  WHERE COD_FUNZIONE='FIRMA_HSM'
  
  If (@Cnt = 0 )  -- inserisco la microfunzione non esistente
	begin
		INSERT  INTO DOCSADM.DPA_ANAGRAFICA_FUNZIONI
		( COD_FUNZIONE,VAR_DESC_FUNZIONE
		,CHA_TIPO_FUNZ,DISABLED )
		Values
		( 'FIRMA_HSM',
		'Abilita la firma HSM',
		NULL,
		'N'
		)
	End
END