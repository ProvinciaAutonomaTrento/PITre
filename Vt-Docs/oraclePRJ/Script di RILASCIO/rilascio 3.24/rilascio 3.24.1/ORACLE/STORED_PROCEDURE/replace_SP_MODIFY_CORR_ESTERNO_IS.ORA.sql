--begin 	utl_backup_sp ('SP_MODIFY_CORR_ESTERNO_IS','3.24');end;
--/

begin 
	Utl_Backup_Plsql_code ('PROCEDURE','SP_MODIFY_CORR_ESTERNO_IS'); 
end;
/


create or replace PROCEDURE                  SP_MODIFY_CORR_ESTERNO_IS (
   idcorrglobale     IN       NUMBER,
   desc_corr         IN       VARCHAR2,
   nome              IN       VARCHAR2,
   cognome           IN       VARCHAR2,
   codice_aoo        IN       VARCHAR2,
   codice_amm        IN       VARCHAR2,
   email             IN       VARCHAR2,
   indirizzo         IN       VARCHAR2,
   cap               IN       VARCHAR2,
   provincia         IN       VARCHAR2,
   nazione           IN       VARCHAR2,
   citta             IN       VARCHAR2,
   cod_fiscale       IN       VARCHAR2,
   partita_iva       IN       VARCHAR2,
   telefono          IN       VARCHAR2,
   telefono2         IN       VARCHAR2,
   note              IN       VARCHAR2,
   fax               IN       VARCHAR2,
   var_iddoctype     IN       NUMBER,
   inrubricacomune   IN       CHAR,
   tipourp           IN       CHAR,
   localita          IN       VARCHAR2,
   luogoNascita      IN       VARCHAR2,
   dataNascita       IN       VARCHAR2,
   titolo            IN       VARCHAR2,
   SimpInteropUrl    IN       VARCHAR2,
   IdPeople          IN       NUMBER,
   IdGruppoPeople    IN       NUMBER,
   newid             OUT      NUMBER,  
   returnvalue       OUT      NUMBER
)
Is
-- BEGIN   DECLARE
      cnt   integer; 
      cod_rubrica              VARCHAR2 (128);
      id_reg                   NUMBER;
      idamm                    NUMBER;
      new_var_cod_rubrica      VARCHAR2 (128);
      new_var_cod_rubrica1     VARCHAR2 (128);
      cha_dettaglio            CHAR (1):= '0';
      cha_tipourp              CHAR (1);
      myprofile                NUMBER;
      new_idcorrglobale        NUMBER;
      identitydettglobali      NUMBER;
      outvalue                 NUMBER         := 1;
      rtn                      NUMBER;
      v_id_doctype             NUMBER;
      identitydpatcanalecorr   NUMBER;
      chaTipoIE                CHAR (1);
      numLivello               NUMBER          := 0;
      idParent                 NUMBER;
      idPesoOrg                NUMBER;
      idUO                     NUMBER;
      idGruppoOld              NUMBER;
      idTipoRuolo              NUMBER;
      cha_tipo_corr            CHAR (1);
      chapa                    CHAR (1);
      var_desc_old             VARCHAR2(256);
      v_curdoc_docnumber       NUMBER;
      v_curdoc_idtemplate      NUMBER;
      v_curdoc_idoggetto       NUMBER;
      v_curfasc_idproject      NUMBER;
      v_curfasc_idtemplate     NUMBER;
      V_Curfasc_Idoggetto      Number;
      Countprofilato           Number     := 0;
      Countprofdoc             Number     := 0;
      countProfFasc            number     := 0;
      IdRuolo                  number;
      url                      varchar2(4000);

    CURSOR cursor_doc
   IS
      select at.doc_number, at.id_template, at.id_oggetto 
      from dpa_associazione_templates at, dpa_oggetti_custom oc, dpa_tipo_oggetto dto 
      where at.valore_oggetto_db = to_char(idcorrglobale)
      and at.ID_OGGETTO = oc.SYSTEM_ID
      and oc.ID_TIPO_OGGETTO = dto.SYSTEM_ID
      and upper(dto.DESCRIZIONE) = 'CORRISPONDENTE';

CURSOR cursor_fasc
   IS
      select atf.id_project, atf.id_template, atf.id_oggetto 
      from dpa_ass_templates_fasc atf, dpa_oggetti_custom_fasc ocf, dpa_tipo_oggetto_fasc tof
      where atf.valore_oggetto_db = to_char(idcorrglobale)
      and atf.ID_OGGETTO = ocf.SYSTEM_ID
      and ocf.ID_TIPO_OGGETTO = tof.SYSTEM_ID
      and upper(tof.DESCRIZIONE) = 'CORRISPONDENTE';

Begin
--dbms_output.put_line(IdGruppoPeople) ;
    <<reperimento_dati>>
    BEGIN
         SELECT var_cod_rubrica, cha_tipo_urp, id_registro, id_amm,cha_pa, cha_tipo_ie, num_livello, id_parent, id_peso_org, id_uo, id_tipo_ruolo, id_gruppo,var_desc_corr_old
           INTO cod_rubrica, cha_tipourp, id_reg, idamm, chapa, chaTipoIE, numLivello, idParent, idPesoOrg, IdUO, idTipoRuolo, idGruppoOld, var_desc_old
           FROM dpa_corr_globali
          WHERE system_id = idcorrglobale;
         
         Select System_Id Into Idruolo 
         from dpa_corr_globali where id_gruppo = IdGruppoPeople;
      
    If(Tipourp Is Not Null And Cha_Tipourp Is Not Null And Cha_Tipourp!=Tipourp) Then
    -- forzo il tipo_urp = al parametro passato alla store; input parameter supercedes the actual value
        cha_tipourp := tipourp;
    end if;

    If  Cha_Tipourp = 'U' Or Cha_Tipourp = 'P'          Or Cha_Tipourp = 'F' 
    THEN            Cha_Dettaglio := '1';
    End If;                                                       

    dbms_output.put_line('select effettuata') ; 
    EXCEPTION
         WHEN NO_DATA_FOUND          THEN
         dbms_output.put_line('primo blocco eccezione') ; 
            outvalue := 0;
            RETURN;
    END reperimento_dati;
      
 
      <<dati_canale_utente>>
    if cha_tipourp = 'P'       THEN
    BEGIN
         SELECT id_documenttype            INTO v_id_doctype
           FROM dpa_t_canale_corr          WHERE id_corr_globale = idcorrglobale;
    EXCEPTION
         WHEN NO_DATA_FOUND         THEN
            dbms_output.put_line('2do blocco eccezione') ; 
            outvalue := 2;
    END dati_canale_utente;
    end if;

--controlli preliminari
--verifico se il corrisp è stato utilizzato come dest/mitt di protocolli
    SELECT COUNT (id_profile)
      INTO myprofile
    FROM dpa_doc_arrivo_par
    Where Id_Mitt_Dest = Idcorrglobale;
-- verifico se il corrispondente é stato usato o meno nei campi profilati
    Select Count(System_Id) 
      Into Countprofdoc 
    From Dpa_Associazione_Templates 
    where valore_oggetto_db = to_char(IDCorrGlobale);
          
    Select Count(System_Id) 
      Into Countproffasc 
    from dpa_ass_templates_fasc
    where valore_oggetto_db = to_char(IdCorrGlobale);
          
    Countprofilato := Countprofdoc + Countproffasc;
--FINE controlli preliminari

          
-- 1) il corripondente non è stato mai utilizzato in un protocollo, con o senza campi profilati 
IF /* 1 */ (myprofile = 0 AND Countprofilato = 0)  -- non occorre storicizzare il corrispondente
    Then
    
    BEGIN
      UPDATE dpa_t_canale_corr
        SET id_documenttype = var_iddoctype
      WHERE id_corr_globale = idcorrglobale;
    EXCEPTION
    WHEN OTHERS  THEN
      dbms_output.put_line('5o blocco eccezione') ; 
      outvalue := 5;
      RETURN;
    End;
         
    Begin
      UPDATE dpa_corr_globali
        SET var_codice_aoo = codice_aoo,
        var_codice_amm = codice_amm,
        var_email = email,
        var_desc_corr = desc_corr,
        var_nome = nome,
        var_cognome = cognome,
        cha_pa=chapa,
        cha_tipo_urp=cha_tipourp,
        InteropUrl = SimpInteropUrl
    WHERE system_id = idcorrglobale;
    EXCEPTION
    WHEN OTHERS THEN
                 dbms_output.put_line('3o blocco eccezione') ; 
                 Outvalue := 3;
    RETURN;
    End;
    
    /* SE L'UPDATE SU DPA_CORR_GLOBALI è ANDATA A BUON FINE, PER UTENTI E UO DEVO AGGIORNARE IL RECORD SULLA DPA_DETT_GLOBALI                    */
    IF /* 1a */ cha_tipourp = 'U' OR cha_tipourp = 'P'  Or Cha_Tipourp = 'F' 
    Then
    <<update_dpa_dett_globali>>
        BEGIN
             SELECT count(*) into cnt
             FROM dpa_dett_globali
             WHERE id_corr_globali = idcorrglobale ; 
                          
             If (Cnt = 0)          Then 
             dbms_output.put_line('sono nella INSERT in dpa_dett_globali con id_corr_globali =  '||idcorrglobale) ; 
               Insert Into Dpa_Dett_Globali 
                (System_Id,Id_Corr_Globali, Var_Indirizzo 
                ,Var_Cap  ,Var_Provincia , Var_Nazione 
                , Var_Cod_Fisc, Var_cod_pi ,Var_Telefono , Var_Telefono2 
                ,Var_Note   ,Var_Citta,Var_Fax 
                ,Var_Localita,Var_Luogo_Nascita
                ,dta_nascita,var_titolo)
               Values 
               (Seq.Nextval,Idcorrglobale, Indirizzo
               ,Cap,Provincia,Nazione
               ,Cod_Fiscale, partita_iva        ,Telefono,Telefono2
               ,Note         ,Citta,Fax
               ,Localita,Luogonascita
               ,Datanascita,Titolo);
             END IF;
             
             If (Cnt = 1)
             Then    
             dbms_output.put_line('sono nella UPDATE di dpa_dett_globali') ; 
              UPDATE dpa_dett_globali SET
              var_indirizzo = indirizzo,
              var_cap = cap,
              var_provincia = provincia,
              Var_Nazione = Nazione,
              var_cod_fisc = cod_fiscale,
              var_cod_pi = partita_iva,
              var_telefono = telefono,
              var_telefono2 = telefono2,
              var_note = note,
              var_citta = citta,
              var_fax = fax,
              var_localita = localita,
              var_luogo_nascita = luogoNascita,
              dta_nascita = dataNascita,
              var_titolo = titolo
              WHERE (id_corr_globali = idcorrglobale) ; 
             END IF;
             dbms_output.put_line('sono nella merge') ; 
                Exception
                WHEN OTHERS THEN
                Dbms_Output.Put_Line('4o blocco eccezione'||Sqlerrm) ;                             
                Outvalue := 4;
                RETURN;
             End Update_Dpa_Dett_Globali;
    End If;  /* 1a */

                              
Else -- occorre storicizzare il corrispondente 
-- ramo else /* 1 */ (myprofile = 0 AND Countprofilato = 0)  
-- perché il corrisp è stato utilizzato in un protocollo

-- storicizzo il corrispondente
-- Ricavo il codice rubrica del corrispondente
   Select Var_Cod_Rubrica                    
    INTO new_var_cod_rubrica1
   FROM dpa_corr_globali
   WHERE system_id = IDCorrGlobale;

-- Costruisco il codice rubrica da attribuire la corrispondente storicizzato
new_var_cod_rubrica1 := cod_rubrica || '_' || TO_CHAR (idcorrglobale);

    UPDATE DPA_CORR_GLOBALI
      SET DTA_FINE = SYSDATE(),
                    var_cod_rubrica = new_var_cod_rubrica1,
                    var_codice      = new_var_cod_rubrica1,
                    Id_Parent = Null
     WHERE SYSTEM_ID = IDCorrGlobale;
            
                    <<inserimento_nuovo_corrisp>>
                    BEGIN
                    SELECT seq.NEXTVAL
                    INTO newid
                    FROM DUAL;

                        If (Inrubricacomune = '1')                        
                        THEN
                            cha_tipo_corr := 'C';
                        ELSE
                            cha_tipo_corr := 'S';
                        END IF;

                        INSERT INTO dpa_corr_globali
                           (system_id, num_livello, cha_tipo_ie, id_registro,
                            id_amm, var_desc_corr, var_nome, var_cognome,
                            id_old, dta_inizio, id_parent, var_codice,
                            cha_tipo_corr, cha_tipo_urp,
                            var_codice_aoo, var_cod_rubrica, cha_dettagli,
                            var_email, var_codice_amm, cha_pa, id_peso_org,
                            id_gruppo, id_tipo_ruolo, id_uo, var_desc_corr_old, InteropUrl
                           )
                        VALUES (newid, numLivello, chaTipoIE, id_reg,
                            idamm, desc_corr, nome, cognome,
                            idcorrglobale, SYSDATE, idParent, cod_rubrica,
                            cha_tipo_corr, cha_tipourp,
                            codice_aoo, cod_rubrica, cha_dettaglio,
                            email, codice_amm, chapa, idPesoOrg,
                            idGruppoOld, idTipoRuolo, idUO, var_desc_old , SimpInteropUrl
                           );
                        EXCEPTION
                        WHEN OTHERS
                        THEN
                            dbms_output.put_line('7o blocco eccezione') ; 
                            outvalue := 7;
                            RETURN;
                    END inserimento_nuovo_corrisp;

--INSERISCO IL CANALE PREFERITO DEL NUOVO CORRISP ESTERNO SIA ESSO UO, RUOLO, PERSONA
    <<inserimento_dpa_t_canale_corr>>
    Begin
               --SELECT seq.NEXTVAL INTO identitydpatcanalecorr FROM DUAL;
        
      Insert Into Dpa_T_Canale_Corr                           
      (system_id, id_corr_globale, id_documenttype,cha_preferito)
      Values (seq.NEXTVAL, Newid, Var_Iddoctype,'1');
               
    Exception
     WHEN OTHERS THEN
                  dbms_output.put_line('9o blocco eccezione') ; 
                  outvalue := 9;
                  Return;
    END inserimento_dpa_t_canale_corr;
    

        /* DOPO L'INSERIMENTO DEL NUOVO CORRISPONDENTE POSSO INSERIRE IL
        RELATIVO RECORD NELLA DPA_DETT_GLOBALI, MA SOLO PER CORRISPONDENTI UTENTI
        E UNITA ORGANIZZATIVE */
            IF /* 4 */ cha_tipourp = 'U' OR cha_tipourp = 'P' Or Cha_Tipourp = 'F'
            THEN
               SELECT seq.NEXTVAL
                 INTO identitydettglobali
                 FROM DUAL;

               <<inserimento_dettaglio_corrisp>>
               BEGIN
                  INSERT INTO dpa_dett_globali
                              (system_id, id_corr_globali, var_indirizzo,
                               var_cap, var_provincia, var_nazione,
                               var_cod_fisc, var_cod_pi, var_telefono, var_telefono2,
                               var_note, var_citta, var_fax, var_localita, var_luogo_nascita, dta_nascita, var_titolo
                              )
                       VALUES (identitydettglobali, newid, indirizzo,
                               cap, provincia, nazione,
                               cod_fiscale, partita_iva, telefono, telefono2,
                               note, citta, fax, localita, luogoNascita, dataNascita, titolo
                              );
               EXCEPTION
                  WHEN OTHERS
                  THEN
                     dbms_output.put_line('8o blocco eccezione') ; 
                     outvalue := 8;
                     RETURN;
               END inserimento_dettaglio_corrisp;
            End If;   /* 4 */
     
     /* 2 */
 
  --                dbms_output.put_line('countProfDoc ' || countProfDoc);
  --                dbms_output.put_line('countProfFasc ' || countProfFasc);        
            
        IF(countProfDoc > 0) THEN
        BEGIN
                        OPEN cursor_doc;
                        LOOP
                           FETCH cursor_doc
                            INTO v_curdoc_docnumber, v_curdoc_idtemplate, v_curdoc_idoggetto;
                         EXIT WHEN cursor_doc%NOTFOUND;
                            
                          --      dbms_output.put_line('v_curdoc_docnumber ' || v_curdoc_docnumber);                                dbms_output.put_line('v_curdoc_idtemplate ' || v_curdoc_idtemplate);                                        dbms_output.put_line('v_curdoc_idoggetto ' || v_curdoc_idoggetto);
                                INSERT INTO dpa_profil_sto
                                    (systemid, id_template,
                                    dta_modifica, id_profile, id_ogg_custom,
                                    id_people, id_ruolo_in_uo, var_desc_modifica
                                    )
                                VALUES (seq_dpa_profil_sto.NEXTVAL, v_curdoc_idtemplate,
                                    SYSDATE, v_curdoc_docnumber, v_curdoc_idoggetto,
                                    IdPeople, IdRuolo, 'Corrispondente storicizzato per modifica da rubrica'
                                    );

                        END LOOP;
                        CLOSE cursor_doc;
         End;
         End If;            -- FINE          IF(countProfDoc > 0) THEN
         
         IF(countProfFasc > 0) THEN
         BEGIN
            
                   OPEN cursor_fasc;
                   LOOP
                
                   FETCH cursor_fasc
                    INTO v_curfasc_idproject, v_curfasc_idtemplate, v_curfasc_idoggetto;

                    EXIT WHEN cursor_fasc%NOTFOUND;

                   --             dbms_output.put_line('v_curfasc_idproject ' || v_curfasc_idproject);dbms_output.put_line('v_curfasc_idtemplate ' || v_curfasc_idtemplate);        
                                dbms_output.put_line('v_curfasc_idoggetto ' || v_curfasc_idoggetto);

                                INSERT INTO dpa_profil_fasc_sto
                                    (systemid, id_template,
                                    dta_modifica, id_project, id_ogg_custom,
                                    id_people, id_ruolo_in_uo, var_desc_modifica
                                    )
                                VALUES (seq_dpa_profil_sto.NEXTVAL, v_curfasc_idtemplate,
                                    SYSDATE, v_curfasc_idproject, v_curfasc_idoggetto,
                                    IdPeople, IdRuolo, 'Corrispondente storicizzato per modifica da rubrica'
                                    );

                            
                        End Loop;
                        Close Cursor_Fasc;
         End;
         END IF; -- FINE IF(countProfFasc > 0) 
          
End If;   

--se fa parte di una lista, allora la devo aggiornare
if newid IS NOT NULL 
THEN 
    Update Dpa_Liste_Distr D 
      Set D.Id_Dpa_Corr=Newid 
    where d.ID_DPA_CORR=idcorrglobale;
end if;

Outvalue:=1;   -- CAS0 4.1.1- la disabilitazione VA a buon fine  (VALORE RITORNATO = 7).
Returnvalue := Outvalue;            
              

EXCEPTION
When Others Then
    dbms_output.put_line('errore ' || sqlerrm);
   Outvalue:=10;-- CAS0 4.1.1- la disabilitazione NON va a buon fine  (VALORE RITORNATO = 5).
   Returnvalue := Outvalue;            
   RETURN;
           
End SP_MODIFY_CORR_ESTERNO_IS ;
/
