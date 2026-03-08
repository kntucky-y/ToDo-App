namespace ToDoApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("addtodo", typeof(Pages.AddTodoPage));
        Routing.RegisterRoute("edittodo", typeof(Pages.EditTodoPage));
        Routing.RegisterRoute("editcompleted", typeof(Pages.EditCompletedPage));
    }
}
