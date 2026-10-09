namespace RevitAi.UI.Services;

public interface INavigationService
{
	bool CanGoBack { get; }

	void NavigateTo<TViewModel>() where TViewModel : class;

	void NavigateTo<TViewModel>(object parameter) where TViewModel : class;

	bool GoBack();
}
