--------------------------------------------------------
--  DDL for Function DOCCLASSCOMPTABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DOCCLASSCOMPTABLEFUNCTION" (
   id_amm       NUMBER,
   id_registr   NUMBER,
   id_anno      NUMBER,
   sede         VARCHAR,
   titolario    NUMBER
)
   RETURN docclasscomptablerow PIPELINED
/*
ritorna, in modalita pipelined, il conteggio totale dei fascicoli e il totale distinto per ogni voce di titolario di primo livello
la modalita pipelined permette l'uso della seguente sintassi:
SELECT * FROM table(INFOTN_COLL.DocClassCompTableFunction (361,86107,2011,'',7067503))

il titolario e una struttura gerarchica formata da nodi di tipo T(titolario), F(Fascicolo) e C(cartella alias Folder)
 legati tra loro da relazione id_parent:
un nodo C afferisce o a un nodo C o a un nodo F; idem, un nodo F afferisce o a un nodo F o a un nodo T
un nodo di tipo T afferisce invece sempre a un nodo di tipo T. visualmente, si pensi ad un albero le cui foglie principali
sono foglie di tipo TFC

attenzione un fascicolo puo essere associato a piu voci di titolario, per cui il conteggio e indistinto e viene ripetuto tante volte
quante il documento e associato al titolario
tra l'altro questo e l'unico modo per poter disaggregare il totale sulle voci di primo livello ed avere un conteggio consistente
*/
IS


-- modifica Aprile 2011 - uso di EXECUTE IMMEDIATE per migliorare la leggibilita del codice
istruzioneSQLbase  varchar2(20000);
istruzioneSQL      varchar2(20000);
condizione_sede     varchar2(2000);
condizione_titolario varchar2(200);

  outrec                  docclasscomptabletype
                := docclasscomptabletype (NULL, NULL, NULL, NULL, NULL, NULL);
   totdocclass             FLOAT;
   codclass                VARCHAR (255);
   descclass               VARCHAR (25500);
   totdocclassvt           NUMBER;
   percdocclassvt          FLOAT;
   num_livello1            VARCHAR (255);
   tot_primo_livello       NUMBER;
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
   dataa                   DATE;
   datada                  DATE;
   sdataa                  VARCHAR (19);
   sdatada                 VARCHAR (19);

   CURSOR vocititolario_primoliv
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           AND pr.id_titolario = titolario
           And Pr.Id_Amm = Id_Amm
           And Pr.Cha_Tipo_Proj = 'T'
           -- escludo titolari in definizione o eliminati
           and cha_stato <> 'D' and dta_chiusura is null
           And (Pr.Id_Registro = Id_Registr Or Pr.Id_Registro Is Null)
           And Pr.Num_Livello = 1
           
      ORDER BY pr.var_cod_liv1;
      
   CURSOR vocititolario_primoliv_notit
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           --AND pr.id_titolario = titolario
           AND pr.id_amm = id_amm
           AND pr.cha_tipo_proj = 'T'
           AND (pr.id_registro = id_registr OR pr.id_registro IS NULL)
           And Pr.Num_Livello = 1
           -- escludo titolari in definizione o eliminati
           and cha_stato <> 'D' and dta_chiusura is null
      ORDER BY pr.var_cod_liv1;
      
      
BEGIN
   sdatada := '01/01/' || TO_CHAR (id_anno) || ' 00:00:00';
   sdataa := '31/12/' || TO_CHAR (id_anno) || ' 23:59:59';
   dataa := TO_DATE (sdataa, 'dd/mm/yyyy HH24:mi:ss');
   datada := TO_DATE (sdatada, 'dd/mm/yyyy HH24:mi:ss');
   percdocclassvt := 0;
   totdocclass := 0;
   tot_primo_livello := 0;

--   IF (sede = '')   THEN       v_var_sede := NULL;    ELSE       v_var_sede := sede;    END IF;


IF   trim(sede) IS NULL then    condizione_sede :=  ' and PROFILE.var_sede IS NULL' ;
else                            condizione_sede :=  ' and PROFILE.var_sede = '||sede||' '; 
end if; 

IF (titolario = 0 OR titolario is NULL) THEN    condizione_titolario := ' ';
ELSE                                            condizione_titolario := ' AND pr.id_titolario = '||titolario||' ' ;
END IF; 

 istruzioneSQLbase := 
 'SELECT COUNT ((PROFILE.system_id))        --INTO totdocclass
        FROM project_components prc, PROFILE, project pr     --, primo_livello
       WHERE prc.LINK = PROFILE.system_id         AND prc.project_id = pr.system_id
       and nvl(profile.CHA_IN_CESTINO,''0'') <>  ''1''
         --and profile.dta_proto is not null  
         '         ||condizione_sede||
         '  AND PROFILE.creation_date BETWEEN TO_DATE ('''||sdatada||''', ''dd/mm/yyyy HH24:mi:ss'')
         AND   TO_DATE ('''||sdataa||''', ''dd/mm/yyyy HH24:mi:ss'')
         AND (   (PROFILE.id_registro = '||id_registr||') OR (PROFILE.id_registro IS NULL)      )
-- regole sui protocolli
         AND (   -- partenza
                 (    PROFILE.cha_tipo_proto = ''P'' AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- arrivo
              OR (    PROFILE.cha_tipo_proto = ''A'' AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- interni
              OR (    PROFILE.cha_tipo_proto = ''I'' AND PROFILE.dta_proto IS NOT NULL
                  AND PROFILE.dta_annulla IS NULL    AND num_proto IS NOT NULL                 )
              -- grigi
              OR (    PROFILE.cha_tipo_proto = ''G'' AND PROFILE.id_documento_principale IS NULL                 )
              -- annullati
              OR (                             --profile.dta_proto is null and
                  PROFILE.dta_annulla IS NOT NULL AND num_proto IS NOT NULL                 )             )
         AND prc.project_id IN (                            --= system_id_fold
                SELECT system_id                  FROM project
                 WHERE cha_tipo_proj = ''C''
                   AND id_fascicolo IN (                       -----= parentid
                          SELECT system_id                            FROM project
                           WHERE cha_tipo_proj = ''F''
                             AND id_parent IN (
-- recupero le voci di titolario non occorre ricostruire il titolario a partire dal primo livello con la sua gerarchia
                                    SELECT system_id
                                      --, description, var_codice, num_livello
                                    FROM   project pr
                                     WHERE pr.var_codice IS NOT NULL '||
                                       condizione_titolario||
                                       ' AND pr.id_amm = '||id_amm||'
                                       AND pr.cha_tipo_proj = ''T''
                                       AND (   pr.id_registro = '||id_registr||'
                                            OR pr.id_registro IS NULL                                           ) '; 

istruzioneSQL     := istruzioneSQLbase ||
' -- in questo caso non serve ricostruire la gerarchia tra i nodi
 --CONNECT BY PRIOR pr.system_id = pr.id_parent START WITH pr.num_livello = 1
                                 )))    ';

   --END IF;
execute immediate istruzioneSQL into totdocclass; 


IF (titolario = 0 OR titolario is NULL) THEN

   FOR cursore IN vocititolario_primoliv_notit
   LOOP
      var_codice_livello1 := cursore.var_codice;
      description__livello1 := cursore.descrizione;
   
    istruzioneSQL     := istruzioneSQLbase ||'

    CONNECT BY PRIOR pr.system_id =pr.id_parent
    -- cursore
    START WITH pr.system_id = '||cursore.sysid||'))) ';
    execute immediate istruzioneSQL into tot_primo_livello  ; 





      percdocclassvt := 0;

      IF totdocclass <> 0
      THEN
         percdocclassvt :=
                        ROUND (((tot_primo_livello / totdocclass) * 100), 2);
      END IF;

      outrec.tot_doc_class := totdocclass;
      outrec.cod_class := var_codice_livello1;
      outrec.desc_class := description__livello1;
      outrec.tot_doc_class_vt := tot_primo_livello;
      outrec.perc_doc_class_vt := percdocclassvt;
      outrec.num_livello := '1';
      PIPE ROW (outrec);
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;

 


   RETURN;
   
ELSE       

   FOR cursore IN vocititolario_primoliv
    LOOP
      var_codice_livello1 := cursore.var_codice;
      description__livello1 := cursore.descrizione;
   
    istruzioneSQL     := istruzioneSQLbase ||'
    CONNECT BY PRIOR pr.system_id =pr.id_parent
    -- cursore
    START WITH pr.system_id = '||cursore.sysid||'))) ';
    execute immediate istruzioneSQL into tot_primo_livello  ; 



      percdocclassvt := 0;

      IF totdocclass <> 0
      THEN
         percdocclassvt :=
                        ROUND (((tot_primo_livello / totdocclass) * 100), 2);
      END IF;

      outrec.tot_doc_class := totdocclass;
      outrec.cod_class := var_codice_livello1;
      outrec.desc_class := description__livello1;
      --outrec.desc_class := substr(istruzioneSQL,1,2000);
      
      outrec.tot_doc_class_vt := tot_primo_livello;
      outrec.perc_doc_class_vt := percdocclassvt;
      outrec.num_livello := '1';
      PIPE ROW (outrec);
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;

   RETURN;
 
END IF; 

   
   
EXCEPTION
   WHEN OTHERS
   THEN
      
      outrec.tot_doc_class := length(istruzioneSQL);
      outrec.cod_class := NULL;
      outrec.desc_class := SQLERRM||substr(istruzioneSQL,1,3000);

      outrec.tot_doc_class_vt := NULL;
      outrec.perc_doc_class_vt := NULL;
      outrec.num_livello := NULL;
      PIPE ROW (outrec);
      --RAISE;
      RETURN;
END docclasscomptablefunction;

/
