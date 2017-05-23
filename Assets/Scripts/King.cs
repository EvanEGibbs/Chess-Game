using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman {

	public override bool[,] PossibleMove() {
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//Top Side
		i = CurrentX - 1;
		j = CurrentY + 1;
		if (CurrentY != 7){
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans[i, j];
					if (c == null) {
						r[i, j] = true;
					} else if (c.isWhite != isWhite) {
						r[i, j] = true;
					}
				}
				i++;
			}
		}
		//Bottom Side
		i = CurrentX - 1;
		j = CurrentY - 1;
		if (CurrentY != 0) {
			for (int k = 0; k < 3; k++) {
				if (i >= 0 && i < 8) {
					c = BoardManager.Instance.Chessmans[i, j];
					if (c == null) {
						r[i, j] = true;
					}
					else if (c.isWhite != isWhite) {
						r[i, j] = true;
					}
				}
				i++;
			}
		}

		//Middle Left
		if(CurrentX != 0) {
			c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
			if (c == null || c.isWhite != isWhite) {
				r[CurrentX - 1, CurrentY] = true;
			}
		}
		//Middle Right
		if (CurrentX != 7) {
			c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
			if (c == null || c.isWhite != isWhite) {
				r[CurrentX + 1, CurrentY] = true;
			}
		}

		return r;
	}
}
