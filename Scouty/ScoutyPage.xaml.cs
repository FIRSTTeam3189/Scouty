using System.Threading.Tasks;
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
			Task
				.Run(() => DbContext.Instance.InitalizeDb(DbContext.DefaultDatabase))
				.ContinueWith(async t => {
					await t;
					//Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new NavigationPage(new EventsPage())));
					Device.BeginInvokeOnMainThread(() => Navigation.PushModalAsync(new LoginPage()));
				});
			base.OnAppearing();
		}
	}
}
