// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Sharpliner.AzureDevOps;

namespace Pipelines;

public partial class PlatformMatrix : JobTemplateDefinition
{
    private List<Platform> Platforms => new()
    {
        new("Linux_arm", "Linux", "arm", "linux-arm",
            Container: "ubuntu-16.04-cross-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm"),

        new("Linux_arm64", "Linux", "arm64", "linux-arm64",
            Container: "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm64"),

        new("Linux_x64", "Linux", "x64", "linux-x64",
            Container:
                If.Equal(parameters["container"], "''")
                    .Value("ubuntu-16.04-cross-arm64-20210719121212-8a8d3be")
                .Else
                    .Value(parameters["container"]),
            CrossBuild: true),
    };
}
