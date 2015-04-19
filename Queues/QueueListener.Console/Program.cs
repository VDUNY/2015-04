using Topshelf;

namespace QueueListener.Console
{
	class Program
	{
		static void Main( string[] args )
		{
			HostFactory.Run( x =>
			{
				x.Service<MailListener>( s =>
				{
					s.ConstructUsing( c => new MailListener() );
					s.WhenStarted( c => c.Start() );
					s.WhenStopped( c => c.Stop() );
				} );

				x.SetDescription( "Listens for and processes messages from the Bus." );
				x.SetDisplayName( "VDUNY QueueListener" );
				x.SetServiceName( "QueueListener" );
			} );
		}
	}
}
