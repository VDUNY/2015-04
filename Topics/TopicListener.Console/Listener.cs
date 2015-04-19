using System;
using System.Configuration;
using System.Threading;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace TopicListener.Console
{
	class Listener
	{
		// Thread control
		private volatile bool _isInRunningState;
		private Thread _workerThread;

		// constants
		private const string TopicPath = "mailtopic";

		// Service Bus
		private readonly string _connectionString ;
		private readonly NamespaceManager _namespaceManager;
		private readonly Guid _subscriptionId = Guid.NewGuid();
		private SubscriptionClient _subscriptionClient;

		public Listener()
		{
			System.Console.WriteLine( "Starting subscription '{0}'", _subscriptionId );

			// read configuration settings
			var sbConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var sasName = ConfigurationManager.AppSettings["SAS_NAME"];
			var sasKey = ConfigurationManager.AppSettings["SAS_KEY"];

			// create connection string
			_connectionString = sbConnectionString.Replace( "[SAS_NAME]", sasName ).Replace( "[SAS_KEY]", sasKey );

			_namespaceManager = NamespaceManager.CreateFromConnectionString( _connectionString );
		}

		#region Service control

		public void Start()
		{
			// Perform initialization prior to starting the background thread here
			_namespaceManager.CreateSubscription( TopicPath, _subscriptionId.ToString() );
			_subscriptionClient = SubscriptionClient.CreateFromConnectionString( _connectionString, TopicPath, _subscriptionId.ToString() );

			// start the background thread for the main processing loop
			_isInRunningState = true;
			_workerThread = new Thread( Listen )
			{
				Name = "TopicListener"		// name for the thread to help when process debugging
			};

			_workerThread.Start();

			while ( !_workerThread.IsAlive ) { } // block until thread is in an active state
		}

		public void Stop()
		{
			// stop the background thread
			_isInRunningState = false;
			_workerThread.Join();	// block this thread until child thread exits

			// perform any cleanup actions neccessary here
			_namespaceManager.DeleteSubscription( TopicPath, _subscriptionId.ToString() );
		}

		#endregion

		private void Listen()
		{
			while ( _isInRunningState )
			{
				try
				{
					BrokeredMessage message;
					while ( ( message = _subscriptionClient.Receive( TimeSpan.FromSeconds( 30 ) ) ) != null )
					{
						// Process message from queue
						System.Console.WriteLine( "Body: {0}", message.GetBody<string>() );
						System.Console.WriteLine( "Label: {0}", message.Label );
						System.Console.WriteLine( "MessageID: {0}", message.MessageId );
						System.Console.WriteLine( "'From' Property: {0}\n", message.Properties["From"] );

						// Remove message from queue
						message.Complete();
					}
				}
				catch ( Exception )
				{
					// handle internal errors as usual
					// special care needs to be taken if SB Connection dies
					throw;
				}
			}
		}
	}
}
