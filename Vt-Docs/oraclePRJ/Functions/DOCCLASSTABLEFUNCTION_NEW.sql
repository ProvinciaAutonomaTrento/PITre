--------------------------------------------------------
--  DDL for Function DOCCLASSTABLEFUNCTION_NEW
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DOCCLASSTABLEFUNCTION_NEW" (
   id_amm       NUMBER,
   id_registr   NUMBER,
 --  id_anno      NUMBER,
-- il filtro per sede si fa poi direttamente nella select 

   sede         VARCHAR,
   titolario    NUMBER
)
   RETURN docclasstablerow_p PIPELINED
/*
ritorna, in modalita pipelined, il conteggio totale dei fascicoli AGGIORNATO in base a REFRESH della MV  mv_prospetti_documenticlass
e il totale distinto per ogni voce di titolario (vt= voce di titolario), a qualsiasi livello

c'e una funziona gemella che torna il totale solo per le voci di primo livello

la modalita pipelined permette l'uso della seguente sintassi:
SELECT * FROM table(INFOTN_COLL.docclasstablefunction (361,86107,2011,'',7067503))

il titolario e una struttura gerarchica formata da nodi di tipo T(titolario), F(Fascicolo) e C(cartella alias Folder)
 legati tra loro da relazione id_parent:
un nodo C afferisce o a un nodo C o a un nodo F; idem, un nodo F afferisce o a un nodo F o a un nodo T
un nodo di tipo T afferisce invece sempre a un nodo di tipo T. visualmente, si pensi ad un albero le cui foglie principali
sono foglie di tipo TFC

attenzione un fascicolo puo essere associato a piu voci di titolario, per cui il conteggio e indistinto e viene ripetuto tante volte
quante il documento e associato al titolario
tra l'altro questo e l'unico modo per poter disaggregare il totale sulle voci ed avere un conteggio consistente
*/
IS
   outrec                  docclasstabletype_p
                := docclasstabletype_p (NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);
   totdocclass             FLOAT :=0 ;
   codclass                VARCHAR (255);
   descclass               VARCHAR (255);
   totdocclassvt           NUMBER;
   percdocclassvt          FLOAT :=0 ;

   num_livello1            VARCHAR (255);
   tot_primo_livello       NUMBER :=0 ;
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
  
    sdatada VARCHAR (19) ;
    sdataa  VARCHAR (19) ;
 
  
   CURSOR vocititolario
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello AS livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           AND pr.id_titolario = titolario
           AND pr.id_amm = id_amm
           AND pr.cha_tipo_proj = 'T'
           AND (pr.id_registro = id_registr OR pr.id_registro IS NULL)
-- rispetto alla versione compatta si puo togliere il filtro su num_livello
           --AND pr.num_livello = 1
      ORDER BY pr.var_cod_liv1;

   CURSOR vocititolario_notit -- da usare se parametro titolario = 0, si commenta il filtro sul titolario
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello AS livello
               -- new
               ,pr.id_titolario as id_titolario
               ,pr.var_cod_liv1
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           --AND pr.id_titolario = titolario
           AND pr.id_amm = id_amm
           AND pr.cha_tipo_proj = 'T'
           AND (pr.id_registro = id_registr OR pr.id_registro IS NULL)
      ORDER BY pr.var_cod_liv1;      

istruzioneSQLconteggiotot   varchar2(20000);
istruzioneSQL               varchar2(20000);
istruzioneSQLbase           varchar2(20000);

--condizione_sede             varchar2(2000);
condizione_titolario        varchar2(200);

min_anno integer; 
max_anno integer; 
contatore integer; 
nome_indice varchar2(32);
BEGIN

-- il filtro per sede si fa poi direttamente nella select 
--IF  trim(sede) IS NULL then   condizione_sede :=  ' and p.var_sede IS NULL ' ;    else                      condizione_sede :=  ' and p.var_sede = '||sede||' '; end if; 

IF (trim(titolario) = 0 OR titolario is NULL) THEN  condizione_titolario := ' ';
    ELSE                                            condizione_titolario := ' AND p.id_titolario = '||titolario ;
END IF;
 
    BEGIN    -- controllo esistenza indice su dta_proto per evitare full scan nella select successiva 
        select index_name into nome_indice from user_ind_columns
            where table_name = 'PROFILE' and column_name = 'DTA_PROTO' and column_position = 1 ; 
      -- se non esiste    fa raise ed esce
        EXCEPTION WHEN NO_DATA_FOUND THEN            RAISE ; RETURN;
    END ; 
    
select to_char(min(dta_proto),'YYYY') into min_anno from profile; 
contatore := min_anno;    
select to_char(max(dta_proto),'YYYY') into max_anno from profile; 
for cicla_su_anno in min_anno .. max_anno -- loop su tutti gli anni per cui esistono protocolli
loop 
    exit when contatore =  max_anno + 1;
    contatore := contatore + 1 ;

    sdatada  := '01/01/' || TO_CHAR (cicla_su_anno) || ' 00:00:00';
    sdataa   := '31/12/' || TO_CHAR (cicla_su_anno) || ' 23:59:59';

   totdocclass             :=0 ;
   codclass                := NULL;
   descclass               := NULL;
   totdocclassvt           := 0;
   percdocclassvt          := 0;
   tot_primo_livello       := 0;
   
   num_livello1            := NULL;
   var_codice_livello1     := NULL;
   description__livello1   := NULL;
   system_id_vt            := NULL;
   description_vt          := NULL;
   var_codice_vt           := NULL;

istruzioneSQLconteggiotot := --  INTO totdocclass
    'select sum(p.NOT_DISTINCT_COUNT) from mv_prospetti_documenticlass p 
    where p.CREATION_MONTH BETWEEN TO_DATE (''01/01/' || TO_CHAR (cicla_su_anno) || ' 00:00:00'', ''dd/mm/yyyy HH24:mi:ss'') 
                               AND TO_DATE (''31/12/' || TO_CHAR (cicla_su_anno) || ' 23:59:59'', ''dd/mm/yyyy HH24:mi:ss'')  '||         
                ' AND p.NVL_CHA_IN_CESTINO <> ''1'' '|| 
                --condizione_sede||
                condizione_titolario||
                ' AND p.id_amm = '||id_amm||
                ' AND (p.id_registro = '||id_registr||' OR p.id_registro IS NULL) '
                  ||
 -- regole sui protocolli
            ' AND (   -- partenza
                 (    p.cha_tipo_proto = ''P'' AND flag_dta_proto = ''1''
                  AND flag_annullato = ''0''    AND flag_num_proto = ''1''                 )
              -- arrivo
              OR (    p.cha_tipo_proto = ''A'' AND flag_dta_proto = ''1''
                  AND flag_annullato = ''0''    AND flag_num_proto = ''1''                 )
              -- interni
              OR (    p.cha_tipo_proto = ''I'' AND flag_dta_proto = ''1''
                  AND flag_annullato = ''0''    AND flag_num_proto = ''1''                 )
              -- grigi
              OR (    p.cha_tipo_proto = ''G'' --AND p.id_documento_principale IS NULL                 
                 )
              -- annullati
              OR (                             --p.dta_annulla IS NOT NULL AND num_proto IS NOT NULL 
              flag_annullato = ''1''    AND flag_num_proto = ''1''   ) ) ' ;

execute immediate istruzioneSQLconteggiotot INTO totdocclass ; 

-- inizio calcoli per ogni singola voce di titolario (vt= voce di titolario) 

istruzioneSQLbase := 
'SELECT COUNT (p.system_id) -- INTO tot_primo_livello  
           FROM project_components prc,  PROFILE p,   project pr
          WHERE prc.LINK = p.system_id
            AND prc.project_id = pr.system_id
            AND p.creation_date BETWEEN TO_DATE ('''||sdatada||''', ''dd/mm/yyyy HH24:mi:ss'') 
                                    AND TO_DATE ('''||sdataa||''', ''dd/mm/yyyy HH24:mi:ss'')
            -- segnalazione A. Marta dic 2011
            -- and p.dta_annulla is null 
            and nvl(p.CHA_IN_CESTINO,''0'') <>  ''1''            
            AND (pr.id_registro = '||id_registr||' OR pr.id_registro IS NULL) '
           || --condizione_sede||
         
  -- regole sui protocolli
            ' AND (   -- partenza
                 (    p.cha_tipo_proto = ''P'' AND p.dta_proto IS NOT NULL
                  AND p.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- arrivo
              OR (    p.cha_tipo_proto = ''A'' AND p.dta_proto IS NOT NULL
                  AND p.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- interni
              OR (    p.cha_tipo_proto = ''I'' AND p.dta_proto IS NOT NULL
                  AND p.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- grigi
              OR (    p.cha_tipo_proto = ''G'' --AND p.id_documento_principale IS NULL                 
                 )
              -- annullati
              OR (                             --profile.dta_proto is null and
                  p.dta_annulla IS NOT NULL AND num_proto IS NOT NULL                 )             ) ' ||
           ' AND prc.project_id IN (                         --= system_id_fold
                   SELECT system_id FROM project
                    WHERE cha_tipo_proj = ''C''
                      AND id_fascicolo IN (                    -----= parentid
                             SELECT system_id FROM project
                              WHERE cha_tipo_proj = ''F'' '; 
    
   IF titolario = 0 THEN 
   
   FOR cursore IN vocititolario_notit
   LOOP
      var_codice_livello1 := cursore.var_codice;
      description__livello1 := cursore.descrizione;
      
     istruzioneSQL := istruzioneSQLbase|| 
                               ' AND id_parent = '||cursore.sysid||
                               ' )) '; -- la condizione sul titolario si applica al cursore, non qui sulla select 
     
    execute immediate istruzioneSQL INTO tot_primo_livello; 

      percdocclassvt := 0;
      percdocclassvt := ROUND (((tot_primo_livello / totdocclass) * 100), 2);
      
      outrec.id_amm       := id_amm     ; 
      outrec.id_registr   := id_registr ;  
      outrec.id_anno      := cicla_su_anno    ;
      outrec.sede         := sede       ;
      outrec.titolario    := cursore.id_titolario  ; -- in questo caso valorizzo con l'id_titolario del cursore, non con il valore del parametro = 0 per avere tutti i titolari 
      
      outrec.tot_doc_class      := totdocclass;
      outrec.cod_class          := var_codice_livello1;
      outrec.desc_class         := description__livello1;
      outrec.tot_doc_class_vt   := tot_primo_livello;
      outrec.perc_doc_class_vt  := percdocclassvt;
      outrec.num_livello        := cursore.livello;
      outrec.var_cod_liv1       := cursore.var_cod_liv1 ;
      PIPE ROW (outrec);
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;

ELSE -- id_titolario <> 0  
   
   FOR cursore IN vocititolario
   LOOP
      var_codice_livello1 := cursore.var_codice;
      description__livello1 := cursore.descrizione;
      
     istruzioneSQL := istruzioneSQLbase|| 
                               ' AND id_parent = '||cursore.sysid||
                               ' )) '; -- la condizione sul titolario si applica al cursore, non qui sulla select 
          
    execute immediate istruzioneSQL INTO tot_primo_livello; 

      percdocclassvt := 0;
      percdocclassvt := ROUND (((tot_primo_livello / totdocclass) * 100), 2);
      
      outrec.id_amm       := id_amm     ; 
      outrec.id_registr   := id_registr ;  
      outrec.id_anno      := cicla_su_anno    ;
      outrec.sede         := sede       ;
      outrec.titolario    := titolario  ;
      
      outrec.tot_doc_class      := totdocclass ;
      outrec.cod_class          := var_codice_livello1;
      outrec.desc_class         :=  description__livello1;
      outrec.tot_doc_class_vt   := tot_primo_livello;
      outrec.perc_doc_class_vt  := percdocclassvt;
      outrec.num_livello        := cursore.livello;
      PIPE ROW (outrec);
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;
END IF; 

end loop; -- end loop su tutti gli anni per cui esistono protocolli 
 
   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN     -- RAISE;
      outrec.id_amm       := id_amm     ; 
      outrec.id_registr   := id_registr ;  
      outrec.id_anno      := 0 ;
      outrec.sede         := sede       ;
      outrec.titolario    := titolario  ;
      
      outrec.tot_doc_class := NULL;
      outrec.cod_class := NULL;
      outrec.desc_class := SQLERRM;
      outrec.tot_doc_class_vt := NULL;
      outrec.perc_doc_class_vt := NULL;
      outrec.num_livello := NULL;
      PIPE ROW (outrec);
      RETURN;
END docclasstablefunction_new;

/
