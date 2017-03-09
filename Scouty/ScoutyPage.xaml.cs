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
				.Run(async () => await DbContext.Instance.InitalizeDb(DbContext.DefaultDatabase))
				.ContinueWith(async t => {
					await t;
				Device.BeginInvokeOnMainThread(async () => await Navigation.PushModalAsync(new NavigationPage(new EventsPage())));
				});
			base.OnAppearing();
		}
	}
}
