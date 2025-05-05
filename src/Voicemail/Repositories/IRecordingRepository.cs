// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2025 Daniel Lo Nigro <d@d.sb>

using Voicemail.Models;

namespace Voicemail.Repositories;

public interface IRecordingRepository
{
	/// <summary>
	/// Saves the recording for the specified call.
	/// </summary>
	public Task Save(Call call, Stream stream);

	/// <summary>
	/// Gets the recording for this call, if any.
	/// </summary>
	public Stream? Get(Call call);
}
