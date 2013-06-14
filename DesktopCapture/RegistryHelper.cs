// Copyright © 2005 by Omar Al Zabir. All rights are reserved.
// 
// If you like this code then feel free to go ahead and use it.
// The only thing I ask is that you don't remove or alter my copyright notice.
//
// Your use of this software is entirely at your own risk. I make no claims or
// warrantees about the reliability or fitness of this code for any particular purpose.
// If you make changes or additions to this code please mark your code as being yours.
// 
// website http://www.oazabir.com, email OmarAlZabir@gmail.com, msn oazabir@hotmail.com

using System;
using Microsoft.Win32;

namespace DesktopCapture
{

	/// <summary>
	/// Summary description for RegistryHelper.
	/// </summary>
	internal static class RegistryHelper
	{	
		private const string REGISTRY_VALUE = "StickOutStarter";

		public static void RegisterAtStartup()
		{
			// get reference to the HKLM registry key...
			RegistryKey rkHKLM = Registry.LocalMachine;
			RegistryKey rkRun;

			// get reference to Software\Microsoft\Windows\CurrentVersion\Run subkey
			// with permission to write to it...
			try
			{
				rkRun = 
					rkHKLM.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",true);
			}
			catch
			{
				// close the HKLM key...
				rkHKLM.Close();
				return;
			}
			
			try
			{
				// Check if there is any value already there
				//if( null != rkRun.GetValue("RSS Feeder") )
				//{
				// create a value with name same as the data...
				System.Reflection.Assembly ass = System.Reflection.Assembly.GetEntryAssembly();
				string directory = System.IO.Path.GetDirectoryName( ass.Location );
				
				string starterPath = System.IO.Path.Combine( directory, "DesktopCapture.exe" );

				rkRun.SetValue(REGISTRY_VALUE, starterPath);

				Console.WriteLine("Entry successfully created in the registry!");
				//}
			}
			catch
			{
				// error while creating entry...
				Console.WriteLine("Unable to create an entry for the application!");
			}

			// close the subkey...
			rkRun.Close();
			// close the HKLM key...
			rkHKLM.Close();
		}

		public static void UnregisterAtStartup()
		{
			// get reference to the HKLM registry key...
			RegistryKey rkHKLM = Registry.LocalMachine;
			RegistryKey rkRun;

			// get reference to Software\Microsoft\Windows\CurrentVersion\Run subkey
			// with permission to write to it...
			try
			{
				rkRun = 
					rkHKLM.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",true);
			}
			catch
			{
				// close the HKLM key...
				rkHKLM.Close();
				return;
			}

			try
			{
				rkRun.DeleteValue( REGISTRY_VALUE );
			}
			catch
			{
			}

			// close the subkey...
			rkRun.Close();
			// close the HKLM key...
			rkHKLM.Close();
		}
	}
}
