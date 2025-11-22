namespace BillingServiceApp;

public class Subscriber
{
    public enum SubscriptionStatus { Trial, Basic, Pro, Student };
    public string Id { get; private set; }
    public string Region { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public int TenureMonths { get; private set; }
    public int Devices { get; private set; }
    public double BasePrice { get; private set; }

    public Subscriber(string id, string region, string status, int tenureMonths, int devices, double basePrice)
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

        Status = (SubscriptionStatus)Enum.Parse(typeof(SubscriptionStatus), status);
        TenureMonths = tenureMonths;
        Devices = devices;
    }
}

public class BillingService
{
    private static double ApplyStatusDiscount(Subscriber.SubscriptionStatus status, int tenure, double basePrice) =>
        status switch
        {
            Subscriber.SubscriptionStatus.Trial => 0,
            Subscriber.SubscriptionStatus.Student => basePrice * 0.5,
            Subscriber.SubscriptionStatus.Pro when tenure >= 24 => basePrice * 0.85,
            Subscriber.SubscriptionStatus.Pro when tenure >= 12 => basePrice * 0.9,
            Subscriber.SubscriptionStatus.Pro => basePrice,
            Subscriber.SubscriptionStatus.Basic => basePrice
        };

    private static double ApplyDevicesQuantity(int devices, double basePrice) =>
        devices switch
        {
            > 3 => basePrice + 4.99,
            _ => basePrice
        };

    private static double ApplyRegionTax(string region, double basePrice) =>
        region switch
        {
            "EU" => basePrice + basePrice * 0.21,
            "US" => basePrice + basePrice * 0.07,
            _ => basePrice
        };

    public double CalcTotal(Subscriber s)
    {
        s = s ?? throw new ArgumentNullException(nameof(s));

        double PriceAfterStatus() => ApplyStatusDiscount(s.Status, s.TenureMonths, s.BasePrice);
        double WithDevices(double x) => ApplyDevicesQuantity(s.Devices, x);
        double WithTax(double x) => ApplyRegionTax(s.Region, x);

        return WithTax(WithDevices(PriceAfterStatus()));
    }

    public (bool Ok, string Error) Validate(Subscriber s)
    {
        if (s is null) return (false, "No subscriber\n");
        if (s.BasePrice < 0) return (false, "Price cannot be negative\n");
        if (s.Id == null) return (false, "Id is missing\n");

        return (true, "");
    }

}
