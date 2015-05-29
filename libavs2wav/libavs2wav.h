
#include "..\Avisynth\avisynth.h"
#include <fstream>
using namespace System;
using namespace System::ComponentModel;
namespace libavs2wav
{
	private class AvisynthCPP
	{
	public:
		AvisynthCPP(){};

		PClip clip;
		IScriptEnvironment* env;
		AVSValue res;
		VideoInfo vi;
	};
	public ref class WavEncoder
	{
	public:
		WavEncoder(System::String^ filename);
		~WavEncoder(){ delete m_sc; }
		String^ Start(String^ filename, BackgroundWorker^ bw);
	private:
		bool State = false;
		AvisynthCPP* m_sc;
	};
}
