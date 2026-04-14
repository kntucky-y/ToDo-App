using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToDoApp.Helpers;
using ToDoApp.Services;

namespace ToDoApp.ViewModels
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public static AppViewModel Instance { get; } = new();

        public ObservableCollection<ToDoClass> PendingItems => AppData.PendingItems;
        public ObservableCollection<ToDoClass> CompletedItems => AppData.CompletedItems;

        public bool IsPendingEmpty => PendingItems.Count == 0;
        public bool IsCompletedEmpty => CompletedItems.Count == 0;

        public ICommand AddCommand { get; }
        public ICommand EditTodoCommand { get; }
        public ICommand DeleteTodoCommand { get; }
        public ICommand CompleteTodoCommand { get; }
        public ICommand EditCompletedCommand { get; }
        public ICommand DeleteCompletedCommand { get; }
        public ICommand UndoCompletedCommand { get; }

        private AppViewModel()
        {
            AddCommand = new Command(async () =>
            {
                try
                {
                    NavigationContext.EditingItem = null;
                    await Shell.Current.GoToAsync("addtodo");
                }
                catch { }
            });

            EditTodoCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    NavigationContext.EditingItem = item;
                    await Shell.Current.GoToAsync("edittodo");
                }
                catch { }
            });

            DeleteTodoCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    bool ok = await Shell.Current.DisplayAlert("Delete", $"Delete \"{item.item_name}\"?", "Delete", "Cancel");
                    if (ok)
                    {
                        var (deleted, message) = await ToDoApiService.DeleteItemAsync(item.item_id);
                        if (!deleted)
                        {
                            await Shell.Current.DisplayAlert("Delete Failed", message, "OK");
                            return;
                        }

                        PendingItems.Remove(item);
                        OnPropertyChanged(nameof(IsPendingEmpty));
                    }
                }
                catch { }
            });

            CompleteTodoCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    var (updated, message) = await ToDoApiService.ChangeStatusAsync(item.item_id, "inactive");
                    if (!updated)
                    {
                        await Shell.Current.DisplayAlert("Status Update Failed", message, "OK");
                        return;
                    }

                    item.status = "inactive";
                    PendingItems.Remove(item);
                    CompletedItems.Add(item);
                    OnPropertyChanged(nameof(IsPendingEmpty));
                    OnPropertyChanged(nameof(IsCompletedEmpty));
                }
                catch { }
            });

            EditCompletedCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    NavigationContext.EditingItem = item;
                    await Shell.Current.GoToAsync("editcompleted");
                }
                catch { }
            });

            DeleteCompletedCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    bool ok = await Shell.Current.DisplayAlert("Delete", $"Delete \"{item.item_name}\"?", "Delete", "Cancel");
                    if (ok)
                    {
                        var (deleted, message) = await ToDoApiService.DeleteItemAsync(item.item_id);
                        if (!deleted)
                        {
                            await Shell.Current.DisplayAlert("Delete Failed", message, "OK");
                            return;
                        }

                        CompletedItems.Remove(item);
                        OnPropertyChanged(nameof(IsCompletedEmpty));
                    }
                }
                catch { }
            });

            UndoCompletedCommand = new Command<ToDoClass>(async item =>
            {
                if (item == null) return;
                try
                {
                    var (updated, message) = await ToDoApiService.ChangeStatusAsync(item.item_id, "active");
                    if (!updated)
                    {
                        await Shell.Current.DisplayAlert("Status Update Failed", message, "OK");
                        return;
                    }

                    item.status = "active";
                    CompletedItems.Remove(item);
                    PendingItems.Add(item);
                    OnPropertyChanged(nameof(IsPendingEmpty));
                    OnPropertyChanged(nameof(IsCompletedEmpty));
                }
                catch { }
            });

            PendingItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(IsPendingEmpty));
            CompletedItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(IsCompletedEmpty));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RefreshComputedFlags()
        {
            OnPropertyChanged(nameof(IsPendingEmpty));
            OnPropertyChanged(nameof(IsCompletedEmpty));
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
