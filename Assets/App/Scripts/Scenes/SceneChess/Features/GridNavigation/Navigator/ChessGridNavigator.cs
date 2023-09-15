﻿using System;
using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Piece;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private ChessGrid _grid;

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            _grid = grid;

            List<Vector2Int> openedList = new List<Vector2Int>();
            HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> ortogonalValues = new Dictionary<Vector2Int, float>();
            Dictionary<Vector2Int, float> heuristicValues = new Dictionary<Vector2Int, float>();

            openedList.Add(from);

            ortogonalValues[from] = 0;
            heuristicValues[from] = 0;

            foreach (ChessUnit piece in _grid.Pieces)
            {
                closedList.Add(piece.CellPosition);
            }

            while (openedList.Count > 0)
            {
                Vector2Int currentCell = DefineLowestHeuristicValue(openedList, heuristicValues);

                if (currentCell == to)
                    return CreatePath(cameFrom, currentCell);

                openedList.Remove(currentCell);
                closedList.Add(currentCell);

                foreach (Vector2Int neighbor in GetValidMovesForCurrentUnit(unit, currentCell).Invoke())
                {
                    if (closedList.Contains(neighbor))
                        continue;

                    float tempOrtogonalValue = ortogonalValues[currentCell] + 1;

                    if (openedList.Contains(neighbor) == false || tempOrtogonalValue < ortogonalValues[neighbor])
                    {
                        cameFrom[neighbor] = currentCell;
                        ortogonalValues[neighbor] = tempOrtogonalValue;
                        heuristicValues[neighbor] = ortogonalValues[neighbor];

                        if (openedList.Contains(neighbor) == false)
                            openedList.Add(neighbor);
                    }
                }
            }

            return null;
        }

        private Vector2Int DefineLowestHeuristicValue(List<Vector2Int> openedList, Dictionary<Vector2Int, float> heuristicValues)
        {
            Vector2Int currentLowestCell = openedList[0];

            foreach (Vector2Int openedCell in openedList)
            {
                if (heuristicValues.ContainsKey(openedCell) && heuristicValues[openedCell] < heuristicValues[currentLowestCell])
                    currentLowestCell = openedCell;
            }

            return currentLowestCell;
        }

        private List<Vector2Int> CreatePath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int currentCell)
        {
            List<Vector2Int> path = new List<Vector2Int> { currentCell };

            while (cameFrom.ContainsKey(currentCell))
            {
                currentCell = cameFrom[currentCell];
                path.Insert(0, currentCell);
            }

            path.RemoveAt(0);
            return path;
        }

        private Func<List<Vector2Int>> GetValidMovesForCurrentUnit(ChessUnitType unit, Vector2Int position)
        {
            Dictionary<ChessUnitType, Func<List<Vector2Int>>> availableValues = new Dictionary<ChessUnitType, Func<List<Vector2Int>>>()
            {
                { ChessUnitType.Pawn,  () => GetValidMovesForPawn(position) },
                { ChessUnitType.Bishop, () => GetValidMovesForBishop(position) },
                { ChessUnitType.Rook, () => GetValidMovesForRook(position) },
                { ChessUnitType.Knight, () => GetValidMovesForKnight(position) },
                { ChessUnitType.King, () => GetValidMovesForKing(position) },
                { ChessUnitType.Queen, () => GetValidMovesForQueen(position) },
            };

            return availableValues[unit];
        }

        private List<Vector2Int> GetValidMovesForPawn(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>
            {
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1)
            };

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesForKing(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>
            {
                new Vector2Int(position.x - 1, position.y - 1),
                new Vector2Int(position.x, position.y - 1),
                new Vector2Int(position.x + 1, position.y - 1),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y + 1),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x + 1, position.y + 1)
            };

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesForQueen(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>();

            availableMoves.AddRange(GetValidMovesInDirection(position, -1, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 0, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, -1, 0));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, 0));
            availableMoves.AddRange(GetValidMovesInDirection(position, -1, 1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 0, 1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, 1));

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesForRook(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>();

            availableMoves.AddRange(GetValidMovesInDirection(position, 0, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, -1, 0));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, 0));
            availableMoves.AddRange(GetValidMovesInDirection(position, 0, 1));

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesForKnight(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>
            {
                new Vector2Int(position.x - 1, position.y - 2),
                new Vector2Int(position.x + 1, position.y - 2),
                new Vector2Int(position.x - 2, position.y - 1),
                new Vector2Int(position.x + 2, position.y - 1),
                new Vector2Int(position.x - 2, position.y + 1),
                new Vector2Int(position.x + 2, position.y + 1),
                new Vector2Int(position.x - 1, position.y + 2),
                new Vector2Int(position.x + 1, position.y + 2)
            };

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesForBishop(Vector2Int position)
        {
            List<Vector2Int> availableMoves = new List<Vector2Int>();

            availableMoves.AddRange(GetValidMovesInDirection(position, -1, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, -1));
            availableMoves.AddRange(GetValidMovesInDirection(position, -1, 1));
            availableMoves.AddRange(GetValidMovesInDirection(position, 1, 1));

            return availableMoves;
        }

        private List<Vector2Int> GetValidMovesInDirection(Vector2Int position, int xDirection, int yDirection)
        {
            List<Vector2Int> validMoves = new List<Vector2Int>();
            Vector2Int current = position;
            bool isOutOfBounds;

            while (true)
            {
                isOutOfBounds = current.x < 0 || current.x >= _grid.Size.x || current.y < 0 || current.y >= _grid.Size.y;
                current += new Vector2Int(xDirection, yDirection);


                if (isOutOfBounds || _grid.Get(current) != null)
                    break;

                validMoves.Add(current);
            }

            return validMoves;
        }
    }
}