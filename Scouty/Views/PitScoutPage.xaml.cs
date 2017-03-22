using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Scouty
{
	public partial class PitScoutPage : ContentPage
	{
		private static readonly Dictionary<string, ExLevel> ExperienceDict = new Dictionary<string, ExLevel>() {
			{ "None", ExLevel.NA },
			{ "Little", ExLevel.Little },
			{ "Some", ExLevel.Some },
			{ "One Year", ExLevel.OneYear },
			{ "Two Years", ExLevel.TwoYears },
			{ "Three Years", ExLevel.ThreeYears },
			{ "Four to Six Years", ExLevel.FourToSixYears },
			{ "Seven+ Years", ExLevel.SevenPlusYears }
		};
		public PitScoutPage(Team team)
		{
			InitializeComponent();
			foreach (var key in ExperienceDict.Keys)
			{
				DriverEx.Items.Add(key);
				CoDriverEx.Items.Add(key);
				CoachEx.Items.Add(key);
				HumanPlayerEx.Items.Add(key);
			}
		}
	}
}
