// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using Sharpliner;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Pipelines;

public partial class PlatformMatrix : JobTemplateDefinition
{
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

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

    public override ConditionedList<JobBase> Definition
    {
        get
        {
            var list = new ConditionedList<JobBase>();

            foreach (Conditioned<JobBase> job in Platforms.Select(CreateTemplate))
            {
                list.Add(job);
            }

            return list;
        }
    }

    private Conditioned<JobBase> CreateTemplate(Platform platform)
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
                { "registry", "mcr" },
            };
        }

        var jobParameters = new TemplateParameters
        {
            { "runtimeFlavor", platform.RuntimeFlavor ?? parameters["runtimeFlavor"] },
            { "stagedBuild", parameters["stagedBuild"] },
            { "buildConfig", parameters["buildConfig"] },
            { "helixQueueGroup", parameters["helixQueueGroup"] },
            { If.Equal("parameters.passPlatforms", "true"),
                new TemplateParameters
                {
                    { "platforms", parameters["platforms"] },
                }
            },
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

        return If.ContainsValue($"'{platform.Name}'", "parameters.platforms")
                 .JobTemplate("xplat-setup.yml", templateParameters);
    }
}
