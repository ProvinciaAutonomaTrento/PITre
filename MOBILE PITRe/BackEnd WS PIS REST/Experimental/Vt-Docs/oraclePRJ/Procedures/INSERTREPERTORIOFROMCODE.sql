--------------------------------------------------------
--  DDL for Procedure INSERTREPERTORIOFROMCODE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "ITCOLL_6GIU12"."INSERTREPERTORIOFROMCODE" (
  tipologyId In Integer,
  counterId In Integer,
  counterType In Varchar,
  returnValue Out Integer) AS 
BEGIN
  INSERTREGISTROREPERTORIO (tipologyId, counterId, counterType, 'D');
  
  returnValue := 0;

END InsertRepertorioFromCode;

/
