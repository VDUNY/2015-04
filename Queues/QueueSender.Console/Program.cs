namespace QueusSender.Console
{
	class Program
	{
		static void Main( string[] args )
		{
			var sender = new MailSender();
			sender.Send();
		}
	}
}
