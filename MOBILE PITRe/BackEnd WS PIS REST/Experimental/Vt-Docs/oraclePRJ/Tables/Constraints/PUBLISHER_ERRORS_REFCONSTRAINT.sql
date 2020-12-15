--------------------------------------------------------
--  Ref Constraints for Table PUBLISHER_ERRORS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PUBLISHER_ERRORS" ADD CONSTRAINT "FK_PUBLISHER_ERRORS" FOREIGN KEY ("PUBLISHINSTANCEID")
	  REFERENCES "ITCOLL_6GIU12"."PUBLISHER_INSTANCES" ("ID") ON DELETE CASCADE ENABLE NOVALIDATE;
