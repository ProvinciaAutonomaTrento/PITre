--------------------------------------------------------
--  DDL for Table UTL_SYSTEM_LOG
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."UTL_SYSTEM_LOG" 
   (	"ID" NUMBER(*,0), 
	"DATA_OPERAZIONE" DATE, 
	"COMANDO_RICHIESTO" VARCHAR2(2000 BYTE), 
	"CATEGORIA_COMANDO" VARCHAR2(2000 BYTE), 
	"ESITO_OPERAZIONE" VARCHAR2(2000 BYTE)
   ) ;
