# Migrating from Xamarin to .NET 6 Mobile targets

- Make sure to install a Visual Studio 2022 versin compatible with .NET 6 Mobile targets.
- Make sure to run `uno-check --pre`.

## Migrate the iOS Head
Publication directives: https://github.com/dotnet/maui/issues/4397

## Migrate the Android Head

Publication directives: https://github.com/dotnet/maui/issues/4377

## Use the Catalyst Head

## Migrate the macOS Head
Publication directives: https://github.com/dotnet/maui/issues/5399

## Migrate libraries
- Uno 4.2.6 or later must be used
- Add the following new targets: 
    ```xml
	<TargetFrameworks>$(TargetFrameworks);net6.0-ios;net6.0-macos;net6.0-android;net6.0-maccatalyst</TargetFrameworks>
    ```
- Update msbuild.sdk.extras to 3.0.44 or later

Running on CI:
- Visual Studio 2022 is required (Azure Devop's `windows-2022` image)
- When building with .NET 6 Mobile Preview 14, a global.json file must be created/modified to add the following:
    ```json
    {
        "sdk": {
            "allowPrerelease": true
        }
    }
     ```
- uno-check must be used to install the appropriate workloads:
    ```shell
      & dotnet tool update --global uno.check --version 1.2.0-dev.10 --add-source https://api.nuget.org/v3/index.json
      & uno-check -v --ci --non-interactive --fix --skip xcode --skip gtk3 --skip vswin --skip vsmac --manifest https://raw.githubusercontent.com/unoplatform/uno.check/d14571a546b55f58e51e392c04cf098168d6fe2d/manifests/uno.ui-preview.manifest.json
    ```
- 

## Apple troubleshooting

### errSecInternalComponent
When signing on the CLI, you may get this error message `errSecInternalComponent`. This fix this:
```
security unlock-keychain login.keychain
```