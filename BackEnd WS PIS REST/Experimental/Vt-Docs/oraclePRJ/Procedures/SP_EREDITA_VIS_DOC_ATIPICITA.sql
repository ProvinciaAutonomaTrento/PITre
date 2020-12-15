--------------------------------------------------------
--  DDL for Procedure SP_EREDITA_VIS_DOC_ATIPICITA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_EREDITA_VIS_DOC_ATIPICITA" (
    idcorrglobaleuo    IN NUMBER,
    idcorrglobaleruolo IN NUMBER,
    idgruppo           IN NUMBER,
    livelloruolo       IN NUMBER,
    idregistro         IN NUMBER,
    parilivello        IN NUMBER,
    atipicita          IN VARCHAR,
    returnvalue OUT NUMBER )
IS
BEGIN

--DICHIARAZIONE DELLA STRINGA INSERT INTO SECURITY CHA VALE PER TUTTE LE INSERT DELLA STORED
declare ins_security varchar2(1000):= 'INSERT INTO security (thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto) ';

--DICHIARAZIONE DELLA SELECT PER L'INSERIMENTO CHE VALE PER TUTTE LE INSERT DELLA STORED
select_for_ins varchar2(1000):= 
'SELECT /*+ index(s) index(p) */ 
DISTINCT s.thing, 
'||idgruppo||',
(CASE WHEN (s.accessrights = 255 AND (s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''A'')) THEN 63 ELSE s.accessrights END) AS acr,
NULL,
''A''
FROM security s, PROFILE p ';

 --DICHIARAZIONE DELLA SELECT PER L'EVENTUALE STORED DI ATIPICITA' CHE VALE PER TUTTE LE PROCEDURE DI ATIPICITA EVENTUALMENTE LANCIATE NELLA STORED
select_for_ins_store varchar2(1000):= 'SELECT /*+ index(s) index(p) */ DISTINCT s.thing FROM security s, PROFILE p ';

id_amministrazione number;

  BEGIN
    returnvalue   := 0;
    select id_amm into id_amministrazione from dpa_el_registri where system_id = idregistro;
    
    IF parilivello = 0 THEN
      
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL PRIMO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id = s.thing
      AND p.cha_privato = ''0''
      '
      || atipicita ||      
      '
      AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a, dpa_corr_globali b, GROUPS c, dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = ''R''
        AND b.cha_tipo_ie    = ''I''
        AND b.dta_fine      IS NULL
        AND a.num_livello    > '||livelloruolo||'
        AND b.id_uo          = '||idcorrglobaleuo||'
        AND d.id_registro    = '||idregistro||'
        )
      AND NOT EXISTS
        (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
        
        
      condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id = s.thing
      AND p.cha_privato = ''0''
      AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
      AND s.personorgroup IN
        (SELECT c.system_id
        FROM dpa_tipo_ruolo a, dpa_corr_globali b, GROUPS c, dpa_l_ruolo_reg d
        WHERE a.system_id    = b.id_tipo_ruolo
        AND b.id_gruppo      = c.system_id
        AND d.id_ruolo_in_uo = b.system_id
        AND b.cha_tipo_urp   = ''R''
        AND b.cha_tipo_ie    = ''I''
        AND b.dta_fine      IS NULL
        AND a.num_livello    > '||livelloruolo||'
        AND b.id_uo          = '||idcorrglobaleuo||'
        AND d.id_registro    = '||idregistro||'
        )
      AND NOT EXISTS
        (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
      
      BEGIN
      
        -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
        -- includere nell'estensione tutti quei documenti che sono atipici 
        -- a causa della presenza del ruolo per cui si sta calcolando la
        -- visibilit
        If atipicita is not null And length(atipicita) > 2 Then
          Begin
            -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
            Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
            
            -- Calcolo atipicit
            VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);

            -- Riabilitazione ruolo
            Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
          End;
        End If;
      
        --INIZIO PRIMA INSERT
        execute immediate(ins_security || select_for_ins || condizioni_select);
        --FINE PRIMA INSERT
        
        --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
        VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
        --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA        
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    ELSE
      
        --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL SECONDO INSERIMENTO
        declare condizioni_select varchar2(10000):=
        'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          ( SELECT DISTINCT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '|| idgruppo ||' AND s1.thing = p.system_id) ';
        
        condizioni_select_1 varchar2(10000):=
        'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          ( SELECT DISTINCT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '|| idgruppo ||' AND s1.thing = p.system_id) ';
        
        BEGIN
          -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
          -- includere nell'estensione tutti quei documenti che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);

              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
        
        
          --INIZIO SECONDA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE SECONDA INSERT
          
          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    END IF;
    /* UO INFERIORI */
    IF parilivello = 0 THEN
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL TERZO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie  = ''I''
            AND dta_fine       IS NULL
            AND id_old         = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
      condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0'' 
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro    IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello    > '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie  = ''I''
            AND dta_fine       IS NULL
            AND id_old         = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
      
      BEGIN
          -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
          -- includere nell'estensione tutti quei documenti che sono atipici 
          -- a causa della presenza del ruolo per cui si sta calcolando la
          -- visibilit
          If atipicita is not null And length(atipicita) > 2 Then
            Begin
              -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
              Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
              
              -- Calcolo atipicit
              VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
              
              -- Riabilitazione ruolo
              Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
            End;
          End If;
      
         --INIZIO TERZA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE TERZA INSERT

          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    ELSE
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUARTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
         '
        || atipicita ||      
        '
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie = ''I''
            AND dta_fine      IS NULL
            AND id_old        = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
        condizioni_select_1 varchar2(10000):=
      'WHERE p.system_id    = s.thing
        AND p.cha_privato    = ''0''
        AND ( p.id_registro  = '||idregistro||' OR p.id_registro IS NULL )
        AND s.personorgroup IN
          (SELECT c.system_id
          FROM dpa_tipo_ruolo a,
            dpa_corr_globali b,
            GROUPS c,
            dpa_l_ruolo_reg d
          WHERE a.system_id    = b.id_tipo_ruolo
          AND b.id_gruppo      = c.system_id
          AND d.id_ruolo_in_uo = b.system_id
          AND b.cha_tipo_urp   = ''R''
          AND b.cha_tipo_ie    = ''I''
          AND b.dta_fine      IS NULL
          AND a.num_livello   >= '||livelloruolo||'
          AND d.id_registro    = '||idregistro||'
          AND b.id_uo IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie = ''I''
            AND dta_fine      IS NULL
            AND id_old        = 0
              START WITH id_parent = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          )
        AND NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.personorgroup = '||idgruppo||' AND s1.thing = p.system_id) ';
          
      BEGIN
        -- Se  stata richiesta l'esclusione dei documenti atipici, bisogna
        -- includere nell'estensione tutti quei documenti che sono atipici 
        -- a causa della presenza del ruolo per cui si sta calcolando la
        -- visibilit
        If atipicita is not null And length(atipicita) > 2 Then
          Begin
            -- DISABILITAZIONE RUOLO SU CUI CALCOLARE ESTENSIONE
            Update dpa_corr_globali Set dta_fine = sysdate Where system_id = idcorrglobaleruolo;
            
            -- Calcolo atipicit
            VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select_1);
            
            -- Riabilitazione ruolo
            Update dpa_corr_globali Set dta_fine = null Where system_id = idcorrglobaleruolo;
          End;
        End If;
      
        --INIZIO QUARTA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE QUARTA INSERT
          
          --CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_DOC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CALCOLO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 1;
        RETURN;
      END;
    END IF;
  END;
END; 

/
