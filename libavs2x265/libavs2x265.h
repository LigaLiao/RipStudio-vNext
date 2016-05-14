#include <Windows.h>
#include <string>
#include <stdint.h>
#include <fstream>
#include "..\Avisynth\avisynth.h"
#include "x265.h"

#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libx265.lib")

using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::ComponentModel;
using namespace System::Text::RegularExpressions;
using namespace System::Runtime::InteropServices;
namespace libavs2x265
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

	public ref class X265Encoder
	{
	public:
		X265Encoder(String^ script, bool isfile);
		~X265Encoder(){ if (m_sc){ delete m_sc; } }
		String^ Start(String^ filename, BackgroundWorker^ bw, String^ Config);
	private:
		//bool State = false;
		AvisynthCPP* m_sc;
	};
}
