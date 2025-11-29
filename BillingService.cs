using System;

namespace BillingServiceApp;

public enum SubscriptionStatus { Trial, Basic, Pro, Student };

public class Subscriber
{
    public string Id { get; private set; }
    public string Region { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public int TenureMonths { get; private set; }
    public int Devices { get; private set; }
    public double BasePrice { get; private set; }
    public (int day, int month)? BirthDate { get; private set; } = null;

    public Subscriber(string id, string region, SubscriptionStatus status,
    int tenureMonths, int devices, double basePrice, int day, int month)
    {
        Id = string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentException("Id required")
            : id;

        Region = string.IsNullOrWhiteSpace(region)
            ? throw new ArgumentException("Region required")
            : region;

        BasePrice = basePrice < 0
            ? throw new ArgumentException("BasePrice cannot be negative")
            : basePrice;

        Status = status;
        Devices = devices;
        TenureMonths = tenureMonths;

        BirthDate ??= (day, month);
    }

    public bool HasManyDevices => Devices > 3;
    public bool IsLongTermPro => Status == SubscriptionStatus.Pro || TenureMonths > 24;
}

public class BillingService
{
    private static double ApplyStatusDiscount(SubscriptionStatus status, int tenure, double basePrice) =>
        status switch
        {
            SubscriptionStatus.Trial => 0,
            SubscriptionStatus.Student => basePrice * 0.5,
            SubscriptionStatus.Pro when tenure >= 24 => basePrice * 0.85, // s.IsLongTermPro => ...
            SubscriptionStatus.Pro when tenure >= 12 => basePrice * 0.9,  // !s.IsLongTermPro => ...
            SubscriptionStatus.Pro => basePrice,
            SubscriptionStatus.Basic => basePrice
        };

    private static double ApplySurchargeToDevices(int devices, double basePrice) =>
        devices switch
        {
            > 3 => basePrice + 4.99, // s.HasManyDevices => ...
            _ => basePrice
        };

    private static double ApplyRegionTax(string region, double basePrice) =>
        region switch
        {
            "EU" => basePrice + basePrice * 0.21,
            "US" => basePrice + basePrice * 0.07,
            _ => basePrice
        };

    public int IsBirthDate(Subscriber s, int date, int month)
    {
        if (s.BirthDate?.day == date && s.BirthDate?.month == month)
        {
            Console.WriteLine($"Happy Birthday! Your personal discount is {date}$");
            return date;
        }

        return 0;
    }

    public double CalcTotal(Subscriber s)
    {
        s = s ?? throw new ArgumentNullException(nameof(s));
        var (ok, error) = Validate(s);
        if (!ok) throw new ArgumentException(error);

        var today = (day : DateTime.Today.Day, month : DateTime.Today.Month);

        double PriceAfterStatus() => ApplyStatusDiscount(s.Status, s.TenureMonths, s.BasePrice);
        double WithDevices(double x) => ApplySurchargeToDevices(s.Devices, x);
        double WithTax(double x) => ApplyRegionTax(s.Region, x);

        return WithTax(WithDevices(PriceAfterStatus())) - IsBirthDate(s, today.day, today.month);
    }

    public (bool Ok, string Error) Validate(Subscriber s)
    {
        if (s is null) return (false, "No subscriber\n");
        if (s.Devices < 1) return (false, "Quantity of devices cannot be less then 1\n");
        if (s.TenureMonths < 0) return (false, "Tenure months cannot be negative\n");
        if (!(s.Region is "EU" or "US")) return (false, "Billing service in your region is not supported\n");

        return (true, "");
    }

}
