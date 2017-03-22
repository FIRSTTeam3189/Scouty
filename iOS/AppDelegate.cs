using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace Scouty.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			var fh = new FileHelper();
			System.Diagnostics.Debug.WriteLine(fh.GetLocalFilePath("test.jwt"));
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
