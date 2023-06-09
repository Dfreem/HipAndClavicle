using Newtonsoft.Json;

namespace HipAndClavicle.ViewModels.Payment;

public class PaymentIntentCreateRequest
{
    [JsonProperty("items")]
    public OrderItem[] Items { get; set; } = default!;
}
