OpenCV is open source computer vision library
ultra fast image processing 
C++, python 

EmguCV is fast but not faster than openCV

HaarCascade: Machine learning
Algorithm: Positive and Negative
Classifier use to detect objects in other images 
Haarcascase HS = new Haarcascade("XML file")

Image<parameter1, parameter2> class
parameter1: color
parameter2: depth

Color: gray, Bgr(Blue Green Red), Bgra(Blue, Red, Green, Alpha), Hsv(Hue Saturation Value)
Depth: Byte(unsigned), SByte, single(float), Double

Capture class
capture images from camera and videos

Capture cap = new Capture(1)  1 == the index of the camera you are using

DetectHaarCascade method: Brain of recognition
Has 5 parameters
DetectHaarCascase(HaarCascade, Double, Int32, HAAR_DETECTION_TYPE, Size)
HaarCascade: Load our Haarcascade
Double: Search window scale (increase window size 1.1 = 10% )
Int32: Neighbor rectangles (Use Maximum Value)
HAAR_DETECTION_TYPE: rejects some image regions that contain few or two much edges [CV_HAAR_DO_CANNY_PRUNNING]
size: Minimum window size(20X20)

MCVAvgComp method: stores the result of Detected Haar cascade

Draw Method: 
Draw(Rectange, color, int)
Rectangle: Shape
color: color of shape
int: Thickness of the shape











