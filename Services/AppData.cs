using System.Collections.ObjectModel;
using ToDoApp.Models;

namespace ToDoApp.Services
{
    public static class AppData
    {
        private static int _nextUserId = 1;
        private static int _nextItemId = 1;

        public static List<UserModel> Users { get; } = new();
        public static UserModel? CurrentUser { get; set; }

        public static ObservableCollection<ToDoClass> PendingItems { get; } = new();
        public static ObservableCollection<ToDoClass> CompletedItems { get; } = new();

        public static int NextUserId() => _nextUserId++;
        public static int NextItemId() => _nextItemId++;
    }
}
