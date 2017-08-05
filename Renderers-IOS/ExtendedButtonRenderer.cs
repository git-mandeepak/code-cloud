using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ADVPlatform;
using ADVPlatform.iOS;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace ADVPlatform.iOS
{
	public class ExtendedButtonRenderer: ButtonRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged (e);

			var button = e.NewElement as ExtendedButton;

			if (button != null) {
				var btn = (UIButton)Control;
				btn.TouchDown += (sender, arg) => {
					if(button.ClassId == "2"){
					btn.Highlighted = false;
					button.BackgroundColor = Color.FromHex("#ea202b");
					button.TextColor = Color.White;
					}
				};
				btn.TouchUpInside+= (sender, args) => {
					if(button.ClassId == "2"){
					btn.Highlighted = false;
					button.BackgroundColor = Color.FromHex("#ea202b");
					button.TextColor = Color.White;
					}
				};
            }
		}
	}
}

