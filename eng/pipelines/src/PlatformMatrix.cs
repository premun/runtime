// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Pipelines;

public class PlatformMatrix : JobTemplateDefinition
{
    public override string TargetFile => Configuration.PipelinesPath + "common/platform-matrix.yml";

    public override List<TemplateParameter> Parameters => new()
    {
        StringParameter("runtimeFlavor", "coreclr", new[] { "coreclr", "mono" }),
        JobParameter("jobTemplate"),
        StringParameter("buildConfig", string.Empty),
        ObjectParameter("platforms"),

        // platformGroup is a named collection of platforms.
        StringParameter("platformGroup", string.Empty, new[]
        {
            "all", // all platforms
            "gcstress" // platforms that support running under GCStress0x3 and GCStress0xC scenarios
        }),

        StringParameter("helixQueueGroup", "pr", new[]
        {
            "pr",
            "ci",
            "all"
        }),

        // helixQueuesTemplate is a yaml template which will be expanded in order to set up the helix queues
        // for the given platform and helixQueueGroup.
        StringParameter("helixQueuesTemplate", string.Empty),
        BooleanParameter("stagedBuild", false),

        // When set to false, suppresses reuse of OSX managed build artifacts (for pipelines without an OSX obj)
        // When set to true, passes the 'platforms' value as a job parameter also named 'platforms'.
        // Handled as an opt-in parameter to avoid excessive yaml.
        BooleanParameter("passPlatforms", false),
        StringParameter("container"),
        ObjectParameter("jobParameters"),
        ObjectParameter("variables"),
    };

    private static readonly List<Platform> _platforms = new()
    {
        new("Linux_arm", "Linux", "arm", "linux-arm",
            Container: "ubuntu-16.04-cross-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm"),

        new("Linux_arm", "Linux", "arm64", "linux-arm64",
            Container: "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be",
            CrossBuild: true,
            CrossRootFsDir: "/crossrootfs/arm64"),
    };

    public override ConditionedList<JobBase> Definition
    {
        get
        {
            var list = new ConditionedList<JobBase>();

            foreach (Conditioned<JobBase> job in _platforms.Select(Platform))
            {
                list.Add(job);
            }

            return list;
        }
    }

    private Conditioned<JobBase> Platform(Platform platform)
    {
        var templateParameters = new TemplateParameters
        {
            { "jobTemplate", parameters["jobTemplate"] },
            { "helixQueuesTemplate", parameters["helixQueuesTemplate"] },
            { "variables", parameters["variables"] },
            { "osGroup", platform.OsGroup },
            { "archType", platform.Architecture },
            { "targetRid", platform.TargetRid },
            { "platform", platform.PlatformId ?? platform.OsGroup + "_" + platform.Architecture },
        };

        if (platform.OsSubGroup != null)
        {
            templateParameters["osSubGroup"] = platform.OsSubGroup;
        }

        if (platform.Container != null)
        {
            templateParameters["container"] = new TemplateParameters
            {
                { "image", platform.Container },
                { "platform", "mcr" },
            };
        }

        var jobParameters = new TemplateParameters
        {
            { "runtimeFlavor", platform.RuntimeFlavor ?? parameters["runtimeFlavor"] },
            { "stagedBuild", parameters["stagedBuild"] },
            { "buildConfig", parameters["buildConfig"] },
            { "helixQueueGroup", parameters["helixQueueGroup"] },
            { "${{ if eq(parameters.passPlatforms, true) }}:", new TemplateParameters
            {
                { "platforms", parameters["platforms"] },
            }},
        };

        if (platform.CrossBuild)
        {
            jobParameters["crossBuild"] = true;
        }

        if (platform.CrossRootFsDir != null)
        {
            jobParameters["crossrootfsDir"] = platform.CrossRootFsDir;
        }

        if (platform.AdditionalJobParams != null)
        {
            foreach (var pair in platform.AdditionalJobParams)
            {
                jobParameters[pair.Key] = pair.Value;
            }
        }

        jobParameters["${{ insert }}"] = parameters["jobParameters"];

        templateParameters["jobParameters"] = jobParameters;

        return If.Equal($"containsValue(parameters.platforms, '{platform.Name}')", "in(parameters.platformGroup, 'all', 'gcstress')")
            .JobTemplate("xplat-setup.yml", templateParameters);
    }
}

public record Platform(
    string Name,
    string OsGroup,
    string Architecture,
    string TargetRid,
    string? PlatformId = null,
    string? OsSubGroup = null,
    string? Container = null,
    string? RuntimeFlavor = null,
    bool CrossBuild = false,
    string? CrossRootFsDir = null,
    TemplateParameters? AdditionalJobParams = null);
