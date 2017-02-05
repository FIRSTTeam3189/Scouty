using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scouty.Models;

using Xamarin.Forms;

namespace Scouty.Views
{
	public partial class MatchesPage : TabbedPage
	{
        List<>
        List<Match> matches = new List<Match>();

		public MatchesPage ()
		{
            Children.Add(new MatchesPage());
            Children.Add(new Teams());
		}
    
        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

    }
}
