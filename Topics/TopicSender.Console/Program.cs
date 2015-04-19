namespace TopicSender.Console
{
	public class Program
	{
		static void Main( string[] args )
		{
			var sender = new Sender();
			sender.Send();
		}
	}
}
