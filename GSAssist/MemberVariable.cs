﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            Name = codeVariable.Name;
            Type = codeVariable.Type.AsFullName;
            FunctionsChoice = FunctionsChoice.Both;
            ClassName = className;
        }

        public string ClassName { get; set; }

        public string Name { get; set; }

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