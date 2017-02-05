using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scouty.Models;

using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class EventsPage : ContentPage
	{
        List<Event> events = new List<Event>();

		public EventsPage ()
		{
			InitializeComponent ();
		}

        async void onClick(object sender, EventArgs e) {
            await Navigation.PushAsync(new MatchesPage());
        }
	}
}
