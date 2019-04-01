using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SWZSR.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWZSR.Infrastructure
{
    public enum MailType
    {
        NewOrder,
        AcceptedOrder,
        StartedOrder,
        FinishedOrder,
        Registration
    };

    public class NotificationManager
    {
        private readonly ApplicationDbContext _db;
        public NotificationManager(ApplicationDbContext context)
        {
            _db = context;
        }

        // Wysyłanie wiadomości email dotyczącej statusu naprawy
        public async Task SendEmail(MailType mailType, string messageTo, IHostingEnvironment _env, int orderId = 0, decimal total = 0)
        {
            var builder = new BodyBuilder();

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplates"
                            + Path.DirectorySeparatorChar.ToString();

            string messageBody = "";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SWZSR INFO", "swzsr465@gmail.com"));
            message.To.Add(new MailboxAddress(messageTo));

            switch (mailType)
            {
                case MailType.NewOrder:
                    using (StreamReader SourceReader = File.OpenText(pathToTemplate + "OrderNew.html"))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    messageBody = string.Format(builder.HtmlBody, orderId);
                    message.Subject = "SWZSR: Zlecenie zostało przyjęte!";
                    break;
                case MailType.StartedOrder:
                    using (StreamReader SourceReader = File.OpenText(pathToTemplate + "OrderStarted.html"))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    messageBody = string.Format(builder.HtmlBody, orderId);
                    message.Subject = "SWZSR: Naprawa została rozpoczęta!";
                    break;
                case MailType.FinishedOrder:
                    using (StreamReader SourceReader = File.OpenText(pathToTemplate + "OrderFinish.html"))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    messageBody = string.Format(builder.HtmlBody, orderId, total.ToString());
                    message.Subject = "SWZSR: Zlecenie jest gotowe do odbioru!";
                    break;
            }

            message.Body = new TextPart("html") { Text = messageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("swzsr465@gmail.com", "nqtnvprxmlewjdbr");
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        // Wysyłanie wiadomości email dotyczącej rejestracji użytkownika
        public async Task SendEmail(MailType mailType, string messageTo, IHostingEnvironment _env, string messageContent)
        {
            var builder = new BodyBuilder();

            var pathToTemplate = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplates"
                            + Path.DirectorySeparatorChar.ToString();

            string messageBody = "";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SWZSR INFO", "swzsr465@gmail.com"));
            message.To.Add(new MailboxAddress(messageTo));

            switch (mailType)
            {
                case MailType.Registration:
                    using (StreamReader SourceReader = File.OpenText(pathToTemplate + "ConfirmEmail.html"))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    messageBody = string.Format(builder.HtmlBody, messageContent);
                    message.Subject = "SWZSR: Potwierdź swój adres email";
                    break;
            }

            message.Body = new TextPart("html") { Text = messageBody };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("swzsr465@gmail.com", "nqtnvprxmlewjdbr");
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        public void SendSMS(string phoneNumber, int orderId, decimal total)
        {
            var messageContent = "SWZSR Info: Twoje zlecenie nr " + orderId + " zostało zakończone i jest gotowe do odbioru! Kwota do zapłaty: " + total.ToString() + " PLN.";
            var SMSApiUserEmail = _db.Settings.Where(k => k.Key == "smsapiemail").FirstOrDefault().Value;
            var SMSApiUserPasswordHash = _db.Settings.Where(k => k.Key == "smsapihash").FirstOrDefault().Value;

            try
            {
                SMSApi.Api.Client client = new SMSApi.Api.Client(SMSApiUserEmail);
                client.SetPasswordHash(SMSApiUserPasswordHash);

                var smsApi = new SMSApi.Api.SMSFactory(client);

                var result =
                    smsApi.ActionSend()
                        .SetText(messageContent)
                        .SetTo(phoneNumber)
                        .SetSender("ECO") //Pole nadawcy lub typ wiadomość 'ECO', '2Way'
                        .Execute();

                System.Console.WriteLine("SMS sent with code: " + result.Count);

                string[] ids = new string[result.Count];

                for (int i = 0, l = 0; i < result.List.Count; i++)
                {
                    if (!result.List[i].isError())
                    {
                        //Nie wystąpił błąd podczas wysyłki (numer|treść|parametry... prawidłowe)
                        if (!result.List[i].isFinal())
                        {
                            //Status nie jest koncowy, może ulec zmianie
                            ids[l] = result.List[i].ID;
                            l++;
                        }
                    }
                }

                System.Console.WriteLine("Get:");
                result =
                    smsApi.ActionGet()
                        .Ids(ids)
                        .Execute();

                foreach (var status in result.List)
                {
                    System.Console.WriteLine("ID: " + status.ID + " Number: " + status.Number + " Points:" + status.Points + " Status:" + status.Status + " IDx: " + status.IDx);
                }

                for (int i = 0; i < result.List.Count; i++)
                {
                    if (!result.List[i].isError())
                    {
                        var deleted =
                            smsApi.ActionDelete()
                                .Id(result.List[i].ID)
                                .Execute();
                        System.Console.WriteLine("Deleted: " + deleted.Count);
                    }
                }
            }
            catch (SMSApi.Api.ActionException e)
            {
                /**
                 * Błędy związane z akcją (z wyłączeniem błędów 101,102,103,105,110,1000,1001 i 8,666,999,201)
                 * http://www.smsapi.pl/sms-api/kody-bledow
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ClientException e)
            {
                /**
                 * 101 Niepoprawne lub brak danych autoryzacji.
                 * 102 Nieprawidłowy login lub hasło
                 * 103 Brak punków dla tego użytkownika
                 * 105 Błędny adres IP
                 * 110 Usługa nie jest dostępna na danym koncie
                 * 1000 Akcja dostępna tylko dla użytkownika głównego
                 * 1001 Nieprawidłowa akcja
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.HostException e)
            {
                /* błąd po stronie servera lub problem z parsowaniem danych
                 * 
                 * 8 - Błąd w odwołaniu
                 * 666 - Wewnętrzny błąd systemu
                 * 999 - Wewnętrzny błąd systemu
                 * 201 - Wewnętrzny błąd systemu
                 * SMSApi.Api.HostException.E_JSON_DECODE - problem z parsowaniem danych
                 */
                System.Console.WriteLine(e.Message);
            }
            catch (SMSApi.Api.ProxyException e)
            {
                // błąd w komunikacji pomiedzy klientem a serverem
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
