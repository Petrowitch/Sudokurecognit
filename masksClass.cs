/*
 * Created by SharpDevelop.
 * User: Taras.Omarov
 * Date: 23.10.2015
 * Time: 9:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace DefaultNamespace
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class NumberRecognitor
	{
		
		private static int [,,]masks;
		private static int masksX,masksY,masksN;
				
			
		public NumberReconitor(String maskFileName, Bitmap )
		{
			if (null==masks)
			{
				setMasks(maskFileName)
			}
					
		}
		
		
		public void setMasks (String maskFileName)
		{
			
		}
	}
}
