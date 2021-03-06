// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#ifndef __HEADER_H__
#define __HEADER_H__

#include <cstdint>
#include "pal.h"
#include "reader.h"

namespace bundle
{
    // The Bundle Header (v1)
    // Fixed size thunk (header_fixed_t)
    //   - Major Version     
    //   - Minor Version     
    //   - Number of embedded files
    // Variable size portion:
    //   - Bundle ID (7-bit extension encoded length prefixed string)
    // The Bundle Header (v2) [additional content]
    // Fixed size thunk (header_fixed_v2_t)
    //   - DepsJson Location (Offset, Size)
    //   - RuntimeConfig Location (Offset, Size)
    //   - Flags

#pragma pack(push, 1)
    struct header_fixed_t
    {
    public:
        uint32_t major_version;
        uint32_t minor_version;
        int32_t num_embedded_files;

        bool is_valid(bool exact_match = false) const;
    };
#pragma pack(pop)

    // netcoreapp3_compat_mode flag is set on a .net5 app, which chooses to build single-file apps in .netcore3.x compat mode,
    // This indicates that:
    //   All published files are bundled into the app; some of them will be extracted to disk.
    //   AppContext.BaseDirectory is set to the extraction directory (and not the AppHost directory).
    enum header_flags_t : uint64_t
    {
        none = 0,
        netcoreapp3_compat_mode = 1
    };

#pragma pack(push, 1)
    struct location_t
    {
    public:
        int64_t offset;
        int64_t size;
    };

    // header_fixed_v2_t is available in single-file apps targetting .net5+ frameworks.
    // It stores information that facilitates the host to process contents of the bundle without extraction.
    //
    // The location of deps.json and runtimeconfig.json is already available in the Bundle manifest.
    // However, the data is cached here in order to simplify the bundle-processing performed by hostfxr.
    struct header_fixed_v2_t
    {
    public:
        location_t deps_json_location;
        location_t runtimeconfig_json_location;
        header_flags_t flags;
    };
#pragma pack(pop)

    struct header_t
    {
    public:
        header_t(int32_t num_embedded_files = 0)
            : m_num_embedded_files(num_embedded_files)
            , m_bundle_id()
            , m_v2_header(NULL)

        {
        }

        static header_t read(reader_t& reader, bool need_exact_version);
        const pal::string_t& bundle_id() { return m_bundle_id; }
        int32_t num_embedded_files() { return m_num_embedded_files; }

        static const uint32_t major_version = 2;
        static const uint32_t minor_version = 0;

    private:
        int32_t m_num_embedded_files;
        pal::string_t m_bundle_id;
        const header_fixed_v2_t* m_v2_header;

    };
}
#endif // __HEADER_H__
