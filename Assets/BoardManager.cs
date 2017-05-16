using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public List<GameObject> chessmanPrefabs;

	private List<GameObject> activeChessman;

	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f;

	private int selectionX = -1;
	private int selectionY = -1;

	private void Update() {
		UpdateSelection();
		DrawChessboard();
	}
	
	private void UpdateSelection() {
		if (!Camera.main) {
			return;
		}
		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane"))) {
			//Debug.Log(hit.point);
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.z;
		} else {
			selectionX = -1;
			selectionY = -1;
		}
	}

	private void DrawChessboard() {

		//draw the chess board
		Vector3 widthLine = Vector3.right * 8;
		Vector3 heighthLine = Vector3.forward * 8;

		for (int i = 0; i <= 8; i++) {
			Vector3 startWidthLine = Vector3.forward * i;
			Debug.DrawLine(startWidthLine, startWidthLine + widthLine);
			Vector3 startHeighthLine = Vector3.right * i;
			Debug.DrawLine(startHeighthLine, startHeighthLine + heighthLine);
		}

		// Draw the slection
		if (selectionX >= 0 && selectionY >= 0) {
			Debug.DrawLine(
				Vector3.forward * selectionY + Vector3.right * selectionX,
				Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
			Debug.DrawLine(
				Vector3.forward * (selectionY +1) + Vector3.right * selectionX,
				Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
		}
	}
}
