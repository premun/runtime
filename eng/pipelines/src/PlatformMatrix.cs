// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Sharpliner.AzureDevOps;

namespace Pipelines;

public partial class PlatformMatrix : JobTemplateDefinition
{
    private List<Platform> Platforms => new()
    {
        new("Linux_arm", "Linux", "arm",
            Container: "ubuntu-16.04-cross-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm"),

        new("Linux_arm64", "Linux", "arm64",
            Container: "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm64"),

        new("Linux_musl_x64", "Linux", "x64",
            OsSubGroup: "_musl",
            Container: "alpine-3.13-WithNode-20210910135845-c401c85"),

        new("Linux_musl_arm", "Linux", "arm",
            OsSubGroup: "_musl",
            Container: "ubuntu-16.04-cross-arm-alpine-20210923140502-78f7860",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm"),

        new("Linux_musl_arm64", "Linux", "arm64",
            OsSubGroup: "_musl",
            Container: "ubuntu-16.04-cross-arm64-alpine-20210923140502-78f7860",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm64"),

        new("Linux_x64", "Linux", "x64", "linux-x64",
            Container:
                new TemplateParameters
                {
                    { If.Equal("parameters.container", "''"), new TemplateParameters
                    {
                        { "image", "centos-7-20210714125435-9b5bbc2" }
                    }},
                    { If.NotEqual("parameters.container", "''"), new TemplateParameters
                    {
                        { "image", parameters["container"] }
                    }}
                }),

        new("Linux_x86", "Linux", "x86",
            Container: "ubuntu-18.04-cross-x86-linux-20211022152824-f853169",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/x86"),

        new("SourceBuild_Linux_x64", "Linux", "x64",
            Container: "centos-7-source-build-20210714125450-5d87b80",
            AdditionalJobParams: new()
            {
                { "buildingOnSourceBuildImage", true }
            }),

        new("Linux_s390x", "Linux", "s390x",
            Container: "ubuntu-18.04-cross-s390x-20201102145728-d6e0352",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/s390x"),

        new("Browser_wasm", "Browser", "wasm",
            Container: "ubuntu-18.04-webassembly-20210531091624-f5c7a43"),

        new("Browser_wasm_win", "Browser", "wasm",
            PlatformId: "Browser_wasm_win",
            Container: "ubuntu-18.04-webassembly-20210531091624-f5c7a43"),

        new("FreeBSD_x64", "FreeBSD", "x64",
            Container: "ubuntu-18.04-cross-freebsd-12-20210917001307-f13d79e",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/x64"),

        new("Android_x64", "Android", "x64",
            Container: "ubuntu-18.04-android-20200422191843-e2c3f83",
            RuntimeFlavor: "mono"),

        new("Android_x86", "Android", "x86",
            Container: "ubuntu-18.04-android-20200422191843-e2c3f83",
            RuntimeFlavor: "mono"),

        new("Android_arm", "Android", "arm",
            Container: "ubuntu-18.04-android-20200422191843-e2c3f83",
            RuntimeFlavor: "mono"),

        new("Android_arm64", "Android", "arm64",
            Container: "ubuntu-18.04-android-20200422191843-e2c3f83",
            RuntimeFlavor: "mono"),

        new("MacCatalyst_x64", "MacCatalyst", "x64",
            RuntimeFlavor: "mono"),

        new("MacCatalyst_arm64", "MacCatalyst", "arm64",
            RuntimeFlavor: "mono"),

        new("tvOS_arm64", "tvOS", "arm64",
            RuntimeFlavor: "mono"),

        new("tvOSSimulator_x64", "tvOSSimulator", "x64",
            RuntimeFlavor: "mono"),

        new("tvOSSimulator_arm64", "tvOSSimulator", "arm64",
            RuntimeFlavor: "mono"),

        new("iOS_arm", "iOS", "arm",
            RuntimeFlavor: "mono"),

        new("iOS_arm64", "iOS", "arm64",
            RuntimeFlavor: "mono"),

        new("iOSSimulator_x64", "iOSSimulator", "x64",
            RuntimeFlavor: "mono"),

        new("iOSSimulator_x86", "iOSSimulator", "x86",
            RuntimeFlavor: "mono"),

        new("iOSSimulator_arm64", "iOSSimulator", "arm64",
            RuntimeFlavor: "mono"),

        new("OSX_arm64", "OSX", "arm64"),

        new("OSX_x64", "OSX", "x64"),

        new("Tizen_armel", "Tizen", "armel",
            Container: "ubuntu-18.04-cross-armel-tizen-20210719212651-8b02f56",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/armel",
            AdditionalJobParams: new()
            {
                { "disableClrTest", true }
            }),

        new("windows_x64", "Windows", "x64"),

        new("windows_x86", "Windows", "x86"),

        new("windows_arm", "Windows", "arm"),

        new("windows_arm64", "Windows", "arm64"),
    };
}
