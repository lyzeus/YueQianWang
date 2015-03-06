using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YueQian.ShortUrl.Models
{
    public class ComfirmationCode : UserEntity
    {
        public string UserName { get; set; }
        public string Code { get; set; }
        public bool IsConfirm { get; set; }
        public DateTime ComfirmDate { get; set; }
    }
}
