--------------------------------------------------------
--  DDL for Function GETNOMERUOLOBYSYSID
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETNOMERUOLOBYSYSID" (my_system_id INT)
   RETURN VARCHAR
IS
   risultato   VARCHAR (256);
BEGIN
   SELECT group_name
     INTO risultato
     FROM GROUPS g
    WHERE system_id = my_system_id;

   RETURN risultato;
END getnomeruolobysysid; 

/
