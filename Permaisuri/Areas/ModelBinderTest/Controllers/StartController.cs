using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;

namespace Permaisuri.Areas.ModelBinderTest.Controllers
{
    public class StartController : Controller
    {
        //
        // GET: /ModelBinderTest/Start/

        public ActionResult Index()
        {
            var persons = new Persons
            {
                Data = new List<Person>
                {
                    new Person {Name = "张全蛋", Age = 20, Slogan = "简直so no face"},
                    new Person {Name = "刘斌", Age = 25, Slogan = "女神痘痘多"},
                    new Person {Name = "郑和如", Age = 20, Slogan = "2014年12月2日"}
                }
            };
            return View(persons);
        }

        public class Persons
        {
            public List<Person> Data { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Slogan { get; set; }
        }



        public ActionResult DoQuery(IEnumerable<Persons> Data)
        {
            foreach (var item in Data)
            {
                var aaa = item;
            }
            return null;
        }


        public ActionResult DoQuedy2(IList<Person> persons)
        {
            persons = persons ?? new List<Person>();
            return View(persons);
        }


        public ActionResult Names(IList<String> names)
        {
            names = names ?? new List<string>();
            return View(names);
        }

    }
}
