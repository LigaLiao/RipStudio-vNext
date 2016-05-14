
#include "libavs2x265.h"
using namespace libavs2x265;

X265Encoder::X265Encoder(String^ script, bool isfile)
{
	m_sc = new AvisynthCPP;
	const char* Script = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(script));
	try
	{
		AVSValue arg(Script);
		m_sc->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
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
			m_sc->env->ThrowError("didn't return clip.");
		}
		m_sc->clip = m_sc->res.AsClip();
		m_sc->vi = m_sc->clip->GetVideoInfo();
		if (!m_sc->vi.HasVideo())
		{
			m_sc->env->ThrowError("No Video.");
		}
		else
		{
			if (!m_sc->vi.IsYV12() && !m_sc->vi.IsYV24())
			{
				if (m_sc->vi.IsRGB())
				{
					m_sc->res = m_sc->env->Invoke("ConvertToYV24", AVSValue(&m_sc->res, 1));
					m_sc->clip = m_sc->res.AsClip();
					m_sc->vi = m_sc->clip->GetVideoInfo();
				}
				else
				{
					m_sc->res = m_sc->env->Invoke("ConvertToYV12", AVSValue(&m_sc->res, 1));
					m_sc->clip = m_sc->res.AsClip();
					m_sc->vi = m_sc->clip->GetVideoInfo();
				}
			}
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

String^ X265Encoder::Start(String^ filename, BackgroundWorker^ bw, String^ config)
{
	const char* outfile = (const char*) (void *) (System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename));
	std::fstream OutFile;
	OutFile.open(outfile, std::fstream::binary | std::fstream::out);

	const x265_api* api;

	x265_nal *Nals = NULL;
	uint32_t iNal = 0;
	x265_param* param ;
	//x265_encoder* Encoder = NULL;
	x265_encoder *encoder = NULL;
	x265_picture *pic_in;
	uint32_t outFrameCount = 0;
	uint64_t totalbytes = 0;


	char* preset =NULL;
	char* tune =NULL;
	char*  profile = NULL;
	char*  depth ="0";
	//应用设置
	array<String^>^ ConfigArray = gcnew array<String^>(48);
	array<String^>^ Config = gcnew array<String^>(2);
	array<String^>^ SplitString = gcnew array<String^>(1);
	try
	{
		SplitString[0] = "--";
		ConfigArray = config->Split(SplitString, StringSplitOptions::RemoveEmptyEntries);
		for each (String^ var in ConfigArray)
		{
			if (!String::IsNullOrWhiteSpace(var))
			{
				Config = var->Split(' ');
				if (Config[0]->ToLower() == "preset")
				{
					preset = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				else if (Config[0]->ToLower() == "tune")
				{
					tune = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				else if (Config[0]->ToLower() == "profile")
				{
					profile = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				}
				//else if (Config[0]->ToLower() == "output-depth")
				//{
				//	depth = (char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->Replace(" ", "")->ToLower()));
				//}
			}
			Config[0] = Config[1] = "";
		}
		api = x265_api_get(atoi(depth));
		if (!api)
		{
			return "depth 设置错误";
			//api = x265_api_get(0);
		}

		param = api->param_alloc();
		if (!param)
		{
			return "Param Alloc failed!";
		}

		api->param_default(param);

		if (api->param_default_preset(param, preset, tune) < 0)
		{
			return "preset or tune unrecognized\n";
		}

		param->vui.sarHeight = param->vui.sarWidth = 1;
		param->bRepeatHeaders = 1;//write sps,pps before keyframe
		param->totalFrames = m_sc->vi.num_frames;
		param->sourceWidth = m_sc->vi.width;
		param->sourceHeight = m_sc->vi.height;
		param->fpsNum = m_sc->vi.fps_numerator;
		param->fpsDenom = m_sc->vi.fps_denominator;
		if (m_sc->vi.IsYV24())
		{
			param->internalCsp = X265_CSP_I444;
		}
		else
		{
			param->internalCsp = X265_CSP_I420;
		}
		//Param->internalBitDepth = 10;

		for each (String^ var in ConfigArray)
		{
			if (!String::IsNullOrWhiteSpace(var))
			{
				Config = var->Split(' ');
				if (Config[0]->ToLower() == "preset")
				{
				}
				else if (Config[0]->ToLower() == "tune")
				{
				}
				else if (Config[0]->ToLower() == "profile")
				{
				}
				//else if (Config[0]->ToLower() == "output-depth")
				//{
				//}
				else
				{
					switch (api->param_parse(param, (const char*) (void *) (Marshal::StringToHGlobalAnsi(Config[0]->ToLower())), (const char*) (void *) (Marshal::StringToHGlobalAnsi(Config[1]->ToLower()))))
					{
					case X265_PARAM_BAD_NAME:
						return  L"BAD_NAME:" + Config[0] + " " + Config[1];
						break;
					case X265_PARAM_BAD_VALUE:
						return  L"BAD_VALUE:" + Config[0] + " " + Config[1];
						break;
					}
				}
			}
		}



		if (api->param_apply_profile(param, profile) < 0)
		{
			return "profile Value Error";
		}
		//应用设置结束


		//Encoder = x265_encoder_open(Param);
		//if (Encoder == NULL){ return L"x265_encoder_open err"; }
		//x265_encoder_parameters(Encoder, Param);

		encoder = api->encoder_open(param);
		if (!encoder)
		{
			return "failed to open encoder\n";

			api->param_free(param);
			api->cleanup();
			exit(2);
		}

		if (!param->bRepeatHeaders)
		{
			if (api->encoder_headers(encoder, &Nals, &iNal) < 0)
			{
				return "Failure generating stream headers.";
			}
			else
			{
				for (uint32_t i = 0; i < iNal; i++)
				{
					OutFile.write((const char*) Nals->payload, Nals->sizeBytes);
					totalbytes += Nals->sizeBytes;
					Nals++;
				}
			}
		}

		


		pic_in = api->picture_alloc();
		pic_in->colorSpace = param->internalCsp;
		pic_in->sliceType = X265_TYPE_AUTO;

		api->picture_init(param, pic_in);

		////pic_in->planes[0] = new char[Param->sourceWidth * Param->sourceHeight * 3 / 2];
		//pic_in->planes[1] = (char*) pic_in->planes[0] + pic_in->stride[0] * Param->sourceHeight;
		//pic_in->planes[2] = (char*) pic_in->planes[1] + pic_in->stride[1] * (Param->sourceHeight >> x265_cli_csps[pic_in->colorSpace].height[1]);
		//pic_in->stride[0] = Param->sourceWidth;
		//pic_in->stride[1] = pic_in->stride[0] >> x265_cli_csps[pic_in->colorSpace].width[1];
		//pic_in->stride[2] = pic_in->stride[0] >> x265_cli_csps[pic_in->colorSpace].width[2];


		double start = GetTickCount();
		array<double>^ myArray = gcnew array<double>(3);
		for (int i_frame = 0; i_frame < m_sc->vi.num_frames; i_frame++)
		{
			if (!bw->CancellationPending)
			{
				/* Read input frame */
				PVideoFrame f = m_sc->clip->GetFrame(i_frame, m_sc->env);
				pic_in->planes[0] = (void *) f->GetReadPtr(PLANAR_Y);
				pic_in->planes[1] = (void *) f->GetReadPtr(PLANAR_U);
				pic_in->planes[2] = (void *) f->GetReadPtr(PLANAR_V);
				pic_in->stride[0] = f->GetPitch(PLANAR_Y);
				pic_in->stride[1] = f->GetPitch(PLANAR_U);
				pic_in->stride[2] = f->GetPitch(PLANAR_V);
				pic_in->pts = pic_in->poc = i_frame;
				int numEncoded = api->encoder_encode(encoder, &Nals, &iNal, pic_in, NULL);
				if (numEncoded >= 0)
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

				myArray[0] = (double) outFrameCount / ((GetTickCount() - start) / 1000);
				myArray[1] = ((double) param->totalFrames - (double) outFrameCount) / myArray[0];
				myArray[2] = 0.008f * totalbytes * ((float) param->fpsNum / (float) param->fpsDenom) / ((float) outFrameCount);
				bw->ReportProgress((int)(outFrameCount * 100 / param->totalFrames), myArray);
			}
			else
			{
				OutFile.flush();
				OutFile.close();
				api->encoder_close(encoder);
				api->cleanup();
				api->param_free(param);
				api->picture_free(pic_in);

				if (m_sc){ delete m_sc; }


				int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
				wchar_t *wide = new wchar_t[num];
				MultiByteToWideChar(0, 0, outfile, -1, wide, num);
				DeleteFile(wide);
				return  L"Cancel";
			}
		}
		while (int numEncoded = x265_encoder_encode(encoder, &Nals, &iNal, NULL, NULL))
		{
			if (!bw->CancellationPending)
			{
				//int numEncoded = x265_encoder_encode(Encoder, &Nals, &iNal, NULL, NULL);
				if (numEncoded < 0)
				{
					break;
					//return  L"已完成编码";
				}
				for (uint32_t i = 0; i < iNal; i++)
				{
					OutFile.write((const char*) Nals[i].payload, Nals[i].sizeBytes);
					totalbytes += Nals[i].sizeBytes;
				}
				outFrameCount += numEncoded;

				myArray[0] = (double) outFrameCount / ((GetTickCount() - start) / 1000);
				myArray[1] = ((double) param->totalFrames - (double) outFrameCount) / myArray[0];
				myArray[2] = 0.008f * totalbytes * (param->fpsNum / param->fpsDenom) / ((float) outFrameCount);
				bw->ReportProgress((int)(outFrameCount * 100 / param->totalFrames), myArray);
			}
			else
			{
				OutFile.flush();
				OutFile.close();
				api->encoder_close(encoder);
				api->cleanup();
				api->param_free(param);
				api->picture_free(pic_in);
				if (m_sc){ delete m_sc; }


				int num = MultiByteToWideChar(0, 0, outfile, -1, NULL, 0);
				wchar_t *wide = new wchar_t[num];
				MultiByteToWideChar(0, 0, outfile, -1, wide, num);
				DeleteFile(wide);
				return  L"Cancel";
			}
		}
		OutFile.flush();
		OutFile.close();
		api->encoder_close(encoder);
		api->cleanup();
		api->param_free(param);
		api->picture_free(pic_in);
		if (m_sc){ delete m_sc; }


	}
	catch (AvisynthError err) {
		OutFile.flush();
		OutFile.close();
		api->encoder_close(encoder);
		api->cleanup();
		api->param_free(param);
		api->picture_free(pic_in);
		if (m_sc){ delete m_sc; }

		return  gcnew String(err.msg);
	}
	catch (exception err)
	{
		OutFile.flush();
		OutFile.close();
		api->encoder_close(encoder);
		api->cleanup();
		api->param_free(param);
		api->picture_free(pic_in);
		if (m_sc){ delete m_sc; }

		return  gcnew String(err.what());
	}
	return  L"END";

}