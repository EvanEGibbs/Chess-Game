using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Chessman {

	public override bool[,] PossibleMove() {
		bool[,] r = new bool[8, 8];
		Chessman c, c2;
		int[] e = BoardManager.Instance.EnPassantMove;

		//white team move
		if (isWhite) {
			//diagonal left
			//check if an opponent's pawn's move made this piece elegable for en passant
			if (e[0] == CurrentX -1 && e[1] == CurrentY + 1) {
				r[CurrentX - 1, CurrentY + 1] = true;
			}
			//if an opponent's piece is at the diagonal, the pawn can move there.
			if (CurrentX != 0 && CurrentY != 7) {
				c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY + 1];
				if (c != null && !c.isWhite) {
					r[CurrentX - 1, CurrentY + 1] = true;
				}
			}
			//diagonal right
			if (e[0] == CurrentX + 1 && e[1] == CurrentY + 1) {
				r[CurrentX + 1, CurrentY + 1] = true;
			}
			if (CurrentX != 7 && CurrentY != 7) {
				c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY + 1];
				if (c != null && !c.isWhite) {
					r[CurrentX + 1, CurrentY + 1] = true;
				}
			}
			//middle
			if (CurrentY != 7) {
				c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
				if (c == null) {
					r[CurrentX, CurrentY + 1] = true;
				}
			}
			//middle on first move
			if (CurrentY == 1) {
				c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
				c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 2];
				if (c == null && c2 == null) {
					r[CurrentX, CurrentY + 2] = true;
				}
			}
		}
		//black team move
		else {
			//diagonal left
			if (CurrentY > 0) {
				if (e[0] == CurrentX - 1 && e[1] == CurrentY - 1) {
					r[CurrentX - 1, CurrentY - 1] = true;
				}
				if (CurrentX != 0 && CurrentY != 0) {
					c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY - 1];
					if (c != null && c.isWhite) {
						r[CurrentX - 1, CurrentY - 1] = true;
					}
				}
				//diagonal right
				if (e[0] == CurrentX + 1 && e[1] == CurrentY - 1) {
					r[CurrentX + 1, CurrentY - 1] = true;
				}
				if (CurrentX != 7 && CurrentY != 0) {
					c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY - 1];
					if (c != null && c.isWhite) {
						r[CurrentX + 1, CurrentY - 1] = true;
					}
				}
				//middle
				if (CurrentY != 0) {
					c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
					if (c == null) {
						r[CurrentX, CurrentY - 1] = true;
					}
				}
				//middle on first move
				if (CurrentY == 6) {
					c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
					c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 2];
					if (c == null && c2 == null) {
						r[CurrentX, CurrentY - 2] = true;
					}
				}
			}
		}

		return r;
	}
	
}
