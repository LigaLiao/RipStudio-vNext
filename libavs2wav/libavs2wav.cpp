#include "libavs2wav.h"
using namespace libavs2wav;

WavEncoder::WavEncoder(String^ script, bool isfile)
{
	m_sc = new AvisynthCPP;
	const char* Script = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(script));
	try
	{
		AVSValue arg(Script);
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		if (isfile)
		{
			m_sc->res = m_sc->env->Invoke("Import", AVSValue(&arg, 1));
		}
		else
		{
			m_sc->res = m_sc->env->Invoke("Eval", AVSValue(&arg, 1));
		}
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
	case SAMPLE_INT32:format = WAVE_FORMAT_PCM2; break;
	case SAMPLE_FLOAT:format = WAVE_FORMAT_IEEE_FLOAT2; break;
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

	try
	{
		double start = GetTickCount();
		for (uint64_t i = 0; i < target; i += count)
		{
			if (!bw->CancellationPending)
			{
				if (target - i < count)
				{
					count = (size_t) (target - i);
				}
				m_sc->clip->GetAudio(buff, i, count, m_sc->env);
				out.write(buff, sc);

				array<double>^ myArray = gcnew array<double>(2);
				myArray[0] = ((float) i / (float) m_sc->vi.audio_samples_per_second) / ((GetTickCount() - start) / 1000);
				myArray[1] = (target / m_sc->vi.audio_samples_per_second / myArray[0]);
				bw->ReportProgress(((int) i * 100 / (int) target), myArray);
			}
			else
			{
				out.flush();
				out.close();
				if (buff){delete buff;}
				if (m_sc){ delete m_sc; }

				int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
				wchar_t *wide = new wchar_t[num];
				MultiByteToWideChar(0, 0, outfile, -1, wide, num);
				DeleteFile(wide);
				return  L"Cancel";
			}
		}
		out.flush();
		out.close();
		if (buff){ delete buff; }
		if (m_sc){ delete m_sc; }

		return  L"END";
	}
	catch (System::Runtime::InteropServices::SEHException^)
	{
		out.flush();
		out.close();
		if (buff){ delete buff; }
		if (m_sc){ delete m_sc; }

		return "Source error,Avisynth cannot read!";
	}
	catch (exception err)
	{
		out.flush();
		out.close();
		if (buff){ delete buff; }
		if (m_sc){ delete m_sc; }

		return  gcnew String(err.what());
	}

}