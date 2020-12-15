-- MEV Attivazione Trasmissione Automatica
-- by Iacozzilli Giordano.
-- Se il campo spedizione automatica è già presente e valorizzato con 1 
-- anche il campo trasmissione automatica andrà valorizzato con 1
-- altrimenti di default è 0
--Uso un cursore nella dpa_amministra per prevedere una futura multiamministrazione. 
BEGIN
	DECLARE
	  @v_spedizione_auto_doc  char(1),
	  @v_system_id  int
	DECLARE db_cursor CURSOR FOR  
	 SELECT system_id, spedizione_auto_doc FROM DPA_AMMINISTRA


	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @v_spedizione_auto_doc,  @v_system_id

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		  if @v_spedizione_auto_doc ='1'
			   update DPA_AMMINISTRA set DPA_AMMINISTRA.TRASMISSIONE_AUTO_DOC ='1' where DPA_AMMINISTRA.SYSTEM_ID = @v_system_id 
		  else
			   update DPA_AMMINISTRA set DPA_AMMINISTRA.TRASMISSIONE_AUTO_DOC ='0' where DPA_AMMINISTRA.SYSTEM_ID = @v_system_id 
		  end
	  FETCH NEXT FROM db_cursor INTO @v_spedizione_auto_doc,  @v_system_id
	END  

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
END