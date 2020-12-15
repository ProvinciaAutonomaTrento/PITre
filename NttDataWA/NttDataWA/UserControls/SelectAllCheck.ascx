<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectAllCheck.ascx.cs" Inherits="NttDataWA.UserControls.SelectAllCheck" %>
  <script type="text/javascript">

      function HeaderClick(CheckBox) {

          var TargetBaseControl = document.getElementById('<%= Grid%>');
          var TargetChildControl = "checkDocumento";
          if (TargetBaseControl != null) {
              var Inputs = TargetBaseControl.getElementsByTagName("input");

              for (var n = 0; n < Inputs.length; ++n)
                  if (Inputs[n].type == 'checkbox' &&
                    Inputs[n].id.indexOf(TargetChildControl, 0) >= 0)
                      Inputs[n].checked = CheckBox.checked;
          }
      }

      function ChildClick(CheckBox, HCheckBox) {
          var HeaderCheckBox = document.getElementById(HCheckBox);

          var TargetBaseControl = document.getElementById('<%= Grid%>');
          var TargetChildControl = "checkDocumento";
          var Inputs = TargetBaseControl.getElementsByTagName("input");
          HeaderCheckBox.checked = true;

          //verifica se almeno 1 non è checked
          for (var n = 0; n < Inputs.length; ++n)
              if (Inputs[n].type == 'checkbox' &&
                Inputs[n].id.indexOf(TargetChildControl, 0) >= 0)
                  if (Inputs[n].checked == false) {
                      HeaderCheckBox.checked = false;
                      break;
                  }
      }
    </script>