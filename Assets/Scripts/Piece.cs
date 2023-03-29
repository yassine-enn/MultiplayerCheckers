using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        if (isWhite || isKing)
        {
            //Top Left
            if(x>=2 && y<=5)
            {
                Piece p = board[x-1, y+1];
                //Check if there is a piece to jump over
                if(p != null && p.isWhite != isWhite)
                {   
                    //Check if there is a space to land
                    if(board[x-2, y+2] == null)
                    {
                        return true;
                    }
                }
            }
            //Top Right
            if(x<=5 && y<=5)
            {
                Piece p = board[x+1, y+1];
                //Check if there is a piece to jump over
                if(p != null && p.isWhite != isWhite)
                {   
                    //Check if there is a space to land
                    if(board[x+2, y+2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if(!isWhite || isKing){
            //Bottom Left
            if(x>=2 && y>=2)
            {
                Piece p = board[x-1, y-1];
                //Check if there is a piece to jump over
                if(p != null && p.isWhite != isWhite)
                {   
                    //Check if there is a space to land
                    if(board[x-2, y-2] == null)
                    {
                        return true;
                    }
                }
            }
            //Bottom Right
            if(x<=5 && y>=2)
            {
                Piece p = board[x+1, y-1];
                //Check if there is a piece to jump over
                if(p != null && p.isWhite != isWhite)
                {   
                    //Check if there is a space to land
                    if(board[x+2, y-2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2)
    {
        //Check if moving on top of another piece
        if (board[x2, y2] != null)
        {
            return false;
        }

        int deltaMoveX = (int)Mathf.Abs(x1 - x2);
        int deltaMoveY = y2-y1;

        //White team
        if(isWhite || isKing)
        {
           if (deltaMoveX == 1)
           {
               if (deltaMoveY == 1)
               {
                   return true;
               }
           }
           else if(deltaMoveX == 2)
           {
               if(deltaMoveY == 2)
               {
                   Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                   if(p != null && p.isWhite != isWhite)
                   {
                       return true;
                   }
               }

           }
        }

        //Black team
        if(!isWhite || isKing)
        {
           if (deltaMoveX == 1)
           {
               if (deltaMoveY == -1)
               {
                   return true;
               }
           }
           else if(deltaMoveX == 2)
           {
               if(deltaMoveY == -2)
               {
                   Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null && p.isWhite != isWhite)
                     {
                          return true;
                     }
               }

           }
        }
        return false;
    }
}
