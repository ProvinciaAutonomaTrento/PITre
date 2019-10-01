--------------------------------------------------------
--  DDL for Function DOCCLASSTABLEFUNCTION
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."DOCCLASSTABLEFUNCTION" (
   id_amm       NUMBER,
   id_registr   NUMBER,
   id_anno      NUMBER,
   sede         VARCHAR,
   titolario    NUMBER
)
   RETURN docclasscomptablerow PIPELINED
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
   outrec                  docclasscomptabletype
                := docclasscomptabletype (NULL, NULL, NULL, NULL, NULL, NULL);
   totdocclass             FLOAT;
   codclass                VARCHAR (255);
   descclass               VARCHAR (255);
   totdocclassvt           NUMBER;
   percdocclassvt          FLOAT;

   num_livello1            VARCHAR (255);
   tot_primo_livello       NUMBER;
   var_codice_livello1     VARCHAR (255);
   description__livello1   VARCHAR (255);
   system_id_vt            NUMBER;
   description_vt          VARCHAR (255);
   var_codice_vt           VARCHAR (255);
  
 sdataa                  VARCHAR (19);
 sdatada                 VARCHAR (19);

   CURSOR vocititolario
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello AS livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           AND pr.id_titolario = titolario
           AND pr.id_amm = id_amm
           AND pr.cha_tipo_proj = 'T'
           And (Pr.Id_Registro = Id_Registr Or Pr.Id_Registro Is Null)
           -- escludo titolari in definizione o eliminati
           and cha_stato <> 'D' and dta_chiusura is null
-- rispetto alla versione compatta si puo togliere il filtro su num_livello
           --AND pr.num_livello = 1
      ORDER BY pr.var_cod_liv1;

   CURSOR vocititolario_notit -- da usare se parametro titolario = 0, si commenta il filtro sul titolario
   IS
      SELECT   pr.system_id AS sysid, pr.description AS descrizione,
               pr.var_codice AS var_codice, pr.num_livello AS livello
          FROM project pr
         WHERE pr.var_codice IS NOT NULL
           --AND pr.id_titolario = titolario
           And Pr.Id_Amm = Id_Amm
           And Pr.Cha_Tipo_Proj = 'T'-- escludo titolari in definizione o eliminati
           and cha_stato <> 'D' and dta_chiusura is null
           And (Pr.Id_Registro = Id_Registr Or Pr.Id_Registro Is Null)
           
      ORDER BY pr.var_cod_liv1;      

istruzioneSQLconteggiotot  varchar2(20000);
istruzioneSQL      varchar2(20000);
istruzioneSQLbase      varchar2(20000);

condizione_sede     varchar2(2000);
condizione_titolario varchar2(200);

BEGIN
  sdatada := '01/01/' || TO_CHAR (id_anno) || ' 00:00:00';
  sdataa  := '31/12/' || TO_CHAR (id_anno) || ' 23:59:59';
   percdocclassvt := 0;
   totdocclass := 0;
   tot_primo_livello := 0;

IF  trim(sede) IS NULL then   condizione_sede :=  ' and p.var_sede IS NULL ' ;
else                          condizione_sede :=  ' and p.var_sede = '||sede||' '; 
end if; 

IF (titolario = 0 OR titolario is NULL) THEN    condizione_titolario := ' ';
ELSE                                            condizione_titolario := ' AND p.id_titolario = '||titolario ;
END IF; 


  istruzioneSQLconteggiotot := --  INTO totdocclass
    'select sum(p.NOT_DISTINCT_COUNT) from mv_prospetti_documenticlass p 
    where p.CREATION_MONTH BETWEEN TO_DATE (''01/01/' || TO_CHAR (id_anno) || ' 00:00:00'', ''dd/mm/yyyy HH24:mi:ss'') 
                               AND TO_DATE (''31/12/' || TO_CHAR (id_anno) || ' 23:59:59'', ''dd/mm/yyyy HH24:mi:ss'')          
                 and p.FLAG_ANNULLATO = 0 '||
                ' AND p.NVL_CHA_IN_CESTINO <> ''1'' '|| 
                condizione_sede||
                condizione_titolario||
                ' AND p.id_amm = '||id_amm||
                ' AND (p.id_registro = '||id_registr||' OR p.id_registro IS NULL) ';

execute immediate istruzioneSQLconteggiotot INTO totdocclass ; 


-- inizio calcoli per ogni singola voce di titolario (vt= voce di titolario) 

istruzioneSQLbase := 
'SELECT COUNT (p.system_id) -- INTO tot_primo_livello  
           FROM project_components prc,  PROFILE p,   project pr
          WHERE prc.LINK = p.system_id
            AND prc.project_id = pr.system_id
            AND p.creation_date BETWEEN TO_DATE ('''||sdatada||''', ''dd/mm/yyyy HH24:mi:ss'') 
                                    AND TO_DATE ('''||sdataa||''', ''dd/mm/yyyy HH24:mi:ss'')
            and p.dta_annulla is null 
            and nvl(p.CHA_IN_CESTINO,''0'') <>  ''1''            
            AND (pr.id_registro = '||id_registr||' OR pr.id_registro IS NULL) '
           ||condizione_sede||
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
      outrec.tot_doc_class := totdocclass;
      outrec.cod_class := var_codice_livello1;
      outrec.desc_class :=  description__livello1;
      outrec.tot_doc_class_vt := tot_primo_livello;
      outrec.perc_doc_class_vt := percdocclassvt;
      outrec.num_livello := cursore.livello;
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
      outrec.tot_doc_class := totdocclass ;
      outrec.cod_class := var_codice_livello1;
      outrec.desc_class :=  description__livello1;
      outrec.tot_doc_class_vt := tot_primo_livello;
      outrec.perc_doc_class_vt := percdocclassvt;
      outrec.num_livello := cursore.livello;
      PIPE ROW (outrec);
      tot_primo_livello := 0;
      percdocclassvt := 0;
   END LOOP;
END IF; 

   RETURN;
EXCEPTION
   WHEN OTHERS
   THEN     -- RAISE;
      outrec.tot_doc_class := NULL;
      outrec.cod_class := NULL;
      outrec.desc_class := SQLERRM;
      outrec.tot_doc_class_vt := NULL;
      outrec.perc_doc_class_vt := NULL;
      outrec.num_livello := NULL;
      PIPE ROW (outrec);
      RETURN;
END docclasstablefunction;

/
