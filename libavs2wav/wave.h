
#ifndef WAVE_H
#define WAVE_H

#include <stdint.h>

typedef enum WaveFormatType WaveFormatType;
typedef struct WaveGuid WaveGuid;
typedef struct WaveChunkHeader WaveChunkHeader;
typedef struct WaveRiffChunk WaveRiffChunk;
typedef struct WaveDs64Chunk WaveDs64Chunk;
typedef struct WaveFormatChunk WaveFormatChunk;
typedef struct WaveFactChunk WaveFactChunk;
typedef struct WaveDataChunk WaveDataChunk;
typedef struct WaveRiffHeader WaveRiffHeader;
typedef struct WaveRf64Header WaveRf64Header;

enum WaveFormatType {
	WAVE_FORMAT_PCM2 = 0x0001,   // samples are ints
	WAVE_FORMAT_IEEE_FLOAT2 = 0x0003,   // samples are floats
	WAVE_FORMAT_EXTENSIBLE2 = 0xFFFE, // not a real type.
};

#define WAVE_FOURCC(ch0, ch1, ch2, ch3) \
    ((uint32_t)(uint8_t)(ch0) | ((uint32_t)(uint8_t)(ch1) << 8) |\
    ((uint32_t)(uint8_t)(ch2) << 16) | ((uint32_t)(uint8_t)(ch3) << 24 ))

void
wave_guid_copy(WaveGuid *dst, WaveGuid *src);

WaveRiffHeader *
wave_create_riff_header (WaveFormatType format,
                         uint16_t       channels,
                         uint32_t       sample_rate,
                         uint16_t       byte_depth,
                         uint64_t       samples);

WaveRf64Header *
wave_create_rf64_header (WaveFormatType format,
                           uint16_t       channels,
                           uint32_t       sample_rate,
                           uint16_t       byte_depth,
                           uint64_t       samples);

// set packing alignment to 1 byte so we can just fwrite structs
// gcc docs say it supports this to be compatable with vs.
#pragma pack(push, 1)

// docs state a chunk should be [id, size, [data, ...]] but this way 
// means I can get size = sizeof(chunk) - sizeof(chunk.header)

// just a uuid, but this works so...
struct WaveGuid {
    uint32_t d1;
    uint16_t d2;
    uint16_t d3;
    uint8_t  d4[8];
};

// really RiffChunkHeader, but Wave* naming seems neater
struct WaveChunkHeader {            
    uint32_t        id;             // FOURCC
    uint32_t        size;           // size of chunk data
};

// first riff chunk at the start of the file
struct WaveRiffChunk {
    WaveChunkHeader header;         // id = RIFF, size = total size - sizeof(header)
    uint32_t        type;           // WAVE
};

// data size chunk for RF64 format
struct WaveDs64Chunk {
    WaveChunkHeader header;         // id = ds64, size = sizeof(WaveRf64Chunk) - sizeof(header)
    uint64_t        riff_size;      // replaces WaveRiffHeader.size when latter = -1
    uint64_t        data_size;      // replaces WaveDataHeader.size when latter = -1
    uint64_t        fact_samples;   // replaces WaveFactChunk.samples when latter = -1
    uint32_t        table_size;     // 0, spec does not say what this is for, anyone???
};

// wave format chunk based on WAVE_FORMAT_EXTENSIBLE
struct WaveFormatChunk {
    WaveChunkHeader header;         // id = FMT_, size = sizeof(WaveFormatChunk) - sizeof(header)
    uint16_t        tag;            // WAVE_FORMAT_EXTENSIBLE
    uint16_t        channels;       // number of channels
    uint32_t        sample_rate;    // samples per second per channel
    uint32_t        byte_rate;      // bytes per second per channel
    uint16_t        block_align;    // number of channels * bytes per sample
    uint16_t        bit_depth;      // bits per sample
    uint16_t        ext_size;       // sizeof(valid_bits + channel_mask + sub_format)
    uint16_t        valid_bits;     // equal to bit_depth if uncompressed
    uint32_t        channel_mask;   // speaker position mask
    WaveGuid        sub_format;     // guid of sub format eg. pcm, float, ...
};

// wave fact chunk for WAVE_FORMAT_EXTENSIBLE, required when not PCM, always used in this imp
struct WaveFactChunk {
    WaveChunkHeader header;         // id = FACT, size = sizeof(WaveFactChunk) - sizeof(header)
    uint32_t        samples;        // number of channels * total samples per channel
};

// partial data chunk, just the header, would be stupid to place ALL data into a struct
struct WaveDataChunk {
    WaveChunkHeader header;         // id = FACT, size = sizeof(data)
};

// complete RIFF header for a WAVE_FORMAT_EXTENSIBLE file
struct WaveRiffHeader {
    WaveRiffChunk   riff;
    WaveFormatChunk format;
    WaveFactChunk   fact;
    WaveDataChunk   data;
};

// complete RF64 header
struct WaveRf64Header {
    WaveRiffChunk   riff;           // id = RF64
    WaveDs64Chunk   ds64;
    WaveFormatChunk format;
    WaveFactChunk   fact;
    WaveDataChunk   data;
};

// pop previous packing alignment
#pragma pack(pop)


#endif // WAVE_H
