using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{
    public class WithDrawalsManager : RequestUser
    {
        public Tuple<bool, string> Pass()
        {
            var originalState = Withdrawals.State;
            Withdrawals.State = WithdrawalsType.申请通过;

            var wR = MongoHelper.Instance.Save(Withdrawals);
            if (wR)
            {
                ////增加积分使用记录
                //var mR = MongoHelper.Instance.Save(new IntegralUsed
                //{
                //    CreationDate = DateTime.Now,
                //    Integral = Withdrawals.Amount,
                //    IntegralUsedType = IntegralUsedType.提现,
                //    UserId = CurrentUser.ProviderUserKey.ToString()
                //});

                //string msg = mR ? string.Format("扣除积分{0} ", Withdrawals.Amount) : "并未扣除积分";

                //增加同意申请Log
                LogManager.AddLog(new WithdrawalsLog
                {
                    WithdrawalsId = Withdrawals.Id,
                    UserId = CurrentUser.ProviderUserKey.ToString(),
                    CreationDate = DateTime.Now,
                    Contents = string.Format("{0} 将状态更新为 {1}", CurrentUser.UserName, Withdrawals.State.ToString())
                });


                #region /*(-_-)*/发送通知给用户/*(-_-)*/
                var notification = new NotificationManager(Withdrawals.UserId, "提现消息", "您的提现已经审核通过了。");
                notification.Send();
                #endregion
            }
            return new Tuple<bool, string>(true, "提现申请通过");
        }

        /// <summary>
        /// 拒绝提现
        /// </summary>
        /// <param name="msg">拒绝原因</param>
        /// <returns></returns>
        public Tuple<bool, string> Reject(string reason = "")
        {
            if (string.IsNullOrEmpty(reason)) reason = "未填写";
            Withdrawals.State = WithdrawalsType.申请被拒;

            var r = MongoHelper.Instance.Save(Withdrawals);
            if (r)
            {
                #region  /*(-_-)*/添加拒绝申请的Log/*(-_-)*/

                LogManager.AddLog(new WithdrawalsLog
                {
                    WithdrawalsId = Withdrawals.Id,
                    UserId = CurrentUser.ProviderUserKey.ToString(),
                    CreationDate = DateTime.Now,
                    Contents = string.Format("{0} 拒绝了提现原因是：{1}", "管理员", reason)
                });

                #endregion

                #region  /*(-_-)*/解冻相关的积分/*(-_-)*/
                ThawIntegral("拒绝提现");
                #endregion

                #region /*(-_-)*/发送通知给用户/*(-_-)*/
                var notification = new NotificationManager(Withdrawals.UserId, "提现消息", string.Format("{0} 拒绝了提现原因是：{1}", "管理员", reason));
                notification.Send();
                #endregion

            }
            return new Tuple<bool, string>(true, "提现被拒绝");
        }

        /// <summary>
        /// 打款
        /// </summary>
        /// <param name="contents">打款相关信息(交易号等信息)</param>
        /// <returns></returns>
        public Tuple<bool, string> Pay(Payment payment)
        {
            Withdrawals.State = WithdrawalsType.已打款;

            var r = MongoHelper.Instance.Save(Withdrawals);
            if (r)
            {
                #region /*(-_-)*/记录金额/*(-_-)*/
                var finance = new FinanceManager<Payment>(payment);
                finance.Add();
                #endregion

                #region  /*(-_-)*/解冻相关的积分/*(-_-)*/
                ThawIntegral("拒绝提现");
                #endregion

                #region /*(-_-)*/发送通知给用户/*(-_-)*/
                var notification = new NotificationManager(Withdrawals.UserId, "提现消息",
                    string.Format("打款日期:{0}<br/>交易号:{1}<br/>备注:{2}", payment.CreationDate.ToString(), payment.TradeCode, payment.Contents));
                notification.Send();
                #endregion
            }
            return new Tuple<bool, string>(true, "打款操作成功");
        }

        public bool ThawIntegral(string reason = "")
        {
            var condition = Query.EQ("WithdrawalsId", Withdrawals.Id);
            condition = Query.And(condition, Query.EQ("IsDelete", false));
            var frozen = MongoHelper.Instance.FindOne<IntegralFrozen>(condition);
            if (frozen != null)
            {
                frozen.IsDelete = true;
                frozen.ThawDate = DateTime.Now;
                frozen.ThawReason = reason;
                return MongoHelper.Instance.Save(frozen);
            }
            return false;

        }

        public Withdrawals Withdrawals { get; private set; }

        public WithDrawalsManager(long withdrawalsId)
        {
            Withdrawals = MongoHelper.Instance.FindOne<Withdrawals>(withdrawalsId);
            if (Withdrawals == null) throw new NotImplementedException();
        }

    }
}
