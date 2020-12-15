--------------------------------------------------------
--  DDL for Function GETIFMODELLOAUTORIZZATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETIFMODELLOAUTORIZZATO" (
   id_ruolo          NUMBER,
   id_people         NUMBER,
   system_id         NUMBER,
   id_modellotrasm   NUMBER,
   accesssrigth      NUMBER
)
   RETURN NUMBER
IS
   retval         NUMBER;
--accesssRigth number;
   idragione      NUMBER;
   tipo_diritto   CHAR;
   
   hide_versions INT := NULL;
   isDocument INT := NULL;
   consolidationState INT := 0;
   idObject NUMBER(10) := NULL;

   CURSOR cur
   IS
      SELECT DISTINCT id_ragione
                 FROM dpa_modelli_mitt_dest
                WHERE id_modello = id_modellotrasm
                  AND cha_tipo_mitt_dest <> 'M';
BEGIN
   retval := 1;
   
   -- Verifica se il modello trasmissione includa almeno una trasmissione singola
   -- con modalita nascondi versioni precedenti
   select count(*) into hide_versions 
   from dpa_modelli_mitt_dest 
   where id_modello = id_modellotrasm and hide_doc_versions = '1'; 
   
   if (not hide_versions is null and hide_versions > 0) then
       idObject := system_id;
       
       -- verifica se l'id fornito si riferisce ad un documento o fascicolo
       select count(*) into isDocument
       from profile p
       where p.system_id = idObject;
           
       if (isDocument > 0) then
        -- L'istanza su cui si sta applicando il modello e un documento,
        -- verifica se sia consolidato
            
            select p.consolidation_state into consolidationState
            from profile p
            where p.system_id = idObject;
                
            if (consolidationState is null or consolidationState = 0) then
                -- Il modello prevede di nascondere le versioni di un documento precedenti a quella corrente
                -- al destinatario della trasmissione, ma in tal caso il documento non e stato ancora consolidato,
                -- pertanto il modello non puo essere utilizzato
                retval := 0;
            end if;
       end if;

   end if;

    if (retval = 1) then
    --accesssRigth:= getaccessrights(id_ruolo, id_people, system_id);
       IF (accesssrigth = 45)
       THEN
          BEGIN
             OPEN cur;

             LOOP
                FETCH cur
                 INTO idragione;

                EXIT WHEN cur%NOTFOUND;

                SELECT cha_tipo_diritti
                  INTO tipo_diritto
                  FROM dpa_ragione_trasm
                 WHERE system_id = idragione;

                IF (tipo_diritto <> 'R' AND tipo_diritto <> 'C')
                THEN
                   BEGIN
                      retval := 0;
                   END;
                END IF;

                EXIT WHEN retval = 0;
             END LOOP;

             CLOSE cur;
          END;
       END IF;
    end if;       

   RETURN retval;
END getifmodelloautorizzato; 

/
