// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.CSharp.Model;
using AutoRest.CSharp.Server.Templates;

namespace AutoRest.CSharp.Server
{
    public class CodeGeneratorCs : CodeGenerator
    {
        private const string ClientRuntimePackage = "Microsoft.Rest.ClientRuntime.2.2.0";

        public override bool IsSingleFileGenerationSupported => true;


        public override string UsageInstructions => string.Format(CultureInfo.InvariantCulture,
            Properties.Resources.UsageInformation, ClientRuntimePackage);

        public override string ImplementationFileExtension => ".cs";
        
        /// <summary>
        /// Generates C# code for service client.
        /// </summary>
        /// <param name="cm"></param>
        /// <returns></returns>
        public override async Task Generate(CodeModel cm)
        {
            // get c# specific codeModel
            var codeModel = cm as CodeModelCs;
            if (codeModel == null)
            {
                throw new InvalidCastException("CodeModel is not a c# CodeModel");
            }

            if (String.IsNullOrEmpty(Settings.Instance.Namespace))
            {
                Settings.Instance.Namespace = codeModel.Name;
            }

            // Models
            foreach (CompositeTypeCs model in codeModel.ModelTypes.Union(codeModel.HeaderTypes))
            {
                var modelTemplate = new AutoRest.CSharp.Templates.ModelTemplate { Model = model };
                await Write(modelTemplate, Path.Combine(Settings.Instance.ModelsName, $"{model.Name}{ImplementationFileExtension}"));
            }

            // Enums
            foreach (EnumTypeCs enumType in codeModel.EnumTypes)
            {
                var enumTemplate = new AutoRest.CSharp.Templates.EnumTemplate { Model = enumType };
                await Write(enumTemplate, Path.Combine(Settings.Instance.ModelsName, $"{enumTemplate.Model.Name}{ImplementationFileExtension}"));
            }

            // Exceptions
            foreach (CompositeTypeCs exceptionType in codeModel.ErrorTypes)
            {
                var exceptionTemplate = new AutoRest.CSharp.Templates.ExceptionTemplate { Model = exceptionType, };
                await Write(exceptionTemplate, Path.Combine(Settings.Instance.ModelsName, $"{exceptionTemplate.Model.ExceptionTypeDefinitionName}{ImplementationFileExtension}"));
            }

            Settings.Instance.Header += " It is advisable to place any custom logic in a separate partial implementation of the class contained in this file.";

            // Service client
            if (codeModel.Methods.Where(m => m.Group.IsNullOrEmpty()).Any())
            {
                var serviceClientTemplate = new ServiceClientTemplate { Model = codeModel };
                await Write(serviceClientTemplate, Path.Combine("Controllers", $"{codeModel.Name}Controller{ImplementationFileExtension}"));
            }

            // operations
            foreach (MethodGroupCs methodGroup in codeModel.Operations)
            {
                if (!methodGroup.Name.IsNullOrEmpty())
                {
                    // Operation
                    var operationsTemplate = new MethodGroupTemplate { Model = methodGroup };
                    await Write(operationsTemplate, Path.Combine("Controllers", $"{operationsTemplate.Model.TypeName}Controller{ImplementationFileExtension}"));
                }
            }
        }
    }
}
