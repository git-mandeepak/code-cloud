using System;
using Xamarin.Forms;

namespace ADVPlatform
{
	public class ExtendedPicker: Picker
	{
		public static readonly BindableProperty TextColorProperty = 
			BindableProperty.Create ("TextColor", typeof(Color), typeof(ADVPlatform.ExtendedPicker), Color.Default);
		

		public Color TextColor
		{
			get
			{
				return (Color)GetValue (TextColorProperty);
			}
			set
			{
				SetValue (TextColorProperty, value);
			}
		}

		public static readonly BindableProperty HasBorderProperty = 
			BindableProperty.Create ("HasBorder", typeof(bool), typeof(ADVPlatform.ExtendedPicker), true);

		public bool HasBorder
		{
			get
			{
				return (bool)GetValue (HasBorderProperty);
			}
			set
			{
				SetValue (HasBorderProperty, value);
			}
		}

		public static readonly BindableProperty TextSizeProperty = 
			BindableProperty.Create ("TextSize", typeof(float), typeof(ADVPlatform.ExtendedPicker), 10f);

		public float TextSize
		{
			get
			{
				return (float)GetValue (TextSizeProperty);
			}
			set
			{
				SetValue (TextSizeProperty, value);
			}
		}

		public static readonly BindableProperty TextFontFamilyProperty = 
			BindableProperty.Create ("TextFontFamily", typeof(string), typeof(ExtendedPicker), "Calibri");

		public string TextFontFamily
		{
			get
			{
				return (string)GetValue (TextFontFamilyProperty);
			}
			set
			{
				SetValue (TextSizeProperty, value);
			}
		}

		public static readonly BindableProperty BackGroundProperty =
			BindableProperty.Create("BackGround", typeof(bool), typeof(ExtendedPicker), true);

		public bool BackGround
		{
			get { return (bool)GetValue(BackGroundProperty); }
			set { SetValue(BackGroundProperty, value); }
		}

        public static readonly BindableProperty PlaceholderTextColorProperty =
			BindableProperty.Create("PlaceholderTextColor", typeof(Color), typeof(ExtendedPicker), Color.Default);

		public Color PlaceholderTextColor
        {
            get { return (Color)GetValue(PlaceholderTextColorProperty); }
            set { SetValue(PlaceholderTextColorProperty, value); }
        }
	}
}
