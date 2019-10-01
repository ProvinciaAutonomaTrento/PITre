<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page1.aspx.cs" Inherits="AjaxPopupControl.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
<base target="_self" />
    <title></title>
	<script src="http://code.jquery.com/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/FDB_tools.js" type="text/javascript"></script>
    <link href="css/flick/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form runat="server">
    <div>
        

<p>ROMA - La spesa delle famiglie sul territorio nazionale nel secondo trimestre 2012 ha registrato un calo del 3,5%, dovuto a diminuzioni del 10,1% degli acquisti di beni durevoli, del 3,5% per quelli non durevoli e dell'1,1% per gli acquisiti di servizi. Lo comunica l'Istat, comunicando i dati sul Pil.</p>

<p>L'Istat ha rivisto al ribasso il dato sul Pil nel secondo trimestre 2012: il calo è stato dello 0,8% rispetto al trimestre precedente e del 2,6% nei confronti del secondo trimestre 2011, rispetto alla stima preliminare, diffusa ad agosto, che indicava un calo congiunturale dello 0,7% e su base annua del 2,5%.</p>

<p>Il calo del Pil del 2,6%, registrato nel secondo trimestre 2012 rispetto allo stesso trimestre del 2011, è il dato peggiore dal quarto trimestre 2009, quando il calo era stato del 3,5%. Lo comunica l'Istat. Su base congiunturale il calo dello 0,8% invece era stato registrato anche nel primo trimestre dell'anno.</p>

<p>Nel secondo trimestre del 2012 tutti i grandi comparti di attività economica registrano una diminuzione congiunturale del valore aggiunto: -1,9% per l'agricoltura, -1,6% per l'industria e -0,5% per i servizi. Lo comunica l'Istat (con i dati sul Pil), aggiungendo che in termini tendenziali il valore aggiunto è aumentato dello 0,9% nell'agricoltura, mentre è diminuito del 6% nell'industria in senso stretto, del 6,5% nelle costruzioni e dell'1,1% nel complesso dei servizi.</p>
        <website:ajaxpopup2 ID="filippo2" runat="server"
            text="Apri altro popup in basso a destra"
            title="Titolo secondo popup"
            url="page2.aspx"
            width="600"
            height="400"
            posh="right"
            posv="bottom"
         />
         <asp:Button ID="Button2" runat="server" Text="Button" onclientclick="return ajaxModalPopupfilippo2();" />
         <asp:Button ID="Button1" runat="server" Text="Visualizza valore di ritorno" onclick="Button1_Click" />
         <asp:Label ID="Label1" runat="server" Text=""></asp:Label><br />
         <input type="button" value="Chiudi senza modificare il valore di ritorno" onclick="closeAjaxModal('<%=Request.QueryString["popupid"] %>')" />
    </div>
    </form>
</body>
</html>
