using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using facebook.Web;

namespace facebook.Linq.Demo
{
	public partial class _Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (FacebookContext.Current.TryAuthenticating(true))
			{
				var db = new FacebookDataContext();

				var useQuerySyntax = true;
				var useMethodSyntax = false;

				if (useQuerySyntax)
				{
					var friendIds = from friend in db.friend_info where friend.uid1 == db.uid select friend.uid2;
					var friendDetails = (from user in db.user where friendIds.Contains(user.uid) select new { Name = user.name, Picture = user.pic_small }).Take(5);
					listFriends.DataSource = friendDetails.ToArray();
					DataBind();
				}
				else if (useMethodSyntax)
				{
					var friendIds2 = db.friend_info.Where(t => t.uid1 == db.uid).Select(t => t.uid2);
					var friendDetails2 = db.user.Where(t => friendIds2.Contains(t.uid)).Select(t => new { Name = t.name, Picture = t.pic_small }).Take(5);
					listFriends.DataSource = friendDetails2.ToArray();
					DataBind();
				}
			}
		}
	}
}
