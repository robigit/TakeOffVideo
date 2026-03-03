using System;

namespace TakeOffVideo.Library.Global
{
    public class Atleta
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public string? Pettorale { get; set; }
        public string? Societa { get; set; }
        public string? Categoria { get; set; }
        
        public override string ToString()
        {
            return $"{Pettorale} - {Cognome} {Nome}".Trim(' ', '-');
        }

        public int PettNum
        {
            get
            {
                if(int.TryParse(Pettorale, out int p))
                    return p;
                return 0 ;
            }
        }
    }
}
