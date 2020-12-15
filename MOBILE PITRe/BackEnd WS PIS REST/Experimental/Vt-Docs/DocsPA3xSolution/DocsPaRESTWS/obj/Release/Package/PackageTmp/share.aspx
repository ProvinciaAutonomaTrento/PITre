<%@ Page Language="C#" %>
<script runat="server">
  protected override void OnLoad(EventArgs e)
  {
    string key = HttpUtility.UrlEncodeUnicode(Request.QueryString["key"]);
    Response.Redirect("p3it://informaticatrentina.pi3.it/share/"+key);
  }
</script>
