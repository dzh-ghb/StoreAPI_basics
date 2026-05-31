namespace Api.Model
{
    // модель ответа на платеж
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string IntentId { get; set; } // уникальный идентификатор транзакции (~возвращается платежной системой)
        public string Secret { get; set; } // (~возвращается платежной системой)
        public string ErrorMessage { get; set; }
    }
}