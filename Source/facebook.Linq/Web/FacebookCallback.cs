using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace facebook.Web
{
	/// <summary>
	/// Facebook callback URL should be pointed to here
	/// </summary>
	public class FacebookCallback : IHttpHandler, IRequiresSessionState
	{
		public void ProcessRequest(HttpContext context)
		{
			var fc = FacebookContext.Get(context);
			var session = fc.Session;
			var authenticated = session.IsAuthenticated;
			if (!authenticated)
				authenticated = session.TryAuthenticating(context.Request);
			if (authenticated)
			{
				var page = Facebook.DefaultPage ?? "./?";
				if (page.Contains("?"))
					page += "&";
				else
					page += "?";
				page += HttpUtility.UrlDecode(context.Request.QueryString.ToString());
				context.Response.Redirect(page);
			}
			else
			{
				if (context.Request["app"].IsNotNullOrEmpty())
				{
					var appName = context.Request["app"];
					if (appName.Contains("?"))
						appName = appName.Substring(0, appName.IndexOf('?'));
					var requestUrl = context.Request.Url;
					var redirectedPageQueryString = HttpUtility.UrlEncode("?" + HttpUtility.UrlDecode(context.Request.QueryString.ToString()).Replace('?', '&'));
					var url = String.Format("http://apps.facebook.com/{0}/?redirectTo=http://{1}{2}{3}", appName, requestUrl.Host, requestUrl.AbsolutePath, redirectedPageQueryString);
					fc.RedirectTopFrame(url);
				}
				else if (Facebook.HasValidConfiguration)
				{
					fc.RedirectTopFrame(Facebook.FacebookLoginUrl);
				}
				else
				{
					context.Response.Write("Facebook configuration is missing. (facebook.Linq.APIKey, facebook.Linq.Secret, facebook.Linq.ApplicationID)");
				}
			}
		}



		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
