using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.Web.Helpers;

namespace YueQian.ShortUrl.Web.Controllers
{
    public class ValidateController : Controller
    {
        /// <summary>
        /// 检查用户名时候存在
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public JsonResult CheckUserName(string username)
        {
            return Json(!UserHelper.Exists(username));
        }

        /// <summary>
        /// 获取邀请码列表
        /// </summary>
        /// <param name="used"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public JsonResult GetUserInviteCodeLIst(bool? used, string userName = "")
        {
            var icm = new InviteCodeManager(userName);
            var source = icm.GetInviteCodeList(used);
            return Json(source.Select(s => new
            {
                s.Code,
                s.CreationDate,
                s.UsedDate,
                UserName = s.InvitedUserName
            }));
        }

        /// <summary>
        /// 新增一个邀请码
        /// </summary>
        /// <returns></returns>
        public JsonResult Invite()
        {
            var icm = new InviteCodeManager();
            return Json(new { code = icm.GenerateCode(alwaysNew: true) });
        }

        /// <summary>
        /// 检测当前用户是否已认证
        /// </summary>
        /// <returns></returns>
        public ContentResult CheckVerification()
        {
            var user = System.Web.Security.Membership.GetUser(User.Identity.Name);
            if (user != null)
            {
                var ver = UserHelper.GetVerification(user.ProviderUserKey.ToString());
                if (ver != null)
                    return Content("true");
            }
            return Content("false");
        }

    }
}
