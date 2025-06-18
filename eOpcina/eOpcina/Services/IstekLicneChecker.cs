using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using eOpcina.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

public class LicnaKartaExpiryChecker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public LicnaKartaExpiryChecker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddHours(12); // Noon today
            if (now > nextRunTime)
                nextRunTime = nextRunTime.AddDays(1); // Schedule for next day if already past noon

            var delay = nextRunTime - now;
            await Task.Delay(delay, stoppingToken);

            await CheckAndSendEmails();

            // Wait 24h before next run
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }

    private async Task CheckAndSendEmails()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var usersToNotify = await db.Users
            .Where(k => k.RokTrajanjaLicneKarte.Date == DateTime.Today.AddDays(30)
                     || k.RokTrajanjaLicneKarte.Date == DateTime.Today.AddDays(15))
            .ToListAsync();

        foreach (var korisnik in usersToNotify)
        {
            await PosaljiEmail(korisnik.Email, "ID Expiry Notification", "halo bolan");
        }
    }

    private async Task PosaljiEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("eopcina@gmail.com", "eOpcina");
        var toAddress = new MailAddress(toEmail);
        const string fromPassword = "whxbfafujcewcjpp";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };

        await smtp.SendMailAsync(message);
    }
}
