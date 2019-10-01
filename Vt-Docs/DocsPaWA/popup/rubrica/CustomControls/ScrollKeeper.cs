using System;
using System.Web.UI;

namespace DocsPAWA.popup.RubricaDocsPA
{
    /// <exclude></exclude>
	public class ScrollKeeper : 
		System.Web.UI.HtmlControls.HtmlInputHidden, 
		IPostBackDataHandler
	{
		public string WebControl;

		public int VPos
		{
			get 
			{
				if(ViewState["VPos"]==null)
				{
					ViewState["VPos"] = 0;
				}
				return (int)ViewState["VPos"];
			}
			set { ViewState["VPos"] = value; }
		}

		public int HPos
		{
			get
			{
				if(ViewState["HPos"]==null)
				{
					ViewState["HPos"] = 0;
				}
				return (int)ViewState["HPos"];
			}
			set { ViewState["HPos"] = value; }
		}

		public bool LoadPostData(String postDataKey, 
			System.Collections.Specialized.NameValueCollection values) 
		{
			bool _returnV;
			bool _returnH;
        
			string Val = values[this.UniqueID].Trim();
			char Eperluette = Char.Parse("&");
			string[] _Val = Val.Split(Eperluette);
			if(_Val.Length>1)
			{
				if(!HPos.ToString().Equals(_Val[0])  && _Val[0].Trim()!=null )
				{
					HPos= Int32.Parse(_Val[0]);
					_returnH=true;  
				}
				else _returnH=false;

				if(!VPos.ToString().Equals(_Val[1])  && _Val[1].Trim()!=null )
				{
					VPos= Int32.Parse(_Val[1]);
					_returnV=true;  
				}
				else _returnV=false;  
			}
			else
			{
				HPos = 0;
				VPos=0;
				return false;
			}

			//_______return_________

			if(_returnV || _returnH) return false; 

			else return false;    
		}

		public void RaisePostDataChangedEvent() 
		{
		}    

		protected override void OnPreRender(EventArgs e) 
		{        
			string ctl = "document.getElementById('" + WebControl + "')";
			this.Attributes.Add("id", this.ClientID);
			string _start = "<script language='Javascript'>" + ctl + ".onscroll = function(){";
				
				_start += "document.getElementById('" + this.UniqueID + 
					"').value = " + ctl + ".scrollLeft+'&'+ " + ctl + ".scrollTop;";
				_start += "}</script>";
			if(!Page.IsStartupScriptRegistered("_scrollkeeper_start"))
			{
				Page.RegisterStartupScript("_scrollkeeper_start",_start);
			}

			string _goScroll = "<script language='javascript'>" +
				ctl + ".scrollLeft= " + this.HPos + "; " +
				ctl + ".scrollTop = " + this.VPos + "; " +
				"</script>";
			if(!Page.IsStartupScriptRegistered("_scrollkeeper_execute"))
			{
				Page.RegisterStartupScript("_scrollkeeper_execute",_goScroll);
			}
		}    
	}
}