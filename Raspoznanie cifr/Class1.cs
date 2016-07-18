/*
 * Created by SharpDevelop.
 * User: Taras.Omarov
 * Date: 23.10.2015
 * Time: 16:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Drawing;
using System.Text;

namespace Raspoznanie_cifr
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class NumberRecognitor
	{		
		public static int [,,]masks;
		public static int []nombers;
		public static int masksX,masksY,masksN;
		
		public int [,] localMask;
		public Bitmap picture;
		public int [,] localMask1;
		
		public  int [] leftConturMask;
		public  int leftConturN;
		public  int [] rightConturMask;
		public  int rightConturN;
		
		private int blackPixels;
		private float percentage;
		private double coef;
		
		public int rezult;
		public int Nomer;
		
		public NumberRecognitor(Bitmap numberPic, String maskFileName = "MASKS.txt")
		{			
			if (null==masks)
			{
				setMasks(maskFileName);
			}
			picture = (Bitmap)numberPic.Clone();
			blackPixels = 0;
			coef =0.8;// ((3.0) / 4)
		}
		
		public int recognize ()
		{			
			if (0==pictureCut())
			{return 0;} // если картинка пуста сразу возвращаем 0
			
			createMask();//создаем маску
			rezult = compareMasks(); //находим подходящую маску для определения числа и сохраняем в результ
			return rezult; //возвращаем результ
		}
		
		public int pictureCut ()
		{
			Bitmap tempimage = (Bitmap)picture.Clone();
			int startX = tempimage.Width;
			int startY = tempimage.Height;
			int lastX = 0;
			int lastY = 0;			
			
			//вычесление прямоугольника
			for (int j=1;j<tempimage.Height;j++)			
				for (int i=1;i<tempimage.Width;i++)
				{					
					if ((tempimage.GetPixel(i,j).GetBrightness()<0.5))
					{
						if (lastX<i) lastX=i;
						if (lastY<j) lastY=j;
						if (startX>i) startX=i;
						if (startY>j) startY=j;
						blackPixels++;
					}
				}							
					
			if ((lastX<=startX)||(lastY<=startY))
			{
				return 0;
			}
				
			//если сторона не делится нацело на размер маски, 
			//то подгоняем размеры с двух сторон пропорционально
			int temp=masksX-(lastX-startX)%masksX;			
			if (temp<masksX)
			{
				lastX+=(temp/2);//половинка с округлением
				startX-=(temp-(temp/2));//остаток
			}			
			
			temp=masksY-(lastY-startY)%masksY;
			if (temp<masksY)
			{
				lastY+=(temp/2);
				startY-=(temp-(temp/2));
			}	
			
			//обрезка
			Rectangle cutRect = new Rectangle(startX,startY,lastX-startX,lastY-startY);			
			picture = tempimage.Clone(cutRect,tempimage.PixelFormat);
			percentage = (float)blackPixels/(picture.Width*picture.Height);
			return 1;
						
		}
		
		
		public Bitmap ShowContur()	
		{
			Bitmap tempbit = new Bitmap(40,50,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g1 = Graphics.FromImage(tempbit);
			g1.FillRectangle(Brushes.White,1,1,tempbit.Width,tempbit.Height);
			Pen p1 = new Pen(Color.Black,1);
			//int width = tempbit.Width / 8;
			//int height = tempbit.Height / (leftConturN - 1);
			
			for (int i=0;i<leftConturN-1;i++)
				g1.DrawLine(p1,
				           	(1 + ((tempbit.Width / 4)*(1 - leftConturMask[i]))),//x1
				           	(tempbit.Height / (leftConturN-1)) * i,//y1
				          	(1 + ((tempbit.Width / 4)*(1 - leftConturMask[i+1]))),//x2
				          	(tempbit.Height / (leftConturN-1)) * (i+1));//y2
				
			
			for (int i=0;i<rightConturN-1;i++)
				g1.DrawLine(p1,
				           	(1 + ((tempbit.Width / 4)*(3 + rightConturMask[i]))),//x1
				           	(tempbit.Height / (rightConturN - 1)) * i,//y1
				          	(1 + ((tempbit.Width / 4)*(3 + rightConturMask[i+1]))),//x2
				          	(tempbit.Height / (rightConturN - 1)) * (i+1));//y2
			
			return (Bitmap)tempbit.Clone();
		}
		
		public Bitmap ShowContur1()
		{
			Bitmap tempbit = new Bitmap(40,50,System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g1 = Graphics.FromImage(tempbit);
			g1.FillRectangle(Brushes.White,1,1,tempbit.Width,tempbit.Height);
			Pen p1 = new Pen(Color.Black,1);
			int width = tempbit.Width / 4;
			int height;
			if (leftConturN>0)
			{
				height = tempbit.Height / leftConturN;			
				for (int i=0;i<leftConturN;i++)				
					/*g1.DrawArc (p1, 
					            ((2 * width) - (width * Math.Sign(leftConturMask[i]))) , //x1
					            height * (i + (((leftConturMask[i]+3)%4)/2)) , //y1
					            2 * width, //Width
					            height,  //Height
					            180 - (180 * (Math.Abs(leftConturMask[i]) / 2)) + (45 * (Math.Sign(leftConturMask[i])+1)),
					            90);*/
				{
					g1.DrawLine(p1,	//	            
					            (1 + (width * (1 + (leftConturMask[i] % 2)))),//x1
					            (height * i),//y1
					            (1 + ((width / 2) * (2 - (leftConturMask[i] / 2)))),//x2
					            ((height * i) + (height / 2))); //y2
					g1.DrawLine(p1,	//	            				            
					            (1 + ((width / 2) * (2 - (leftConturMask[i] / 2)))),//x1
					            ((height * i) + (height / 2)), //y1
					            (1 + (width * (1 - (leftConturMask[i] % 2)))),//x1
					            (height * (i+1)));//y1
					            
				}
			}
			if (rightConturN>0)
			{
				height = tempbit.Height / rightConturN;
				for (int i=0;i<rightConturN;i++)				
				{
					g1.DrawLine(p1,	//	            
					            (1 + (width * (3 + (rightConturMask[i] % 2)))),//x1
					            (height * i),//y1
					            (1 + ((width / 2) * (6 - (rightConturMask[i] / 2)))),//x2
					            ((height * i) + (height / 2))); //y2
					g1.DrawLine(p1,	//	            				            
					            (1 + ((width / 2) * (6 - (rightConturMask[i] / 2)))),//x1
					            ((height * i) + (height / 2)), //y1
					            (1 + (width * (3 - (rightConturMask[i] % 2)))),//x1
					            (height * (i+1)));//y1
					            
				}
			}
			return tempbit;
		}
		
		public void CreateConturs1 ()
		{
			int [] x   = new int[picture.Height];
			int [] dx  = new int[picture.Height];
			int [] ddx = new int[picture.Height];
			
			int [] tempСontur = new int[picture.Height];
			int conturN = 0;
			
			int lastdx = 0;
			int lastddx = 0;
			int znakdx;
			int znakddx;
			bool change = false;
			int k = 0;
			
			for (int j=0; j<picture.Height; j++)
			{
				int i=0;				
				while ((i < picture.Width) && (0.5 < picture.GetPixel(i,j).GetBrightness()))
					{i++;} //ищем значение по X для построения кривой				
								
				if (i < picture.Width-1)
				{
					x[k] = i; //заносим значения по X
					if (k > 2) //если любой елемент после второго, то
					{
						dx[k]=x[k]-x[k-1]; //первая производная
						ddx[k]=dx[k]-dx[k-1]; //вторая производная
						
						znakdx = Math.Sign(dx[k]);
						znakddx = Math.Sign(ddx[k]);								
						
						if (znakdx == lastdx)
							{}
						else if (znakdx == 0)
							{znakdx = lastdx;}
						else if (lastdx == 0)
							{lastdx = znakdx;}
						else 
							{change = true;}
							
						/*if (znakddx == lastddx)
							{}
						else if (znakddx == 0)
							{znakddx = lastddx;}
						else if (lastddx == 0)
							{lastddx = znakddx;}
						else 
							{change = true;}	*/
						
						if (change)
						{
							tempСontur[conturN++] = 2*lastdx + lastddx;
							lastdx = znakdx;
							lastddx = znakddx;
							change = false;
						}												
					}
					else if (1 == k) //если второй елемент
						dx[k]=x[k]-x[k-1]; //то только первая произвондная
						else if (2 == k)
							{
							dx[k]=x[k]-x[k-1]; //первая производная
							ddx[k]=dx[k]-dx[k-1]; //вторая производная
							lastdx = Math.Sign(dx[k]);
							lastddx = Math.Sign(ddx[k]);
							}
					k++;
				}
				else if (k > 0)
				{
					tempСontur[conturN++] = 2*lastdx + lastddx;
					break;
				}//закрыть последний контур								
				
			}
			leftConturN = conturN;
			leftConturMask = new int[conturN];
			for (int i=0;i<conturN;i++)
				leftConturMask[i] =tempСontur[i];
						
			conturN = 0;			
			lastdx = 0;
			lastddx = 0;
			change = false;
			k = 0;
			
			for (int j=0; j<picture.Height; j++)
			{
				int i=0;				
				while ((i < picture.Width) && (0.5 < picture.GetPixel(picture.Width-i-1,j).GetBrightness()))
					{i++;} //ищем значение по X для построения кривой				
								
				if (i < picture.Width)
				{
					x[k] = i; //заносим значения по X
					if (k > 2) //если любой елемент после второго, то
					{
						dx[k]=x[k]-x[k-1]; //первая производная
						ddx[k]=dx[k]-dx[k-1]; //вторая производная
						
						znakdx = Math.Sign(dx[k]);
						znakddx = Math.Sign(ddx[k]);								
						
						if (znakdx == lastdx)
							{}
						else if (znakdx == 0)
							{znakdx = lastdx;}
						else if (lastdx == 0)
							{lastdx = znakdx;}
						else 
							{change = true;}
							
						if (znakddx == lastddx)
							{}
						else if (znakddx == 0)
							{znakddx = lastddx;}
						else if (lastddx == 0)
							{lastddx = znakddx;}
						else 
							{change = true;}	
						
						if (change)
						{
							tempСontur[conturN++] = 2*lastdx + lastddx;
							lastdx = znakdx;
							lastddx = znakddx;
							change = false;
						}												
					}
					else if (1 == k) //если второй елемент
						dx[k]=x[k]-x[k-1]; //то только первая произвондная
						else if (2 == k)
							{
							dx[k]=x[k]-x[k-1]; //первая производная
							ddx[k]=dx[k]-dx[k-1]; //вторая производная
							lastdx = Math.Sign(dx[k]);
							lastddx = Math.Sign(ddx[k]);
							}
					k++;
				}
				else if (k > 0)
				{
					tempСontur[conturN++] = 2*lastdx + lastddx;
					break;
				}//закрыть последний контур								
				
			}
			rightConturN = conturN;
			rightConturMask = new int[conturN];
			for (int i=0; i<conturN; i++)
				rightConturMask[i] = tempСontur[i];
		}
		
		
		public void CreateConturs ()
		{			
			//analiz
			int [] tempContur = new int[picture.Height];
			int [] tempMask = new int[picture.Height];
			int znakRaznici;
			
			int conturN=0;
			int lastZnak=1;			
			
			for (int j=0; j<picture.Height; j++)
			{
				int i=0;
				while ((i < picture.Width) && (0.5 < picture.GetPixel(i,j).GetBrightness()))
					{i++;}
				if (i<=picture.Width)
				{
					tempContur[j] = i;					
					if (j>0)
					{
						if (0==(tempContur[j-1]-tempContur[j]))
							{znakRaznici=lastZnak;}//если прямая то оставляем без изменений						
						else if ((tempContur[j-1]-tempContur[j])>0)
								{znakRaznici = 1;}
							else
								{znakRaznici = -1;}
						
						if 	(znakRaznici!=lastZnak)
						{
							tempMask[conturN++]=lastZnak;
							lastZnak = znakRaznici;
						}
					}
				}
			}
			leftConturN = conturN+2;
			leftConturMask = new int[conturN+2];
			leftConturMask[0] = -1;
			for (int i=0;i<conturN;i++)
				{leftConturMask[i+1]=tempMask[i];}
			leftConturMask[conturN+1] = -1;
			
			//povtor dlya pravogo
			conturN=0;
			lastZnak=1;		
			
			for (int j=0;j<picture.Height;j++)
			{
				int i=0;
				while ((i < picture.Width)&&(0.5 < picture.GetPixel(picture.Width-i-1,j).GetBrightness()))
					{i++;}
				if (i < picture.Width)
				{
					tempContur[j] = i;					
					if (j>0)
					{
						if (0==(tempContur[j-1] - tempContur[j]))
							{znakRaznici=lastZnak;}//если прямая то оставляем без изменений						
						else if ((tempContur[j-1] - tempContur[j])>0)
								{znakRaznici = 1;}
							else
								{znakRaznici = -1;}
						
						if 	(znakRaznici!=lastZnak)
						{
							tempMask[conturN++] = lastZnak;
							lastZnak = znakRaznici;
						}
					}
				}
			}		
			rightConturN = conturN+2;			
			rightConturMask = new int[conturN+2];
			rightConturMask[0] = -1;
			for (int i=0;i<conturN;i++)
				{rightConturMask[i+1]=tempMask[i];}
			rightConturMask[conturN+1] = -1;
		}
		
		
		public void createMask ()
		{
			int pixelCol = 0;
			localMask = new int[masksX,masksY];
			for(int j=0;j<masksY;j++)
			{
				for(int i=0;i<masksX;i++)
				{
					int blackPixelCell=0;
					int pixelCell=0;
					for(int l=(1+j*(picture.Height/masksY));l<((j+1)*(picture.Height/masksY));l++)
						for (int k=(1+i*(picture.Width/masksX));k<((i+1)*(picture.Width/masksX));k++)
						{
							if ((k<picture.Width)&&(l<picture.Height))							
								pixelCol =
									picture.GetPixel(k,l).R+
									picture.GetPixel(k,l).G+
									picture.GetPixel(k,l).B;
							else 
								pixelCol = 500;
							
							if (pixelCol<400)
								blackPixelCell++;
							
							pixelCell++;
						}
					
					//if ((((float)blackPixelCell/pixelCell)>(percentage/3)*2)||(localpercentage>(percentage/3)*2))
					if (((float)blackPixelCell/pixelCell)>(percentage*coef))
						{this.localMask[i,j] = 1;}					
					else
						{this.localMask[i,j] = 0;}
				}
			}
		}
		
		public Bitmap ShowLocalMask()
		{
			Bitmap tempbit = new Bitmap(masksX*10, masksY*10, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g1 = Graphics.FromImage(tempbit);
			for (int j=0;j<masksY;j++)			
				for (int i=0;i<masksX;i++)				
					if (1==localMask[i,j])
						{g1.FillRectangle(Brushes.Black, 1+i*10, 1+j*10, 10, 10);}
					else
						{g1.FillRectangle(Brushes.White, 1+i*10, 1+j*10, 10, 10);}						
					
			return (Bitmap)tempbit.Clone();
		}
		
		
		public static Bitmap ShowMaskN (int n)
		{
			Bitmap tempbit = new Bitmap(masksX*10, masksY*10, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			Graphics g1 = Graphics.FromImage(tempbit);
			for (int j=0;j<masksY;j++)			
				for (int i=0;i<masksX;i++)				
					if (1==masks[n,i,j])
						{g1.FillRectangle(Brushes.Black, 1+i*10, 1+j*10, 10, 10);}
					else if (2==masks[n,i,j])
							{g1.FillRectangle(Brushes.Gray, 1+i*10, 1+j*10, 10, 10);}
						else
							{g1.FillRectangle(Brushes.White, 1+i*10, 1+j*10, 10, 10);}
						
			return (Bitmap)tempbit.Clone();
		}
		
		public int compareMasks()
		{
			int output=0;
			int n=0;
			while ((n<masksN)&&(output>=0))
			{
				bool same=true;
				for (int j = 0;j<masksY;j++)				
					for (int i = 0;i<masksX;i++)					
						if ((2==masks[n,i,j])||(masks[n,i,j]==localMask[i,j]))
							{ }//если совпадают ищем дальше 
						else
							{same = false;}//если не совпадает ставим
				
				if (same)				
					if (0 == output)
						{output = nombers[n];Nomer = n;}//записать распознаное значение
					else
						{return -1;}//распознаных значений больше 1				
					
				n++;
			}
			return output;//
		}
		
		public void setMasks (String maskFileName)
		{			
			if (File.Exists(maskFileName))
       		{   // Open the text file using a stream reader.
            	using (StreamReader sr = File.OpenText(maskFileName))
            	{	        		
	        		String [] firstLine = strSlasher(sr.ReadLine(),';');
	        		masksX = Convert.ToInt32(firstLine[0]);
	        		masksY = Convert.ToInt32(firstLine[1]);
	        		masksN = Convert.ToInt32(firstLine[2]);
	        		masks = new int[masksN,masksX,masksY];
	        		nombers = new int[masksN];
	        		for (int n=0;n<masksN;n++)
	        		{
	        			String [] head = strSlasher(sr.ReadLine(),':');
	        			nombers[n] = Convert.ToInt32(head[0]);
	        			for (int j=0;j<masksY;j++)
	        			{
	        				for (int i=0;i<masksX;i++)
	        					{masks[n,i,j] = sr.Read()-48;}
	        				sr.ReadLine();
	        			}	        				
	        		}
            	}
        	}        
		}
		
		private String[] strSlasher (String line,char Breaker)
		{
			if (line==null || line=="")
				{return null;}
			StringBuilder copyof = new StringBuilder(line);
			int start,length,n;
			String []output;
			String []tempString = new String[line.Length];
			//svalka 
			/* 			
 			if (Char.GetUnicodeCategory(copyof[ctr]) == UnicodeCategory.DecimalDigitNumber)			 	
			*/
			int i=0;
			n=0;
			while (i<copyof.Length)				
			{
				start=i;
				while ((i < copyof.Length)&&(copyof[i] != Breaker))
					{i++;}
				length = (i - start);
				tempString[n++] = copyof.ToString(start, length);
				i++;
			}
			if (n>0)
			{
				output=new String[n];
				for (int j=0;j<n;j++)
					{output[j] = tempString[j];}
				return output;
			}
			return null;
		}
		
	}
}
