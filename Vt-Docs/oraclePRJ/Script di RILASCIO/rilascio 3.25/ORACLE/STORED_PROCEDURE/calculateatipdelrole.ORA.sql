begin 
Utl_Backup_Plsql_code ('PROCEDURE','calculateatipdelrole'); 
end;
/


create or replace
PROCEDURE calculateatipdelrole (
   -- Id corr globali della UO cui apparteneva il ruolo eliminato
   iduo          IN       NUMBER,
   -- Id dell'amministrazione cui appartiene la UO
   idamm         IN       NUMBER,
   -- Id del livello del ruolo eliminato
   rolelevelid   IN       NUMBER,
   returnvalue   OUT      NUMBER
)
AS
BEGIN
     /******************************************************************************

     AUTHOR:   Samuele Furnari

     NAME:     CalculateAtipDelRole

     PURPOSE:  Store per il calcolo dell'atipicit¿ di documenti e fascicoli di un ruolo
               e dei suoi sottoposti. Questo calcolo viene eseguito quando si elimina
               un ruolo

   ******************************************************************************/

   -- Id del tipo ruolo del ruolo eliminato
   DECLARE
      rolelevel      NUMBER;
      keyvalue       VARCHAR (200)  := '';
      -- Query per l'estrazione degli id degli oggetti visti dai ruoli presenti nella
      -- UO del ruolo eliminato che abbiano livello inferiore o uguale a quello
      -- del ruolo elminato
      rolesobjects   VARCHAR (2000);
   BEGIN
      -- Selezione del livello ruolo
      SELECT num_livello
        INTO rolelevel
        FROM dpa_tipo_ruolo
       WHERE system_id = rolelevelid;

      rolesobjects :=
            'Select distinct(s.thing)
      From security s
      Inner Join profile p
      On p.system_id = s.thing
      Where personorgroup In (
        Select distinct(p.id_gruppo)
        From dpa_corr_globali p
        Where id_amm = '
         || idamm
         || '
        And ((Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= '
         || rolelevel
         || ')
        And p.id_uo In (
          Select p.SYSTEM_ID
          From dpa_corr_globali p
          Start With p.SYSTEM_ID ='
         || iduo
         || '
          Connect By Prior
          p.SYSTEM_ID = p.ID_PARENT
          And p.CHA_TIPO_URP = ''U''
          And p.ID_AMM = '
         || idamm
         || '))';

      -- Esecuzione calcolo di atipicit¿ su documenti e fascicoli solo se attiva
       -- Recupero dello stato di abilitazione del calcolo di atipicit¿
      SELECT var_valore
        INTO keyvalue
        FROM dpa_chiavi_configurazione
       WHERE var_codice = 'ATIPICITA_DOC_FASC'
         AND (id_amm = 0 OR id_amm = idamm)
         AND ROWNUM = 1;

      IF keyvalue = '1'
      THEN
         BEGIN
            vis_doc_anomala_custom (idamm, rolesobjects);
            rolesobjects :=
                  'Select distinct(s.thing)
          From security s
          Inner Join project p
          On p.system_id = s.thing
          Where p.cha_tipo_fascicolo = ''P''
            And personorgroup In (
            Select distinct(p.id_gruppo)
            From dpa_corr_globali p
            Where id_amm = '
               || idamm
               || '
            And ((Select num_livello From dpa_tipo_ruolo Where system_id = p.id_tipo_ruolo) >= '
               || rolelevel
               || ')
            And p.id_uo In (
              Select p.SYSTEM_ID
              From dpa_corr_globali p
              Start With p.SYSTEM_ID ='
               || iduo
               || '
              Connect By Prior
              p.SYSTEM_ID = p.ID_PARENT
              And p.CHA_TIPO_URP = ''U''
              And p.ID_AMM = '
               || idamm
               || '))';
            vis_fasc_anomala_custom (idamm, rolesobjects);
         END;
      END IF;

      returnvalue := 1;
   END;
END calculateatipdelrole;
/
