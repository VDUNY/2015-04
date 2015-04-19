using System;
using System.Configuration;
using System.Threading;

using Microsoft.ServiceBus.Messaging;

namespace QueusSender.Console
{
	class MailSender
	{
		private readonly QueueClient _queueClient;

		public MailSender()
		{
			// read configuration settings
			var sbConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var sasName = ConfigurationManager.AppSettings["SAS_NAME"];
			var sasKey = ConfigurationManager.AppSettings["SAS_KEY"];
			
			// create connection string
			var cs = sbConnectionString.Replace( "[SAS_NAME]", sasName ).Replace( "[SAS_KEY]", sasKey );

			// get the queue client (requires 'send' permissions)
			_queueClient = QueueClient.CreateFromConnectionString( cs, "Mail" );
		}

		public void Send()
		{
			int amount = 10;

			while ( amount > 0 )
			{
				System.Console.WriteLine( "\n\nSending {0} messages...", amount );

				for ( int i = 0; i < amount; i++ )
				{
					var body = "Message sent at " + DateTime.UtcNow;
					var msg = new BrokeredMessage( body )
					{
						Label = "Message #" + i,
						ContentType = "text/plain"
					};
					msg.Properties["From"] = "QueueSender.Console";

					_queueClient.Send( msg );

					System.Console.WriteLine( "Sent message #" + i );

					Thread.Sleep( 1500 );
				}

				System.Console.Write( "\nSend how many messages? " );
				Int32.TryParse( System.Console.ReadLine(), out amount );
			}
		}
	}
}
