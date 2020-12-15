--------------------------------------------------------
--  Ref Constraints for Table PUBLISHER_INSTANCES
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PUBLISHER_INSTANCES" ADD CONSTRAINT "FK_IDADMIN" FOREIGN KEY ("IDADMIN")
	  REFERENCES "ITCOLL_6GIU12"."DPA_AMMINISTRA" ("SYSTEM_ID") ON DELETE CASCADE ENABLE NOVALIDATE;
