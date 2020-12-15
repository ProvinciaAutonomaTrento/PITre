--------------------------------------------------------
--  DDL for Function GETLONG
--------------------------------------------------------

  CREATE OR REPLACE FUNCTION "ITCOLL_6GIU12"."GETLONG" ( p_tname in varchar2,
                                         p_cname in varchar2,
                                         p_rowid in rowid ) return varchar2
    as
        l_cursor    integer default dbms_sql.open_cursor;
        l_n         number;
        l_long_val  varchar2(4000);
        l_long_len  number;
        l_buflen    number := 4000;
       l_curpos    number := 0;
   begin
       dbms_sql.parse( l_cursor,
                      'select ' || p_cname || ' from ' || p_tname ||
                                                        ' where rowid = :x',
                       dbms_sql.native );
       dbms_sql.bind_variable( l_cursor, ':x', p_rowid );
   
       dbms_sql.define_column_long(l_cursor, 1);
       l_n := dbms_sql.execute(l_cursor);
   
       if (dbms_sql.fetch_rows(l_cursor)>0)
       then
          dbms_sql.column_value_long(l_cursor, 1, l_buflen, l_curpos ,
                                     l_long_val, l_long_len );
      end if;
      dbms_sql.close_cursor(l_cursor);
      return l_long_val;
   End Getlong;

/
