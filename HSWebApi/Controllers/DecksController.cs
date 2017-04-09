using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HSCore;
using HSCore.Model;

namespace HSWebApi.Controllers
{
    public class DecksController : ApiController
    {
        // GET api/cards
        public IEnumerable<Deck> Get()
        {
            return NetDecks.Decks;
        }

        // GET api/values/5
        public IEnumerable<Deck> Get(int id)
        {
            return NetDecks.DownloadDecks();
        }
    }
}
