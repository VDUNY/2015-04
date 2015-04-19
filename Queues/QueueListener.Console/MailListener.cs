using System;
using System.Configuration;
using System.Threading;

using Microsoft.ServiceBus.Messaging;

namespace QueueListener.Console
{
	class MailListener
	{
		// Thread control
		private volatile bool _isInRunningState;
		private Thread _workerThread;
		// Service Bus
		private readonly QueueClient _queueClient;

		public MailListener()
		{
			// read configuration settings
			var sbConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
			var sasName = ConfigurationManager.AppSettings["SAS_NAME"];
			var sasKey = ConfigurationManager.AppSettings["SAS_KEY"];

			// create connection string
			var cs = sbConnectionString.Replace( "[SAS_NAME]", sasName ).Replace( "[SAS_KEY]", sasKey );

			// get the queue client (requires 'send' permissions)
			_queueClient = QueueClient.CreateFromConnectionString( cs, "Mail", ReceiveMode.PeekLock );
		}

		#region Service Start/Stop

		public void Start()
		{
			// Perform initialization prior to starting the background thread here

			// start the background thread for the main processing loop
			_isInRunningState = true;
			_workerThread = new Thread( Listen )
			{
				Name = "QueueListener"		// name for the thread to help when process debugging
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
		}

		#endregion

		private void Listen()
		{
			while ( _isInRunningState )
			{
				try
				{
					BrokeredMessage message;
					while ( ( message = _queueClient.Receive( TimeSpan.FromSeconds( 30 ) ) ) != null )
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
