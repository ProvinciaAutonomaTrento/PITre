--------------------------------------------------------
--  DDL for Table DPA_COPY_LOG
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_COPY_LOG" 
   (	"THING" NUMBER, 
	"ID_RUOLO_DEST" NUMBER, 
	"ID_RUOLO_ORIG" NUMBER, 
	"ACCESSRIGHTS" NUMBER, 
	"CHA_TIPO_DIRITTO" CHAR(1 BYTE), 
	"VAR_NOTE_SEC" VARCHAR2(256 BYTE), 
	"TS_COPY" TIMESTAMP (6) DEFAULT SYSTIMESTAMP
   ) ;
