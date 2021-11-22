// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Pipelines;

public record Platform(
    string Name,
    string OsGroup,
    string Architecture,
    string TargetRid,
    string? PlatformId = null,
    string? OsSubGroup = null,
    Conditioned<string>? Container = null,
    string? RuntimeFlavor = null,
    bool CrossBuild = false,
    string? CrossRootFsDir = null,
    TemplateParameters? AdditionalJobParams = null);
