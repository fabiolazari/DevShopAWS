using Amazon.DynamoDBv2.DataModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compartilhado.Model
{
    public enum StatusDoPedido
	{
        Coletado,
        Reservado,
        Pago,
        Faturado
	}

    [DynamoDBTable("pedidos")]
    public class Pedido
    {
        public string Id { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataDeCriacao { get; set; }
        public List<Produto> Produtos { get; set; }
        public Cliente Cliente { get; set; }
        public Pagamento Pagamento { get; set; }
        public string JustificativaDeCancelamento { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatusDoPedido Status { get; set; }
		public bool Cancelado { get; set; }
		public bool Pago { get; set; }
        public bool Faturado { get; set; }
        public bool Enviado { get; set; }

		public override string ToString()
		{
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Pedido Id   : {Id}");
            sb.AppendLine($"Valor Total : {ValorTotal}");
            sb.AppendLine($"Cliente     : {Cliente.Nome}");
            sb.AppendLine($"Status      : {Status.ToString()}");
            return sb.ToString();
		}
	}

    
}
