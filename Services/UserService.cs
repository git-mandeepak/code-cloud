#region Usings
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

#endregion

namespace ADVPlatform
{
	public class UserService : IUserService
	{
		#region Private Variables

		readonly HttpClient client;

		#endregion

		#region Constructor

		public UserService ()
		{
			client = new HttpClient ();
			client.MaxResponseContentBufferSize = Constants.MaxResponseContentBufferSize;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Login the specified loginDetails.
		/// </summary>
		/// <param name="loginDetails">Login details.</param>
		public async Task<ResultWrapper<UserResponse>> Login (Login loginDetails)
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.Login));
			ResultWrapper<UserResponse> obj = new ResultWrapper<UserResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (loginDetails);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PostAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<UserResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		/// <summary>
		/// Signs up.
		/// </summary>
		/// <returns>The up.</returns>
		/// <param name="signupDetails">Signup details.</param>
		public async Task<ResultWrapper<UserResponse>> SignUp (SignUp signupDetails)
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.SignUp));
			ResultWrapper<UserResponse> obj = new ResultWrapper<UserResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (signupDetails);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PostAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<UserResponse>> (content);
					if (obj.status_code == Constants.SuccessStatus) {
						SessionManager.AccessToken = obj.response.access_token;
						SessionManager.UserId = obj.response.user.id;
						SessionManager.PhoneNumber = obj.response.user.mobile;
					}
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		/// <summary>
		/// Forgots the password.
		/// </summary>
		/// <returns>The password.</returns>
		/// <param name="forgotPassword">Forgot password.</param>
		public async Task<ResultWrapper<ForgotPasswordResponse>> ForgotPassword (ForgotPassword forgotPassword)
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.ForgotPassword));
			ResultWrapper<ForgotPasswordResponse> obj = new ResultWrapper<ForgotPasswordResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (forgotPassword);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PostAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<ForgotPasswordResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		/// <summary>
		/// Resets the password.
		/// </summary>
		/// <returns>The password.</returns>
		/// <param name="resetPassword">Reset password.</param>
		public async Task<ResultWrapper<ResetPasswordResponse>> ResetPassword (ResetPassword resetPassword)
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.ResetPassword));
			ResultWrapper<ResetPasswordResponse> obj = new ResultWrapper<ResetPasswordResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (resetPassword);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PostAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<ResetPasswordResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		/// <summary>
		/// Logout user.
		/// </summary>
		public async Task<ResultWrapper<LogoutResponse>> Logout ()
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.Logout));
			ResultWrapper<LogoutResponse> obj = new ResultWrapper<LogoutResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (string.Empty);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PutAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<LogoutResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		/// <summary>
		/// Creates user profile.
		/// </summary>
		/// <returns>The profile.</returns>
		/// <param name="userProfile">User profile.</param>
		public async Task<ResultWrapper<UserProfileResponse>> CreateProfile (UserProfile userProfile)
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.CreateProfile));
			ResultWrapper<UserProfileResponse> obj = new ResultWrapper<UserProfileResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				var json = JsonConvert.SerializeObject (userProfile);
				var httpcontent = new StringContent (json, Encoding.UTF8, Constants.JsonContent);
				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PutAsync (uri, httpcontent);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<UserProfileResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		public async Task<ResultWrapper<UserHintsResponse>> UserHints ()
		{
			var uri = new Uri (string.Format ("{0}{1}", Constants.AppApiServiceBaseUrl, Constants.UserHints));
			ResultWrapper<UserHintsResponse> obj = new ResultWrapper<UserHintsResponse> ();
			try {
				await new CommonService ().GetAccessToken ();

				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue (Constants.TokenTypeBearer, SessionManager.AccessToken);
				var response = await client.PostAsync (uri, null);
				if (response.IsSuccessStatusCode) {
					var content = await response.Content.ReadAsStringAsync ();
					obj = JsonConvert.DeserializeObject <ResultWrapper<UserHintsResponse>> (content);
				}
			} catch (Exception ex) {
				Debug.WriteLine (@"ERROR {0}", ex.Message);
			}
			return obj;
		}

		#endregion
	}
}

