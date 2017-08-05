#region Usings
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Linq;
using Plugin.Connectivity;
using System.Threading.Tasks;
using Plugin.DeviceInfo;

#endregion

namespace ADVPlatform
{
	public partial class LoginPage : ContentPage
	{
		#region Private Variables

		new event PropertyChangedEventHandler PropertyChanged;

		IContactService contacts;
		List<Contacts> contactsList;

		Enums.FromPage fromPage;

		private int countryID;

		private string phoneNumber;
		private string password;
		private string loginText;
		private string countryCode;

		private bool phoneNumberInvalidImageVisibility;
		private bool phoneNumberValidImageVisibility;
		private bool passwordInvalidImageVisibility;
		private bool passwordValidImageVisibility;

		private bool isLoading;
		private bool isCountryTapped = false;

		#endregion

		#region Public Properties

		public int CountryID {
			get {
				return countryID;
			}
			set {
				countryID = value;
				OnPropertyChanged ("CountryID");
			}
		}

		public string CountryCode {
			get {
				return countryCode;
			}
			set {
				countryCode = value;
				OnPropertyChanged ("CountryCode");
			}
		}

		public string LoginText {
			get {
				return loginText;
			}
			set {
				loginText = value;
				OnPropertyChanged ("LoginText");
			}
		}

		public string PhoneNumber {
			get {
				return phoneNumber;
			}
			set {
				if (!string.IsNullOrEmpty (value)) {
					PhoneNumberInvalidImageVisibility = false;	
				}
				phoneNumber = value;
				OnPropertyChanged ("PhoneNumber");
			}
		}

		public string Password {
			get {
				return password;
			}
			set {
				if (!string.IsNullOrEmpty (value)) {
					PasswordInvalidImageVisibility = false;		
					PasswordValidImageVisibility = true;			
				} else {
					PasswordValidImageVisibility = false;
				}

				password = value;
				OnPropertyChanged ("Password");
			}
		}

		public bool PhoneNumberInvalidImageVisibility {
			get {
				return phoneNumberInvalidImageVisibility;
			}
			set {
				phoneNumberInvalidImageVisibility = value;
				OnPropertyChanged ("PhoneNumberInvalidImageVisibility");
			}
		}

		public bool PhoneNumberValidImageVisibility {
			get {
				return phoneNumberValidImageVisibility;
			}
			set {
				phoneNumberValidImageVisibility = value;
				OnPropertyChanged ("PhoneNumberValidImageVisibility");
			}
		}

		public bool PasswordInvalidImageVisibility {
			get {
				return passwordInvalidImageVisibility;
			}
			set {
				passwordInvalidImageVisibility = value;
				OnPropertyChanged ("PasswordInvalidImageVisibility");
			}
		}

		public bool PasswordValidImageVisibility {
			get {
				return passwordValidImageVisibility;
			}
			set {
				passwordValidImageVisibility = value;
				OnPropertyChanged ("PasswordValidImageVisibility");
			}
		}

		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				isLoading = value;
				OnPropertyChanged ("IsLoading");
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ADVPlatform.LoginPage"/> class.
		/// </summary>
		public LoginPage (Enums.FromPage fromPage)
		{
			InitializeComponent ();
			this.fromPage = fromPage;
			setDefaultValues ();
			BindingContext = this;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the default values.
		/// </summary>
		void setDefaultValues ()
		{
			contactsList = new List<Contacts> ();
			frmOverLay.BackgroundColor = new Color (0, 0, 0, 0.5);
			NavigationPage.SetHasNavigationBar (this, false);

			phoneNumber = string.Empty;
			password = string.Empty;

			loginText = Constants.LogInText;
			countryCode = Constants.SelectCountry;

			txtPhoneNumber.Unfocused += (sender, e) => {
				//PhoneNumberValidImageVisibility |= !string.IsNullOrEmpty (PhoneNumber);
			};

			txtPassword.Unfocused += (sender, e) => {
				PasswordValidImageVisibility |= !string.IsNullOrEmpty (Password);
			};

			if(Device.OS == TargetPlatform.iOS)				
				RequestPermission();
		}

		void RequestPermission ()
		{
			contacts = DependencyService.Get<IContactService> ();
			contacts.RequestPermission();
		}

		async Task GetContacts ()
		{
			contactsList.Clear ();

			if (contacts == null) {
				contacts = DependencyService.Get<IContactService> ();
			}

			contactsList = await contacts.GetContacts ();

			if (contactsList != null && contactsList.Count > 0) 
			{
				await Common.SyncContacts (contactsList);
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		bool Validate ()
		{
			bool isValid = true;

			if (string.IsNullOrEmpty (PhoneNumber)) {
				isValid = false;
				PhoneNumberValidImageVisibility = false;
				PhoneNumberInvalidImageVisibility = true;
			} else if (!Regex.IsMatch (PhoneNumber, Constants.PhoneNumberRegEx)) {
				isValid = false;
				PhoneNumberValidImageVisibility = false;
				PhoneNumberInvalidImageVisibility |= PhoneNumberValidationBehavior.IsInvalidCodeProperty;
			}

			if (string.IsNullOrEmpty (Password)) {
				isValid = false;
				PasswordInvalidImageVisibility = true;
			}

			isValid &= PhoneNumberValidationBehavior.IsInvalidCodeProperty;

			return isValid;
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises the appearing event.
		/// </summary>
		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			if (!CrossConnectivity.Current.IsConnected) {
				DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.NoInternetMessage, Constants.AlertDoneButtonDefaultText);
			}

			isCountryTapped = false;
			CountryCode = App.CountryCode;
			CountryID = App.CountryID;
		}

		/// <summary>
		/// Buttons the login clicked.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void BtnLoginClicked (object sender, EventArgs e)
		{	
			if (!CrossConnectivity.Current.IsConnected) {
				IsLoading = false;
				await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.NoInternetMessage, Constants.AlertDoneButtonDefaultText);
			} else {				
				// disable the button to avoid multiple clicks
				if (Validate ()) {

					if (CountryID == 0) {
						IsLoading = false;
						await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.CountryCodeInvalidMessage, Constants.AlertDoneButtonDefaultText);
						return;
					}	

					IsLoading = true;
					btnLogin.IsEnabled = false;
					Login login = new Login ();
					login.country = CountryID; 
					login.mobile = txtPhoneNumber.Text;
					login.password = txtPassword.Text;
					login.device_id = CrossDeviceInfo.Current.Id; 
					login.device_token = SessionManager.DeviceToken;
					login.device_type = Device.OS == TargetPlatform.iOS ? (int)Enums.DEVICETYPE.IOS : (int)Enums.DEVICETYPE.ANDROID;
					var userDetail = await new UserManager ().Login (login);

					if (userDetail.response == null && userDetail.status_code == Constants.ServerNotRunningStatus) {
						if (userDetail.status_code == Constants.ServerNotRunningStatus) {
							IsLoading = false;
							await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.ServerNotRunningMessage, Constants.AlertCancelButtonDefaultText);
						} else {
							IsLoading = false;
							await DisplayAlert (Constants.AlertTitleDefaultText, Constants.MobileNumberPasswordIncorrect, Constants.AlertDoneButtonDefaultText);
						}
					} else {
						if (userDetail.status_code == Constants.SuccessStatus) {
							SessionManager.AccessToken = userDetail.response.access_token;
							SessionManager.UserId = userDetail.response.user.id;
							SessionManager.PhoneNumber = userDetail.response.user.mobile;
							SessionManager.CurrentUserProfilePic = string.Concat (Constants.AWS.S3URI, userDetail.response.user.profile_pic) ;

							if ((Enums.UserStage)userDetail.response.user.stage == Enums.UserStage.OnBoarding)
								await Navigation.PushAsync (new UserProfilePage (userDetail.response.user, Enums.FromPage.LoginPage));
							else if ((Enums.UserStage)userDetail.response.user.stage == Enums.UserStage.Hints || (Enums.UserStage)userDetail.response.user.stage == Enums.UserStage.Completed)
								await Navigation.PushAsync (new HomePage (Enums.FromPage.LoginPage, (Enums.UserStage)userDetail.response.user.stage), true);
							else
								await Navigation.PushAsync (new HomePage (Enums.FromPage.LoginPage, Enums.UserStage.Completed), true);

							IsLoading = false;

							await GetContacts ();

						} else {
							IsLoading = false;
							await DisplayAlert (Constants.AlertTitleDefaultText, Constants.MobileNumberPasswordIncorrect, Constants.AlertCancelButtonDefaultText);
						}
					}
				} else {
					await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.ReqFieldCommonMessage, Constants.AlertDoneButtonDefaultText);
				}
			}

			btnLogin.IsEnabled = true;
		}

		/// <summary>
		/// Raises the close button gesture recognizer tapped event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnBackButtonGestureRecognizerTapped (object sender, EventArgs e)
		{
			if (fromPage == Enums.FromPage.ChangePassword) {
				Navigation.PushAsync (new LandingPage (), true);
			} else {
				Navigation.PopAsync (true);

			}

			App.CountryCode = Constants.SelectCountry;
			App.CountryID = 0;
		}

		/// <summary>
		/// Raises the forgot button gesture recognizer tapped event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnForgotButtonGestureRecognizerTapped (object sender, EventArgs e)
		{
			Navigation.PushAsync (new ResetPasswordPage (), true);
		}

		/// <summary>
		/// /*Raises the create account button gesture recognizer tapped event.*/
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnCreateAccountButtonGestureRecognizerTapped (object sender, EventArgs e)
		{
			Navigation.PushAsync (new SignUpPage (), true);
			Navigation.RemovePage (this);
		}

		async void OnCountryGestureRecognizerTapped (object sender, EventArgs e)
		{
			if (!isCountryTapped) {
				isCountryTapped = true;
				await Navigation.PushAsync (new SelectCountryPage (), true);
			}
		}

		/// <param name="propertyName">The name of the property that changed.</param>
		/// <summary>
		/// Call this method from a child class to notify that a change happened on a property.
		/// </summary>
		protected override void OnPropertyChanged (string propertyName = null)
		{
			base.OnPropertyChanged (propertyName);
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
		}

		#endregion
	}
}

