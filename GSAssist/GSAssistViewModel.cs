using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GSAssist
{
    public enum FileLocation
    {
        Source,
        Header
    }
    public class GSAssistViewModel
    {

        public GSAssistViewModel(Dictionary<string, MemberVariable> memberVariables,FileLocation fileLocation, CodeReader codeReader, CodeWriter codeWriter)
        {
            MemberVariables = memberVariables;
            FileLocation = fileLocation;
            CodeReader = codeReader;
            CodeWriter = codeWriter;
        }

        public Dictionary<string, MemberVariable> MemberVariables { get; set; }

        public FileLocation FileLocation { get; set; }

        public CodeReader CodeReader { get; set; }

        public CodeWriter CodeWriter { get; set; }

        public void ChangeMemberVariable(string variableName, FunctionsChoice functionsChoice)
        {
            MemberVariables[variableName].FunctionsChoice = functionsChoice;
        }

        public void ChangeFileLocation(FileLocation fileLocation)
        {
            FileLocation = fileLocation;
        }

        public List<FileLocation> FileLocations
        {
            get
            {
                return Enum.GetValues(typeof(FileLocation)).Cast<FileLocation>().ToList();
            }
        }

        static public GSAssistViewModel GetViewModel()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = GSAssistWindowCommand.Dte;

            CodeReader codeReader = new CodeReader(dte);
            //Get active document
            ProjectItem activeDocument = codeReader.GetActiveDocument();
            //Get active document class
            CodeClass codeClass = codeReader.GetClass(activeDocument);

            if (codeClass == null)
            {
                return null;
            }

            CodeWriter codeWriter = new CodeWriter();

            Dictionary<string, MemberVariable> classVariables = codeReader.GetClassVariables(codeClass);

            if (classVariables.Count == 0)
            {
                ErrorHandler.ShowMessageBox("Can not find any class variables.");
                return null;
            }

            return new GSAssistViewModel(classVariables, FileLocation.Source, codeReader,codeWriter);
        }
    }
}
