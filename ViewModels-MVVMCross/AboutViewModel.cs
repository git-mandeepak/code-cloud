﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PAR.Mobile;

namespace PAR.Core
{
    public class AboutViewModel : BaseViewModel
    {
        public event Action OnOkCommand;

        private string _parVersion;
        public string ParVersion
        {
            get { return _parVersion; }
            set { SetProperty(ref _parVersion, value); }
        }

        private string _aboutTitle;
        public string AboutTitle
        {
            get { return _aboutTitle; }
            set { SetProperty(ref _aboutTitle, value); }
        }

        public string AboutSummary
        {
            get { return Constants.AboutSummary; }
        }

        public string CustomerServiceText
        {
            get { return Constants.CustomerServiceText; }
        }

        public string CustomerServiceNumber
        {
            get { return Constants.CustomerServiceNumber; }
        }

        public string UrlText
        {
            get { return Constants.UrlText; }
        }

        public string UrlLink
        {
            get { return Constants.UrlLink; }
        }

        public string VersionText
        {
            get { return Constants.VersionText; }
        }

        /// <summary>
        /// Confirm button command.
        /// </summary>
        private ICommand _okCommand;
        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new MvxCommand(() =>
                    {
						NavigateToDashboard();
                    });
                }
                return _okCommand;
            }
        }

		private ICommand _navigationCommand;
		public ICommand NavigationCommand
		{
			get
			{
				if (_navigationCommand == null)
				{
					_navigationCommand = new MvxCommand(() =>
					{
                        ShowViewModel<MenuViewModel>();
					});
				}
				return _navigationCommand;
			}
		}


        /// <summary>
        /// Navigates to dashboard.
        /// </summary>
        private void NavigateToDashboard()
        {
            var platform = Ioc.Container.Resolve<IEnvironment>().GetPlatform();

            switch(platform)
            {
                case EnvironmentPlatform.Android:
                    OnOkCommand.Invoke();
                    break;
                case EnvironmentPlatform.iOS:
                    ShowViewModel<DashboardViewModel>();
                    break;
            }
        }

        private ICommand _launchAppUrlCommand;
        public ICommand LaunchAppUrlCommand
        {
            get
            {
                if (_launchAppUrlCommand == null)
                {
                    _launchAppUrlCommand = new MvxCommand(() =>
                    {
                        var param = new Dictionary<string, string>()
                        {
                            {Constants.WEB_URL, UrlLink}
                        };

                        ShowViewModel<PageUrlViewModel>(param);
                    });
                }

                return _launchAppUrlCommand;
            }
        }

        public AboutViewModel()
        {
            _parVersion = Ioc.Container.Resolve<IEnvironment>().GetParVersion();
        }
		public void ShowMenu()
		{
			ShowViewModel<MenuViewModel>();
		}
    }
}
