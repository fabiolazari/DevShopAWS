using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SimpleNotificationService;
using Compartilhado.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService.Model;

namespace Compartilhado
{
	public static class AmazonUtil
	{
		public static async Task SalvarAsync(this Pedido pedido)
		{
			var client = new AmazonDynamoDBClient(RegionEndpoint.SAEast1);
			var context = new DynamoDBContext(client);
			await context.SaveAsync(pedido);
		}

		public static async Task SalvarAsync(this Pagamento pagamento)
		{
			var client = new AmazonDynamoDBClient(RegionEndpoint.SAEast1);
			var context = new DynamoDBContext(client);
			await context.SaveAsync(pagamento);
		}

		public static T ToObject<T>(this Dictionary<string, AttributeValue> dictionary)
		{
			var client = new AmazonDynamoDBClient(RegionEndpoint.SAEast1);
			var context = new DynamoDBContext(client);
			var doc = Document.FromAttributeMap(dictionary);
			return context.FromDocument<T>(doc);
		}

		public static async Task SolicitarEnviarEmail(Pedido pedido)
		{
			if(Email.EnviarEmail(pedido))
			{
				pedido.Enviado = true;
				var client = new AmazonDynamoDBClient(RegionEndpoint.SAEast1);
				var context = new DynamoDBContext(client);
				await context.SaveAsync(pedido);
			}
		}

		public static async Task EnviarParaFila(EnumFilasSQS fila, Pedido pedido)
		{
			var json = JsonConvert.SerializeObject(pedido);
			var client = new AmazonSQSClient();
			var request = new SendMessageRequest
			{
				QueueUrl = $"https://sqs.sa-east-1.amazonaws.com/552166525553/{fila}",
				MessageBody = json
			};
			await client.SendMessageAsync(request);
		}

		public static async Task EnviarParaFila(EnumFilasSNS fila, Pedido pedido)
		{
			var json = JsonConvert.SerializeObject(pedido);
			var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.SAEast1);
			var request = new PublishRequest  
			{
				TopicArn = $"arn:aws:sns:sa-east-1:552166525553:{fila}",
				Message = json
			};
			await client.PublishAsync(request);
		}
	}
}
