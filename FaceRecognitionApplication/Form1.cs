using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Necesssary References of this program
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;


namespace FaceRecognitionApplication
{
    public partial class Form1 : Form
    {
        //Capture Class
        Capture capture;

        //Instead of HaarCascade
        HaarCascade hc, face;
        //Image Class
        //Current Frame is for the camera
        Image<Bgr, byte> currentFrame;
        //gray is for gray image
        Image<Gray, byte> gray;
        //Font
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);

        Image<Gray, Byte> result, trainedFace = null;
        //List of images, strings
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();

        int ContTrain, NumLabels, t;
        string name, names = null;

        public Form1()
        {
            InitializeComponent();
            capture = new Capture(0);
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            Application.Idle += new EventHandler(Function2);
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedPeople/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split(',');
                //The first label before, will be the number of faces saved
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                //facenumber.bmp file
                string LoadFaces;

                for (int i = 1; i < NumLabels + 1; i++)
                {
                    LoadFaces = "face" + i + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedPeople/TrainedLabels.txt"));
                    labels.Add(Labels[i]);
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
                MessageBox.Show("Nothing in database");
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Face Counter
                ContTrain = ContTrain + 1;
                //Get Gray from captured Device
                gray = capture.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //Face Detector
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                //Action for each element detected
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    trainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                    break;
                }

                //resize face detected image for force to compare the same size with the 
                //test image with cubic interpolation type method
                trainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(trainedFace);
                labels.Add(tbName.Text);

                //Write the number of triained faces in a file text for further load
                File.WriteAllText(Application.StartupPath + "/TrainedPeople/TrainedLabels.txt", trainingImages.ToArray().Length.ToString() + ",");

                //Write the labels of triained faces in a file text for further load
                for (int i = 1; i < trainingImages.ToArray().Length + 1; i++)
                {
                    trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/TrainedPeople/TrainedLabels" + i + ".bmp");
                    File.AppendAllText(Application.StartupPath + "/TrainedPeople/TrainedLabels", labels.ToArray()[i - 1] + ",");
                }
                MessageBox.Show(tbName + "Added Successfully");
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }


       

            //Created eventhandler
            private void Function(object sender, EventArgs e)
            {
                currentFrame = capture.QueryFrame();
                gray = currentFrame.Convert<Gray, Byte>();
                MCvAvgComp[][] detectedFaces = gray.DetectHaarCascade(hc, 1.1, 20, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(60, 60));

                foreach (MCvAvgComp a in detectedFaces[0])
                {
                    currentFrame.Draw(a.rect, new Bgr(Color.Red), 2);
                }
                ImageBox1.Image = currentFrame.ToBitmap();

            }

             private void Function2(object sender, EventArgs e)
             {
                NamePersons.Add("");
                //Get the current frame form capture device
                 currentFrame = capture.QueryFrame();   //.Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                //Convert it to Grayscale
                gray = currentFrame.Convert<Gray, Byte>();

                //Face Detector
                MCvAvgComp[][] facedetectedNow = gray.DetectHaarCascade(face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(60,60));

                     //Action for each element detected
                        foreach (MCvAvgComp f in facedetectedNow[0])
                        {
                                         t = t + 1;
                                     //TermCriteria for face recognition with numbers of trained images like maxIteration
                                      result = currentFrame.Copy(f.rect).Convert<Gray, Byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                                    currentFrame.Draw(f.rect, new Bgr(Color.Green), 3);
                                 if(trainingImages.ToArray().Length !=0)
                                 {
                                     MCvTermCriteria termCriterias = new MCvTermCriteria(ContTrain, 0.001);
                                        
                                        //face recognizer
                                     EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), labels.ToArray(), 1500, ref termCriterias);
                                     name = recognizer.Recognize(result);
                                     currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Red));

                                 }
                                    NamePersons[t - 1] = name;
                                    NamePersons.Add("");
                                    label1.Text = facedetectedNow[0].Length.ToString();


                                    
                                    //Names and concatenation of people recognised 
                                    for (int nnn = 0; nnn < facedetectedNow[0].Length; nnn++)
                                    {
                                      names = names + NamePersons[nnn] + ", ";
                                    }
                                         ImageBox1.Image = currentFrame.ToBitmap();
                                          names = "";
                                          NamePersons.Clear();
                        }

                             t = 0;

             }

    } 
}

