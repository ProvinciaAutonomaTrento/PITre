--------------------------------------------------------
--  DDL for Table DPA_TIMESTAMP_DOC
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_TIMESTAMP_DOC" 
   (	"SYSTEM_ID" NUMBER(*,0), 
	"DOC_NUMBER" NUMBER(*,0), 
	"VERSION_ID" NUMBER(*,0), 
	"ID_PEOPLE" NUMBER(*,0), 
	"DTA_CREAZIONE" DATE, 
	"DTA_SCADENZA" DATE, 
	"NUM_SERIE" VARCHAR2(64 BYTE), 
	"S_N_CERTIFICATO" VARCHAR2(64 BYTE), 
	"ALG_HASH" VARCHAR2(64 BYTE), 
	"SOGGETTO" VARCHAR2(64 BYTE), 
	"PAESE" VARCHAR2(64 BYTE), 
	"TSR_FILE" CLOB
   ) ;
