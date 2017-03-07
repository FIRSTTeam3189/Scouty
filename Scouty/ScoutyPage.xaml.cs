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
			Device.BeginInvokeOnMainThread(async () =>
			{
				var context = new BlueAllianceClient.BlueAllianceContext();
				var ev = await context.GetEvent(2017, "cama");

				var dbContext = DbContext.Instance;
				await dbContext.InitalizeDb(DbContext.DefaultDatabase);

				await dbContext.InsertOrUpdateEvent(ev);
				System.Diagnostics.Debug.WriteLine("Yay");
			});
			base.OnAppearing();
		}
	}
}
