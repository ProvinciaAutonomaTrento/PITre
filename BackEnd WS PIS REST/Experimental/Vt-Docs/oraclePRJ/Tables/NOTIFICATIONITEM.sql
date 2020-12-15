--------------------------------------------------------
--  DDL for Table NOTIFICATIONITEM
--------------------------------------------------------

  CREATE TABLE "ITCOLL_6GIU12"."NOTIFICATIONITEM" 
   (	"ID" NUMBER, 
	"AUTHOR" VARCHAR2(100 BYTE), 
	"TITLE" VARCHAR2(2000 BYTE), 
	"TEXT" VARCHAR2(2000 BYTE), 
	"FEEDLINK" VARCHAR2(1000 BYTE), 
	"LASTUPDATE" DATE, 
	"PUBLISHDATE" DATE, 
	"MESSAGEID" NUMBER, 
	"MESSAGENUMBER" NUMBER(*,0)
   ) ;
