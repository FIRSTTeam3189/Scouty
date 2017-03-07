using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Scouty
{
	public partial class EventsPage : ContentPage
	{
		public EventsPage()
		{
			InitializeComponent();
		}
	}

	public class EventGroup : ObservableCollection<Event> {
		public string Week { get; set; } = "";
		public string WeekShort { get; set; } = "";
	}
}
