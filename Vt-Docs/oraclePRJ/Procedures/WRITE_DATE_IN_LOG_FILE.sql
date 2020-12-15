--------------------------------------------------------
--  DDL for Procedure WRITE_DATE_IN_LOG_FILE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."WRITE_DATE_IN_LOG_FILE" (
   log_file_name   IN OUT   UTL_FILE.file_type,
   action          IN       VARCHAR2
)
IS
BEGIN
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line
           (log_file_name,
            '***************************************************************'
           );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line (log_file_name, action);
   UTL_FILE.put_line (log_file_name,
                      '   ' || TO_CHAR (SYSDATE, 'MM/DD/YY HH24:MI:SS')
                     );
   UTL_FILE.put_line (log_file_name, '');
   UTL_FILE.put_line
            (log_file_name,
             '***************************************************************'
            );
   UTL_FILE.put_line (log_file_name, '');
END write_date_in_log_file; 

/
