// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AutoRest.Core.Model;
using AutoRest.Core.Utilities;
using AutoRest.Extensions;
using Newtonsoft.Json;

namespace AutoRest.CSharp.Server.Model
{
    public class MethodCs : AutoRest.CSharp.Model.MethodCs
    {
        /// <summary>
        /// Generate the method parameter declaration for sync methods and extensions
        /// </summary>
        /// <param name="addCustomHeaderParameters">If true add the customHeader to the parameters</param>
        /// <returns>Generated string of parameters</returns>
        public override string GetSyncMethodParameterDeclaration(bool addCustomHeaderParameters)
        {
            List<string> declarations = new List<string>();
            foreach (var parameter in LocalParameters)
            {
                string format = "{0}{1} {2}";
                var attribute = "";
                switch (parameter.Location)
                {
                    case ParameterLocation.Body:
                        attribute = "[FromBody]";
                        break;
                    case ParameterLocation.Header:
                        attribute = "[FromHeader]";
                        break;
                    case ParameterLocation.Query:
                        attribute = "[FromQuery]";
                        break;
                    case ParameterLocation.FormData:
                        attribute = "[FromForm]";
                        break;
                }
                declarations.Add(string.Format(CultureInfo.InvariantCulture, format, attribute, parameter.ModelTypeName, parameter.Name));
            }

            return string.Join(", ", declarations);
        }

        /// <summary>
        /// Get the parameters that are actually method parameters in the order they appear in the method signature
        /// exclude global parameters
        /// </summary>
        [JsonIgnore]
        public new IEnumerable<AutoRest.CSharp.Model.ParameterCs> LocalParameters
        {
            get
            {
                return
                    Parameters.Cast<AutoRest.CSharp.Model.ParameterCs>();
            }
        }

        /// <summary>
        /// Get the type name for the method's return type
        /// </summary>
        public override string ReturnTypeString
        {
            get
            {
                if (ReturnType.Body != null)
                {
                    return ReturnType.Body.AsNullableType(HttpMethod != HttpMethod.Head);
                }
                if (ReturnType.Headers != null)
                {
                    return ReturnType.Headers.AsNullableType(HttpMethod != HttpMethod.Head);
                }
                else
                {
                    return "void";
                }
            }
        }

        public string HttpAttribute
        {
            get
            {
                var builder = new StringBuilder();
                builder.Append("[Http");
                builder.Append(HttpMethod.ToString());                
                if (!string.IsNullOrEmpty(Url))
                {
                    builder.Append($"(\"{Url.TrimStart('/')}\")");
                }
                builder.Append("]");
                return builder.ToString();
            }
            
        }
    }
}
