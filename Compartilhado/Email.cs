using System;
using Amazon;
using Compartilhado.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Collections.Generic;

namespace Compartilhado
{
	public static class Email
	{
		public static bool EnviarEmail(Pedido pedido)
		{
			string senderAddress = "lazari.developer@gmail.com";
			string receiverAddress = "flc.rock13@gmail.com";
			string subject = $"Pedido No.:{pedido.Id}";
			string textBody = pedido.ToString();
			string htmlBody = @$"<html>
									<head></head>
									<body>
										<h1>Vendas DEVSHOP</h1>
										<p>Dados pedido:
											<a href='https://aws.amazon.com/'>Amazon SES</a> using the
											<a>  {pedido}</a>.
										</p>
									</body>
									</html>";

			string configSet = "ConfigSet";

			using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.SAEast1))
			{
				var sendRequest = new SendEmailRequest
				{
					Source = senderAddress,
					Destination = new Destination
					{
						ToAddresses =
						new List<string> { receiverAddress }
					},
					Message = new Message
					{
						Subject = new Content(subject),
						Body = new Body
						{
							Html = new Content
							{
								Charset = "UTF-8",
								Data = htmlBody
							},
							Text = new Content
							{
								Charset = "UTF-8",
								Data = textBody
							}
						}
					},

					ConfigurationSetName = configSet
				};
				try
				{
					var response = client.SendEmailAsync(sendRequest);
				}
				catch (Exception)
				{
					return false;
				}
			}
			return true;
		}
	}
}
