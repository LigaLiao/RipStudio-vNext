
#include "..\Avisynth\avisynth.h"
#include <fstream>
using namespace System;
using namespace System::ComponentModel;
namespace libavs2x264
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
	public ref class X264Encoder
	{
	public:
		X264Encoder(System::String^ filename);
		~X264Encoder(){ delete m_sc; }
		String^ Start(String^ filename, BackgroundWorker^ bw);
	private:
		bool State = false;
		AvisynthCPP* m_sc;
	};
}
