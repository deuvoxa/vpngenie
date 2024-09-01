using Newtonsoft.Json;

namespace vpngenie.API.Contracts;

public class PaymentNotification
{
    // [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("event")] public string Event { get; set; }

    [JsonProperty("object")] public PaymentObject Object { get; set; }
}

public class PaymentObject
{
    [JsonProperty("id")] public string Id { get; set; }

    // [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("amount")] public Amount Amount { get; set; }

    // [JsonProperty("income_amount")] public Amount IncomeAmount { get; set; }

    // [JsonProperty("description")] public string Description { get; set; }

    // [JsonProperty("recipient")] public Recipient Recipient { get; set; }
    //
    // [JsonProperty("payment_method")] public PaymentMethod PaymentMethod { get; set; }
    //
    // [JsonProperty("captured_at")] public DateTime CapturedAt { get; set; }
    //
    // [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }
    //
    // [JsonProperty("test")] public bool Test { get; set; }
    //
    // [JsonProperty("refunded_amount")] public Amount RefundedAmount { get; set; }
    //
    // [JsonProperty("paid")] public bool Paid { get; set; }
    //
    // [JsonProperty("refundable")] public bool Refundable { get; set; }
    
    [JsonProperty("metadata")] public Metadata Metadata { get; set; }
}

public class Amount
{
    [JsonProperty("value")] public decimal Value { get; set; }
    
    [JsonProperty("currency")] public string Currency { get; set; }
}

public class Recipient
{
    [JsonProperty("account_id")] public string AccountId { get; set; }
    
    [JsonProperty("gateway_id")] public string GatewayId { get; set; }
}

public class PaymentMethod
{
    [JsonProperty("type")] public string Type { get; set; }
    
    [JsonProperty("id")] public string Id { get; set; }
    
    [JsonProperty("saved")] public bool Saved { get; set; }
    
    [JsonProperty("title")] public string Title { get; set; }
    
    [JsonProperty("account_number")] public string AccountNumber { get; set; }
}

public class Metadata
{
    [JsonProperty("telegram_id")] public long TelegramId { get; set; }
}