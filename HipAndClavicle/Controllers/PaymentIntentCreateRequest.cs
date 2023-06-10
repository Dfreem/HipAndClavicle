using Newtonsoft.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HipAndClavicle.ViewModels.Payment;

public class PaymentIntentCreateRequest
{
    [JsonProperty("items")]
    public OrderItem[] Items { get; set; } = default!;
}