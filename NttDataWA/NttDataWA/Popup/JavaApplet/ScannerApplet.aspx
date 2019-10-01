<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScannerApplet.aspx.cs" Inherits="NttDataWA.Popup.JavaApplet.ScannerApplet" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="background-color: gray;">
    <form id="form1" runat="server">
    <DIV style="text-align: center;">
    	<object  classid="clsid:8AD9C840-044E-11D1-B3E9-00805F499D93" width="100%" height="85%">
            <param name="id" value="scannerJavaApplet" />
            <param name="name" value="scannerJavaApplet" />
            <param name="code" value="it.integra.scannerApplet.gui.ScanApplet" />
            <param name="codebase" value="http://localhost/NttDataWA/Libraries/" />
            <param name="archive" value="applet.jar,Libs/morena.jar,Libs/morena_windows.jar,Libs/morena_osx.jar,Libs/morena_license.jar,Libs/iText-2.1.7.jar,Libs/jai_codec.jar,Libs/jai_core.jar" />
            <param name="data" value="ScanApplet.class" />
            <param name="type" value="application/x-java-applet;version=1.7" />
            <param name="codetype=" value="application/x-java-applet;version=1.7" />
            <param name="FileType" value="0" />
			<param name="ApplyImageCompression" value="true" />
        </object> 
    </DIV>
    </form>
</body>
</html>
