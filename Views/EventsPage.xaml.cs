using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scouty.Models;

using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class EventsPage : ContentPage
	{
        
        ObservableCollection<Team> Teams { get; set; }

        ObservableCollection<Match> Matches { get; set; }


		public EventsPage ()
		{
			InitializeComponent ();
		}

        async void onClick(object sender, EventArgs e) {
            await Navigation.PushAsync(new MatchesPage());
        }
	}
}
