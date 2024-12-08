
## Development 

Set access tokens/keys in .NET Secrets:
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
