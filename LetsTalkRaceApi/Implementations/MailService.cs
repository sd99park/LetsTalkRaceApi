using LetsTalkRaceApi.Interfaces;
using LetsTalkRaceApi.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LetsTalkRaceApi.Implementations;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettingsOptions)
    {
        _mailSettings = mailSettingsOptions.Value;
    }

    public bool SendMail(MailData mailData)
    {
        using var emailMessage = new MimeMessage();
        var emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
        emailMessage.From.Add(emailFrom);
        var emailTo = new MailboxAddress(mailData.UserName, mailData.Email);
        emailMessage.To.Add(emailTo);
        
        emailMessage.Subject = mailData.Subject;
        
        var filePath = Directory.GetCurrentDirectory() + "/Templates/CreateProfileTemplate.html";
        var emailTemplateText = File.ReadAllText(filePath);
        emailTemplateText = emailTemplateText.Replace("{userName}", mailData.UserName);
        emailTemplateText = emailTemplateText.Replace("{userEmail}", mailData.Email);
        emailTemplateText = emailTemplateText.Replace("{userPassword}", mailData.Password);
        
        BodyBuilder emailBodyBuilder = new BodyBuilder();
        emailBodyBuilder.HtmlBody = emailTemplateText;
        emailBodyBuilder.TextBody = mailData.Body;

        emailMessage.Body = emailBodyBuilder.ToMessageBody();
        
        using var mailClient = new SmtpClient();
        mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        mailClient.Authenticate(_mailSettings.UserName, _mailSettings.Password);
        mailClient.Send(emailMessage);
        mailClient.Disconnect(true);

        return true;
    }
}