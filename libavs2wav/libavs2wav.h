#include <Windows.h>
#include <fstream>
#include <string>
#include "wave.h"
#include "..\Avisynth\avisynth.h"

#pragma comment(lib, "..\\Avisynth\\avisynth.lib")

using namespace std;
using namespace System;
using namespace System::ComponentModel;

namespace libavs2wav
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
	public ref class WavEncoder
	{
	public:
		WavEncoder(String^ script, bool isfile);
		~WavEncoder(){ if (m_sc){ delete m_sc; } }
		String^ Start(String^ filename, BackgroundWorker^ bw);
	private:
		//bool State = false;
		AvisynthCPP* m_sc;
	};
}
