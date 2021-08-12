namespace Graph
{
    class Edge
    {
        public Vertex From { get; set; }
        public Vertex To { get; set; }
        public int Weight { get; set; }
        public bool Used { get; set; }
        public Edge(Vertex from, Vertex to, int weight = 1, bool used = false)
        {
            Used = used;
            From = from;
            To = to;
            Weight = weight;
        }
        public override string ToString()
        {
            return $"({From}; {To})";
        }

    }
}