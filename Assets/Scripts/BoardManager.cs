using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAF.Core;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

	public static BoardManager Instance { set; get; }
	private bool[,] allowedMoves { set; get; }

	public GameObject whitePromotion;
	public GameObject blackPromotion;
	private GameObject chessPlane;

	//array of positions on the board, stores a chessman
	public Chessman[,] Chessmans { set; get; }
	private Chessman selectedChessman;
	private Chessman promotionPiece;

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

	public GameObject blackWinsAnimation1;
	public GameObject blackWinsAnimation2;
	public GameObject whiteWinsAnimation1;
	public GameObject whiteWinsAnimation2;
	public GameObject flipBoardButton;
	public GameObject staleMate;
	public GameObject turnColorText1;
	public GameObject turnColorText2;

	private Material previousMat;
	public Material selectedMat;

	public int[] EnPassantMove { set; get; } 

	public bool isWhiteTurn = true;

	private void Start() {
		Instance = this;
		whitePromotion.SetActive(false);
		blackPromotion.SetActive(false);
		chessPlane = transform.FindChild("ChessPlane").gameObject;

		SpawnAllChessmans();
	}

	private void Update() {
		//check where mouse is
		UpdateSelection();
		DrawChessboard();

		if (Input.GetMouseButtonDown(0)) {
			if (selectionX >= 0 && selectionY >= 0) {
				//if no piece selected
				if (selectedChessman == null) {
					//select the chessman clicked on
					SelectChessman(selectionX, selectionY);
				}
				//if there is a piece where you click
				else if (selectedChessman != null && Chessmans[selectionX, selectionY] != null) {
					//if the piece is the same color, select it
					if (selectedChessman.isWhite == Chessmans[selectionX, selectionY].isWhite) {
						DeselectChessman();
						SelectChessman(selectionX, selectionY);
					}
					//if not, move the piece there
					else {
						MoveChessman(selectionX, selectionY);
					}
				}
				//if piece is selected and there is no piece where clicking on
				else {
					//move the chessman to space clicked on
					MoveChessman(selectionX, selectionY);
				}
			}
			//if clicked somewhere off the board
			else {
				BoardHighlights.Instance.HideHighlights();
				if (selectedChessman != null) { 
					selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
					selectedChessman = null;
				}
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

		bool hasAtLeastOneMove = false;

		allowedMoves = Chessmans[x, y].PossibleMove();

		checkIllegalMoves(x, y);

		for(int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if(allowedMoves[i, j]) {
					hasAtLeastOneMove = true;
				}
			}
		}
		if (!hasAtLeastOneMove) {
			return;
		}

		BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);

		//select the clicked on chess piece
		selectedChessman = Chessmans[x, y];
		previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
		selectedMat.mainTexture = previousMat.mainTexture;
		selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
	}

	private void MoveChessman (int x, int y) {
		//if the move is possible
		if (allowedMoves[x,y]) {
			Chessman c = Chessmans[x, y];
			if (c != null && c.isWhite != isWhiteTurn) {
				//Capture a piece
				activeChessman.Remove(c.gameObject);
				Destroy(c.gameObject);
			}

			if (x == EnPassantMove[0] && y == EnPassantMove[1]) {
				if (isWhiteTurn) {
					c = Chessmans[x, y - 1];
				}
				else {
					c = Chessmans[x, y + 1];
				}
				activeChessman.Remove(c.gameObject);
				Destroy(c.gameObject);
			}

			EnPassantMove[0] = -1;
			EnPassantMove[1] = -1;
			if (selectedChessman.GetType() == typeof(Pawn)) {
				//promotion
				//white
				if( y == 7) {
					promotionPiece = selectedChessman;
					whitePromotion.SetActive(true);
					chessPlane.SetActive(false);
				//black
				} else if (y == 0) {
					promotionPiece = selectedChessman;
					blackPromotion.SetActive(true);
					chessPlane.SetActive(false);
				}
				//check for en passant
				if (selectedChessman.CurrentY == 1 && y == 3) {
					EnPassantMove[0] = x;
					EnPassantMove[1] = y-1;
				} else if (selectedChessman.CurrentY == 6 && y == 4) {
					EnPassantMove[0] = x;
					EnPassantMove[1] = y+1;
				} 
			}

			//check for castling
			if (selectedChessman.GetType() == typeof(King)) {
				//castling right
				if (selectedChessman.CurrentX == 4 && x == 6) {
					//remove the rook (look after check for castling section for comments on moving a piece
					Chessman rook = Chessmans[7, y];
					Chessmans[7, y] = null;
					rook.transform.position = GetTileCenter(5, y);
					rook.HasMoved = true;
					rook.SetPosition(5, y);
					Chessmans[5, y] = rook;
				}
				//castling left
				if (selectedChessman.CurrentX == 4 && x == 2) {
					//remove the rook (look after check for castling section for comments on moving a piece
					Chessman rook = Chessmans[0, y];
					Chessmans[0, y] = null;
					rook.transform.position = GetTileCenter(3, y);
					rook.HasMoved = true;
					rook.SetPosition(3, y);
					Chessmans[3, y] = rook;
				}
			}

			//remove selected chessman from the array
			Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
			//move the piece
			selectedChessman.transform.position = GetTileCenter(x, y);
			selectedChessman.HasMoved = true;
			//set the position variable within the chess piece class
			selectedChessman.SetPosition(x, y);
			//put the chessman back into the array in it's new position
			Chessmans[x, y] = selectedChessman;

			//switch player turn
			isWhiteTurn = !isWhiteTurn;

			if (isWhiteTurn) {
				turnColorText1.GetComponent<Text>().text = "White";
				turnColorText2.GetComponent<Text>().text = "White";
			}
			else {
				turnColorText1.GetComponent<Text>().text = "Black";
				turnColorText2.GetComponent<Text>().text = "Black";
			}

			if (!CheckCanMove()) {
				if (IsStalemate()) {
					staleMate.SetActive(true);
				}
				else {
					EndGame();
				}
			}
		}

		selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
		selectedChessman = null;
		BoardHighlights.Instance.HideHighlights();
	}

	private void DeselectChessman() {
		selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
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
		blackWinsAnimation1.GetComponent<GAFMovieClip>().stop();
		blackWinsAnimation2.GetComponent<GAFMovieClip>().stop();
		whiteWinsAnimation1.GetComponent<GAFMovieClip>().stop();
		whiteWinsAnimation2.GetComponent<GAFMovieClip>().stop();
		blackWinsAnimation1.SetActive(false);
		blackWinsAnimation2.SetActive(false);
		whiteWinsAnimation1.SetActive(false);
		whiteWinsAnimation2.SetActive(false);

		turnColorText1.GetComponent<Text>().text = "White";
		turnColorText2.GetComponent<Text>().text = "White";

		flipBoardButton.SetActive(true);
		staleMate.SetActive(false);

		//list of all of the chessman prefabs
		activeChessman = new List<GameObject>();
		//the array of the board has it's size defined (takes in type Chessman)
		Chessmans = new Chessman[8, 8];
		EnPassantMove = new int[2] { -1, -1 };

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

	public void NewGame() {
		//if (isWhiteTurn) {
		//	Debug.Log("White team wins!");
		//} else {
		//	Debug.Log("Black team wins!");
		//}

		foreach (GameObject go in activeChessman) {
			Destroy(go);
		}

		isWhiteTurn = true;
		whitePromotion.SetActive(false);
		blackPromotion.SetActive(false);
		chessPlane.SetActive(true);
		BoardHighlights.Instance.HideHighlights();
		SpawnAllChessmans();
	}

	private void checkIllegalMoves(int x, int y) {
		//create a temp variable for selected chessman
		Chessman tempSelectedChessman = Chessmans[x, y];
		//temporarily remove selected piece from chessmans
		Chessmans[x, y] = null;
		//white king's position
		King whiteKing = GameObject.FindGameObjectWithTag("WhiteKing").GetComponent<King>();
		King blackKing = GameObject.FindGameObjectWithTag("BlackKing").GetComponent<King>();
		int whiteKingX = whiteKing.CurrentX;
		int whiteKingY = whiteKing.CurrentY;
		int blackKingX = blackKing.CurrentX;
		int blackKingY = blackKing.CurrentY;

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (allowedMoves[i, j]) {

					//reset x and y of king in case piece is not king (see next comment)
					whiteKingX = whiteKing.CurrentX;
					whiteKingY = whiteKing.CurrentY;
					blackKingX = blackKing.CurrentX;
					blackKingY = blackKing.CurrentY;
					//if the piece is a king, reset their x and y to the square they are moving to
					if (tempSelectedChessman.GetType() == typeof(King)) {
						if (isWhiteTurn) {
							whiteKingX = i;
							whiteKingY = j;
						}
						else {
							blackKingX = i;
							blackKingY = j;
						}
					}

						//Debug.Log("allowed move at: " + i + ", " + j);

						Chessman originalStateChessman = Chessmans[i, j];
					Chessmans[i, j] = tempSelectedChessman;

					//if where the piece is going to move has an enemy piece, temporarily remove that piece for the check

					foreach(GameObject c in activeChessman) {
						Chessman ch = c.GetComponent<Chessman>();
						//if the piece is not about to be taken
						if (ch.CurrentX == i && ch.CurrentY == j) {
							//the piece has been taken, so don't check if it can eat the king
						}
						else {
							//white turn
							if (isWhiteTurn) {
								if (!ch.isWhite) {
									bool[,] enemyPossibleMoves = ch.PossibleMove();
									if (enemyPossibleMoves[whiteKingX, whiteKingY]) {
										allowedMoves[i, j] = false;
									}
								}
							}
							//black turn
							else {
								if (ch.isWhite) {
									bool[,] enemyPossibleMoves = ch.PossibleMove();
									if (enemyPossibleMoves[blackKingX, blackKingY]) {
										allowedMoves[i, j] = false;
									}
								}
							}
						}
					}

					Chessmans[i, j] = originalStateChessman;

				}
			}
		}

		//return removed chessman to it's rightful spot
		Chessmans[x, y] = tempSelectedChessman;
	}

	private bool CheckCanMove() {
		bool canMove = false;

		foreach(GameObject go in activeChessman) {
			Chessman chessmanPiece = go.GetComponent<Chessman>();
			if (isWhiteTurn && chessmanPiece.isWhite) {
				allowedMoves = chessmanPiece.PossibleMove();
				checkIllegalMoves(chessmanPiece.CurrentX, chessmanPiece.CurrentY);
				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						if (allowedMoves[i,j]) {
							canMove = true;
						}
					}
				}
			} else if (!isWhiteTurn && !chessmanPiece.isWhite) {
				allowedMoves = chessmanPiece.PossibleMove();
				checkIllegalMoves(chessmanPiece.CurrentX, chessmanPiece.CurrentY);
				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						if (allowedMoves[i, j]) {
							canMove = true;
						}
					}
				}
			}
		}

		return canMove;
	}

	private bool IsStalemate() {
		bool isStalemate = true;

		if (isWhiteTurn) {
			King whiteKing = GameObject.FindGameObjectWithTag("WhiteKing").GetComponent<King>();
			int whiteKingX = whiteKing.CurrentX;
			int whiteKingY = whiteKing.CurrentY;
			foreach (GameObject go in activeChessman) {
				Chessman chessPiece = go.GetComponent<Chessman>();
				if (!chessPiece.isWhite) {
					allowedMoves = chessPiece.PossibleMove();
					checkIllegalMoves(chessPiece.CurrentX, chessPiece.CurrentY);
					if (allowedMoves[whiteKingX, whiteKingY]) {
						isStalemate = false;
					}
				}
			}
		}
		else {
			King blackKing = GameObject.FindGameObjectWithTag("BlackKing").GetComponent<King>();
			int blackKingX = blackKing.CurrentX;
			int blackKingY = blackKing.CurrentY;
			foreach (GameObject go in activeChessman) {
				Chessman chessPiece = go.GetComponent<Chessman>();
				if (chessPiece.isWhite) {
					allowedMoves = chessPiece.PossibleMove();
					checkIllegalMoves(chessPiece.CurrentX, chessPiece.CurrentY);
					if (allowedMoves[blackKingX, blackKingY]) {
						isStalemate = false;
					}
				}
			}
		}

		return isStalemate;
	}

	public bool ThreatenedSpace(int x, int y) {

	bool threatened = false;
	bool[,] possibleMovesList = new bool[8,8];

		foreach (GameObject chessman in activeChessman) {
			Chessman ch = chessman.GetComponent<Chessman>();
			if (ch.GetType() != typeof(King)) {
				if (ch.isWhite != isWhiteTurn) {
					possibleMovesList = ch.PossibleMove();
					if (possibleMovesList[x, y]) {
						threatened = true;
					}
				}
			}
		}
		return threatened;
	}

	public void Promote(string pieceType) {
		int x = promotionPiece.CurrentX;
		int y = promotionPiece.CurrentY;
		activeChessman.Remove(promotionPiece.gameObject);
		Destroy(promotionPiece.gameObject);
		//replace with new piece
		//white
		if (promotionPiece.isWhite) {
			if (pieceType == "Queen") {
				SpawnChessman(1, x, y);
			}
			else if (pieceType == "Rook") {
				SpawnChessman(2, x, y);
			}
			else if (pieceType == "Bishop") {
				SpawnChessman(3, x, y);
			}
			else if (pieceType == "Knight") {
				SpawnChessman(4, x, y);
			}
		}
		//black
		else {
			if (pieceType == "Queen") {
				SpawnChessman(7, x, y);
			}
			else if (pieceType == "Rook") {
				SpawnChessman(8, x, y);
			}
			else if (pieceType == "Bishop") {
				SpawnChessman(9, x, y);
			}
			else if (pieceType == "Knight") {
				SpawnChessman(10, x, y);
			}
		}
		
		//selectedChessman = Chessmans[x, y];
		chessPlane.SetActive(true);
		whitePromotion.SetActive(false);
		blackPromotion.SetActive(false);
	}

	private void EndGame() {
		flipBoardButton.SetActive(false);
		Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
		if (!isWhiteTurn) {
			if (camera.enabled) {
				whiteWinsAnimation1.SetActive(true);
				whiteWinsAnimation1.GetComponent<GAFMovieClip>().play();
			} else {
				whiteWinsAnimation2.SetActive(true);
				whiteWinsAnimation2.GetComponent<GAFMovieClip>().play();
			}
		} else {
			if (camera.enabled) {
				blackWinsAnimation1.SetActive(true);
				blackWinsAnimation1.GetComponent<GAFMovieClip>().play();
			}
			else {
				blackWinsAnimation2.SetActive(true);
				blackWinsAnimation2.GetComponent<GAFMovieClip>().play();
			}
		}
	}
}
