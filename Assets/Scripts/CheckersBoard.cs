using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour
{
   public Piece[,] pieces = new Piece[8, 8];
   public GameObject whitePiecePrefab;
   public GameObject blackPiecePrefab;

   public  bool isWhite;
   private bool isWhiteTurn = true;
   private bool hasKilled;

   private Piece selectedPiece;
   private List<Piece> forcedPieces;

   private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
   private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

   private Vector2 mouseOver;
   private Vector2 startDrag;
   private Vector2 endDrag;


    private void Start()
    {
        isWhiteTurn = true;
        forcedPieces = new List<Piece>();
        GenerateBoard();
    }

    private void Update()
    {
        UpdateMouseOver();

        {
            //if it's our turn
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;
            
            if (selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }

            if(Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }
            if(Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
        //  Debug.Log(mouseOver);
    }

    private void UpdateMouseOver()
    {
        //if it's our turn
        if(!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    
    private void UpdatePieceDrag(Piece p)
    {
        if(!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }
    
    private void SelectPiece(int x, int y)
    {
        //Out of bounds
        if(x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }

        Piece p = pieces[x, y];
        if(p != null && p.isWhite == isWhiteTurn)
        {
            if(forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                //Look for the piece under our forced pieces list
                if(forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForPossibleMove();

        //Multiplayer Support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];


        MovePiece(selectedPiece, x2, y2);
        //Out of bounds
        if(x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8)
        {
        if(selectedPiece != null)
        {
            MovePiece(selectedPiece, x1, y1);
        }    
          startDrag = Vector2.zero;
          selectedPiece = null;
          return;
        }
        if (selectedPiece != null)
        {
            //if the piece hasn't moved
            if (endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            //Check if the move is valid
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                //Check if it's a jump
                //Check if we are capturing an opposing piece
                if(Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                //Check if the piece was supposed to kill
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

               
                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();

            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        
    }
    }

    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        //Promote piece to king
        if (selectedPiece != null)
        {
            if(selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
            else if(!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }

        startDrag = Vector2.zero;
        selectedPiece = null;

        if(ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
        {
            return;
        }

        isWhiteTurn = !isWhiteTurn;
        hasKilled = false;
        CheckVictory();
    }

    private void CheckVictory()
    {
        //  
    }

    private List<Piece> ScanForPossibleMove(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();

        if(pieces[x,y].IsForceToMove(pieces, x, y))
        {
            forcedPieces.Add(pieces[x,y]);
        }

        return forcedPieces;
    }
    private List<Piece> ScanForPossibleMove()
    {
        forcedPieces = new List<Piece>();
        //Loop through all the pieces
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(pieces[i,j] != null && pieces[i,j].isWhite == isWhiteTurn)
                {
                    //If it has at least one valid move
                    if(pieces[i,j].IsForceToMove(pieces, i, j))
                    {
                        //Add it to the forced list
                        forcedPieces.Add(pieces[i,j]);
                    }
                }
            }
        }
        return forcedPieces;
    }

    private void GenerateBoard()
    {
        // Generate the white team
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);
            for (int x=0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        // Generate the black team
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

    
    }

    private void GeneratePiece(int x, int y)
    {
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
                
       
}
