using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YueQian.ShortUrl.Web.Models
{
    /// <summary>
    /// 个人用户注册Model
    /// </summary>
    public class UserRegisterModel : RegisterModel
    {
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "格式错误")]
        public string Email { get; set; }

    }

}