using System.Threading.Tasks;
using Scouty.Client;
using Xamarin.Forms;

namespace Scouty
{
	public partial class ScoutyPage : ContentPage
	{
		public ScoutyPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			//DependencyService.Get<ISQLPlatformHelper>().DropDatabase(DbContext.DefaultDatabase);
			DbContext.Instance.InitalizeDb(DbContext.DefaultDatabase);
			ServerClient.Instance.Initialize();
			if (ServerClient.Instance.AccessToken != null)
				Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new NavigationPage(new EventsPage())));
			else
				Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new NavigationPage(new EventsPage())));
				//Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new LoginPage()));
			
		}
	}
}
