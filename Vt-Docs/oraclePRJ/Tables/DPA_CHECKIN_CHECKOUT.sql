--------------------------------------------------------
--  DDL for Table DPA_CHECKIN_CHECKOUT
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."DPA_CHECKIN_CHECKOUT" 
   (	"SYSTEM_ID" NUMBER(*,0), 
	"ID_DOCUMENT" NUMBER(*,0), 
	"DOCUMENT_NUMBER" NUMBER(*,0), 
	"ID_USER" NUMBER(*,0), 
	"ID_ROLE" NUMBER(*,0), 
	"CHECK_OUT_DATE" DATE, 
	"DOCUMENT_LOCATION" NVARCHAR2(1000), 
	"MACHINE_NAME" NVARCHAR2(50)
   ) ;
