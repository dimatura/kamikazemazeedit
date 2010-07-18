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
//#define DEBUG
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace kamikazeMazeEdit {

    // enums 
    public enum Heading { NONE, UP, DOWN, LEFT, RIGHT }
    
    static class StateUtils {
        
        public static Heading CharToHeading(char c) {
            switch (c) {
            case 'u':
                return Heading.UP;
            case 'd':
                return Heading.DOWN;
            case 'l':
                return Heading.LEFT;
            case 'r':
                return Heading.RIGHT;
            default:
                return Heading.NONE;
            }
        }
		
		public static char HeadingToChar(Heading h) {
			switch (h) {
			case Heading.UP:
				return 'u';
			case Heading.DOWN:
				return 'd';
			case Heading.LEFT:
				return 'l';
			case Heading.RIGHT:
				return 'r';
			default:
				return ' ';
			}
		}
        
    }

    /// <summary>
    /// Un estado es una tupla (row, col, heading) que corresponde
    /// a un estado del robot en el laberinto. Es un struct puesto
    /// que es pequeno y usamos varios. Es inmutable (readonly).
    /// </summary>
    public struct State : IEquatable<State> {
        public readonly int Row;
        public readonly int Col;
        public readonly Heading Heading;

        public State(int row, int col, Heading heading) {
            this.Row = row;
            this.Col = col;
            this.Heading = heading;
        }
        public override bool Equals(object o) {
            if (!(o is State)) return false;
            return Equals((State) o);
        }
        public bool Equals(State st) {
            return (st.Row == this.Row && st.Col == this.Col && st.Heading == this.Heading);
        }

        public override String ToString() {
            return string.Format("(St: {0,2},{1,2},{2,5})", this.Row, this.Col, this.Heading);
        }
        public override int GetHashCode() {
            if (Row > Col)
                return Row * 31 + Col;
            else
                return Col * 31 + Row;
        }
        
    }

}//namespace
