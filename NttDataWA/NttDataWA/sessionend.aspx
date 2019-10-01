<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sessionend.aspx.cs" Inherits="NttDataWA.sessionend" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>sessionend</title>

    <script language="JavaScript" type="text/javascript">
        if (navigator.appCodeName != "Mozilla") {
            if (window.addEventListener) { // all browsers except IE before version 9
                window.addEventListener("beforeunload", OnBeforeUnLoad, false);
            } else {
                if (window.attachEvent) { // IE before version 9
                    window.attachEvent("onbeforeunload", OnBeforeUnLoad);
                }
            }
        }

        function OnBeforeUnLoad(my_event) {
            if (my_event) {
                //if (document.getElementById('user_id') != null) {
                    //if (document.getElementById('user_id').value != '') {
                        window.open("LogOut.aspx?param=CloseWindow",
                            "dummy",
                            "height = 0, width = 0, modal=yes, alwaysRaised=yes");
                    //}
                //}
            }
        }
    </script>

</head>
<body>
<form id="SessionEnd" runat="server">
    <input type="hidden" id="user_id"/>
</form>
</body>
</html>