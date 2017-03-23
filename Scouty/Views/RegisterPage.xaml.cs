using System;
using System.Collections.Generic;
using Scouty.Client;
using Xamarin.Forms;

namespace Scouty
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			InitializeComponent();

			Login.Clicked += async (sender, e) => await Navigation.PushModalAsync(new LoginPage());
			Register.Clicked += Register_Clicked;
		}

		async void Register_Clicked(object sender, EventArgs e)
		{
			if (Password.Text != Confirm.Text)
				await DisplayAlert("Invalid Password", "Passwords don't match", "OK");
			else if (string.IsNullOrWhiteSpace(Username.Text))
				await DisplayAlert("Invalid User", "No Username specified", "OK");
			else if (FullName.Text.Split(null as char[]).Length < 2)
				await DisplayAlert("Invalid Name", "Must put in full name", "OK");
			else {
				var result = await ServerClient.Instance.Register(Username.Text.Trim(), Password.Text.Trim(), FullName.Text.Trim());
				System.Diagnostics.Debug.WriteLine($"Succeeded: {result}");

				if (result) {
					// Now login
					if (await ServerClient.Instance.Login(Username.Text.Trim(), Password.Text.Trim()))
					{
						await Navigation.PushModalAsync(new NavigationPage(new EventsPage()));
						return;
					}

					await DisplayAlert("Login Failed", "Failed to login to Scouty. Please retry at login page", "OK");
					return;
				}

				await DisplayAlert("Register Failed", "Failed to register to Scouty.", "OK");
			}
		}
	}
}
