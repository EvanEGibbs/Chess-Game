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

		//castling right
		if (!HasMoved) {
			//check all three spaces aren't threatened
			if(!BoardManager.Instance.ThreatenedSpace(CurrentX, CurrentY) && !BoardManager.Instance.ThreatenedSpace(CurrentX +1, CurrentY) && !BoardManager.Instance.ThreatenedSpace(CurrentX +2, CurrentY)) {
				//check that two spaces to the right are empty
				if (BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY] == null && BoardManager.Instance.Chessmans[CurrentX + 2, CurrentY] == null) {
					//white
					if (BoardManager.Instance.isWhiteTurn) {
						//if the rook hasn't moved
						if (BoardManager.Instance.Chessmans[7, 0] != null) {
							Chessman possibleRook = BoardManager.Instance.Chessmans[7, 0].GetComponent<Chessman>();
							if (possibleRook.GetType() == typeof(Rook) && possibleRook.HasMoved == false) {
								r[CurrentX + 2, CurrentY] = true;
							}
						}
					}
					//black
					else {
						if (BoardManager.Instance.Chessmans[7, 7] != null) {
							Chessman possibleRook = BoardManager.Instance.Chessmans[7, 7].GetComponent<Chessman>();
							if (possibleRook.GetType() == typeof(Rook) && possibleRook.HasMoved == false) {
								r[CurrentX + 2, CurrentY] = true;
							}
						}
					}
				}
			}
		}

		//castling left
		if (!HasMoved) {
			//check all three spaces aren't threatened
			if (!BoardManager.Instance.ThreatenedSpace(CurrentX, CurrentY) && !BoardManager.Instance.ThreatenedSpace(CurrentX - 1, CurrentY) && !BoardManager.Instance.ThreatenedSpace(CurrentX - 2, CurrentY)) {
				//check that two spaces to the right are empty
				if (BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY] == null && BoardManager.Instance.Chessmans[CurrentX - 2, CurrentY] == null) {
					//white
					if (BoardManager.Instance.isWhiteTurn) {
						//if the rook hasn't moved
						if (BoardManager.Instance.Chessmans[0, 0] != null) {
							Chessman possibleRook = BoardManager.Instance.Chessmans[0, 0].GetComponent<Chessman>();
							if (possibleRook.GetType() == typeof(Rook) && possibleRook.HasMoved == false) {
								r[CurrentX - 2, CurrentY] = true;
							}
						}
					}
					//black
					else {
						if (BoardManager.Instance.Chessmans[0, 7] != null) {
							Chessman possibleRook = BoardManager.Instance.Chessmans[0, 7].GetComponent<Chessman>();
							if (possibleRook.GetType() == typeof(Rook) && possibleRook.HasMoved == false) {
								r[CurrentX - 2, CurrentY] = true;
							}
						}
					}
				}
			}
		}

		return r;
	}
}
