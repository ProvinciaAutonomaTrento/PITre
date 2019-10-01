select  sess_id,
        username,
        program,
        wait_event,
        sess_time,
        round(100 * (sess_time / total_time),2) pct_time_waited
from
(select a.session_id sess_id,
        decode(session_type,'background',session_type,c.username) username,
        a.program program,
        b.name wait_event,
        sum(a.time_waited) sess_time
from    sys.v_$active_session_history a,
        sys.v_$event_name b,
        sys.dba_users c
where   a.event# = b.event# and
        a.user_id = c.user_id and
        sample_time > sysdate - 1/48 and 
        sample_time < sysdate and
        b.wait_class = 'User I/O'
group by a.session_id,
        decode(session_type,'background',session_type,c.username),
        a.program,
        b.name),
(select sum(a.time_waited) total_time
from    sys.v_$active_session_history a,
        sys.v_$event_name b
where   a.event# = b.event# and
       sample_time > sysdate - 1/48 and 
        sample_time < sysdate and
         b.wait_class = 'User I/O')
order by 6 desc;
