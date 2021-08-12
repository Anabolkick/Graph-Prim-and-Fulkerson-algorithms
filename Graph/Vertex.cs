namespace Graph
{
    class Vertex
    {
        public int Number { get; set; }
        public bool Connected { get; set; }
        public bool Visited { get; set; }

        public Vertex(int number, bool connected = false)
        {
            Number = number;
            Connected = connected;
        }

        public override string ToString()
        {
            return Number.ToString();
        }

    }
}