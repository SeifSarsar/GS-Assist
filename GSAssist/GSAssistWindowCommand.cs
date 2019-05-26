using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;
using EnvDTE;

namespace GSAssist
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class GSAssistWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f923722d-a01b-4a3b-8be0-2dcfa8ae18d4");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="GSAssistWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private GSAssistWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GSAssistWindowCommand Instance
        {
            get;
            private set;
        }


        public static DTE Dte { get; set; }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in GSAssistWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            Dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            Microsoft.Assumes.Present(Dte);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new GSAssistWindowCommand(package, commandService);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //If Dte service is unavailable or if there is no active document, GS Assist won't be used.
            if (Dte == null)
            {
                ErrorHandler.ShowMessageBox("GS Assist is unavailable.");
                return;
            }
            else if (Dte.ActiveDocument == null)
            {
                ErrorHandler.ShowMessageBox("Can not use GS Assist because there is no active header file.");
                return;
            }
            if (Dte.ActiveDocument.Name.Contains(".cpp"))
            {
                ErrorHandler.ShowMessageBox("Can not use GS Assist with an active source file. You need a header file.");
                return;
            }

            GSAssistWindow window = new GSAssistWindow();

            GSAssistWindowControl windowControl = window.Content as GSAssistWindowControl;

            //if GS AssistViewModel is null, then we
            if (windowControl.GSAssistViewModel != null)
            {
                window.ShowDialog();
            }
        }
    }
}
