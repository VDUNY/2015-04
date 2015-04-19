using Topshelf;

namespace TopicListener.Console
{
	class Program
	{
		static void Main( string[] args )
		{
			HostFactory.Run( x =>
			{
				x.Service<Listener>( s =>
				{
					s.ConstructUsing( c => new Listener() );
					s.WhenStarted( c => c.Start() );
					s.WhenStopped( c => c.Stop() );
				} );

				x.SetDescription( "Listens for and processes messages from the Bus." );
				x.SetDisplayName( "VDUNY TopicListener" );
				x.SetServiceName( "TopicListener" );
			} );
		}
	}
}
