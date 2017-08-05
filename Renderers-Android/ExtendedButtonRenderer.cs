using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using ADVPlatform.Droid;
using Android.Views;
using ADVPlatform;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]
namespace ADVPlatform.Droid
{
    class ExtendedButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            var button = e.NewElement as ExtendedButton;

            if (button != null)
            {
                Android.Widget.Button btn = Control as Android.Widget.Button;
                btn.SetAllCaps(false);
            }
        }

    }
}

