using Emgu.CV.Structure;
using Emgu.CV;
using System.Windows.Forms;
using System.Xml.Linq;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.UI;

namespace lammaProject
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> image;
        Mat imageMatrix = new Mat();
        string imgName;
        string[] imgNames;
        int index;
        public Form1()
        {
            InitializeComponent();
        }
        //load image
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Images |*.jpg;*.png";
            open.Multiselect = true;
            open.CheckFileExists = true;
            DialogResult dialog = open.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                panAndZoomPictureBox1.Image = new Bitmap(open.FileName);
                imgName = open.FileName;
                imgNames = open.FileNames;
                listBox1.Items.Clear();
                listBox1.Items.AddRange(open.FileNames);
                index = 0;
                listBox1.SelectedIndex = index;
                panAndZoomPictureBox1.Image = new Bitmap(imgNames[index]);

            }
        }
        //save image
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            DialogResult dialog = save.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                panAndZoomPictureBox1.Image.Save(save.FileName);
            }
        }
        //create histogram
        private void button3_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            histogramBox1.ClearHistogram();
            histogramBox1.GenerateHistograms(imageMatrix, 256);
            histogramBox1.Refresh();
        }
        //convert to rgb 
        private void button4_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            VectorOfMat vm = new VectorOfMat();
            CvInvoke.Split(bgr, vm);
            Mat z = Mat.Zeros(bgr.Rows, bgr.Cols, bgr.Depth, 1);

            Mat blue = new Mat();
            VectorOfMat blue_vm = new VectorOfMat(vm[0], z, z);
            CvInvoke.Merge(blue_vm, blue);

            Mat green = new Mat();
            VectorOfMat green_vm = new VectorOfMat(z, vm[1], z);
            CvInvoke.Merge(green_vm, green);

            Mat red = new Mat();
            VectorOfMat red_vm = new VectorOfMat(z, z, vm[2]);
            CvInvoke.Merge(red_vm, red);


            panAndZoomPictureBox2.Image = blue.ToBitmap();
            panAndZoomPictureBox3.Image = green.ToBitmap();
            panAndZoomPictureBox4.Image = red.ToBitmap();
        }
        //convert to gray
        private void button13_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat gray1 = new Mat();
            CvInvoke.CvtColor(bgr, gray1, ColorConversion.Bgr2Gray);
            panAndZoomPictureBox2.Image = gray1.ToBitmap();
        }
        //convert to CMY
        private void button5_Click(object sender, EventArgs e)
        {
            Mat ymc = 255 - BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            VectorOfMat vm = new VectorOfMat();
            CvInvoke.Split(ymc, vm);
            Mat z = Mat.Zeros(ymc.Rows, ymc.Cols, ymc.Depth, 1);

            Mat yellow = new Mat();
            VectorOfMat yellow_vm = new VectorOfMat(z, vm[0], vm[0]);
            CvInvoke.Merge(yellow_vm, yellow);

            Mat magenta = new Mat();
            VectorOfMat magenta_vm = new VectorOfMat(vm[1], z, vm[1]);
            CvInvoke.Merge(magenta_vm, magenta);

            Mat cyan = new Mat();
            VectorOfMat cyan_vm = new VectorOfMat(vm[2], vm[2], z);
            CvInvoke.Merge(cyan_vm, cyan);


            panAndZoomPictureBox2.Image = yellow.ToBitmap();
            panAndZoomPictureBox3.Image = magenta.ToBitmap();
            panAndZoomPictureBox4.Image = cyan.ToBitmap();
        }
        //convert to CMYK
        private void button6_Click(object sender, EventArgs e)
        {
            Mat ymc = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            ymc.ConvertTo(ymc, DepthType.Cv32F);
            ymc /= 255.0;
            VectorOfMat vm = new VectorOfMat();
            CvInvoke.Split(ymc, vm);
            Mat z = Mat.Zeros(ymc.Rows, ymc.Cols, DepthType.Cv8U, 1);

            Mat black = new Mat();
            CvInvoke.Min(vm[0], vm[1], black);
            CvInvoke.Min(vm[2], black, black);

            // y = (y-k)/(255-k)
            Mat yellow = vm[0];
            yellow = 255 * Divide(yellow, black);
            yellow.ConvertTo(yellow, DepthType.Cv8U);
            VectorOfMat yellow_vm = new VectorOfMat(z, yellow, yellow);
            CvInvoke.Merge(yellow_vm, yellow);

            Mat magenta = vm[1];
            magenta = 255 * Divide(magenta, black);
            magenta.ConvertTo(magenta, DepthType.Cv8U);
            VectorOfMat magenta_vm = new VectorOfMat(magenta, z, magenta);
            CvInvoke.Merge(magenta_vm, magenta);

            Mat cyan = vm[2];
            cyan = 255 * Divide(cyan, black);
            cyan.ConvertTo(cyan, DepthType.Cv8U);
            VectorOfMat cyan_vm = new VectorOfMat(cyan, cyan, z);
            CvInvoke.Merge(cyan_vm, cyan);

            black *= 255;
            black.ConvertTo(black, DepthType.Cv8U);
            panAndZoomPictureBox1.Image = yellow.ToBitmap();
            panAndZoomPictureBox2.Image = magenta.ToBitmap();
            panAndZoomPictureBox3.Image = cyan.ToBitmap();
            panAndZoomPictureBox4.Image = black.ToBitmap();

             Mat Divide(Mat c, Mat k)
            {
                Image<Gray, float> color = c.ToImage<Gray, float>();
                Image<Gray, float> black = k.ToImage<Gray, float>();

                for (int i = 0; i < c.Rows; i++)
                {
                    for (int j = 0; j < c.Cols; j++)
                    {
                        if (black.Data[i, j, 0] == 1)
                            color.Data[i, j, 0] = 0;
                        else
                        {
                            color.Data[i, j, 0] = (color.Data[i, j, 0] - black.Data[i, j, 0]) / (1 - black.Data[i, j, 0]);
                        }
                    }
                }

                return color.Mat;
            }


        }
        //convert to HSI
        private void button7_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat hls = new Mat();
            CvInvoke.CvtColor(bgr, hls, ColorConversion.Bgr2Hls);
            panAndZoomPictureBox2.Image = hls.ToBitmap();
        }
        //convert to hsv
        private void button8_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat hsv = new Mat();
            CvInvoke.CvtColor(bgr, hsv, ColorConversion.Bgr2Hsv);
            panAndZoomPictureBox2.Image = hsv.ToBitmap();
        }
        //convert to lab
        private void button9_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat lab = new Mat();
            CvInvoke.CvtColor(bgr, lab, ColorConversion.Bgr2Lab);
            panAndZoomPictureBox2.Image = lab.ToBitmap();
        }
        //convert to luv
        private void button10_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat luv = new Mat();
            CvInvoke.CvtColor(bgr, luv, ColorConversion.Bgr2Luv);
            panAndZoomPictureBox2.Image = luv.ToBitmap();
        }
        //convert to ybcr
        private void button11_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat ycpcr = new Mat();
            CvInvoke.CvtColor(bgr, ycpcr, ColorConversion.Bgr2YCrCb);
            panAndZoomPictureBox2.Image = ycpcr.ToBitmap();
        }
        //convert to yuv
        private void button12_Click(object sender, EventArgs e)
        {
            Mat bgr = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat Yuv = new Mat();
            CvInvoke.CvtColor(bgr, Yuv, ColorConversion.Bgr2Yuv);
            panAndZoomPictureBox2.Image = Yuv.ToBitmap();
        }
        //averaing filter
        private void button23_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);
            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);
            Mat kernal = Mat.Ones(7, 7, DepthType.Cv32F, 1) / 49;

            CvInvoke.BoxFilter(
                src: image,
                dst: filtered,
                ddepth: image.Depth,
                ksize: new Size(3, 3),
                anchor: new Point(-1, -1)
                );
            panAndZoomPictureBox2.Image = filtered.ToBitmap();

            //Create our Diffrence Image
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F); //convert to depth type float
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox3.Image = diff.ToBitmap();
        }
        //next
        private void button24_Click(object sender, EventArgs e)
        {
            index++;
            if (index >= listBox1.Items.Count)
            {
                index = 0;
            }
            listBox1.SelectedIndex = index;
            panAndZoomPictureBox1.Image = new Bitmap(listBox1.SelectedItem.ToString());
        }
        //previous
        private void button25_Click(object sender, EventArgs e)
        {
            index--;
            if (index < 0)
            {
                index = listBox1.Items.Count - 1;
            }
            listBox1.SelectedIndex = index;
            panAndZoomPictureBox1.Image = new Bitmap(listBox1.SelectedItem.ToString());
        }
        //selected index changed
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = listBox1.SelectedIndex;
            imgName = listBox1.SelectedItem.ToString();
            panAndZoomPictureBox1.Image = new Bitmap(imgName);
        }
        //Gaussian
        private void button14_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image); 
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray); 

            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels); 


            CvInvoke.GaussianBlur(
                src: image,
                dst: filtered,
                ksize: new Size(7, 7),
                sigmaX: 1);

            //Get Difference Between two images 
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F);
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox2.Image = filtered.ToBitmap();
            panAndZoomPictureBox3.Image = diff.ToBitmap();
        }
        //Median
        private void button22_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);

            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);

            CvInvoke.MedianBlur(
                src: image,
                dst: filtered,
                ksize: 7);

            //diff
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F);
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox2.Image = filtered.ToBitmap();
            panAndZoomPictureBox3.Image = diff.ToBitmap();
        }
        //Laplacian
        private void button21_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);

            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);

            CvInvoke.Laplacian(
                src: image,
                dst: filtered,
                ddepth: image.Depth);

            //diff
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F);
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox2.Image = filtered.ToBitmap();
            panAndZoomPictureBox3.Image = diff.ToBitmap();
        }
        //Sobel
        private void button20_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);

            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);

            CvInvoke.Sobel(
                src: image,
                dst: filtered,
                ddepth: image.Depth,
                1,
                1);

            //diff
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F);
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox2.Image = filtered.ToBitmap();
            panAndZoomPictureBox3.Image = diff.ToBitmap();
        }
        //highboost
        private void button19_Click(object sender, EventArgs e)
        {
            Mat image = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.CvtColor(image, image, ColorConversion.Bgra2Gray);

            Mat filtered = new Mat(image.Rows, image.Cols, image.Depth, image.NumberOfChannels);

            CvInvoke.GaussianBlur(
                src: image,
                dst: filtered,
                ksize: new Size(7, 7),
                sigmaX: 1);

            //diff
            Mat diff = image - filtered;
            diff.ConvertTo(diff, DepthType.Cv32F);
            CvInvoke.Normalize(diff, diff, normType: NormType.MinMax);
            diff *= 255;
            diff.ConvertTo(diff, DepthType.Cv8U);

            //Sharp
            image.ConvertTo(image, DepthType.Cv32F);
            diff.ConvertTo(diff, DepthType.Cv32F);
            Mat sharp = image - diff;
            sharp.ConvertTo(sharp, DepthType.Cv32F);
            CvInvoke.Normalize(sharp, sharp, normType: NormType.MinMax);
            sharp *= 255;
            sharp.ConvertTo(sharp, DepthType.Cv8U);
            image.ConvertTo(image, DepthType.Cv8U);
            diff.ConvertTo(diff, DepthType.Cv8U);

            panAndZoomPictureBox2.Image = diff.ToBitmap();
            panAndZoomPictureBox3.Image = sharp.ToBitmap();
        }
        //histogram equlizatoin
        private void button18_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat equlized = new Mat();
            CvInvoke.CvtColor(imageMatrix, imageMatrix, ColorConversion.Bgra2Gray);
            CvInvoke.EqualizeHist(imageMatrix, equlized);
            panAndZoomPictureBox2.Image = equlized.ToBitmap();
        }
        //Cold
        private void button17_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);

            Matrix<byte> increase_lut = generate_gray_level_lut(new double[] { 0, 64, 128, 192, 255 }, new double[] { 0, 80, 160, 220, 255 });
            Matrix<byte> decrease_lut = generate_gray_level_lut(new double[] { 0, 64, 128, 192, 255 }, new double[] { 0, 50, 100, 150, 255 });

            CvInvoke.ConvertScaleAbs(imageMatrix, imageMatrix, 0.5, 0);

            VectorOfMat vm = new VectorOfMat();
            CvInvoke.Split(imageMatrix, vm);

            Mat new_b = new Mat();
            CvInvoke.LUT(vm[0], increase_lut, new_b);
            Mat new_g = vm[1];
            Mat new_r = new Mat();
            CvInvoke.LUT(vm[2], decrease_lut, new_r);

            VectorOfMat new_vm = new VectorOfMat(new_b, new_g, new_r);
            Mat cold_image = new Mat();
            CvInvoke.Merge(new_vm, cold_image);

            panAndZoomPictureBox2.Image = cold_image.ToBitmap();

            static Matrix<byte> generate_gray_level_lut(double[] current, double[] target)
            {
                byte[] lut = new byte[256];

                if (current.Length != target.Length)
                    throw new ArgumentException("inputs should be of the same size");

                double[,] equations_coefficient = get_equations_coefficient(current, target);

                for (int i = 0; i < lut.Length; i++)
                {
                    int index = 0;
                    for (int j = 0; j < current.Length - 2; j++)
                        if (i >= current[j] && i <= current[j + 1])
                        {
                            index = j;
                            break;
                        }
                    double temp = equations_coefficient[index, 0] * i + equations_coefficient[index, 1];

                    if (temp < 0)
                        temp = 0;
                    if (temp > 255)
                        temp = 255;
                    lut[i] = (byte)temp;
                }



                return new Matrix<byte>(lut);
            }
            static double[,] get_equations_coefficient(double[] current, double[] target)
            {
                double[,] equations_coefficient = new double[current.Length - 1, 2];

                for (int i = 0; i < current.Length - 1; i++)
                {
                    equations_coefficient[i, 0] = (target[i + 1] - target[i]) / (current[i + 1] - current[i]);
                    equations_coefficient[i, 1] = target[i + 1] - equations_coefficient[i, 0] * current[i + 1];
                }

                return equations_coefficient;
            }
        }
        //Warm
        private void button16_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);

            Matrix<byte> increase_lut = generate_gray_level_lut(new double[] { 0, 64, 128, 192, 255 }, new double[] { 0, 80, 160, 220, 255 });
            Matrix<byte> decrease_lut = generate_gray_level_lut(new double[] { 0, 64, 128, 192, 255 }, new double[] { 0, 50, 100, 150, 255 });

            CvInvoke.ConvertScaleAbs(imageMatrix, imageMatrix, 0.5, 0);

            VectorOfMat vm = new VectorOfMat();
            CvInvoke.Split(imageMatrix, vm);

            Mat new_b = new Mat();
            Mat new_g = vm[1];
            Mat new_r = new Mat();
            VectorOfMat new_vm = new VectorOfMat(new_b, new_g, new_r);

            new_b = new Mat();
            CvInvoke.LUT(vm[0], decrease_lut, new_b);
            new_g = vm[1];
            new_r = new Mat();
            CvInvoke.LUT(vm[2], increase_lut, new_r);

            new_vm = new VectorOfMat(new_b, new_g, new_r);
            Mat warm_image = new Mat();
            CvInvoke.Merge(new_vm, warm_image);
            static Matrix<byte> generate_gray_level_lut(double[] current, double[] target)
            {
                byte[] lut = new byte[256];

                if (current.Length != target.Length)
                    throw new ArgumentException("inputs should be of the same size");

                double[,] equations_coefficient = get_equations_coefficient(current, target);

                for (int i = 0; i < lut.Length; i++)
                {
                    int index = 0;
                    for (int j = 0; j < current.Length - 2; j++)
                        if (i >= current[j] && i <= current[j + 1])
                        {
                            index = j;
                            break;
                        }
                    double temp = equations_coefficient[index, 0] * i + equations_coefficient[index, 1];

                    if (temp < 0)
                        temp = 0;
                    if (temp > 255)
                        temp = 255;
                    lut[i] = (byte)temp;
                }



                return new Matrix<byte>(lut);
            }
            static double[,] get_equations_coefficient(double[] current, double[] target)
            {
                double[,] equations_coefficient = new double[current.Length - 1, 2];

                for (int i = 0; i < current.Length - 1; i++)
                {
                    equations_coefficient[i, 0] = (target[i + 1] - target[i]) / (current[i + 1] - current[i]);
                    equations_coefficient[i, 1] = target[i + 1] - equations_coefficient[i, 0] * current[i + 1];
                }

                return equations_coefficient;
            }
            panAndZoomPictureBox2.Image = warm_image.ToBitmap();
        }
        //log
        private void button15_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            RangeF min_max = imageMatrix.GetValueRange();
            double max = min_max.Max;
            double c = 255 / Math.Log(1 + max, Math.E);
            Mat imageMat_log = new Mat();
            imageMatrix.ConvertTo(imageMatrix, DepthType.Cv32F);
            CvInvoke.Log(imageMatrix, imageMat_log);
            imageMat_log *= c;
            imageMatrix.ConvertTo(imageMatrix, DepthType.Cv8U);
            imageMat_log.ConvertTo(imageMat_log, DepthType.Cv8U);
            panAndZoomPictureBox2.Image = imageMat_log.ToBitmap();
        }
        //power
        private void button26_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            double c = 1 * 255;
            double g = 0.2;
            Mat imageMat_power = new Mat();
            imageMatrix.ConvertTo(imageMatrix, DepthType.Cv32F);
            CvInvoke.Pow(imageMatrix / 255.0, g, imageMat_power);
            imageMat_power *= c;
            imageMatrix.ConvertTo(imageMatrix, DepthType.Cv8U);
            imageMat_power.ConvertTo(imageMat_power, DepthType.Cv8U);
            panAndZoomPictureBox2.Image = imageMat_power.ToBitmap();
        }
        //resize
        private void button27_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Mat imageResized = new Mat();
            CvInvoke.Resize(imageMatrix, imageResized,
                new Size((int)numericUpDown1.Value, (int)numericUpDown2.Value),
                interpolation: Inter.Cubic);
            panAndZoomPictureBox1.Image = imageResized.ToBitmap();
        }
        //rotation
        private void button32_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            double[,] tmatrix = new double[,] { { 0.5, 0.5, 10 }, { 0, 0.5, 50 } };
            Matrix<double> tmat = new Matrix<double>(tmatrix);
            Mat rmat = new Mat();
            CvInvoke.GetRotationMatrix2D(
                new PointF(imageMatrix.Width / 2, imageMatrix.Height / 2),
                90,
                0.5, rmat);
            Mat imageMat_t = new Mat();
            CvInvoke.WarpAffine(
                imageMatrix, imageMat_t,
                rmat,
                new Size(imageMatrix.Width, imageMatrix.Height)
                );
            panAndZoomPictureBox1.Image = imageMat_t.ToBitmap();
        }
        //add text to photo
        private void button31_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            CvInvoke.PutText(imageMatrix,
                textBox1.Text,
                new Point(100, 100),
                FontFace.HersheyComplex,
                2.0, new Bgr(255, 255, 255).MCvScalar);
            panAndZoomPictureBox1.Image = imageMatrix.ToBitmap();
        }
        //undo step
        private void button30_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Image<Gray, byte> img = imageMatrix.ToImage<Gray, byte>();
            int step = (int)numericUpDown3.Value;

            Image<Gray, byte> converted = ConvertStep(img, step);
            panAndZoomPictureBox2.Image = converted.ToBitmap();




            Image<Gray, byte> ConvertStep(Image<Gray, byte> image, int step)
            {
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        for (int c = 0; c < image.NumberOfChannels; c++)
                        {
                            double pixle = image.Data[i, j, c];
                            pixle = Math.Round(pixle / step) * step + step / 2;
                            image.Data[i, j, c] = (byte)pixle;
                        }
                    }
                }
                return image;
            }
        }
        //Detalis
        private void button29_Click(object sender, EventArgs e)
        {
            imageMatrix = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            image = imageMatrix.ToImage<Bgr, byte>();
            MessageBox.Show("Width = " + imageMatrix.Width.ToString() + "   " + "Height = " + imageMatrix.Height.ToString() + "\n " + " depthType " + imageMatrix.Depth );
        }
        //add logo
        private void button28_Click(object sender, EventArgs e)
        {

            Mat wall_mat = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Image<Bgr, byte> wall = wall_mat.ToImage<Bgr, byte>();
            Mat logo_mat = BitmapExtension.ToMat((Bitmap)panAndZoomPictureBox1.Image);
            Image<Bgr, byte> logo = logo_mat.ToImage<Bgr, byte>();
            Mat logo_resized = new Mat();
            CvInvoke.Resize(logo_mat, logo_resized,
                new Size(200, 300),
                interpolation:Inter.Cubic);
            CvInvoke.Resize(logo_mat, logo_resized,
                new Size(0, 0),
                fx: 0.1,
                fy: 0.1,
                interpolation:Inter.Cubic);

           
            Mat logo_comp = new Mat();

           
            CvInvoke.BitwiseNot(logo_mat, logo_comp);
            Mat logo_gray = new Mat();
            CvInvoke.CvtColor(logo_resized,
                logo_gray,
                Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            Mat mask = new Mat();
            CvInvoke.Threshold(logo_gray, mask, 0, 255,
                Emgu.CV.CvEnum.ThresholdType.Binary);
            Mat mask_inv = new Mat();
            CvInvoke.BitwiseNot(mask, mask_inv);
            int w = wall_mat.Cols;
            int h = wall_mat.Rows;
            CvInvoke.Resize(logo_mat, logo_mat, new Size(w, h));

          
            Mat image_add = new Mat();
            CvInvoke.AddWeighted(logo_mat, 0.2, wall_mat, 0.8, 0, image_add);

            int logo_w = logo_resized.Cols;
            int logo_h = logo_resized.Rows;
            Rectangle roi = new Rectangle(0, 0, logo_w, logo_h);
            Mat wall_subset = new Mat(wall_mat, roi);

            Mat background = new Mat();
            CvInvoke.BitwiseAnd(wall_subset, wall_subset, background, mask_inv);
            Mat foreground = new Mat();
            CvInvoke.BitwiseAnd(logo_resized, logo_resized, foreground, mask);

            Mat full = background + foreground;

            wall.ROI = roi;
            full.CopyTo(wall);
            wall.ROI = Rectangle.Empty;
            wall_mat = wall.Mat;

            wall.ROI = roi;
            wall.And(wall, mask_inv.ToImage<Gray, byte>());
            Image<Bgr, byte> full2 = wall + foreground.ToImage<Bgr, byte>();
            full2.CopyTo(wall);
            wall.ROI = Rectangle.Empty;
            wall_mat = wall.Mat;

            panAndZoomPictureBox2.Image = wall.ToBitmap();
        }
    }
}