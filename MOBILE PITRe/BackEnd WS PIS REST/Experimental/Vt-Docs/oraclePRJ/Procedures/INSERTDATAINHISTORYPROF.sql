--------------------------------------------------------
--  DDL for Procedure INSERTDATAINHISTORYPROF
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INSERTDATAINHISTORYPROF" (
   objtype              VARCHAR,
   idtemplate           VARCHAR,
   iddocorfasc          VARCHAR,
   idoggcustom          VARCHAR,
   idpeople             VARCHAR,
   idruoloinuo          VARCHAR,
   descmodifica         VARCHAR,
   returnvalue    OUT   NUMBER
)
AS
BEGIN
   /******************************************************************************

      AUTHOR:   Samuele Furnari

      NAME:     InsertDataInHistoryProf

      PURPOSE:  Store per l'inserimento di una voce nello storico dei campi
                profilati di documenti / fascicoli.

    ******************************************************************************/
   BEGIN
      DECLARE
         enhis   CHAR := '';
      -- Verifica del flag di attivazione storico per il campo
      BEGIN
         IF objtype = 'D'
         THEN
            SELECT enabledhistory
              INTO enhis
              FROM dpa_ogg_custom_comp
             WHERE id_ogg_custom = idoggcustom AND id_template = idtemplate;
         ELSE
            SELECT enabledhistory
              INTO enhis
              FROM dpa_ogg_custom_comp_fasc
             WHERE id_ogg_custom = idoggcustom AND id_template = idtemplate;
         END IF;

         -- Se e attiva la storicizzazione del campo, viene inserita una riga nello storico
         IF (enhis = '1')
         THEN
            BEGIN
               -- Se l'oggetto da storicizzare e un documento, viene inserita una riga
               -- nello storico dei documenti, altrimenti viene inserita in quella dei
               -- fascicoli
               IF objtype = 'D'
               THEN
                  INSERT INTO dpa_profil_sto
                              (systemid, id_template,
                               dta_modifica, id_profile, id_ogg_custom,
                               id_people, id_ruolo_in_uo, var_desc_modifica
                              )
                       VALUES (seq_dpa_profil_sto.NEXTVAL, idtemplate,
                               SYSDATE, iddocorfasc, idoggcustom,
                               idpeople, idruoloinuo, descmodifica
                              );
               ELSE
                  INSERT INTO dpa_profil_fasc_sto
                              (systemid, id_template,
                               dta_modifica, id_project, id_ogg_custom,
                               id_people, id_ruolo_in_uo, var_desc_modifica
                              )
                       VALUES (seq_dpa_profil_sto.NEXTVAL, idtemplate,
                               SYSDATE, iddocorfasc, idoggcustom,
                               idpeople, idruoloinuo, descmodifica
                              );
               END IF;
            END;
         END IF;
      END;

      returnvalue := 0;
   END;
END insertdatainhistoryprof; 

/
