-- MEV Attivazione Trasmissione Automatica
-- by Iacozzilli Giordano.
-- Se il campo spedizione automatica è già presente e valorizzato con 1 
-- anche il campo trasmissione automatica andrà valorizzato con 1
-- altrimenti di default è 0
--Uso un cursore nella dpa_amministra per prevedere una futura multiamministrazione. 
DECLARE
  v_spedizione_auto_doc     DPA_AMMINISTRA.SPEDIZIONE_AUTO_DOC%TYPE;
  v_system_id  DPA_AMMINISTRA.SYSTEM_ID%TYPE;

  CURSOR c1 IS SELECT system_id, spedizione_auto_doc FROM DPA_AMMINISTRA;

BEGIN

  OPEN c1;
      LOOP
        FETCH c1 INTO v_system_id, v_spedizione_auto_doc; 
              
        if v_spedizione_auto_doc = '1' then
			   update DPA_AMMINISTRA 
					set DPA_AMMINISTRA.TRASMISSIONE_AUTO_DOC ='1' 
				where DPA_AMMINISTRA.SYSTEM_ID = v_system_id and  TRASMISSIONE_AUTO_DOC is null;
       else
           update DPA_AMMINISTRA 
				set DPA_AMMINISTRA.TRASMISSIONE_AUTO_DOC ='0' 
		   where DPA_AMMINISTRA.SYSTEM_ID = v_system_id and  TRASMISSIONE_AUTO_DOC is null;
        end if;
   EXIT WHEN c1%NOTFOUND;
      END LOOP;
  CLOSE c1;
 COMMIT;
END;
/
