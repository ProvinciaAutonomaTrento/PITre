--------------------------------------------------------
--  DDL for Table COMPONENTS
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."COMPONENTS" 
   (	"PATH" VARCHAR2(500 BYTE), 
	"LOCKED" VARCHAR2(1 BYTE), 
	"COMPTYPE" VARCHAR2(3 BYTE), 
	"VERSION_ID" NUMBER(10,0), 
	"DOCNUMBER" NUMBER(10,0), 
	"FILE_SIZE" NUMBER(10,0), 
	"VAR_IMPRONTA" VARCHAR2(64 BYTE), 
	"EXT" VARCHAR2(256 BYTE) DEFAULT 0, 
	"CHA_FIRMATO" VARCHAR2(1 BYTE) DEFAULT 0
   ) ;
