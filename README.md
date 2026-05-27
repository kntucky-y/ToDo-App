# To Do App

A cross-platform To Do app built with .NET 9 MAUI. It uses a remote PHP API for
authentication and task CRUD operations.

## Features

- Sign in and sign up via the remote API
- Create, edit, delete, and complete tasks
- Completed tasks view with undo support
- Profile page with user info and sign out

## Tech Stack

- .NET 9 MAUI (Android, iOS, MacCatalyst, Windows)
- MVVM-style state via `AppViewModel` singleton
- Shell navigation with tab routes
- Remote API service in `ToDoApiService`

## API

Base URL: https://todo-list.dcism.org/

The app calls these endpoints:

- `signin_action.php` (GET)
- `signup_action.php` (POST)
- `getItems_action.php` (GET)
- `addItem_action.php` (POST)
- `editItem_action.php` (PUT)
- `statusItem_action.php` (PUT)
- `deleteItem_action.php` (DELETE, with GET fallback)

## Project Structure

The active MAUI project is in the nested folder below. Root-level XAML/C# files
are older duplicates and should be ignored for app changes.

```
ToDoApp/
├── Models/         # ToDoClass, UserModel
├── Pages/          # XAML pages and code-behind
├── ViewModels/     # AppViewModel
├── Services/       # AppData, ToDoApiService
├── Helpers/        # NavigationContext
├── Resources/      # Styles, icons, images, fonts
└── Platforms/      # Platform-specific entry points
```

## How to Run

1. Open the solution in Visual Studio 2022 (or later) with the MAUI workload.
2. Select a target framework, for example `net9.0-android` or `net9.0-windows10.0.19041.0`.
3. Start an emulator or pick a Windows target.
4. Run the app.

## License

MIT. See LICENSE.
