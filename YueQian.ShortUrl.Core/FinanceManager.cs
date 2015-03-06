using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Models;

namespace YueQian.ShortUrl.Core
{
    public class FinanceManager<T> where T : FinanceEntity
    {
        private T _Entity;
        public FinanceManager(T entity)
        {
            _Entity = entity;
        }

        public bool Add()
        {
            return MongoHelper.Instance.Save(_Entity);
        }
    }
}
