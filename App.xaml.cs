﻿using Xamarin.Forms;

namespace Scouty
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

            MainPage = new TabbedPage
            {
                Children =
                {
                    new Views.EventsPage(),
                    new Views.GamePage(),
                    new Views.TeamStats()
                    
                }
            };
		}

		protected override void OnStart()
		{
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
