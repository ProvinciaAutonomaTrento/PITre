--------------------------------------------------------
--  DDL for Function TMP_GET_ELAPSED
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."TMP_GET_ELAPSED" (start_time IN timestamp, end_time IN timestamp,  elapsed OUT number)
return number 
is  
begin
select sum(
    (extract(hour from end_time)-extract(hour from start_time))*3600+
    (extract(minute from end_time)-extract(minute from start_time))*60+
   extract(second from end_time)-extract(second from start_time))*1000 ms into  elapsed  from dual ;
  end; 

/
