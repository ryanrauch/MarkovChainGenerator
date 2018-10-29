using MarkovChainGenerator.ViewModels;
using Xamarin.Forms.Xaml;

namespace MarkovChainGenerator.Views.ContentPages
{
    public class TwitterSearchUsersViewBase : ViewPageBase<TwitterSearchUsersViewModel> { }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TwitterSearchUsersView : TwitterSearchUsersViewBase
	{
		public TwitterSearchUsersView ()
		{
			InitializeComponent ();
		}
	}
}