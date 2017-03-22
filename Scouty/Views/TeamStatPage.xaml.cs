using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Scouty
{
	public partial class TeamStatPage : TabbedPage
	{
		public TeamStatPage(List<TeamStat> allStats, int teamNumber) 
		{
			InitializeComponent();
		}
	}
}
