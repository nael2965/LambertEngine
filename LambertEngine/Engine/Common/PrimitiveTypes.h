#pragma once
#include <stdint.h>



using U8	= uint8_t;
using U16	= uint16_t;
using U32	= uint32_t;
using U64	= uint64_t;

using S8	= int8_t;
using S16	= int16_t;
using S32	= int32_t;
using S64	= int64_t;

constexpr U64	U64_invalid_ID	{ 0xffff'ffff'ffff'ffffui64 };
constexpr U32	U32_invalid_ID	{ 0xffff'ffffui32 };
constexpr U16	U16_invalid_ID	{ 0xffffui16 };
constexpr U8	U8_invalid_ID	{ 0xffui8 };

constexpr S64	S64_invalid_ID	{ 0xffff'ffff'ffff'ffffui64 };
constexpr S32	S32_invalid_ID	{ 0xffff'ffffui32 };
constexpr S16	S16_invalid_ID	{ 0xffffui16 };
constexpr S8	S8_invalid_ID	{ 0xffui8 };





using F32 = float;
