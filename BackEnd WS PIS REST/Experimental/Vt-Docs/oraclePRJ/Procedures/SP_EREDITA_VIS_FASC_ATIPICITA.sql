--------------------------------------------------------
--  DDL for Procedure SP_EREDITA_VIS_FASC_ATIPICITA
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."SP_EREDITA_VIS_FASC_ATIPICITA" (
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
declare ins_security varchar2(1000):= 'INSERT INTO security(thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto) ';

--DICHIARAZIONE DELLA SELECT PER L'INSERIMENTO CHE VALE PER TUTTE LE INSERT DELLA STORED
select_for_ins varchar2(1000):=
'SELECT /*+ index(s) index(p) */
DISTINCT s.thing,
'||idgruppo||',
(CASE WHEN ( s.accessrights = 255 AND ( s.cha_tipo_diritto = ''P'' OR s.cha_tipo_diritto = ''A'' OR s.cha_tipo_diritto = ''F'' ) ) THEN 63 ELSE s.accessrights END ) AS acr,
NULL,
''A''
FROM security s, project p ';
          
--DICHIARAZIONE DELLA SELECT PER L'EVENTUALE STORED DI ATIPICITA' CHE VALE PER TUTTE LE PROCEDURE DI ATIPICITA EVENTUALMENTE LANCIATE NELLA STORED
select_for_ins_store varchar2(1000):= 'SELECT /*+ index(s) index(p) */ DISTINCT s.thing FROM security s, project p ';

id_amministrazione number;

  BEGIN
    returnvalue := 0;
    select id_amm into id_amministrazione from dpa_el_registri where system_id = idregistro;
    
      --ATTENZIONE QUESTA PRIMA INSERT E' UN CASO SPECFICO NON GESTITO CON LE VARIABILI SOPRA DEFINITE
      
      --DICHIARAZIONE SELECT SPECIFICCHE PER QUESTO PRIMA INSERT
      declare select_for_ins_1 varchar2(1000):='SELECT thing, personorgroup, accessrights, id_gruppo_trasm, cha_tipo_diritto FROM ';
      
      select_for_ins_2 varchar2(1000):=
      '(SELECT 
      /*+ index (a) */ 
      DISTINCT a.system_id AS thing, 
      '||idgruppo||' AS personorgroup, 
      255 AS accessrights,
      NULL AS id_gruppo_trasm,
      ''P'' AS cha_tipo_diritto
      FROM project a ';
       
      select_for_ins_3 varchar2(1000):= 
      '(SELECT
      /*+ index (b) */
      DISTINCT b.system_id AS thing,
      '||idgruppo||' AS personorgroup,
      255 AS accessrights,
      NULL AS id_gruppo_trasm,
      ''P'' AS cha_tipo_diritto
      FROM project b ';
        
      --DICHIARAZIONE SELECT PER L'EVENTUALE STORED DI ATIPICITA' SPECIFICHE PER QUESTA INSERT
      select_for_ins_atipicita_1 varchar2(1000):='SELECT thing FROM '; 
       
      select_for_ins_atipicita_2 varchar2(1000):='(SELECT /*+ index (a) */ DISTINCT a.system_id AS thing FROM project a ';
      
      select_for_ins_atipicita_3 varchar2(1000):='(SELECT /*+ index (b) */ DISTINCT b.system_id AS thing FROM project b ';
      
      --DICHIARAZIONE CONDIZIONI DELLA SELECT
      condizioni_select_1 varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=a.system_id)
        AND ( ( a.cha_tipo_proj  = ''T''
        AND ( a.id_registro      = '||idregistro||' OR a.id_registro IS NULL ) )
        OR ( a.cha_tipo_proj     = ''F''
        AND a.cha_tipo_fascicolo = ''G''
        AND ( a.id_registro      = '||idregistro||' OR a.id_registro IS NULL ) ) )
        '
        || atipicita ||      
        '
        )
      UNION ';
      
      condizioni_select_2 varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=b.system_id)
        AND b.cha_tipo_proj = ''C''
        '
        || atipicita ||      
        '
        AND id_parent      IN
          (SELECT
            /*+ index (project) */
            system_id
          FROM project
          WHERE cha_tipo_proj    = ''F''
          AND cha_tipo_fascicolo = ''G''
          AND ( id_registro      = '||idregistro||' OR id_registro IS NULL ))
        ) ';       
        
    BEGIN
      
      --INIZIO PRIMA INSERT       
       execute immediate(ins_security || select_for_ins_1 || select_for_ins_2 || condizioni_select_1 || select_for_ins_3 || condizioni_select_2);
      --FINE SECONDA INSERT
      
      --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      VIS_FASC_ANOMALA_CUSTOM(id_amministrazione,select_for_ins_atipicita_1 || select_for_ins_atipicita_2 || condizioni_select_1 || select_for_ins_atipicita_3 || condizioni_select_2);
      --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA 
    EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN
      NULL;
    WHEN OTHERS THEN
      returnvalue := 1;
      RETURN;
    END;
    IF parilivello = 0 THEN
      
        --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL SECONDO INSERIMENTO
        declare condizioni_select varchar2(10000):=
        'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';      
          
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';      
          
          BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
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
          
          --INIZIO SECONDA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE SECONDA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA      
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 2;
        RETURN;
      END;
    ELSE
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL TERZO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id )
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';
    
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id )
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo          = '||idcorrglobaleuo||'
          AND d.id_registro    = '||idregistro||'
          ) ';
      
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
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
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 3;
        RETURN;
      END;
    END IF;
    IF parilivello = 0 THEN
    
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUARTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
    
          condizioni_select_1 varchar2(10000):=
          'WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.cha_tipo_proj = ''C''
            AND b.id_parent      IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
    
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
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
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA      
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 4;
        RETURN;
      END;
    ELSE
      
      --DICHIARAZIONE CONDIZIONE DELLA SELECT PER IL QUINTO INSERIMENTO
      declare condizioni_select varchar2(10000):=
      ' WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
        AND p.system_id  = s.thing
        AND p.cha_privato  = ''0'' 
        '
        || atipicita ||      
        '
        AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.id_parent IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
          
          condizioni_select_1 varchar2(10000):=
          ' WHERE NOT EXISTS
          (SELECT ''x'' FROM security s1 WHERE s1.PERSONORGROUP='||idgruppo||' AND s1.THING=p.system_id)
          AND p.system_id  = s.thing
          AND p.cha_privato  = ''0'' 
          AND p.system_id IN
          (SELECT        *
          FROM
            (SELECT
              /*+ index (a ) */
              a.system_id
            FROM project a
            WHERE ( ( a.cha_tipo_proj = ''F''
            AND a.cha_tipo_fascicolo  = ''P'' )
            AND ( a.id_registro       = '||idregistro||' OR a.id_registro IS NULL ) )
            )
          UNION
            (SELECT
              /*+ index (b )  */
              b.system_id
            FROM project b
            WHERE b.id_parent IN
              (SELECT
                /*+ index (project) */
                system_id
              FROM project
              WHERE cha_tipo_proj    = ''F''
              AND cha_tipo_fascicolo = ''P''
              AND ( id_registro      = '||idregistro||' OR id_registro IS NULL )
              )
            )
          )
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
          AND b.id_uo         IN
            (SELECT system_id
            FROM dpa_corr_globali
            WHERE cha_tipo_ie            = ''I''
            AND dta_fine                IS NULL
            AND id_old                   = 0
              START WITH id_parent       = '||idcorrglobaleuo||'
              CONNECT BY PRIOR system_id = id_parent
            )
          ) ';
          
      BEGIN
          -- Se  stata richiesta l'esclusione dei fascicoli atipici, bisogna
          -- includere nell'estensione tutti quei fascicoli che sono atipici 
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
          
          --INIZIO QUINTA INSERT       
          execute immediate(ins_security || select_for_ins || condizioni_select);
          --FINE QUINTA INSERT
          
          --CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA
          VIS_FASC_ANOMALA_CUSTOM(id_amministrazione, select_for_ins_store||condizioni_select);
          --FINE CACOLCO ATIPICITA CONDIZIONATO ALLA FUNZIONALITA' ABILITATA           
      EXCEPTION
      WHEN DUP_VAL_ON_INDEX THEN
        NULL;
      WHEN OTHERS THEN
        returnvalue := 5;
        RETURN;
      END;
    END IF;
  END;
END;              
----------- FINE -
              
---- sp_rename_column.ORA.sql  marcatore per ricerca ----
-- es. invocazione:
-- exec rename_column('VERSIONE CD', --->es. 3.16.1
--                    'NOME UTENTE', --->es. DOCSADM
--                    'NOME TABELLA', --->es. DPA_LOG
--                    'NOME COLONNA', --->es. COLONNA_A
--                    'NOME COLONNA NUOVA', --->es. COLONNA_B
--                    'RFU' ---> per uso futuro")
-- =============================================
-- Author:        Gabriele Serpi
-- Create date: 21/07/2011
-- Description:    
-- ============================================= 

/
