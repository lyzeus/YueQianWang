using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.Models.Enums;

namespace YueQian.ShortUrl.Core
{
    public class WithDrawalsGenerator
    {

        /// <summary>
        /// 检查用户个人信息是否已认证
        /// </summary>
        /// <returns></returns>
        public bool IsVerificated()
        {
            IMongoQuery condition = Query.EQ("UserId", _WithDrawalsIntegral.UserId);
            condition = Query.And(condition, Query.EQ("IsDelete", false));

            var model = MongoHelper.Instance.FindOne<Verification>(condition);
            return model != null;
        }

        /// <summary>
        /// 判断是否符合提现条件
        /// </summary>
        /// <returns></returns>
        private Tuple<bool, string> CheckConditon()
        {
            if (!IsVerificated())
                return new Tuple<bool, string>(false, "您还没有认证您的个人信息");

            if (_WithDrawalsIntegral.Amount <= 0)
                return new Tuple<bool, string>(false, "提现积分不符合要求");

            if (_WithDrawalsIntegral.Amount > AvaliableIntegral)
                return new Tuple<bool, string>(false, "提现积分大于可用积分");

            return new Tuple<bool, string>(true, "符合提现条件");
        }


        private bool AddFrozen(string contents = "")
        {
            var frozon = new IntegralFrozen
            {
                WithdrawalsId = _WithDrawalsIntegral.Id,
                UserId = _WithDrawalsIntegral.UserId,
                Integral = _WithDrawalsIntegral.Amount,
                CreationDate = DateTime.Now,
                Contents = contents
            };
            return MongoHelper.Instance.Save(frozon);
        }

        /// <summary>
        /// 申请提现
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> Generator()
        {
            var check = CheckConditon();
            if (check.Item1)
            {
                try
                {
                    //添加提现申请
                    _WithDrawalsIntegral.CreationDate = DateTime.Now;
                    _WithDrawalsIntegral.State = WithdrawalsType.提交申请;
                    var r = MongoHelper.Instance.Save(_WithDrawalsIntegral);
                    if (r)
                    {
                        AddFrozen(string.Format("{0},申请提现.", DateTime.Now.ToString()));
                        LogManager.AddLog(new WithdrawalsLog()
                        {
                            UserId = _WithDrawalsIntegral.UserId,
                            WithdrawalsId = _WithDrawalsIntegral.Id,
                            CreationDate = DateTime.Now,
                            Contents = "提交申请"
                        });
                        return new Tuple<bool, string>(true, "申请提现成功");
                    }
                    return new Tuple<bool, string>(false, "申请提现失败");
                }
                catch (Exception e)
                {
                    return new Tuple<bool, string>(false, e.Message);
                }
            }
            return check;
        }

        /// <summary>
        /// 重新发起提现
        /// </summary>
        /// <returns></returns>
        public Tuple<bool, string> ReGenerator()
        {
            var check = CheckConditon();
            if (check.Item1)
            {
                try
                {
                    //更新提现申请
                    _WithDrawalsIntegral.CreationDate = DateTime.Now;
                    _WithDrawalsIntegral.State = WithdrawalsType.提交申请;
                    var r = MongoHelper.Instance.Save(_WithDrawalsIntegral);
                    if (r)
                    {
                        AddFrozen(string.Format("{0},重新提交申请.", DateTime.Now.ToString()));
                        LogManager.AddLog(new WithdrawalsLog()
                        {
                            UserId = _WithDrawalsIntegral.UserId,
                            WithdrawalsId = _WithDrawalsIntegral.Id,
                            CreationDate = DateTime.Now,
                            Contents = "重新提交申请"
                        });
                        return new Tuple<bool, string>(true, "重新提交提现成功");
                    }
                    return new Tuple<bool, string>(false, "重新提交提现失败");
                }
                catch (Exception e)
                {
                    return new Tuple<bool, string>(false, e.Message);
                }
            }
            return check;
        }

        /// <summary>
        /// 提现信息
        /// </summary>
        private Withdrawals _WithDrawalsIntegral;

        /// <summary>
        /// 可用积分
        /// </summary>
        public int AvaliableIntegral { get; private set; }
        public WithDrawalsGenerator(Withdrawals wd)
        {
            _WithDrawalsIntegral = wd;
            AvaliableIntegral = new IntegralCalculator(wd.UserId).Integral;
        }

        public List<WithdrawalsLog> GetLogs()
        {
            return LogManager.GetLogs<WithdrawalsLog>("WithdrawalsId", _WithDrawalsIntegral.Id).ToList();
        }

    }
}
