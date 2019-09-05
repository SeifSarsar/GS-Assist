using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;

namespace GSAssist
{
    public enum FunctionsChoice
    {
        Both,
        Getter,
        Setter,
        None        
    }

    public class MemberVariable
    {
        public MemberVariable(CodeVariable codeVariable, string className)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            FullName = codeVariable.Name;
            
            //Remove special characters
            PartialName = Regex.Replace(FullName, "[^a-zA-Z0-9]", "");

            string variableSignature = codeVariable.Prototype[(int)vsCMPrototype.vsCMPrototypeType];
           
            //Format Type of variable
            Type = variableSignature.Substring(0, variableSignature.IndexOf(FullName));

            int characterIndexToRemove = Type.LastIndexOf(" ");
            Type = Type.Remove(characterIndexToRemove, 1);
          
            FunctionsChoice = FunctionsChoice.Both;
            ClassName = className;
        }

        public string ClassName { get; set; }

        public string FullName { get; set; }

        public string PartialName { get; set; }

        public string Type { get; set; }

        public FunctionsChoice FunctionsChoice { get; set; }

        public List<FunctionsChoice> FunctionsChoices
        {
            get
            {
                return Enum.GetValues(typeof(FunctionsChoice)).Cast<FunctionsChoice>().ToList();
            }
        }
    }
}
