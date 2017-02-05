using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scouty.Models;

using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class Teams : ContentPage
	{
        List<Team> teams = new List<Team>();
		public Teams ()
		{
			InitializeComponent ();
		}
	}
}
