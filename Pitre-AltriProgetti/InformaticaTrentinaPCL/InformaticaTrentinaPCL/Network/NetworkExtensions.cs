using System;
using System.Net;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Network
{
	public static class NetworkExtensions
	{
		public static bool CodeOk(this BaseResponseModel res)
		{
			return res.Code != 0 ? false : true;
		}

		public static bool StatusOk(this BaseResponseModel res)
		{
            return res.StatusCode == HttpStatusCode.OK;
		}
	}

}
