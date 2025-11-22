using System;
using System.Collections.Generic;

namespace BillingServiceApp;

class Program
{
    static void Main()
    {
        var subscribers = new List<Subscriber>
        {
            new Subscriber("A-1", "EU", "Trial", 0, 1, 9.99),
            new Subscriber("B-2", "US", "Pro", 18, 4, 14.99),
            new Subscriber("C-3", "EU", "Student", 6, 2, 12.99)
        };

        var billing = new BillingService();

        foreach (var sub in subscribers)
        {
            double total = billing.CalcTotal(sub);
            Console.WriteLine($"Subscriber {sub.Id} ({sub.Status}): Total = {total:F2}");
        }
    }
}
