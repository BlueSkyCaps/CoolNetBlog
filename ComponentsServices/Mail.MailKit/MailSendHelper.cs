using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComponentsServices.Mail.MailKit
{
    /// <summary>
    /// 发送邮件类
    /// </summary>
    public class MailSendHelper
    {
        private MimeMessage emailData;
        private string _host;
        private int _port;
        private bool _useSsl;
        /// <summary>
        /// 初始化Mail发送类，后续必须调用InputEmailServerAddr()指定邮箱服务器地址。
        /// </summary>
        public MailSendHelper()
        {
            emailData =new MimeMessage();
        }

        /// <summary>
        /// 指定邮箱服务器地址初始化Mail发送类
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="useSsl"></param>
        public MailSendHelper(string host, int port, bool useSsl=false)
        {
            emailData = new MimeMessage();
            _host = host;
            _port = port;
            _useSsl = useSsl;
        }

        /// <summary>
        /// 指定邮箱服务器地址
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="useSsl"></param>
        public void InputSmtpServerHost(string host, int port, bool useSsl=false)
        {
            _host = host;
            _port = port;
            _useSsl = useSsl;
        }

        /// <summary>
        /// 指定发送者的邮件地址
        /// </summary>
        /// <param name="youName">此次发送的发送人姓名</param>
        /// <param name="youEmail">此次发送的发送人邮件地址</param>
        public void InputYourEmail(string youName, string youEmail)
        {
            emailData.From.Clear();
            emailData.From.Add(new MailboxAddress(youName, youEmail));
            
        }

        /// <summary>
        /// 指定要接收对象的邮件地址
        /// </summary>
        /// <param name="friendName">此次发送的接收人姓名</param>
        /// <param name="friendEmail">此次发送的接收人邮件地址</param>
        public void InputFriendEmail(string friendName, string friendEmail)
        {
            emailData.To.Add(new MailboxAddress(friendName, friendEmail));
        }

        /// <summary>
        /// 设置邮件内容
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="bodyContent">邮件内容</param>
        /// <param name="useHtmlText">是否内容文本是html格式的文本</param>
        public void InputContent(string subject, string bodyContent, bool useHtmlText=false)
        {
            emailData.Subject = subject;
            var textType = useHtmlText ? "html" : "plain";
            emailData.Body = new TextPart(textType)
            {
                Text = bodyContent
            };
        }


        /// <summary>
        /// 尝试发送邮件
        /// </summary>
        public void Send() {
            using (var client = new SmtpClient())
            {
                client.Connect(_host, _port, _useSsl);
                client.Send(emailData);
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// 尝试发送邮件，使用发送人邮箱服务器的账户密码
        /// </summary>
        public void SendByAuthenticate(string emailUsername, string emailPassword)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(_host, _port, _useSsl);
                client.Authenticate(emailUsername, emailPassword);
                client.Send(emailData);
                client.Disconnect(true);
            }
        }
    }
}
