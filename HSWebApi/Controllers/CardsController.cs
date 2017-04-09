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
    public class CardsController : ApiController
    {
        // GET api/cards
        public IEnumerable<Card> Get()
        {
            return MyCollection.Cards;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }
    }
}
