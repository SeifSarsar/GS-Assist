namespace GSAssist
{
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using Microsoft.VisualStudio.Shell;
    using EnvDTE;
    /// <summary>
    /// Interaction logic for GSAssistControl.
    /// </summary>
    public partial class GSAssistWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GSAssistControl"/> class.
        /// </summary>

        public GSAssistViewModel GSAssistViewModel { get; set; }

        public Action CloseWindow { get; set; }

        public GSAssistWindowControl(Action closeWindow)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            InitializeComponent();
            GSAssistViewModel = GSAssistViewModel.GetViewModel();
            Attributes.DataContext = GSAssistViewModel;
            CloseWindow = closeWindow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CodeReader codeReader = GSAssistViewModel.CodeReader;

            EditPoint headerFileEditPoint = codeReader.GetHeaderFileEditPoint();
            if (headerFileEditPoint == null)
            {
                return;
            }
            if (GSAssistViewModel.FileLocation == FileLocation.Source)
            {
                EditPoint sourceFileEditPoint = codeReader.GetSourceFileEditPoint();
                if (sourceFileEditPoint == null)
                {
                    return;
                }

                GSAssistViewModel.CodeWriter.WriteInHeaderFile(headerFileEditPoint, GSAssistViewModel.MemberVariables, false);

                GSAssistViewModel.CodeWriter.WriteInSourceFile(sourceFileEditPoint, GSAssistViewModel.MemberVariables);
            }
            else
            {
                GSAssistViewModel.CodeWriter.WriteInHeaderFile(headerFileEditPoint, GSAssistViewModel.MemberVariables, true);
            }
            CloseWindow();
        }
    }
}