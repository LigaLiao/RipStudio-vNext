#include "AvisynthWrapper.h"
using namespace AvisynthWrapper;

Bitmap^ Avisynth::GetVideoFrame(int frm)
{
	VideoInfo vi = m_sc->clip->GetVideoInfo();
	System::Drawing::Bitmap^ bmp = gcnew System::Drawing::Bitmap(vi.width, vi.height, System::Drawing::Imaging::PixelFormat::Format24bppRgb);
	System::Drawing::Rectangle rect = System::Drawing::Rectangle(0, 0, bmp->Width, bmp->Height);
	BitmapData ^ bmpData = bmp->LockBits(rect, ImageLockMode::ReadWrite, bmp->PixelFormat);
	try
	{
		IntPtr ptr = bmpData->Scan0;
		m_sc->avs_getframe(ptr.ToPointer(), bmpData->Stride, frm);
		bmp->UnlockBits(bmpData);
		return bmp;
	}
	catch (AvisynthError err)
	{
		string s(err.msg);
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
}
ScriptInfo^ Avisynth::GetScriptInfo()
{
	VideoInfo vi = m_sc->clip->GetVideoInfo();
	ScriptInfo^ si = gcnew ScriptInfo();
	si->height = vi.height;
	si->width = vi.width;
	si->fps_denominator = vi.fps_denominator;
	si->fps_numerator = vi.fps_numerator;
	si->audio_samples_per_second = vi.audio_samples_per_second;
	si->num_frames = vi.num_frames;
	si->fps_numerator = vi.fps_numerator;
	si->pixel_type = vi.pixel_type;
	si->sample_type = vi.sample_type;

	si->nchannels = vi.AudioChannels();
	si->sample_type = vi.sample_type;
	si->audio_samples_per_second = vi.audio_samples_per_second;
	si->num_audio_samples = vi.num_audio_samples;

	si->HasVideo = vi.HasVideo();
	si->HasAudio = vi.HasAudio();

	si->IsRGB = vi.IsRGB();
	si->IsRGB24 = vi.IsRGB24();
	si->IsRGB32 = vi.IsRGB32();

	si->IsYUV = vi.IsYUV();
	si->IsYUY2 = vi.IsYUY2();
	si->IsYV24 = vi.IsYV24();
	si->IsYV16 = vi.IsYV16();
	si->IsYV12 = vi.IsYV12();
	si->IsYV411 = vi.IsYV411();
	si->IsY8 = vi.IsY8();
	return si;
}
Avisynth::Avisynth(System::String^ script, bool isfile)
{
	m_sc = new AvisynthCPP();
	const char* Script = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(script));
	try {
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		AVSValue arg(Script);
		if (isfile)
		{
			m_sc->res = m_sc->env->Invoke("Import", AVSValue(&arg, 1));
		}
		else
		{
			m_sc->res = m_sc->env->Invoke("Eval", AVSValue(&arg, 1));
		}
		if (!m_sc->res.IsClip()) 
		{
			m_sc->env->ThrowError("didn't return a clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
			if (!m_sc->clip->GetVideoInfo().IsRGB24())
			{
				m_sc->res = m_sc->env->Invoke("ConvertToRGB24", AVSValue(&m_sc->res, 1));
				m_sc->clip = m_sc->res.AsClip();
			}
		//if (!m_sc->clip->GetVideoInfo().HasVideo())
		//{
		//	m_sc->env->ThrowError("No Video.");
		//}
		//else
		//{
		//	if (!m_sc->clip->GetVideoInfo().IsRGB24())
		//	{
		//		m_sc->res = m_sc->env->Invoke("ConvertToRGB24", AVSValue(&m_sc->res, 1));
		//		m_sc->clip = m_sc->res.AsClip();
		//	}
		//}
	}
	catch (AvisynthError err)
	{
		string s(err.msg);
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
	catch (exception eee)
	{
		string s(eee.what());
		String^ str3 = gcnew String(s.c_str());
		throw gcnew System::Exception(str3);
	}
}
void AvisynthCPP::avs_getframe(void *buf, int stride, int frm)
{
	PVideoFrame f = clip->GetFrame(frm, env);
	if (buf && stride)
	{
		env->BitBlt((BYTE*) buf, stride, f->GetReadPtr(), f->GetPitch(), f->GetRowSize(), f->GetHeight());
	}
}
