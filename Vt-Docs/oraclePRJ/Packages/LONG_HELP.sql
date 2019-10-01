--------------------------------------------------------
--  DDL for Package LONG_HELP
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE "ITCOLL_6GIU12"."LONG_HELP" 
    authid current_user
    as
       function substr_of
       ( p_query in varchar2,
          p_from  in number,
          p_for   in number,
         p_name1 in varchar2 default NULL,
         p_bind1 in varchar2 default NULL,
         p_name2 in varchar2 default NULL,
         p_bind2 in varchar2 default NULL,
         p_name3 in varchar2 default NULL,
         p_bind3 in varchar2 default NULL,
         p_name4 in varchar2 default NULL,
         p_bind4 in varchar2 default NULL )
       return varchar2;
   End;
   

/

--------------------------------------------------------
--  DDL for Package Body LONG_HELP
--------------------------------------------------------

  CREATE OR REPLACE PACKAGE BODY "ITCOLL_6GIU12"."LONG_HELP" 
    as
       g_cursor number := dbms_sql.open_cursor;
       G_Query  Varchar2(32765);
 
 procedure bind_variable( p_name in varchar2, p_value in varchar2 )
    is    begin
      if ( p_name is not null )      then
       dbms_sql.bind_variable( g_cursor, p_name, p_value );
      End If;
  End;  
  
  function substr_of
   ( p_query in varchar2,
     p_from  in number,
     p_for   in number,
     p_name1 in varchar2 default NULL,
     p_bind1 in varchar2 default NULL,
     p_name2 in varchar2 default NULL,
     p_bind2 in varchar2 default NULL,
     p_name3 in varchar2 default NULL,
     p_bind3 in varchar2 default NULL,
     p_name4 in varchar2 default NULL,
     p_bind4 in varchar2 default NULL )
   return varchar2
   as
       l_buffer       varchar2(4000);
       l_buffer_len   number;
   begin

-- first thing our code does is a sanity check on the P_FROM and P_FOR inputs.  P_FROM must 
-- be a number greater than or equal to 1 and P_FOR must be between 1 and 4000 - just like the built-in function SUBSTR:

       if ( nvl(p_from,0) <= 0 )        then
           raise_application_error
           (-20002, 'From must be >= 1 (positive numbers)' );
       end if;
       if ( nvl(p_for,0) not between 1 and 4000 )        then
           raise_application_error
           (-20003, 'For must be between 1 and 4000' );
       End If;
  if ( p_query <> g_query or g_query is NULL )
       then
           if ( upper(trim(nvl(p_query,'x'))) not like 'SELECT%')
           then
               raise_application_error
               (-20001, 'This must be a select only' );
           end if;
           dbms_sql.parse( g_cursor, p_query, dbms_sql.native );
           G_Query := P_Query;
       end if;
         bind_variable( p_name1, p_bind1 );
       bind_variable( p_name2, p_bind2 );
       Bind_Variable( P_Name3, P_Bind3 );
       bind_variable( p_name4, p_bind4 );
       
       dbms_sql.define_column_long(g_cursor, 1);
       if (dbms_sql.execute_and_fetch(g_cursor)>0)
       then
           dbms_sql.column_value_long
           (g_cursor, 1, p_for, p_from-1,
            l_buffer, l_buffer_len );
       end if;
       return l_buffer;
   end substr_of;
 
   End;
   

/