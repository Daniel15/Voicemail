// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Voicemail.Models;

namespace Voicemail.Repositories;

/// <summary>
/// Implementation of <see cref="IRecordingRepository"/> that stores recordings locally.
/// </summary>
public class LocalRecordingRepository(IHttpClientFactory _httpClientFactory) : IRecordingRepository
{
	/// <inheritdoc />
	public async Task Save(Call call, Stream stream)
	{
		await using var fileStream = File.OpenWrite(BuildRecordingPath(call));
		await stream.CopyToAsync(fileStream);
	}

	/// <inheritdoc />
	public Stream? Get(Call call)
	{
		var filePath = BuildRecordingPath(call);
		return File.Exists(filePath) ? File.OpenRead(filePath) : null;
	}

	private static string BuildRecordingPath(Call call) => 
		Path.Combine(VoicemailContext.DataPath, "recordings", $"{call.Id}.mp3");
}
