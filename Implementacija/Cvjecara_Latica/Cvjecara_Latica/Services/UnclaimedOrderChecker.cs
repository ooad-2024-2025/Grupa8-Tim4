using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Cvjecara_Latica.Data;
using Cvjecara_Latica.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class UnclaimedOrderChecker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UnclaimedOrderChecker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var _emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                    // 1. Podsjetnici
                    var unclaimedOrders = await _context.Orders
                        .Where(o => !o.IsPickedUp && o.DeliveryDate < DateTime.Now)
                        .Include(o => o.Person)
                        .ToListAsync();

                    foreach (var order in unclaimedOrders)
                    {
                        var person = order.Person;
                        if (person != null)
                        {
                            await _emailService.SendReminder(person.Email);
                        }
                    }

                    // 2. Deaktivacije
                    var overdueUsers = await _context.Orders
                        .Where(o => !o.IsPickedUp && o.DeliveryDate < DateTime.Now.AddDays(-1))
                        .Select(o => o.PersonID)
                        .Distinct()
                        .ToListAsync();

                    foreach (var userId in overdueUsers)
                    {
                        var user = await _context.Users.FindAsync(userId);
                        if (user != null && !user.LockoutEnabled)
                        {
                            user.LockoutEnabled = true;
                            user.LockoutEnd = DateTimeOffset.MaxValue;
                            _context.Users.Update(user);
                            await _emailService.SendDeactivationNotice(user.Email);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Greška u UnclaimedOrderChecker servisu: " + ex.Message);
                // Po potrebi loguj u bazu ili fajl
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}