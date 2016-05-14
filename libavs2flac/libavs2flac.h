#include <Windows.h>
#include <string>
#include <fstream>
#include "..\Avisynth\avisynth.h"
#include "FLAC/metadata.h"
#include "FLAC/stream_encoder.h"
#include "share/compat.h"

#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libFLAC_dynamic.lib")

using namespace std;
using namespace System;
using namespace System::ComponentModel;

namespace libavs2flac 
{
	private class AvisynthCPP
	{
	public:
		AvisynthCPP(){};
		~AvisynthCPP()
		{
			if (res.IsClip())
			{
				res.~AVSValue();
				clip.~PClip();
			}
			if (env != NULL){ env->DeleteScriptEnvironment(); }
		};
		PClip clip;
		IScriptEnvironment* env;
		AVSValue res;
		VideoInfo vi;
	};
	public ref class FlacEncoderConfig
	{
	private:
		int  levels = 5;
	public:
		property int  Levels
		{
			int get(){ return levels; }
			void set(int value) { levels = value; }
		}
	};
	public ref class FlacEncoder
	{
	public:
		FlacEncoder(System::String^ script, bool isfile);
		~FlacEncoder(){ if (m_sc){ delete m_sc; } }
		String^ Start(String^ filename, BackgroundWorker^ bw,FlacEncoderConfig^ Config);
	private:
		//bool State = false;
		AvisynthCPP* m_sc;
	};
}
