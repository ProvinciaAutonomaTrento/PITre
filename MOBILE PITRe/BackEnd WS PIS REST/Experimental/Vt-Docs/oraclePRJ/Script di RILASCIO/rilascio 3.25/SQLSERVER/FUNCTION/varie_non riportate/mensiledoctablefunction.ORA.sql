begin 
Utl_Backup_Plsql_code ('FUNCTION','MENSILEDOCTABLEFUNCTION'); 
end;
/

create or replace
FUNCTION MENSILEDOCTABLEFUNCTION (
   mese      NUMBER,
   p_anno    NUMBER,
   id_reg    NUMBER,
   id_ammi   NUMBER,
   var_sed   VARCHAR
   ,   tit       INTEGER
)
   RETURN annualedoctablerow PIPELINED
IS
  

-- modifica APrile 2011 - uso di EXECUTE IMMEDIATE per migliorare la leggibilit del codice
istruzioneSQLbase varchar2(2000);
istruzioneSQL     varchar2(2000); 
condizione_sede   varchar2(200); 

 out_rec                 annualedoctabletype
      := annualedoctabletype (NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL,
                              NULL
                             );
--dichiarazioni variabili
   totanndoc               FLOAT;
   totannprot              FLOAT;
   percannprot             FLOAT;
   totannprota             FLOAT;
   percannprota            FLOAT;
   totannprotp             FLOAT;
   percannprotp            FLOAT;
   totannproti             FLOAT;
   percannproti            FLOAT;
   totanndocgrigi          FLOAT;
   percanndocgrigi         FLOAT;
   totanndocclass          FLOAT;
   percanndocclass         FLOAT;
   totanndocprof           FLOAT;
   totannprotclass         FLOAT;
   percannprotclass        FLOAT;
   totannprotaclass        FLOAT;
   percannprotaclass       FLOAT;
   totannprotpclass        FLOAT;
   percannprotpclass       FLOAT;
   totannproticlass        FLOAT;
   percannproticlass       FLOAT;
   totannprotannul         FLOAT;
   percannprotannul        FLOAT;
/******************************************************************/
/**************Dati Riepilogativi del Mese*****************************/
/******************************************************************/
/*Dati Generali*/
   totmondoc               FLOAT;
   totmonprot              FLOAT;
   totmonprota             FLOAT;
   totmonprotp             FLOAT;
   totmonproti             FLOAT;
   totmonprotannul         FLOAT;
   totmondocgrigi          FLOAT;
   totmondocclass          FLOAT;
/*docs senza docs acq*/
   totmondocprof           FLOAT;
   totmonprotclass         FLOAT;
   totmonprotaclass        FLOAT;
   totmonprotpclass        FLOAT;
   totmonproticlass        FLOAT;
/*Percentuali*/
   percmonprot             FLOAT;
   percmonprota            FLOAT;
   percmonprotp            FLOAT;
   percmonproti            FLOAT;
   percmonprotannul        FLOAT;
   percmondocgrigi         FLOAT;
   percmondocclass         FLOAT;
   percmonprotclass        FLOAT;
   percmonprotaclass       FLOAT;
   percmonprotpclass       FLOAT;
   percmonproticlass       FLOAT;
/*Dichiarazione delle variabili per i profili (Immagini) *************************************************************************************************/
/*Mensili*/
   totmonprof              FLOAT;
   totmonprofprot          FLOAT;
   totmonprofprota         FLOAT;
   totmonprofprotp         FLOAT;
   totmonprofproti         FLOAT;
   totmonprofgrigi         FLOAT;
   totmonprofprotannul     FLOAT;
   totmonprotannulclass    FLOAT;
   totmondocgrigiclass     FLOAT;
/*Annuali*/
   totannprof              FLOAT;
   totannprofprot          FLOAT;
   totannprofprota         FLOAT;
   totannprofprotp         FLOAT;
   totannprofproti         FLOAT;
   totannprofgrigi         FLOAT;
   totannprofprotannull    FLOAT;
/*Percentuali*/
   percannprofprot         FLOAT;
   percannprofprota        FLOAT;
   percannprofprotp        FLOAT;
   percannprofproti        FLOAT;
   percannprofgrigi        FLOAT;
   percannprofprotannull   FLOAT;
   totanndocgrigiclass     FLOAT;
   percanndocgrigiclass    FLOAT;
   totannprotannulclass    FLOAT;
   percannprotannulclass   FLOAT;
   mese_vc                 VARCHAR (255);
   --var_s                   VARCHAR (255);
   i                       NUMBER;
BEGIN
--verifica valore parametro var_sede

/*   IF (var_sed = '')
   THEN
      var_s := NULL;
   ELSE
      var_s := var_sed;
   END IF;
*/ 

/*Impostiamo i valori di default*/
/*Mensili*/
   totmonprof := 0;
   totmonprofprot := 0;
   totmonprofprota := 0;
   totmonprofprotp := 0;
   totmonprofproti := 0;
   totmonprofgrigi := 0;
   totmonprofprotannul := 0;
   totmonprotannulclass := 0;
   totmondocgrigiclass := 0;
/*Annuali*/
   totannprof := 0;
   totannprofprot := 0;
   totannprofprota := 0;
   totannprofprotp := 0;
   totannprofproti := 0;
   totannprofgrigi := 0;
/*Percentuali*/
   percannprofprot := 0;
   percannprofprota := 0;
   percannprofprotp := 0;
   percannprofproti := 0;
   percannprofgrigi := 0;
/**************************************************************************************************************************************************/
   percmonprot := 0;
   percmonprota := 0;
   percmonprotp := 0;
   percmonproti := 0;
   percmonprotannul := 0;
   percmondocgrigi := 0;
   percmondocclass := 0;
   percmonprotclass := 0;
   percmonprotaclass := 0;
   percmonprotpclass := 0;
   percmonproticlass := 0;
/******************************************************************/
   totanndoc := 0;
   totannprot := 0;
   totannprota := 0;
   totannprotp := 0;
   totannproti := 0;
   totanndocgrigi := 0;
   totanndocclass := 0;
   totanndocprof := 0;
   totannprotclass := 0;
   totannprotaclass := 0;
   totannprotpclass := 0;
   totannproticlass := 0;
   totannprotannul := 0;
   percannprot := 0;
   percannprota := 0;
   percannprotp := 0;
   percannproti := 0;
   percanndocgrigi := 0;
   percanndocclass := 0;
   percannprotclass := 0;
   percannprotaclass := 0;
   percannprotpclass := 0;
   percannproticlass := 0;
   percannprotannul := 0;
   totmonprofprot := 0;
   totmonprofprota := 0;
   totmonprofprotp := 0;
   totmonprofproti := 0;
   totmonprofgrigi := 0;
   totanndocgrigiclass := 0;
   percanndocgrigiclass := 0;
   totannprotannulclass := 0;
   percannprotannulclass := 0;
   totannprofprotannull := 0;
   percannprofprotannull := 0;
/*cicliamo dall'inizio dell'anno fino al mese di interesse*/

   --i := 0;

   --while (i < mese)
--loop
/*Incrementiamo il contatore*/
--i := i + 1;

   /*Query che recupera i dati del singolo mese*/
/*Totale dati del mese*/
/*Non filtriamo sul registro, questa query deve essere ripetuta per tutti i mesi di interesse per ogni registro*/
   i := mese;

   IF (var_sed IS NULL)
   THEN
        condizione_sede := ' AND p.var_sede IS NULL '; 
   ELSE
        condizione_sede := ' AND p.var_sede = '''||var_sed||''' ';
   END IF; 


--IF ((var_s != ' ') AND (var_s IS NOT NULL))    THEN

-- protocolli 

      /*SELECT COUNT (PROFILE.system_id)
        INTO totmonprota
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'A'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL
         AND PROFILE.var_sede = var_sed;
        */
         
  istruzioneSQLbase := ' 
    select count(p.system_id)  from profile p 
        where to_number(to_char(p.DTA_PROTO,''MM'')) = '||i||' 
        AND p.NUM_ANNO_PROTO = '||p_anno||'
        and nvl(p.CHA_IN_CESTINO,''0'') <> ''1'' 
      --AND p.cha_da_proto = ''0'' OBSOLETA
        and p.id_registro = '||id_reg||' '
        ||condizione_sede ;  
           
    istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''A'' AND p.dta_annulla is null 
                                           and NUM_PROTO is not null ';
    execute immediate istruzioneSQL into totMonProtA;          
         
      /*SELECT COUNT (PROFILE.system_id)
        INTO totmonprotp
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'P'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL
         AND PROFILE.var_sede = var_s;
         */

istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''P'' AND p.dta_annulla is null  
                                           and NUM_PROTO is not null ';
    execute immediate istruzioneSQL into totmonprotp;          
    

/*      SELECT COUNT (PROFILE.system_id)
        INTO totmonproti
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'I'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL
         AND PROFILE.var_sede = var_s;
*/
istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''I'' AND p.dta_annulla is null  
                                           and NUM_PROTO is not null ';
    execute immediate istruzioneSQL into totmonproti;          


/*      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotannul
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NOT NULL
         AND PROFILE.num_proto IS NOT NULL
         AND PROFILE.var_sede = var_s;
*/         
istruzioneSQL := istruzioneSQLbase || ' AND p.num_proto IS NOT NULL  AND p.dta_annulla is not null ';
    execute immediate istruzioneSQL into totmonprotannul;          


istruzioneSQLbase := '      SELECT COUNT (p.system_id)
         --INTO totmondocgrigi
        FROM PROFILE p, people, dpa_l_ruolo_reg
       WHERE p.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = '||id_reg||'
         AND p.author = people.system_id
         
         AND people.id_amm = '||id_ammi||'
         and nvl(p.CHA_IN_CESTINO,''0'') <> ''1''
          AND p.num_proto IS NULL
         AND p.cha_tipo_proto = ''G''
         AND id_documento_principale IS NULL
         AND TO_NUMBER (TO_CHAR (p.creation_date, ''MM''))   = '||i||'
         AND TO_NUMBER (TO_CHAR (p.creation_date, ''YYYY'')) = '||p_anno
         ||condizione_sede          ;

istruzioneSQL := istruzioneSQLbase ;
    execute immediate istruzioneSQL into totmondocgrigi;          

/*      SELECT COUNT (PROFILE.system_id)
        INTO totmondocgrigiclass
        FROM PROFILE, people, dpa_l_ruolo_reg
       WHERE PROFILE.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = id_reg
         AND PROFILE.author = people.system_id
         AND people.id_amm = id_ammi
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'MM')) = i
                  AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = p_anno
         AND PROFILE.cha_tipo_proto = 'G'
         AND id_documento_principale IS NULL
         AND PROFILE.var_sede = var_s;
*/       
istruzioneSQL := istruzioneSQLbase || ' AND exists (select ''x'' 
                                            from project_components pg where pg.link=p.system_id ) ' ;
    execute immediate istruzioneSQL into totmondocgrigiclass;          



istruzioneSQLbase := '      
      SELECT COUNT (p.system_id)
        --INTO totmonprofprota
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, ''MM'')) = '||i||'
         AND p.num_anno_proto = '||p_anno||'
         AND p.cha_da_proto = ''0''
         and nvl(p.CHA_IN_CESTINO,''0'') <> ''1''
          --AND p.cha_img = ''0'' OBSOLETA
         and Getchaimg(p.docnumber) ! = ''0'' 
         
         AND p.id_registro = '||id_reg
         ||condizione_sede          ; 

istruzioneSQL := istruzioneSQLbase ||' AND p.cha_tipo_proto = ''A''   AND p.dta_annulla IS NULL' ; 
    execute immediate istruzioneSQL into totmonprofprota;


/*      SELECT COUNT (p.system_id)
        INTO totmonprofprotp
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND p.cha_tipo_proto = 'P'
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NULL
         AND p.var_sede = var_s;
*/        
istruzioneSQL := istruzioneSQLbase ||' AND p.cha_tipo_proto = ''P''   AND p.dta_annulla IS NULL' ; 
    execute immediate istruzioneSQL into totmonprofprotp;


/*      SELECT COUNT (p.system_id)
        INTO totmonprofproti
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND p.cha_tipo_proto = 'I'
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NULL
         AND p.var_sede = var_s;
*/
istruzioneSQL := istruzioneSQLbase ||' AND p.cha_tipo_proto = ''I''   AND p.dta_annulla IS NULL' ; 
    execute immediate istruzioneSQL into totmonprofproti;          

/*      SELECT COUNT (p.system_id)
        INTO totmonprofprotannul
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND num_proto IS NOT NULL
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NOT NULL
         AND p.var_sede = var_s;
*/
istruzioneSQL := istruzioneSQLbase ||' AND num_proto IS NOT NULL      AND p.dta_annulla IS NOT NULL' ; 
    execute immediate istruzioneSQL into totmonprofprotannul;          


      /*SELECT COUNT (PROFILE.system_id)
        INTO totmonprofgrigi
        FROM PROFILE, people, dpa_l_ruolo_reg
       WHERE PROFILE.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = id_reg
         AND PROFILE.author = people.system_id
         AND people.id_amm = id_ammi
         AND PROFILE.cha_img = '0'
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'MM')) = i
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = p_anno
         AND PROFILE.cha_tipo_proto = 'G'
         AND id_documento_principale IS NULL
         AND PROFILE.var_sede = var_s;
         */

istruzioneSQLbase := '
        SELECT COUNT (p.system_id)         --INTO totmonprofgrigi
        FROM PROFILE p, people, dpa_l_ruolo_reg
       WHERE p.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND P.author = people.system_id
         AND dpa_l_ruolo_reg.id_registro = '||id_reg||'
         AND people.id_amm = '||id_ammi||'
        and nvl(p.CHA_IN_CESTINO,''0'') <> ''1''
          -- AND PROFILE.cha_img = ''0'' OBSOLETA
         and Getchaimg(p.docnumber) ! = ''0'' 
         
         AND TO_NUMBER (TO_CHAR (p.creation_date, ''MM''))   = '||i||'
         AND TO_NUMBER (TO_CHAR (P.creation_date, ''YYYY'')) = '||p_anno
         ||condizione_sede          ;         

istruzioneSQL := istruzioneSQLbase ||' AND p.cha_tipo_proto = ''G''   AND p.id_documento_principale IS NULL ' ; 
    execute immediate istruzioneSQL into totmonprofgrigi;          



istruzioneSQLbase := 'SELECT COUNT (p.system_id) --        INTO totmonprotaclass
            FROM PROFILE p
           WHERE TO_NUMBER (TO_CHAR (p.dta_proto, ''MM'')) = '||i||'
             AND p.num_anno_proto = '||p_anno||'
         and nvl(p.CHA_IN_CESTINO,''0'') <> ''1''
             AND p.id_registro = '||id_reg||'
             AND exists (select ''x'' from project_components pg where pg.link=p.system_id )
                AND p.num_proto IS NOT NULL'          
                ||condizione_sede          ; 
                    

istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''A'' 
                                        AND p.cha_da_proto = ''0''    AND dta_annulla IS NULL ' ;               
execute immediate istruzioneSQL into totmonprotaclass;

      /*SELECT COUNT (PROFILE.system_id)
        INTO totmonprotpclass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'P'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
                AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND dta_annulla IS NULL
         AND PROFILE.var_sede = var_s;
*/
istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''P'' 
                                        AND p.cha_da_proto = ''0''    AND dta_annulla IS NULL ' ;               
execute immediate istruzioneSQL into totmonprotpclass;

/*      SELECT COUNT (PROFILE.system_id)
        INTO totmonproticlass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'I'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
                AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND dta_annulla IS NULL
         AND PROFILE.var_sede = var_s;
*/         
istruzioneSQL := istruzioneSQLbase || ' AND p.cha_tipo_proto = ''I'' 
                                        AND p.cha_da_proto = ''0''    AND dta_annulla IS NULL ' ;               
execute immediate istruzioneSQL into totmonproticlass;

/*      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotannulclass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
             AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND PROFILE.dta_annulla IS NOT NULL
         AND PROFILE.var_sede = var_s;
*/         
istruzioneSQL := istruzioneSQLbase || ' AND p.cha_da_proto = ''0''    AND dta_annulla IS NOT NULL ' ;               
execute immediate istruzioneSQL into totmonprotannulclass;

/*
   ELSE
      SELECT COUNT (PROFILE.system_id)
        INTO totmonprota
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'A'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotp
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'P'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonproti
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'I'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotannul
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND PROFILE.dta_annulla IS NOT NULL
         AND PROFILE.num_proto IS NOT NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmondocgrigi
        FROM PROFILE, people, dpa_l_ruolo_reg
       WHERE PROFILE.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = id_reg
         AND PROFILE.author = people.system_id
         AND people.id_amm = id_ammi
         AND PROFILE.num_proto IS NULL
         AND PROFILE.cha_tipo_proto = 'G'
         AND id_documento_principale IS NULL
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'MM')) = i
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = p_anno;

      SELECT COUNT (p.system_id)
        INTO totmonprofprota
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND p.cha_tipo_proto = 'A'
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NULL;

      SELECT COUNT (p.system_id)
        INTO totmonprofprotp
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND p.cha_tipo_proto = 'P'
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NULL;

      SELECT COUNT (p.system_id)
        INTO totmonprofproti
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND p.cha_tipo_proto = 'I'
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NULL;

      SELECT COUNT (p.system_id)
        INTO totmonprofprotannul
        FROM PROFILE p
       WHERE TO_NUMBER (TO_CHAR (p.dta_proto, 'MM')) = i
         AND p.num_anno_proto = p_anno
         AND num_proto IS NOT NULL
         AND p.cha_da_proto = '0'
         AND p.cha_img = '0'
         AND p.id_registro = id_reg
         AND p.dta_annulla IS NOT NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprofgrigi
        FROM PROFILE, people, dpa_l_ruolo_reg
       WHERE PROFILE.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = id_reg
         AND PROFILE.author = people.system_id
         AND people.id_amm = id_ammi
         AND PROFILE.cha_img = '0'
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'MM')) = i
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = p_anno
         AND PROFILE.cha_tipo_proto = 'G'
         AND id_documento_principale IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotaclass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'A'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
               AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotpclass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'P'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
                AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonproticlass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_tipo_proto = 'I'
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
         AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND dta_annulla IS NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmonprotannulclass
        FROM PROFILE
       WHERE TO_NUMBER (TO_CHAR (PROFILE.dta_proto, 'MM')) = i
         AND PROFILE.num_anno_proto = p_anno
         AND PROFILE.cha_da_proto = '0'
         AND PROFILE.id_registro = id_reg
            AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND PROFILE.num_proto IS NOT NULL
         AND PROFILE.dta_annulla IS NOT NULL;

      SELECT COUNT (PROFILE.system_id)
        INTO totmondocgrigiclass
        FROM PROFILE, dpa_l_ruolo_reg, people
       WHERE PROFILE.id_ruolo_prot = dpa_l_ruolo_reg.id_ruolo_in_uo
         AND dpa_l_ruolo_reg.id_registro = id_reg
         AND PROFILE.author = people.system_id
         AND people.id_amm = id_ammi
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'MM')) = i
           AND exists (select 'x' from project_components pg where pg.link=profile.system_id )
         AND TO_NUMBER (TO_CHAR (PROFILE.creation_date, 'YYYY')) = p_anno
         AND PROFILE.cha_tipo_proto = 'G'
         AND id_documento_principale IS NULL;
   END IF;
*/

/*Calcoliamo i valori annuali a partire dai dati del mese***************************************/
--
   totmonprot := totmonprota + totmonprotp + totmonproti + totmonprotannul;
   totmondoc := totmonprot + totmondocgrigi;
   totanndoc := totanndoc + totmondoc;
   totannprot := totannprot + totmonprot;
   totannprota := totannprota + totmonprota;
   totannprotp := totannprotp + totmonprotp;
   totannproti := totannproti + totmonproti;
   totanndocgrigi := totanndocgrigi + totmondocgrigi;
   totmondocclass :=
        totmondocgrigiclass
      + totmonprotannulclass
      + totmonprotaclass
      + totmonprotpclass
      + totmonproticlass;
   totanndocclass := totanndocclass + totmondocclass;
   totmonprotclass :=
        totmonprotannulclass
      + totmonprotaclass
      + totmonprotpclass
      + totmonproticlass;
   totannprotclass := totannprotclass + totmonprotclass;
   totannprotaclass := totannprotaclass + totmonprotaclass;
   totannprotpclass := totannprotpclass + totmonprotpclass;
   totannproticlass := totannproticlass + totmonproticlass;
   totannprotannulclass := totannprotannulclass + totmonprotannulclass;
   totannprotannul := totannprotannul + totmonprotannul;
--
   totmonprof :=
        totmonprofprotannul
      + totmonprofgrigi
      + totmonprofprota
      + totmonprofprotp
      + totmonprofproti;
   totannprof := totannprof + totmonprof;
--
   totmonprofprot :=
      totmonprofprotannul + totmonprofprota + totmonprofprotp
      + totmonprofproti;
   totannprofprot := totannprofprot + totmonprofprot;
   totannprofprota := totannprofprota + totmonprofprota;
   totannprofprotp := totannprofprotp + totmonprofprotp;
   totannprofproti := totannprofproti + totmonprofproti;
   totannprofgrigi := totannprofgrigi + totmonprofgrigi;
   totannprofprotannull := totannprofprotannull + totmonprofprotannul;
   totanndocgrigiclass := totanndocgrigiclass + totmondocgrigiclass;

/*****Percentuali************************************************************************/
/*Percentuale dei protoclli annullati classificati*/
   IF (totannprotannulclass <> 0 AND totannprotclass <> 0)
   THEN
      percannprotannulclass :=
                 ROUND (((totannprotannulclass / totannprotclass) * 100), 2);
   END IF;

/*Percentuale annuale dei documenti grigi classificati*/
   IF (totanndocclass <> 0 AND totanndocgrigiclass <> 0)
   THEN
      percanndocgrigiclass :=
                   ROUND (((totanndocgrigiclass / totanndocclass) * 100), 2);
   END IF;

/*Percentuale dei profili annullati*/
   IF ((totannprofprot <> 0) AND (totannprofprotannull <> 0))
   THEN
      percannprofprotannull :=
                  ROUND (((totannprofprotannull / totannprofprot) * 100), 2);
   END IF;

   IF (totannprot <> 0)
   THEN
/*Percentuale di documenti protocollati*/
      percannprot := ROUND (((totannprot / totanndoc) * 100), 2);

      IF (totannprota <> 0)
      THEN
/*Percentuale di protocolli in arrivo*/
         percannprota := ROUND (((totannprota / totannprot) * 100), 2);
      END IF;

      IF (totannprotp <> 0)
      THEN
/*Percentuale di protocolli in partenza*/
         percannprotp := ROUND (((totannprotp / totannprot) * 100), 2);
      END IF;

      IF (totannproti <> 0)
      THEN
/*Percentuale di protocolli interni*/
         percannproti := ROUND (((totannproti / totannprot) * 100), 2);
      END IF;

      IF (totannprotannul <> 0)
      THEN
/*Percentuale di protocolli annullati*/
         percannprotannul :=
                           ROUND (((totannprotannul / totannprot) * 100), 2);
      END IF;
   END IF;

   IF (totanndoc <> 0)
   THEN
      IF (totanndocgrigi <> 0)
      THEN
/*Percentuale di doc grigi*/
         percanndocgrigi := ROUND (((totanndocgrigi / totanndoc) * 100), 2);
      END IF;

      IF (totanndocclass <> 0)
      THEN
/*Percentuale di doc classificati*/
         percanndocclass := ROUND (((totanndocclass / totanndoc) * 100), 2);
      END IF;
   END IF;

   IF (totanndocclass <> 0)
   THEN
      IF (totannprotclass <> 0)
      THEN
/*Percentuale di doc classificati e protocollati*/
         percannprotclass :=
                       ROUND (((totannprotclass / totanndocclass) * 100), 2);

         IF (totannprotaclass <> 0)
         THEN
/*Percentuale di doc classificati e protocollati in arrivo*/
            percannprotaclass :=
                     ROUND (((totannprotaclass / totannprotclass) * 100), 2);
         END IF;

         IF (totannprotpclass <> 0)
         THEN
/*Percentuale di doc classificati e protocollati in partenza*/
            percannprotpclass :=
                     ROUND (((totannprotpclass / totannprotclass) * 100), 2);
         END IF;

         IF (totannproticlass <> 0)
         THEN
/*Percentuale di doc classificati e protocollati interni*/
            percannproticlass :=
                     ROUND (((totannproticlass / totannprotclass) * 100), 2);
         END IF;
      END IF;
   END IF;

/*Calcoliamo le percentuali mensili**************************************************************************************/
   IF (totmondoc <> 0)
   THEN
      IF (totmonprot <> 0)
      THEN
/*Percentuale mensile di protocolli*/
         percmonprot := ROUND (((totmonprot / totmondoc) * 100), 2);

         IF (totmonprota <> 0)
         THEN
/*Percentuale mensile di protocolli ARRIVO*/
            percmonprota := ROUND (((totmonprota / totmonprot) * 100), 2);
         END IF;

         IF (totmonprotp <> 0)
         THEN
/*Percentuale mensile di protocolli PARTENZA*/
            percmonprotp := ROUND (((totmonprotp / totmonprot) * 100), 2);
         END IF;

         IF (totmonproti <> 0)
         THEN
/*Percentuale mensile di protocolli INTERNI*/
            percmonproti := ROUND (((totmonproti / totmonprot) * 100), 2);
         END IF;

         IF (totmonprotannul <> 0)
         THEN
/*Percentuale mensile di protocolli Annullati*/
            percmonprotannul :=
                           ROUND (((totmonprotannul / totmonprot) * 100), 2);
         END IF;
      END IF;

      IF (totmondocgrigi <> 0)
      THEN
/*Percentuale mensile di Doc Grigi*/
         percmondocgrigi := ROUND (((totmondocgrigi / totmondoc) * 100), 2);
      END IF;

      IF (totmondocclass <> 0)
      THEN
/*Percentuale mensile di Doc Class*/
         percmondocclass := ROUND (((totmondocclass / totmondoc) * 100), 2);
      END IF;

      IF (totmonprotclass <> 0)
      THEN
/*Percentuale mensile di protocolli Class*/
         percmonprotclass :=
                            ROUND (((totmonprotclass / totmondoc) * 100), 2);
      END IF;

      IF (totmonprotaclass <> 0)
      THEN
/*Percentuale mensile di protocolli Arrivo Class*/
         percmonprotaclass :=
                           ROUND (((totmonprotaclass / totmondoc) * 100), 2);
      END IF;

      IF (totmonprotpclass <> 0)
      THEN
/*Percentuale mensile di protocolli Partenza Class*/
         percmonprotpclass :=
                           ROUND (((totmonprotpclass / totmondoc) * 100), 2);
      END IF;

      IF (totmonproticlass <> 0)
      THEN
/*Percentuale mensile di protocolli Interni Class*/
         percmonproticlass :=
                           ROUND (((totmonproticlass / totmondoc) * 100), 2);
      END IF;
   END IF;

/*******************************************************************************************************************/
/*Calcoliamo le percentuali  dei profili ( Immagini)  */
   IF (totannprof <> 0)
   THEN
      IF (totannprofgrigi <> 0)
      THEN
/*Percentuale  annuale di profili grigi*/
         percannprofgrigi :=
                           ROUND (((totannprofgrigi / totannprof) * 100), 2);
      END IF;

      IF (totannprofprot <> 0)
      THEN
/*Percentuale  annuale di profili protocollati*/
         percannprofprot := ROUND (((totannprofprot / totannprof) * 100), 2);
      END IF;

      IF (totannprofprota <> 0)
      THEN
/*Percentuale  annuale di profili protocollati ARRIVO*/
         percannprofprota :=
                       ROUND (((totannprofprota / totannprofprot) * 100), 2);
      END IF;

      IF (totannprofprotp <> 0)
      THEN
/*Percentuale  annuale di profili protocollati PARTENZA*/
         percannprofprotp :=
                       ROUND (((totannprofprotp / totannprofprot) * 100), 2);
      END IF;

      IF (totannprofproti <> 0)
      THEN
/*Percentuale  annuale di profili protocollati PARTENZA*/
         percannprofproti :=
                       ROUND (((totannprofproti / totannprofprot) * 100), 2);
      END IF;
   END IF;

/*******************************************************************************************************************/

   /*******************************************************************************************************************/
   mese_vc :=
      CASE i
         WHEN 1
            THEN 'Gennaio'
         WHEN 2
            THEN 'Febbraio'
         WHEN 3
            THEN 'Marzo'
         WHEN 4
            THEN 'Aprile'
         WHEN 5
            THEN 'Maggio'
         WHEN 6
            THEN 'Giugno'
         WHEN 7
            THEN 'Luglio'
         WHEN 8
            THEN 'Agosto'
         WHEN 9
            THEN 'Settembre'
         WHEN 10
            THEN 'Ottobre'
         WHEN 11
            THEN 'Novembre'
         WHEN 12
            THEN 'Dicembre'
      END;
/*inseriamo i dati mensili in una tabella*/
   out_rec.thing := mese_vc;
   out_rec.tot_doc := totmondoc;
   out_rec.grigi := totmondocgrigi;
   out_rec.perc_grigi := TO_CHAR (percmondocgrigi);
   out_rec.prot := totmonprot;
   out_rec.perc_prot := TO_CHAR (percmonprot);
   out_rec.annull := totmonprotannul;
   out_rec.perc_annull := TO_CHAR (percmonprotannul);
   out_rec.arrivo := totmonprota;
   out_rec.perc_arrivo := TO_CHAR (percmonprota);
   out_rec.partenza := totmonprotp;
   out_rec.perc_partenza := TO_CHAR (percmonprotp);
   out_rec.interni := totmonproti;
   out_rec.perc_interni := TO_CHAR (percmonproti);
   PIPE ROW (out_rec);
/*RESET DELLE VARIABILI*/
   totmondoc := 0;
   totmonprot := 0;
   totmonprota := 0;
   totmonprotp := 0;
   totmonproti := 0;
   totmondocgrigi := 0;
/*RESET DELLE PERCENTUALI MENSILI*/
   percmonprot := 0;
   percmonprota := 0;
   percmonprotp := 0;
   percmonproti := 0;
   percmonprotannul := 0;
   percmondocgrigi := 0;
   percmondocclass := 0;
   percmonprotclass := 0;
   percmonprotaclass := 0;
   percmonprotpclass := 0;
   percmonproticlass := 0;
/**********************************/
--end loop;

   /*Inseriamo nella tabella i valori reltivi all'anno*/
/*Aggiungiamo al totale dei documenti annuale il totale dei documenti grigi dell'anno */
/*totAnnDoc := totAnnDoc + totAnnDocGrigi;*/
   out_rec.thing := p_anno;
   out_rec.tot_doc := totanndoc;
   out_rec.grigi := totanndocgrigi;
   out_rec.perc_grigi := TO_CHAR (percanndocgrigi);
   out_rec.prot := totannprot;
   out_rec.perc_prot := TO_CHAR (percannprot);
   out_rec.annull := totannprotannul;
   out_rec.perc_annull := TO_CHAR (percannprotannul);
   out_rec.arrivo := totannprota;
   out_rec.perc_arrivo := TO_CHAR (percannprota);
   out_rec.partenza := totannprotp;
   out_rec.perc_partenza := TO_CHAR (percannprotp);
   out_rec.interni := totannproti;
   out_rec.perc_interni := TO_CHAR (percannproti);
   PIPE ROW (out_rec);
/*Inseriamo nella tabella i valori reltivi alla classificazione*/
   out_rec.thing := 'Classificati*';
   out_rec.tot_doc := totanndocclass;
   out_rec.grigi := totanndocgrigiclass;
   out_rec.perc_grigi := percanndocgrigiclass;
   out_rec.prot := totannprotclass;
   out_rec.perc_prot := TO_CHAR (percannprotclass);
   out_rec.annull := totannprotannulclass;
   out_rec.perc_annull := percannprotannulclass;
   out_rec.arrivo := totannprotaclass;
   out_rec.perc_arrivo := TO_CHAR (percannprotaclass);
   out_rec.partenza := totannprotpclass;
   out_rec.perc_partenza := TO_CHAR (percannprotpclass);
   out_rec.interni := totannproticlass;
   out_rec.perc_interni := TO_CHAR (percannproticlass);
   PIPE ROW (out_rec);
/*Inseriamo nella tabella i valori reltivi alle Immagini - Doc. Fisici Acquisiti -*/
   out_rec.thing := 'Senza Img.';
   out_rec.tot_doc := totannprof;
   out_rec.grigi := totannprofgrigi;
   out_rec.perc_grigi := percannprofgrigi;
   out_rec.prot := totannprofprot;
   out_rec.perc_prot := TO_CHAR (percannprofprot);
--out_rec.ANNULL := '0';
--out_rec.PERC_ANNULL := '0';
   out_rec.annull := TO_CHAR (totannprofprotannull);
   out_rec.perc_annull := TO_CHAR (percannprofprotannull);
   out_rec.arrivo := totannprofprota;
   out_rec.perc_arrivo := TO_CHAR (percannprofprota);
   out_rec.partenza := totannprofprotp;
   out_rec.perc_partenza := TO_CHAR (percannprofprotp);
   out_rec.interni := totannprofproti;
   out_rec.perc_interni := TO_CHAR (percannprofproti);
   PIPE ROW (out_rec);
/*RESET DELLE PERCENTUALI ANNUALI*/
   percannprot := 0;
   percannprota := 0;
   percannprotp := 0;
   percannproti := 0;
   percannprotannul := 0;
   percanndocgrigi := 0;
   percanndocclass := 0;
   percannprotclass := 0;
   percannprotaclass := 0;
   percannprotpclass := 0;
   percannproticlass := 0;
   percannprofgrigi := 0;
   percannprofprot := 0;
   percannprofprota := 0;
   percannprofprotp := 0;
   percannprofproti := 0;
   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN
   
   -- out_rec.thing := SQLERRM;     PIPE ROW (out_rec); 
   
   out_rec.thing :=     'istruzione eseguita:';
   PIPE ROW (out_rec); 
   
    out_rec.thing :=     istruzioneSQL    ;    
    PIPE ROW (out_rec);
   
   --RAISE ; 
      --RETURN;
END mensiledoctablefunction;
/
