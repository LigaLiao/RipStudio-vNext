#include "..\Avisynth\avisynth.h"
#include <fstream>
using namespace System;
using namespace System::ComponentModel;

namespace libavs2aac
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
		AacEncoder(System::String^ filename);
		~AacEncoder(){ delete m_sc; }
		String^ Start(String^ filename, BackgroundWorker^ bw, AacEncoderConfig^ Config);
	private:
		bool State = false;
		AvisynthCPP* m_sc;
	};
}
