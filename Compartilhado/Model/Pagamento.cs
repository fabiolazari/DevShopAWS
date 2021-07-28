using Amazon.DynamoDBv2.DataModel;

namespace Compartilhado.Model
{
    [DynamoDBTable("pagamento")]
    public class Pagamento
    {
        public string NumeroDoCartao { get; set; }

        public string Validade { get; set; }

        public string CVV { get; set; }
    }
}
