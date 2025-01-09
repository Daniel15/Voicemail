# Voicemail

Basic voicemail system, *powered by AI* because it's the cool thing to do these days. It is **currently under construction** and isn't ready for usage yet.

This project uses the following third-party providers:

|Provider|Use|Approx. cost (as of December 2024)|
|--|--|--|
|Twilio|Incoming phone calls|\$1.15/month plus \$0.0085/min|
|AssemblyAI|AI analysis of voicemail messages - Transcription, summarization, entity extraction|\$0.008/minute / \$0.48/hour (\$0.37 for speech-to-text, \$0.08 for entity detection, \$0.03 for summarization). Free \$50 credit will last a long time.|
|TresleIQ|Caller ID ("Smart CNAM API"). I tried several caller ID APIs and this was the most accurate one that offers as-you-go pricing.|$0.015/query

# Features

TBD

# Installation

## Docker

TBD

## Manually

A binary of the latest build can be downloaded from [TBD]

## From source
See "Development" section below.

# Configuration

Copy `appsettings.Production.json.example` and change settings to suit your environment.

# Development

1. Install [.NET 9.0](https://learn.microsoft.com/en-us/dotnet/core/install/linux)
2. Clone the repo
3. Set access tokens/keys in .NET Secrets, to avoid accidentally commiting them to the repo:
```
# Twilio
dotnet user-secrets set 'Twilio:AuthToken' '.....'
dotnet user-secrets set 'Twilio:Client:AccountSid' '.....'
dotnet user-secrets set 'Twilio:Client:ApiKeySid' '.....'
dotnet user-secrets set 'Twilio:Client:ApiKeySecret' '.....'

# AssemblyAI
dotnet user-secrets set 'AssemblyAI:ApiKey' '....'

# Trestle
dotnet user-secrets set 'Trestle:ApiKey' '....'
```
4. Run it using `dotnet run` or with your favourite editor (e.g. Rider or VS Code).

To build the release version:
```shell
dotnet publish -c Release -r linux-x64
```
(`linux-x64` can be replaced with [any architecture supported by .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet9plus#platformarchitecture-restrictions)), such as `linux-arm64`, `windows-x64`, etc)

It will build into the `src/Voicemail/bin/Release/net9.0/linux-x64/publish` folder. The release version is built as a self-contained executable and does not require .NET to be installed.
