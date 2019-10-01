--------------------------------------------------------
--  DDL for Table DPA_GRIDS
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_GRIDS" 
   (	"SYSTEM_ID" NUMBER(*,0), 
	"USER_ID_CREATORE" NUMBER(*,0), 
	"ROLE_ID_CREATORE" NUMBER(*,0), 
	"ADMINISTRATION_ID" NUMBER(*,0), 
	"SEARCH_ID" NUMBER(*,0), 
	"SERIALIZED_GRID" CLOB, 
	"TYPE_GRID" VARCHAR2(30 BYTE), 
	"IS_SEARCH_GRID" CHAR(1 BYTE) DEFAULT 'Y', 
	"GRID_NAME" VARCHAR2(100 BYTE), 
	"CHA_VISIBILE_A_UTENTE_O_RUOLO" CHAR(1 BYTE)
   ) ;
