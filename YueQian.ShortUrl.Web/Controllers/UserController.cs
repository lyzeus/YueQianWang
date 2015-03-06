using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YueQian.ShortUrl.Web.Helpers;
using YueQian.ShortUrl.Web.Models;
using YueQian.ShortUrl.Extensions;
using System.Web.Security;
using WebMatrix.WebData;
using Microsoft.Web.WebPages.OAuth;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.ViewModels;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : BaseController
    {
        public ActionResult Index(string s = "")
        {
            ViewBag.Message = User.Identity.Name + "已签到";
            if (!string.IsNullOrEmpty(s))
            {
                var su = ShortUrlFinder.Find(s);
                su.UserId = CurrentUserId;
                su.UserName = User.Identity.Name;
                MongoHelper.Instance.Save(su);
                return RedirectPermanent("/User");
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        #region  /*(-_-)*/登录注册/*(-_-)*/

        [AllowAnonymous]
        public ActionResult Register(string returnUrl, string invite = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            if (Request.IsAuthenticated && Roles.IsUserInRole(User.Identity.Name, "User"))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return RedirectPermanent(returnUrl);
                return RedirectToAction("Index", "User");
            }
            if (!string.IsNullOrEmpty(invite))
            {
                var icm = new InviteCodeManager();
                if (!icm.Valid(invite))
                    return Content("您的邀请码无效，请先去获取邀请码吧！");
                ViewBag.InviteCode = invite;
                return View();
            }
            return Content("未开放未测！");

        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Register(string returnUrl, UserRegisterModel usr, string InviteCode, string VerifyCode = "")
        {
            if (ModelState.IsValid)
            {
                var icm = new InviteCodeManager();
                if (!icm.Valid(InviteCode))
                    return Json(new { result = false, msg = "您的邀请码已被使用或无效,请重新获取邀请码." });
                if (string.IsNullOrEmpty(VerifyCode) || !string.Equals(CurrentVerifyCode, VerifyCode, StringComparison.InvariantCultureIgnoreCase))
                    return Json(new { result = false, msg = "您输入的验证码错误,请重新输入." });
                if (UserHelper.Exists(usr.UserName))
                    return Json(new { result = false, msg = "您选择用户名已被注册,请重新输入一个." });
                if (!usr.Password.Equals(usr.ConfirmPassword, StringComparison.InvariantCultureIgnoreCase))
                    return Json(new { result = false, msg = "您两次输入的密码不一致,请重试." });
                if (string.IsNullOrEmpty(usr.Email) || !usr.Email.IsEmail())
                    return Json(new { result = false, msg = "您输入的邮箱格式不正确,请重新输入." });
                if (UserHelper.Register(usr))
                {
                    //使用邀请码
                    icm.UseInviteCode(InviteCode, Membership.GetUser(usr.UserName).ProviderUserKey.ToString(), usr.UserName);

                    #region /*(-_-)*/发送激活邮件/*(-_-)*/
                    //生成激活邮件
                    var cm = new ConfirmationManager(usr.UserName);
                    var code = cm.Generator();
                    //发送邮件
                    var confirmUrl = string.Format("http://yqurl.com/User/Confirmation/{0}", code);
                    var email = new EmailManager(usr.Email, "跃迁网用户验证邮件", string.Format("请点击链接以下链接，若不能点击请复制到浏览器地址栏。<a href=\"0\" target=\"_blank\">{0}</a>", confirmUrl));
                    email.SendEmail();
                    #endregion

                    return Json(new { result = true, msg = "注册成功!将转到用户中心.", url = string.IsNullOrEmpty(returnUrl) ? "/User" : returnUrl });
                }

            }
            return Json(new { result = false, msg = "注册失败,请重试!" });
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (Request.IsAuthenticated && Roles.IsUserInRole(User.Identity.Name, "User"))
                return RedirectToAction("Index", "User");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
                return Json(new { result = true, msg = "登录成功!", url = string.IsNullOrEmpty(returnUrl) ? "/User" : returnUrl });
            return Json(new { result = false, msg = "登录失败,请检查您的用户名密码!", url = returnUrl });
        }

        #endregion

        #region  /*(-_-)*/找回密码/*(-_-)*/
        [AllowAnonymous]
        public ActionResult Forgot()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Forgot(string UserName = "", string Email = "")
        {

            return Json(new { });
        }

        #endregion

        #region  /*(-_-)*/更改密码/*(-_-)*/
        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // 在某些出错情况下，ChangePassword 将引发异常，而不是返回 false。
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                        return Json(new { result = true, msg = "修改密码成功", url = "/User/Manage" });
                    return Json(new { result = false, msg = "当前密码不正确或新密码无效", url = "/User/Manage" });
                }
            }
            else
            {
                // 用户没有本地密码，因此将删除由于缺少
                // OldPassword 字段而导致的所有验证错误
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return Json(new { result = true, msg = "设置密码成功!", url = "/User/Manage" });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("无法创建本地帐户。可能已存在名为“{0}”的帐户。", User.Identity.Name));
                    }
                }
            }

            return Json(new { result = false, msg = "当前密码不正确或新密码无效", url = "/User/Manage" });
        }

        #endregion

        #region  /*(-_-)*/链接/*(-_-)*/
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Update(long id)
        {
            var model = MongoHelper.Instance.FindOne<WebUrl>(id);
            return View("Create", model);
        }

        [HttpPost]
        public JsonResult Edit(WebUrl model)
        {
            var isNew = model.Id <= 0;

            if (isNew)
                model = new WebUrl() { ShortUrl = ShortUrl.Core.ShortUrlGenerator.Generator(6) };
            UpdateModel(model);
            if (!model.Url.IsHttpUrl())
                return Json(new { result = false, msg = "您填写网址格式错误,请重新输入!" });
            if (string.IsNullOrEmpty(model.Title))
                model.Title = "无标题";
            model.CreationDate = DateTime.Now;
            model.UserId = CurrentUserId;
            model.UserName = User.Identity.Name;
            model.BrowerInfo = BrowserInfo.GetFullUserAgent();
            model.Ip = IPAddress.IP;
            if (isNew && MongoHelper.Instance.Has(UserIdentityQueryBuilder(Query.EQ("Url", model.Url)), "WebUrl"))
                return Json(new { result = false, msg = "该网址您已经添加过了,请重新填写一个吧!" });
            if (MongoHelper.Instance.Save(model))
            {
                var msg = "更新短址信息成功!";
                if (isNew) msg = "新增短址信息成功!";
                return Json(new { result = true, url = string.Format("/User/Succeed/{0}?msg={1}", model.ShortUrl, msg) });
            }
            return Json(new { result = false, msg = "提交短址信息错误,请重试!" });
        }

        public ActionResult Succeed(string id, string msg)
        {

            return View(new WebUrl { Title = msg, ShortUrl = id });
        }

        public ActionResult List(int? categoryId, string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            return View(new WebUrlListViewModel(categoryId, keyword, startDate, endDate, page, pageSize));
        }

        [HttpPost]
        public JsonResult DeleteUrls(long id, string returnUrl)
        {
            if (MongoHelper.Instance.SafeDelete<WebUrl>(id))
                return Json(new { result = true, msg = "删除短址成功!", url = returnUrl });
            return Json(new { result = false, msg = "删除短址失败,请重试!" });
        }
        #endregion

        #region  /*(-_-)*/消息通知/*(-_-)*/
        public ActionResult Notification(string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            return View(new UserNotificationViewModel(startDate, endDate, page, pageSize));
        }

        public ActionResult SendNotification()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendNotification(UserNotification model, string UserNames)
        {

            if (!string.IsNullOrEmpty(UserNames))
            {
                var usrNames = UserNames.Split(',');

                var nm = new NotificationManager(usrNames, UserMessageType.用户消息, model.Title, model.Contents);
                nm.Send();
                return Json(new { result = true, msg = "发送站内消息成功" });
            }



            return Json(new { result = false, msg = "请填写正确的接受人用户名" });
        }


        [HttpPost]
        public JsonResult ReadNotification(long id)
        {
            return Json(new { result = NotificationFinder.Read(id) });
        }
        #endregion

        #region /*(-_-)*/积分|统计/*(-_-)*/

        public ActionResult MyIntegral()
        {
            return View(new MyIntegralViewModel(CurrentUserId));
        }

        public ActionResult CountList(int? categoryId, string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            return View(new CountViewModel(keyword, startDate, endDate, page, pageSize));
        }

        #endregion

        #region  /*(-_-)*/提现/*(-_-)*/

        public ActionResult Withdrawals(long id = 0)
        {
            var model = MongoHelper.Instance.FindOne<Withdrawals>(id);
            if (model == null)
                model = new Withdrawals { Id = 0, UserId = CurrentUserId, UserName = User.Identity.Name };
            return View(model);
        }

        [HttpPost]
        public JsonResult Withdrawals(Withdrawals model)
        {
            var isNew = model.Id <= 0;

            UpdateModel(model);
            model.UserId = CurrentUserId;
            model.UserName = User.Identity.Name;
            var wd = new WithDrawalsGenerator(model);
            if (isNew)
            {
                var result = wd.Generator();
                return Json(new { result = result.Item1, msg = result.Item2, url = "/User/WithdrawalsList" });
            }
            else
            {
                var result = wd.ReGenerator();
                return Json(new { result = result.Item1, msg = result.Item2, url = "/User/WithdrawalsList" });
            }
        }


        public ActionResult WithdrawalsList(int? state, string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            return View(new WithdrawalsListViewModels(state, keyword, startDate, endDate, page, pageSize));
        }
        #endregion

        #region  /*(-_-)*/个人信息/*(-_-)*/
        public ActionResult MyVerification()
        {
            var model = UserHelper.GetVerification(CurrentUserId);
            if (model == null)
                model = new Verification { Id = 0 };
            return View(model);
        }

        [HttpPost]
        public JsonResult Verification(long id)
        {
            var isNew = id <= 0;
            var model = new Verification();
            UpdateModel(model);
            model.UserId = CurrentUserId;
            model.CreationDate = DateTime.Now;
            if (!isNew)
            {
                var old = UserHelper.GetVerification(CurrentUserId);
                old.IsDelete = true;
                MongoHelper.Instance.Save(old);
            }
            if (MongoHelper.Instance.Save(model))
                return Json(new { result = true, msg = "提交认证信息成功!", url = "/User/MyVerification" });
            return Json(new { result = false, msg = "提交认证信息失败!", url = "/User/MyVerification" });

        }

        public ActionResult Invite()
        {
            var ICM = new InviteCodeManager();
            ViewBag.Invite = ICM.GenerateCode();
            return View();
        }

        /// <summary>
        /// 邮件确认
        /// </summary>
        /// <returns></returns>
        public ActionResult Confirmation(string id)
        {
            var cm = new ConfirmationManager();

            var result = cm.Comfirm(id);
            ViewBag.IsConfirm = result;

            if (result) FormsAuthentication.SetAuthCookie(cm.CurrentUser.UserName, true);
            return View();
        }
        #endregion

        #region  /*(-_-)*/签到/*(-_-)*/
        public JsonResult Sign()
        {
            var behavior = new UserBehavior().Sign();
            return Json(new { result = behavior.Item1, msg = behavior.Item2 });
        }
        #endregion

    }
}
