using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Compartilhado;
using Compartilhado.Model;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Faturador
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
            pedido.Status = StatusDoPedido.Faturado;
            pedido.Faturado = true;

            await AmazonUtil.EnviarParaFila(EnumFilasSNS.faturado, pedido);
            await pedido.SalvarAsync();
            context.Logger.LogLine($"Pedido faturado com sucesso {pedido.Id} - Cliente: {pedido.Cliente.Nome}");
        }
    }
}
