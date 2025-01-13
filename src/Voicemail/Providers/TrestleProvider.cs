// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using PhoneNumbers;
using Voicemail.Configuration;
using Voicemail.Extensions;
using Voicemail.Models;
using Voicemail.Models.Trestle;

namespace Voicemail.Providers;

/// <summary>
/// Handles caller ID lookups using Trestle's API
/// </summary>
/// <see href="https://trestle-api.redoc.ly/Current/" />
public class TrestleProvider(IHttpClientFactory _factory, IOptionsMonitor<TrestleConfig> _options)
	: ICallerIdProvider, IThirdPartyApi
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
	};
	
	/// <inheritdoc />
	public async Task<CallerId?> GetCallerId(PhoneNumber phoneNumber)
	{
		// https://trestle-api.redoc.ly/Current/tag/Smart-CNAM-API#operation/getCNAM
		var client = CreateClient();
		var uri = QueryHelpers.AddQueryString("cnam", new Dictionary<string, string?>
		{
			{ "phone", phoneNumber.ToE164() }
		});
		var response = await client.GetFromJsonAsync<SmartCnamApiResponse>(
			uri, 
			_jsonSerializerOptions
		);
		return response is { IsValid: true, BelongsTo: not null }
			? new CallerId { CallerName = response.BelongsTo.Value.Name }
			: null;
	}

	private HttpClient CreateClient()
	{
		var client = _factory.CreateClient();
		client.BaseAddress = new Uri("https://api.trestleiq.com/3.1/");
		client.DefaultRequestHeaders.Add("x-api-key", _options.CurrentValue.ApiKey);
		return client;
	}

	/// <inheritdoc />
	public Task EnsureApiIsFunctional()
	{
		// Trestle don't seem to have any non-billed APIs, so checking for the API key is the
		// best we can do for now.
		if (string.IsNullOrWhiteSpace(_options.CurrentValue.ApiKey))
		{
			throw new Exception("Trestle API key is missing.");
		}
		return Task.CompletedTask;
	}
}
