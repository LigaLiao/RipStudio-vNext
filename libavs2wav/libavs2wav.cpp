#include <Windows.h>
#include <string>
#include "wave.h"
#include "..\Avisynth\avisynth.h"
#include "libavs2wav.h"
#include <fstream>
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
using namespace libavs2wav;
using namespace std;
using namespace System;
//using namespace System::IO;
using namespace System::ComponentModel;
WavEncoder::WavEncoder(String^ filename)
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

String^ WavEncoder::Start(String^ filename, BackgroundWorker^ bw)
{
	WaveFormatType format;
	switch (m_sc->vi.sample_type)
	{
	case SAMPLE_INT8:
	case SAMPLE_INT16:
	case SAMPLE_INT24:
	case SAMPLE_INT32:format = WAVE_FORMAT_PCM; break;
	case SAMPLE_FLOAT:format = WAVE_FORMAT_IEEE_FLOAT; break;
	default:
		return  L"audio format unknown trying PCM.";
	}
	std::fstream out;
	size_t size = m_sc->vi.BytesPerChannelSample() * m_sc->vi.nchannels, count = m_sc->vi.audio_samples_per_second;
	uint64_t  target = m_sc->vi.num_audio_samples;
	uint64_t sc = count * size;
	char *buff = new char[sc];
	WaveRiffHeader *header = wave_create_riff_header(format, m_sc->vi.nchannels, m_sc->vi.audio_samples_per_second, m_sc->vi.BytesPerChannelSample(), m_sc->vi.num_audio_samples);
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	out.open(outfile, std::fstream::binary | std::fstream::out);
	out.write((char*) header, sizeof(*header));
	out.flush();
	delete header;
	uint64_t lingshi = 0;
	for (uint64_t i = 0; i < target; i += count)
	{
		if (!bw->CancellationPending)
		{
			if (target - i < count) count = (size_t) (target - i);
			m_sc->clip->GetAudio(buff, i, count, m_sc->env);
			out.write(buff, sc);
			lingshi += count;
			bw->ReportProgress((i * 100 / target));
		}
		else
		{
			out.flush();
			out.close();
			delete buff;
			//m_sc->res.~AVSValue();
			//m_sc->clip.~PClip();
			//m_sc->env->DeleteScriptEnvironment();
			bw->ReportProgress(0);
			int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
			wchar_t *wide = new wchar_t[num];
			MultiByteToWideChar(0, 0, outfile, -1, wide, num);

			if (DeleteFile(wide))
			{
				return  L"已中止,并已删除未完成的文件";
			}
			else
			{
				return  L"已中止,删除未完成的文件失败";
			}
		}
	}
	out.flush();
	out.close();
	delete buff;

	//m_sc->clip.~PClip();
	//m_sc->res.~AVSValue();
	//m_sc->env->DeleteScriptEnvironment();
	//delete m_sc;
	m_sc->clip->~IClip();
	m_sc->clip.~PClip();
	m_sc->res.~AVSValue();
	m_sc->env->DeleteScriptEnvironment();

	bw->ReportProgress(100);
	return  L"已完成编码";

}