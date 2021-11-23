// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Sharpliner.AzureDevOps;

namespace Pipelines;

public record Platform(
    string Name,
    string OsGroup,
    string Architecture,
    string? TargetRid = null,
    string? PlatformId = null,
    string? OsSubGroup = null,
    string? HostedOs = null,
    object? Container = null,
    string? RuntimeFlavor = null,
    bool CrossBuild = false,
    string? CrossRootFsDir = null,
    IEnumerable<string>? RunForPlatforms = null,
    TemplateParameters? AdditionalJobParams = null);
