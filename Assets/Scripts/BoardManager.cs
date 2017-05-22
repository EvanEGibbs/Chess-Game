using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public static BoardManager Instance { set; get; }
	private bool[,] allowedMoves { set; get; }

	//array of positions on the board, stores a chessman
	public Chessman[,] Chessmans { set; get; }
	private Chessman selectedChessman;

	public List<GameObject> chessmanPrefabs;

	private List<GameObject> activeChessman;

	//offset to calculate center of the board square
	private const float TILE_SIZE = 1.0f;
	private const float TILE_OFFSET = 0.5f;

	//where the mouse is on the board, -1 is off the board
	private int selectionX = -1;
	private int selectionY = -1;

	//orientation for the pieces
	private Quaternion orientation = Quaternion.Euler(0,180, 0);

	public bool isWhiteTurn = true;

	private void Start() {
		Instance = this;
		SpawnAllChessmans();
	}

	private void Update() {
		//check where mouse is
		UpdateSelection();
		DrawChessboard();

		if (Input.GetMouseButtonDown(0)) {
			if (selectionX >= 0 && selectionY >= 0) {
				//if no piece selected
				if(selectedChessman == null) {
					//select the chessman clicked on
					SelectChessman(selectionX, selectionY);
				}
				//if piece is selected
				else {
					//move the chessman to space clicked on
					MoveChessman(selectionX, selectionY);
				}
			}
			//if clicked somewhere off the board
			else {
				BoardHighlights.Instance.HideHighlights();
				selectedChessman = null;
			}
		}
	}

	private void SelectChessman(int x, int y) {
		//if there is no chessman on the space clicked
		if (Chessmans[x, y] == null) {
			return;
		}
		//if not turn of the color of piece clicked on
		if (Chessmans[x,y].isWhite != isWhiteTurn) {
			return;
		}

		allowedMoves = Chessmans[x, y].PossibleMove();
		BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);

		//select the clicked on chess piece
		selectedChessman = Chessmans[x, y];
	}

	private void MoveChessman (int x, int y) {
		//if the move is possible
		if (allowedMoves[x,y]) {
			Chessman c = Chessmans[x, y];
			if (c != null && c.isWhite != isWhiteTurn) {
				//Capture a piece

				//if it is the king, end the game
				if (c.GetType() == typeof(King)) {
					// End the Game
					return;
				}
				activeChessman.Remove(c.gameObject);
				Destroy(c.gameObject);
			}
			//remove selected chessman from the array
			Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
			//move the piece
			selectedChessman.transform.position = GetTileCenter(x, y);
			//set the position variable within the chess piece class
			selectedChessman.SetPosition(x, y);
			//put the chessman back into the array in it's new position
			Chessmans[x, y] = selectedChessman;

			

			//switch player turn
			isWhiteTurn = !isWhiteTurn;
		}

		selectedChessman = null;
		BoardHighlights.Instance.HideHighlights();
	}
	
	private void UpdateSelection() {
		if (!Camera.main) {
			return;
		}
		RaycastHit hit;
		//if the the mouse is hitting the chess plane, update the selection variables with the x and y of the cursor
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane"))) {
			//Debug.Log(hit.point);
			selectionX = (int)hit.point.x;
			selectionY = (int)hit.point.z;
		} else {
			selectionX = -1;
			selectionY = -1;
		}
	}

	private void SpawnAllChessmans() {
		//list of all of the chessman prefabs
		activeChessman = new List<GameObject>();
		//the array of the board has it's size defined (takes in type Chessman)
		Chessmans = new Chessman[8, 8];

		//Spawn chessman, first input is piece type, second is x position, third is z position)

		//All Pawns
		for (int i = 0; i <= 7; i++) {
			SpawnChessman(5, i, 1);
			SpawnChessman(11, i, 6);
		}

		// Spawn white pieces

		//King
		SpawnChessman(0, 4, 0);
		//Queen
		SpawnChessman(1, 3, 0);
		//Rooks
		SpawnChessman(2, 0, 0);
		SpawnChessman(2, 7, 0);
		//Bishops
		SpawnChessman(3, 2, 0);
		SpawnChessman(3, 5, 0);
		//Knights
		SpawnChessman(4, 1, 0);
		SpawnChessman(4, 6, 0);

		// Spawn black pieces

		//King
		SpawnChessman(6, 4, 7);
		//Queen
		SpawnChessman(7, 3, 7);
		//Rooks
		SpawnChessman(8, 0, 7);
		SpawnChessman(8, 7, 7);
		//Bishops
		SpawnChessman(9, 2, 7);
		SpawnChessman(9, 5, 7);
		//Knights
		SpawnChessman(10, 1, 7);
		SpawnChessman(10, 6, 7);

	}

	private void SpawnChessman(int index, int x, int y) {
		//instantiate piece of index type in position x and y
		GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter (x, y), orientation) as GameObject;
		//set parent to the game board
		go.transform.SetParent(transform);
		//set chessmans array of the position of the new piece to the new piece
		Chessmans[x, y] = go.GetComponent<Chessman>();
		//store position information within the piece
		Chessmans[x, y].SetPosition(x, y);
		//add to list of chessmen
		activeChessman.Add(go);
	}

	private Vector3 GetTileCenter(int x, int z) {
		//takes in an x and y, returns a vector 3 with an offset so the pieces will be in the center of the space
		Vector3 origin = Vector3.zero;
		origin.x += (TILE_SIZE * x) + TILE_OFFSET;
		origin.z += (TILE_SIZE * z) + TILE_OFFSET;
		return origin;
	}

	private void DrawChessboard() {

		//draw the chess board
		Vector3 widthLine = Vector3.right * 8;
		Vector3 heighthLine = Vector3.forward * 8;

		//run 9 lines horizontal and vertically
		for (int i = 0; i <= 8; i++) {
			Vector3 startWidthLine = Vector3.forward * i;
			Debug.DrawLine(startWidthLine, startWidthLine + widthLine);
			Vector3 startHeighthLine = Vector3.right * i;
			Debug.DrawLine(startHeighthLine, startHeighthLine + heighthLine);
		}

		// Draw the selection if the selection is on the board
		if (selectionX >= 0 && selectionY >= 0) {
			Debug.DrawLine(
				//line drawn from bottom left
				Vector3.forward * selectionY + Vector3.right * selectionX,
				//to top right
				Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
			Debug.DrawLine(
				//and line drawn from top left
				Vector3.forward * (selectionY +1) + Vector3.right * selectionX,
				//to bottom right
				Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
		}
	}
}
