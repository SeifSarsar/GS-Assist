namespace GSAssist
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.PlatformUI;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("d5683612-64aa-4f21-aca7-e65046ebf312")]
    public class GSAssistWindow : DialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GSAssistWindow"/> class.
        /// </summary>
        public GSAssistWindow()
        {
            Title = "GS Assist";

            Width = 400;
            MaxHeight = 450;

            SizeToContent = System.Windows.SizeToContent.Height;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.

            Action closeWindow = () =>
            {
                Close();
            };

            Content = new GSAssistWindowControl(closeWindow);
        }
    }
}
