using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace StampaRegistri.Properties
{
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "8.0.0.0"), CompilerGenerated]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		[ApplicationScopedSetting, DefaultSettingValue("http://localhost/Docspa/v2.0_std/dev/BackEnd/DocsPaWS/DocsPaWS.asmx"), SpecialSetting(SpecialSetting.WebServiceUrl), DebuggerNonUserCode]
		public string StampaRegistri_DocsPaWR25_DocsPaWebService
		{
			get
			{
				return (string)this["StampaRegistri_DocsPaWR25_DocsPaWebService"];
			}
		}

		[ApplicationScopedSetting, DefaultSettingValue("http://localhost/DocsPa310/DocsPaWS/DocsPaWS.asmx"), SpecialSetting(SpecialSetting.WebServiceUrl), DebuggerNonUserCode]
		public string StampaRegistri_DocsPaWR305_DocsPaWebService
		{
			get
			{
				return (string)this["StampaRegistri_DocsPaWR305_DocsPaWebService"];
			}
		}
	}
}
