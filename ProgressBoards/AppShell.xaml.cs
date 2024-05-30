using ProgressBoards.Views;

namespace ProgressBoards;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
		Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
		Routing.RegisterRoute(nameof(ProjectListView), typeof(ProjectListView));
		Routing.RegisterRoute(nameof(ProjectEditView), typeof(ProjectEditView));
		Routing.RegisterRoute(nameof(TaskListView), typeof(TaskListView));
		Routing.RegisterRoute(nameof(AddProjectView), typeof(AddProjectView));
		Routing.RegisterRoute(nameof(TaskEditView), typeof(TaskEditView));
		Routing.RegisterRoute(nameof(TaskAddView), typeof(TaskAddView));
	}
}