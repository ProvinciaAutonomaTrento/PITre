begin
--Update Apps	Set Mime_Type='text/plain'  where application in ('PUB XML','GEN_XML')  and Mime_Type <> 'text/plain';

Update Apps
	Set Mime_Type = 'application/msword'
where Application = 'MS WORD'				and Mime_Type <>  'application/msword';

end;
/


