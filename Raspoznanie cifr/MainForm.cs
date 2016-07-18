/*
 * Created by SharpDevelop.
 * User: Taras.Omarov
 * Date: 12.10.2015
 * Time: 16:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Raspoznanie_cifr
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public Bitmap image1;
		public string [] filenames;
		public int maxfiles;
		public int nfile;
		public Bitmap slisedBit;
		public Bitmap modeling;
		int [,] mask;
		int maskX,maskY;
		int [,,] allmasks;
		
		public SudokuRecognitor sudRec;
		
		public void redraw (Bitmap temp)
		{			
			Graphics g1 = pictureBox1.CreateGraphics();
			g1.FillRectangle(Brushes.White,1,1,pictureBox1.Width,pictureBox1.Height);
			g1.DrawImage(image1,1,1,60,100);
			g1.DrawImage(slisedBit,80,1,60,100);
			g1.DrawImage(modeling,1,110,60,100);
			g1.DrawImage(temp,80,110,60,100);
			g1.Dispose();
		}
		
		public void calculations ()
		{
			String filen = Convert.ToString(nfile)+".jpg";						
			Bitmap tempimage = new Bitmap(filen);
			NumberRecognitor rec = new NumberRecognitor(tempimage);
			image1 = (Bitmap)rec.picture.Clone();
			String str= Convert.ToString(rec.recognize());
			
			//нарисовка сетки
			slisedBit = (Bitmap)rec.picture.Clone();
			Graphics slisedGraf = Graphics.FromImage(slisedBit);
			for (int i=1;i<maskX;i++)
				slisedGraf.DrawLine
					(Pens.Green,
					 (slisedBit.Width/maskX)*i,0,
					 (slisedBit.Width/maskX)*i,slisedBit.Height);
			for (int i=1;i<maskY;i++)
				slisedGraf.DrawLine
					(Pens.Green,
					 0,(slisedBit.Height/maskY)*i,
					 slisedBit.Width,(slisedBit.Height/maskY)*i);
			
					
			//textBox1.Text= Convert.ToString(image1.Width*image1.Height);			
			
			modeling = rec.ShowLocalMask();

			for(int j=0;j<NumberRecognitor.masksY;j++)
				for(int i=0;i<NumberRecognitor.masksX;i++)
					{allmasks[nfile,i,j] = rec.localMask[i,j];}
			
			textBox1.Text = str;
			redraw(tempimage);		
			tempimage.Dispose();
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//

			InitializeComponent();
			maxfiles=69; 
			nfile=0;			
			modeling = new Bitmap(30, 50, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			maskX = 3;
			maskY = 5;
			mask = new int[maskX,maskY];
			allmasks = new int[maxfiles+1,maskX,maskY];
				
			calculations ();

			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void MainFormLoad(object sender, EventArgs e)
		{
	
		}
		
		void PictureBox1Paint(object sender, PaintEventArgs e)
		{			
			//redraw(new Bitmap(30, 50, System.Drawing.Imaging.PixelFormat.Format24bppRgb));
		}
		void TextBox1TextChanged(object sender, EventArgs e)
		{
	
		}
		void Button3Click(object sender, EventArgs e)
		{
			for(int i=0;i<maxfiles;i++)				
				if (nfile<maxfiles)
				{
					nfile++;
					calculations();
				}
				else
				{textBox1.Text = "there is no file";}
		}
		void Label1Click(object sender, EventArgs e)
		{
	
		}
		void Button4Click(object sender, EventArgs e)
		{
			if (1==textBox2.Text.Length)
			{
				if (nfile <= maxfiles)
				{
					int val = Convert.ToInt32(textBox2.Text);
					Graphics g = pictureBox1.CreateGraphics();					
					g.FillRectangle(Brushes.White,1,1,pictureBox1.Width,pictureBox1.Height);
					for(int k=0;k<=(maxfiles/10);k++)
					{							
						int nomber = k*10+val;
						g.DrawImage(new Bitmap(Convert.ToString(nomber)+".jpg"), 40, 1+k*60, 30, 50);
						for (int j=0;j<maskY;j++)						
							for (int i=0;i<maskX;i++)							
								if (1==allmasks[nomber,i,j])									
									g.FillRectangle(Brushes.Black,1+i*10,1+k*60+j*10,10,10);
								else
									g.FillRectangle(Brushes.White,1+i*10,1+k*60+j*10,10,10);
					}
				}
			}
			else {textBox1.Text="check VALUE!!!";}
		}
		void Button5Click(object sender, EventArgs e)
		{
			button1.Enabled=true;
			button2.Enabled=true;
			button1.Visible = true;
			button2.Visible = true;
			nfile=0;
			DrawingTheRecogn();
			button5.Enabled = false;
			button3.Enabled = false;
			button4.Enabled = false;
			
		}
		
		
		void DrawingTheRecogn ()
		{
			String str = Convert.ToString(nfile) + ".jpg";
			
			NumberRecognitor number = new NumberRecognitor(new Bitmap(str));
			if (nfile<maxfiles)
				{											
					// dopisat
					Graphics g1= pictureBox1.CreateGraphics();
					textBox1.Text = Convert.ToString(number.recognize());
					g1.DrawImage(number.picture,1,1,30,50);
					g1.DrawImage(number.ShowLocalMask(),41,1,30,50);
					g1.DrawImage(NumberRecognitor.ShowMaskN(number.Nomer),81,1,30,50);
					number.CreateConturs1();
					textBox3.Text = Convert.ToString(number.leftConturN)+";"+
									Convert.ToString(number.rightConturN)+";"+"\n";
					for (int i=0;i<number.leftConturN;i++)
						textBox3.Text += (Convert.ToString(number.leftConturMask[i])+",");
						textBox3.Text +=";";
					for (int i=0;i<number.rightConturN;i++)
						textBox3.Text += (Convert.ToString(number.rightConturMask[i])+",");
						textBox3.Text +=";";
						
					g1.DrawImage(number.ShowContur1(),121,1,40,50);
					for (int i=0;i<NumberRecognitor.masksN;i++)
						{g1.DrawImage(NumberRecognitor.ShowMaskN(i),
						              1+(i/6)*40, (60*(1+i))-(i/6)*300, 30, 50);}
					nfile++;					
				}
				else
					{textBox1.Text = "there is no file";}
			
		}
		void Button1Click(object sender, EventArgs e)
		{
			DrawingTheRecogn();
			
		}
		void Button2Click(object sender, EventArgs e)
		{
			DrawingTheRecogn();
		}
		void Button6Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.InitialDirectory = "." ;
   			openFileDialog1.Filter = "Image|*.jpg;*.bmp;*.gif";
  			openFileDialog1.Title = "Load an Image File";
		    openFileDialog1.ShowDialog();
		    
		    if (openFileDialog1.FileName != ""){
		    	sudRec = new SudokuRecognitor(new Bitmap(openFileDialog1.FileName));
		    	button7.Enabled = true;
				Graphics g1 = pictureBox1.CreateGraphics();
				g1.DrawImage(sudRec.picture,1,1,300,300);
		    }			
		}
		
		void Button7Click(object sender, EventArgs e)
		{			
			
			Rectangle rect = sudRec.seekSudoku();
			Graphics g1 = pictureBox1.CreateGraphics();
			g1.DrawImage(sudRec.picture1,1,1,300,300);
			
			if (rect.Height > 0)
			{
				//g1.DrawRectangle(Pens.Green,rect);
				textBox3.Text = Convert.ToString(rect.X)+";"+Convert.ToString(rect.Y)+";"+
					Convert.ToString(rect.Width)+";"+Convert.ToString(rect.Height)+";";
				button9.Enabled = true;
			}
			else
			{textBox3.Text = "на картинке нету судоку";}
		}
		void Button9Click(object sender, EventArgs e)
		{
			sudRec.recognit();
			textBox3.Text="";
			for (int j=0; j<sudRec.maxX; j++)
			{
				for (int i=0; i<sudRec.maxY; i++)				
					textBox3.Text = textBox3.Text + Convert.ToString(sudRec.board[i,j])+";";
				textBox3.Text += (char)13;
			}
			Graphics g1 = pictureBox1.CreateGraphics();
			g1.DrawImage(sudRec.picture1,1,1,300,300);
		}
		
	}
}
