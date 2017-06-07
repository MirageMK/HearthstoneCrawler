using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HSCore;
using HSCore.Model;

namespace HSWebApi.Controllers
{
    public class DecksController : ApiController
    {
        // GET api/cards
        public IEnumerable<String> Get()
        {
            return NetDecks.Decks.Where(x => x.MyDust == 0).GroupBy(x => x.Class).Select(x => x.OrderBy(y => y.Tier).ThenByDescending(y => y.Dust).First().DeckCode).ToList();
        }

        // GET api/values/5
        public IEnumerable<Deck> Get(int id)
        {
            return NetDecks.DownloadDecks();
        }
    }
}