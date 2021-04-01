using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Forms;


namespace laab_4_fractals
{
    public partial class Form1 : Form
    {
        string filename;
        Bitmap img_2;
        double[,] u0, u, b;
        int w, h;
        const int delta = 3;
        double[] xi = new double[delta];
        double[] yi = new double[delta];
        double[] vol = new double[delta];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string path2 = "result.jpg";
            Image<Bgr, Byte> image = new Image<Bgr, Byte>("sm_2.jpg");
            Image<Gray, Byte> image2 = image.Convert<Gray, Byte>();

            CvInvoke.Imwrite("result.jpg", image2);

            img_2 = new Bitmap(path2);
            pictureBox1.Image = img_2;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            h = img_2.Height;
            w = img_2.Width;
            u0 = new double[w, h];
            u = new double[w, h];
            b = new double[w, h];
            setU0(img_2);
            double s;
            //calc u and b 2 times
            for(int k = 1; k <= delta; k++)
            {
                calcU(k);
                calcB(k);
                s = 0;
                for (int i = w - 1; i >= 0; i--)
                {
                    for (int j = h - 1; j >= 0; j--)
                    {
                        s += u[i, j] - b[i, j];
                    }
                }
                vol[k-1] = s;

            }
            for (int k = 1; k < delta; k++)
            {
                yi[k - 1] = Math.Log((vol[k] - vol[k - 1]) / 2);
                xi[k - 1] = Math.Log(k + 1);
            }
            double res = 2.0 + MNK(delta-1);

            textBox1.Text = res.ToString();
            
        }
        private void setU0(Bitmap img)
        {
            for (int i = w - 1; i >= 0; i--)
            {
                for (int j = h - 1; j >= 0; j--)
                {
                    u0[i, j] = getIntense(img.GetPixel(i, j));
                }
            }

            return;
        }
        //верхняя поверхность uδ(i,j)
        private void calcU(int delta)
        {
            if (delta > 1)
            {
                for (int i = w - 1; i >= 0; i--)
                {
                    for (int j = h - 1; j >= 0; j--)
                    {
                        u0[i, j] = u[i, j];
                    }
                }
            }

            for (int i = w - 1; i >= 0; i--)
            {
                for (int j = h - 1; j >= 0; j--)
                {
                    u[i, j] = Math.Max(
                        u0[i, j] + 1,
                        Math.Max(
                            Math.Max(
                                i > 0 ? u0[i - 1, j] : 0,
                                j > 0 ? u0[i, j - 1] : 0
                                ),
                            Math.Max(
                                i < w - 1 ? u0[i + 1, j] : 0,
                                j < h - 1 ? u0[i, j + 1] : 0
                                )
                            )
                        );
                }
            }
            return;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void calcB(int delta)
        {
            if (delta > 1)
            {
                for (int i = w - 1; i >= 0; i--)
                {
                    for (int j = h - 1; j >= 0; j--)
                    {
                        u0[i, j] = b[i, j];
                    }
                }
            }

            for (int i = w - 1; i >= 0; i--)
            {
                for (int j = h - 1; j >= 0; j--)
                {
                    b[i, j] = Math.Min(
                        u0[i, j] - 1,
                        Math.Min(
                            Math.Min(
                                i > 0 ? u0[i - 1, j] : 256,
                                j > 0 ? u0[i, j - 1] : 256
                                ),
                            Math.Min(
                                i < w - 1 ? u0[i + 1, j] : 256,
                                j < h - 1 ? u0[i, j + 1] : 256
                                )
                            )
                        );
                }
            }
            return;
        }


        private double MNK(int n)
        {
            double sx = 0,
                sx2 = 0,
                sxy = 0,
                sy = 0;
            for (int i = 0; i < n; i++)
            {
                sx += xi[i];
                sx2 += xi[i] * xi[i];
                sxy += xi[i] * yi[i];
                sy += yi[i];
            }
            sx /= n;
            sx2 /= n;
            sxy /= n;
            sy /= n;
            return (sxy - sx * sy) / (sx2 - sx * sx);
        }
        private double getIntense(Color color)
        {
            return (color.R + color.G + color.B) / 3;
        }
    }
}
