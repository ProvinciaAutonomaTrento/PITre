--------------------------------------------------------
--  Ref Constraints for Table PUBLISHER_EVENTS
--------------------------------------------------------

  ALTER TABLE "ITCOLL_6GIU12"."PUBLISHER_EVENTS" ADD CONSTRAINT "FK_PUBLISHERINSTANCEID" FOREIGN KEY ("PUBLISHINSTANCEID")
	  REFERENCES "ITCOLL_6GIU12"."PUBLISHER_INSTANCES" ("ID") ON DELETE CASCADE ENABLE NOVALIDATE;
