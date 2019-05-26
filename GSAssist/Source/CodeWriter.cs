using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace GSAssist
{
    public class CodeWriter
    {
        public void WriteInSourceFile(EditPoint editPoint, Dictionary<string,MemberVariable> memberVariables)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string newText = null;
            foreach (MemberVariable memberVariable in memberVariables.Values)
            {
                if (memberVariable.FunctionsChoice == FunctionsChoice.Getter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteSourceGetterFunction(memberVariable) + Environment.NewLine;
                }
                if (memberVariable.FunctionsChoice == FunctionsChoice.Setter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteSourceSetterFunction(memberVariable) + Environment.NewLine;
                }
            }
            editPoint.Insert(newText);
        }

        public void WriteInHeaderFile(EditPoint editPoint, Dictionary<string, MemberVariable> memberVariables, bool isWithFunctionDefinition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string newText = null;
            foreach (MemberVariable memberVariable in memberVariables.Values)
            {
                if (memberVariable.FunctionsChoice == FunctionsChoice.Getter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteHeaderGetterFunction(memberVariable, isWithFunctionDefinition);
                }
                if (memberVariable.FunctionsChoice == FunctionsChoice.Setter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteHeaderSetterFunction(memberVariable, isWithFunctionDefinition);
                }
                if (memberVariable.FunctionsChoice != FunctionsChoice.None)
                {
                    newText += Environment.NewLine;
                }
            }
            editPoint.Insert(newText);
        }

        public string WriteHeaderGetterFunction(MemberVariable variable, bool isWithFunctionDefinition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            string functionName = "get" + char.ToUpper(variable.Name[0]) + variable.Name.Substring(1);

            string functionSignature = "".PadLeft(4) + variable.Type + " " + functionName + "() const";

            string functionDefinition = "return " + variable.Name + ";";

            if (isWithFunctionDefinition)
            {
                return functionSignature + " { " + functionDefinition + " }" + Environment.NewLine;
            }
            else
            {
                return functionSignature + ";" + Environment.NewLine;
            }
        }

        public string WriteHeaderSetterFunction(MemberVariable variable, bool isWithFunctionDefinition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string functionName = "set" + char.ToUpper(variable.Name[0]) + variable.Name.Substring(1);

            string functionSignature = "".PadLeft(4) + "void " + functionName + "(" + variable.Type + " " + variable.Name + ")";

            string functionDefinition = "this->" + variable.Name + " = " + variable.Name + ";";

            if (isWithFunctionDefinition)
            {
                return functionSignature + " { " + functionDefinition + " }" + Environment.NewLine;
            }
            else
            {
                return functionSignature + ";" + Environment.NewLine;
            }
        }

        public string WriteSourceGetterFunction(MemberVariable variable)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
           
            string functionName = "get" + char.ToUpper(variable.Name[0]) + variable.Name.Substring(1);

            string functionSignature = variable.Type + " " + variable.ClassName + "::" + functionName + "() const";

            string functionDefinition = "return " + variable.Name + ";";

            return functionSignature + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "".PadLeft(4) + functionDefinition + Environment.NewLine +
                   "}" + Environment.NewLine;
        }

        public string WriteSourceSetterFunction(MemberVariable variable)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string functionName = "set" + char.ToUpper(variable.Name[0]) + variable.Name.Substring(1);

            string functionSignature = "void " + variable.ClassName + "::" + functionName + "(" + variable.Type + " " + variable.Name + ")";

            string functionDefinition = "this->" + variable.Name + " = " + variable.Name + ";";

            return functionSignature + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "".PadLeft(4) + functionDefinition + Environment.NewLine +
                   "}" + Environment.NewLine;
        }
    }
}
