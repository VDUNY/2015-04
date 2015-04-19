using System;
using System.Configuration;
using System.Threading;

using Microsoft.ServiceBus.Messaging;

namespace TopicSender.Console
{
	public class Sender
	{
		private readonly TopicClient _topicClient;

		public Sender()
		{
			// read configuration settings
			var sbConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var sasName = ConfigurationManager.AppSettings["SAS_NAME"];
			var sasKey = ConfigurationManager.AppSettings["SAS_KEY"];

			// create connection string
			var cs = sbConnectionString.Replace( "[SAS_NAME]", sasName ).Replace( "[SAS_KEY]", sasKey );
			_topicClient = TopicClient.CreateFromConnectionString( cs, "mailtopic" );
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
					msg.Properties["From"] = "TopicSender.Console";

					_topicClient.Send( msg );

					System.Console.WriteLine( "Sent message #" + i );

					Thread.Sleep( 1500 );
				}

				System.Console.Write( "\nSend how many messages? " );
				Int32.TryParse( System.Console.ReadLine(), out amount );
			}
		}
	}
}
