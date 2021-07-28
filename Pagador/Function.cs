using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Compartilhado;
using Compartilhado.Model;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Pagador
{
    public class Function
    {
        public Function()
        {
        }

        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {
            if (evnt.Records.Count > 1) throw new InvalidOperationException("Somente uma mensagem pode ser tratado por vez");
            var message = evnt.Records.FirstOrDefault();
            if (message == null) return;
            await ProcessMessageAsync(message, context);
        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var pedido = JsonConvert.DeserializeObject<Pedido>(message.Body);
            pedido.Status = StatusDoPedido.Pago;

            try
            {
                await SalvarPagamento(pedido.Pagamento);
                context.Logger.LogLine($"Pagamento registrado com sucesso {pedido.Id} - Cartão: {pedido.Pagamento.NumeroDoCartao}");
            }
            catch (ConditionalCheckFailedException)
            {
                pedido.JustificativaDeCancelamento = $"Pagamento recusado {pedido.Id} - Cartão: {pedido.Pagamento.NumeroDoCartao}";
                pedido.Cancelado = true;
                context.Logger.LogLine($"Erro: {pedido.JustificativaDeCancelamento}");
            }

            if (pedido.Cancelado)
            {
                await AmazonUtil.EnviarParaFila(EnumFilasSNS.falha, pedido);
                await pedido.SalvarAsync();
            }
            else
            {
                await AmazonUtil.EnviarParaFila(EnumFilasSQS.pago, pedido);
                await pedido.SalvarAsync();
            }
        }

		private async Task SalvarPagamento(Pagamento pagamento)
		{
            await pagamento.SalvarAsync();
        }
	}
}
