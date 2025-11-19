// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
	public sealed class LocalDtdResolver : XmlResolver
	{
		public XmlResolver sytemResolver = null;
		public LocalDtdResolver()
		{
			if (sytemResolver == null)
			{
				sytemResolver = new XmlUrlResolver();

			}
		}

		public override System.Net.ICredentials Credentials
		{
			set { throw new NotSupportedException(); }
		}

		public override object GetEntity(Uri absoluteUri, string role, Type t)
		{
			return sytemResolver.GetEntity(absoluteUri, role, t);
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			Uri retval = null;

			//String filenameDtd = AppDomain.CurrentDomain.BaseDirectory;
			//filenameDtd += System.Configuration.ConfigurationManager.AppSettings.Get("DTD_SEGNATURA_PATH");

			//if (relativeUri.ToLower().Contains("segnatura.dtd"))
			//{
			//	retval = new Uri(filenameDtd);
			//}
			//else
			{
				retval = sytemResolver.ResolveUri(baseUri, relativeUri);
			}

			return retval;
		}
	}
}
