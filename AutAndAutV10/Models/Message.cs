using MimeKit;

namespace AutAndAutV10.Models
{
    public class Message
    {
        public MailboxAddress EmailTo { get; set; }
        public string Subject { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, string> MessageTemplateParameters { get; set; }

        public Message(string emailTo, string subject, string messageTemplate, Dictionary<string, string> messageTemplateParameters)
        {
            EmailTo = new MailboxAddress(string.Empty, emailTo);
            Subject = subject;
            MessageTemplate = GetFormatEmailString(messageTemplate, messageTemplateParameters);
        }

        private string GetFormatEmailString(string messageTemplate, Dictionary<string, string> messageTemplateParameters)
        {
            if (messageTemplateParameters == null && string.IsNullOrEmpty(messageTemplate))
            {
                return null;
            }

            if (messageTemplateParameters != null)
            {
                foreach (var param in messageTemplateParameters)
                {
                    messageTemplate = messageTemplate.Replace(param.Key, param.Value);
                }
            }

            return messageTemplate;
        }
    }
}
