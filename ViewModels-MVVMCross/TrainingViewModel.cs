using System;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace PAR.Core
{
	public class TrainingViewModel : BaseViewModel
	{
		/// <summary>
        /// Property to change ok button text language text.
        /// </summary>
		private string _okButtonText;
		public string OkButtonText
		{
			get { return _okButtonText; }
			set { SetProperty(ref _okButtonText, value); }
		}

        /// <summary>
        /// The page header
        /// </summary>
        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
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
						ShowViewModel<ConnectingViewModel>();
					});
				}
				return _okCommand;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PAR.Core.TrainingViewModel"/> class.
        /// </summary>
        public TrainingViewModel()
        {
            InitializeLanguage();
        }

		private void InitializeLanguage()
		{
			base.GetAppLanguage();
            Header = LanguageEntries.GetValue("TrainingScreenTitle");
            OkButtonText = LanguageEntries.GetValue("OkButtonText");
		}
	}
}
