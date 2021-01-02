using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Работа_5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowCount = 10;
        }

        Integral I;
        public Graphics Area;
        public Pen P;

        private void button1_Click(object sender, EventArgs e)
        {
            // ввод параметров интегрирования
            double A = Convert.ToDouble(textBox1.Text);
            double B = Convert.ToDouble(textBox2.Text);
            int M = Convert.ToInt32(textBox3.Text);
            // создания объекта класса Integral
            I = new Integral(A, B, M);
            // скрытие элементов
            this.label1.Visible = false;
            this.label2.Visible = false;
            this.label3.Visible = false;
            this.label4.Visible = false;
            this.textBox1.Visible = false;
            this.textBox2.Visible = false;
            this.textBox3.Visible = false;
            this.button1.Visible = false;
            this.button1.Visible = false;
            // редактирование первой строки окна
            this.Text += I;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int M0 = Convert.ToInt32(textBox3.Text);
            for (int i = 0; i < 10; i++)
            {
                I.m = (int)((i + 1) / 10.0 * M0);
                dataGridView1.Rows[i].Cells[0].Value = I.m;
                dataGridView1.Rows[i].Cells[1].Value = I.ИнтСимпсон;
                dataGridView1.Rows[i].Cells[2].Value = I.ИнтЛейбниц;
                dataGridView1.Rows[i].Cells[3].Value = I.ОстаточныйЧленСимпсона;
            }
            dataGridView1.Visible = true;
            button2.Visible = false;
            button3.Visible = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Area = panel1.CreateGraphics();
            int w = panel1.ClientSize.Width;
            int h = panel1.ClientSize.Height;
            P = new Pen(Color.Brown, 2);
            Area.DrawLine(P, 0, 0, w, 0);
            Area.DrawLine(P, 0, 0, 0, h);
            Area.DrawLine(P, 0, h, w, h);
            Area.DrawLine(P, w, 0, w, h);
            P = new Pen(Color.DarkMagenta, 3);
            Area.DrawLine(P, 10, 10, 10, h - 10);
            Area.DrawLine(P, 10, h - 10, w - 10, h - 10);
            Area.DrawString("f(x)",
                new Font("Corier new", 12),
                new SolidBrush(Color.DarkRed),
                new Point(20, 10));
            Area.DrawString("x",
                new Font("Corier new", 12),
                new SolidBrush(Color.DarkRed),
                new Point(w - 20, h - 40));

            float[] x = new float[11];
            float[] fx = new float[11];
            for (int i = 0; i <= 10; i++)
            {
                x[i] = i / 10F;
                fx[i] = (float)I.fx(x[i]);
            }
            float fmax = fx[0];
            foreach (float f in fx) if (fmax < f) fmax = f;
            float My = (h - 20) / fmax;
            float Mx = w - 20;
            P = new Pen(Color.DarkGreen, 3);
            float ix, iy, ix1 = 10 + x[0] * Mx, iy1 = h - 10 - fx[0] * My;
            for (int i = 0; i < 10; i++)
            {
                ix = ix1;
                iy = iy1;
                ix1 = 10 + x[i + 1] * Mx;
                iy1 = h - 10 - fx[i + 1] * My;
                Area.DrawLine(P, ix, iy, ix1, iy1);
            }
            panel1.Visible = true;
        }
    }

    class Integral
    {
        double A;
        double B;
        int M;
        public Integral(double a, double b, int m)
        {
            A = a < b ? a : b;
            B = b < a ? a : b;
            M = m;
        }

        public int m
        {
            get { return M; }
            set { M = value; }
        }

        public double fx(double x)
        { return 4*Math.Sin(x) - 0.5*x; }
        public double Fx(double x)
        { return -4 * Math.Cos(x) - 0.25 * x * x; }
        public double h //длинна одного шага
        {
            get { return (B - A) / M; }
        }
        public double ИнтЛейбниц
        {
            get { return Fx(B) - Fx(A); }
        }
        public double ИнтТрапеции
        {
            get
            {
                double sum = (fx(A) + fx(B)) / 2;
                for (double i = A + h; i < B; i += h) sum += fx(i);
                sum *= h;
                return sum;
            }

        }

        public double ИнтСимпсон
        {
            get
            {
               
                double sum = 0.0;
                double hs = (B - A) / (2*M);
                double[] xi = new double[2 * M];
                double[] fxi = new double[2 * M];
                for (int i = 0; i < 2 * M; i++)
                {
                    xi[i] = A + i * hs;
                    fxi[i] = fx(xi[i]);

                    if ((i > 0) && (i % 2 == 0))
                        sum += 2*fxi[i];
                    else
                        sum += 4 * fxi[i];
                    
                }
                sum += fx(A);
                sum += fx(B);
                sum *= hs/3;
  
                return sum; 
            }
        }

        public double ОстаточныйЧленСимпсона
        {
            get
            {
                double ostChlen = 0.0;
                double ksi = 0.0;
                double hs = (B - A) / (2 * M);
                double[] xi = new double[2 * M];
                
                for (int i = 0; i < 2 * M; i++)
                {
                    xi[i] = A + i * hs;
                    if (Math.Abs(4 * Math.Sin(xi[i])) > ksi) ksi = Math.Abs(4 * Math.Sin(xi[i])); //вычисление кси
                }
                ostChlen = (Math.Pow((B - A), 5) / (180 * Math.Pow(2 * M, 4))) * ksi;

                return ostChlen;
            }
        }
        public override string ToString()
        {
            return String.Format(" A={0:f2}    B={1:f2}   M={2,4}", A, B, M);
        }
    }
}
