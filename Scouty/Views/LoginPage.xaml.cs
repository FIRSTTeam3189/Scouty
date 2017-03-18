using System;
using System.Collections.Generic;
using Scouty.Client;
using Xamarin.Forms;

namespace Scouty
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			Register.Clicked += async (a, b) => await Navigation.PushModalAsync(new RegisterPage());
			Login.Clicked += Login_Clicked;
		}

		async void Login_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(Username.Text))
				await DisplayAlert("Invalid User", "No Username specified", "OK");
			else
			{
				var result = await ServerClient.Instance.Login(Username.Text.Trim(), Password.Text.Trim());
				System.Diagnostics.Debug.WriteLine($"Succeeded: {result}");

			}
		}
	}
}
