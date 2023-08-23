using Backend.Common.Utills;
using Backend.Contract.Entity.VO;
using Backend.Service.Interface;
using my_project_backend.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Service.Implementation
{
    public class EmailService : IEmailSerivce
    {
        public Task SendEmailAsync(Dictionary<string, string> data)
        {
            return Task.Run(() =>
            {
                string type = data.GetValueOrDefault("type") ?? throw new ArgumentException("didn't set email type!");
                string code = data.GetValueOrDefault("code") ?? throw new ArgumentException("didn't set email code!");
                string toEmail = data.GetValueOrDefault("email") ?? throw new ArgumentException("didn't set email address!");

                string content = type switch
                {
                    Const.EMAIL_TYPE_REGISTRATION => "您的邮件注册验证码为: " + code + "，有效时间3分钟，为了保障您的账户安全，请勿向他人泄露验证码信息。",
                    Const.EMAIL_TYPE_RESET => "你好，您正在执行重置密码操作，验证码: " + code + "，有效时间3分钟，如非本人操作，请无视。",
                    _ => string.Empty
                };
                string subject = type switch
                {
                    Const.EMAIL_TYPE_REGISTRATION => "欢迎注册我们的网站",
                    Const.EMAIL_TYPE_RESET => "您的密码重置邮件",
                    _ => string.Empty
                };

                if (string.IsNullOrEmpty(content))
                    return;

                var message = CreateMessage(subject, content, toEmail);

                var smtpclient = new SmtpClient("smtp.163.com", 25)
                {
                    Credentials = new NetworkCredential(
                        userName: ConfigurationStringManager.Instance.EmailAddress,
                        password: ConfigurationStringManager.Instance.EmailCredential),
                };
                smtpclient.SendMailAsync(message);
            });
        }

        private MailMessage CreateMessage(string subject, string content, string email)
        {
            var message = new MailMessage
            {
                From = new MailAddress(ConfigurationStringManager.Instance.EmailAddress),
                Subject = subject,
                Body = content,
            };
            message.To.Add(new MailAddress(email));

            return message;
        }
    }
}
