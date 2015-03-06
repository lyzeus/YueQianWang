using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YueQian.ShortUrl.Core;
using YueQian.ShortUrl.Models;
using YueQian.ShortUrl.ViewModels;

namespace YueQian.ShortUrl.Admin.ViewModels
{
    public class AdminInviteModel : ViewModelBase
    {
        public IEnumerable<InviteCode> InUseInviteCodes { get; set; }
        public IEnumerable<InviteCode> UsedInviteCodes { get; set; }


        public AdminInviteModel()
        {
            PageTitle = "用户管理";
            SubTitle = "邀请注册";

            var icm = new InviteCodeManager();
            var source = icm.GetInviteCodeList(null);

            InUseInviteCodes = source.Where(s => !s.IsUsed);
            UsedInviteCodes = source.Where(s => s.IsUsed);
        }

    }
}
