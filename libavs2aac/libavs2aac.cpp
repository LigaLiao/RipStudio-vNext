// 这是主 DLL 文件。
#include <Windows.h>
#include <string>
#include <fstream>
#include <stdint.h>
#include "..\Avisynth\avisynth.h"
#include "aacenc_lib.h"
#include "libavs2aac.h"
#pragma comment(lib, "..\\Avisynth\\Avisynth.lib")
#pragma comment(lib, "libfdk-aac.lib")
using namespace libavs2aac;
using namespace std;
using namespace System;
using namespace System::ComponentModel;

AacEncoder::AacEncoder(String^ filename)
{
	m_sc = new AvisynthCPP;
	const char* infile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	try {
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		m_sc->res = m_sc->env->Invoke("Import", AVSValue(infile));
		if (!m_sc->res.IsClip())
		{
			m_sc->env->ThrowError("didn't return clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
		m_sc->vi = m_sc->clip->GetVideoInfo();
		if (!m_sc->vi.HasAudio())
		{
			m_sc->env->ThrowError("No audio.");
		}
	}
	catch (AvisynthError err) {
		string s(err.msg);
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
	catch (exception eee)
	{
		string s(eee.what());
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
}

String^ AacEncoder::Start(String^ filename, BackgroundWorker^ bw, AacEncoderConfig^ Config)
{
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	std::fstream out;
	out.open(outfile, std::fstream::binary | std::fstream::out);

	uint64_t i,count, target;
	int  sample_rate, channels;
	int input_size;
	uint8_t* input_buf;
	int16_t* convert_buf;
	HANDLE_AACENCODER handle;
	CHANNEL_MODE mode;
	AACENC_InfoStruct info = { 0 };

	channels = m_sc->vi.AudioChannels();
	sample_rate = m_sc->vi.audio_samples_per_second;
	count = m_sc->vi.audio_samples_per_second;
	target = m_sc->vi.num_audio_samples;

	switch (channels) {
	case 1: mode = MODE_1;       break;
	case 2: mode = MODE_2;       break;
	case 3: mode = MODE_1_2;     break;
	case 4: mode = MODE_1_2_1;   break;
	case 5: mode = MODE_1_2_2;   break;
	case 6: mode = MODE_1_2_2_1; break;
	default:return L"Unsupported audio channels.";
	}

	if (aacEncOpen(&handle, 0, channels) != AACENC_OK) {return L"nable to open encoder.";}

	if (aacEncoder_SetParam(handle, AACENC_AOT, Config->AOT) != AACENC_OK) { return L"Unable to set the AOT."; }

	if (aacEncoder_SetParam(handle, AACENC_SAMPLERATE, sample_rate) != AACENC_OK) {return L"Unable to set the AOT.";}

	if (aacEncoder_SetParam(handle, AACENC_CHANNELMODE, mode) != AACENC_OK) {return L"nable to set the channel mode.";}

	if (aacEncoder_SetParam(handle, AACENC_CHANNELORDER, 1) != AACENC_OK) {return L"Unable to set the wav channel order.";}

	if (Config->VBR) { if (aacEncoder_SetParam(handle, AACENC_BITRATEMODE, Config->VBR) != AACENC_OK) { return L"Unable to set the VBR bitrate mode."; } }
	else { if (aacEncoder_SetParam(handle, AACENC_BITRATE, Config->Bitrate) != AACENC_OK) { return L"Unable to set the bitrate."; } }

	if (aacEncoder_SetParam(handle, AACENC_TRANSMUX, 2) != AACENC_OK) {return L"Unable to set the ADTS transmux.";}

	if (aacEncoder_SetParam(handle, AACENC_AFTERBURNER, Config->Afterburner) != AACENC_OK) { return L"Unable to set the afterburner mode."; }

	if (aacEncEncode(handle, NULL, NULL, NULL, NULL) != AACENC_OK) {return L"Unable to initialize the encoder.";}

	if (aacEncInfo(handle, &info) != AACENC_OK) {return L"Unable to get the encoder info.";}

	AACENC_BufDesc in_buf = { 0 }, out_buf = { 0 };
	AACENC_InArgs in_args = { 0 };
	AACENC_OutArgs out_args = { 0 };
	int in_identifier = IN_AUDIO_DATA;
	int in_size, in_elem_size;
	int out_identifier = OUT_BITSTREAM_DATA;
	int out_size, out_elem_size;
	int read, i2;
	void *in_ptr, *out_ptr;
	uint8_t outbuf[20480];
	AACENC_ERROR err;

	count = info.frameLength;
	input_size = channels * 2 * info.frameLength;;
	input_buf = new uint8_t[input_size];
	convert_buf = new int16_t[input_size];
	//input_buf = new uint8_t[input_size*sizeof(int8_t)];
	//convert_buf = new int16_t[input_size*sizeof(int16_t)];
	//input_buf = (uint8_t*) malloc(input_size);
	//convert_buf = (int16_t*) malloc(input_size);

	for (i = 0; i < target; i += count) {
		if (target - i < count) count = (size_t) (target - i);
		m_sc->clip->GetAudio(input_buf, i, count, m_sc->env);
		read = channels * 2 * count;
		for (i2 = 0; i2 < read / 2; i2++) {
			const uint8_t* in = &input_buf[2 * i2];
			convert_buf[i2] = in[0] | (in[1] << 8);
		}
		if (count <= 0)
		{
			in_args.numInSamples = -1;
		}
		else
		{
			in_ptr = convert_buf;
			in_size = read;
			in_elem_size = 2;

			in_args.numInSamples = read / 2;
			in_buf.numBufs = 1;
			in_buf.bufs = &in_ptr;
			in_buf.bufferIdentifiers = &in_identifier;
			in_buf.bufSizes = &in_size;
			in_buf.bufElSizes = &in_elem_size;
		}
		out_ptr = outbuf;
		out_size = sizeof(outbuf);
		out_elem_size = 1;
		out_buf.numBufs = 1;
		out_buf.bufs = &out_ptr;
		out_buf.bufferIdentifiers = &out_identifier;
		out_buf.bufSizes = &out_size;
		out_buf.bufElSizes = &out_elem_size;

		if ((err = aacEncEncode(handle, &in_buf, &out_buf, &in_args, &out_args)) != AACENC_OK) {
			if (err == AACENC_ENCODE_EOF)
			{
				break;
			}
			delete input_buf;
			delete convert_buf;
			aacEncClose(&handle);
			out.close();
			m_sc->clip->~IClip();
			m_sc->clip.~PClip();
			m_sc->res.~AVSValue();
			m_sc->env->DeleteScriptEnvironment();
			return L"Encoding failed.";
		}
		if (out_args.numOutBytes == 0)
			continue;
		out.write((char*) outbuf, out_args.numOutBytes);
		bw->ReportProgress((i * 100 / target));
	}

	delete input_buf;
	delete convert_buf;
	aacEncClose(&handle);
	out.close();
	//m_sc->clip.~PClip();
	//m_sc->res.~AVSValue();
	//m_sc->env->DeleteScriptEnvironment();
	//delete m_sc;
	m_sc->clip->~IClip();
	m_sc->clip.~PClip();
	m_sc->res.~AVSValue();
	m_sc->env->DeleteScriptEnvironment();

	bw->ReportProgress(100);

	return  L"Complete";
}
