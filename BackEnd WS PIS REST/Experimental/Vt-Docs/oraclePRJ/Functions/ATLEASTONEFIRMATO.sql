--------------------------------------------------------
--  DDL for Function ATLEASTONEFIRMATO
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."ATLEASTONEFIRMATO" (docnum number)
RETURN VARCHAR2
IS
isFirmato   VARCHAR2 (16);
BEGIN

DECLARE
vmaxidgenerica   NUMBER;

BEGIN
    SELECT MAX (v1.version_id)
    INTO vmaxidgenerica
    FROM VERSIONS v1, components c
    WHERE v1.docnumber = docnum AND v1.version_id = c.version_id;
  
    SELECT cha_firmato
    INTO isFirmato
    FROM components
    WHERE docnumber = docnum AND version_id = vmaxidgenerica;
end;

   if(isFirmato = '0') then
    begin
        declare item varchar(255);
        maxVersion number;
        CURSOR curAllegato IS select system_id from profile where id_documento_principale = docnum; 
        BEGIN
            OPEN curAllegato;
            LOOP
            FETCH curAllegato INTO item;
            EXIT WHEN (curAllegato%NOTFOUND or isFirmato='1');
               SELECT MAX (v1.version_id)
               INTO maxVersion
               FROM VERSIONS v1, components c
               WHERE v1.docnumber = item AND v1.version_id = c.version_id;
               
               SELECT cha_firmato
               INTO isFirmato
               FROM components
               WHERE docnumber = item AND version_id = maxVersion;
            END LOOP;
            CLOSE curAllegato;
        END;
    END;    
    end if;
   
RETURN isFirmato;

exception
when no_data_found
then
isFirmato :=  '0';
RETURN isFirmato;
when others
then
isFirmato := '0'; 
RETURN isFirmato;

End AtLeastOneFirmato; 

/
