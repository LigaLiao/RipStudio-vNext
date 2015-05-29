#include "..\Avisynth\avisynth.h"
#include <fstream>
using namespace System;
using namespace System::ComponentModel;

namespace libavs2flac 
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
		FlacEncoder(System::String^ filename);
		~FlacEncoder(){ delete m_sc; }
		String^ Start(String^ filename, BackgroundWorker^ bw,FlacEncoderConfig^ Config);
	private:
		bool State = false;
		AvisynthCPP* m_sc;
	};
}
