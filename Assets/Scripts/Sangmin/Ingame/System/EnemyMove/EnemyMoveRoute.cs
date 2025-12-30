using System.Collections.Generic;
using UnityEngine;

namespace Sangmin
{
    /// <summary>
    /// Grid의 활성 셀들(isCellActive)을 기반으로 테두리 경로를 자동 생성하는 유틸리티.
    /// 왼쪽 위에서 시작해서 반시계 방향으로 활성 셀들의 바깥 경계를 따라 이동하는 경로를 만듭니다.
    /// </summary>
    public static class EnemyMoveRoute
    {
        /// <summary>
        /// GridUnitPlacement의 현재 활성 셀 상태를 기반으로 테두리 경로를 생성합니다.
        /// </summary>
        /// <returns>월드 좌표 경로 (Vector3 배열). 경로가 없으면 빈 배열.</returns>
        public static Vector3[] GenerateBoundaryRoute()
        {
            if (GridUnitPlacement.Instance == null)
            {
                Debug.LogWarning("GridUnitPlacement.Instance가 없습니다.");
                return new Vector3[0];
            }

            var grid = GridUnitPlacement.Instance;
            var cellInfos = GetCellInfos(grid);

            if (cellInfos == null)
                return new Vector3[0];

            // 활성 셀들의 경계 경로를 찾기
            List<Vector2Int> boundaryPath = FindBoundaryPath(cellInfos, grid.gridHeight, grid.gridWidth);

            if (boundaryPath.Count == 0)
                return new Vector3[0];

            // 그리드 좌표를 월드 좌표로 변환
            return ConvertToWorldPositions(boundaryPath, grid);
        }

        /// <summary>
        /// 활성 셀들의 테두리를 찾아서 반시계 방향 경로를 생성합니다.
        /// </summary>
        private static List<Vector2Int> FindBoundaryPath(bool[,] cellInfos, int height, int width)
        {
            var path = new List<Vector2Int>();

            // 1. 왼쪽 위에서 시작하는 경계점 찾기
            Vector2Int? startPoint = FindTopLeftBoundaryPoint(cellInfos, height, width);

            if (!startPoint.HasValue)
                return path; // 활성 셀이 없거나 경계를 찾을 수 없음

            // 2. 경계를 따라 반시계 방향으로 추적
            TraceBoundary(cellInfos, height, width, startPoint.Value, path);

            return path;
        }

        /// <summary>
        /// 왼쪽 위에서 시작하는 경계점을 찾습니다.
        /// 활성 셀의 왼쪽 위 모서리를 찾습니다.
        /// </summary>
        private static Vector2Int? FindTopLeftBoundaryPoint(bool[,] cellInfos, int height, int width)
        {
            // 위에서 아래로, 왼쪽에서 오른쪽으로 스캔
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!cellInfos[row, col])
                        continue; // 비활성 셀은 건너뛰기

                    // 이 셀의 왼쪽 위 모서리가 경계인지 확인
                    // 왼쪽이나 위쪽이 비활성 셀이거나 맵 밖이면 경계
                    bool leftIsBoundary = col == 0 || !cellInfos[row, col - 1];
                    bool topIsBoundary = row == 0 || !cellInfos[row - 1, col];

                    if (leftIsBoundary || topIsBoundary)
                    {
                        // 셀의 왼쪽 위 모서리 좌표 반환
                        // 셀 중심이 (row, col)이면, 왼쪽 위 모서리는 (col - 0.5, row + 0.5)
                        // 하지만 정수 좌표로 반환하기 위해 셀 중심 좌표 사용
                        return new Vector2Int(col, row);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 경계를 따라 반시계 방향으로 추적합니다.
        /// </summary>
        private static void TraceBoundary(bool[,] cellInfos, int height, int width, Vector2Int start, List<Vector2Int> path)
        {
            var visited = new HashSet<Vector2Int>();
            Vector2Int current = start;
            Vector2Int direction = Vector2Int.right; // 처음에는 오른쪽으로 시작 (반시계 방향)

            // 반시계 방향: 오른쪽(0), 아래(1), 왼쪽(2), 위(3)
            Vector2Int[] directions = {
                Vector2Int.right,   // 0: 오른쪽
                Vector2Int.down,     // 1: 아래
                Vector2Int.left,     // 2: 왼쪽
                Vector2Int.up        // 3: 위
            };

            int maxIterations = height * width * 4; // 무한 루프 방지
            int iterations = 0;

            path.Add(current);

            while (iterations < maxIterations)
            {
                iterations++;

                // 현재 방향 기준으로 왼쪽, 직진, 오른쪽, 뒤 순서로 다음 경계점 찾기
                bool foundNext = false;

                // 왼쪽부터 시작 (반시계 방향 유지)
                for (int offset = -1; offset <= 2; offset++)
                {
                    int dirIndex = (GetDirectionIndex(direction, directions) + offset + 4) % 4;
                    Vector2Int nextDir = directions[dirIndex];
                    Vector2Int next = current + nextDir;

                    // 다음 위치가 경계인지 확인
                    if (IsBoundaryPoint(cellInfos, height, width, current, next))
                    {
                        // 시작점으로 돌아왔는지 확인
                        if (next.Equals(start) && path.Count > 2)
                        {
                            path.Add(next); // 시작점 추가하고 종료
                            return;
                        }

                        // 이미 방문한 점이 아니면 추가
                        if (!visited.Contains(next))
                        {
                            path.Add(next);
                            visited.Add(next);
                            current = next;
                            direction = nextDir;
                            foundNext = true;
                            break;
                        }
                    }
                }

                if (!foundNext)
                {
                    // 더 이상 진행할 수 없으면 종료
                    break;
                }
            }
        }

        /// <summary>
        /// 두 점 사이가 경계인지 확인합니다.
        /// </summary>
        private static bool IsBoundaryPoint(bool[,] cellInfos, int height, int width, Vector2Int from, Vector2Int to)
        {
            // 범위 체크
            if (to.x < 0 || to.x >= width || to.y < 0 || to.y >= height)
                return false;

            int fromRow = from.y;
            int fromCol = from.x;
            int toRow = to.y;
            int toCol = to.x;

            // from이 활성 셀이고, to가 비활성 셀이거나 맵 밖이면 경계
            bool fromActive = cellInfos[fromRow, fromCol];
            bool toActive = (toRow >= 0 && toRow < height && toCol >= 0 && toCol < width)
                ? cellInfos[toRow, toCol] : false;

            // 활성 셀에서 비활성 셀로 가는 경계
            if (fromActive && !toActive)
                return true;

            // 비활성 셀에서 활성 셀로 가는 경계 (반대 방향)
            if (!fromActive && toActive)
                return true;

            return false;
        }

        /// <summary>
        /// 방향 벡터의 인덱스를 찾습니다.
        /// </summary>
        private static int GetDirectionIndex(Vector2Int dir, Vector2Int[] directions)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                if (directions[i].Equals(dir))
                    return i;
            }
            return 0;
        }

        /// <summary>
        /// GridUnitPlacement에서 활성 셀 정보를 가져옵니다.
        /// </summary>
        private static bool[,] GetCellInfos(GridUnitPlacement grid)
        {
            // 리플렉션을 사용하여 private 필드에 접근
            var field = typeof(GridUnitPlacement).GetField("cellInfos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError("cellInfos 필드를 찾을 수 없습니다.");
                return null;
            }

            var cellInfos = field.GetValue(grid) as UnitCell[,];

            if (cellInfos == null)
                return null;

            bool[,] activeCells = new bool[grid.gridHeight, grid.gridWidth];

            for (int row = 0; row < grid.gridHeight; row++)
            {
                for (int col = 0; col < grid.gridWidth; col++)
                {
                    if (cellInfos[row, col] != null)
                    {
                        activeCells[row, col] = cellInfos[row, col].isCellActive;
                    }
                }
            }

            return activeCells;
        }

        /// <summary>
        /// 그리드 좌표 경로를 월드 좌표로 변환합니다.
        /// UnitCell의 실제 transform.position을 사용합니다.
        /// </summary>
        private static Vector3[] ConvertToWorldPositions(List<Vector2Int> gridPath, GridUnitPlacement grid)
        {
            // cellInfos 가져오기
            var field = typeof(GridUnitPlacement).GetField("cellInfos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogError("cellInfos 필드를 찾을 수 없습니다.");
                return new Vector3[0];
            }

            var cellInfos = field.GetValue(grid) as UnitCell[,];

            if (cellInfos == null)
                return new Vector3[0];

            Vector3[] worldPath = new Vector3[gridPath.Count];

            for (int i = 0; i < gridPath.Count; i++)
            {
                Vector2Int gridPos = gridPath[i];
                int row = gridPos.y;
                int col = gridPos.x;

                // 범위 체크
                if (row >= 0 && row < grid.gridHeight && col >= 0 && col < grid.gridWidth)
                {
                    if (cellInfos[row, col] != null)
                    {
                        // UnitCell의 실제 위치 사용
                        worldPath[i] = cellInfos[row, col].transform.position;
                    }
                    else
                    {
                        // 셀이 없으면 계산된 위치 사용
                        worldPath[i] = CalculateWorldPosition(row, col, grid);
                    }
                }
                else
                {
                    // 맵 밖이면 계산된 위치 사용
                    worldPath[i] = CalculateWorldPosition(row, col, grid);
                }
            }

            return worldPath;
        }

        /// <summary>
        /// row, col을 월드 좌표로 변환합니다 (셀이 없을 때 사용).
        /// </summary>
        private static Vector3 CalculateWorldPosition(int row, int col, GridUnitPlacement grid)
        {
            // 첫 번째 셀의 위치를 기준으로 계산
            var field = typeof(GridUnitPlacement).GetField("cellInfos",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                var cellInfos = field.GetValue(grid) as UnitCell[,];
                if (cellInfos != null && cellInfos[0, 0] != null)
                {
                    Vector3 firstCellPos = cellInfos[0, 0].transform.position;
                    float offsetX = (col - 0) * grid.cellSize;
                    float offsetY = (0 - row) * grid.cellSize; // row가 증가하면 y는 감소
                    return firstCellPos + new Vector3(offsetX, offsetY, 0f);
                }
            }

            // 기본 계산 (중심 기준)
            float worldX = (col - grid.gridWidth * 0.5f + 0.5f) * grid.cellSize;
            float worldY = (grid.gridHeight * 0.5f - row - 0.5f) * grid.cellSize;
            return new Vector3(worldX, worldY, 0f);
        }
    }
}
