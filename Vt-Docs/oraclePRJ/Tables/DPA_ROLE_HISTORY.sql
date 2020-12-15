--------------------------------------------------------
--  DDL for Table DPA_ROLE_HISTORY
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_ROLE_HISTORY" 
   (	"SYSTEM_ID" NUMBER, 
	"ORIGINAL_CORR_ID" NUMBER, 
	"ACTION" CHAR(1 BYTE), 
	"ROLE_DESCRIPTION" VARCHAR2(384 BYTE), 
	"ROLE_TYPE_ID" NUMBER, 
	"ACTION_DATE" DATE, 
	"UO_ID" NUMBER(*,0), 
	"UO_DESCRIPTION_" VARCHAR2(4000 BYTE), 
	"ROLE_TYPE_DESCRIPTION_" VARCHAR2(4000 BYTE), 
	"ROLE_ID" NUMBER
   ) ;
