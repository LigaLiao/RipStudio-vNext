#include <Windows.h>
#include <string>
#include "..\Avisynth\avisynth.h"
#using <System.Drawing.dll>
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")

using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::Drawing::Imaging;
using namespace System::Drawing;

namespace AvisynthWrapper 
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

			if (env != NULL){env->DeleteScriptEnvironment();}
		};
		PClip clip;
		IScriptEnvironment* env;
		AVSValue res;
		void avs_getframe(void *buf, int stride, int frm);
	};
	public ref class ScriptInfo{
	public:
		ScriptInfo(){};
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
	};
	public ref class Avisynth
	{
	public:
		Avisynth(System::String^ script, bool isfile);
		~Avisynth(){delete m_sc;}
		void FreeAvisynth(){ delete this; };
		Bitmap^ GetVideoFrame(int frm);
		ScriptInfo^ GetScriptInfo();
	private:
		AvisynthCPP* m_sc;
	};
	//public ref class TryAvisynth
	//{
	//public:
	//	TryAvisynth(System::String^ script, bool isfile)
	//	{
	//		m_sc = new AvisynthCPP();
	//		const char* Script = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(script));
	//		try {
	//			m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
	//			AVSValue arg(Script);
	//			if (isfile)
	//			{
	//				m_sc->res = m_sc->env->Invoke("Import", AVSValue(&arg, 1));
	//			}
	//			else
	//			{
	//				m_sc->res = m_sc->env->Invoke("Eval", AVSValue(&arg, 1));
	//			}
	//			if (!m_sc->res.IsClip())
	//			{
	//				m_sc->env->ThrowError("didn't return a clip.");
	//			}
	//		}
	//		catch (AvisynthError err)
	//		{
	//			string s(err.msg);
	//			String^ str3 = gcnew String(s.c_str());
	//			throw gcnew System::Exception(str3);
	//		}
	//		catch (exception eee)
	//		{
	//			string s(eee.what());
	//			String^ str3 = gcnew String(s.c_str());
	//			throw gcnew System::Exception(str3);
	//		}
	//	   finally
	//	   {
	//		   delete this;
	//	   }
	//	};
	//	~TryAvisynth(){ delete m_sc; }
	//	//void FreeTryAvisynth(){ delete this; };
	//private:
	//	AvisynthCPP* m_sc;
	//};
}
