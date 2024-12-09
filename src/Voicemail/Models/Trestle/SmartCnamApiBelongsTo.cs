// SPDX-License-Identifier: MIT
// SPDX-FileCopyrightText: 2024 Daniel Lo Nigro <d@d.sb>

namespace Voicemail.Models.Trestle;

/// <summary>
/// <c>belongs_to</c> field in Trestle Smart CNAM API response.
/// </summary>
public readonly record struct SmartCnamApiBelongsTo(
	string Id,
	string? Name
);
