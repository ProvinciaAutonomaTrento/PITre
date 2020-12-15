--------------------------------------------------------
--  DDL for Table DELETED_SECURITY
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DELETED_SECURITY" 
   (	"THING" NUMBER(10,0), 
	"PERSONORGROUP" NUMBER(10,0), 
	"ACCESSRIGHTS" NUMBER(10,0), 
	"ID_GRUPPO_TRASM" NUMBER(10,0), 
	"CHA_TIPO_DIRITTO" VARCHAR2(1 BYTE), 
	"NOTE" VARCHAR2(128 BYTE), 
	"DTA_REVOCA" DATE, 
	"ID_UTENTE_REV" NUMBER(10,0), 
	"ID_RUOLO_REV" NUMBER(10,0), 
	"HIDE_DOC_VERSIONS" CHAR(1 BYTE)
   ) ;
