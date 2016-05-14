#include <Windows.h>
#include <string>
#include <stdint.h>
#include <fstream>
#include "..\Avisynth\avisynth.h"
extern "C"
{
#include "x264.h"
};
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libx264.dll.lib")

using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::ComponentModel;
using namespace System::Runtime::InteropServices;

namespace libavs2x264
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

	public ref class X264Encoder
	{
	public:
		X264Encoder(String^ script, bool isfile);
		~X264Encoder(){ if (m_sc){ delete m_sc; } }
		String^ Start(String^ filename, BackgroundWorker^ bw, String^ config);
	private:
		//bool State = false;
		AvisynthCPP* m_sc;
	};
}
