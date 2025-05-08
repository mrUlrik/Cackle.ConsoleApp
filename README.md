# Cackle Console Extensions

This repository contains a set of libraries designed to enhance the development of console applications using [Spectre.Console](https://spectreconsole.net/) in .NET.

## Libraries

* **`Cackle.ConsoleApp`**: Provides a foundational layer that integrates Spectre.Console with modern .NET features like Microsoft Dependency Injection (`IServiceCollection`), Configuration (`IConfiguration`), and Options (`IOptions<T>`). It also introduces an `AsyncCommand` base class with built-in `CancellationToken` and escape sequence handling.

* **`Cackle.ConsoleApp.Extensions.Logging.SpectreConsole`**: An extension library that implements a logging provider for [Microsoft.Extensions.Logging](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging), leveraging Spectre.Console for styled and informative console log output.

## Getting Started

Refer to the individual README files within each library's directory for detailed installation and usage instructions.

## License

This repository is licensed under the [MIT License](LICENSE).