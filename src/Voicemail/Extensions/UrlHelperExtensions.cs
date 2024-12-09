using Microsoft.AspNetCore.Mvc;

namespace Voicemail;

/// <summary>
/// Extensions to <see cref="IUrlHelper"/>.
/// </summary>
public static class UrlHelperExtensions
{
	/// <summary>
	/// Builds an absolute URL for a controller action
	/// </summary>
	public static string ActionAbsolute(this IUrlHelper urlHelper, string controller, string action)
	{
		var url = urlHelper.Action(
			action: action,
			controller: controller,
			values: null,
			protocol: urlHelper.ActionContext.HttpContext.Request.Scheme
		);
		if (url is null)
		{
			throw new ArgumentNullException(nameof(action));
		}
		return url;
	}
}