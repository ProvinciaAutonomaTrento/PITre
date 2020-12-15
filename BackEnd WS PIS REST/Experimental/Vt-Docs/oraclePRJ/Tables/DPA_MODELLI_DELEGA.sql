--------------------------------------------------------
--  DDL for Table DPA_MODELLI_DELEGA
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_MODELLI_DELEGA" 
   (	"SYSTEM_ID" NUMBER, 
	"ID_PEOPLE_DELEGANTE" NUMBER, 
	"ID_RUOLO_DELEGANTE" NUMBER, 
	"ID_PEOPLE_DELEGATO" NUMBER, 
	"ID_RUOLO_DELEGATO" NUMBER, 
	"INTERVALLO" NUMBER(*,0), 
	"DTA_INIZIO" DATE, 
	"DTA_FINE" DATE, 
	"NOME" VARCHAR2(100 BYTE)
   ) ;
