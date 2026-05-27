namespace Api.Model
{
    // модель ответа на платеж
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string IntentId { get; set; }
        public string Secret { get; set; }
        public string ErrorMessage { get; set; }
    }
}