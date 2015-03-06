using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YueQian.ShortUrl.Admin.ViewModels;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;
using YueQian.ShortUrl.Web.Models;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using ExtendedMongoMembership;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemController : BaseController
    {
        //
        // GET: /System/

        public ActionResult Index()
        {
            return View(new IndexModel());
        }

        #region  /*(-_-)*/用户管理/*(-_-)*/

        public ActionResult UserManager(string id = "")
        {
            switch (id.ToLower())
            {
                default:
                    return View("UserList", new IndexModel() { PageTitle = "用户管理", SubTitle = "用户列表" });
                case "level":
                    return View("UserLevelManager", new IndexModel() { PageTitle = "用户管理", SubTitle = "用户等级" });
                case "invite":
                    return View("InviteUser", new AdminInviteModel());
            }
        }

        [HttpPost]
        public JsonResult UserList(JQueryDataTableParamModel param, DateTimeRange periodtime, string keyword = "")
        {
            var provider = new MongoSession();
            var role = provider.Roles.ToList().FirstOrDefault(s => string.Equals(s.RoleName, "User", StringComparison.InvariantCultureIgnoreCase));
            var source = provider.Users.Where(s => s.Roles.Contains(role));

            //var all = Roles.GetUsersInRole("User");
            //var source = new List<MembershipUser>();
            //var provider = Membership.Provider as WebMatrix.WebData.ExtendedMembershipProvider;


            //foreach (var item in all)
            //{
            //    var user = provider.GetUser(item);
            //    source.Add(user);
            //}
            Func<MembershipAccount, bool> condition = n => Roles.IsUserInRole(n.UserName, "User") && (n.UserName.Contains(keyword) && (periodtime.GetDateTimeCondition(n)));

            var filteredSource = (from q in source.Where(condition)
                                  orderby q.CreationDate descending
                                  select q);

            var result = filteredSource.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = source.Count(),
                iTotalDisplayRecords = result.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.UserId),
                    Title = string.Format("<a title=\"编辑\" href=\"javascript:op.edit({0})\">{1}</a>", s.UserId, s.UserName),
                    Level = new UserLevelManager(s.UserId.ToString()).GetLevelName(),
                    UserId = s.UserId,
                    CreationDate = s.CreationDate.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    LastLoginDate = s.LastLoginDate.HasValue ? s.LastLoginDate.Value.ToString("yyyy/MM/dd HH:mm:ss") : "",
                    Operate = string.Format("<a href=\"/System/UrlList?u={0}\">短址</a> <a href=\"/System/WithdrawalsList?u={0}\">提现</a> <a href=\"/System/UserIntegral?u={0}\">积分</a>", s.UserName)

                })
            }, JsonRequestBehavior.AllowGet);
        }

        #region /*(-_-)*/用户等级/*(-_-)*/
        [HttpPost]
        public JsonResult MemberShipLevel(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<MemberShipLevel>(eId);
                if (m != null)
                {
                    return Json(new { result = true, model = m });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                MemberShipLevel model;
                if (id <= 0) model = new ShortUrl.Models.MemberShipLevel();
                else model = MongoHelper.Instance.FindOne<ShortUrl.Models.MemberShipLevel>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增用户等级成功!" });
                    else return Json(new { result = true, msg = "更新用户等级成功!" });
                }
                return Json(new { result = false, msg = "提交用户等级失败,请重试!" });
            }
        }

        [HttpPost]
        public JsonResult MemberShipLevelList(JQueryDataTableParamModel param)
        {
            var all = MongoHelper.Instance.Find<MemberShipLevel>(Query.Null);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = all.OrderByDescending(s => s.CreationDate).Select(s => new
                {
                    Name = s.Name,
                    Integral = string.Format("{0}~{1}", s.MinIntegral, s.MaxIntegral),
                    s.Coefficient,
                    s.Contents,
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = string.Format("<a href=\"javascript:op.edit({0})\"><i class=\"icon-pencil\"></i>编辑</a>", s.Id),
                })
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult UpdateUserLevel()
        {
            return Json(new { });
        }
        #endregion

        #region /*(-_-)*/积分/*(-_-)*/
        public ActionResult UserIntegral(string u)
        {
            if (WebSecurity.UserExists(u))
            {
                var userId = WebSecurity.GetUserId(u).ToString();

                var calc = new IntegralCalculator(userId);

                return View(new AdminCommonModel()
                {
                    PageTitle = "用户积分",
                    SubTitle = "管理 & 更新",
                    UserName = u,
                    SearchKeyWord = string.Format("<b>{0}</b> 可用积分 {1},历史总积分 {2},当前冻结积分 {3}", u, calc.Integral, calc.TotalIntegral, calc.FrozenIntegral)
                });
            }
            return Content(string.Format("不存在用户\"{0}\"", u));
        }


        [HttpPost]
        public JsonResult UserIntegral(string u, JQueryDataTableParamModel param, DateTimeRange periodtime)
        {
            var condition = Query.Null;
            var userId = WebSecurity.GetUserId(u).ToString();
            condition = periodtime.GetDateTimeQuery();
            condition = Query.And(condition, Query.EQ("UserId", userId));
            var all = MongoHelper.Instance.Find<Integral>(condition).OrderByDescending(s => s.CreationDate);

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    Title = string.Format("{0}{1}", s.IntegralType.ToString(), s.IntegralType == IntegralType.有效访问 ? "(yqurl.com/" + s.ShortUrl + ")" : ""),
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss")
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddIntegral(string UserName, string Contents = "")
        {
            try
            {
                var userId = WebSecurity.GetUserId(UserName);
                if (userId > 0)
                {
                    var integral = new Integral
                    {
                        UserId = userId.ToString(),
                        CalculatorSystem = Application.CalculatorSystem,
                        IntegralType = IntegralType.管理员加分,
                        CreationDate = DateTime.Now,
                        Contents = Contents
                    };
                    MongoHelper.Instance.Save(integral);

                    return Json(new { result = true, msg = "为用户加分成功!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { result = false, msg = "为用户加分失败!不存在该用户!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = false, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        [HttpPost]
        public JsonResult GetUserName(string query)
        {
            var provider = new MongoSession();
            var role = provider.Roles.ToList().FirstOrDefault(s => string.Equals(s.RoleName, "User", StringComparison.InvariantCultureIgnoreCase));
            var source = provider.Users.Where(s => s.Roles.Contains(role));

            Func<MembershipAccount, bool> condition = n => n.UserName.Contains(query);
            var filteredSource = (from q in source.Where(condition)
                                  orderby q.CreationDate descending
                                  select q.UserName);

            return Json(filteredSource.ToArray(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region /*(-_-)*/短址管理/*(-_-)*/

        public ActionResult UrlList(string u = "")
        {
            return View(new AdminCommonModel { PageTitle = "短址列表", SubTitle = "数据 & 统计", UserName = u });
        }

        [HttpPost]
        public JsonResult UrlList(JQueryDataTableParamModel param, DateTimeRange periodtime, string keyword = "", string username = "")
        {
            var query = Query.EQ("IsDelete", false);
            query = Query.And(query, periodtime.GetDateTimeQuery());
            if (!string.IsNullOrEmpty(keyword))
            {
                query = Query.And(query, Query.Matches("Title", keyword));
            }
            if (!string.IsNullOrEmpty(username))
            {
                var userid = WebMatrix.WebData.WebSecurity.GetUserId(username).ToString();

                query = Query.And(query, Query.EQ("UserId", userid));
            }

            var all = MongoHelper.Instance.Find<WebUrl>(query).SetSortOrder(SortBy.Descending("CreationDate"));

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    UserId = s.UserName,
                    Title = string.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", s.Url, s.Title),
                    FullUrl = string.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", s.FullUrl, s.FullUrl),
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Count = s.ViewCount,
                    Operate = string.Format("<a href=\"javascript:op.edit({0});\">修改</a> <a href=\"javascript:op.delete({0},'{1}');\">删除</a>", s.Id, s.FullUrl)
                })
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult DeleteUrls(long id)
        {
            if (MongoHelper.Instance.SafeDelete<WebUrl>(id))
                return Json(new { result = true, msg = "删除短址成功!" });
            return Json(new { result = false, msg = "删除短址失败,请重试!" });
        }

        [HttpPost]
        public JsonResult WebUrlInfo(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<WebUrl>(eId);
                if (m != null)
                {
                    return Json(new { result = true, model = m });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                WebUrl model;
                if (id <= 0) model = new ShortUrl.Models.WebUrl();
                else model = MongoHelper.Instance.FindOne<ShortUrl.Models.WebUrl>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增短址成功!" });
                    else return Json(new { result = true, msg = "更新短址成功!" });
                }
                return Json(new { result = false, msg = "提交短址失败,请重试!" });
            }
        }
        #endregion

        #region /*(-_-)*/设置/*(-_-)*/
        public ActionResult Setting(string id)
        {
            switch (id.ToLower())
            {
                default:
                case "":
                case "a":
                    return View(new SettingModel() { PageTitle = "全局变量", SubTitle = "设置" });
                case "c"://积分系统设置
                    return View("CalculatorSetting", new IndexModel() { PageTitle = "积分系统", SubTitle = "设置" });
                case "account"://财务帐号
                    return View("CompanyAccountSetting", new IndexModel() { PageTitle = "财务帐号", SubTitle = "设置" });
            }

        }

        [HttpPost]
        public JsonResult Setting(string id, string calculatorsystem = "")
        {
            var model = MongoHelper.Instance.FindLast<Setting>();
            UpdateModel(model);
            model.CreationDate = DateTime.Now;
            if (MongoHelper.Instance.Save(model))
                return Json(new { result = true, msg = "更新全局设置成功!" });
            return Json(new { result = false, msg = "更新全局设置成功失败,请重试!" });
        }

        #region /*(-_-)*/积分模式/*(-_-)*/
        [HttpPost]
        public JsonResult CalculatorSetting(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<CalculatorSystem>(eId);
                if (m != null)
                {
                    return Json(new { result = true, model = m });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                CalculatorSystem model;
                if (id <= 0) model = new CalculatorSystem();
                else model = MongoHelper.Instance.FindOne<CalculatorSystem>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增积分模式成功!" });
                    else return Json(new { result = true, msg = "更新积分模式成功!" });
                }
                return Json(new { result = false, msg = "提交积分模式失败,请重试!" });
            }
        }

        [HttpPost]
        public JsonResult CalculatorSettingList(JQueryDataTableParamModel param)
        {
            var all = MongoHelper.Instance.Find<CalculatorSystem>(Query.Null);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = all.OrderByDescending(s => s.CreationDate).Select(s => new
                {
                    Name = s.Name,
                    s.PerViewIntegral,
                    s.SignIntegral,
                    s.SpreadIntegral,
                    s.AdminAddIntegral,
                    s.Contents,
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = string.Format("<a href=\"javascript:op.edit({0})\"><i class=\"icon-pencil\"></i>编辑</a>", s.Id),
                })
            }, JsonRequestBehavior.AllowGet);

        }


        #endregion

        #region /*(-_-)*/财务帐号/*(-_-)*/

        [HttpPost]
        public JsonResult CompanyAccountList(JQueryDataTableParamModel param)
        {
            var all = MongoHelper.Instance.Find<CompanyAccount>(Query.Null);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = all.OrderByDescending(s => s.CreationDate).Select(s => new
                {
                    PaymentType = s.PaymentType.ToString(),
                    s.RealName,
                    s.AccountBank,
                    s.Account,
                    s.Contents,
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = string.Format("<a href=\"javascript:op.edit({0})\"><i class=\"icon-pencil\"></i>编辑</a>", s.Id),
                })
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult CompanyAccountSetting(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<CompanyAccount>(eId);
                if (m != null)
                {
                    return Json(new { result = true, model = m });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                CompanyAccount model;
                if (id <= 0) model = new CompanyAccount();
                else model = MongoHelper.Instance.FindOne<CompanyAccount>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增财务帐号成功!" });
                    else return Json(new { result = true, msg = "更新财务帐号成功!" });
                }
                return Json(new { result = false, msg = "提交财务帐号失败,请重试!" });
            }
        }
        #endregion


        #region  /*(-_-)*/更新密码/*(-_-)*/
        public ActionResult Manage()
        {
            return View(new IndexModel() { PageTitle = "账户管理", SubTitle = "设置密码" });
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
                        return Json(new { result = true, msg = "修改密码成功", url = "/System/Manage" });
                    return Json(new { result = false, msg = "当前密码不正确或新密码无效", url = "/System/Manage" });
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
                        return Json(new { result = true, msg = "设置密码成功!", url = "/System/Manage" });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("无法创建本地帐户。可能已存在名为“{0}”的帐户。", User.Identity.Name));
                    }
                }
            }

            return Json(new { result = false, msg = "当前密码不正确或新密码无效", url = "/System/Manage" });
        }

        #endregion

        #endregion

        #region /*(-_-)*/提现管理/*(-_-)*/
        public ActionResult WithdrawalsList(string u = "")
        {
            return View(new AdminCommonModel() { PageTitle = "提现列表", SubTitle = "数据 & 统计", UserName = u });
        }

        private string getWithdrawalsOperate(long id, WithdrawalsType state)
        {
            string operate1 = "<a class=\"btn btn-success\" href=\"javascript:op.pass({0});\">同意</a>"
                            + "<a class=\"btn btn-danger\" href=\"javascript:op.reject({0});\" id=\"rj{0}\">拒绝</a>";
            string operate2 = "<a class=\"btn btn-warning\" href=\"javascript:op.pay({0});\"  id=\"py{0}\">打款</a>";
            switch (state)
            {
                default:
                case WithdrawalsType.提交申请:
                    return string.Format(operate1, id);
                case WithdrawalsType.申请通过:
                    return string.Format(operate2, id);
                case WithdrawalsType.已打款:
                case WithdrawalsType.申请被拒:
                    return "";
            }
        }

        [HttpPost]
        public JsonResult WithdrawalsList(JQueryDataTableParamModel param, DateTimeRange periodtime, WithdrawalsType? state, string username = "")
        {
            var condition = Query.Null;

            condition = periodtime.GetDateTimeQuery();

            if (state.HasValue)
            {
                condition = Query.And(condition, Query.EQ("State", state));
            }

            if (!string.IsNullOrEmpty(username))
            {
                var userid = WebMatrix.WebData.WebSecurity.GetUserId(username).ToString();

                condition = Query.And(condition, Query.EQ("UserId", userid));
            }

            var all = MongoHelper.Instance.Find<Withdrawals>(condition).OrderByDescending(s => s.CreationDate);

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    UserId = s.UserName,
                    State = string.Format("{0}<br/><a id=\"lg{1}\" href=\"javascript:op.logs({1})\">查看日志</a>", s.State.ToString(), s.Id),
                    Amount = s.Amount,
                    Detail = s.Verification.ToString(),
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = getWithdrawalsOperate(s.Id, s.State)
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Logs(long id)
        {
            try
            {
                var logs = LogManager.GetLogs<WithdrawalsLog>("WithdrawalsId", id).OrderBy(s => s.CreationDate).Select(s => new
                {
                    s.Id,
                    s.Contents,
                    CreationDate = s.CreationDate.ToString()
                });
                return Json(new { result = true, msg = logs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = false, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Pass(long id)
        {
            try
            {
                var manager = new WithDrawalsManager(id);
                var result = manager.Pass();
                return Json(new { result = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = false, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Reject(long id, string reason = "")
        {
            try
            {
                var manager = new WithDrawalsManager(id);
                var result = manager.Reject(reason);
                return Json(new { result = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = false, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Pay(long id, Payment payment)
        {
            try
            {
                var manager = new WithDrawalsManager(id);
                payment.CreationDate = DateTime.Now;
                payment.MoneyCount = manager.Withdrawals.Amount;
                payment.UserId = CurrentUserId;
                payment.UserName = User.Identity.Name;
                payment.WithdrawalsId = id;
                var result = manager.Pay(payment);
                return Json(new { result = result.Item1, msg = result.Item2 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = false, msg = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region /*(-_-)*/广告管理/*(-_-)*/
        public ActionResult AdvManager(string id)
        {

            switch (id.ToLower())
            {
                default:
                case "":
                case "adv":
                    return View(new IndexModel() { PageTitle = "广告", SubTitle = "数据 & 管理" });
                case "position":
                    return View("AdvPosition", new IndexModel() { PageTitle = "广告位", SubTitle = "数据 & 管理" });
            }


        }

        [HttpPost]
        public JsonResult AdvList(JQueryDataTableParamModel param, DateTimeRange periodtime, string positionnumber = "")
        {
            var query = Query.Null;
            query = periodtime.GetDateTimeQuery();
            if (!string.IsNullOrEmpty(positionnumber))
            {
                query = Query.And(query, Query.EQ("PositionNumber", positionnumber));
            }

            var all = MongoHelper.Instance.Find<Advertisement>(query);

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    Available = s.IsAvailable ? "有效" : "无效",
                    s.AdvertiseNumber,
                    PositionNumber = string.Format("{0}({1})", s.Position.Name, s.Position.PositionNumber),
                    DataRange = string.Format("{0} 至 {1}", s.StartTime.HasValue ? s.StartTime.Value.ToString("yyyy/M/d HH:mm") : "当前", s.EndTime.HasValue ? s.EndTime.Value.ToString("yyyy/M/d HH:mm") : "一直"),
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = string.Format("<a href='javascript:op.edit({0});'>更新</a>", s.Id)
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AdvertisementInfo(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<Advertisement>(eId);
                if (m != null)
                {
                    return Json(new
                    {
                        result = true,
                        model = new
                        {
                            m.Id,
                            m.AdvType,
                            m.Url,
                            Width = m.Position.Width,
                            Height = m.Position.Height,
                            m.FilePath,
                            m.IsAvailable,
                            m.PositionNumber,
                            m.ActualPrice,
                            m.AdvertiseNumber,
                            m.Contents,
                            m.CreationDate,
                            EndTime = m.EndTime.HasValue ? m.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                            StartTime = m.StartTime.HasValue ? m.StartTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                        }
                    });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                Advertisement model;
                if (id <= 0) model = new Advertisement();
                else model = MongoHelper.Instance.FindOne<ShortUrl.Models.Advertisement>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增广告成功!" });
                    else return Json(new { result = true, msg = "更新广告成功!" });
                }
                return Json(new { result = false, msg = "提交广告失败,请重试!" });
            }
        }


        [HttpPost]
        public JsonResult AdvPositionList(JQueryDataTableParamModel param, DateTimeRange periodtime, string keyword = "")
        {
            var query = Query.Null;
            query = periodtime.GetDateTimeQuery();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = Query.And(query, Query.Matches("Name", keyword));
            }

            var all = MongoHelper.Instance.Find<AdvertisePosition>(query);

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    s.PositionNumber,
                    s.Name,
                    Detail = string.Format("宽:{0}px 高:{1}px", s.Width, s.Height),
                    s.Price,
                    s.Description,
                    CreationDate = s.CreationDate.ToString("yyyy/MM/dd HH:mm:ss"),
                    Operate = string.Format("<a href='javascript:op.edit({0});'>更新</a>", s.Id)
                })
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AdvertisementPositionInfo(long id = 0, long eId = 0)
        {
            if (eId > 0)
            {
                var m = MongoHelper.Instance.FindOne<AdvertisePosition>(eId);
                if (m != null)
                {
                    return Json(new
                    {
                        result = true,
                        model = m
                    });
                }
                return Json(new { result = false, model = "找不到相关记录!" });
            }
            else
            {
                AdvertisePosition model;
                if (id <= 0) model = new AdvertisePosition();
                else model = MongoHelper.Instance.FindOne<ShortUrl.Models.AdvertisePosition>(id);
                UpdateModel(model);
                model.CreationDate = DateTime.Now;
                if (string.IsNullOrEmpty(model.PositionNumber))
                    model.PositionNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                if (MongoHelper.Instance.Save(model))
                {
                    if (id <= 0) return Json(new { result = true, msg = "新增广告位成功!" });
                    else return Json(new { result = true, msg = "更新广告位成功!" });
                }
                return Json(new { result = false, msg = "提交广告位失败,请重试!" });
            }
        }


        #endregion

        #region /*(-_-)*/上传文件/*(-_-)*/
        static string[] AllowExs = new string[] { ".jpg", ".gif", ".png", ".swf" };
        [HttpPost]
        public JsonResult Upload(string id, string name = "")
        {
            if (Request.Files == null || Request.Files.Count == 0) return Json(new { filename = "", response = "请选择一个文件" }, "text/html");
            var file = Request.Files[0];
            if (file.ContentLength == 0) return Json(new { filename = "", response = "文件为空" }, "text/html");
            var ex = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (!AllowExs.Contains(ex)) return Json(new { filename = "", response = "文件格式不符合要求" }, "text/html");

            string path = Request.PhysicalApplicationPath + "Upload\\" + id + "\\";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string filename = !string.IsNullOrEmpty(name) ? name : (DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random(DateTime.Now.Millisecond).Next(9999));

            try
            {
                if (System.IO.File.Exists(path + filename + ex))
                    System.IO.File.Delete(path + filename + ex);
                file.SaveAs(path + filename + ex);
                return Json(new { filename = string.Format("/Upload/{0}/{1}{2}", id, filename, ex), response = "上传成功", format = ex.Replace(".", "").ToLower() }, "text/html");
            }
            catch
            {
                return Json(new { filename = "", response = "未知错误" });
            }
        }

        #endregion

        #region /*(-_-)*/消息管理/*(-_-)*/
        public ActionResult Notification(string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            return View(new IndexModel() { PageTitle = "通知", SubTitle = "数据 & 管理" });
        }

        [HttpPost]
        public JsonResult NotificationList(JQueryDataTableParamModel param, DateTimeRange periodtime, string keyword = "")
        {
            var condition = Query.Null;
            condition = periodtime.GetDateTimeQuery();
            if (!string.IsNullOrEmpty(keyword))
                condition = Query.And(condition, Query.Or(Query.Matches("Title", keyword), Query.Matches("Contents", keyword)));
            var all = MongoHelper.Instance.Find<UserNotification>(condition).SetSortOrder(SortBy.Descending("CreationDate"));

            var result = all.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = all.Count(),
                iTotalDisplayRecords = all.Count(),
                aaData = result.Select(s => new
                {
                    CheckBox = string.Format("<label><input name=\"sId\" value=\"{0}\" type=\"checkbox\" /><span class=\"lbl\"></span></label>", s.Id),
                    Receiver = s.UserName,
                    MessageType = s.MessageType.ToString(),
                    Contents = string.Format("{0}<br/>{1}", s.Title, s.Contents),
                    CreationDate = s.CreationDate.ToString(),
                    Status = s.IsRead ? string.Format("已读({0})", s.ReadDate.Value.ToString()) : "未读"
                })
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NotificationSend()
        {
            return View(new IndexModel() { PageTitle = "通知", SubTitle = "发送通知" });
        }

        [HttpPost]
        public JsonResult SendNotification(UserNotification model, string UserNames)
        {

            if (!string.IsNullOrEmpty(UserNames))
            {
                var usrNames = UserNames.Split(',');

                var nm = new NotificationManager(usrNames, UserMessageType.系统消息, model.Title, model.Contents);
                nm.Send();
                return Json(new { result = true, msg = "发送站内消息成功" });
            }



            return Json(new { result = false, msg = "请填写正确的接收人用户名" });
        }

        #endregion

    }


    public class DateTimeRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool GetDateTimeCondition(LongIdEntity n)
        {
            if ((EndTime == DateTime.MinValue) || (EndTime < StartTime))
                EndTime = DateTime.MaxValue;
            if (EndTime < DateTime.MaxValue)
                EndTime = EndTime.AddDays(1);
            return n.CreationDate >= StartTime && n.CreationDate < EndTime;
        }

        public bool GetDateTimeCondition(MembershipUser n)
        {
            if ((EndTime == DateTime.MinValue) || (EndTime < StartTime))
                EndTime = DateTime.MaxValue;
            if (EndTime < DateTime.MaxValue)
                EndTime = EndTime.AddDays(1);
            return n.CreationDate >= StartTime && n.CreationDate < EndTime;
        }

        public bool GetDateTimeCondition(MembershipAccount n)
        {
            if ((EndTime == DateTime.MinValue) || (EndTime < StartTime))
                EndTime = DateTime.MaxValue;
            if (EndTime < DateTime.MaxValue)
                EndTime = EndTime.AddDays(1);
            return n.CreationDate >= StartTime && n.CreationDate < EndTime;
        }

        public IMongoQuery GetDateTimeQuery()
        {
            if ((EndTime == DateTime.MinValue) || (EndTime < StartTime))
                EndTime = DateTime.MaxValue;
            if (EndTime < DateTime.MaxValue)
                EndTime = EndTime.AddDays(1);
            return Query.And(Query.GTE("CreationDate", StartTime), Query.LT("CreationDate", EndTime));
        }
    }
}
