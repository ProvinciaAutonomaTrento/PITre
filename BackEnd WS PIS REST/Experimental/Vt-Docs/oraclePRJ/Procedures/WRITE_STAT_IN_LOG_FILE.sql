--------------------------------------------------------
--  DDL for Procedure WRITE_STAT_IN_LOG_FILE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."WRITE_STAT_IN_LOG_FILE" (
   log_file_name      IN OUT   UTL_FILE.file_type,
   xtot_records       IN       NUMBER,
   xins_records       IN       NUMBER,
   xupd_records       IN       NUMBER,
   xinvalid_rec_ctr   IN       NUMBER
)
IS
BEGIN
   UTL_FILE.put_line (log_file_name, ' ');
   UTL_FILE.put_line
           (log_file_name,
            '***************************************************************'
           );
   UTL_FILE.put_line (log_file_name,
                      'Numero Record  Processati : ' || xtot_records
                     );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line (log_file_name,
                      'Numero Record  Inseriti : ' || xins_records
                     );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line (log_file_name,
                      'Numero Record  Aggiornati : ' || xupd_records
                     );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line (log_file_name,
                      'Numero Record  Scartati : ' || xinvalid_rec_ctr
                     );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line
            (log_file_name,
             '***************************************************************'
            );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.fflush (log_file_name);
END write_stat_in_log_file; 

/
