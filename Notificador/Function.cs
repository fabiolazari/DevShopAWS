using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;
using Compartilhado;
using Compartilhado.Model;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Notificador
{
    public class Function
    {
        public Function()
        {
        }

        public async Task FunctionHandler(SNSEvent evnt, ILambdaContext context)
        {
            if (evnt.Records.Count > 1) throw new InvalidOperationException("Somente uma mensagem pode ser tratado por vez");
            var message = evnt.Records.FirstOrDefault();
            if (message == null) return;
            await ProcessRecordAsync(message, context);
        }

        private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
        {
            var pedido = JsonConvert.DeserializeObject<Pedido>(record.Sns.Message);
            context.Logger.LogLine($"Pedido enviado por e-mail {pedido.Id} - Cliente: {pedido.Cliente.Nome}");
            await AmazonUtil.SolicitarEnviarEmail(pedido);
        }
    }
}
