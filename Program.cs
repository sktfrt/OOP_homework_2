using BillingServiceApp;

class Program
{
    static void Main()
    {
        var billing = new BillingService();

        // -----------------------------------------
        // 1. Демонстрация кортежей + деконструкции
        // -----------------------------------------
        var regionCheck = billing.Validate(
            new Subscriber("temp", "EU", SubscriptionStatus.Basic, 5, 1, 10, 1, 1)
        );

        var (ok, error) = regionCheck; 
        Console.WriteLine($"Tuple validation demo: OK={ok}, Error='{error}'");

        Console.WriteLine();


        // -----------------------------------------
        // 2. Создаём тестовых пользователей
        // -----------------------------------------

        var s1 = new Subscriber(
            id: "A001",
            region: "EU",
            status: SubscriptionStatus.Trial,
            tenureMonths: 2,
            devices: 1,
            basePrice: 10,
            day: 1,
            month: 12
        );

        var s2 = new Subscriber(
            id: "B002",
            region: "US",
            status: SubscriptionStatus.Pro,
            tenureMonths: 36,
            devices: 4,
            basePrice: 15,
            day: 5,
            month: 7
        );

        var s3 = new Subscriber(
            id: "C003",
            region: "EU",
            status: SubscriptionStatus.Student,
            tenureMonths: 10,
            devices: 2,
            basePrice: 12,
            day: 1,
            month: 12
        );


        // -----------------------------------------
        // 3. Выполняем тесты
        // -----------------------------------------
        Test("Trial user (birthday today)", billing, s1);
        Test("Pro long-term with many devices", billing, s2);
        Test("Student + birthday discount", billing, s3);


        // -----------------------------------------
        // 4. Негативные тесты Validate
        // -----------------------------------------
        Console.WriteLine("\nNegative tests:");

        var bad1 = new Subscriber("X", "EU", SubscriptionStatus.Basic, -5, 2, 10, 1, 1);
        var (ok1, err1) = billing.Validate(bad1);
        Console.WriteLine($"Tenure < 0 -> OK={ok1}, Error='{err1}'");

        var bad2 = new Subscriber("Y", "Unknown", SubscriptionStatus.Basic, 3, 2, 10, 1, 1);
        var (ok2, err2) = billing.Validate(bad2);
        Console.WriteLine($"Invalid region -> OK={ok2}, Error='{err2}'");

        var bad3 = new Subscriber("Z", "US", SubscriptionStatus.Basic, 3, 0, 10, 1, 1);
        var (ok3, err3) = billing.Validate(bad3);
        Console.WriteLine($"Devices < 1 -> OK={ok3}, Error='{err3}'");
    }


    static void Test(string title, BillingService billing, Subscriber s)
    {
        Console.WriteLine($"--- {title} ---");

        var (ok, error) = billing.Validate(s);
        if (!ok)
        {
            Console.WriteLine("Validation failed: " + error);
            return;
        }

        var total = billing.CalcTotal(s);
        Console.WriteLine($"Total price for {s.Id}: {total:F2}\n");
    }
}
