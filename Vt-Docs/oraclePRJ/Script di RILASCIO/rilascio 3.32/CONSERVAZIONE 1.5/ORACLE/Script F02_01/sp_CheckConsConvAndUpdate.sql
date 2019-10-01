CREATE OR REPLACE PROCEDURE sp_CheckConsConvAndUpdate (
   p_id_istanza                           NUMBER,
   p_doc_number                           NUMBER,
   p_newVersionId                         NUMBER,
   returnvalue                       OUT  NUMBER
  
)
IS
    estenzioneNuovaVersione    VARCHAR(5);
    numDocReportDaConv         NUMBER;
    numDocReportConvertiti     NUMBER;
    numDocReportInErrore       NUMBER;
    
    
BEGIN
DBMS_OUTPUT.put_line ('inizio stored');
       
      
   BEGIN
        DBMS_OUTPUT.put_line ('Controllo conversione documento in PDF');
        returnvalue := 1;
        
        select c.EXT 
        into estenzioneNuovaVersione
        from components c
        where c.docnumber = p_doc_number 
        AND c.version_id = p_newVersionId;
        
       DBMS_OUTPUT.put_line ('Aggiornamento stato documento del report EXT ' || estenzioneNuovaVersione);
        
        IF (estenzioneNuovaVersione = 'pdf')
         THEN
         
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
            
            UPDATE dpa_items_conservazione
            SET    VAR_TIPO_FILE = '.pdf'
            WHERE  ID_CONSERVAZIONE = p_id_istanza
            AND    ID_PROFILE = p_doc_number;
            
            commit;
            --resultConv := '1';
            
            DBMS_OUTPUT.put_line ('Aggiornamento reportConservazione esito OK! Estensione documento: '|| estenzioneNuovaVersione);
          
          ELSE
          
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1',
                   ERRORE = '1',
                   TIPOERRORE = '2'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
    
            --resultConv := '1';
            commit;
            DBMS_OUTPUT.put_line ('Aggiornamento reportConservazione esito KO! Estensione documento:'|| estenzioneNuovaVersione);
        END IF;
      
  EXCEPTION
     WHEN OTHERS
     THEN 
            UPDATE dpa_verifica_formati_cons
            SET    CONVERTITO = '1',
                   ERRORE = '1',
                   TIPOERRORE = '2'
            WHERE  ID_ISTANZA = p_id_istanza
            AND    DOCNUMBER = p_doc_number;
        DBMS_OUTPUT.put_line ('ERRORE Controllo conversione documento in PDF');
        commit;
        --resultConv := 0;
       
  END;
   
   
   BEGIN
      --result := 0;

      SELECT    count(*) 
        INTO    numDocReportDaConv
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = 1;

      SELECT    count(*) 
        INTO    numDocReportConvertiti
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = '1'
        AND     rc.convertito = '1'
        AND     (rc.errore = 0 OR rc.errore is null);
              
      SELECT    count(*) 
        INTO    numDocReportInErrore
        FROM    dpa_verifica_formati_cons rc
        WHERE   rc.id_istanza = p_id_istanza
        AND     rc.daConvertire = '1'
        AND     rc.convertito = '1'
        AND     rc.errore = '1'
        AND     rc.tipoerrore = '2';
      
      DBMS_OUTPUT.put_line ('Controllo documenti istanza '|| p_id_istanza ||' ' || numDocReportDaConv ||' ' || numDocReportConvertiti);
      
      IF (numDocReportDaConv > 0 AND numDocReportDaConv = numDocReportConvertiti)
      THEN
          IF numDocReportInErrore > 0 
          THEN
            
            UPDATE dpa_area_conservazione
            SET cha_stato = 'Z' --ERRORE CONVERSIONE
            WHERE system_id = p_id_istanza;
            
            DBMS_OUTPUT.put_line ('stato in errore');
            commit;
          ELSE
          
            UPDATE dpa_area_conservazione
            SET cha_stato = 'I',
                data_invio = SYSDATE
            WHERE system_id = p_id_istanza;
            
            DBMS_OUTPUT.put_line ('stato Inviata');
            commit;
          END IF;
         
      
         
         
      END IF;

      EXCEPTION
         WHEN OTHERS
         THEN 
           UPDATE dpa_area_conservazione
            SET cha_stato = 'Z' --ERRORE CONVERSIONE
            WHERE system_id = p_id_istanza;
            commit;
            DBMS_OUTPUT.put_line ('eccezione: stato in errore');
      END;

    DBMS_OUTPUT.put_line ('fine stored');
END sp_CheckConsConvAndUpdate;
/
