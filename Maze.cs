using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace kamikazeMazeEdit
{
    [Flags] // to be used as bit flags
    public enum MazeCell : byte
    {
        NONE = 0,
        UP = 1,
        DOWN = 2,
        LEFT = 4,
        RIGHT = 8,
        GOAL = 16,
        START = 32,
        ELECTROLYTE=64,
        KEY=128
    }

    public class Maze {
        MazeCell[,] mazeData;
        int rows;
        int cols;
        List<State> goalStates = new List<State>();
        List<State> startStates = new List<State>();
        List<State> electrolitos = new List<State>();
        List<State> llaves = new List<State>();

        /// <summary>
        /// return empty maze. Useful for mapping
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Maze(int rows, int cols, List<State> startsts, List<State> goalsts)
        {
            Init(rows, cols, startsts, goalsts);
        }
        
        public Maze(int rows, int cols) {
            Init(rows, cols, null, null);
        }

        public Maze(string filename)
        {
            rows = cols = -1;
            LoadMaze(filename);
            putBoundaries();
        }
        
        void Init(int rows, int cols, List<State> startsts, List<State> goalsts) {
            this.rows = rows;
            this.cols = cols;
            if (goalsts != null)
                this.goalStates = goalsts;
            if (startsts != null)
                this.startStates = startsts;
            this.mazeData = new MazeCell[rows, cols];
            
            foreach (State st in this.goalStates)
            {
                mazeData[st.Row, st.Col] |= MazeCell.GOAL;
            }
            foreach (State st in this.startStates)
            {
                mazeData[st.Row, st.Col] |= MazeCell.START;
            }
            putBoundaries();            
        }

        /// <summary>
        /// make a boundary around map so robot doesn't "escape"
        /// </summary>
        private void putBoundaries()
        {
            // top & bottom
            for (int c = 0; c < cols; ++c)
            {
                mazeData[0, c] |= MazeCell.UP;
                mazeData[rows - 1, c] |= MazeCell.DOWN;
            }
            // left & right
            for (int r = 0; r < rows; ++r)
            {
                mazeData[r, 0] |= MazeCell.LEFT;
                mazeData[r, cols - 1] |= MazeCell.RIGHT;
            }
        }

        public void SaveMaze(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            // write number of rows and cols
            sw.WriteLine(rows + " " + cols);
            // format:
            // row col up left down right
            for (int i=0; i < rows; ++i) {
				for (int j=0; j < cols; ++j) {					
					string line = "";
		            int row, col;
		            // reverse of correction in LoadMaze
		            row = rows - i -1;
		            col = j;
		            // or i, j?
					line += row + " ";
					line += col + " ";
					line += (mazeData[i,j] & MazeCell.UP) != 0 ? "1 " : "0 ";
					line += (mazeData[i,j] & MazeCell.LEFT) != 0 ? "1 " : "0 ";
					line += (mazeData[i,j] & MazeCell.DOWN) != 0 ? "1 " : "0 ";					
					line += (mazeData[i,j] & MazeCell.RIGHT) != 0 ? "1" : "0";
					sw.WriteLine(line);
            	}
			}
			sw.WriteLine("START");
			sw.WriteLine(startStates.Count);
			foreach (State st in startStates) {
				string line = String.Format("{0} {1} {2}", rows - st.Row - 1, st.Col, 
				                             StateUtils.HeadingToChar(st.Heading));
				sw.WriteLine(line);
			}
			
			sw.WriteLine("GOAL");			
			sw.WriteLine(goalStates.Count);
			foreach (State st in goalStates) {
				string line = String.Format("{0} {1} {2}", rows - st.Row - 1, st.Col, 
				                             StateUtils.HeadingToChar(st.Heading));				
				sw.WriteLine(line);
			}
			
			// write down arbitrary (irrelevant?) max search depth
			sw.WriteLine("5");
			sw.Close();
        }

        private void LoadMaze(string filename)
        {
            int lineno = 1;
            string line = "";
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                // read number of rows and cols (1st line)
                char[] separators = {' '};
                string[] line_elts = sr.ReadLine().Split(separators);
				if (line_elts.Length==0) {
					throw new MazeException("Expected rows and columns in line " + lineno);
				}
                lineno++;
                rows = int.Parse(line_elts[0]);
                cols = int.Parse(line_elts[1]);                
                //Debug.WriteLine(string.Format("Rows: {0}, {1}", rows, cols));
                mazeData = new MazeCell[rows, cols];
                int row, col, up, left, down, right;
                for (int i = 0; i < rows * cols; ++i)
                {
                    line_elts = sr.ReadLine().Trim().Split(separators);
                    lineno++;
                    // added "rows - " because origin is at lower left corner.
                    row = rows - int.Parse(line_elts[0]) - 1; 
                    col = int.Parse(line_elts[1]);
                    up = int.Parse(line_elts[2]);
                    left = int.Parse(line_elts[3]);
                    down = int.Parse(line_elts[4]);
                    right = int.Parse(line_elts[5]);
                    if (up == 1) mazeData[row, col] |= MazeCell.UP;
                    if (left == 1) mazeData[row, col] |= MazeCell.LEFT;
                    if (down == 1) mazeData[row, col] |= MazeCell.DOWN;
                    if (right == 1) mazeData[row, col] |= MazeCell.RIGHT;
                }
                line = sr.ReadLine().Trim(); lineno++;// read "START" line
                if (line != "START") {
                    throw new MazeException("Expected START at line " + lineno + ", read " + line, lineno);
                }
                
                line = sr.ReadLine().Trim(); lineno++; // read number of start states
                int num_starts = int.Parse(line);
                
                // read start states
                for (int i = 0; i < num_starts; ++i)
                {
                    State st = parseLine(sr); lineno++;
                    startStates.Add(st); 
                    mazeData[st.Row, st.Col] |= MazeCell.START;
                }
                line = sr.ReadLine().Trim(); lineno++; // read "GOAL" line
                System.Console.WriteLine("goal line:" + line);
                if (line != "GOAL") {
                    throw new MazeException("Expected GOAL at line " + lineno + ", read " + line, lineno);
                }
                line = sr.ReadLine().Trim(); lineno++; // read number of goal states
                int num_goals = int.Parse(line);
                
                // read goal states
                for (int i = 0; i < num_goals; i++)
                {
                    State st = parseLine(sr); lineno++;
                    goalStates.Add(st);
                    mazeData[st.Row, st.Col] |= MazeCell.GOAL;
                }
                try {
                    line = sr.ReadLine(); lineno++; // leer ELECTROLITOS
                    // if we have exception here then this file says nothing about
                    // electrolytes and so on
                } catch (EndOfStreamException ex) {
                    sr.Close(); 
                    System.Console.Error.WriteLine("ERROR: " + ex.Message);
                    return;
                }
                if (!line.Trim().Equals("ELECTROLITO")) {
                    // maybe allow for a stray endline
                    sr.Close(); 
                    return;
                }
                // read electrolytes
                line = sr.ReadLine().Trim(); lineno++; // leer num electrolitos
                int num_electrolitos = int.Parse(line);
                for (int i = 0; i < num_electrolitos; i++)
                {
                    State st = parseLine(sr); lineno++;
                    electrolitos.Add(st);
                    mazeData[st.Row, st.Col] |= MazeCell.ELECTROLYTE;
                }
                // read keys
                line = sr.ReadLine(); lineno++; // leer llaves
                if (!line.Equals("LLAVES"))
                {
                    throw new MazeException("expected LLAVES at line " + lineno + ", read " + line, lineno);
                }
                line = sr.ReadLine().Trim(); lineno++; // leer num llaves
                int num_llaves = int.Parse(line);
                for (int i = 0; i < num_llaves; i++)
                {
                    State st = parseLine(sr); lineno++;
                    llaves.Add(st);
                    mazeData[st.Row, st.Col] |= MazeCell.KEY;
                }                
                sr.Close();
            }
            catch (IndexOutOfRangeException ex)
            {
                //System.Console.WriteLine("Error reading maze in line: " + lineno);
                System.Console.Error.WriteLine("Error reading maze in line: " + lineno + ": ", ex.Message);
                throw new MazeException("Index out of bounds exception in line " + lineno + "\n" + line , lineno);
            }
            finally {
                if (sr != null) 
                    sr.Close();
            }
        }
        
        State parseLine(StreamReader sr) {
            char[] separators = {' '};
            string line = sr.ReadLine().Trim();
            string[] line_elts = line.Split(separators);
            // NOTE: row inversion
            int row = this.rows - int.Parse(line_elts[0]) - 1;
            int col = int.Parse(line_elts[1]);
            Heading head;
            if (line_elts.Length == 3) {
                head = StateUtils.CharToHeading(line_elts[2][0]);
            } else {
                head = Heading.NONE;
            }
            return new State(row, col, head);
        }

        /// <summary>
        /// For adding walls at runtime, eg. when mapping.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="heading"></param>
        public void AddWall(int row, int col, Heading heading) {
            switch (heading) {
                case Heading.DOWN:
                mazeData[row, col] |= MazeCell.DOWN;
                if (row < rows - 1) 
                    mazeData[row,col] |= MazeCell.UP;
                break;
                case Heading.LEFT:
                mazeData[row, col] |= MazeCell.LEFT;
                if (col > 0)
                    mazeData[row,col-1] |= MazeCell.RIGHT;
                break;
                case Heading.RIGHT:
                if (col < cols - 1) 
                    mazeData[row, col + 1] |= MazeCell.LEFT;
                mazeData[row, col] |= MazeCell.RIGHT;
                break;
                case Heading.UP:
                if (row > 0) 
                    mazeData[row - 1, col] |= MazeCell.DOWN;
                mazeData[row, col] |= MazeCell.UP;
                break;
                default:
                break;
            }
        }
        
        public void ToggleGoal(int row, int col) {
            mazeData[row, col] ^= MazeCell.GOAL;
            State gst = new State(row, col, Heading.NONE);
            if (goalStates.Contains(gst)) {
                goalStates.Remove(gst);
            } else {
                goalStates.Add(gst);
            }
        }
        
        public void ToggleStart(int row, int col, Heading heading) {
            mazeData[row, col] ^= MazeCell.START;
            State st = new State(row, col, heading);
            if (startStates.Contains(st)) {
                startStates.Remove(st);
            } else {
                startStates.Add(st);
            }
            
        }
        
        public void ToggleWall(int row, int col, Heading heading) {
            switch (heading) {
                case Heading.DOWN:
                mazeData[row, col] ^= MazeCell.DOWN;
                if (row < rows-1) 
                    mazeData[row+1, col] ^= MazeCell.UP;
                break;
                
                case Heading.LEFT:                
                mazeData[row, col] ^= MazeCell.LEFT;
                if (col > 0) 
                    mazeData[row,col-1] ^= MazeCell.RIGHT;
                break;
                
                case Heading.RIGHT:
                mazeData[row, col] ^= MazeCell.RIGHT;
                if (col < cols - 1) 
                    mazeData[row,col+1] ^= MazeCell.LEFT;
                break;
                
                case Heading.UP:
                mazeData[row, col] ^= MazeCell.UP;
                if (row > 0) 
                    mazeData[row-1, col] ^= MazeCell.DOWN;
                break;
                
                default:
                break;
            }
        }

        public void RemoveElectrolyte(int row, int col)
        {
            //Debug.Assert((mazeData[row, col] & MazeCell.ELECTROLYTE) != 0);
            //Debug.WriteLine("RemoveElectrolyte: electrolitos.count before: " + electrolitos.Count);
            mazeData[row, col] &= ~MazeCell.ELECTROLYTE;
            electrolitos.Remove(new State(row, col, Heading.NONE));
            //Debug.WriteLine("RemoveElectrolyte: electrolitos.count after: " + electrolitos.Count);
        }

        public void AddElectrolyte(int row, int col)
        {
            mazeData[row, col] |= MazeCell.ELECTROLYTE;
            electrolitos.Add(new State(row, col, Heading.NONE));
        }

        public void RemoveKey(int row, int col)
        {
            //Debug.Assert((mazeData[row, col] & MazeCell.KEY) != 0);
            mazeData[row, col] &= ~MazeCell.KEY;
            llaves.Remove(new State(row, col, Heading.NONE));
        }

        public override string ToString()
        {
            return "Maze object. Size (" + rows + ", " + cols + ") - " + base.ToString();
        }

        public MazeCell this[int row, int col]
        {
            get { return mazeData[row, col]; }
        }

        #region walls

        public bool WallDown(MazeCell cell)
        {
            return ((cell & MazeCell.DOWN) != 0);
        }

        public bool WallLeft(MazeCell cell)
        {
            return ((cell & MazeCell.LEFT) != 0);
        }
        public bool WallRight(MazeCell cell)
        {
            return ((cell & MazeCell.RIGHT) != 0);
        }
        public bool WallUp(MazeCell cell)
        {
            return ((cell & MazeCell.UP) != 0);
        }

        public bool WallDown(int row, int col)
        {
            return (mazeData[row, col] & MazeCell.DOWN) != 0;
        }

        public bool WallUp(int row, int col)
        {
            return (mazeData[row, col] & MazeCell.UP) != 0;
        }

        public bool WallLeft(int row, int col)
        {
            return (mazeData[row, col] & MazeCell.LEFT) != 0;
        }

        public bool WallRight(int row, int col)
        {
            return (mazeData[row, col] & MazeCell.RIGHT) != 0;
        } 
        #endregion

        #region properties

        public int Rows
        {
            get
            {
                return rows;
            }
        }
        public int Cols
        {
            get
            {
                return cols;
            }
        }
        public List<State> GoalStates
        {
            get
            {
                return goalStates;
            }
        }
        public List<State> Electrolitos
        {
            get
            {
                return electrolitos;
            }
        }
        public List<State> KeyStates
        {
            get
            {
                return llaves;
            }
        }

        public List<State> StartStates
        {
            get
            {
                return startStates;
            }
        }
        #endregion
    }

    public class MazeException : Exception
    {
        int lineno;
        public MazeException()
            : base()
        {
        }

        public MazeException(string message)
            : base(message)
        {
        }
        
        public MazeException(string message, int lineno)
            : base(message)
        {
            this.lineno = lineno;
        }

        public MazeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        public int LineNo {
            get { return lineno; }
        }
        
        
    }
}
