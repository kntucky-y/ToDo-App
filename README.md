# To Do App

A simple To Do mobile app built with **.NET 9 MAUI**, targeting Android.

## Features

- **Sign In / Sign Up** — basic user authentication stored locally
- **To Do Tab** — add, edit, delete, and complete tasks
- **Completed Tab** — view completed tasks, undo or delete them
- **Profile Tab** — view current user info and sign out

## Tech Stack

- .NET 9 MAUI (Android)
- MVVM pattern — `AppViewModel` singleton
- `ListView` with `INotifyPropertyChanged` model (`ToDoClass`)
- Shell navigation with tab bar and page routes

## Project Structure

```
ToDoApp/
├── Models/         # ToDoClass, UserModel
├── Pages/          # All UI pages (XAML)
├── ViewModels/     # AppViewModel
├── Services/       # AppData (in-memory data store)
├── Helpers/        # NavigationContext, InvertBoolConverter
└── Resources/      # Styles, colors, icons
```

## How to Run

1. Open `ToDoApp.csproj` in Visual Studio or Rider
2. Start an Android emulator (Pixel 8 recommended)
3. Select `net9.0-android` as the target framework
4. Press Run
