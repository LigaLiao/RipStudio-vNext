#include <Windows.h>
#include <string>
#include <stdint.h>
#include <fstream>
#include "libavs2x264.h"
#include "..\Avisynth\avisynth.h"
extern "C"
{
#include "x264.h"
};
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libx264.lib")
using namespace libavs2x264;
using namespace std;
using namespace System;
using namespace System::ComponentModel;
X264Encoder::X264Encoder(String^ filename)
{
	m_sc = new AvisynthCPP;
	const char* infile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	try {
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		m_sc->res = m_sc->env->Invoke("Import", AVSValue(infile));
		if (!m_sc->res.IsClip())
		{
			m_sc->env->ThrowError("didn't return clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
		m_sc->vi = m_sc->clip->GetVideoInfo();
		if (!m_sc->vi.HasVideo())
		{
			m_sc->env->ThrowError("No Video.");
		}
	}
	catch (AvisynthError err) {
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

String^ X264Encoder::Start(String^ filename, BackgroundWorker^ bw)
{
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	std::fstream OutFile;
	OutFile.open(outfile, std::fstream::binary | std::fstream::out);

	x264_param_t param;
	x264_picture_t pic;
	x264_picture_t pic_out;
	x264_t *h;
	int i_frame_size;
	x264_nal_t *nal;
	int i_nal;
	try 
	{
	if (x264_param_default_preset(&param, "medium", NULL) < 0){ exit(1); }

	// Configure non-default params
	param.i_csp = X264_CSP_I420;
	param.i_width = m_sc->vi.width;
	param.i_height = m_sc->vi.height;
	param.b_vfr_input = 0;
	param.b_repeat_headers = 1;
	param.b_annexb = 1;
	param.i_fps_num = m_sc->vi.fps_numerator;
	param.i_fps_den = m_sc->vi.fps_denominator;

	//Apply profile restrictions.
	if (x264_param_apply_profile(&param, "high") < 0){ exit(2); }
	if (x264_picture_alloc(&pic, param.i_csp, param.i_width, param.i_height) < 0){ exit(3); }

	if (x264_encoder_open(&param)==NULL)
	{
		return L"Open The Encoder Failure!";
	} 

	pic.img.plane[0] = (uint8_t *) malloc(param.i_width*param.i_height * 3 / 2);
	pic.img.plane[1] = pic.img.plane[0] + param.i_width * param.i_height;
	pic.img.plane[2] = pic.img.plane[1] + param.i_width * param.i_height / 4;

	for (uint64_t i_frame = 0; i_frame <m_sc->vi.num_frames; i_frame++)
	{
		/* Read input frame */
		PVideoFrame f = m_sc->clip->GetFrame(i_frame, m_sc->env);
		pic.img.plane[0] = (uint8_t *) f->GetReadPtr(PLANAR_Y);
		pic.img.plane[1] = (uint8_t *) f->GetReadPtr(PLANAR_U);
		pic.img.plane[2] = (uint8_t *) f->GetReadPtr(PLANAR_V);

		pic.i_pts = i_frame;
		i_frame_size = x264_encoder_encode(h, &nal, &i_nal, &pic, &pic_out);
		if (i_frame_size < 0)
		{
			return  L"±àÂëÖÐÍ¾Ê§°Ü";
		}
		else if (i_frame_size)
		{
			OutFile.write((const char*) nal->p_payload, i_frame_size);
		}
		bw->ReportProgress((i_frame * 100 / m_sc->vi.num_frames));
	}
	while (x264_encoder_delayed_frames(h))
	{
		i_frame_size = x264_encoder_encode(h, &nal, &i_nal, NULL, &pic_out);
		if (i_frame_size < 0)
		{
			exit(5);
		}
		else if (i_frame_size)
		{
			OutFile.write((const char*) nal->p_payload, i_frame_size);
		}
		//printf("Flush 1 Ö¡\n");
	}
	x264_encoder_close(h);
	OutFile.flush();
	OutFile.close();



	//m_sc->clip.~PClip();
	//m_sc->res.~AVSValue();
	//m_sc->env->DeleteScriptEnvironment();
	//delete m_sc;
	m_sc->clip->~IClip();
	m_sc->clip.~PClip();
	m_sc->res.~AVSValue();
	m_sc->env->DeleteScriptEnvironment();
}
catch (AvisynthError err) {
	//fprintf(stderr, "\nAvisynth error:\n%s\n", err.msg);
	return  gcnew String(err.msg);
}
	bw->ReportProgress(100);
	return  L"ÒÑÍê³É±àÂë";

}