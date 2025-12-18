using System.Collections.Generic;

namespace Sangmin
{
    /// <summary>
    /// Union-Find (Disjoint Set) 자료구조를 사용하여 연결된 컴포넌트를 찾는 클래스
    /// 더 효율적인 방법이며, 정점이 동적으로 추가/제거되는 경우 유용합니다
    /// </summary>
    public class UnionFind
    {
        private Dictionary<int, int> parent;
        private Dictionary<int, int> rank;
        
        public UnionFind()
        {
            parent = new Dictionary<int, int>();
            rank = new Dictionary<int, int>();
        }
        
        /// <summary>
        /// 정점이 없으면 추가하고, 있으면 아무것도 하지 않습니다
        /// </summary>
        public void MakeSet(int x)
        {
            if (!parent.ContainsKey(x))
            {
                parent[x] = x;
                rank[x] = 0;
            }
        }
        
        /// <summary>
        /// 경로 압축을 사용한 Find 연산
        /// </summary>
        public int Find(int x)
        {
            MakeSet(x);
            
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]); // 경로 압축
            }
            
            return parent[x];
        }
        
        /// <summary>
        /// Union by Rank를 사용한 Union 연산
        /// </summary>
        public void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            
            if (rootX == rootY)
                return; // 이미 같은 집합에 있음
            
            // Rank를 사용하여 효율적으로 합치기
            if (rank[rootX] < rank[rootY])
            {
                parent[rootX] = rootY;
            }
            else if (rank[rootX] > rank[rootY])
            {
                parent[rootY] = rootX;
            }
            else
            {
                parent[rootY] = rootX;
                rank[rootX]++;
            }
        }
        
        /// <summary>
        /// 두 정점이 같은 컴포넌트에 속하는지 확인
        /// </summary>
        public bool AreConnected(int x, int y)
        {
            return Find(x) == Find(y);
        }
        
        /// <summary>
        /// 방향 그래프에서 마주보는 관계를 기반으로 Union 수행
        /// </summary>
        public void BuildComponentsFromDirectedGraph(Dictionary<int, HashSet<int>> directedGraph)
        {
            // 모든 정점을 추가
            foreach (var vertex in directedGraph.Keys)
            {
                MakeSet(vertex);
            }
            
            // 양방향 간선(마주보는 관계)을 찾아서 Union 수행
            foreach (var kvp in directedGraph)
            {
                int from = kvp.Key;
                foreach (var to in kvp.Value)
                {
                    // 역방향 간선도 있는지 확인
                    if (directedGraph.ContainsKey(to) && 
                        directedGraph[to].Contains(from))
                    {
                        Union(from, to);
                    }
                }
            }
        }
        
        /// <summary>
        /// 모든 연결된 컴포넌트를 그룹으로 반환
        /// </summary>
        public Dictionary<int, List<int>> GetAllComponents()
        {
            Dictionary<int, List<int>> components = new Dictionary<int, List<int>>();

            // 순회 도중에 parent 딕셔너리가 수정될 수 있으므로
            // 키 목록을 미리 복사해서 사용한다.
            List<int> keysSnapshot = new List<int>(parent.Keys);
            
            foreach (var vertex in keysSnapshot)
            {
                int root = Find(vertex);
                if (!components.ContainsKey(root))
                {
                    components[root] = new List<int>();
                }
                components[root].Add(vertex);
            }
            
            return components;
        }
        
        /// <summary>
        /// 특정 정점이 속한 컴포넌트의 모든 정점 반환
        /// </summary>
        public List<int> GetComponent(int vertex)
        {
            if (!parent.ContainsKey(vertex))
                return new List<int> { vertex };
            
            int root = Find(vertex);
            List<int> component = new List<int>();

            // 마찬가지로 키 스냅샷을 사용해서 순회 중 수정 문제를 방지한다.
            List<int> keysSnapshot = new List<int>(parent.Keys);
            
            foreach (var v in keysSnapshot)
            {
                if (Find(v) == root)
                {
                    component.Add(v);
                }
            }
            
            return component;
        }
    }
}

