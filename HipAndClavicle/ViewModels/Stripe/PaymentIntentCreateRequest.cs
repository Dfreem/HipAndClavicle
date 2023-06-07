using Newtonsoft.Json;

namespace HipAndClavicle.ViewModels.Stripe;

public class Item
{
    [JsonProperty("id")]
    public string Id { get; set; } = default!;
}

public class PaymentIntentCreateRequest
{
    [JsonProperty("items")]
    public Item[] Items { get; set; } = default!;
}
