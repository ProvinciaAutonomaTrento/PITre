--Script di inserimento della micro funzione FIRMA_HSM in DPA_ANAGRAFICA_FUNZIONI

DECLARE 
cnt int;

BEGIN
SELECT COUNT(*)  INTO cnt
  FROM dpa_anagrafica_funzioni
  Where COD_FUNZIONE='FIRMA_HSM';
  
  If (Cnt         = 0 ) Then -- inserisco la microfunzione non esistente
    dbms_output.put_line ('ok entro nell''if ');
    INSERT  INTO dpa_anagrafica_funzioni
        ( COD_FUNZIONE,VAR_DESC_FUNZIONE
        ,CHA_TIPO_FUNZ,DISABLED )
        Values
        ( 'FIRMA_HSM',
        'Abilita la firma HSM',
        NULL,
        'N'
        );
  End if;   
END;