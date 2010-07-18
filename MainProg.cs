// Copyright (C) 2008 Daniel Maturana
// This file is part of KamikazeMazeEdit.
// 
// KamikazeMazeEdit is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// KamikazeMazeEdit is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with KamikazeMapEdit. If not, see <http://www.gnu.org/licenses/>.
// 
// MainProg.cs 
// User: acantostega at 8:19 PM 7/14/2008
//

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;

namespace kamikazeMazeEdit
{
    public class MainProg
    {
        public static void Main() {
            //System.Console.WriteLine("bla");
            MapEditorForm mef = new MapEditorForm();
            //mef.Run();
            Application.Run(mef);
        }
    }
}
