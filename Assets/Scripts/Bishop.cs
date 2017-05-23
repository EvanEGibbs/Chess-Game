using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman {

	public override bool[,] PossibleMove() {
		bool[,] r = new bool[8, 8];

		Chessman c;
		int i, j;

		//forward left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j++;
			if (i < 0 || j >= 8) {
				break;
			}
			c = BoardManager.Instance.Chessmans[i, j];
			if (c == null) {
				r[i, j] = true;
			}
			else {
				if (c.isWhite != isWhite) {
					r[i, j] = true;
				}
				break;
			}
		}
		//forward right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j++;
			if (i >= 8 || j >= 8) {
				break;
			}
			c = BoardManager.Instance.Chessmans[i, j];
			if (c == null) {
				r[i, j] = true;
			}
			else {
				if (c.isWhite != isWhite) {
					r[i, j] = true;
				}
				break;
			}
		}

		//backward left
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i--;
			j--;
			if (i < 0 || j < 0) {
				break;
			}
			c = BoardManager.Instance.Chessmans[i, j];
			if (c == null) {
				r[i, j] = true;
			}
			else {
				if (c.isWhite != isWhite) {
					r[i, j] = true;
				}
				break;
			}
		}
		//Backward right
		i = CurrentX;
		j = CurrentY;
		while (true) {
			i++;
			j--;
			if (i >= 8 || j < 0) {
				break;
			}
			c = BoardManager.Instance.Chessmans[i, j];
			if (c == null) {
				r[i, j] = true;
			}
			else {
				if (c.isWhite != isWhite) {
					r[i, j] = true;
				}
				break;
			}
		}

		return r;
	}
}
