// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// 

using AutoRest.Core;
using AutoRest.Core.Extensibility;

namespace AutoRest.CSharp.Server
{
    public class GeneratorSettingsCs : AutoRest.CSharp.GeneratorSettingsCs
    {
        public override string Name => "CSharp.Server";

        public override string Description => "Generic C# server-side code generator.";
    }
}