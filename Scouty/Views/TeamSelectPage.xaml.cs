using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class TeamSelectPage : ContentPage
	{
		public ObservableCollection<Alliance> Alliances { get; set; }
		public TeamSelectPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		public class Alliance : ObservableCollection<Team> 
		{
			public AllianceColor AllianceColor { get; }
			public string Name => AllianceColor == Scouty.AllianceColor.Blue ? "Blue" : "Red";
			public Color HeaderColor => AllianceColor == Scouty.AllianceColor.Blue ? Color.FromHex("#9494ff") : Color.FromHex("#ff9494");
		}
	}
}
