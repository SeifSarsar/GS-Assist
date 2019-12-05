using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;

namespace GSAssist
{
    public class CodeReader
    {
        public CodeReader(DTE dte)
        {
            Dte = dte;
        }

        public DTE Dte { get; set; }

        //Returns the first class in the file
        public CodeClass GetClass(ProjectItem activeDocument)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            CodeElements fileCodeModel = activeDocument.FileCodeModel.CodeElements;
            TextSelection textSelection = Dte.ActiveDocument.Selection as TextSelection;
            VirtualPoint point = textSelection.ActivePoint;
            CodeElement codeElement = point.CodeElement[vsCMElement.vsCMElementClass];

            if (codeElement != null)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementClass) //verify if it's a class
                {
                    return codeElement as CodeClass;
                }
                else if (codeElement.Kind == vsCMElement.vsCMElementVariable)
                {
                    CodeVariable codeVariable = codeElement as CodeVariable;
                    codeElement = codeVariable.Parent as CodeElement;
                    return codeElement as CodeClass;

                }
                else if (codeElement.Kind == vsCMElement.vsCMElementFunction)
                {
                    CodeFunction codeFunction = codeElement as CodeFunction;
                    codeElement = codeFunction.Parent as CodeElement;
                    return codeElement as CodeClass;

                }  
            }
            
            ErrorHandler.ShowMessageBox("Please select a class element.");
            return null;
           
        }

        //Gets the variables in the class
        public Dictionary<string, MemberVariable> GetClassVariables(CodeClass codeClass)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Dictionary<string, MemberVariable> memberVariables = new Dictionary<string, MemberVariable>();
            List<CodeVariable> attributes = new List<CodeVariable>();

            foreach (CodeElement codeElement in codeClass.Children)
            {
                if (codeElement.Kind == vsCMElement.vsCMElementVariable)
                {
                    MemberVariable memberVariable = new MemberVariable((codeElement as CodeVariable), codeClass.Name);
                    memberVariables.Add(memberVariable.FullName, memberVariable);
                }
            }
            return memberVariables;
        }

        public ProjectItem GetActiveDocument()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return Dte.ActiveDocument.ProjectItem;
        }

        public EditPoint GetSourceFileEditPoint()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
        
            string headerFilename = Dte.ActiveDocument.Name;
            string sourceFilename = headerFilename.Substring(0, headerFilename.Length - 2) + ".cpp";

            Solution solution = Dte.Solution;
            if (solution == null)
            {
                ErrorHandler.ShowMessageBox("Can not find source file without a solution.");
                return null;
            }

            ProjectItem sourceFile = solution.FindProjectItem(sourceFilename);

            if (sourceFile == null)
            {
                ErrorHandler.ShowMessageBox("Can not find source file.");
                return null;
            }

            if (!sourceFile.IsOpen)
            {
                sourceFile.Open();
            }

            TextDocument sourceFileDocument = sourceFile.Document.Object("TextDocument") as TextDocument;
            return sourceFileDocument.EndPoint.CreateEditPoint();

        }

        public EditPoint GetHeaderFileEditPoint()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            CodeClass codeClass = GetClass(GetActiveDocument());

            if (codeClass == null)
            {
              
                ErrorHandler.ShowMessageBox("Can not find a class in the header file.");
                return null;
            }

            return codeClass.GetEndPoint(vsCMPart.vsCMPartBody).CreateEditPoint();
        }
    }
}
