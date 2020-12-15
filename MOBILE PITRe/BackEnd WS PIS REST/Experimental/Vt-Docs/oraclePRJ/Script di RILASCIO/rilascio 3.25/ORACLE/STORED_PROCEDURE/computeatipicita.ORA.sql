begin 
Utl_Backup_Plsql_code ('PROCEDURE','computeatipicita'); 
end;
/

create or replace
PROCEDURE computeatipicita(
    -- Id della UO in cui ? inserito il ruolo
    iduo IN NUMBER,
    -- Id del gruppo per cui calcolare l'atipicit?
    idgroup IN VARCHAR2
    -- Id dell'ammionistrazione
    ,
    idamm IN VARCHAR2
    -- Id del tipo ruolo
    ,
    idtiporuolo NUMBER
    -- Id del veccho tipo ruolo
    ,
    idtiporuolovecchio NUMBER
    -- Id della vecchia UO
    ,
    idvecchiauo NUMBER
    -- 1 se ? stato richiesto di calcolare l'atipicit? sui sottoposti
    ,
    calcolasuisottoposti NUMBER
    -- Valore restitua
    ,
    returnvalue OUT INTEGER )
AS
BEGIN
  /******************************************************************************
  AUTHOR:   Samuele Furnari
  NAME:     COMPUTEATIPICITA
  PURPOSE:  Store per il calcolo dell'atipicit? di documenti e fascicoli di un ruolo
  ATTENZIONE! Questa procedura non deve essere utilizzata per calcolare
  l'atipicit? di un ruolo appena inserito. Per il calcolo di atipicit?
  su ruoli appena inseriti utilizzare la store COMPUTEATIPICITAINSROLE
  ******************************************************************************/
  -- Livello del ruolo prima e dopo la modifica
  DECLARE
    oldlevel NUMBER;
    newlevel NUMBER;
    -- Calcolo della atipicit? su documenti e fascicoli visti dal ruolo, solo se
    -- attiva per l'amministrazione o per l'installazione e se richiesto
    keyvalue VARCHAR (128);
    -- Query custom da eseguire per il calcolo degli id degli oggetti sui cui calcolare l'atipicit?
    querycustomdoc  VARCHAR(2000) := '';
    querycustomfasc VARCHAR(2000) := '';
  BEGIN
    -- Recupero dello stato di abilitazione del calcolo di atipicit?
    SELECT var_valore
    INTO keyvalue
    FROM dpa_chiavi_configurazione
    WHERE var_codice = 'ATIPICITA_DOC_FASC'
    AND (id_amm      = 0
    OR id_amm        = idamm)
    AND ROWNUM       = 1;
    -- Recupero dei livelli del ruolo prima e dopo la modifica
    SELECT num_livello
    INTO oldlevel
    FROM dpa_tipo_ruolo
    WHERE system_id = idtiporuolovecchio;
    SELECT num_livello
    INTO newlevel
    FROM dpa_tipo_ruolo
    WHERE system_id = idtiporuolo;
    IF keyvalue     = '1' THEN
      BEGIN
        -- Calcolo dell'atipicit? per gli oggetti del ruolo che ha subito modifiche
        querycustomdoc  := 'Select p.system_id                              

From security s                              

Inner Join profile p                              

On p.system_id = s.thing                              

Where personorgroup = ' || idgroup || ' And accessrights > 20                              

And p.cha_tipo_proto In (''A'',''P'',''I'',''G'')                              

And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'')';
        querycustomfasc := 'Select p.system_id                            

From security s                            

Inner Join project p                            

On p.system_id = s.thing                            

Where personorgroup = ' || idgroup || ' And accessrights > 20                            

And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )                            

AND p.cha_tipo_fascicolo = ''P''';
        -- Esecuzione del calcolo dell'atipicit?
        vis_doc_anomala_custom(idamm, querycustomdoc);
        vis_fasc_anomala_custom(idamm, querycustomfasc);
        -- Pulizia delle query
        querycustomdoc  := '';
        querycustomfasc := '';
        -- Se c'? stata discesa gerarchica, viene calcolata l'atipicit? sui superiori
        -- che prima della modifica erano sottoposti
        IF newlevel > oldlevel THEN
          BEGIN
            querycustomdoc := ' Select Distinct(s.thing)                                

From security s                                

Inner Join profile p                                

On p.system_id = s.thing                                

Where personorgroup In (                                  

Select Distinct(p.id_gruppo)

From dpa_corr_globali p                                  

Where id_amm = ' || idamm || '                                  

And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldlevel || '                                  

And (Select num_livello from dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) <= ' || newlevel || '                                  

And id_gruppo != ' || idgroup ||
            '                                  

And p.id_uo In (                                    

Select p.SYSTEM_ID                                    

From dpa_corr_globali p                                    

Start With p.SYSTEM_ID = ' || iduo || '                                    

Connect By Prior                                    

p.ID_PARENT = p.SYSTEM_ID And                                    

p.CHA_TIPO_URP = ''U'' And                                    

p.ID_AMM = ' || idamm || '                                  

))                                  

And s.accessrights > 20                                  

And p.cha_tipo_proto in (''A'',''P'',''I'',''G'')                                  

And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )';
            querycustomfasc := '  Select Distinct(s.thing)                                  

From security s                                  

Inner Join project p                                  

On p.system_id = s.thing                                  

Where personorgroup In (                                    

Select Distinct(p.id_gruppo)                                    

From dpa_corr_globali p                                    

Where id_amm = ' || idamm || 'And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldlevel || '                                    

And (Select num_livello from dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) <= ' || newlevel || '                                    

And id_gruppo != ' || idgroup ||
            '                                    

And p.id_uo In (                                      

Select p.SYSTEM_ID                                      

From dpa_corr_globali p                                      

Start With p.SYSTEM_ID = ' || iduo || '                                      

Connect By Prior                                      

p.ID_PARENT = p.SYSTEM_ID And                                      

p.CHA_TIPO_URP = ''U'' And                                      

p.ID_AMM = ' || idamm || '                                      

))                                  

AND s.accessrights > 20                                  

And p.cha_tipo_fascicolo = ''P''';
          END;
        ELSE
          -- Altrimenti se c'? stata salita gerarchica o se c'? stato spostamento
          -- ed ? stato richiesto il calcolo dell'atipicit?, viene calcolata
          -- l'atipicit? sui sottoposti
          IF newlevel < oldlevel OR (iduo != idvecchiauo AND calcolasuisottoposti = 1) THEN
            BEGIN
              Querycustomdoc := ' select /*+ index(s)*/ distinct(s.thing)                                    

from security s                                    

inner join profile p                                    

on p.system_id = s.thing                                    

where personorgroup in (                                      

select /*+ index(p)*/ distinct(p.id_gruppo)                                      

from dpa_corr_globali p                                      

where id_amm = ' || idamm || ' AND                                      

p.id_uo in (                                        

select /*+ index(p)*/ p.SYSTEM_ID                                        

from dpa_corr_globali p                                        

start with p.SYSTEM_ID = ' || iduo ||
              ' connect by prior                                        

p.SYSTEM_ID = p.ID_PARENT AND                                        

p.CHA_TIPO_URP = ''U'' AND                                        

p.ID_AMM = ' || idamm || ') minus select ' || idgroup || ' from dual)                                        

AND s.accessrights > 20                                        

AND p.cha_tipo_proto in (''A'',''P'',''I'',''G'')                                        

AND (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )';
              --And s.personorgroup != ' || idGroup;
              querycustomfasc := 'select /*+ index(s)*/ distinct(s.thing)                                    

from security s                                    

inner join project p                                    

on p.system_id = s.thing                                    

where personorgroup in (                                      

select /*+ index(p)*/ distinct(p.id_gruppo)                                      

from dpa_corr_globali p                                      

where id_amm = ' || idamm || ' AND                                      

p.id_uo in (                                        

select /*+ index(p)*/ p.SYSTEM_ID                                        

from dpa_corr_globali p                                        

start with p.SYSTEM_ID = ' || iduo ||
              'connect by prior                                        

p.SYSTEM_ID = p.ID_PARENT AND                                        

p.CHA_TIPO_URP = ''U'' AND                                        

p.ID_AMM = ' || idamm || ')                                        

AND s.accessrights > 20                                        

and p.cha_tipo_fascicolo = ''P''                                        

minus select ' || idgroup || ' from dual)';
              --) And s.personorgroup !=  || idGroup;
              -- Esecuzione del calcolo dell'atipicit?
              vis_doc_anomala_custom(idamm, querycustomdoc);
              vis_fasc_anomala_custom(idamm, querycustomfasc);
            END;
          END IF;
        END IF;
        -- Se ? stato compiuto uno spostamento viene ricalcolata l'atipicit? anche
        -- sui sottoposti del ruolo nella catena di origine
        IF iduo != idvecchiauo THEN
          BEGIN
            vis_doc_anomala_custom(idamm,'Select Distinct(s.thing)                                        

From security s                                        

Inner Join profile p                                        

On p.system_id = s.thing                                        

Where personorgroup In (                                          

Select Distinct(p.id_gruppo)                                          

From dpa_corr_globali p                                          

Where id_amm = ' || idamm || '                                          

And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldlevel || '

And p.id_uo In (                                            

Select p.SYSTEM_ID                                            

From dpa_corr_globali p                                            

Start With p.SYSTEM_ID = ' || idvecchiauo ||
            '                                            

Connect By Prior                                            

p.system_id = p.id_parent And                                            

p.CHA_TIPO_URP = ''U'' And                                            

p.ID_AMM = ' || idamm || '                                          

))                                          

And s.accessrights > 20                                          

And p.cha_tipo_proto in (''A'',''P'',''I'',''G'')                                          

And (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''T'' )');
            vis_fasc_anomala_custom(idamm, '  Select Distinct(s.thing)                                            

From security s                                            

Inner Join project p                                            

On p.system_id = s.thing                                            

Where personorgroup In (                                              

Select Distinct(p.id_gruppo)                                              

From dpa_corr_globali p                                              

Where id_amm = ' || idamm || '                                              

And (Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= ' || oldlevel || '                                              

And p.id_uo In (                                                

Select p.SYSTEM_ID                                                

From dpa_corr_globali p                                                

Start With p.SYSTEM_ID = ' ||
            idvecchiauo || '                                                

Connect By Prior                                                

p.system_id = p.id_parent And                                                

p.CHA_TIPO_URP = ''U'' And                                                

p.ID_AMM = ' || idamm || '                                              

))                                            

And s.accessrights > 20                                            

And p.cha_tipo_fascicolo = ''P''');
          END;
        END IF;
      END;
    END IF;
    returnvalue := 0;
  END;
END computeatipicita;
/
