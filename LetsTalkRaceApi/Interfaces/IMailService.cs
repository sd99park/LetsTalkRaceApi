using LetsTalkRaceApi.Models;

namespace LetsTalkRaceApi.Interfaces;

public interface IMailService
{
    bool SendMail(MailData mailData);
}