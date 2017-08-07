using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Collections.Generic;

namespace PAR.Core
{
	public class TermViewModel: BaseViewModel
	{
		private ISettingService _settingService;

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private string _acceptButtonText;
        public string AcceptButtonText
        {
            get { return _acceptButtonText; }
            set { SetProperty(ref _acceptButtonText, value); }
        }

		/// <summary>
		/// Gets the URL for terms and condition
		/// </summary>
		/// <value>The URL.</value>
		public string URL
		{
			get
			{
				return Constants.URLTermsAndConditions;
			}
		}

		/// <summary>
		/// Represents the View Content.
		/// </summary>
		/// <value>The content.</value>
		public string Content
		{
			get
			{
				return Constants.TermsScreenContent;
			}
		}

        /// <summary>
        /// The Terms Accepted Check.
        /// </summary>
        private string _accepted;
        public string Accepted
        {
            get { return _accepted; }
            set { SetProperty(ref _accepted, value); }
        }

		/// <summary>
		/// Represents the Check box check or not.
		/// </summary>
		private bool _hasChecked;
		public bool HasChecked
		{
			get
			{
				return _hasChecked;
			}
			set
			{
				SetProperty(ref _hasChecked, value);
			}
		}

		/// <summary>
		/// Check box checked command.
		/// </summary>
		private ICommand _acceptedCommand;
		public ICommand AcceptedCommand
		{
			get
			{
				if (_acceptedCommand == null)
				{
					_acceptedCommand = new MvxCommand(() =>
					{
						this.HasChecked = !this.HasChecked;
					});
				}
				return _acceptedCommand;
			}
		}

		/// <summary>
		/// Confirm button command.
		/// </summary>
		private ICommand _confirmCommand;
		public ICommand ConfirmCommand
		{
			get
			{
				if (_confirmCommand == null)
				{
					_confirmCommand = new MvxCommand(() =>
					{
						if (this.HasChecked)
						{
							_settingService.Save(new KeyValuePair<EnumSetting, string>(EnumSetting.TermCondition, "true"));
							ShowViewModel<TaskViewModel>();
                            Close(this);
						}
					});
				}
				return _confirmCommand;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PAR.Core.TermViewModel"/> class.
		/// </summary>
		/// <param name="settingService">Setting service.</param>
		public TermViewModel(ISettingService settingService)
		{
			this._settingService = settingService;

            InitializeLanguage();
		}

		private void InitializeLanguage()
		{
            base.GetAppLanguage();
            Header = LanguageEntries.GetValue("TermsScreenTitle");
            Accepted = LanguageEntries.GetValue("TermScreenAgreed");
            AcceptButtonText = LanguageEntries.GetValue("AcceptButtonText");
		}
	}
}
