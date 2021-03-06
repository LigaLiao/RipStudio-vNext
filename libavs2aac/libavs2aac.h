#include <Windows.h>
#include <string>
#include <fstream>
#include <stdint.h>
#include "..\Avisynth\avisynth.h"
#include "aacenc_lib.h"

#pragma comment(lib, "..\\Avisynth\\Avisynth.lib")
#pragma comment(lib, "libfdk-aac.lib")

using namespace std;
using namespace System;
using namespace System::ComponentModel;

namespace libavs2aac
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
	public ref class AacEncoderConfig
	{
	private:
		int bitrate = 3200;
		int	vbr = 0;
		int	aot = 2;
		int	afterburner = 1;
	public:
		property int Bitrate
		{
			int get(){ return bitrate; }
			void set(int value) { bitrate = value; }
		}
		property int VBR
		{
			int get(){ return vbr; }
			void set(int value) { vbr = value; }
		}
		property int AOT
		{
			int get(){ return aot; }
			void set(int value) { aot = value; }
		}
		property int Afterburner
		{
			int get(){ return afterburner; }
			void set(int value) { afterburner = value; }
		}

	};
	public ref class AacEncoder
	{
	public:
		AacEncoder(System::String^ script, bool isfile);
		~AacEncoder(){ if (m_sc){ delete m_sc; } }
		String^ Start(String^ filename, BackgroundWorker^ bw, AacEncoderConfig^ Config);
	private:
		AvisynthCPP* m_sc;
	};
}
