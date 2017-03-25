using System;
using System.Collections.Generic;
using System.Text;
using Scouty.Stats;
using Xamarin.Forms;

namespace Scouty
{
	public partial class TeamStatPage : ContentPage
	{
		public TeamStatPage(List<TeamStatReport> allStats, int teamNumber) 
		{
			InitializeComponent();

			Title = $"Team {teamNumber} Stats";

			var b = new StringBuilder();
			foreach (var t in allStats) {
				b.Append($"{t.StatType.GetEnumString()}\n\tRank: {t.Rank}\n\tZ-Score: {(t.ZScore).ToString("N4")}\n\tAvg: {t.Value.ToString("N")}\n\n-----------\n\n");
			}

			Info.Text = b.ToString();
		}
	}
}
