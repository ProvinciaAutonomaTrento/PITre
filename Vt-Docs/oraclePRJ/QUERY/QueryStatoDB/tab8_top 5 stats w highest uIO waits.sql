select *
from
(select sql_text,
        sql_id,
        elapsed_time,
        cpu_time,
        user_io_wait_time
from    sys.v_$sqlarea
order by 5 desc)
where rownum < 6;