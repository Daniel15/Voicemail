// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

namespace Voicemail.Models.Trestle;

/// <summary>
/// Response to Trestle Smart CNAM API response.
/// </summary>
public readonly record struct SmartCnamApiResponse(
	bool IsValid,
	SmartCnamApiBelongsTo? BelongsTo
);
