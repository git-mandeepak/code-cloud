#region Usings
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.ComponentModel;
using Plugin.Media;
using System.Windows.Input;
using Plugin.Connectivity;
using System.Linq;
using System.IO;

#endregion

namespace ADVPlatform
{
	public partial class UserProfilePage : ContentPage
	{
		#region Private Variables

		new event PropertyChangedEventHandler PropertyChanged;

		IContactService contacts;
		List<Contacts> contactsList;

		User user;

		private bool descriptionPlaceHolderVisibility;
		private bool optionalPlaceHolderVisibility;
		private bool isLoading;
		private bool isImageLoading;

		private string profilePicture;
		private int gender;
		private string location;
		private string description;
		string lat = string.Empty;
		string lng = string.Empty;

		private string dobformat;
		private DateTime dobmaximumdate;
		private DateTime dobminimumdate;

		private string filepath = string.Empty;
		private string filename = string.Empty;
		private readonly string SavedUserImageName = Constants.AvatarImage;

		private bool imageCroped = false;
		private bool profilePhotoValidation = false;

		#endregion

		#region Public Properties

		public bool OptionalPlaceHolderVisibility {
			get {
				return optionalPlaceHolderVisibility;
			}
			set {
				optionalPlaceHolderVisibility = value;
				OnPropertyChanged ("OptionalPlaceHolderVisibility");
			}
		}

		public bool DescriptionPlaceHolderVisibility {
			get {
				return descriptionPlaceHolderVisibility;
			}
			set {
				descriptionPlaceHolderVisibility = value;
				OnPropertyChanged ("DescriptionPlaceHolderVisibility");
			}
		}

		public string ProfilePicture {
			get {
				return profilePicture;
			}
			set {
				profilePicture = value;
				OnPropertyChanged ("ProfilePicture");
			}
		}

		public string DOBFormat {
			get {
				return dobformat;
			}
			set {
				dobformat = value;
				OnPropertyChanged ("DOBFormat");
			}
		}

		public DateTime DOBMaximumDate {
			get {
				return dobmaximumdate;
			}
			set {
				dobmaximumdate = value;
				OnPropertyChanged ("DOBMaximumDate");
			}
		}

		public DateTime DOBMinimumDate {
			get {
				return dobminimumdate;
			}
			set {
				dobminimumdate = value;
				OnPropertyChanged ("DOBMinimumDate");
			}
		}

		public int Gender {
			get {
				return gender;
			}
			set {
				gender = value;
				OnPropertyChanged ("Gender");
			}
		}

		public string Location {
			get {
				return location;
			}
			set {
				location = value;
				OnPropertyChanged ("Location");
			}
		}

		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				OnPropertyChanged ("Description");
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

		public bool IsImageLoading {
			get {
				return isImageLoading;
			}
			set {
				isImageLoading = value;
				OnPropertyChanged ("IsImageLoading");
			}
		}

		#endregion

		#region Commands

		public ICommand LogoutCommand{ get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="ADVPlatform.UserProfilePage"/> class.
		/// </summary>
		public UserProfilePage (User user, Enums.FromPage fromPage)
		{
			InitializeComponent ();
			IsLoading = true;
			this.user = user;
			setDefaultValues ();
			if (this.user != null && fromPage == Enums.FromPage.LoginPage) {
				BindUserDetail ();
			}
			BindingContext = this;
			IsLoading = false;
		}

		#endregion

		#region Private Methods

		void BindUserDetail ()
		{
			if (this.user.dob != null)
				pkDOB.Date = Convert.ToDateTime (this.user.dob);
			if (Enum.IsDefined (typeof(Enums.Gender), this.user.gender_type)) {
				Gender = Convert.ToInt32 (this.user.gender_type);
				lblGender.IsVisible = false;
			} else {
				lblGender.IsVisible = true;
			}
			if (this.user.location != null)
				App.LocationName = this.user.location;
			if (this.user.about_me != null)
				Description = this.user.about_me;
			lat = this.user.latitude;
			lng = this.user.longitude;
			filename = this.user.profile_pic;
			GetFileFromS3Bucket (this.user.profile_pic);
		}

		/// <summary>
		/// Sets the default values.
		/// </summary>
		void setDefaultValues ()
		{
			btnSave.Text = Constants.SaveContinue;
			frmOverLay.BackgroundColor = new Color (0, 0, 0, 0.5);
			frmImageOverLay.BackgroundColor = new Color (0, 0, 0, 0.5);
			DOBFormat = Constants.DOBDateFormat;
			DOBMaximumDate = DateTime.Now;
			DOBMinimumDate = DateTime.Now.AddYears (Constants.DOBMinimumDate);
			pkDOB.Date = Constants.DOBDefaultDate;
			gender = Constants.GenderDefault;
			BindGenderPicker ();
			NavigationPage.SetHasNavigationBar (this, false);
			txtDescription.Focused += TxtDescriptionFocussed;
			txtDescription.Unfocused += TxtDescriptionUnfocussed;
			txtLocation.Focused += TxtLocationFocussed;
			txtDescription.TextChanged += (sender, e) => {
				DescriptionPlaceHolderVisibility = OptionalPlaceHolderVisibility = string.IsNullOrEmpty (e.NewTextValue);
			};

			profilePicture = Constants.DefaultProfilePicture;
			descriptionPlaceHolderVisibility = optionalPlaceHolderVisibility = true;

			LogoutCommand = new Command (LogOut);

			if(Device.OS == TargetPlatform.iOS)				
				RequestPermission();
		}

		void RequestPermission ()
		{
			contacts = DependencyService.Get<IContactService> ();
			contacts.RequestPermission();
		}

		async void GetContacts ()
		{
			contactsList = new List<Contacts> ();
			contactsList = await contacts.GetContacts ();

			if (contactsList != null && contactsList.Count > 0)
			{
				var result = await Common.SyncContacts (contactsList);
			}
		}

		/// <summary>
		/// Logs the out.
		/// </summary>
		async void LogOut ()
		{
			var userDetail = await new UserManager ().Logout ();

			if (userDetail.response == null) {
				if (userDetail.status_code == Constants.ServerNotRunningStatus)
					await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.ServerNotRunningMessage, Constants.AlertCancelButtonDefaultText);
				else {
					await DisplayAlert (Constants.AlertTitleDefaultText, Constants.MobileNumberNotValid, Constants.AlertCancelButtonDefaultText);
				}
			} else {	
				Common.DoLogout ();
				await Navigation.PushAsync (new LandingPage (), true);
				Navigation.RemovePage (this);
			}		
		}

		void BindGenderPicker ()
		{
			var genderItems = Enum.GetValues (typeof(Enums.Gender)).Cast<Enums.Gender> ();
			foreach (var item in genderItems)
				pkGender.Items.Add (item.ToString ());
		}

		bool Validate ()
		{
			bool isValid = true;

			if (pkDOB.Date > DOBMaximumDate || pkDOB.Date < DOBMinimumDate) {
				isValid = false;
				DisplayAlert (Constants.AlertTitleDefaultText, Constants.EnterValidDate, Constants.AlertCancelButtonDefaultText);
			} else if (pkGender.SelectedIndex == -1) {
				isValid = false;
				DisplayAlert (Constants.AlertTitleDefaultText, Constants.GenderRequired, Constants.AlertCancelButtonDefaultText);
			} else if (string.IsNullOrEmpty (Location)) {
				isValid = false;
				DisplayAlert (Constants.AlertTitleDefaultText, Constants.LocationRequired, Constants.AlertCancelButtonDefaultText);
			} else if (!profilePhotoValidation) {
				isValid = false;
				DisplayAlert (Constants.AlertTitleDefaultText, Constants.ProfilePhotoRequired, Constants.AlertCancelButtonDefaultText);
			}

			return isValid;
		}

		/// <summary>
		/// Uploads user image to s3 bucket.
		/// </summary>
		/// <param name="filepath">Filepath.</param>
		async void GetFileFromS3Bucket (string filename)
		{
			if (!string.IsNullOrEmpty (filename)) {
				profilePhotoValidation = true;
				IsImageLoading = true;
				var shared = DependencyService.Get<ISharedService> ();
				var response = await shared.DownloadFile (filename);
				if (response == true) {
					imgProfileImage.Source = ImageSource.FromFile (System.IO.Path.Combine (App.PersonalFolderPath, filename));
				}
				IsImageLoading = false;
			}
		}

		#endregion

		#region Event

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (!CrossConnectivity.Current.IsConnected) {
				DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.NoInternetMessage, Constants.AlertDoneButtonDefaultText);
			}

			Location = App.LocationName;
		}

		/// <summary>
		/// Texts the description focussed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void TxtDescriptionFocussed (object sender, FocusEventArgs e)
		{
			DescriptionPlaceHolderVisibility = OptionalPlaceHolderVisibility = false;
		}

		/// <summary>
		/// Texts the description unfocussed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void TxtDescriptionUnfocussed (object sender, FocusEventArgs e)
		{
			if (!string.IsNullOrEmpty (Description))
				DescriptionPlaceHolderVisibility = OptionalPlaceHolderVisibility = false;
			else
				DescriptionPlaceHolderVisibility = OptionalPlaceHolderVisibility = true;
		}

		/// <summary>
		/// Texts the location focussed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void TxtLocationFocussed (object sender, FocusEventArgs e)
		{
			Navigation.PushAsync (new SetLocationPage (Enums.FromPage.UserProfilePage), true);
		}

		/// <summary>
		/// Raises the back button gesture recognizer tapped event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void OnBackButtonGestureRecognizerTapped (object sender, EventArgs e)
		{
			Navigation.PopAsync ();
		}

		/// <summary>
		/// Raises the image gesture recognizer tapped event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void OnImageGestureRecognizerTapped (object sender, EventArgs e)
		{
			var action = await DisplayActionSheet (Constants.ActionSheetTitle, Constants.AlertCancelButtonDefaultText, null, Constants.TakePicture, Constants.CameraRoll);

			if (action == Constants.TakePicture) {
				TakePicture ();
			} else if (action == Constants.CameraRoll) {
				var file = await CrossMedia.Current.PickPhotoAsync ();
				if (file != null) {
					//filepath = file.Path;
					imgProfileImage.Source = ImageSource.FromStream (() => {
						var stream = file.GetStream ();

						// RESIZE IMAGE
						byte[] imageData;
						using (MemoryStream ms = new MemoryStream ()) {
							stream.CopyTo (ms);
							imageData = ms.ToArray ();
						}

						var shared = DependencyService.Get<ISharedService> ();
						byte[] resizedImage = shared.ResizeImage (imageData, 200, 200);
										
						file.Dispose ();
						stream.Dispose();
						imageData = null;
						shared= null;
						profilePhotoValidation = true;
						return new MemoryStream (resizedImage);
					});
					imageCroped = true;

				}
			}
		}

		/// <summary>
		/// Takes the picture.
		/// </summary>
		async void TakePicture ()
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported) {
				await DisplayAlert (Constants.NoCameraTitle, Constants.NoCameraAvailableMsg, Constants.AlertDoneButtonDefaultText);
				return;
			}

			var file = await CrossMedia.Current.TakePhotoAsync (new Plugin.Media.Abstractions.StoreCameraMediaOptions ());

			if (file == null)
				return;

			imgProfileImage.Source = ImageSource.FromStream (() => {
				var stream = file.GetStream ();

				// RESIZE IMAGE
				byte[] imageData;
				using (MemoryStream ms = new MemoryStream ()) {
					stream.CopyTo (ms);
					imageData = ms.ToArray ();
				}

				var shared = DependencyService.Get<ISharedService> ();
				byte[] resizedImage = shared.ResizeImage (imageData, 200, 200);

				file.Dispose ();
				stream.Dispose();
				imageData = null;
				shared= null;
				profilePhotoValidation = true;
				return new MemoryStream (resizedImage);
			});
			imageCroped = true;
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

		/// <summary>
		/// Save profile details of user.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void BtnSaveClicked (object sender, EventArgs e)
		{
			if (!CrossConnectivity.Current.IsConnected) {
				IsLoading = false;
				await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.NoInternetMessage, Constants.AlertDoneButtonDefaultText);
			} else if (Validate ()) {				
				IsLoading = true;
				if (!string.IsNullOrEmpty (App.LocationPlaceID)) {
					// get lat and long from google
					var wrapper = await new PlacesManager ().GetPlaceDetail (App.LocationPlaceID);
					if (wrapper.result != null) {
						lat = Convert.ToString (wrapper.result.geometry.location.lat);
						lng = Convert.ToString (wrapper.result.geometry.location.lng);
					}
				}

				// Upload to S3 bucket.
				//UploadToS3Bucket (filepath);
				if (imageCroped) {
					imageCroped = false;
					filepath = System.IO.Path.Combine (App.PersonalFolderPath, SavedUserImageName);
					if (!string.IsNullOrEmpty (filepath)) {		
						filename = Guid.NewGuid () + Path.GetExtension (filepath);
						var shared = DependencyService.Get<ISharedService> ();
						var response = await shared.UploadFile (filepath, filename);
						if (response != null) {

						}
					}
				}

				// Save Profile
				UserProfile userProfile = new UserProfile ();
				userProfile.gender_type = Gender.ToString ();
				userProfile.dob = pkDOB.Date.ToString (Constants.DateFormatForService);
				userProfile.about_me = Description;
				userProfile.location = Location;
				userProfile.latitude = lat;
				userProfile.longitude = lng;
				if (!string.IsNullOrEmpty (filename))
					userProfile.profile_pic = filename;
				var userDetail = await new UserManager ().CreateProfile (userProfile);
				if (userDetail.response == null && userDetail.status_code == Constants.ServerNotRunningStatus) {
					if (userDetail.status_code == Constants.ServerNotRunningStatus) {
						await DisplayAlert (Constants.AlertErrorTitleDefaultText, Constants.ServerNotRunningMessage, Constants.AlertCancelButtonDefaultText);
					} else {
						await DisplayAlert (Constants.AlertTitleDefaultText, Constants.AlertAnErrorOccurred, Constants.AlertDoneButtonDefaultText);
					}
				} else {
					if (userDetail.status_code == Constants.SuccessStatus) {
						await DisplayAlert (Constants.AlertTitleSuccessText, Constants.ProfileSaved, Constants.AlertDoneButtonDefaultText);
						
						await Navigation.PushAsync (new HomePage (Enums.FromPage.LoginPage, (Enums.UserStage)userDetail.response.user.stage), true); 

						GetContacts ();

					} else {
						await DisplayAlert (Constants.AlertTitleDefaultText, Constants.AlertAnErrorOccurred, Constants.AlertDoneButtonDefaultText);
					}
				}
				filepath = string.Empty;				
			}

			IsLoading = false;
		}

		/// <summary>
		/// Pks the gender picker selected index changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void PkGenderPicker_SelectedIndexChanged (object sender, EventArgs e)
		{
			if (pkGender.SelectedIndex != -1) {
				lblGender.IsVisible = false;
				Gender = pkGender.SelectedIndex;
			} else {
				lblGender.IsVisible = true;
			}
		}

		#endregion
	}
}

