namespace ProgressBoards
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();

			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				HandleException(e.ExceptionObject as Exception);
			};

			TaskScheduler.UnobservedTaskException += (sender, e) =>
			{
				HandleException(e.Exception);
			};
		}

		private void HandleException(Exception ex)
		{
			if (ex != null)
			{
				Console.WriteLine($"Unhandled exception: {ex.Message}");
			}

			if (System.Diagnostics.Debugger.IsAttached)
			{
				System.Diagnostics.Debugger.Break();
			}
		}
	}
}
