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
            editPoint.Insert(Environment.NewLine + newText);
        }

        public void WriteInHeaderFile(EditPoint editPoint, Dictionary<string, MemberVariable> memberVariables, bool isWithFunctionDefinition)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string newText = null;
            foreach (MemberVariable memberVariable in memberVariables.Values)
            {
                if (memberVariable.FunctionsChoice == FunctionsChoice.Getter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteHeaderGetterFunction(memberVariable, isWithFunctionDefinition,editPoint.LineCharOffset);
                }
                if (memberVariable.FunctionsChoice == FunctionsChoice.Setter || memberVariable.FunctionsChoice == FunctionsChoice.Both)
                {
                    newText += WriteHeaderSetterFunction(memberVariable, isWithFunctionDefinition, editPoint.LineCharOffset);
                }
                if (memberVariable.FunctionsChoice != FunctionsChoice.None)
                {
                    newText += Environment.NewLine;
                }
            }
            editPoint.Insert("public:" + Environment.NewLine + newText);
        }

        public string WriteHeaderGetterFunction(MemberVariable variable, bool isWithFunctionDefinition, int offset)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            
            string functionName = "get" + char.ToUpper(variable.PartialName[0]) + variable.PartialName.Substring(1);

            string functionSignature = "".PadLeft(4 * offset) + variable.Type + " " + functionName + "() const";

            string functionDefinition = "return " + variable.FullName + ";";

            if (isWithFunctionDefinition)
            {
                return functionSignature + " { " + functionDefinition + " }" + Environment.NewLine;
            }
            else
            {
                return functionSignature + ";" + Environment.NewLine;
            }
        }

        public string WriteHeaderSetterFunction(MemberVariable variable, bool isWithFunctionDefinition, int offset)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            string functionName = "set" + char.ToUpper(variable.PartialName[0]) + variable.PartialName.Substring(1);

            string functionSignature = "".PadLeft(4*offset) + "void " + functionName + "(" + variable.Type + " " + variable.PartialName + ")";

            string functionDefinition =  variable.FullName + " = " + variable.PartialName + ";";

            if (variable.FullName == variable.PartialName)
                functionDefinition = "this->" + functionDefinition;


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
           
            string functionName = "get" + char.ToUpper(variable.PartialName[0]) + variable.PartialName.Substring(1);

            string functionSignature = variable.Type + " " + variable.ClassName + "::" + functionName + "() const";

            string functionDefinition = "return " + variable.FullName + ";";

            return functionSignature + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "".PadLeft(4) + functionDefinition + Environment.NewLine +
                   "}" + Environment.NewLine;
        }

        public string WriteSourceSetterFunction(MemberVariable variable)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string functionName = "set" + char.ToUpper(variable.PartialName[0]) + variable.PartialName.Substring(1);

            string functionSignature = "void " + variable.ClassName + "::" + functionName + "(" + variable.Type + " " + variable.PartialName + ")";

            string functionDefinition = variable.FullName + " = " + variable.PartialName + ";";

            if (variable.FullName == variable.PartialName)
                functionDefinition = "this->" + functionDefinition;

            return functionSignature + Environment.NewLine +
                   "{" + Environment.NewLine +
                   "".PadLeft(4) + functionDefinition + Environment.NewLine +
                   "}" + Environment.NewLine;
        }
    }
}
