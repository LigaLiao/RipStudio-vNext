#include <Windows.h>
#include <string>
#include <stdint.h>
#include <fstream>
#include "libavs2x265.h"
#include "..\Avisynth\avisynth.h"
#include "x265.h"
#include <sys/types.h>
#include <sys/timeb.h>
#pragma comment(lib, "..\\Avisynth\\avisynth.lib")
#pragma comment(lib, "libx265.lib")
using namespace libavs2x265;
using namespace std;
using namespace System;
using namespace System::ComponentModel;
X265Encoder::X265Encoder(String^ filename)
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

String^ X265Encoder::Start(String^ filename, BackgroundWorker^ bw, X265EncoderConfig^ Config)
{
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	std::fstream OutFile;
	OutFile.open(outfile, std::fstream::binary | std::fstream::out);
	struct timeb tb2;
	ftime(&tb2);
	int64_t startTime = ((int64_t) tb2.time * 1000 + (int64_t) tb2.millitm) * 1000;
	try 
	{
		int depth = 8;

		char *buff = NULL;
		x265_nal *Nals = NULL;
		uint32_t iNal = 0;
		x265_param* Param = NULL;
		x265_encoder* Encoder = NULL;
		x265_picture pic_orig, pic_out;
		x265_picture *pic_in = &pic_orig;
		uint32_t outFrameCount = 0;
		uint64_t totalbytes;
		Param = x265_param_alloc();
		x265_param_default(Param);
		Param->bRepeatHeaders = 1;//write sps,pps before keyframe
		Param->sourceWidth = m_sc->vi.width;
		Param->sourceHeight = m_sc->vi.height;
		Param->fpsNum = m_sc->vi.fps_numerator;
		Param->fpsDenom = m_sc->vi.fps_denominator;
		Param->rc.rateControlMode = X265_RC_CRF;
		Param->rc.rfConstant = 23;
		Encoder = x265_encoder_open(Param);
		if (Encoder == NULL){
			return L"x265_encoder_open err";
		}
		x265_picture_init(Param, pic_in);
		//uint32_t pixelbytes = depth > 8 ? 2 : 1;
		//buff = (char *) malloc(pParam->sourceWidth * pParam->sourceHeight * 3 / 2);
		buff = new char[Param->sourceWidth * Param->sourceHeight * 3 / 2];
		pic_orig.colorSpace = X265_CSP_I420;
		pic_orig.planes[0] = buff;
		pic_orig.planes[1] = (char*) pic_orig.planes[0] + pic_orig.stride[0] * Param->sourceHeight;
		pic_orig.planes[2] = (char*) pic_orig.planes[1] + pic_orig.stride[1] * (Param->sourceHeight >> x265_cli_csps[X265_CSP_I420].height[1]);
		pic_orig.stride[0] = Param->sourceWidth* (depth > 8 ? 2 : 1);
		pic_orig.stride[1] = pic_orig.stride[0] >> x265_cli_csps[X265_CSP_I420].width[1];
		pic_orig.stride[2] = pic_orig.stride[0] >> x265_cli_csps[X265_CSP_I420].width[2];

		for (uint64_t i_frame = 0; i_frame <m_sc->vi.num_frames; i_frame++)
		{
			/* Read input frame */
			PVideoFrame f = m_sc->clip->GetFrame(i_frame, m_sc->env);
			pic_orig.planes[0] = (void *) f->GetReadPtr(PLANAR_Y);
			pic_orig.planes[1] = (void *) f->GetReadPtr(PLANAR_U);
			pic_orig.planes[2] = (void *) f->GetReadPtr(PLANAR_V);

			int numEncoded = x265_encoder_encode(Encoder, &Nals, &iNal, pic_in, NULL);
		    if(numEncoded>=0)
			{
				for (uint32_t i = 0; i < iNal; i++)
				{
					OutFile.write((const char*) Nals[i].payload, Nals[i].sizeBytes);
					totalbytes += Nals[i].sizeBytes;
				}
			}
			else
			{
				break;
			}
			outFrameCount += numEncoded;
			//int64_t time = x265_mdate();

			//if (!bProgress || !frameNum || (prevUpdateTime && time - prevUpdateTime < UPDATE_INTERVAL))
			//	return;

			struct timeb tb;
			ftime(&tb);
			int64_t time = ((int64_t) tb.time * 1000 + (int64_t) tb.millitm) * 1000;
			
			int64_t elapsed = time - startTime;
			double fps = elapsed > 0 ? outFrameCount * 1000000. / elapsed : 0;
			float bitrate = 0.008f * totalbytes * (Param->fpsNum / Param->fpsDenom) / ((float) outFrameCount);

			//int eta = (int) (elapsed * (m_sc->vi.num_frames - outFrameCount) / ((int64_t) outFrameCount * 1000000));
			//	sprintf("x265 [%.1f%%] %d/%d frames, %.2f fps, %.2f kb/s, eta %d:%02d:%02d",
			//		100. * outFrameCount / m_sc->vi.num_frames, outFrameCount, m_sc->vi.num_frames, fps, bitrate,
			//		eta / 3600, (eta / 60) % 60, eta % 60);


			bw->ReportProgress((outFrameCount * 100 / m_sc->vi.num_frames), fps);
		}
		while (1)
		{
			int numEncoded = x265_encoder_encode(Encoder, &Nals, &iNal, NULL, NULL);
			if (numEncoded < 0)
			{
				break;
			}
			for (uint32_t i = 0; i < iNal; i++)
			{
				OutFile.write((const char*) Nals[i].payload, Nals[i].sizeBytes);
				totalbytes += Nals[i].sizeBytes;
			}
			outFrameCount += numEncoded;
			struct timeb tb;
			ftime(&tb);
			int64_t time = ((int64_t) tb.time * 1000 + (int64_t) tb.millitm) * 1000;

			int64_t elapsed = time - startTime;
			double fps = elapsed > 0 ? outFrameCount * 1000000. / elapsed : 0;
			float bitrate = 0.008f * totalbytes * (Param->fpsNum / Param->fpsDenom) / ((float) outFrameCount);

			bw->ReportProgress((outFrameCount * 100 / m_sc->vi.num_frames), fps);
		}
		x265_encoder_close(Encoder);
		x265_picture_free(pic_in);
		x265_param_free(Param);
		//free(buff);
		delete buff;
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