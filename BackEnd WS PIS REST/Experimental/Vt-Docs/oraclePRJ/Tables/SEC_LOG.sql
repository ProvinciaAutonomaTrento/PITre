--------------------------------------------------------
--  DDL for Table SEC_LOG
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."SEC_LOG" 
   (	"THING" NUMBER(10,0), 
	"PERSONORGROUP" NUMBER(10,0), 
	"ACCESSRIGHTS" NUMBER(10,0), 
	"ID_GRUPPO_TRASM" NUMBER(10,0), 
	"CHA_TIPO_DIRITTO" VARCHAR2(1 BYTE), 
	"HIDE_DOC_VERSIONS" CHAR(1 BYTE), 
	"TS_INSERIMENTO" TIMESTAMP (6), 
	"VAR_NOTE_SEC" VARCHAR2(255 BYTE)
   ) ;
