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

// AskRowCols.cs 
// User: acantostega at 7:34 PM 7/15/2008
//

using System;
using System.Windows.Forms;
using System.Drawing;

namespace kamikazeMazeEdit
{
    
    public class AskRowCols : Form
    {
        NumericUpDown rowUD = new NumericUpDown();
        NumericUpDown colUD = new NumericUpDown();
        
        public AskRowCols()
        {
            // this settings are meant to 
            // simulate a dialog box
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.HelpButton = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Maze dimensions";
            this.Size = new Size(320,120);
            
            //this.AutoSize = true;
            int delta = 80;
            int margin = 10;
            
            // this is a just a hack to arrange widgets
            // since I'm not using VS and that gui drag and drop thingy
            
            Label rowLbl = new Label();
            rowLbl.Text = "Rows: ";
            Label colLbl = new Label();
            colLbl.Text = "Columns: ";
            rowLbl.Width = colLbl.Width = delta - 2*margin;
            rowUD.Maximum = colUD.Maximum = 28;
            rowUD.Minimum = colUD.Minimum = 1;
            rowUD.Value = colUD.Value = 2;
            rowUD.Increment = colUD.Increment = 1;
            rowUD.DecimalPlaces = colUD.DecimalPlaces = 0;
            rowUD.Width = colUD.Width = delta - 2*margin;
            
            rowUD.Top = colUD.Top = rowLbl.Top = colLbl.Top = margin;
            rowLbl.Left = margin;
            rowUD.Left = delta + margin;
            
            colLbl.Left = 2*delta + margin;
            colUD.Left = 3*delta + margin;
            
            Button okBtn = new Button();
            okBtn.Text = "&Ok";
            okBtn.DialogResult = DialogResult.OK;
            Button cancelBtn = new Button();
            cancelBtn.Text = "&Cancel";
            cancelBtn.DialogResult = DialogResult.Cancel;
            
            okBtn.Top = cancelBtn.Top = delta/2 + margin;
            okBtn.Left = delta + margin;
            cancelBtn.Left = 2*delta + margin;
            
            this.AcceptButton = okBtn;
            this.CancelButton = cancelBtn;
            
            this.Controls.AddRange(new Control[] {okBtn, cancelBtn, rowLbl, colLbl, rowUD, colUD});
            
        }
        
        public int Rows {
            get {
                return (int) rowUD.Value; 
            }
        }
        public int Cols {
            get {
                return (int) colUD.Value;
            }
        }
            
    }
}
