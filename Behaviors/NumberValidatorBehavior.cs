using System;
using Xamarin.Forms;

namespace ADVPlatform
{
	public class NumberValidatorBehavior : Behavior<Entry> 
	{
		static readonly BindablePropertyKey IsValidPropertyKey = BindableProperty.CreateReadOnly("IsValid", typeof(bool), typeof(NumberValidatorBehavior), false);

		public static readonly BindableProperty IsValidProperty = IsValidPropertyKey.BindableProperty;

		public bool IsValid
		{
			get { return (bool)base.GetValue(IsValidProperty); }
			private set { base.SetValue(IsValidPropertyKey, value); }
		}

		public static bool IsValidCodeProperty
		{
			get;
			set;
		}

		protected override void OnAttachedTo(Entry bindable)
		{
			bindable.TextChanged += BindableTextChanged;
		}

		private void BindableTextChanged (object sender, TextChangedEventArgs e)
		{
			double result;
			if (string.IsNullOrEmpty (e.NewTextValue)) {
				IsValid = true;
				IsValidCodeProperty = true;
			}
			else {
				IsValidCodeProperty = double.TryParse (e.NewTextValue, out result);
				IsValid = double.TryParse (e.NewTextValue, out result);
				((Entry)sender).TextColor = IsValid ? Color.FromHex("#454545") : Color.Red;
			}

		}

		protected override void OnDetachingFrom(Entry bindable)
		{
			bindable.TextChanged -= BindableTextChanged;
		}


	}
}

