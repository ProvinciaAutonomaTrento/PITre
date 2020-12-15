CREATE OR REPLACE FUNCTION @db_user.getPeopleName (peopleId INT)
RETURN varchar IS risultato varchar(256);
BEGIN

select full_name into risultato from people where system_id = peopleId;

RETURN risultato;
END getPeopleName;
/