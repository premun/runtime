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

        new("Linux_musl_x64", "Linux", "x64", "linux-musl-x64",
            OsSubGroup: "_musl",
            Container: "alpine-3.13-WithNode-20210910135845-c401c85"),

        new("Linux_musl_arm", "Linux", "arm", "linux-musl-arm",
            OsSubGroup: "_musl",
            Container: "ubuntu-16.04-cross-arm-alpine-20210923140502-78f7860",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm"),

        new("Linux_musl_arm64", "Linux", "arm64", "linux-musl-arm64",
            OsSubGroup: "_musl",
            Container: "ubuntu-16.04-cross-arm64-alpine-20210923140502-78f7860",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm64"),

        new("Linux_x64", "Linux", "x64", "linux-x64",
            Container:
                If.Equal("parameters.container", "''")
                    .Value(new TemplateParameters
                    {
                        { "image", "centos-7-20210714125435-9b5bbc2" }
                    })
                .Else
                    .Value(new TemplateParameters
                    {
                        { "image", parameters["container"] }
                    })),

        new("Linux_x86", "Linux", "x86", "linux-x86",
            Container: "ubuntu-18.04-cross-x86-linux-20211022152824-f853169",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/x86"),

        new("SourceBuild_Linux_x64", "Linux", "x64", "linux-x64",
            Container: "centos-7-source-build-20210714125450-5d87b80",
            AdditionalJobParams: new()
            {
                { "buildingOnSourceBuildImage", true }
            }),

        new("Linux_s390x", "Linux", "s390x", "linux-s390x",
            Container: "ubuntu-18.04-cross-s390x-20201102145728-d6e0352",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/s390x"),
    };
}
