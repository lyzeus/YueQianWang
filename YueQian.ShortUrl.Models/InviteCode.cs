using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class InviteCode : LongIdEntity
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public string InvitedUserId { get; set; }
        public string InvitedUserName { get; set; }
        public DateTime UsedDate { get; set; }
        public bool IsUsed { get; set; }
    }
}
