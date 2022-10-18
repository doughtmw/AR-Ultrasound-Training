#pragma once
#include <opencv2/core.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/aruco.hpp>
#include <map>
#include "windowsnumerics.h"

#define DllExport   __declspec( dllexport )

extern "C"
{
	DllExport bool InitializeDll(
		float mS,
		float fx, float fy,
		float px, float py,
		float rx, float ry, float rz,
		float tx, float ty,
		int imgWidth, int imgHeight);
	
	DllExport bool StartMarkerTracking(
		unsigned char* texData,
		int width, int height);
	
	DllExport int GetDetectedMarkerCount();
	
	DllExport bool GetDetectedMarkerIds(
		unsigned int* detectedIds, 
		int size);
	
	DllExport bool GetDetectedMarkerPose(
		int detectedId,
		float* position,
		float* rotation,
		float* cameraToWorldUnity);
}

struct DetectedMarker
{
	int32_t markerId;

	Windows::Foundation::Numerics::float3 point;
	Windows::Foundation::Numerics::float3 rotation;

	// Image position
	int x;
	int y;
};

class OpenCvDll
{
public:
	OpenCvDll(
		float mS,
		float fx, float fy,
		float px, float py,
		float rx, float ry, float rz,
		float tx, float ty,
		int imgWidth, int imgHeight);
	~OpenCvDll();
	std::map<int32_t, DetectedMarker> ArUcoMarkerDetection(cv::Mat mat);

	std::map<int, DetectedMarker> observedMarkers;
	int width;
	int height;
	std::vector<cv::Vec3d> rVecs;
	std::vector<cv::Vec3d> tVecs;

private:

	float _markerSize;
	cv::Mat _cameraMatrix;
	cv::Mat _distortionCoefficientsMatrix;
	cv::Ptr<cv::aruco::Dictionary> _dictionary;
	cv::Ptr<cv::aruco::DetectorParameters> _detectorParameters;
};