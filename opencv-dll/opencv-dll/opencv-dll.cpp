// dllmain.cpp : Defines the entry point for the DLL application.
#include "opencv-dll.h"
#include "trace.h"

OpenCvDll::OpenCvDll(
    float mS,
    float fx, float fy, 
    float px, float py, 
    float rx, float ry, float rz, 
    float tx, float ty, 
    int imgWidth, int imgHeight)
{
    // Set marker size, 0.08 m (8 cm)
    _markerSize = mS;

	// Set camera intrinsic parameters for aruco based pose estimation
	cv::Mat cMat(3, 3, CV_64F, cv::Scalar(0));
	cMat.at<double>(0, 0) = fx;
	cMat.at<double>(0, 2) = px;
	cMat.at<double>(1, 1) = fy;
	cMat.at<double>(1, 2) = py;
	cMat.at<double>(2, 2) = 1.0;

	// Set distortion matrix for aruco based pose estimation
	cv::Mat dcM(1, 5, CV_64F);
	dcM.at<double>(0, 0) = rx;
	dcM.at<double>(0, 1) = ry;
	dcM.at<double>(0, 2) = tx;
	dcM.at<double>(0, 3) = ty;
	dcM.at<double>(0, 4) = rz;

    // Cache the camera and distortion matrices
    _cameraMatrix = cMat;
    _distortionCoefficientsMatrix = dcM;
    width = width;
    height = height;

    dbg::trace(L"OpenCvDll::OpenCvDll: created the camera and distortion matrices.");

    // Create the aruco dictionary from id
    _dictionary = cv::aruco::getPredefinedDictionary(cv::aruco::DICT_6X6_250);

    // Create detector parameters
    _detectorParameters = cv::aruco::DetectorParameters::create();

    dbg::trace(L"OpenCvDll::OpenCvDll: created the aruco dictionary ID and detector params.");
}

OpenCvDll::~OpenCvDll()
{
}

std::map<int32_t, DetectedMarker> OpenCvDll::ArUcoMarkerDetection(
    cv::Mat mat)
{
    std::map<int32_t, DetectedMarker> detectedMarkers;

    // https://docs.opencv.org/4.1.1/d5/dae/tutorial_aruco_detection.html
    std::vector<std::vector<cv::Point2f>> markers, rejectedCandidates;
    std::vector<int32_t> markerIds;

    // Convert cv::Mat to grayscale for detection
    cv::Mat grayMat;
    cv::cvtColor(mat, grayMat, cv::COLOR_BGRA2GRAY);

    // Detect markers
    cv::aruco::detectMarkers(
        grayMat,
        _dictionary,
        markers,
        markerIds,
        _detectorParameters,
        rejectedCandidates);

    dbg::trace(
        L"ArUcoMarkerTracker::DetectArUcoMarkersInFrame: %i markers found",
        markerIds.size());


    if (!markerIds.empty())
    {
        // Estimate pose of single markers
        cv::aruco::estimatePoseSingleMarkers(
            markers,
            _markerSize,
            _cameraMatrix,
            _distortionCoefficientsMatrix,
            rVecs,
            tVecs);

        //// Draw markers and axis using opencv tool
        //cv::aruco::drawDetectedMarkers(
        //    mat,
        //    markers, 
        //    markerIds);
        //
        //// draw axis for each marker
        //for (size_t i = 0; i < markerIds.size(); i++)
        //{
        //    cv::aruco::drawAxis(
        //        mat,
        //        _cameraMatrix,
        //        _distortionCoefficientsMatrix,
        //        rVecs[i], tVecs[i],
        //        0.1f);
        //}

        //cv::imshow("ArUco Detections", mat);
        //cv::waitKey(0);
        //cv::destroyAllWindows();

        for (size_t i = 0; i < markerIds.size(); i++)
        {
            auto id = markerIds[i];

            auto posText = L"OpenCV Marker Position: " + 
                std::to_wstring(tVecs[i][0]) + L", " + 
                std::to_wstring(tVecs[i][1]) + L", " + 
                std::to_wstring(tVecs[i][2]);
            dbg::trace(posText.data());

            auto rotText = L"OpenCV Marker Rotation: " + 
                std::to_wstring(rVecs[i][0]) + L", " + 
                std::to_wstring(rVecs[i][1]) + L", " +
                std::to_wstring(rVecs[i][2]);
            dbg::trace(rotText.data());

            DetectedMarker marker;
            marker.markerId = id;
            marker.point = Windows::Foundation::Numerics::float3(
                tVecs[i][0], tVecs[i][1], tVecs[i][2]);
            marker.rotation = Windows::Foundation::Numerics::float3(
                rVecs[i][0], rVecs[i][1], rVecs[i][2]);
            detectedMarkers[id] = marker;
        }
    }
    return detectedMarkers;
}

#pragma region DllMethods

OpenCvDll* _openCvDll;

bool InitializeDll(
    float mS,
    float fx, float fy,
    float px, float py,
    float rx, float ry, float rz,
    float tx, float ty,
    int imgWidth, int imgHeight)
{
    _openCvDll = new OpenCvDll(
        mS,
        fx, fy,
        px, py,
        rx, ry, rz,
        tx, ty,
        imgWidth, imgHeight);

    return true;
}

bool StartMarkerTracking(
    unsigned char* texData,
    int width, int height)
{
    // Instantiate matrix of 8UC4 (RGBA32) for texture data
    cv::Mat texture(
        height,
        width,
        CV_8UC4, texData);
    cv::cvtColor(texture, texture, cv::COLOR_BGRA2RGB);

    //cv::imshow("Unity Texture", texture);
    //cv::waitKey(0);
    //cv::destroyAllWindows();

    // Flip the texture on the x-axis
    cv::Mat flipTexture;
    cv::flip(texture, flipTexture, 0);

    // Flip the texture on the y-axis
    cv::flip(flipTexture, flipTexture, 1);

    // Detect ArUco markers in frame
    _openCvDll->observedMarkers = _openCvDll->ArUcoMarkerDetection(flipTexture);

    //cv::imshow("Unity Texture Flipped", flipTexture);
    //cv::waitKey(0);
    //cv::destroyAllWindows();

    return true;
}

int GetDetectedMarkerCount()
{
    return _openCvDll->observedMarkers.size();
}

bool GetDetectedMarkerIds(unsigned int* detectedIds, int size)
{
    if (size != _openCvDll->observedMarkers.size())
    {
        dbg::trace(L"Provided int array isn't large enough");
        return false;
    }

    int count = 0;
    for (auto marker : _openCvDll->observedMarkers)
    {
        detectedIds[count] = marker.second.markerId;
        count++;
    }

    return true;
}

bool GetDetectedMarkerPose(
    int detectedId, 
    float* position, 
    float* rotation, 
    float* cameraToWorldUnity)
{
    if (_openCvDll->observedMarkers.find(detectedId) == _openCvDll->observedMarkers.end())
    {
        dbg::trace(L"Marker was not detected");
        return false;
    }

    position[0] = _openCvDll->observedMarkers.at(detectedId).point.x;
    position[1] = _openCvDll->observedMarkers.at(detectedId).point.y;
    position[2] = _openCvDll->observedMarkers.at(detectedId).point.z;

    // Note: This is a rodrigues vector (magnitude is equal to the angle in radians, normalized vector is the axis for rotation)
    // These are NOT euler angles
    rotation[0] = _openCvDll->observedMarkers.at(detectedId).rotation.x;
    rotation[1] = _openCvDll->observedMarkers.at(detectedId).rotation.y;
    rotation[2] = _openCvDll->observedMarkers.at(detectedId).rotation.z;
    
    return true;
}

#pragma endregion

