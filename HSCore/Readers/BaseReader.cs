using System.Collections.Generic;
using HSCore.Model;

namespace HSCore.Readers
{
    public abstract class BaseReader
    {
        public abstract List<Deck> GetDecks();
    }
}
