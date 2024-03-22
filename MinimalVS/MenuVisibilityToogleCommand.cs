using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace MinimalVS
{
    internal sealed class MenuVisibilityToogleCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("1901ec3a-bb19-4ff6-a290-084536af3115");

        private readonly MinimalVSPackage _package;
        private readonly MenuCommand _menuItem;

        private MenuVisibilityToogleCommand(MinimalVSPackage package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            _menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(_menuItem);
        }

        public static MenuVisibilityToogleCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(MinimalVSPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new MenuVisibilityToogleCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _package.SwitchMenuVisibility();
        }
    }
}
