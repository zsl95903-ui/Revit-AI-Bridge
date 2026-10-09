using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace RevitAi.UI.Services;

public class NavigationService : INavigationService
{
	private readonly Stack<object> _navigationStack;

	private readonly IServiceProvider _serviceProvider;

	public bool CanGoBack => _navigationStack.Count > 1;

	public NavigationService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_navigationStack = new Stack<object>();
	}

	public void NavigateTo<TViewModel>() where TViewModel : class
	{
		NavigateTo<TViewModel>(null);
	}

	public void NavigateTo<TViewModel>(object parameter) where TViewModel : class
	{
		if (_serviceProvider.GetService<TViewModel>() == null)
		{
			throw new InvalidOperationException("无法创建 ViewModel: " + typeof(TViewModel).Name);
		}
		_navigationStack.Push(typeof(TViewModel));
	}

	public bool GoBack()
	{
		if (_navigationStack.Count > 1)
		{
			_navigationStack.Pop();
			_navigationStack.Peek();
			return true;
		}
		return false;
	}
}
