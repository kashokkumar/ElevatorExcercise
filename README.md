# ElevatorExcercise

ElevatorExcercise is a Razor Pages web application that demonstrates elevator control logic and a simple simulation UI. The project targets .NET 8 and is intended as a learning / demo project for elevator state management and scheduling strategies.

## Tech stack
- .NET 8
- C# (Razor Pages)
- ASP.NET Core Razor Pages
- Dependency Injection (built-in ASP.NET Core DI)
- Unit testing: xUnit (if tests present)
- Front-end: Razor + plain CSS/JS (in `wwwroot/`)

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022/2029 or later with ASP.NET and web development workload (or VS Code)
- Git

## Setup
1. Clone the repository:

```bash
git clone https://github.com/kashokkumar/ElevatorExcercise.git
cd ElevatorExcercise
```

2. Restore and build:

```bash
dotnet restore
dotnet build
```

## Running the app

### Visual Studio
1. Open the solution in Visual Studio.
2. Set the Razor Pages project (startup project) and run (F5 or Ctrl+F5).

### .NET CLI

```bash
dotnet run --project ElevatorOne.csproj
```

Open the URL shown in the console (typically `https://localhost:5001`).

## Project structure (selected)
- `Pages/` - Razor Pages (UI)
- `wwwroot/` - static assets (JS/CSS)
- `Services/` - elevator logic and scheduling services
- `Program.cs` - app startup and DI
- `Properties/launchSettings.json` - local launch profiles
- `Tests/` - unit tests (if present)

<img width="728" height="708" alt="image" src="https://github.com/user-attachments/assets/37f52681-7b53-435c-bddc-cbc227b5e64d" />


## Contributing
Contributions are welcome. Please follow repository conventions and open issues or pull requests against `main`.

## Coding standards
Follow `.editorconfig` (if present) and keep code targeting .NET 8. Write small, testable services and prefer dependency injection for services used by pages.

## License
Add a `LICENSE` file to state the project license (e.g., MIT).

## Contact
For issues or questions, open an issue on the repository: https://github.com/kashokkumar/ElevatorExcercise
