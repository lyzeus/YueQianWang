using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace YueQian.ShortUrl.Core
{


    public class EmailManager
    {
        private const string EMAIL_ADDRESS = "xx0611qinchengye@163.com";
        private const string EMAIL_PASSWORD = "!!20060303123";

        private MailMessage _Email;

        public EmailManager(string to)
        {
            _Email = new MailMessage();
            _Email.To.Add(new MailAddress(to));
            _Email.From = new MailAddress(EMAIL_ADDRESS, "跃迁网");
            _Email.Subject = "未命名标题";//邮件标题   
            _Email.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题编码   
            _Email.Body = "<a target=\"_blank\" href=\"yqurl.com\">正文</a>";
            _Email.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容编码   
            _Email.IsBodyHtml = true;//是否是HTML邮件   
            _Email.Priority = MailPriority.High;//邮件优先级
        }

        public EmailManager(string to, string title, string content)
            : this(to)
        {
            _Email.Subject = title;
            _Email.Body = content;
        }

        public void SetContent(string content = "")
        {
            _Email.Body = content;
        }

        public bool SendEmail()
        {
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(EMAIL_ADDRESS, EMAIL_PASSWORD);
            //上述写你的GMail邮箱和密码   
            client.Port = 25;//Gmail使用的端口   
            client.Host = "smtp.163.com";
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.EnableSsl = true;//经过ssl加密   
            object userState = _Email;
            try
            {
                client.SendAsync(_Email, userState);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }
            return true;
        }
    }
}
