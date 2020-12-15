--------------------------------------------------------
--  DDL for Table DPA_VIS_MAIL_REGISTRI
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_VIS_MAIL_REGISTRI" 
   (	"SYSTEM_ID" NUMBER(10,0), 
	"ID_REGISTRO" NUMBER(10,0), 
	"ID_RUOLO_IN_UO" NUMBER(10,0), 
	"VAR_EMAIL_REGISTRO" VARCHAR2(128 BYTE), 
	"CHA_CONSULTA" VARCHAR2(1 BYTE) DEFAULT '1', 
	"CHA_NOTIFICA" VARCHAR2(1 BYTE) DEFAULT '1', 
	"CHA_SPEDISCI" VARCHAR2(1 BYTE) DEFAULT '1'
   ) ;
