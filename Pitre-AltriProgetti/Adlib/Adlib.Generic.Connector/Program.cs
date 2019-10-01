////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Adlib.Director.SampleApplication
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
