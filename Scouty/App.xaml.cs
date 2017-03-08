﻿using Xamarin.Forms;

namespace Scouty
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new ScoutyPage();
		}

		protected override void OnStart()
		{
			DependencyService.Get<ISQLPlatformHelper>().DropDatabase(DbContext.DefaultDatabase);
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}