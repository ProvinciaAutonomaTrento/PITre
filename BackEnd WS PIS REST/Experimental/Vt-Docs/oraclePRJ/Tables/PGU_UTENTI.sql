--------------------------------------------------------
--  DDL for Table PGU_UTENTI
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."PGU_UTENTI" 
   (	"ID" NUMBER(*,0), 
	"NOME" VARCHAR2(50 BYTE), 
	"DESCRIZIONE" VARCHAR2(255 BYTE), 
	"PASSWORD" VARCHAR2(50 BYTE), 
	"AMMINISTRATORE" CHAR(1 BYTE), 
	"SECRETKEY" VARCHAR2(255 BYTE), 
	"SECRETIV" VARCHAR2(255 BYTE), 
	"SUPERAMMINISTRATORE" CHAR(1 BYTE)
   ) ;
