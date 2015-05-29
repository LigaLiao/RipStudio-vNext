// AvisynthWrapper.h

//#pragma once


#include "..\Avisynth\avisynth.h"
#using <System.Drawing.dll>

using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::Drawing::Imaging;
using namespace System::Drawing;

namespace AvisynthWrapper {
	private class AvisynthCPP
	{
	public:
		AvisynthCPP();
		~AvisynthCPP();
		PClip clip;
		IScriptEnvironment* env;
		AVSValue res;
		void avs_getframe(void *buf, int stride, int frm);
	};
	public ref class ScriptInfo{
	public:
		ScriptInfo();
		//Video
		int width;
		int height;
		int num_frames;
		int pixel_type;
		unsigned fps_numerator;
		unsigned fps_denominator;
		//Audio
		int audio_samples_per_second;
		int sample_type;
		int nchannels;
		long long int num_audio_samples;

		bool HasVideo;
		bool HasAudio;

		bool IsRGB;
		bool IsRGB24;
		bool IsRGB32;
		bool IsYUV;
		bool IsYUY2;

		bool IsYV24;
		bool IsYV16;
		bool IsYV12;
		bool IsYV411;
		bool IsY8;
	private:

	};
	public ref class Avisynth
	{

	public:
		Avisynth(System::String^ script, bool isfile);
		~Avisynth(){delete m_sc;}
		Bitmap^ GetVideoFrame(int frm);
		ScriptInfo^ GetScriptInfo();
	private:
		!Avisynth(){delete m_sc;}
		AvisynthCPP* m_sc;
	};


}
