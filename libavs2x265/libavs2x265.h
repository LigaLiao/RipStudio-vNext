#include "..\Avisynth\avisynth.h"
#include <fstream>
using namespace System;
using namespace System::ComponentModel;
namespace libavs2x265
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
	public ref class X265EncoderConfig
	{
	public:
		X265EncoderConfig();
		~X265EncoderConfig();

	private:

	};

	X265EncoderConfig::X265EncoderConfig()
	{
	}

	X265EncoderConfig::~X265EncoderConfig()
	{
	}
	public ref class X265Encoder
	{
	public:
		X265Encoder(System::String^ filename);
		~X265Encoder(){ delete m_sc; }
		String^ Start(String^ filename, BackgroundWorker^ bw, X265EncoderConfig^ Config);
	private:
		bool State = false;
		AvisynthCPP* m_sc;
	};
}
