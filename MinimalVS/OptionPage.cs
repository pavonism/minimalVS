using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace MinimalVS
{
	public class OptionPage : DialogPage, INotifyPropertyChanged
	{
		private HideVariant _hideVariant = HideVariant.HideMenuWithToolbar;
        [DisplayName("Hide variant")]
        [Description("Select the hide variant you want from the list.")]
        [DefaultValue(HideVariant.HideMenuWithToolbar)]
        [TypeConverter(typeof(EnumConverter))]
        public HideVariant HideVariant
		{
			get => _hideVariant;

            set
			{
                if (_hideVariant == value)
                {
                    return;
                }

                _hideVariant = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HideVariant)));
            }
		}

        private bool _hideFeedback = true;
        [DisplayName("Hide feedback button")]
		[Description("Hide Feedback button panel")]
		public bool HideFeedback
		{
            get => _hideFeedback;

            set
            {
                if (_hideFeedback == value)
                {
                    return;
                }

                _hideFeedback = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HideFeedback)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
