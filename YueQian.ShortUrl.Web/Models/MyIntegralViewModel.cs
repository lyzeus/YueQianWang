using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Models
{
    public class MyIntegralViewModel : ViewModelBase
    {
        public int Integral { get; private set; }
        public int UsedIntegral { get; private set; }
        public int FrozenIntegral { get; private set; }
        public MyIntegralViewModel(string userid)
        {
            var caculator=new IntegralCalculator(userid);
            Integral = caculator.Integral;
            UsedIntegral= caculator.UsedIntegral;
            FrozenIntegral = caculator.FrozenIntegral;
        }
    }
}
