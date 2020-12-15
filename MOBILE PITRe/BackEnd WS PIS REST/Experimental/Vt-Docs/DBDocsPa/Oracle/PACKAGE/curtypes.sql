CREATE OR REPLACE PACKAGE @db_user.curtypes
AS
TYPE InteropRefCursor IS REF CURSOR;
END;
/