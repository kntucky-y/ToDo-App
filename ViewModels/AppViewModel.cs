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
                NavigationContext.EditingItem = null;
                await Shell.Current.GoToAsync("addtodo");
            });

            EditTodoCommand = new Command<ToDoClass>(async item =>
            {
                NavigationContext.EditingItem = item;
                await Shell.Current.GoToAsync("edittodo");
            });

            DeleteTodoCommand = new Command<ToDoClass>(async item =>
            {
                bool ok = await Shell.Current.DisplayAlert("Delete", $"Delete \"{item.item_name}\"?", "Delete", "Cancel");
                if (ok)
                {
                    PendingItems.Remove(item);
                    OnPropertyChanged(nameof(IsPendingEmpty));
                }
            });

            CompleteTodoCommand = new Command<ToDoClass>(item =>
            {
                item.status = "completed";
                PendingItems.Remove(item);
                CompletedItems.Add(item);
                OnPropertyChanged(nameof(IsPendingEmpty));
                OnPropertyChanged(nameof(IsCompletedEmpty));
            });

            EditCompletedCommand = new Command<ToDoClass>(async item =>
            {
                NavigationContext.EditingItem = item;
                await Shell.Current.GoToAsync("editcompleted");
            });

            DeleteCompletedCommand = new Command<ToDoClass>(async item =>
            {
                bool ok = await Shell.Current.DisplayAlert("Delete", $"Delete \"{item.item_name}\"?", "Delete", "Cancel");
                if (ok)
                {
                    CompletedItems.Remove(item);
                    OnPropertyChanged(nameof(IsCompletedEmpty));
                }
            });

            UndoCompletedCommand = new Command<ToDoClass>(item =>
            {
                item.status = "pending";
                CompletedItems.Remove(item);
                PendingItems.Add(item);
                OnPropertyChanged(nameof(IsPendingEmpty));
                OnPropertyChanged(nameof(IsCompletedEmpty));
            });

            PendingItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(IsPendingEmpty));
            CompletedItems.CollectionChanged += (_, _) => OnPropertyChanged(nameof(IsCompletedEmpty));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
