using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour {

	public int CurrentX { set; get; }
	public int CurrentY { set; get; }
	public bool isWhite;

	public void SetPostion (int x, int y) {
		CurrentX = x;
		CurrentY = y;
	}

	public virtual bool PossibleMove (int x, int y) {
		return true;
	}
}
