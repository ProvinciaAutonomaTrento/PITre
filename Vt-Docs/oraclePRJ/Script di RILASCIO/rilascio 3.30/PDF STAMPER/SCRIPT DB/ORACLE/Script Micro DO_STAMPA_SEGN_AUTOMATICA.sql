--Script di inserimento della micro funzione DO_STAMPA_SEGN_AUTOMATICA in DPA_ANAGRAFICA_FUNZIONI

DECLARE 
cnt int;

BEGIN
SELECT COUNT(*)  INTO cnt
  FROM dpa_anagrafica_funzioni
  Where COD_FUNZIONE='DO_STAMPA_SEGN_AUTOMATICA';
  
  If (Cnt         = 0 ) Then -- inserisco la microfunzione non esistente
    dbms_output.put_line ('ok entro nell''if ');
    INSERT  INTO dpa_anagrafica_funzioni
        ( COD_FUNZIONE,VAR_DESC_FUNZIONE
        ,CHA_TIPO_FUNZ,DISABLED )
        Values
        ( 'DO_STAMPA_SEGN_AUTOMATICA',
        'Abilita il pulsante Segnatura Elettronica per la Firma Automatica di un documento',
        NULL,
        'N'
        );
  End if;   
END;